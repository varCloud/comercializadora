use DB_A57E86_comercializadora
go

-- se crea procedimiento SP_CONSULTA_RETIROS_EFECTIVO
if exists (select * from sysobjects where name like 'SP_CONSULTA_RETIROS_EFECTIVO' and xtype = 'p' and db_name() = 'DB_A57E86_comercializadora')
	drop proc SP_CONSULTA_RETIROS_EFECTIVO
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta numero de ventas, efectivo disponible, y · retiros por estacion
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_RETIROS_EFECTIVO

	@idEstacion			int,
	@fecha				datetime = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''
						

			end  --declaraciones 

			--begin -- principal
				select @fecha = coalesce(@fecha, getdate())
				select @fecha = cast(@fecha as date)

				if not exists ( select 1 from RetirosExcesoEfectivo where idEstacion = @idEstacion  and	cast(fechaAlta as date) = @fecha )
					begin
						select @mensaje = 'No Existen Retiros el dia de hoy.'
						raiserror (@mensaje, 11, -1)
					end

				
			--end -- principal

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

			select	idRetiro,montoRetiro,r.idUsuario,
					u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
					r.idEstacion, e.nombre as nombreEstacion, r.fechaAlta 
			from	RetirosExcesoEfectivo R
						inner join Usuarios u 
							on u.idUsuario = r.idUsuario
						inner join Estaciones e
							on e.idEstacion = r.idEstacion
			where	r.idEstacion = @idEstacion
				and	cast(r.fechaAlta as date) = @fecha
								
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_RETIROS_EFECTIVO to public
go



