--declare
--@idPedidoEspecial bigint = 6437 
--select * from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial
--select * from PedidosEspecialesDetalle  where idPedidoEspecial = @idPedidoEspecial
--select * from PedidosEspecialesMovimientosDeMercancia where idPedidoEspecial = @idPedidoEspecial
--select * from InventarioDetalle where  idProducto  = 744
--select * from InventarioDetalleLog where  idProducto  = 744 order by idInventarioDetalleLOG desc

select * from InventarioDetalle where  idProducto = 3293

select * from CatEstatusPedidoEspecial
select * from CatEstatusPedidoEspecialDetalle
select * from PedidosEspecialesDetalle where idPedidoEspecial in 
(select idPedidoEspecial from PedidosEspeciales /*where idEstatusPedidoEspecial = 3*/)
and cantidad <> cantidadAtendida
order by fechaAlta desc


select * from PedidosEspeciales where esRevisionHandHeld = 1 order by fechaAlta desc


SELECT ID.idProducto,ID.cantidad, PED.cantidadDeberiaDeEstar, U.idUbicacion
FROM 
	InventarioDetalle ID 
JOIN
	(select idProducto, SUM(cantidadAtendida) cantidadDeberiaDeEstar , idAlmacenOrigen  from PedidosEspecialesDetalle where idPedidoEspecial in 
	(select idPedidoEspecial from PedidosEspeciales where idEstatusPedidoEspecial = 3) 
	group by idProducto,idAlmacenOrigen) PED on ID.idProducto = PED.idProducto
JOIN
	Ubicacion U on PED.idAlmacenOrigen = U.idAlmacen and U.idUbicacion  in (3801,3802,3841,3895)
WHERE
	ID.cantidad < PED.cantidadDeberiaDeEstar 
	AND U.idUbicacion = ID.idUbicacion

select * from CatTipoMovimientoInventario

select * from InventarioDetalleLog where idProducto = 3295 and idUbicacion = 3802 order by idInventarioDetalleLog desc

select * from InventarioDetalle where  idProducto = 320
select * from InventarioDetalle where cantidad < 0
update InventarioDetalle set cantidad = 0  where idUbicacion = 3802  and idProducto = 3295

update InventarioGeneral set cantidad = 0  where  idProducto = 320

select * from CatPiso where idPiso = 1003
select top 200 U.idPiso, MI.idTipoMovInventario, MI.descripcion, U.idAlmacen, IDL.*
	FROM InventarioDetalleLog IDL
	join CatTipoMovimientoInventario MI on MI.idTipoMovInventario = IDL.idTipoMovInventario
	join Ubicacion U on IDL.idUbicacion = U.idUbicacion
where  idProducto  = 734 order by idInventarioDetalleLOG desc

select * from CatTipoMovimientoInventario
select * from CatEstatusPedidoInterno
select * from PedidosInternos where IdEstatusPedidoInterno = 2
select * from PedidosInternosDetalle where idPedidoInterno in (
select idPedidoInterno from PedidosInternos where IdEstatusPedidoInterno = 2
)
select * from CatTipoMovimientoInventario
select top 200 * from InventarioDetalleLog where  idPedidoInterno = 38158 order by idInventarioDetalleLOG desc
select top 2000 * from InventarioDetalleLog where  idProducto = 108 and idUbicacion = 3802  order by idInventarioDetalleLOG  desc
select top 2000 * from InventarioDetalleLog where  idProducto = 108 and idUbicacion = 3802  order by idInventarioDetalleLOG  desc

select top 2000 * from InventarioDetalleLog where  idProducto = 108 and idUbicacion = 3802 and idPedidoEspecial = 6558  order by idInventarioDetalleLOG  desc

select top 2000 * from InventarioDetalleLog where  idProducto = 108 and idUbicacion = 3802 and idTipoMovInventario not in (1,18)

select * from CatTipoMovimientoInventario
select * from pedidosEspecialesDetalle where idProducto = 581 order by fechaAlta desc
select * from InventarioDetalle  where idProducto = 581

select * from PedidosInternos where idPedidoInterno = 38158
select * from PedidosInternosDetalle where idPedidoInterno = 38158
select * from PedidosInternosDetalle where idProducto in(734,3404) order by fechaAlta desc

BEGIN TRAN

	update ID
	SET
		ID.cantidad = PED.cantidadDeberiaDeEstar
	FROM
		InventarioDetalle ID 
	join
		(select idProducto, SUM(cantidadAtendida) cantidadDeberiaDeEstar , idAlmacenOrigen  from PedidosEspecialesDetalle where idPedidoEspecial in 
		(select idPedidoEspecial from PedidosEspeciales where idEstatusPedidoEspecial = 3) 
		group by idProducto,idAlmacenOrigen) PED on ID.idProducto = PED.idProducto
	join
		Ubicacion U on PED.idAlmacenOrigen = U.idAlmacen and U.idUbicacion  in (3801,3802,3841,3895)
	WHERE
		ID.cantidad < PED.cantidadDeberiaDeEstar 
		AND U.idUbicacion = ID.idUbicacion

ROLLBACK TRAN 

select * from Ubicacion where idPiso = 1003
select * from Ubicacion where idUbicacion = 3861
select * from CatPiso


select * from PedidosEspecialesDetalle where idPedidoEspecial in 
	(select idPedidoEspecial from PedidosEspeciales where idEstatusPedidoEspecial = 3) and idProducto = 744

select * from InventarioDetalle where  idProducto  = 736
select top 200 * from InventarioDetalleLog where  idProducto  = 736 order by idInventarioDetalleLOG desc


select * from PedidosEspecialesDetalle where idPedidoEspecial =6506

	select * from CatEstatusPedidoEspecial
	select * from PedidosEspeciales where idPedidoEspecial in (6450,6465) 


select * from InventarioDetalle where  idProducto  = 334
select top 200 * from InventarioDetalleLog where  idProducto  = 876  order by idInventarioDetalleLOG desc