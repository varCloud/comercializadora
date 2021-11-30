IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_CONSULTA_INFO_CIERRE_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_CONSULTA_INFO_CIERRE_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consultar informaciòn de cierre de pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_CONSULTA_INFO_CIERRE_PEDIDOS_ESPECIALES]

@idEstacion			int,
@idUsuario			int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@montoApertura							money=0,
						@totalPedidosEspeciales				    int = 0,					
						@montoPedidosEspecialesDelDia			money = 0,						
						@efectivoDisponible						money = 0,					
						@montoIngresosEfectivo					money=0,
						@hoy									datetime,
						@retirosExcesoEfectivo					money=0						
			end  --declaraciones 

			begin -- principal
				
				select @hoy = cast(dbo.FechaActual() as date)
			
				select	@montoApertura = monto 
				from	PedidosEspecialesIngresosEfectivo 
				where	idUsuario = @idUsuario 
					and idTipoIngreso=1 
					and cast(fechaAlta as date) = cast(dbo.FechaActual() as date)

				select	@montoIngresosEfectivo=sum(monto) 
				from	PedidosEspecialesIngresosEfectivo 
				where	idUsuario = @idUsuario 
					and idTipoIngreso=2 
					and cast(fechaAlta as date) = cast(dbo.FechaActual() as date)

				select	v.*
				into	#pedidos_dia
				from	PedidosEspeciales v 
				where	idUsuarioEntrega = @idUsuario
					and cast(fechaAlta as date) = cast(dbo.FechaActual() as date)
					and idEstatusPedidoEspecial in (4,5,6,7)

				select	@totalPedidosEspeciales=count(1)
				from	#pedidos_dia

				select @montoPedidosEspecialesDelDia = coalesce(sum((montoTotal)),0)
				from	#pedidos_dia 

				select	@retirosExcesoEfectivo = SUM(montoRetiro) 
				from	PedidosEspecialesRetirosExcesoEfectivo
				where	idUsuario = @idUsuario
					and cast(fechaAlta as date) = cast(dbo.FechaActual() as date)

				
				select @efectivoDisponible = dbo.redondear( (coalesce(@montoPedidosEspecialesDelDia,0) + coalesce(@montoIngresosEfectivo,0) + coalesce(@montoApertura,0)) - coalesce(@retirosExcesoEfectivo,0))					

					

			end -- principal

		end try

		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@mensaje = error_message()
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
           
		    select	coalesce(@montoApertura,0) as montoApertura,
					coalesce(@montoIngresosEfectivo,0) as montoIngresosEfectivo,					
					coalesce(@totalPedidosEspeciales,0) as totalPedidosEspeciales,					
					coalesce(@montoPedidosEspecialesDelDia,0) as montoPedidosEspecialesDelDia,
					coalesce(@efectivoDisponible,0) as efectivoDisponible,
					coalesce(@retirosExcesoEfectivo,0) as retirosExcesoEfectivo
					
		end -- reporte de estatus
		

	end  -- principal
