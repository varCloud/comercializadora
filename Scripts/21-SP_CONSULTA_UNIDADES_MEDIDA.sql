use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_UNIDADES_MEDIDA
if exists (select * from sysobjects where name like 'SP_CONSULTA_UNIDADES_MEDIDA' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_UNIDADES_MEDIDA
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes unidades de medidas del sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_UNIDADES_MEDIDA

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
				
				if not exists ( select 1 from CatUnidadMedida )
					begin
						select @mensaje = 'No existen unidades de medida registradas.'
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

			if ( @valido = cast(1 as bit) )
				begin
						
					select	distinct 
							@status status,
							@error_procedure error_procedure,
							@error_line error_line,
							@mensaje mensaje,
					
							idUnidadMedida,
							descripcion
					from	CatUnidadMedida 

				end
			else
				begin

					select	-1 status,
							@error_procedure error_procedure,
							@error_line error_line,
							@mensaje as  mensaje
							
				end

				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_UNIDADES_MEDIDA to public
go



