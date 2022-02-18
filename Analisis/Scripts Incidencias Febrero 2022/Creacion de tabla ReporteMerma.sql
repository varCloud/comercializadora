IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'ReporteMerma')
BEGIN
    DROP TABLE ReporteMerma
END
GO

CREATE TABLE ReporteMerma(
idReporteMerma bigInt identity(1,1), 
idProducto bigInt,
inventarioFinalMesAnt float default 0,
totalCompras float default 0,
inventarioSistema float,
merma float,
porcMerma float,
ultCostoCompra money,
costoMerma money,
UltimoDiaMesCalculo date,
UltimoDiaMesAnterior date,
fechaAlta datetime
)