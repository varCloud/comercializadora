
UPDATE LineaProducto SET descripcion='Linea ENVASE' where idLineaProducto=19

IF NOT EXISTS(SELECT 1 FROM LineaProducto WHERE descripcion like 'Linea LIQUIDOS')
INSERT INTO LineaProducto(descripcion,activo) values('Linea LIQUIDOS',1)

IF NOT EXISTS (SELECT 1 FROM catestatusPedidoInterno WHERE descripcion like 'Pedido Vendido')
INSERT INTO catestatusPedidoInterno(descripcion) values ('Pedido Vendido')

select * from lineaProducto
select * from catestatusPedidoInterno















