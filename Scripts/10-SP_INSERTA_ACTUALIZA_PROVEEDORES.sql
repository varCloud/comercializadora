use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_PROVEEDORES
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_PROVEEDORES' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_PROVEEDORES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza los diferentes proveedores del sistema
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_PROVEEDORES

	@idProveedor	int,
	@nombre			varchar(255),
	@descripcion	varchar(255),
	@telefono		varchar(10),
	@direccion		varchar(255),
	@activo			bit

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Proveedor sin modificaciones',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeProveedor			bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if exists ( select 1 from Proveedores where nombre like @nombre )
					begin
						select @existeProveedor = cast(1 as bit)
					end
					
				-- si es modificacion
				if	( (@idProveedor > 0) )
					begin
						
						if not exists ( select 1 from Proveedores where idProveedor = @idProveedor ) 
						begin
							select @mensaje = 'El Proveedor no existe.'
							raiserror (@mensaje, 11, -1)
						end

						if ( (@existeProveedor = cast(1 as bit)) and ( (@activo is null) or (@activo = 0) ) )
							begin
								select	@activo = activo
								from	Proveedores
								where	idProveedor = @idProveedor
							end
							
						update	Proveedores 
						set		nombre = @nombre,
								descripcion = @descripcion,
								telefono = @telefono,
								direccion = @direccion,
								activo = @activo 
						where	idProveedor = @idProveedor

						select @mensaje = 'Proveedor modificado correctamente.'
					end
				-- si es nuevo
				else
					begin
						if ( @existeProveedor = cast(0 as bit) )
							begin
								select @activo = cast(1 as bit)

								select @idProveedor = coalesce(max(idProveedor) + 1, 1) from Proveedores 
								insert into Proveedores (idProveedor,nombre,descripcion,telefono,direccion,activo) 
								values (@idProveedor,@nombre,@descripcion,@telefono,@direccion,@activo)
								select @mensaje = 'Proveedor agregado correctamente.'
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

grant exec on SP_INSERTA_ACTUALIZA_PROVEEDORES to public
go



