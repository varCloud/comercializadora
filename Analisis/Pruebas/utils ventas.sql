declare @idProducto int=626

select * from Productos where idProducto=@idProducto

select 'ProductosPorPrecio',* from ProductosPorPrecio  where idProducto=@idProducto
select 'InventarioGeneral',* from InventarioGeneral where idProducto=@idProducto
select 'InventarioGeneralLog',* from InventarioGeneralLog where idProducto=@idProducto
select 'InventarioDetalle',u.idAlmacen,* from InventarioDetalle d
join Ubicacion u on d.idUbicacion=u.idUbicacion where idProducto=@idProducto
select 'inventariodetallelog',* from inventariodetallelog where idProducto=@idProducto

--select * from InventarioDetalle where idUbicacion=2
--select 'ventas',* from ventas 
--select 'ventasDetalle',* from ventasDetalle

--select * from Usuarios


--select * from Ubicacion where idUbicacion in (2)

--alamcen 1 50
--alamacen 2 100
--almacen 3 150


--select * from proveedores
