IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'errorHumano'
          AND Object_ID = Object_ID(N'AjusteInventarioFisico'))
BEGIN
	 print 'entre'
	ALTER TABLE AjusteInventarioFisico ADD errorHumano INT null

END
GO
