use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_ACTUALIZA_STATUS_LINEAS_PRODUCTO
if exists (select * from sysobjects where name like 'SP_ACTUALIZA_STATUS_LINEAS_PRODUCTO' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_ACTUALIZA_STATUS_LINEAS_PRODUCTO
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Actualiza el estatus de las lineas de producto
status			200 = ok
				-1	= error
*/

create proc SP_ACTUALIZA_STATUS_LINEAS_PRODUCTO

	@idLineaProducto	int,
	@activo				bit

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Linea de Producto modificada correctamente.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if not exists ( select 1 from LineaProducto where idLineaProducto = @idLineaProducto )
					begin
						select @mensaje = 'No existe la Linea de Producto solicitada.'
						raiserror (@mensaje, 11, -1)
					end
				else
					begin
						
						update	LineaProducto
						set		activo = @activo
						where	idLineaProducto = @idLineaProducto

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

grant exec on SP_ACTUALIZA_STATUS_LINEAS_PRODUCTO to public
go



