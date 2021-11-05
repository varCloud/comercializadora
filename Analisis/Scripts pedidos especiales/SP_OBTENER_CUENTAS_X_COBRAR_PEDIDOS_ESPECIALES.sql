IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_OBTENER_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_OBTENER_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta las cuentas por cobrar de pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_OBTENER_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES]
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
					c.idCliente,					
					c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoPaterno nombreCliente,			        
					sum(pe.saldoInicial) montoTotal,
					sum(pe.saldoInicial-pe.saldoActual) montoPagado,
					sum(pe.saldoActual) montoAdeudado					
					into #cuentasPorCobrar
					FROM	
						PedidosEspecialesCuentasPorCobrar pe
						join Clientes c	on pe.idCliente=c.idCliente	
					where saldoActual>0 and c.idCliente=coalesce(@idCliente,c.idCliente)
					group by c.idCliente,c.nombres,c.apellidoPaterno,c.apellidoPaterno
					
					if not exists (select 1 from #cuentasPorCobrar)
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
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DIN�MICA'),
					@error_line = error_line(),
					@mensaje = error_message()
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
           
		    if(@status=200)
				select * from #cuentasPorCobrar
			
					
		end -- reporte de estatus
		

	end  -- principal
