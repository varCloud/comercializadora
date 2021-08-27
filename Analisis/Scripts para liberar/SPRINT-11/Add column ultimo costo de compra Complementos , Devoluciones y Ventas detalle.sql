
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

--select * from DevolucionesDetalle where idVenta=35724

--select * from complementosDetalle where idVenta=35724

--select ultimoCostoCompra,* from VentasDetalle order by idVentaDetalle desc

--select * from Productos
