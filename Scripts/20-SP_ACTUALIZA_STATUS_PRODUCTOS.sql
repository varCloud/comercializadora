use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_ACTUALIZA_STATUS_PRODUCTOS
if exists (select * from sysobjects where name like 'SP_ACTUALIZA_STATUS_PRODUCTOS' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_ACTUALIZA_STATUS_PRODUCTOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Actualiza el estatus los productos del sistema
status			200 = ok
				-1	= error
*/

create proc SP_ACTUALIZA_STATUS_PRODUCTOS

	@idProducto		int,
	@activo			bit

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Producto modificado correctamente.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if not exists ( select 1 from Productos where idProducto = @idProducto )
					begin
						select @mensaje = 'No existe el Producto solicitado.'
						raiserror (@mensaje, 11, -1)
					end
				else
					begin
						
						update	Productos
						set		activo = @activo
						where	idProducto = @idProducto

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
					@mensaje mensaje

				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_ACTUALIZA_STATUS_PRODUCTOS to public
go



