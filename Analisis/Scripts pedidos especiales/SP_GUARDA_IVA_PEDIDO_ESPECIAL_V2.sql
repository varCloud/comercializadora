--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_GUARDA_IVA_PEDIDO_ESPECIAL_V2]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_GUARDA_IVA_PEDIDO_ESPECIAL_V2
if exists (select * from sysobjects where name like 'SP_GUARDA_IVA_PEDIDO_ESPECIAL_V2' and xtype = 'p' )
	drop proc SP_GUARDA_IVA_PEDIDO_ESPECIAL_V2
go

/*
Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/10
Objetivo		Guarda el iva de un pedido especial
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_GUARDA_IVA_PEDIDO_ESPECIAL_V2]

	@idPedidoEspecial			int,
	@idCliente					int,
	@idFactFormaPago			int,
	@idFactUsoCFDI				int
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
				
				-- si no existe
				if not exists ( select top 1 1 from PedidosEspecialesDetalle where idPedidoEspecial = @idPedidoEspecial)
					begin
						select	@mensaje = 'No existe el pedido: ' + cast(@idPedidoEspecial as varchar(20)),
								@valido = cast(0 as bit)
						raiserror (@mensaje, 11, -1)
					end
				
				-- si ya esta facturada
				if exists ( select 1 from FacturasPedidosEspeciales where idEstatusFactura = 1 and idPedidoEspecial = @idPedidoEspecial )
					begin
						select	@mensaje = 'El pedido ya esta facturado, verifique por favor.',
								@valido = cast(0 as bit)
						raiserror (@mensaje, 11, -1)
					end

				update	PedidosEspecialesDetalle
				set		montoIva = a.iva
				from	(
							select	idPedidoEspecial, idPedidoEspecialDetalle, cantidad, precioVenta,
									( cantidad * precioVenta ) * 0.16 as iva
							from	PedidosEspecialesDetalle
							where	idPedidoEspecial = @idPedidoEspecial
						)A
				where	PedidosEspecialesDetalle.idPedidoEspecialDetalle = a.idPedidoEspecialDetalle

				update	PedidosEspeciales
				set		idCliente = @idCliente,
						idFactFormaPago = @idFactFormaPago,
						idFactUsoCFDI = @idFactUsoCFDI	
				where	idPedidoEspecial = @idPedidoEspecial

				
			end -- principal

		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message()
					
		end catch

		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje as  mensaje

			select	idPedidoEspecial,
					idCliente,
					cantidad,
					fechaAlta,
					montoTotal,
					idUsuario,
					idEstatusPedidoEspecial,
					idFactFormaPago,
					idFactUsoCFDI 
			from	PedidosEspeciales
			where	idPedidoEspecial = @idPedidoEspecial

		end -- reporte de estatus

	end  -- principal

grant exec on SP_GUARDA_IVA_PEDIDO_ESPECIAL_V2 to public
go