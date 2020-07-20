
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME='CatEstatusProductoCompra')
	DROP TABLE CatEstatusProductoCompra

CREATE TABLE CatEstatusProductoCompra(
idEstatusProductoCompra int,
descripcion varchar(300)
)

INSERT INTO CatEstatusProductoCompra
values(1,'Cantidad Correcta'),(2,'Cantidad Mayor a la solicitada'),(3,'Cantidad Menor a la solicitada')


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

IF NOT EXISTS(SELECT 1 from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME='observaciones' and TABLE_NAME='Compras')
ALTER TABLE Compras add observaciones VARCHAR(500) null

select * from Compras
select * from ComprasDetalle

