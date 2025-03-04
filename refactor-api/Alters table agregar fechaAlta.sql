IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'fechaAlta'
          AND Object_ID = Object_ID(N'Usuarios'))
BEGIN
	 print 'entre'
	ALTER TABLE Usuarios ADD fechaAlta datetime default CURRENT_TIMESTAMP

END
GO
