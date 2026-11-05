/*
Objetivo	Tipos de movimiento nuevos en CatTipoMovimientoInventario (id identity; los SPs los
			resuelven por descripcion exacta, no por id).
Idempotente.
*/

-- VERIFICACION PREVIA (0 filas la primera vez)
select * from CatTipoMovimientoInventario where descripcion like '%por proceso de produccion de formula%'
GO

IF NOT EXISTS (SELECT 1 FROM CatTipoMovimientoInventario WHERE descripcion = 'Actualizacion de Inventario(carga de producto terminado por proceso de produccion de formula)')
	INSERT INTO CatTipoMovimientoInventario (descripcion, operacion) VALUES ('Actualizacion de Inventario(carga de producto terminado por proceso de produccion de formula)', 1)
IF NOT EXISTS (SELECT 1 FROM CatTipoMovimientoInventario WHERE descripcion = 'Actualizacion de Inventario(salida de materia prima por proceso de produccion de formula)')
	INSERT INTO CatTipoMovimientoInventario (descripcion, operacion) VALUES ('Actualizacion de Inventario(salida de materia prima por proceso de produccion de formula)', -1)
GO

-- VERIFICACION POSTERIOR (2 filas: operacion 1 y -1)
select * from CatTipoMovimientoInventario where descripcion like '%por proceso de produccion de formula%'
GO
