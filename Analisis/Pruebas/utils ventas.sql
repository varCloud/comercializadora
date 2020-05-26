declare @idProducto int=628

select * from Productos where idProducto=@idProducto

select 'ProductosPorPrecio',* from ProductosPorPrecio  where idProducto=@idProducto
select 'InventarioGeneral',* from InventarioGeneral where idProducto=@idProducto
select 'InventarioDetalle',u.idAlmacen,* from InventarioDetalle d
join Ubicacion u on d.idUbicacion=u.idUbicacion where idProducto=@idProducto
select 'inventariodetallelog',* from inventariodetallelog where idProducto=@idProducto
select 'ventas',* from ventas 
select 'ventasDetalle',* from ventasDetalle

--select * from Ubicacion where idUbicacion in (2)

--alamcen 1 50
--alamacen 2 100
--almacen 3 150


--select * from proveedores
