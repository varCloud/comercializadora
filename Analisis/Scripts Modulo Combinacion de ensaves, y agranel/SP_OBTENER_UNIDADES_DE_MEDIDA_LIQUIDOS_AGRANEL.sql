GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_OBTENER_UNIDADES_DE_MEDIDA_LIQUIDOS_AGRANEL'))
BEGIN
    DROP PROCEDURE SP_OBTENER_UNIDADES_DE_MEDIDA_LIQUIDOS_AGRANEL
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
CREATE PROCEDURE SP_OBTENER_UNIDADES_DE_MEDIDA_LIQUIDOS_AGRANEL
AS
BEGIN
	SELECT 200 estatus, 'Ok' mensaje

	select * from CatUnidadMedida WHERE idUnidadMedida in (2,4)
END
GO
