
select * from productos where descripcion like '%FIBRA AJAX EC%'
declare
@idProducto int = 581


select sum(cantidad) cantidadInventarioDetalle from InventarioDetalle where idProducto = @idProducto 
select * from InventarioGeneral where idProducto =  @idProducto 


select I.*, U.idAlmacen , U.idPasillo,U.idPiso,U.idRaq from InventarioDetalle I join Ubicacion U on I.idUbicacion = U.idUbicacion
where 
I.idProducto = @idProducto 
and U.idAlmacen = 4
order by fechaActualizacion desc
select 'idInventarioDetalleLOG',* from InventarioDetalleLog where idProducto = @idProducto order  by idInventarioDetalleLOG desc
select'idInventarioGeneralLog', * from InventarioGeneralLog  where idProducto = @idProducto order  by idInventarioGeneralLog desc
select * from InventarioGeneral
where idProducto = @idProducto

--select * from CatEstatusProductoVenta
select * from VentasDetalle where idVenta=46333
select * from ComplementosDetalle where idVenta=46333
select * from DevolucionesDetalle where idVenta = 46333

