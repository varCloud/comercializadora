
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS ( select * from sysobjects where id = object_id(N'diasTranscurridosCredito') and xtype in (N'FN', N'IF', N'TF') )
begin
    drop function diasTranscurridosCredito
end
GO
-- ============================================================================================================================================
-- Description:	Funcion que consulta los dias que han transcurrido desde que se le otorgo el credito mas viejo y este no ha sido liquidado
-- ============================================================================================================================================
create function [dbo].[diasTranscurridosCredito]
(
	@idCliente			int
)
returns int
as
begin
	
	declare	@diasCredito			int,
			@diasTranscurridos		int = cast(0 as int)

	select	@diasCredito = diasCredito
	from	Clientes 
	where	idCliente = @idCliente
	
	select @diasCredito = coalesce(@diasCredito, 0)

	select	@diasTranscurridos = DATEDIFF(day, min(fechaAlta), dbo.FechaActual() )
	from	(
				select	idCuentaPorCobrar, idPedidoEspecial, idCliente, SaldoInicial, saldoActual, fechaAlta
				from	PedidosEspecialesCuentasPorCobrar
				where	idCliente = @idCliente
					and	saldoActual > 0
			)A

	select @diasTranscurridos =coalesce(@diasTranscurridos, 0)

	return @diasTranscurridos

end
go
