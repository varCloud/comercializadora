


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'idUnidadMedidad'
          AND Object_ID = Object_ID(N'ProductosEnvasadosXAgranel'))
BEGIN
	 print 'entre'
	ALTER TABLE ProductosEnvasadosXAgranel ADD idUnidadMedidad INT null

END
GO


	UPDATE P
	set P.idUnidadMedidad = C.idUnidadMedida
	from ProductosEnvasadosXAgranel P join CatUnidadMedida C on UPPER(REPLACE(P.unidadMedidad,'r',''))  = UPPER(C.unidadSAT)
	select * from  ProductosEnvasadosXAgranel
	select  * from CatUnidadMedida 


