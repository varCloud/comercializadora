
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ultimoCostoCompra' AND OBJECT_ID = OBJECT_ID(N'VentasDetalle'))
BEGIN
	ALTER TABLE VentasDetalle ADD ultimoCostoCompra money default(0);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ultimoCostoCompra' AND OBJECT_ID = OBJECT_ID(N'complementosDetalle'))
BEGIN
	ALTER TABLE complementosDetalle ADD ultimoCostoCompra money default(0);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ultimoCostoCompra' AND OBJECT_ID = OBJECT_ID(N'DevolucionesDetalle'))
BEGIN
	ALTER TABLE DevolucionesDetalle ADD ultimoCostoCompra money default(0);
END  

/********************************************************************************************************************/
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'fechaVentaDetalle' AND OBJECT_ID = OBJECT_ID(N'VentasDetalle'))
BEGIN
	ALTER TABLE VentasDetalle ADD fechaVentaDetalle datetime default([dbo].FechaActual());
	CREATE INDEX index_VentasDetalle_fechaVentaDetalle  ON VentasDetalle (fechaVentaDetalle);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'fechaComplementoDetalle' AND OBJECT_ID = OBJECT_ID(N'complementosDetalle'))
BEGIN
	ALTER TABLE complementosDetalle ADD fechaComplementoDetalle  datetime default([dbo].FechaActual());
	CREATE INDEX index_complementosDetalle_fechaComplementoDetalle ON complementosDetalle (fechaComplementoDetalle);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'fechaDevolucionDetalle' AND OBJECT_ID = OBJECT_ID(N'DevolucionesDetalle'))
BEGIN
	ALTER TABLE DevolucionesDetalle ADD fechaDevolucionDetalle datetime default([dbo].FechaActual());
	CREATE INDEX index_DevolucionesDetalle_fechaDevolucionDetalle  ON DevolucionesDetalle (fechaDevolucionDetalle);
END  

/**************************UPDATE ULTIMO COSTO DE COMPRA ***********************************************************************************/

update VD set VD.ultimoCostoCompra = P.ultimoCostoCompra from VentasDetalle VD join Productos P on VD.idProducto = P.idProducto

update CD set CD.ultimoCostoCompra = P.ultimoCostoCompra from ComplementosDetalle CD join Productos P on CD.idProducto = P.idProducto

update DD set DD.ultimoCostoCompra = P.ultimoCostoCompra from DevolucionesDetalle DD join Productos P on DD.idProducto = P.idProducto


/**************************UPDATE FECHA ALTA  PARA PODER CALCULAR EL INVENTARIO PROMEDIO DIARIO *********************************************/

update VD set VD.fechaVentaDetalle = V.fechaAlta
from Ventas V join  VentasDetalle VD on V.idVenta  = VD.idVenta 
where VD.idComplementoDetalle = 0

update VD set VD.fechaVentaDetalle = C.fechaAlta
from Ventas V join  VentasDetalle VD on V.idVenta  = VD.idVenta 
join Complementos C on V.idVenta = C.idVenta
join ComplementosDetalle CD on CD.idVenta = VD.idVenta and VD.idComplementoDetalle = CD.idComplementoDetalle
where VD.idComplementoDetalle > 0

update CD set cd.fechaComplementoDetalle = C.fechaAlta 
from  Complementos C join complementosDetalle CD on C.idComplemento = CD.idComplemento

update DD set DD.fechaDevolucionDetalle = D.fechaAlta
from  Devoluciones D join DevolucionesDetalle DD on D.idDevolucion = DD.idDevolucion


/************************ NO TIENEN ASIGNADO ULTIMO COSTO DE COMPRA EN PRODUCTOS ****************************************/
select * from productos where ultimoCostoCompra is null and activo =1

--update VentasDetalle set  fechaVentaDetalle = getdate() where 
--select * from DevolucionesDetalle where idVenta=35724
--select * from complementosDetalle where idVenta=35724
--select ultimoCostoCompra,* from VentasDetalle order by idVentaDetalle desc
--select * from Productos
