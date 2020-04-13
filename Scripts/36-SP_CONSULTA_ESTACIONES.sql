use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_ESTACIONES
if exists (select * from sysobjects where name like 'SP_CONSULTA_ESTACIONES' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_ESTACIONES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta las diferentes estaciones l sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_ESTACIONES

	@idEstacion				int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if not exists ( select 1 from Estaciones )
					begin
						select @mensaje = 'No existen Estaciones registradas.'
						raiserror (@mensaje, 11, -1)
					end
				else
					begin
						select @valido = cast(1 as bit)
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
		
			select	distinct 
					@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje

			if ( @valido = cast(1 as bit) )
				begin
						
					select	ROW_NUMBER() OVER(ORDER BY idEstacion DESC) AS contador,
							e.idEstacion,
							e.idAlmacen,
							a.Descripcion as nombreAlmacen,
							e.macAdress,
							e.nombre,
							e.numero,
							e.configurado,
							e.idUsuario,
							e.fechaAlta,
							e.idStatus,
							c.descripcion
					from	Estaciones e
								inner join CatEstaciones c
									on c.idStatus = e.idStatus
								inner join Almacenes a
									on a.idAlmacen = e.idAlmacen
					where	e.idStatus = 1
						and	e.idEstacion =	case
												when @idEstacion = 0 then idEstacion
												when @idEstacion is null then idEstacion
												else @idEstacion
											end

				end
				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_ESTACIONES to public
go



