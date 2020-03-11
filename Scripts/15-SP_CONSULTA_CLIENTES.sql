use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_CLIENTES
if exists (select * from sysobjects where name like 'SP_CONSULTA_CLIENTES' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_CLIENTES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes clientes del sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_CLIENTES

	@idCliente				int

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
				
				if not exists ( select 1 from clientes )
					begin
						select @mensaje = 'No existen clientes registrados.'
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
							@mensaje mensaje
					select 
							ROW_NUMBER() OVER(ORDER BY idCliente DESC) AS idCliente,
							c.nombres,
							c.apellidoPaterno,
							case
								when ( (c.activo = cast(0 as bit)) or (c.activo is null) ) then c.apellidoMaterno + ' (BAJA)'
								else c.apellidoMaterno
							end as apellidoMaterno,
							c.telefono,
							c.correo,
							c.rfc,
							c.calle,
							c.numeroExterior,
							c.colonia,
							c.municipio,
							c.cp,
							c.estado,
							c.fechaAlta,
							c.activo,
							c.idTipoCliente,
							t.descripcion 
					from	Clientes c
							inner join CatTipoCliente t
								on c.idTipoCliente = t.idTipoCliente
					where	c.idCliente	 =	case
												when @idCliente > 0 then @idCliente
												else c.idCliente
											end
						--and	c.activo = cast(1 as bit)
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

grant exec on SP_CONSULTA_CLIENTES to public
go



