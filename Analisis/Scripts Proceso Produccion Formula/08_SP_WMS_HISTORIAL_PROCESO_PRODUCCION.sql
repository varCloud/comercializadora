/*
Objetivo	SPs del WMS (pantalla "Producción" = historial de producciones por formula registradas desde la HH).
			CREATE OR ALTER => idempotente. Solo lectura. No modifica los SP_APP_* ni los de las tablas.
			Primer result set = fila de estatus (status/estatus 200 = ok, -1 = error o sin resultados, mensaje); datos desde el segundo.
			Se devuelven las columnas status Y estatus (los DAOs existentes leen una u otra) y mensaje.

			NOTA DE NOMBRES: SP_CONSULTA_PROCESO_PRODUCCION ya existe (Producción a granel, 2022); por eso estos SP
			se llaman SP_OBTENER_PROCESO_PRODUCCION_FORMULA_HISTORIAL y SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_HISTORIAL.

			SP_OBTENER_PROCESO_PRODUCCION_FORMULA_HISTORIAL
				@fechaIni date = NULL, @fechaFin date = NULL, @idProducto int = NULL (NULL/0 = todos), @idAlmacen int = NULL (NULL/0 = todos)
				Rango por defecto (documentado): si ambas fechas son NULL => ultimos 30 dias (hoy-29 .. hoy, hoy = dbo.FechaActual()).
				Si solo viene @fechaIni => @fechaFin = hoy. Si solo viene @fechaFin => @fechaIni = @fechaFin - 29 dias.
				@fechaFin < @fechaIni => status -1.
				El filtro de fechas es sargable (rango semiabierto sobre fechaAlta); NO se hace CAST de la columna.
				set 2: una fila por produccion (idProcesoProduccion, fechaAlta, idProducto, codigoBarras, descripcionProducto,
					   lotes, cantidadLote, cantidadProducida, unidadMedida, idUsuario, nombreUsuario, idAlmacen, descripcionAlmacen).
				set 3: detalle de las producciones del set 2 (idProcesoProduccion, idProducto, descripcionProducto,
					   cantidadPorLote, cantidadConsumida, unidadMedida).
				Sin producciones en el filtro => status -1 (sin sets adicionales).

			SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_HISTORIAL
				set 2: productos con formula (activa o no) para el filtro de producto (idProducto, descripcion).
*/

-- VERIFICACION PREVIA
select name from sys.procedures where name in ('SP_OBTENER_PROCESO_PRODUCCION_FORMULA_HISTORIAL','SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_HISTORIAL')
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_OBTENER_PROCESO_PRODUCCION_FORMULA_HISTORIAL]
@fechaIni date = null,
@fechaFin date = null,
@idProducto int = null,
@idAlmacen int = null
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @hoy date = CAST(dbo.FechaActual() AS date)

		-- Rango por defecto: ultimos 30 dias
		IF @fechaIni IS NULL AND @fechaFin IS NULL
			SELECT @fechaFin = @hoy, @fechaIni = DATEADD(DAY, -29, @hoy)
		ELSE IF @fechaFin IS NULL
			SET @fechaFin = @hoy
		ELSE IF @fechaIni IS NULL
			SET @fechaIni = DATEADD(DAY, -29, @fechaFin)

		IF @fechaFin < @fechaIni
		BEGIN
			SELECT -1 status, -1 estatus, 'La fecha final no puede ser menor a la fecha inicial' mensaje
			RETURN
		END

		SET @idProducto = NULLIF(@idProducto, 0)
		SET @idAlmacen = NULLIF(@idAlmacen, 0)

		SELECT PP.idProcesoProduccion, PP.fechaAlta, PP.idProducto, P.codigoBarras, P.descripcion descripcionProducto,
			   PP.lotes, PP.cantidadLote, PP.cantidadProducida, UM.descripcion unidadMedida,
			   PP.idUsuario,
			   LTRIM(RTRIM(ISNULL(U.nombre, '') + ' ' + ISNULL(U.apellidoPaterno, '') + ' ' + ISNULL(U.apellidoMaterno, ''))) nombreUsuario,
			   PP.idAlmacen, A.descripcion descripcionAlmacen
		INTO #produccion
		FROM ProcesoProduccion PP
		JOIN Productos P ON P.idProducto = PP.idProducto
		JOIN CatUnidadMedida UM ON UM.idUnidadMedida = PP.idUnidadMedida
		LEFT JOIN Usuarios U ON U.idUsuario = PP.idUsuario
		LEFT JOIN Almacenes A ON A.idAlmacen = PP.idAlmacen
		WHERE PP.fechaAlta >= @fechaIni AND PP.fechaAlta < DATEADD(DAY, 1, @fechaFin)
		  AND (@idProducto IS NULL OR PP.idProducto = @idProducto)
		  AND (@idAlmacen IS NULL OR PP.idAlmacen = @idAlmacen)

		IF NOT EXISTS (SELECT 1 FROM #produccion)
		BEGIN
			SELECT -1 status, -1 estatus, 'No se encontraron producciones con los filtros seleccionados' mensaje
			RETURN
		END

		SELECT 200 status, 200 estatus, 'Consulta correcta' mensaje

		SELECT idProcesoProduccion, fechaAlta, idProducto, codigoBarras, descripcionProducto, lotes, cantidadLote,
			   cantidadProducida, unidadMedida, idUsuario, nombreUsuario, idAlmacen, descripcionAlmacen
		FROM #produccion
		ORDER BY fechaAlta DESC, idProcesoProduccion DESC

		SELECT D.idProcesoProduccion, D.idProducto, P.descripcion descripcionProducto, D.cantidadPorLote, D.cantidadConsumida,
			   UM.descripcion unidadMedida
		FROM ProcesoProduccionDetalle D
		JOIN #produccion X ON X.idProcesoProduccion = D.idProcesoProduccion
		JOIN Productos P ON P.idProducto = D.idProducto
		JOIN CatUnidadMedida UM ON UM.idUnidadMedida = D.idUnidadMedida
		ORDER BY D.idProcesoProduccion, P.descripcion
	END TRY
	BEGIN CATCH
		SELECT -1 status, -1 estatus, 'Ha ocurrido un error al consultar las producciones: ' + ERROR_MESSAGE() mensaje
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_HISTORIAL]
AS
BEGIN
	SET NOCOUNT ON

	SELECT 200 status, 200 estatus, 'Consulta correcta' mensaje

	SELECT P.idProducto, P.descripcion
	FROM Productos P
	WHERE EXISTS (SELECT 1 FROM ProcesoProduccionFormula F WHERE F.idProducto = P.idProducto)
	ORDER BY P.descripcion
END
GO

-- VERIFICACION POSTERIOR (2 filas)
select name from sys.procedures where name in ('SP_OBTENER_PROCESO_PRODUCCION_FORMULA_HISTORIAL','SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_HISTORIAL')
GO
