use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_PASILLO_PISO_RAQ
if exists (select * from sysobjects where name like 'SP_CONSULTA_PASILLO_PISO_RAQ' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_CONSULTA_PASILLO_PISO_RAQ
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/07/23
Objetivo		Consulta el piso/raq/pasillo 0 para todas la ubicaciones
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_PASILLO_PISO_RAQ

	@caso			int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''

						
			end  --declaraciones 


		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message()
					
		end catch

		begin -- reporte de estatus

		--reporte de estatus
			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
							

		---- si todo ok
		if (@caso = 1) -- Piso
		begin
			
			select	idPiso as id,
					descripcion
			from	CatPiso
			where	idPiso > 0

		end

		if (@caso = 2) -- Pasillo
		begin
				
			select	idPasillo as id,
					descripcion
			from	CatPasillo			
			where	idPasillo > 0

		end

		if (@caso = 3) -- Raq
		begin
			
			select	idRaq as id,
					descripcion
			from	CatRaq
			where	idRaq > 0

		end
					

				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_PASILLO_PISO_RAQ to public
go



