-- USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_APP_CONSULTA_PRODUCTOS_POR_DESCRIPCION_X_LINEA_PRODUCTO]    Script Date: 10/4/2023 7:10:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_APP_CONSULTA_PRODUCTOS_POR_DESCRIPCION_X_LINEA_PRODUCTO]
@idLineaProducto int,
@idUsuario int,
@descripcion varchar (500) = null,
@codigoBarras varchar(500) = null
AS
BEGIN
	
	declare
	 @idRol int 
	,@idlineaEnvases int = 19  /*19	Linea ENVASES*/
	,@idlineaLiquidos int = 20 /*20	Linea LIQUIDOS*/
	,@idlineaEnvasado int = 27 /*27	Linea ENVASADO*/
	,@idlineaMPL int = 12      /*12	Linea MPL */
	,@idlineaMatra int = 21    /*21	Linea MATRA */
	,@idlineaBastones int = 26 /*26	Linea BASTONES */
	,@idlineaTrapeador int = 4 /*4	Linea TRAPEADOR */
	--SET @idLineaProducto  = null


	select  @idRol = idRol  from Usuarios where idUsuario = @idUsuario
	
	--if(coalesce(@idLineaProducto,0) = 0)
	--BEGIN
	--	select -1 estatus , 'La lienea de producto es requerida' mensaje
	--	return
	--END
	
	IF EXISTS (SELECT 1 FROM Productos P WHERE P.descripcion LIKE '%'+@descripcion+'%' OR P.idLineaProducto = idLineaProducto OR  P.codigoBarras = coalesce(@codigoBarras , P.codigoBarras) )
	begin
		select 200 estatus , 'se encontraron coincidencias' mensaje
		if @idRol  = 12 -- Encargado de envasado (líquidos)
		begin
			select [dbo].[LineaProductoFraccion](P.idLineaProducto,null) fraccion , P.*
			from Productos P 
			where 
				(descripcion like coalesce('%'+@descripcion+'%',descripcion)
				and P.idLineaProducto in (@idLineaProducto, @idlineaEnvases ,@idlineaLiquidos, @idlineaEnvasado,  @idlineaMPL) 
				and P.codigoBarras = coalesce(@codigoBarras , P.codigoBarras)) 
				and activo= 1 
			order by descripcion
		end
		else if @idRol  = 14 --Encargado de mpl para trapeadores produccion 
		begin
			select [dbo].[LineaProductoFraccion](P.idLineaProducto,null) fraccion , P.*
			from Productos P 
			where 
				(descripcion like coalesce('%'+@descripcion+'%',descripcion)
				and P.idLineaProducto in (@idLineaProducto, @idlineaMatra ,@idlineaBastones, @idlineaTrapeador) 
				and P.codigoBarras = coalesce(@codigoBarras , P.codigoBarras)) 
				and activo= 1 
			order by descripcion
		end
		else
		begin
			select [dbo].[LineaProductoFraccion](P.idLineaProducto,null) fraccion ,P.*
			from Productos P 
			where (descripcion  like coalesce('%'+@descripcion+'%' ,descripcion )
			AND P.idLineaProducto in (coalesce(@idLineaProducto,P.idLineaProducto), @idlineaEnvases ,@idlineaLiquidos, @idlineaEnvasado,  @idlineaMPL) 
			AND  P.codigoBarras = coalesce(@codigoBarras , P.codigoBarras))  and activo= 1 
			order by descripcion
		end
	end
	else
	begin
		select -1 estatus , 'no existe un producto registrado con eso codigo de barras' mensaje
	end
END
