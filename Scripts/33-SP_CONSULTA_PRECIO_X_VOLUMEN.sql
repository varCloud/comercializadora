use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_PRECIO_X_VOLUMEN
if exists (select * from sysobjects where name like 'SP_CONSULTA_PRECIO_X_VOLUMEN' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_PRECIO_X_VOLUMEN
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes clientes del sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_PRECIO_X_VOLUMEN
	@idProducto		int,
	@cantidad		int 
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


			if not exists ( 
							select	1 
							from	ProductosPorPrecio
							where	idProducto = @idProducto
								and @cantidad between min and max
								and activo = cast(1 as bit)
						  )
			begin
				select	@mensaje = 'No existen rangos activos de precio para el producto seleccionado',
						@status = -1,
						@valido = cast(0 as bit)
			end	

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
			if ( @valido = cast(1as bit) )
				begin
					select	contador,
							idProducto,
							min,
							max,
							costo,
							activo,
							idTipoPrecio 
					from	ProductosPorPrecio
					where	idProducto = @idProducto
						and @cantidad between min and max
						and activo = cast(1 as bit)
				end

				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_PRECIO_X_VOLUMEN to public
go



