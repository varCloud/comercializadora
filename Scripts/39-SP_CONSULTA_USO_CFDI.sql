use DB_A57E86_comercializadora
go

-- se crea procedimiento SP_CONSULTA_USO_CFDI
if exists (select * from sysobjects where name like 'SP_CONSULTA_USO_CFDI' and xtype = 'p' and db_name() = 'DB_A57E86_comercializadora')
	drop proc SP_CONSULTA_USO_CFDI
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes claves de productos 
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_USO_CFDI

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

			begin -- principal
				
				if not exists ( select top 1 1 from FactCatUsoCFDI )
					begin
						select	@mensaje = 'No existen claves de uso de CFDI registradas.',
								@valido = cast(0 as bit)
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

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje as  mensaje


			if ( @valido = cast(1 as bit) )
				begin
						
					select	distinct 
							id,
							usoCFDI,
							descripcion
					from	FactCatUsoCFDI 

				end
								
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_USO_CFDI to public
go



