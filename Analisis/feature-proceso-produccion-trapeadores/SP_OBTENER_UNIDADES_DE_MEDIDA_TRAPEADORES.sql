
GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_OBTENER_UNIDADES_DE_MEDIDA_TRAPEADORES'))
BEGIN
   DROP PROCEDURE SP_OBTENER_UNIDADES_DE_MEDIDA_TRAPEADORES
END

GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_UNIDADES_DE_MEDIDA_TRAPEADORES]    Script Date: 10/10/2023 1:02:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].SP_OBTENER_UNIDADES_DE_MEDIDA_TRAPEADORES
AS
BEGIN
	SELECT 200 estatus, 'Ok' mensaje

	select * from CatUnidadMedida WHERE idUnidadMedida in (1,5)
END
