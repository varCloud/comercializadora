
declare @idProducto int =  581 --FIBRA AKAX

select idProducto =@idProducto from Productos where descripcion like '%FIBRA AJAX%'

select 'InventarioGeneral', * from InventarioGeneral where idProducto in(@idProducto)

select 'InventarioDetalleLog Tienda Jarcieria', u.idAlmacen,* from InventarioDetalleLog a join Ubicacion u on a.idUbicacion=u.idUbicacion
where idProducto in(@idProducto) and idAlmacen =4  order by idInventarioDetalleLOG desc

select 'InventarioDetalle Tienda Jarcieria', u.idAlmacen,* from InventarioDetalle a join Ubicacion u on a.idUbicacion=u.idUbicacion
where idProducto in(@idProducto) and idAlmacen =4  order by idInventarioDetalle desc

select 'InventarioDetalle Almacen General', u.idAlmacen,* from InventarioDetalle a join Ubicacion u on a.idUbicacion=u.idUbicacion
where idProducto in(@idProducto) and idAlmacen =1 order by idInventarioDetalle desc

select 'InventarioDetalleLog Almacen General', u.idAlmacen,* from InventarioDetalleLog a join Ubicacion u on a.idUbicacion=u.idUbicacion
where idProducto in(@idProducto) and idAlmacen =1  order by idInventarioDetalleLOG desc

select * from PedidosEspeciales order By fechaAlta desc

select * from PedidosEspecialesDetalle where idPedidoEspecial = 49

select * from PedidosEspecialesMovimientosDeMercancia

select * from Ubicacion where idUbicacion = 3794

select * from [dbo].[CatTipoMovimientoInventario]


--SELECT * FROM inventarioDetalleLog where idProducto in(1,2,3,4)