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
CREATE PROCEDURE SP_AGREGA_ACTUALIZA_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL
@idRelacionEnvasadoAgranel int = null,
@idProductoEnvasado int,
@idProductoAgranel int,
@idProducoEnvase int,
@unidadMedidad varchar(50),
@valorUnidadMedida float
AS
BEGIN
	IF (coalesce(@idRelacionEnvasadoAgranel,0) = 0)
	BEGIN
		INSERT INTO ProductosEnvasadosXAgranel(
					idProductoEnvasado
					,idProductoAgranel
					,idProducoEnvase
					,unidadMedidad
					,valorUnidadMedida)
		VALUES(
					@idProductoEnvasado
					,@idProductoAgranel
					,@idProducoEnvase
					,@unidadMedidad
					,@valorUnidadMedida)
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
				valorUnidadMedida =@valorUnidadMedida
		WHERE idRelacionEnvasadoAgranel = @idRelacionEnvasadoAgranel
		select  200 estatus, 'Relacion de productos actualizado' mensaje
	END
END
GO
