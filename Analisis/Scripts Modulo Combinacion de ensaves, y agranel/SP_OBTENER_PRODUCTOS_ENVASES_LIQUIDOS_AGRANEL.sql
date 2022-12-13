GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_OBTENER_PRODUCTOS_ENVASES_LIQUIDOS_AGRANEL'))
BEGIN
    DROP PROCEDURE SP_OBTENER_PRODUCTOS_ENVASES_LIQUIDOS_AGRANEL
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
create PROCEDURE SP_OBTENER_PRODUCTOS_ENVASES_LIQUIDOS_AGRANEL
AS
BEGIN
	SELECT 200 estatus, 'Ok' mensaje

	 -- select * from LineaProducto
	select * from Productos where idLineaProducto IN (12,20) order by descripcion asc-- MPL / AGRANEL / LIQUIDOS
	select * from Productos where idLineaProducto = 27 order by descripcion asc --  ENVASADO
	select * from Productos where idLineaProducto = 19  order by descripcion asc -- ENVASES 
END
GO
