--declare
--@idPedidoEspecial bigint = 6437 
--select * from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial
--select * from PedidosEspecialesDetalle  where idPedidoEspecial = @idPedidoEspecial
--select * from PedidosEspecialesMovimientosDeMercancia where idPedidoEspecial = @idPedidoEspecial
--select * from InventarioDetalle where  idProducto  = 744
--select * from InventarioDetalleLog where  idProducto  = 744 order by idInventarioDetalleLOG desc

select * from InventarioDetalle where  idProducto = 3293

select * from CatEstatusPedidoEspecial

select * from PedidosEspecialesDetalle where idPedidoEspecial in 
(select idPedidoEspecial from PedidosEspeciales where idEstatusPedidoEspecial = 3) order by idProducto
and idProducto in (114, 771)


select * from InventarioDetalle ID 
join
	(select idProducto, SUM(cantidadAtendida) cantidadActual from PedidosEspecialesDetalle where idPedidoEspecial in 
	(select idPedidoEspecial from PedidosEspeciales where idEstatusPedidoEspecial = 3) group by idProducto) PE
on ID.idProducto = PE.idProducto where ID.idUbicacion in (3801,3802,3841,3895) and ID.cantidad < PE.cantidadActual

select * from InventarioDetalle where  idProducto  = 736
select top 200 * from InventarioDetalleLog where  idProducto  = 736 order by idInventarioDetalleLOG desc


BEGIN TRAN

update ID
set
	ID.cantidad = PE.cantidadActual
from InventarioDetalle ID 
join
	(select idProducto, SUM(cantidadAtendida) cantidadActual from PedidosEspecialesDetalle where idPedidoEspecial in 
	(select idPedidoEspecial from PedidosEspeciales where idEstatusPedidoEspecial = 3) group by idProducto) PE
on ID.idProducto = PE.idProducto where ID.idUbicacion = 3802 and ID.cantidad < PE.cantidadActual

ROLLBACK TRAN 

select * from Ubicacion where idPiso = 1000
select * from CatPiso


select * from PedidosEspecialesDetalle where idPedidoEspecial in 
	(select idPedidoEspecial from PedidosEspeciales where idEstatusPedidoEspecial = 3) and idProducto = 744

select * from InventarioDetalle where  idProducto  = 736
select top 200 * from InventarioDetalleLog where  idProducto  = 736 order by idInventarioDetalleLOG desc


select * from PedidosEspecialesDetalle where idPedidoEspecial in 
	(select idPedidoEspecial from PedidosEspeciales where idEstatusPedidoEspecial = 3)
	and idProducto = 736

	select * from CatEstatusPedidoEspecial
	select * from PedidosEspeciales where idPedidoEspecial in (6450,6465) 


select * from InventarioDetalle where  idProducto  = 736
select top 200 * from InventarioDetalleLog where  idProducto  = 736 and cantidadActual < 0 order by idInventarioDetalleLOG desc