
GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_OBTENER_PRODUCTOS_FABRICAR_TRAPEADORES'))
BEGIN
   DROP PROCEDURE SP_OBTENER_PRODUCTOS_FABRICAR_TRAPEADORES
END

GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_PRODUCTOS_ENVASES_LIQUIDOS_AGRANEL]    Script Date: 10/9/2023 5:16:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_OBTENER_PRODUCTOS_FABRICAR_TRAPEADORES]
AS
BEGIN
	SELECT 200 estatus, 'Ok' mensaje
	select * from Productos where idLineaProducto IN (21) order by descripcion asc -- MATRA
	select * from Productos where idLineaProducto IN (4) order by descripcion asc -- TRAPEADORES
	select * from Productos where idLineaProducto IN (26) order by descripcion asc -- BASTONES
END
	