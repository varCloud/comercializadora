begin tran

select count(*) from  InventarioGeneral I join (
	select  idProducto, sum(cantidad) cantidadActual from InventarioDetalle group by idProducto
)b on  I.idProducto = b.idProducto
where I.cantidad <> b.cantidadActual

	update I set
		I.cantidad  = b.cantidadActual
	from InventarioGeneral I join (
		select  idProducto, sum(cantidad) cantidadActual from InventarioDetalle group by idProducto
	)b on  I.idProducto = b.idProducto
	where I.cantidad <> b.cantidadActual

select count(*) from  InventarioGeneral I join (
	select  idProducto, sum(cantidad) cantidadActual from InventarioDetalle group by idProducto
)b on  I.idProducto = b.idProducto
where I.cantidad <> b.cantidadActual

rollback tran