select * from InventarioGeneral where idProducto in(1,2,3,4)

select u.idAlmacen,* 
from InventarioDetalle a
join Ubicacion u on a.idUbicacion=u.idUbicacion
where idProducto in(1,2,3,4)
order by u.idAlmacen,idProducto

select u.idAlmacen,a.idProducto,sum(cantidad) 
from InventarioDetalle a
join Ubicacion u on a.idUbicacion=u.idUbicacion
where idProducto in(1,2,3,4)
group by u.idAlmacen,a.idProducto
order by u.idAlmacen,a.idProducto

--SELECT * FROM inventarioDetalleLog where idProducto in(1,2,3,4)