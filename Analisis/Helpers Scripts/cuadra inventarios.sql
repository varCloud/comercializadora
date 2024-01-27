


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