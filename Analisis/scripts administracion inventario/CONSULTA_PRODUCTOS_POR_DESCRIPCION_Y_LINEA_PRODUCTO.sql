--USE [DB_A57E86_comercializadora]
GO
/****** Object:  StoredProcedure [dbo].[SP_APP_CONSULTA_PRODUCTOS_POR_DESCRIPCION]    Script Date: 14/06/2022 10:36:01 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[SP_APP_CONSULTA_PRODUCTOS_POR_DESCRIPCION_X_LINEA_PRODUCTO]
@idLineaProducto int,
@idUsuario int,
@descripcion varchar (500)
AS
BEGIN
	select * from CatRoles
	select * from Usuarios where idUsuario = @idUsuario
	IF EXISTS (SELECT 1 FROM Productos P WHERE P.descripcion LIKE '%'+@descripcion+'%' OR P.idLineaProducto = idLineaProducto )
	begin
		select 200 estatus , 'se encontro un coincidencia' mensaje
		select P.*,[dbo].[LineaProductoFraccion](P.idLineaProducto,null) fraccion from Productos P where descripcion  like '%'+@descripcion+'%'  and activo= 1 order by descripcion
	end
	else
	begin
		select -1 estatus , 'no existe un producto registrado con eso codigo de barras' mensaje
	end
END
