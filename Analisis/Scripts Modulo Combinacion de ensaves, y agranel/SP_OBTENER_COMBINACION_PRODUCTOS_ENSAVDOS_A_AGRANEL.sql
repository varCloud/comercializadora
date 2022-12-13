GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_OBTENER_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL'))
BEGIN
    DROP PROCEDURE SP_OBTENER_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE SP_OBTENER_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL
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
	where PE.activo =1
		order by PE.idRelacionEnvasadoAgranel desc
END
GO
