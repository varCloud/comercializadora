use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_TIPOS_CLIENTES
if exists (select * from sysobjects where name like 'SP_CONSULTA_TIPOS_CLIENTES' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_TIPOS_CLIENTES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes clientes del sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_TIPOS_CLIENTES

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)

				
						
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
							

		-- si todo ok
			select	idTipoCliente,
					descripcion,
					descuento,
					activo
			from	CatTipoCliente
			where	activo = cast(1 as bit)
				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_TIPOS_CLIENTES to public
go



