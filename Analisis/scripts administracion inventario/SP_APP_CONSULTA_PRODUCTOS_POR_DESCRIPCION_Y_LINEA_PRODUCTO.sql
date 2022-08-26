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
alter PROCEDURE [dbo].[SP_APP_CONSULTA_PRODUCTOS_POR_DESCRIPCION_X_LINEA_PRODUCTO]
@idLineaProducto int,
@idUsuario int,
@descripcion varchar (500) = null,
@codigoBarras varchar(500) = null
AS
BEGIN
	
	declare
	 @idRol int 
	,@idlineaEnvases int = 19 /*19	Linea ENVASES*/
	select  @idRol = idRol  from Usuarios where idUsuario = @idUsuario
	
	if(coalesce(@idLineaProducto,0) = 0)
	BEGIN
		select -1 estatus , 'La lienea de producto es requerida' mensaje
		return
	END
	
	IF EXISTS (SELECT 1 FROM Productos P WHERE P.descripcion LIKE '%'+@descripcion+'%' OR P.idLineaProducto = idLineaProducto OR  P.codigoBarras = coalesce(@codigoBarras , P.codigoBarras) )
	begin
		select 200 estatus , 'se encontraron coincidencias' mensaje
		if @idRol  = 12 
		begin
			select [dbo].[LineaProductoFraccion](P.idLineaProducto,null) fraccion , P.*
			from Productos P 
			where 
				(descripcion like coalesce('%'+@descripcion+'%',descripcion)
				and P.idLineaProducto in (@idLineaProducto, @idlineaEnvases) 
				and P.codigoBarras = coalesce(@codigoBarras , P.codigoBarras)) 
				and activo= 1 
			order by descripcion
		end
		else
		begin
			select [dbo].[LineaProductoFraccion](P.idLineaProducto,null) fraccion ,P.*
			from Productos P 
			where (descripcion  like coalesce('%'+@descripcion+'%' ,descripcion )
			AND P.idLineaProducto in (coalesce(@idLineaProducto,P.idLineaProducto)) 
			AND  P.codigoBarras = coalesce(@codigoBarras , P.codigoBarras))  and activo= 1 
			order by descripcion
		end
	end
	else
	begin
		select -1 estatus , 'no existe un producto registrado con eso codigo de barras' mensaje
	end
END
