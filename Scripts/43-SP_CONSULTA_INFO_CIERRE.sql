use DB_A57E86_comercializadora
go

-- se crea procedimiento SP_CONSULTA_INFO_CIERRE
if exists (select * from sysobjects where name like 'SP_CONSULTA_INFO_CIERRE' and xtype = 'p' and db_name() = 'DB_A57E86_comercializadora')
	drop proc SP_CONSULTA_INFO_CIERRE
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta numero de ventas, efectivo disponible, y · retiros por estacion
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_INFO_CIERRE

	@idEstacion			int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@totalVentas			int = 0,					
						@efectivoDisponible		money = 0,
						@montoCierre			money = 0,
						@retirosHechosDia		money = 0,
						@hoy					datetime
						

			end  --declaraciones 

			begin -- principal
				
				select @hoy = cast(getdate() as date)

				select	@efectivoDisponible = coalesce(sum(montoTotal), 0),
						@totalVentas = coalesce(count(montoTotal),0)
				from	Ventas 
				where	idEstacion = @idEstacion
					and cast(fechaAlta as date) = @hoy
					and idStatusVenta = 1


				select	@retirosHechosDia = coalesce( sum(montoRetiro),0 )
				from	RetirosExcesoEfectivo
				where	idEstacion = @idEstacion
					and cast(fechaAlta as date) = @hoy

				select @montoCierre = @efectivoDisponible - @retirosHechosDia
				
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

			select	@totalVentas as totalVentas,
					@efectivoDisponible as efectivoDisponible,
					@retirosHechosDia as retirosHechosDia,
					@montoCierre as montoCierre
								
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_INFO_CIERRE to public
go



