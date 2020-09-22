
DECLARE @idPedidoInterno int=null

select @idPedidoInterno=coalesce(@idPedidoInterno,(select max(idPedidoInterno) from pedidosInternos where idTipoPedidoInterno=2))

select 'pedidosInternos',* from pedidosInternos where idTipoPedidoInterno=2 and idPedidoInterno=@idPedidoInterno
select 'PedidosInternosLog',* from PedidosInternosLog where idPedidoInterno=@idPedidoInterno
select 'PedidosInternosDetalle',* from PedidosInternosDetalle where idPedidoInterno=@idPedidoInterno 
select 'movimientosMercancia',* from MovimientosDeMercancia where idPedidoInterno=@idPedidoInterno
order by idMovMercancia

select 'InventarioGeneral',b.* 
from PedidosInternosDetalle a
join InventarioGeneral b on a.idProducto=b.idProducto
where idPedidoInterno=@idPedidoInterno 

select 'InventarioDetalle',u.idAlmacen,b.idUbicacion,b.idProducto,b.cantidad 
from PedidosInternosDetalle a
join InventarioDetalle b on a.idProducto=b.idProducto
join Ubicacion u on b.idUbicacion=u.idUbicacion
where idPedidoInterno=@idPedidoInterno 
order by idAlmacen,idUbicacion

select 'InventarioDetalleLog',b.fechaAlta,u.idAlmacen,b.idUbicacion,b.idProducto,b.cantidad,cantidadActual,b.idTipoMovInventario,b.idPedidoInterno 
from PedidosInternosDetalle a
join InventarioDetalleLog b on a.idProducto=b.idProducto
join Ubicacion u on b.idUbicacion=u.idUbicacion
where a.idPedidoInterno=@idPedidoInterno 
order by b.idInventarioDetalleLOG
