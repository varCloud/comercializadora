USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_TICKET_PEDIDOS_ESPECIALES_V2]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_CONSULTA_TICKET_PEDIDOS_ESPECIALES_V2
if exists (select * from sysobjects where name like 'SP_CONSULTA_TICKET_PEDIDOS_ESPECIALES_V2' and xtype = 'p' )
	drop proc SP_CONSULTA_TICKET_PEDIDOS_ESPECIALES_V2
go

/*
Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/10
Objetivo		Consulta los productos de los diferentes almacenes de un pedido especial
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONSULTA_TICKET_PEDIDOS_ESPECIALES_V2]

	@idPedidoEspecial			int


as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@ini					int = 0,
						@fin					int = 0,
						@valido					bit = cast(1 as bit)
						
			end  --declaraciones 

			begin -- principal

				if not exists ( select 1 from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial )
				begin
					if ( @idPedidoEspecial > 0 )
					begin 
						select @mensaje = 'No existe el pedido especial solicitado.'
						select @valido = cast(0 as bit)
						raiserror (@mensaje, 11, -1)					
					end
				end
				
			end -- principal

			end try

			begin catch 
		
				-- captura del error
				select	@status =			-error_state(),
						@error_procedure =	error_procedure(),
						@error_line =		error_line(),
						@mensaje =			error_message(),
						@valido = cast(0 as bit)
					
			end catch

			begin -- reporte de estatus

			--reporte de estatus
				select	@status status,
						@error_procedure error_procedure,
						@error_line error_line,
						@mensaje mensaje
							

			-- si todo ok
				if ( @valido = 1 )
					begin

						--select * from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial

						create table
							#almacenes
								(
									id					int identity(1,1),
									idPedidoEspecial	int,
									idAlmacenDestino	int
								)
						
						
						--select * from PedidosEspecialesDetalle where idPedidoEspecial = @idPedidoEspecial


						insert into #almacenes (idPedidoEspecial, idAlmacenDestino)
						select	@idPedidoEspecial as idPedidoEspecial, idAlmacenDestino
						from	PedidosEspecialesDetalle
						where	idPedidoEspecial = @idPedidoEspecial
						group by idAlmacenDestino
						order by idAlmacenDestino asc

						--select * from #almacenes

						select @ini = min(idAlmacenDestino), @fin= max(idAlmacenDestino) from #almacenes

						while ( @ini <= @fin )
							begin
								
								select	ped.idPedidoEspecialDetalle,
										ped.idPedidoEspecial,
										ped.idVenta,
										ped.idProducto,
										p.descripcion,
										ped.idUbicacion,
										ped.idAlmacenOrigen,
										ped.idAlmacenDestino,
										ped.fechaAlta,
										ped.cantidad,
										ped.monto,
										ped.cantidadActualInvGeneral,
										ped.cantidadAnteriorInvGeneral,
										ped.precioIndividual,
										ped.precioMenudeo,
										ped.precioRango,
										ped.precioVenta,
										ped.idTicketMayoreo,
										ped.observaciones,
										ped.ultimoCostoCompra,
										ped.cantidadAceptada,
										ped.cantidadAtendida,
										ped.cantidadRechazada,
										ped.idEstatusPedidoEspecialDetalle,
										ped.notificado
								from	PedidosEspecialesDetalle  ped
											join Productos p
												on p.idProducto = ped.idProducto
								where	idPedidoEspecial = @idPedidoEspecial
									and	idAlmacenDestino = @ini


								select @ini = min(idAlmacenDestino) from #almacenes where idAlmacenDestino > @ini

							end  -- while ( @ini <= @fin )





					end
				
			end -- reporte de estatus

	end  -- principal

grant exec on SP_CONSULTA_TICKET_PEDIDOS_ESPECIALES_V2 to public
go