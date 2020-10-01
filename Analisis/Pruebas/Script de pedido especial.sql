
--select * from PedidosInternos where idTipoPedidoInterno = 1 order by idPedidoInterno asc


DECLARE @idPedidoInterno int=225

select @idPedidoInterno=coalesce(@idPedidoInterno,(select max(idPedidoInterno) from pedidosInternos where idTipoPedidoInterno=2))

select 'pedidosInternos',* from pedidosInternos where  idPedidoInterno=@idPedidoInterno
select 'PedidosInternosLog',P.*,EPI.descripcion from PedidosInternosLog  P join CatEstatusPedidoInterno EPI on P.IdEstatusPedidoInterno = EPI.IdEstatusPedidoInterno where P.idPedidoInterno=@idPedidoInterno
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
join InventarioDetalleLog b on a.idProducto=b.idProducto and A.idPedidoInterno = @idPedidoInterno
join Ubicacion u on b.idUbicacion=u.idUbicacion
where a.idPedidoInterno=@idPedidoInterno 
order by b.idInventarioDetalleLOG
