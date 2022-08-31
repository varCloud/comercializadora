-- se crea procedimiento SP_OBTENER_LINEAS_ALMACEN
if exists (select * from sysobjects where name like 'SP_OBTENER_LINEAS_ALMACEN' and xtype = 'p' )
	drop proc SP_OBTENER_LINEAS_ALMACEN
go

/*

Autor			Jessica Almonte Acosta
Fecha			2022/02/18
Objetivo		Consultar lineas por almacen

*/

create proc SP_OBTENER_LINEAS_ALMACEN
@idAlmacen int=null
as

	begin -- principal

	CREATE TABLE #LineaProductoUsuario (contador int primary key identity ,idLineaProducto int , idAlmacen int ,  descripcion varchar(255))
	if(coalesce(@idAlmacen,0)>0)
	begin
		insert into #LineaProductoUsuario select * from obtnerLineasProductosXAlmacen(null , @idAlmacen)
	end
	else
	begin
		insert into #LineaProductoUsuario(idLineaProducto,idAlmacen,descripcion) 
		select idLineaProducto,@idAlmacen,descripcion from LineaProducto where activo=1					
	end


	select * from #LineaProductoUsuario

	end  -- principal
go

grant exec on SP_OBTENER_LINEAS_ALMACEN to public
go
