declare @idPedidoEspecial int=null

--10

set @idPedidoEspecial=COALESCE(@idPedidoEspecial,(select max(idPedidoEspecial) from pedidosespeciales))

select * from PedidosEspeciales where idPedidoEspecial=@idPedidoEspecial

select * from PedidosEspecialesDetalle where idpedidoespecial=@idPedidoEspecial

select * from PedidosEspecialesMovimientosDeMercancia where idPedidoEspecial=@idPedidoEspecial

--select * from Usuarios where idAlmacen=3 and activo=1

--select * from InventarioDetalle where idProducto=94