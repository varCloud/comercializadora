-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE SP_OBTENER_PRODUCTOS_ENVASES_LIQUIDOS_AGRANEL
AS
BEGIN
	SELECT 200 estatus, 'Ok' mensaje

	 -- select * from LineaProducto
	select * from Productos where idLineaProducto = 12 -- MPL / AGRANEL
	select * from Productos where idLineaProducto = 27 -- ENAVSES / ENVASADO
	select * from Productos where idLineaProducto = 20 -- LIQUIDOS
END
GO
