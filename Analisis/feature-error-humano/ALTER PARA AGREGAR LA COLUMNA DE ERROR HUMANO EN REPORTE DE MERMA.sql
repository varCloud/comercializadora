IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'errorHumano'
          AND Object_ID = Object_ID(N'ReporteMerma'))
BEGIN
	 print 'entre'
	ALTER TABLE ReporteMerma ADD errorHumano INT null

END
GO
