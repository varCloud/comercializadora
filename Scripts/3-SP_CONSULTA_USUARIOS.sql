use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_USUARIOS
if exists (select * from sysobjects where name like 'SP_CONSULTA_USUARIOS' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_USUARIOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes usuarios del sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_USUARIOS

	@idUsuario		int

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
				
				if not exists ( select 1 from Usuarios )
					begin
						select @mensaje = 'No existen usuarios registrados.'
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
							u.idUsuario,
							u.idRol,
							u.usuario,
							u.telefono,
							u.contrasena,
							u.idAlmacen,
							u.idSucursal,
							u.nombre,
							u.apellidoPaterno,
							--u.apellidoMaterno,
							case 
								when u.activo = cast(0 as bit) then u.apellidoMaterno + ' (BAJA)'
								else u.apellidoMaterno
							end as apellidoMaterno,
							u.fecha_alta,
							u.activo,
							r.descripcion as descripcionRol,
							s.descripcion as descripcionSucursal,
							a.descripcion as descripcionAlmacen
					from	Usuarios u
								left join catRoles r
									on r.idRol = u.idRol
								left join CatSucursales s
									on s.idSucursal = u.idSucursal
								left join Almacenes a
									on a.idAlmacen = u.idAlmacen
					where	u.idUsuario =	case
												when @idUsuario > 0 then @idUsuario
												else u.idUsuario
											end
						--and	u.activo = cast(1 as bit)
						and r.idRol <> 1 -- administrador

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

grant exec on SP_CONSULTA_USUARIOS to public
go



