--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_INFO_CIERRE_PEDIDOS_ESPECIALES]    Script Date: 09/02/2022 22:19:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consultar informaciòn de cierre de pedidos especiales
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_INFO_CIERRE_PEDIDOS_ESPECIALES]

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
						@montoDevoluciones						money=0,
						@montoAbonos							money=0,
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

				select @montoPedidosEspecialesDelDia=sum(t.montoTotal),@totalPedidosEspeciales=count(1)
				from TicketsPedidosEspeciales t
				join PedidosEspeciales p on t.idPedidoEspecial=p.idPedidoEspecial
				where cast(t.fechaAlta as date)=cast(dbo.FechaActual() as date) and t.idUsuario=@idUsuario
				and idFactFormaPago=1 and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1
				
				select @montoDevoluciones=sum(t.montoTotal)
				from TicketsPedidosEspeciales t
				join PedidosEspeciales p on t.idPedidoEspecial=p.idPedidoEspecial
				where cast(t.fechaAlta as date)=cast(dbo.FechaActual() as date) and t.idUsuario=@idUsuario
				and idTipoTicketPedidoEspecial=2

				select @montoAbonos=sum(montoTotal)
				from [dbo].[PedidosEspecialesAbonoClientes]
				where cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idUsuario=@idUsuario
				and activo=1 and idFactFormaPago=1

				select	@retirosExcesoEfectivo = SUM(montoRetiro) 
				from	PedidosEspecialesRetirosExcesoEfectivo
				where	idUsuario = @idUsuario
					and cast(fechaAlta as date) = cast(dbo.FechaActual() as date)

				
				select @efectivoDisponible = dbo.redondear( (coalesce(@montoPedidosEspecialesDelDia,0) + coalesce(@montoIngresosEfectivo,0) + coalesce(@montoApertura,0)+  coalesce(@montoAbonos,0)) - (coalesce(@retirosExcesoEfectivo,0) + coalesce(@montoDevoluciones,0)))					

					

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
