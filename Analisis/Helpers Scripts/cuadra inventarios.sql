

select * from VentasDetalle where idVenta = 168324
select * from CatEstatusProductoVenta

select * from InventarioDetalle where idUbicacion = 3860 and cantidad < 0

select * from InventarioDetalle where idUbicacion = 3860 and cantidad > 0

select * from InventarioDetalle where idUbicacion = 3861 and cantidad > 0
select * from InventarioDetalle where idUbicacion = 3861 and cantidad < 0


select * from PedidosInternos order by fechaAlta desc

select * from InventarioGeneral where contador in (799,1898)

select * from InventarioDetalle where cantidad < 0
-- update  InventarioGeneral set cantidad = 0 where contador in (799,1898)


select * from InventarioDetalle where idProducto  = 1552

select * from InventarioDetalle where idProducto  = 2586


select * from Ventas order by fechaAlta desc

select * from InventarioDetalle where idUbicacion = 3053
select * from InventarioDetalle where idUbicacion = 3861
select * from Ubicacion where idUbicacion = 3860

select * from InventarioDetalle where idProducto = 3053
select * from InventarioDetalleLog where idProducto = 3053 order by fechaAlta desc


select * from PedidosInternos order by fechaAlta desc


select * from InventarioDetalle where idProducto = 3053
select ID.* , IG.cantidad canitdadGeneral , IG.fechaUltimaActualizacion from  (select idProducto , sum(cantidad) cantidad from InventarioDetalle  group by idProducto) ID
join InventarioGeneral IG on ID.idProducto = IG.idProducto and IG.cantidad <> ID.cantidad

select * from InventarioDetalle where idProducto = 2586 order by idUbicacion desc
select IDL.*,  T.descripcion from InventarioDetalleLog IDL join CatTipoMovimientoInventario T on IDL.idTipoMovInventario = T.idTipoMovInventario
where idProducto = 2586 and idUbicacion =3696 order by  fechaAlta desc

select * from Ubicacion where idUbicacion = 3696
select * from PedidosInternos where idPedidoInterno = 24257
select * from PedidosInternosDetalle where idPedidoInterno = 24257
select * from InventarioDetalleLog where idPedidoInterno = 24257 

select MAX(idInventarioDetalleLOG) , idUbicacion from InventarioDetalleLog where idPedidoInterno = 24257 group by idUbicacion ,idProducto

select MAX(idInventarioDetalleLOG) , idUbicacion from InventarioDetalleLog where idPedidoInterno = 24257 group by idUbicacion ,idProducto

/*CUADRA INVENTARIOS*/
begin tran
select ID.* , IG.cantidad , IG.fechaUltimaActualizacion from  (select idProducto , sum(cantidad) cantidad from InventarioDetalle  group by idProducto) ID
join InventarioGeneral IG on ID.idProducto = IG.idProducto and IG.cantidad <> ID.cantidad

	update IG
	set IG.cantidad = ID.cantidad
	from  (select idProducto , sum(cantidad) cantidad from InventarioDetalle  group by idProducto) ID
	join InventarioGeneral IG on ID.idProducto = IG.idProducto and IG.cantidad <> ID.cantidad

select ID.* , IG.cantidad catidadInventarioGeneral , IG.fechaUltimaActualizacion from  (select idProducto , sum(cantidad) cantidad from InventarioDetalle  group by idProducto) ID
join InventarioGeneral IG on ID.idProducto = IG.idProducto and IG.cantidad <> ID.cantidad
rollback tran 