
declare @idCompra int=3

--select * from CatEstatusProductoCompra

select 'Compras',* from compras where idCompra=@idCompra
select 'Compras Detalle',b.descripcion,c.idCompraDetalle,c.idProducto,c.cantidad,c.cantidadRecibida,c.cantidadDevuelta 
from comprasdetalle c 
left join CatEstatusProductoCompra b on c.idEstatusProductoCompra=b.idEstatusProductoCompra
where c.idCompra=@idCompra 
order by c.idProducto

select 'InventarioDetalleLog',d.* from comprasdetalle c
join InventarioDetalleLog d on c.idCompra=d.idCompra and c.idProducto=d.idProducto
where c.idCompra=@idCompra
order by c.idProducto

select 'InventarioGeneral',* from InventarioGeneral g
join comprasdetalle a on g.idProducto=a.idProducto
where a.idCompra=@idCompra
order by a.idProducto

select 'InventarioDetalle',* from InventarioDetalle d
join comprasdetalle a on d.idProducto=a.idProducto
where a.idCompra=@idCompra
order by a.idProducto

--UPDATE compras set idStatusCompra=1

--UPDATE comprasdetalle set fechaRecibio=null,idEstatusProductoCompra=null,cantidadRecibida=null,cantidadDevuelta=null

--delete d from comprasdetalle c
--join InventarioDetalleLog d on c.idCompra=d.idCompra and c.idProducto=d.idProducto
--where c.idCompra=@idCompra

--delete g from InventarioGeneral g
--join comprasdetalle a on g.idProducto=a.idProducto
--where a.idCompra=@idCompra

--delete d from InventarioDetalle d
--join comprasdetalle a on d.idProducto=a.idProducto
--where a.idCompra=@idCompra

--select * from almacenes

--select * from Ubicacion where idAlmacen=1
--select * from Usuarios where idAlmacen=1




