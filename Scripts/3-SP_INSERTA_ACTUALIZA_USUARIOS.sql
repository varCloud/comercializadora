use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_USUARIOS
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_USUARIOS' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_USUARIOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza los diferentes usuarios del sistema
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_USUARIOS

	@idUsuario				int = null,
	@idRol					int,
	@usuario				varchar(50),
	@telefono				varchar(10),
	@contrasena				varchar(50),
	@idAlmacen				int,
	@idSucursal				int,
	@nombre					varchar(50),
	@apellidoPaterno		varchar(50),
	@apellidoMaterno		varchar(50),
	@fecha_alta				datetime,
	@activo					bit = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Usuario sin modificaciones',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeUsuario			bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if exists ( select 1 from usuarios where nombre like @nombre and apellidoPaterno like @apellidoPaterno and apellidoMaterno like @apellidoMaterno )
					begin
						select @existeUsuario = cast(1 as bit)
					end
					
				-- si es modificacion
				if	( (@idUsuario is not null) and (@idUsuario > 0) )
					begin
						
						if not exists ( select 1 from usuarios where idUsuario = @idUsuario ) 
						begin
							select @mensaje = 'El usuario no existe.'
							raiserror (@mensaje, 11, -1)
						end

						if ( (@existeUsuario = cast(1 as bit)) and ( (@activo is null) or (@activo = 0) ) )
							begin
								select	@activo=activo,
										@contrasena = contrasena 
								from	usuarios 
								where	nombre like @nombre 
									and apellidoPaterno like @apellidoPaterno 
									and apellidoMaterno like @apellidoMaterno
							end
							
						update	usuarios 
						set		idRol = @idRol,
								usuario = @usuario,
								telefono = @telefono,
								contrasena = @contrasena,
								idAlmacen = @idAlmacen,
								idSucursal = @idSucursal,
								nombre = @nombre,
								apellidoPaterno = @apellidoPaterno,
								apellidoMaterno = @apellidoMaterno,
								fecha_alta = @fecha_alta,
								activo = @activo 
						where	idUsuario = @idUsuario

						select @mensaje = 'Usuario modificado correctamente.'
					end
				-- si es nuevo
				else
					begin
						if ( @existeUsuario = cast(0 as bit) )
							begin
								select @activo = cast(1 as bit)
								select @idUsuario = max(idUsuario) + 1 from usuarios 
								insert into usuarios (idUsuario,idRol,usuario,telefono,contrasena,idAlmacen,idSucursal,nombre,apellidoPaterno,apellidoMaterno,fecha_alta,activo) 
								values (@idUsuario,@idRol,@usuario,@telefono,@contrasena,@idAlmacen,@idSucursal,@nombre,@apellidoPaterno,@apellidoMaterno,@fecha_alta,@activo)
								select @mensaje = 'Usuario agregado correctamente.'
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

grant exec on SP_INSERTA_ACTUALIZA_USUARIOS to public
go



