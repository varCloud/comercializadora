/*
Objetivo	SPs de la HH (via servicios web) para Proceso Produccion por formula. CREATE OR ALTER => idempotente.
			Primer result set = fila de estatus (status/estatus 200 = ok, -1 = error, mensaje).

			SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA
				set 2: productos linea 20 con formula activa; set 3: sus materias primas (incluye idProcesoProduccionFormula).
				status -1 si no hay resultados (sin sets adicionales).
			SP_APP_AGREGAR_PROCESO_PRODUCCION
				status 200 => set 2 = idProcesoProduccion.
				status -1  => set 2 = lista de faltantes (idProducto, descripcion, requerido, disponible, unidadMedida);
							  vacio si el error no es por inventario (validacion, inventario fisico, error inesperado).
			SP_APP_OBTENER_PROCESO_PRODUCCION_HISTORIAL
				set 2: historial del usuario (status -1 si vacio).

Decisiones de SP_APP_AGREGAR_PROCESO_PRODUCCION (corrigen las fallas del modelo Envasado):
 - Descuento de MPL: ubicaciones del almacen con piso < 1000 (excluye resguardo 1002; incluye 0-0-0 porque la validacion
   dbo.ExisteCantidadProductoEnAlmancen tambien la cuenta), de mayor a menor cantidad, repartiendo el remanente entre
   ubicaciones; si aun asi falta existencia dentro de la transaccion se aborta todo (rollback).
 - Sin PRINTs de depuracion; ROLLBACK solo si hay transaccion; SET XACT_ABORT ON.
 - cantidadProducida y cantidadConsumida se calculan una sola vez con ROUND(x, 3) (3 decimales, 1 gramo en kg) y de esas variables
   salen ProcesoProduccion/Detalle y los movimientos de inventario.
 - Si la cantidad consumida de una MPL redondea a 0 (< 0.001) se rechaza: aumentar lotes.
*/

-- VERIFICACION PREVIA
select name from sys.procedures where name in ('SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA','SP_APP_AGREGAR_PROCESO_PRODUCCION','SP_APP_OBTENER_PROCESO_PRODUCCION_HISTORIAL')
GO

