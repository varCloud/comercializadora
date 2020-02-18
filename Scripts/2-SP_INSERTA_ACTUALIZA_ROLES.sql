use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_ROLES
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_ROLES' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_ROLES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza los diferentes roles disponibles
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_ROLES

	@descripcion	varchar(50),
	@activo			bit = null,
	@idRol			int = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Rol sin modificaciones',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeDescripcion		bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if exists ( select 1 from CatRoles where descripcion like @descripcion /*and activo = cast(1 as bit)*/ )
					begin
						select @existeDescripcion = cast(1 as bit)
					end
					
				-- si es modificacion
				if	( @idRol is not null )
					begin
						
						if not exists ( select 1 from CatRoles where idRol = @idRol ) 
						begin
							select @mensaje = 'El rol no existe.'
							raiserror (@mensaje, 11, -1)
						end

						if ( (@existeDescripcion = cast(1 as bit)) and ( @activo is null ) )
							begin
								select @activo=activo from CatRoles where descripcion like @descripcion
							end
							
						update	CatRoles 
						set		descripcion = @descripcion, 
								activo = @activo 
						where	idRol = @idRol

						select @mensaje = 'Rol modificado correctamente.'
					end
				-- si es nuevo
				else
					begin
						if ( @existeDescripcion = cast(0 as bit) )
							begin
								select @activo = cast(1 as bit)
								insert into CatRoles (descripcion,activo) values (@descripcion, @activo)
								select @mensaje = 'Rol agregado correctamente.'
							end
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
					@mensaje mensaje
				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_INSERTA_ACTUALIZA_ROLES to public
go



