--USE [DB_A57E86_lluviadesarrollo]
--GO


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_VALIDA_USUARIO')
begin
	DROP PROCEDURE SP_VALIDA_USUARIO
end
GO

/****** Object:  StoredProcedure [dbo].[SP_VALIDA_USUARIO]    Script Date: 03/08/2021 06:51:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/14
Objetivo		Valida contraseña en la tabla de usuarios segun sea el caso:
					1 - Valida Usuario Cierre de Caja				
status			200 = ok
				-1	= error
*/

create  proc [dbo].[SP_VALIDA_USUARIO]

	@usuario		varchar(200),
	@contrasena		varchar(40),
	@caso			int  
	
as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status							int = 200,
						@mensaje						varchar(255) = 'Usuario validado correctamente.',
						@error_line						varchar(255) = '',
						@error_procedure				varchar(255) = ''						

			end  --declaraciones 

			begin -- principal

				if ( @caso = 1 )
					begin

						if not exists ( select contrasena from usuarios where usuario = @usuario and contrasena = @contrasena and activo = 1 ) 
						begin
							select @mensaje = 'La contraseña es incorrecta.'
							raiserror (@mensaje, 11, -1)
						end

						if not exists ( select contrasena from usuarios where usuario = @usuario and contrasena = @contrasena and activo = 1 and puedeAutorizarCierre = 1 ) 
						begin
							select @mensaje = 'El usuario no puede autorizar un cierre de caja.'
							raiserror (@mensaje, 11, -1)
						end

						
					end
				else
					begin

						select @mensaje = 'No existe el caso seleccionado.'
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
							@mensaje mensaje

					select	idUsuario,
							u.idRol,
							usuario,
							telefono,
							contrasena,
							nombre,
							apellidoPaterno,
							apellidoMaterno,
							cast(1 as bit) as usuarioValido
					from	usuarios u
					where	usuario = @usuario 
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

grant exec on SP_VALIDA_USUARIO to public
go