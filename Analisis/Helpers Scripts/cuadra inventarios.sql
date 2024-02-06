


/*CUADRA INVENTARIOS*/
begin tran
select ID.idProducto, ID.cantidad as CantidadInventarioDetalle , IG.cantidad as CantidadInventarioGeneral , IG.fechaUltimaActualizacion from  (select idProducto , sum(cantidad) cantidad from InventarioDetalle  group by idProducto) ID
join InventarioGeneral IG on ID.idProducto = IG.idProducto and IG.cantidad <> ID.cantidad

	update IG
	set IG.cantidad = ID.cantidad
	from  (select idProducto , sum(cantidad) cantidad from InventarioDetalle  group by idProducto) ID
	join InventarioGeneral IG on ID.idProducto = IG.idProducto and IG.cantidad <> ID.cantidad


select ID.* , IG.cantidad catidadInventarioGeneral , IG.fechaUltimaActualizacion from  (select idProducto , sum(cantidad) cantidad from InventarioDetalle  group by idProducto) ID
join InventarioGeneral IG on ID.idProducto = IG.idProducto and IG.cantidad <> ID.cantidad
rollback tran 

select * from InventarioDetalle where cantidad < 0 
select * from CatPasillo
select * from Ubicacion where idUbicacion =3802
select * from InventarioDetalle where idProducto = 84

select * from PedidosEspeciales where  idPedidoEspecial = 13038
select * from InventarioDetalleLog where  idProducto = 84 and idPedidoEspecial = 13038 order by fechaAlta desc
select * from InventarioDetalleLog where  idProducto = 84 order by fechaAlta desc

select  * from catEstatusPedidoEspecial

select * from InventarioDetalleLog

select * from Productos where idProducto = 84