-- Necesario para indices filtrados (sqlcmd los trae en OFF por defecto)
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA]
@idUsuario int,
@descripcion varchar(200) = null,
@codigoBarras varchar(50) = null
AS
BEGIN
	SET NOCOUNT ON
	SELECT @descripcion = LTRIM(RTRIM(coalesce(@descripcion, ''))), @codigoBarras = LTRIM(RTRIM(coalesce(@codigoBarras, '')))

	SELECT P.idProducto, P.codigoBarras, P.descripcion, P.idUnidadMedida, U.descripcion unidadMedida,
		   F.idProcesoProduccionFormula, F.cantidadLote
	INTO #productos
	FROM ProcesoProduccionFormula F
	JOIN Productos P ON P.idProducto = F.idProducto
	JOIN CatUnidadMedida U ON U.idUnidadMedida = P.idUnidadMedida
	WHERE F.activo = 1 AND P.activo = 1
	  AND (@descripcion = '' OR P.descripcion LIKE '%' + @descripcion + '%')
	  AND (@codigoBarras = '' OR P.codigoBarras = @codigoBarras)

	IF NOT EXISTS (SELECT 1 FROM #productos)
	BEGIN
		SELECT -1 status, -1 estatus, 'No se encontraron productos con formula de produccion' mensaje
		RETURN
	END

	SELECT 200 status, 200 estatus, 'Consulta correcta' mensaje

	SELECT idProducto, codigoBarras, descripcion, idUnidadMedida, unidadMedida, idProcesoProduccionFormula, cantidadLote
	FROM #productos ORDER BY descripcion

	SELECT D.idProcesoProduccionFormula, D.idProducto, P.descripcion, D.idUnidadMedida, U.descripcion unidadMedida, D.cantidad
	FROM ProcesoProduccionFormulaDetalle D
	JOIN #productos X ON X.idProcesoProduccionFormula = D.idProcesoProduccionFormula
	JOIN Productos P ON P.idProducto = D.idProducto
	JOIN CatUnidadMedida U ON U.idUnidadMedida = D.idUnidadMedida
	ORDER BY D.idProcesoProduccionFormula, P.descripcion
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_APP_AGREGAR_PROCESO_PRODUCCION]
@idProducto int,
@lotes float,
@idUsuario int,
@idAlmacen int
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @msg varchar(500) = null,
			@idFormula bigint, @cantidadLote float, @idUnidadMedida int, @cantidadProducida float,
			@idProcesoProduccion bigint, @fecha datetime = dbo.FechaActual(),
			@idTipoMovCarga int, @idTipoMovSalida int, @idUbicacion0 int, @idSucursal int,
			@limiteLotes float = 100 -- tope de lotes por produccion (regla de negocio)

	DECLARE @mpl TABLE (idProducto int, cantidadPorLote float, cantidadConsumida float, idUnidadMedida int, disponible float)
	DECLARE @faltantes TABLE (idProducto int, descripcion varchar(200), requerido float, disponible float, unidadMedida varchar(100)) -- variable de tabla: sobrevive a un ROLLBACK

	BEGIN TRY
		SELECT @idTipoMovCarga = idTipoMovInventario FROM CatTipoMovimientoInventario
			WHERE descripcion = 'Actualizacion de Inventario(carga de producto terminado por proceso de produccion de formula)'
		SELECT @idTipoMovSalida = idTipoMovInventario FROM CatTipoMovimientoInventario
			WHERE descripcion = 'Actualizacion de Inventario(salida de materia prima por proceso de produccion de formula)'

		IF coalesce(@lotes, 0) <= 0 SET @msg = 'El numero de lotes debe ser mayor a 0'
		ELSE IF @lotes > @limiteLotes SET @msg = 'El numero de lotes no puede ser mayor a ' + CONVERT(varchar(20), CONVERT(int, @limiteLotes)) + '.'
		ELSE IF @idTipoMovCarga IS NULL OR @idTipoMovSalida IS NULL SET @msg = 'Falta configurar los tipos de movimiento de inventario para produccion por formula'
		ELSE IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE idUsuario = @idUsuario) SET @msg = 'El usuario no es valido'
		ELSE IF NOT EXISTS (SELECT 1 FROM Almacenes WHERE idAlmacen = @idAlmacen) SET @msg = 'El almacen no es valido'

		IF @msg IS NULL
		BEGIN
			SELECT @idSucursal = idSucursal FROM Usuarios WHERE idUsuario = @idUsuario
			IF EXISTS (SELECT 1 FROM InventarioFisico WHERE idEstatusInventarioFisico = 2 AND idSucursal = @idSucursal)
				SET @msg = 'Existe un inventario fisico activo, no se puede registrar la produccion'
		END

		IF @msg IS NULL
		BEGIN
			SELECT @idFormula = F.idProcesoProduccionFormula, @cantidadLote = F.cantidadLote, @idUnidadMedida = F.idUnidadMedida
			FROM ProcesoProduccionFormula F WHERE F.idProducto = @idProducto AND F.activo = 1
			IF @idFormula IS NULL SET @msg = 'El producto no tiene una formula de produccion activa'
		END

		IF @msg IS NULL AND dbo.ExisteProductoEnAlmancen(@idAlmacen, @idProducto) = 0
			SET @msg = 'El producto no puede ser agregado a este almacen'

		IF @msg IS NULL
		BEGIN
			-- calculo unico de cantidades
			SET @cantidadProducida = ROUND(@lotes * @cantidadLote, 3)
			INSERT INTO @mpl (idProducto, cantidadPorLote, cantidadConsumida, idUnidadMedida)
			SELECT D.idProducto, D.cantidad, ROUND(D.cantidad * @lotes, 3), D.idUnidadMedida
			FROM ProcesoProduccionFormulaDetalle D WHERE D.idProcesoProduccionFormula = @idFormula

			IF @cantidadProducida <= 0 SET @msg = 'La cantidad a producir es menor a 0.001, aumente los lotes'
			ELSE IF NOT EXISTS (SELECT 1 FROM @mpl) SET @msg = 'La formula no tiene materias primas'
			ELSE IF EXISTS (SELECT 1 FROM @mpl WHERE cantidadConsumida <= 0) SET @msg = 'La cantidad de una materia prima a consumir es menor a 0.001, aumente los lotes'
		END

		IF @msg IS NULL
		BEGIN
			UPDATE @mpl SET disponible = ROUND(dbo.ExisteCantidadProductoEnAlmancen(@idAlmacen, idProducto), 3)
			INSERT INTO @faltantes
			SELECT M.idProducto, P.descripcion, M.cantidadConsumida, M.disponible, U.descripcion
			FROM @mpl M JOIN Productos P ON P.idProducto = M.idProducto JOIN CatUnidadMedida U ON U.idUnidadMedida = M.idUnidadMedida
			WHERE M.disponible < M.cantidadConsumida
			IF EXISTS (SELECT 1 FROM @faltantes) SET @msg = 'No existe inventario suficiente de una o mas materias primas'
		END

		IF @msg IS NOT NULL
		BEGIN
			SELECT -1 status, -1 estatus, @msg mensaje
			SELECT * FROM @faltantes
			RETURN
		END

		BEGIN TRAN
			INSERT INTO ProcesoProduccion (idProcesoProduccionFormula, idProducto, lotes, cantidadLote, cantidadProducida, idUnidadMedida, idUsuario, idAlmacen, fechaAlta)
			VALUES (@idFormula, @idProducto, @lotes, @cantidadLote, @cantidadProducida, @idUnidadMedida, @idUsuario, @idAlmacen, @fecha)
			SET @idProcesoProduccion = SCOPE_IDENTITY()

			INSERT INTO ProcesoProduccionDetalle (idProcesoProduccion, idProducto, cantidadPorLote, cantidadConsumida, idUnidadMedida)
			SELECT @idProcesoProduccion, idProducto, cantidadPorLote, cantidadConsumida, idUnidadMedida FROM @mpl

			------------------------------------------------------------------
			-- Salida de cada MPL: de mayor a menor cantidad entre ubicaciones
			------------------------------------------------------------------
			DECLARE @idMpl int, @requerido float, @restante float, @idInvDet int, @idUbic int, @cantUbic float, @toma float, @nueva float, @cantGral float
			DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT idProducto, cantidadConsumida FROM @mpl
			OPEN cur
			FETCH NEXT FROM cur INTO @idMpl, @requerido
			WHILE @@FETCH_STATUS = 0
			BEGIN
				SET @restante = @requerido

				DECLARE cur2 CURSOR LOCAL FAST_FORWARD FOR
					SELECT ID.idInventarioDetalle, ID.idUbicacion, ID.cantidad
					FROM InventarioDetalle ID WITH (UPDLOCK, HOLDLOCK)
					JOIN Ubicacion U ON U.idUbicacion = ID.idUbicacion
					WHERE ID.idProducto = @idMpl AND U.idAlmacen = @idAlmacen AND U.idPiso < 1000 AND ID.cantidad > 0
					ORDER BY ID.cantidad DESC, ID.idInventarioDetalle
				OPEN cur2
				FETCH NEXT FROM cur2 INTO @idInvDet, @idUbic, @cantUbic
				WHILE @@FETCH_STATUS = 0 AND @restante > 0
				BEGIN
					SET @toma = CASE WHEN @cantUbic >= @restante THEN @restante ELSE @cantUbic END
					SET @nueva = ROUND(@cantUbic - @toma, 3)

					INSERT INTO InventarioDetalleLog (idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idProcesoProduccion)
					VALUES (@idUbic, @idMpl, @toma, @nueva, @idTipoMovSalida, @idUsuario, @fecha, @idProcesoProduccion)

					UPDATE InventarioDetalle SET cantidad = @nueva, fechaActualizacion = @fecha WHERE idInventarioDetalle = @idInvDet

					SET @restante = ROUND(@restante - @toma, 3)
					FETCH NEXT FROM cur2 INTO @idInvDet, @idUbic, @cantUbic
				END
				CLOSE cur2
				DEALLOCATE cur2

				IF @restante > 0
					RAISERROR('Inventario insuficiente de una materia prima durante la produccion', 16, 1)

				SELECT @cantGral = cantidad FROM InventarioGeneral WHERE idProducto = @idMpl
				INSERT INTO InventarioGeneralLog (idProducto, cantidad, cantidadDespuesDeOperacion, fechaAlta, idTipoMovInventario)
				VALUES (@idMpl, @requerido, coalesce(@cantGral, 0) - @requerido, @fecha, @idTipoMovSalida)
				UPDATE InventarioGeneral SET cantidad = cantidad - @requerido, fechaUltimaActualizacion = @fecha WHERE idProducto = @idMpl

				FETCH NEXT FROM cur INTO @idMpl, @requerido
			END
			CLOSE cur
			DEALLOCATE cur

			------------------------------------------------------------------
			-- Alta del producto terminado en 0-0-0 (sin acomodar)
			------------------------------------------------------------------
			SELECT @idUbicacion0 = idUbicacion FROM Ubicacion WHERE idAlmacen = @idAlmacen AND idPasillo = 0 AND idRaq = 0 AND idPiso = 0
			IF @idUbicacion0 IS NULL
			BEGIN
				INSERT INTO Ubicacion (idAlmacen, idPasillo, idRaq, idPiso) VALUES (@idAlmacen, 0, 0, 0)
				SET @idUbicacion0 = SCOPE_IDENTITY()
			END

			DECLARE @cantActualTerm float = null, @cantTotalTerm float
			SELECT @cantActualTerm = cantidad FROM InventarioDetalle WITH (UPDLOCK, HOLDLOCK) WHERE idUbicacion = @idUbicacion0 AND idProducto = @idProducto
			SET @cantTotalTerm = ROUND(coalesce(@cantActualTerm, 0) + @cantidadProducida, 3)

			INSERT INTO InventarioDetalleLog (idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idProcesoProduccion)
			VALUES (@idUbicacion0, @idProducto, @cantidadProducida, @cantTotalTerm, @idTipoMovCarga, @idUsuario, @fecha, @idProcesoProduccion)

			IF @cantActualTerm IS NULL
				INSERT INTO InventarioDetalle (idProducto, cantidad, fechaAlta, idUbicacion, fechaActualizacion)
				VALUES (@idProducto, @cantTotalTerm, @fecha, @idUbicacion0, @fecha)
			ELSE
				UPDATE InventarioDetalle SET cantidad = @cantTotalTerm, fechaActualizacion = @fecha WHERE idProducto = @idProducto AND idUbicacion = @idUbicacion0

			SELECT @cantGral = cantidad FROM InventarioGeneral WHERE idProducto = @idProducto
			INSERT INTO InventarioGeneralLog (idProducto, cantidad, cantidadDespuesDeOperacion, fechaAlta, idTipoMovInventario)
			VALUES (@idProducto, @cantidadProducida, coalesce(@cantGral, 0) + @cantidadProducida, @fecha, @idTipoMovCarga)
			IF EXISTS (SELECT 1 FROM InventarioGeneral WHERE idProducto = @idProducto)
				UPDATE InventarioGeneral SET cantidad = cantidad + @cantidadProducida, fechaUltimaActualizacion = @fecha WHERE idProducto = @idProducto
			ELSE
				INSERT INTO InventarioGeneral (idProducto, cantidad, fechaUltimaActualizacion) VALUES (@idProducto, @cantidadProducida, @fecha)
		COMMIT TRAN

		SELECT 200 status, 200 estatus, 'Produccion registrada correctamente' mensaje
		SELECT @idProcesoProduccion idProcesoProduccion
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		SELECT -1 status, -1 estatus, 'Ha ocurrido un error al registrar la produccion: ' + ERROR_MESSAGE() mensaje
		SELECT * FROM @faltantes
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_APP_OBTENER_PROCESO_PRODUCCION_HISTORIAL]
@idUsuario int,
@fechaIni datetime = null,
@fechaFin datetime = null
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @hoy date = CAST(dbo.FechaActual() AS date), @ini date, @fin date
	SET @ini = CAST(coalesce(@fechaIni, @fechaFin, @hoy) AS date)
	SET @fin = CAST(coalesce(@fechaFin, @fechaIni, @hoy) AS date)

	SELECT PP.idProcesoProduccion, PP.idProducto, P.descripcion descripcionProducto, PP.lotes, PP.cantidadProducida,
		   U.descripcion unidadMedida, PP.fechaAlta
	INTO #historial
	FROM ProcesoProduccion PP
	JOIN Productos P ON P.idProducto = PP.idProducto
	JOIN CatUnidadMedida U ON U.idUnidadMedida = PP.idUnidadMedida
	WHERE PP.idUsuario = @idUsuario AND PP.fechaAlta >= @ini AND PP.fechaAlta < DATEADD(DAY, 1, @fin)

	IF NOT EXISTS (SELECT 1 FROM #historial)
	BEGIN
		SELECT -1 status, -1 estatus, 'No se encontraron producciones en el rango de fechas' mensaje
		RETURN
	END

	SELECT 200 status, 200 estatus, 'Consulta correcta' mensaje
	SELECT * FROM #historial ORDER BY fechaAlta DESC, idProcesoProduccion DESC
END
GO

-- VERIFICACION POSTERIOR (3 filas)
select name from sys.procedures where name in ('SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA','SP_APP_AGREGAR_PROCESO_PRODUCCION','SP_APP_OBTENER_PROCESO_PRODUCCION_HISTORIAL')
GO
