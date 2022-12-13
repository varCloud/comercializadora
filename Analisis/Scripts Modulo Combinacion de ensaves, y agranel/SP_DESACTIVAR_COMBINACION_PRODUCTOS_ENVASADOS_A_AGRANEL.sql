GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_DESACTIVAR_COMBINACION_PRODUCTOS_ENVASADOS_A_AGRANEL'))
BEGIN
    DROP PROCEDURE SP_DESACTIVAR_COMBINACION_PRODUCTOS_ENVASADOS_A_AGRANEL
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
CREATE PROCEDURE SP_DESACTIVAR_COMBINACION_PRODUCTOS_ENVASADOS_A_AGRANEL
@idRelacionEnvasadoAgranel int
AS
BEGIN

	UPDATE ProductosEnvasadosXAgranel SET activo = 0 where idRelacionEnvasadoAgranel = @idRelacionEnvasadoAgranel
	SELECT 200 Estatus, 'Combinacion de productos eliminada exitosamente' Mensaje	
	
END
GO
