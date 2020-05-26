declare @idPedidoInterno int=null

select 'PedidosInternos',e.descripcion statusPedido,* from PedidosInternos p
join CatEstatusPedidoInterno e on p.IdEstatusPedidoInterno=e.IdEstatusPedidoInterno
where idPedidoInterno=coalesce(@idPedidoInterno,idPedidoInterno)
select * from PedidosInternosDetalle where idPedidoInterno=coalesce(@idPedidoInterno,idPedidoInterno)
select * from PedidosInternosLog where idPedidoInterno=coalesce(@idPedidoInterno,idPedidoInterno)
select * from MovimientosDeMercancia where idPedidoInterno=coalesce(@idPedidoInterno,idPedidoInterno)

