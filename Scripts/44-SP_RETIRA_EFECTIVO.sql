use DB_A57E86_comercializadora
go

-- se crea procedimiento SP_RETIRA_EFECTIVO
if exists (select * from sysobjects where name like 'SP_RETIRA_EFECTIVO' and xtype = 'p' and db_name() = 'DB_A57E86_comercializadora')
	drop proc SP_RETIRA_EFECTIVO
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Realiza un retiro de efectvo por exceso de efectivo o por cierre de caja
status			200 = ok
				-1	= error
*/

create proc SP_RETIRA_EFECTIVO

	@idEstacion			int,
	@idUsuario			int,
	@monto				money,
	@caso				int  -- 1-Exceso de Efectivo / 2-Cierre de Caja

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se inserto correctamente el retiro de dinero.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''
						

			end  --declaraciones 

			begin -- principal

				-- si todo bien
				-- exceso de efectivo
				if ( @caso = 1)
					begin
						insert into 
							RetirosExcesoEfectivo 
								(montoRetiro,idUsuario,idEstacion,fechaAlta)
						values	(@monto, @idUsuario, @idEstacion, getdate())
					end

				-- cierre de caja
				if ( @caso = 2)
					begin
						insert into 
							RetirosCierreDia 
								(monto,idUsuario,idEstacion,fechaAlta)
						values	(@monto, @idUsuario, @idEstacion, getdate())
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
					@mensaje as  mensaje

		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_RETIRA_EFECTIVO to public
go



