IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_CONSULTA_TICKETS_PEDIDO_ESPECIAL')
DROP PROCEDURE SP_CONSULTA_TICKETS_PEDIDO_ESPECIAL
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta los tickets de pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_CONSULTA_TICKETS_PEDIDO_ESPECIAL]

@idPedidoEspecial bigint 

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',						 
						@diasDevoluciones int=0,
						@fechaActual date

			end  --declaraciones 

			begin -- principal			
				
			
					select t.*,
					CONVERT(VARCHAR(10),t.fechaAlta,103) + ' ' + CONVERT(VARCHAR(20),t.fechaAlta,114) fechaTicket,
					ct.tipoTicket
					into #TicketsPedidosEspeciales
					from TicketsPedidosEspeciales t 
					join CatTipoTicketPedidosEspeciales ct on t.idTipoTicketPedidoEspecial=ct.idTipoTicketPedidoEspecial
					where idPedidoEspecial=@idPedidoEspecial
					
					

					if not exists (select 1 from #TicketsPedidosEspeciales)
					begin
						select	
								@status = -1,
								@mensaje = 'No se encontraron tickets para este pedido especial'
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
				select * from #TicketsPedidosEspeciales order by fechaAlta desc
			
					
		end -- reporte de estatus
		

	end  -- principal

go

grant exec on SP_CONSULTA_TICKETS_PEDIDO_ESPECIAL to public
go