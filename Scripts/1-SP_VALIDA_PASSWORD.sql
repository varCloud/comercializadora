use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_VALIDA_CONTRASENA
if exists (select * from sysobjects where name like 'SP_VALIDA_CONTRASENA' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_VALIDA_CONTRASENA
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/14
Objetivo		Valida contraseña encriptada de tabla de usuarios
status			200 = ok
				-1	= error
*/

create proc SP_VALIDA_CONTRASENA

	@nombre		varchar(200),
	@contrasena	varchar(40)

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Usuario validado correctamente.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''

			end  --declaraciones 

			begin -- principal

				if not exists ( select contrasena from usuarios where nombre = @nombre and contrasena = @contrasena ) 
				begin
					select @mensaje = 'La contraseña es incorrecta.'
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

			if @status = 200
				begin
					select	@status status,
							@error_procedure error_procedure,
							@error_line error_line,
							@mensaje mensaje,
							idUsuario,
							idRol,
							nombre,
							telefono,
							contrasena,
							idAlmacen,
							idSucursal
					from	usuarios 
					where	nombre = @nombre 
						and contrasena = @contrasena
				end
			else
				begin
					select	@status status,
							@error_procedure error_procedure,
							@error_line error_line,
							@mensaje mensaje
				end	

		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_VALIDA_CONTRASENA to public
go



