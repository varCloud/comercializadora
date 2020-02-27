use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_LINEAS_PRODUCTO
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_LINEAS_PRODUCTO' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_LINEAS_PRODUCTO
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza las diferentes lineas de producto del sistema
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_LINEAS_PRODUCTO

	@idLineaProducto	int,
	@descripcion		varchar(255),
	@activo				bit

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Linea de Producto sin modificaciones',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeLinea			bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if exists ( select 1 from LineaProducto where descripcion like @descripcion )
					begin
						select @existeLinea = cast(1 as bit)
					end
					
				-- si es modificacion
				if	( (@idLineaProducto > 0) )
					begin
						
						if not exists ( select 1 from LineaProducto where idLineaProducto = @idLineaProducto ) 
						begin
							select @mensaje = 'La Linea de Producto no existe.'
							raiserror (@mensaje, 11, -1)
						end

						if ( (@existeLinea = cast(1 as bit)) and ( (@activo is null) or (@activo = 0) ) )
							begin
								select	@activo = activo
								from	LineaProducto
								where	idLineaProducto = @idLineaProducto
							end
							
						update	LineaProducto 
						set		descripcion = @descripcion,
								activo = @activo 
						where	idLineaProducto = @idLineaProducto

						select @mensaje = 'Linea de Producto modificada correctamente.'
					end
				-- si es nuevo
				else
					begin
						if ( @existeLinea = cast(0 as bit) )
							begin
								select @activo = cast(1 as bit)
								insert into LineaProducto (descripcion,activo) 
								values (@descripcion,@activo) 
								select @mensaje = 'Linea de Prouducto agregada correctamente.'
							end
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

grant exec on SP_INSERTA_ACTUALIZA_LINEAS_PRODUCTO to public
go



