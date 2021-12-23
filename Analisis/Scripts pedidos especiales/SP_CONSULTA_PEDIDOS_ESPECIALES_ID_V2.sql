USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PEDIDOS_ESPECIALES_ID_V2]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_CONSULTA_PEDIDOS_ESPECIALES_ID_V2
if exists (select * from sysobjects where name like 'SP_CONSULTA_PEDIDOS_ESPECIALES_ID_V2' and xtype = 'p' )
	drop proc SP_CONSULTA_PEDIDOS_ESPECIALES_ID_V2
go

/*
Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/10
Objetivo		Consulta los pedidos especiales detalle V2
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONSULTA_PEDIDOS_ESPECIALES_ID_V2]

	@idPedidoEspecial			int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
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

						select	ROW_NUMBER() OVER(ORDER BY p.idPedidoEspecial) AS id, 
								p.idPedidoEspecial,
								c.idCliente,
								c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno as nombreCliente,
								p.cantidad,
								p.fechaAlta,
								p.montoTotal,
								u.idUsuario,
								u.nombre +  ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
								p.idEstatusPedidoEspecial,
								p.idEstacion,
								p.observaciones,
								p.codigoBarras,
								p.idTipoPago,
								p.idUsuarioEntrega,
								p.numeroUnidadTaxi,
								case
									when idEstatusPedidoEspecial = 3 then cast(1 as bit) -- En resguardo
									else cast(0 as bit)
								end as puedeEntregar,
								p.idFactMetodoPago,
								p.idFactFormaPago,
								p.idFactUsoCFDI,
								coalesce(f.idEstatusFactura, 0) as idEstatusFactura
						from	PedidosEspeciales p
									join Clientes c
										on c.idCliente = p.idCliente
									join Usuarios u
										on u.idUsuario = p.idUsuario
								left join FacturasPedidosEspeciales f
									on f.idPedidoEspecial = p.idPedidoEspecial

						where	p.idPedidoEspecial =		case
																when @idPedidoEspecial = 0 then p.idPedidoEspecial
																when @idPedidoEspecial is null then p.idPedidoEspecial
																else @idPedidoEspecial
															end

						order by p.fechaAlta desc

					end
				
			end -- reporte de estatus

	end  -- principal

grant exec on SP_CONSULTA_PEDIDOS_ESPECIALES_ID_V2 to public
go