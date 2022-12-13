GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_AGREGA_ACTUALIZA_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL'))
BEGIN
    DROP PROCEDURE SP_AGREGA_ACTUALIZA_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL
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
CREATE PROCEDURE SP_AGREGA_ACTUALIZA_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL
@idRelacionEnvasadoAgranel int = null,
@idProductoEnvasado int,
@idProductoAgranel int,
@idProducoEnvase int,
@unidadMedidad varchar(50),
@valorUnidadMedida decimal(18,2),
@idUnidadMedida int
AS
BEGIN

	select  @unidadMedidad = LTRIM(TRIM(UPPER(coalesce(unidadSAT,''))))  from CatUnidadMedida where idUnidadMedida = @idUnidadMedida
	IF (coalesce(@idRelacionEnvasadoAgranel,0) = 0)
	BEGIN
		INSERT INTO ProductosEnvasadosXAgranel(
					idProductoEnvasado
					,idProductoAgranel
					,idProducoEnvase
					,unidadMedidad
					,valorUnidadMedida
					,activo
					,fechaAlta
					,idUnidadMedidad)
		VALUES(
					@idProductoEnvasado
					,@idProductoAgranel
					,@idProducoEnvase
					,@unidadMedidad
					,@valorUnidadMedida
					,1
					,dbo.FechaActual()
					,@idUnidadMedida)
		select  200 estatus, 'Relacion de productos agregado' mensaje
		select
			* 
		from 
			ProductosEnvasadosXAgranel 
		where idRelacionEnvasadoAgranel = (select MAX(idRelacionEnvasadoAgranel) idRelacionEnvasadoAgranel from ProductosEnvasadosXAgranel)
	END
	ELSE
	BEGIN
		UPDATE ProductosEnvasadosXAgranel 
		SET
				idProductoEnvasado =@idProductoEnvasado,
				idProductoAgranel =@idProductoAgranel,
				idProducoEnvase =@idProducoEnvase,
				unidadMedidad =@unidadMedidad,
				valorUnidadMedida =@valorUnidadMedida,
				idUnidadMedidad= @idUnidadMedida
		WHERE idRelacionEnvasadoAgranel = @idRelacionEnvasadoAgranel
		select  200 estatus, 'Relacion de productos actualizado' mensaje
	END
END
GO
