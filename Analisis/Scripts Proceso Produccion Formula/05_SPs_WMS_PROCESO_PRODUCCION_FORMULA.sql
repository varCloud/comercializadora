/*
Objetivo	SPs del WMS (pantalla Relacion Produccion). CREATE OR ALTER => idempotente.
			Primer result set = fila de estatus (status/estatus 200 = ok, -1 = error, mensaje); datos desde el segundo.
			Se devuelven las columnas status Y estatus (los DAOs existentes leen una u otra) y mensaje.

			SP_OBTENER_PROCESO_PRODUCCION_FORMULAS
			SP_OBTENER_PROCESO_PRODUCCION_FORMULA_DETALLE
			SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_FORMULA
			SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA
			SP_DESACTIVAR_PROCESO_PRODUCCION_FORMULA
*/

-- VERIFICACION PREVIA
select name from sys.procedures where name in ('SP_OBTENER_PROCESO_PRODUCCION_FORMULAS','SP_OBTENER_PROCESO_PRODUCCION_FORMULA_DETALLE','SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_FORMULA','SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA','SP_DESACTIVAR_PROCESO_PRODUCCION_FORMULA')
GO

-- Necesario para indices filtrados (sqlcmd los trae en OFF por defecto)
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_OBTENER_PROCESO_PRODUCCION_FORMULAS]
AS
BEGIN
	select 200 status, 200 estatus, 'Consulta correcta' mensaje

	select F.idProcesoProduccionFormula, F.idProducto, P.descripcion descripcionProducto,
		   F.idUnidadMedida, U.descripcion unidadMedida, F.cantidadLote, F.fechaAlta
	from ProcesoProduccionFormula F
	join Productos P on P.idProducto = F.idProducto
	join CatUnidadMedida U on U.idUnidadMedida = F.idUnidadMedida
	where F.activo = 1
	order by P.descripcion
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_OBTENER_PROCESO_PRODUCCION_FORMULA_DETALLE]
@idProcesoProduccionFormula bigint
AS
BEGIN
	select 200 status, 200 estatus, 'Consulta correcta' mensaje

	select D.idProcesoProduccionFormulaDetalle, D.idProcesoProduccionFormula, D.idProducto,
		   P.descripcion descripcionProducto, D.idUnidadMedida, U.descripcion unidadMedida, D.cantidad
	from ProcesoProduccionFormulaDetalle D
	join Productos P on P.idProducto = D.idProducto
	join CatUnidadMedida U on U.idUnidadMedida = D.idUnidadMedida
	where D.idProcesoProduccionFormula = @idProcesoProduccionFormula
	order by P.descripcion
END
GO

-- Set 2: productos linea 20 activos sin formula activa (+ el de la formula en edicion). Set 3: materias primas linea 12 activas.
CREATE OR ALTER PROCEDURE [dbo].[SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_FORMULA]
@idProcesoProduccionFormula bigint = null
AS
BEGIN
	select 200 status, 200 estatus, 'Consulta correcta' mensaje

	select P.idProducto, P.descripcion, P.idUnidadMedida, U.descripcion unidadMedida
	from Productos P
	join CatUnidadMedida U on U.idUnidadMedida = P.idUnidadMedida
	where P.idLineaProducto = 20 and P.activo = 1
	  and (not exists (select 1 from ProcesoProduccionFormula F where F.idProducto = P.idProducto and F.activo = 1)
		   or exists (select 1 from ProcesoProduccionFormula F where F.idProducto = P.idProducto and F.idProcesoProduccionFormula = @idProcesoProduccionFormula))
	order by P.descripcion

	select P.idProducto, P.descripcion, P.idUnidadMedida, U.descripcion unidadMedida
	from Productos P
	join CatUnidadMedida U on U.idUnidadMedida = P.idUnidadMedida
	where P.idLineaProducto = 12 and P.activo = 1
	order by P.descripcion
END
GO

