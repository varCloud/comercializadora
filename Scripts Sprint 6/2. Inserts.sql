
UPDATE LineaProducto SET descripcion='Linea ENVASE' where idLineaProducto=19

IF NOT EXISTS(SELECT 1 FROM LineaProducto WHERE descripcion like 'LINEA LIQUIDOS')
INSERT INTO LineaProducto(descripcion,activo) values('Linea LIQUIDOS',1)

select * from lineaProducto















