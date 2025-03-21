USE [DB_A57E86_comercializadora]
GO
/****** Object:  UserDefinedFunction [dbo].[FechaActual]    Script Date: 3/22/2023 10:15:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[FechaActual]()
RETURNS datetime
BEGIN
/*
A continuación, declaramos una variable smalldatetime con '03 / 01 / "más el año o" 11/01 / más el año como fecha, 
ya que sabemos que el horario de verano siempre comienza el segundo domingo de marzo y termina el el primer domingo de noviembre:
*/
 declare 
	@Date datetime = getdate()
	,@DLSStart smalldatetime
	, @DLSEnd smalldatetime
	, @DLSActive tinyint -- SI ES HORARIO DE VERANO
	
	set @DLSStart=(select dbo.fn_GetDaylightSavingsTimeStart(convert(varchar,datepart(year,getdate()))))
	set @DLSEnd = (select dbo.fn_GetDaylightSavingsTimeEnd(convert(varchar,datepart(year,getdate()))))

	if @Date between @DLSStart and @DLSEnd
	begin
		 --print 'Esto entre '+cast(@Date as varchar)+' ese rango de fechas'
		set @DLSActive = 1
		SET @date = DATEADD(hour, 1 ,getdate()) ;
	end
	else
	begin
		--print 'no estoy en ese rango de fechas'
		set @DLSActive = 0
		SET @date = DATEADD(hour, 2 ,getdate()) ;
	end


 RETURN @date

END



--ALTER TABLE ComplementosDetalle DROP CONSTRAINT DF_ComplementosDetalle_fechaComplementoDetalle;
--ALTER TABLE ComplementosDetalle ADD CONSTRAINT DF_ComplementosDetalle_fechaComplementoDetalle DEFAULT dbo.fechaActual() FOR fechaComplementoDetalle;

--ALTER TABLE ventasDetalle DROP CONSTRAINT DF_VentasDetalle_fechaVentaDetalle;
--ALTER TABLE ventasDetalle ADD CONSTRAINT DF_VentasDetalle_fechaVentaDetalle DEFAULT dbo.fechaActual() FOR fechaVentaDetalle;

--ALTER TABLE ComprasDetalle DROP CONSTRAINT DF_ComprasDetalle_fechaAlta;
--ALTER TABLE ComprasDetalle ADD CONSTRAINT DF_ComprasDetalle_fechaAlta DEFAULT dbo.fechaActual() FOR fechaAlta;

--ALTER TABLE DevolucionesDetalle DROP CONSTRAINT DF_DevolucionesDetalle_fechaDevolucionDetalle;
--ALTER TABLE DevolucionesDetalle ADD CONSTRAINT DF_DevolucionesDetalle_fechaDevolucionDetalle DEFAULT dbo.fechaActual() FOR fechaDevolucionDetalle;

