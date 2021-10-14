
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'latitud' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD latitud varchar(255);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'longitud' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD longitud varchar(255);
END  

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'telefonoContacto' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD telefonoContacto varchar(15);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'correoContacto' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD correoContacto varchar(100);
END  

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'nombreCompletoContacto' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD nombreCompletoContacto varchar(255);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'diaCredito' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD diaCredito int default(0);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'montoMaximoCredito' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD montoMaximoCredito float default(0.0);
END  



IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'usarDatosCliente' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD usarDatosCliente int default(0);
END  

