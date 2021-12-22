--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_V2]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_V2
if exists (select * from sysobjects where name like 'SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_V2' and xtype = 'p' )
	drop proc SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_V2
go

/*
Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/10
Objetivo		Consulta el detalle de pedidos especiales V2
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_V2]

	@idPedidoEspecial			int 

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit),
						@idTicketMayoreo		int = 0
						
			end  --declaraciones 

			begin -- principal

				if not exists ( select 1 from PedidosEspecialesDetalle where idPedidoEspecial = @idPedidoEspecial )
				begin
					select @mensaje = 'No existe el pedido especial solicitado.'
					select @valido = cast(0 as bit)
					raiserror (@mensaje, 11, -1)					
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

			select	@idTicketMayoreo = idTicketMayoreo
			from	PedidosEspeciales
			where	idPedidoEspecial = @idPedidoEspecial

			--reporte de estatus
				select	@status status,
						@error_procedure error_procedure,
						@error_line error_line,
						@mensaje mensaje
							

			-- si todo ok
				if ( @valido = 1 )
					begin

						select	ROW_NUMBER() OVER(ORDER BY idPedidoEspecialDetalle) AS id,
								ped.idPedidoEspecialDetalle,
								ped.idPedidoEspecial,
								ped.idVenta,
								p.idProducto,
								p.descripcion,
								ped.idUbicacion,
								ped.idAlmacenOrigen,
								ped.idAlmacenDestino,
								ped.idAlmacenDestino idAlmacen,
								a.Descripcion as Almacen,
								ped.fechaAlta,
								ped.cantidad,
								ped.monto,
								ped.cantidadActualInvGeneral,
								ped.cantidadAnteriorInvGeneral,
								p.precioIndividual,
								p.precioMenudeo,
								ped.precioRango,
								ped.precioVenta,
								ped.idTicketMayoreo,
								ped.observaciones,
								p.ultimoCostoCompra,
								coalesce ( (ped.cantidadAceptada + ped.cantidadRechazada), 0) as cantidadSolicitada, 
								ped.cantidadAceptada,
								ped.cantidadAtendida,
								ped.cantidadRechazada,
								ped.idEstatusPedidoEspecialDetalle,
								est.descripcion estatusPedidoEspecialDetalle,
								ped.observacionesConfirmar,
								ped.notificado,
								invActual.cantidadActual cantidadActualInvAlmacen,
								dbo.LineaProductoFraccion(p.idLineaProducto,p.idProducto) fraccion,
								p.idLineaProducto,
								ped.montoComisionBancaria,
								@idTicketMayoreo as idTicketMayoreo

						from	PedidosEspecialesDetalle ped
									join Productos p 
										on p.idProducto = ped.idProducto
									join Almacenes a
										on a.idAlmacen = ped.idAlmacenDestino
									join CatEstatusPedidoEspecialDetalle est on est.idEstatusPedidoEspecialDetalle=ped.idEstatusPedidoEspecialDetalle
									left join(select sum(cantidad) cantidadActual,idAlmacen,idProducto from InventarioDetalle i
											join Ubicacion u on i.idUbicacion=u.idUbicacion
											group by idAlmacen,idProducto
											) invActual on p.idProducto=invActual.idProducto and invActual.idAlmacen=ped.idAlmacenDestino
						where	idPedidoEspecial = @idPedidoEspecial

					end
				
			end -- reporte de estatus

	end  -- principal
go
grant exec on SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_V2 to public
go