/*
@idProcesoProduccionFormula = 0 => alta; > 0 => edicion (reemplaza el detalle).
@xmlMateriasPrimas: <materiasPrimas><materiaPrima idProducto="907" cantidad="10"/>...</materiasPrimas>
Devuelve status/estatus/mensaje y, en set 2 (solo si 200), el idProcesoProduccionFormula.
*/
CREATE OR ALTER PROCEDURE [dbo].[SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA]
@idProcesoProduccionFormula bigint = 0,
@idProducto int,
@cantidadLote float,
@idUsuario int,
@xmlMateriasPrimas xml
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	DECLARE @limiteLote float = 10000 -- tope de cantidadLote (cambiar aqui)
	DECLARE @idUnidadMedida int, @idFormula bigint = coalesce(@idProcesoProduccionFormula, 0), @fecha datetime = dbo.FechaActual()

	BEGIN TRY
		CREATE TABLE #mp (idProducto int, cantidad float, idUnidadMedida int)

		INSERT INTO #mp (idProducto, cantidad)
		SELECT x.m.value('@idProducto', 'int'), x.m.value('@cantidad', 'float')
		FROM @xmlMateriasPrimas.nodes('/materiasPrimas/materiaPrima') AS x(m)

		IF coalesce(@cantidadLote, 0) <= 0
		BEGIN select -1 status, -1 estatus, 'La cantidad del lote debe ser mayor a 0' mensaje; return END

		IF @cantidadLote > @limiteLote
		BEGIN select -1 status, -1 estatus, 'La cantidad del lote no puede ser mayor a 10,000.' mensaje; return END

		IF NOT EXISTS (select 1 from Productos where idProducto = @idProducto and idLineaProducto = 20 and activo = 1)
		BEGIN select -1 status, -1 estatus, 'El producto a producir debe ser un producto activo de la linea de liquidos' mensaje; return END

		IF NOT EXISTS (select 1 from Usuarios where idUsuario = @idUsuario)
		BEGIN select -1 status, -1 estatus, 'El usuario no es valido' mensaje; return END

		IF NOT EXISTS (select 1 from #mp)
		BEGIN select -1 status, -1 estatus, 'La formula debe tener al menos una materia prima' mensaje; return END

		IF EXISTS (select 1 from #mp where idProducto is null or coalesce(cantidad, 0) <= 0)
		BEGIN select -1 status, -1 estatus, 'Todas las materias primas deben tener una cantidad mayor a 0' mensaje; return END

		IF EXISTS (select idProducto from #mp group by idProducto having count(*) > 1)
		BEGIN select -1 status, -1 estatus, 'Una materia prima no puede repetirse en la formula' mensaje; return END

		IF EXISTS (select 1 from #mp M left join Productos P on P.idProducto = M.idProducto and P.idLineaProducto = 12 and P.activo = 1 where P.idProducto is null)
		BEGIN select -1 status, -1 estatus, 'Todas las materias primas deben ser productos activos de la linea MPL' mensaje; return END

		IF @idFormula > 0 AND NOT EXISTS (select 1 from ProcesoProduccionFormula where idProcesoProduccionFormula = @idFormula and activo = 1)
		BEGIN select -1 status, -1 estatus, 'La formula no existe o ya fue eliminada' mensaje; return END

		IF EXISTS (select 1 from ProcesoProduccionFormula where idProducto = @idProducto and activo = 1 and idProcesoProduccionFormula <> @idFormula)
		BEGIN select -1 status, -1 estatus, 'El producto ya tiene una formula activa' mensaje; return END

		-- la unidad de medida siempre sale del producto
		select @idUnidadMedida = idUnidadMedida from Productos where idProducto = @idProducto
		update M set idUnidadMedida = P.idUnidadMedida from #mp M join Productos P on P.idProducto = M.idProducto

		BEGIN TRAN
			IF @idFormula = 0
			BEGIN
				INSERT INTO ProcesoProduccionFormula (idProducto, cantidadLote, idUnidadMedida, activo, idUsuario, fechaAlta, fechaUltimaActualizacion)
				VALUES (@idProducto, @cantidadLote, @idUnidadMedida, 1, @idUsuario, @fecha, @fecha)
				SET @idFormula = SCOPE_IDENTITY()
			END
			ELSE
			BEGIN
				UPDATE ProcesoProduccionFormula
				SET idProducto = @idProducto, cantidadLote = @cantidadLote, idUnidadMedida = @idUnidadMedida,
					idUsuario = @idUsuario, fechaUltimaActualizacion = @fecha
				WHERE idProcesoProduccionFormula = @idFormula
				DELETE FROM ProcesoProduccionFormulaDetalle WHERE idProcesoProduccionFormula = @idFormula
			END

			INSERT INTO ProcesoProduccionFormulaDetalle (idProcesoProduccionFormula, idProducto, cantidad, idUnidadMedida)
			SELECT @idFormula, idProducto, cantidad, idUnidadMedida FROM #mp
		COMMIT TRAN

		select 200 status, 200 estatus, case when coalesce(@idProcesoProduccionFormula, 0) = 0 then 'Formula agregada correctamente' else 'Formula actualizada correctamente' end mensaje
		select @idFormula idProcesoProduccionFormula
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		select -1 status, -1 estatus, 'Ha ocurrido un error al guardar la formula: ' + ERROR_MESSAGE() mensaje
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_DESACTIVAR_PROCESO_PRODUCCION_FORMULA]
@idProcesoProduccionFormula bigint,
@idUsuario int
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	BEGIN TRY
		IF NOT EXISTS (select 1 from Usuarios where idUsuario = @idUsuario)
		BEGIN select -1 status, -1 estatus, 'El usuario no es valido' mensaje; return END

		IF NOT EXISTS (select 1 from ProcesoProduccionFormula where idProcesoProduccionFormula = @idProcesoProduccionFormula and activo = 1)
		BEGIN select -1 status, -1 estatus, 'La formula no existe o ya fue eliminada' mensaje; return END

		BEGIN TRAN
			UPDATE ProcesoProduccionFormula
			SET activo = 0, idUsuario = @idUsuario, fechaUltimaActualizacion = dbo.FechaActual()
			WHERE idProcesoProduccionFormula = @idProcesoProduccionFormula AND activo = 1
		COMMIT TRAN

		select 200 status, 200 estatus, 'Formula eliminada correctamente' mensaje
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		select -1 status, -1 estatus, 'Ha ocurrido un error al eliminar la formula: ' + ERROR_MESSAGE() mensaje
	END CATCH
END
GO

-- VERIFICACION POSTERIOR (5 filas)
select name from sys.procedures where name in ('SP_OBTENER_PROCESO_PRODUCCION_FORMULAS','SP_OBTENER_PROCESO_PRODUCCION_FORMULA_DETALLE','SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_FORMULA','SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA','SP_DESACTIVAR_PROCESO_PRODUCCION_FORMULA')
GO
