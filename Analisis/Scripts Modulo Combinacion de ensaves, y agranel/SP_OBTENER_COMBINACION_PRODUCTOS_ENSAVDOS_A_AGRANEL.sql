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
CREATE PROCEDURE SP_OBTENER_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL
AS
BEGIN
	SELECT 200 estatus, 'Ok' mensaje
	select 
		PE.*, 
		PEnavase.descripcion envaseDescripcion,
		PAgranel.descripcion agranelDescripcion,
		PEnavsado.descripcion envasadoDescripcion
	from ProductosEnvasadosXAgranel PE 
		join Productos PEnavase on PE.idProducoEnvase = PEnavase.idProducto
		join Productos PAgranel on PE.idProductoAgranel = PAgranel.idProducto
		join Productos PEnavsado on PE.idProductoEnvasado = PEnavsado.idProducto
END
GO
