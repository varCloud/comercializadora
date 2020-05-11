use DB_A57E86_comercializadora
go

-- se crea procedimiento SP_CONSULTA_VENTA
if exists (select * from sysobjects where name like 'SP_CONSULTA_VENTA' and xtype = 'p' and db_name() = 'DB_A57E86_comercializadora')
	drop proc SP_CONSULTA_VENTA
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta una venta por su idVenta
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_VENTA

	@idVenta	int

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
				
				if not exists ( select top 1 1 from Ventas where idVenta = @idVenta)
					begin
						select	@mensaje = 'No existe la venta.' + cast(@idVenta as varchar(20)),
								@valido = cast(0 as bit)
						raiserror (@mensaje, 11, -1)
					end
				
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


			if ( @valido = cast(1 as bit) )
				begin
						
					select	idVenta,
							idCliente,
							cantidad,
							fechaAlta,
							montoTotal,
							idUsuario,
							idStatusVenta,
							idFactFormaPago,
							idFactUsoCFDI
					from	Ventas 
					where	idVenta = @idVenta


				end
								
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_VENTA to public
go



