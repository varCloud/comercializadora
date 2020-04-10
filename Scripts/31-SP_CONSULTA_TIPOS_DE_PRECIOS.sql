use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_TIPOS_DE_PRECIOS
if exists (select * from sysobjects where name like 'SP_CONSULTA_TIPOS_DE_PRECIOS' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_TIPOS_DE_PRECIOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/03/27
Objetivo		Consulta los diferentes rangos de precio del idProducto
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_TIPOS_DE_PRECIOS
	@idProducto			int 
as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeProducto			bit = cast(1 as bit)

			end  --declaraciones 


			begin  --validaciones

				if not exists ( select 1 from ProductosPorPrecio where idProducto = @idProducto )
					begin
						select	@existeProducto = cast(0 as bit),
								@mensaje = 'No existen un precio definido para el producto con la cantidad seleccionada.',
								@status = -1
					end	

			end  --validaciones

			

		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message()
					
		end catch

		begin -- reporte de estatus

		--reporte de estatus
			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
							

		-- si todo ok
			select	contador,
					idProducto,
					min,
					max,
					costo 
			from	ProductosPorPrecio
			where	idProducto = @idProducto
			and		activo = cast(1 as bit)
				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_TIPOS_DE_PRECIOS to public
go



