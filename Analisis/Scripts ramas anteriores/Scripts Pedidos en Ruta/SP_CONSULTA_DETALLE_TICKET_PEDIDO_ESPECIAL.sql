--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_DETALLE_TICKET_PEDIDO_ESPECIAL]    Script Date: 26/03/2022 07:27:41 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consulta la informaciòn del ticket de pedido especial
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_DETALLE_TICKET_PEDIDO_ESPECIAL]

	@idPedidoEspecial				bigint,
	@idTipoTicketPedidoEspecial		int,
	@idTicketPedidoEspecial			bigint=null,
	@ticketFinal					bit
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
			
				if ( @ticketFinal = cast(0 as bit) )
					begin
						if not exists ( select 1 from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial)
						begin
							select	@mensaje = 'No existe el pedido seleccionado.',
									@status = -1
						end								
					end
				else
					begin
						if ( @idTicketPedidoEspecial is null and @idTipoTicketPedidoEspecial in (1,2,3))
						begin
							select @idTicketPedidoEspecial=max(idTicketPedidoEspecial) from TicketsPedidosEspeciales where idPedidoEspecial=@idPedidoEspecial and idTipoTicketPedidoEspecial=@idTipoTicketPedidoEspecial
						end
				
						if (coalesce(@idTicketPedidoEspecial,0)=0) 
						begin
							select	
									@status = -1,
									@mensaje = 'No existe el ticket del pedido especial'
						end
					end
			
				if ( @ticketFinal = cast(0 as bit) )
					begin

						select	ROW_NUMBER() OVER(ORDER BY p.idPedidoEspecial DESC) AS contador,	
								p.idPedidoEspecial, ped.idProducto, ped.cantidad, ped.monto as monto, cast(0 as money) as montoIVA,
								coalesce (((ped.precioIndividual - ped.precioVenta) * ped.cantidad ) , 0 )as ahorro , 
								pro.descripcion as descProducto, p.idCliente, 
								case
									when c.nombres is null then 'PÚBLICO EN GENERAL' 
									else c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno
								end as nombreCliente,
								u.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
								ped.cantidadActualInvGeneral, ped.cantidadAnteriorInvGeneral, p.fechaAlta, ped.precioVenta, ped.idPedidoEspecialDetalle,
								99 as formaPago, 'Por definir' as descFormaPago, 
								p.codigoBarras, cast(0 as money) as montoPagado, cast(0 as money) as montoComisionBancaria,
								upper( (coalesce(calle, 'N/A') + ' ' + coalesce(numeroExterior, 'S/N') + ', Colonia: ' + coalesce(colonia, 'N/A') + ', CP: ' + coalesce(cp, 'N/A') + ', Municipio: ' + coalesce(municipio, 'N/A')  + ', Estado: ' + coalesce(estado, 'N/A') ) ) as direccion
						into #PedidoEspecialPrevio
						from	PedidosEspeciales p 
									inner join PedidosEspecialesDetalle ped
										on p.idPedidoEspecial = ped.idPedidoEspecial
									left join Clientes c
										on c.idCliente = p.idCliente
									inner join Usuarios u
										on u.idUsuario = p.idUsuario
									inner join Productos pro
										on pro.idProducto = ped.idProducto
									left join FactCatFormaPago formaPago
										on p.idFactFormaPago = formaPago.id

						where	p.idPedidoEspecial = @idPedidoEspecial
							and	ped.cantidad > 0
						order by ped.idPedidoEspecialDetalle

					end
				else
				begin
					if(@idTipoTicketPedidoEspecial in (1,2,3)) --si el ticket es original o por devoluciones se obtiene la informaciòn de la tabla TicketsPedidosEspecialesDetalle
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
							tp.idTipoTicketPedidoEspecial,CONVERT(VARCHAR(10),tp.fechaAlta,103) fechaTicket, CONVERT(VARCHAR(20),tp.fechaAlta,114) horaTicket,
							upper( (coalesce(calle, 'N/A') + ' ' + coalesce(numeroExterior, 'S/N') + ', Colonia: ' + coalesce(colonia, 'N/A') + ', CP: ' + coalesce(cp, 'N/A') + ', Municipio: ' + coalesce(municipio, 'N/A')  + ', Estado: ' + coalesce(estado, 'N/A') ) ) as direccion
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
				begin
					if ( @ticketFinal = cast(1 as bit) )
						begin
							select * from #TicketsPedidosEspecialesDetalle
						end
					else
						begin
							select * from #PedidoEspecialPrevio						
						end
				end

		end -- reporte de estatus
		

	end  -- principal

