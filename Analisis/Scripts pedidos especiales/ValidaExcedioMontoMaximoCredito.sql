
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( select * from sysobjects where id = object_id(N'ValidaExcedioMontoMaximoCredito') and xtype in (N'FN', N'IF', N'TF') )
begin
    drop function ValidaExcedioMontoMaximoCredito
end
GO
-- =============================================
-- Description:	Funcion que valida si ha excedido el monto maximo del credito
-- =============================================
create function [dbo].[ValidaExcedioMontoMaximoCredito]
(
	@idCliente			int,
	@monto				float
)
returns bit
as
begin
	
	declare	@montoMaximoCredito		float,
			@adeudo					float,
			@excedioLimite			bit = cast(0 as bit)

	select	@montoMaximoCredito = montoMaximoCredito 
	from	Clientes 
	where	idCliente = @idCliente

	select @montoMaximoCredito = coalesce(@montoMaximoCredito, 0)

	select	@adeudo = sum(saldoActual) 
	from	PedidosEspecialesCuentasPorCobrar
	where	idCliente = @idCliente

	select @adeudo = coalesce(@adeudo, 0)

	if ( ( @monto + @adeudo ) > @montoMaximoCredito )
		begin
			select @excedioLimite = cast(1 as bit)
		end

	return @excedioLimite

end
go