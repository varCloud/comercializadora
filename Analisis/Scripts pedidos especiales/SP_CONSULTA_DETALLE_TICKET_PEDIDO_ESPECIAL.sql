IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_CONSULTA_DETALLE_TICKET_PEDIDO_ESPECIAL')
DROP PROCEDURE SP_CONSULTA_DETALLE_TICKET_PEDIDO_ESPECIAL
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta la informaciòn del ticket de pedido especial
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_CONSULTA_DETALLE_TICKET_PEDIDO_ESPECIAL]

@idPedidoEspecial bigint,
@idTipoTicketPedidoEspecial int,
@idTicketPedidoEspecial bigint=null

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
			
			       if(@idTicketPedidoEspecial is null and @idTipoTicketPedidoEspecial in (1,2))
				     select @idTicketPedidoEspecial=max(idTicketPedidoEspecial) from TicketsPedidosEspeciales where idPedidoEspecial=@idPedidoEspecial and idTipoTicketPedidoEspecial=@idTipoTicketPedidoEspecial
				
				  if (coalesce(@idTicketPedidoEspecial,0)=0) 
				  begin
						select	
								@status = -1,
								@mensaje = 'No existe el ticket del pedido especial'
				  end
			
				if(@idTipoTicketPedidoEspecial in (1,2)) --si el ticket es original o por devoluciones se obtiene la informaciòn de la tabla TicketsPedidosEspecialesDetalle
				begin

				select	ROW_NUMBER() OVER(ORDER BY tp.idPedidoEspecial DESC) AS contador,	
						tp.idPedidoEspecial, ped.idProducto, ped.cantidad, ped.monto as monto, ISNULL(ped.montoIVA,0) montoIVA,
						ISNULL(ped.montoComision,0) montoComisionBancaria,
						coalesce (((ped.precioIndividual - ped.precioVenta) * ped.cantidad ) , 0 )as ahorro , 
						pro.descripcion as descProducto, p.idCliente, 
						case
							when c.nombres is null then 'PÚBLICO EN GENERAL' 
							else c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno
						end as nombreCliente,
						u.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
						ped.cantidadActualInvGeneral, ped.cantidadAnteriorInvGeneral, tp.fechaAlta, ped.precioVenta, ped.idPedidoEspecialDetalle,
						p.idFactFormaPago as formaPago, formaPago.descripcion as descFormaPago, p.codigoBarras, p.montoPagado,
						tp.idTipoTicketPedidoEspecial,CONVERT(VARCHAR(10),tp.fechaAlta,103) fechaTicket, CONVERT(VARCHAR(20),tp.fechaAlta,114) horaTicket
						into #TicketsPedidosEspecialesDetalle
				from	TicketsPedidosEspeciales tp 
							inner join TicketsPedidosEspecialesDetalle ped
								on tp.idTicketPedidoEspecial = ped.idTicketPedidoEspecial
							inner join pedidosespeciales p on tp.idpedidoespecial=p.idpedidoespecial 
							left join Clientes c
								on c.idCliente = p.idCliente
							inner join Usuarios u
								on u.idUsuario = tp.idUsuario
							inner join Productos pro
								on pro.idProducto = ped.idProducto
							inner join FactCatFormaPago formaPago
								on p.idFactFormaPago = formaPago.id
				where	tp.idTicketPedidoEspecial=@idTicketPedidoEspecial
					and	ped.cantidad > 0
				order by ped.idTicketPedidoEspecialDetalle

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
				select * from #TicketsPedidosEspecialesDetalle
			
					
		end -- reporte de estatus
		

	end  -- principal

go

grant exec on SP_CONSULTA_DETALLE_TICKET_PEDIDO_ESPECIAL to public
go