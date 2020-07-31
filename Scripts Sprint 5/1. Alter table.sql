	
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME='CatEstatusProductoVenta')
	DROP TABLE CatEstatusProductoVenta

CREATE TABLE CatEstatusProductoVenta(
idEstatusProductoVenta int primary key identity(1,1),
descripcion varchar(300)
)

INSERT INTO CatEstatusProductoVenta (descripcion) 
values('Devuelto'),('Complemento / Agregado')


IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME='CatEstatusProductoCompra')
	DROP TABLE CatEstatusProductoCompra

CREATE TABLE CatEstatusProductoCompra(
idEstatusProductoCompra int,
descripcion varchar(300)
)



INSERT INTO CatEstatusProductoCompra
values(1,'Cantidad Correcta'),(2,'Cantidad Mayor a la solicitada'),(3,'Cantidad Menor a la solicitada')
,(4,'Devolucion paracial del producto'),(5,'Devolucion completa del producto')


IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='idEstatusProductoCompra' and TABLE_NAME='ComprasDetalle')
ALTER TABLE ComprasDetalle add idEstatusProductoCompra int null

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='cantidadRecibida' and TABLE_NAME='ComprasDetalle')
ALTER TABLE ComprasDetalle add cantidadRecibida int null

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='observaciones' and TABLE_NAME='ComprasDetalle')
ALTER TABLE ComprasDetalle add observaciones VARCHAR(500) null

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='fechaRecibio' and TABLE_NAME='ComprasDetalle')
ALTER TABLE ComprasDetalle add fechaRecibio datetime null

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='idUsuarioRecibio' and TABLE_NAME='ComprasDetalle')
ALTER TABLE ComprasDetalle add idUsuarioRecibio int null

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='cantidadDevuelta' and TABLE_NAME='ComprasDetalle')
ALTER TABLE ComprasDetalle add cantidadDevuelta int null

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='observaciones' and TABLE_NAME='Compras')
ALTER TABLE Compras add observaciones VARCHAR(500) null


IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='idEstatusProductoVenta' and TABLE_NAME='VentasDetalle')
ALTER TABLE VentasDetalle add idEstatusProductoVenta int  null

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='observaciones' and TABLE_NAME='Ventas')
ALTER TABLE Ventas add observaciones varchar(500)

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='idCompra' and TABLE_NAME='InventarioDetallelOG')
ALTER TABLE InventarioDetallelOG add idCompra int null



select * from Compras
select * from ComprasDetalle

if not exists (Select 1 from CatStatusCompra where idStatusCompra = 5)
	insert into CatStatusCompra ( idStatusCompra, descripcion) values (5,'Devolución')

if not exists (Select 1 from CatTipoMovimientoInventario where idTipoMovInventario = 12)
	insert into CatTipoMovimientoInventario ( descripcion , operacion) values ('Carga de producto a inventario por compra recibida' , 1)

-- delete from CatStatusCompra where idStatusCompra is  null
-- select * from CatStatusCompra
-- select * from CatEstatusProductoCompra


