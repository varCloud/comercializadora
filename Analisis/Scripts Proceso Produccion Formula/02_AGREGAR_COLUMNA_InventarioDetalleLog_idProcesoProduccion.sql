/*
Objetivo	Columna InventarioDetalleLog.idProcesoProduccion (bigint NULL): vincula cada movimiento de
			inventario con la produccion por formula (ProcesoProduccion) que lo origino.
			Sin default ni FK (mismo patron que idPedidoEspecial). Verificado: InventarioDetalleLog no tiene
			triggers y ningun modulo hace INSERT sin lista de columnas.
Idempotente.
*/

-- VERIFICACION PREVIA (triggers: 0 filas; columna: 0 filas la primera vez)
select name from sys.triggers where parent_id = OBJECT_ID('dbo.InventarioDetalleLog')
select name from sys.columns where object_id = OBJECT_ID('dbo.InventarioDetalleLog') and name = 'idProcesoProduccion'
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'idProcesoProduccion' AND Object_ID = Object_ID(N'dbo.InventarioDetalleLog'))
BEGIN
	ALTER TABLE dbo.InventarioDetalleLog ADD idProcesoProduccion bigint NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.extended_properties WHERE major_id = OBJECT_ID(N'dbo.InventarioDetalleLog') AND name = N'MS_Description'
			   AND minor_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.InventarioDetalleLog') AND name = N'idProcesoProduccion'))
BEGIN
	EXEC sys.sp_addextendedproperty @name = N'MS_Description',
		@value = N'Id del registro de produccion por formula (ProcesoProduccion) que origino este movimiento. NULL si no proviene de esa funcion. No confundir con idProcesoProduccionAgranel.',
		@level0type = N'SCHEMA', @level0name = N'dbo',
		@level1type = N'TABLE',  @level1name = N'InventarioDetalleLog',
		@level2type = N'COLUMN', @level2name = N'idProcesoProduccion'
END
GO

-- VERIFICACION POSTERIOR (1 fila con descripcion)
select c.name, t.name tipo, c.is_nullable, ep.value descripcion
from sys.columns c join sys.types t on t.user_type_id = c.user_type_id
left join sys.extended_properties ep on ep.major_id = c.object_id and ep.minor_id = c.column_id and ep.name = 'MS_Description'
where c.object_id = OBJECT_ID('dbo.InventarioDetalleLog') and c.name = 'idProcesoProduccion'
GO
