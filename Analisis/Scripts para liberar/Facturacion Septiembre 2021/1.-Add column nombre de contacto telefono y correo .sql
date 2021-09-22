
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'esPersonaMoral' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD esPersonaMoral int default(0);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'nombreContacto' AND OBJECT_ID = OBJECT_ID(N'Clientes'))
BEGIN
	ALTER TABLE Clientes ADD nombreContacto varchar(250) default('');
END  

ALTER TABLE Clientes ALTER COLUMN nombres VARCHAR (250);
------------------------------------------------------------------------------------------------

------------------------------DevolucionesDetalle----------------------------------------------
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'cantidadActualInvGeneral' AND OBJECT_ID = OBJECT_ID(N'DevolucionesDetalle'))
BEGIN
	ALTER TABLE DevolucionesDetalle ADD cantidadActualInvGeneral float default(0);
END  


IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'cantidadDespuesOperacionInvGeneral' AND OBJECT_ID = OBJECT_ID(N'DevolucionesDetalle'))
BEGIN
	ALTER TABLE DevolucionesDetalle ADD cantidadDespuesOperacionInvGeneral float default(0);
END  

ALTER TABLE Clientes ALTER COLUMN nombres VARCHAR (250);

