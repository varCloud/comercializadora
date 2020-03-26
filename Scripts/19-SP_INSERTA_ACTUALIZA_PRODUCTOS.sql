use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_PRODUCTOS
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_PRODUCTOS' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_PRODUCTOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza los diferentes productos del sistema
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_PRODUCTOS

	@idProducto				int,
	@descripcion			varchar(255),
	@idUnidadMedida			int,
	@idLineaProducto		int,
	@cantidadUnidadMedida	float,
	@codigoBarras			nvarchar(4000),
	--@fechaAlta				datetime = null,
	@activo					bit,
	@articulo				varchar(255),
	@claveProdServ			float,
	@claveUnidad			nvarchar(4000)

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Producto sin modificaciones',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeProducto			bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if exists ( select 1 from Productos where idProducto = @idProducto )
					begin
						select @existeProducto = cast(1 as bit)
					end
					
				-- si es modificacion
				if	( (@idProducto > 0) )
					begin
						
						if not exists ( select 1 from Productos where idProducto = @idProducto ) 
						begin
							select @mensaje = 'El Producto no existe.'
							raiserror (@mensaje, 11, -1)
						end

						if ( @existeProducto = cast(1 as bit)) 
							begin
							
								update	Productos 
								set		descripcion = @descripcion,
										idUnidadMedida = @idUnidadMedida ,
										idLineaProducto = @idLineaProducto,
										cantidadUnidadMedida = @cantidadUnidadMedida,
										codigoBarras = @codigoBarras,
										--fechaAlta = @fechaAlta,
										activo = @activo,
										articulo = @articulo,
										claveProdServ = @claveProdServ,
										claveUnidad = @claveUnidad
								where	idProducto = @idProducto
						
								select	@mensaje = 'Cliente modificado correctamente.'
							
							end

					end
				-- si es nuevo
				else
					begin
						if ( @existeProducto = cast(0 as bit) )
							begin
								select @activo = cast(1 as bit)
								select @idProducto = coalesce( max(idProducto) + 1, 1) from Productos
								insert into productos (idProducto,descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,fechaAlta,activo,articulo,claveProdServ,claveUnidad)
								values (@idProducto,@descripcion,@idUnidadMedida,@idLineaProducto,@cantidadUnidadMedida,@codigoBarras,getdate(),@activo,@articulo,@claveProdServ,@claveUnidad)

								select @mensaje = 'Producto agregado correctamente.'
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

grant exec on SP_INSERTA_ACTUALIZA_PRODUCTOS to public
go



