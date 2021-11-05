IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_OBTENER_DETALLE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_OBTENER_DETALLE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta el detalle de cuentas por cobrar
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_OBTENER_DETALLE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES]
@idCliente int=null
as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''
			end  --declaraciones 

			begin -- principal
				
			
					SELECT						
					pe.idPedidoEspecial,
					pe.SaldoInicial,
					pe.saldoActual,
					case when max(a.fechaAlta) is null then 'N/A' else CONVERT(VARCHAR(10),max(a.fechaAlta),103) + ' ' + CONVERT(VARCHAR(20),max(a.fechaAlta),114) end fechaUltimoAbono
					into #cuentasPorCobrarDetalle
					FROM	
						PedidosEspecialesCuentasPorCobrar pe
					left join PedidosEspecialesAbonosCuentasPorCobrar a on pe.idCuentaPorCobrar=a.idCuentaPorCobrar
					where saldoActual>0 and pe.idCliente=coalesce(@idCliente,pe.idCliente)
					group by pe.idPedidoEspecial, pe.SaldoInicial,pe.saldoActual,pe.fechaAlta
					order by pe.fechaAlta
					
					
					if not exists (select 1 from #cuentasPorCobrarDetalle)
					begin
						select	
								@status = -1,
								@mensaje = 'No se encontraron cuentas por cobrar'
					end


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
           
		    if(@status=200)
				select * from #cuentasPorCobrarDetalle
			
					
		end -- reporte de estatus
		

	end  -- principal
