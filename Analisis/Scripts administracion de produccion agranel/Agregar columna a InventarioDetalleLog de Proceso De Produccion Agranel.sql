begin tran
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'idProcesoProduccionAgranel'
          AND Object_ID = Object_ID(N'InventarioDetalleLog'))
BEGIN
    print 'Agregando columnas'
	ALTER TABLE InventarioDetalleLog ADD idProcesoProduccionAgranel bigint null default(0) ;
END
commit tran


select top 10 * from InventarioDetalleLog