use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_CLIENTES
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_CLIENTES' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_CLIENTES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza las diferentes lineas de producto del sistema
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_CLIENTES

	@idCliente				int,
	@nombres				varchar(50),
	@apellidoPaterno		varchar(50),
	@apellidoMaterno		varchar(50),
	@telefono				varchar(50),
	@correo					varchar(50),
	@rfc					varchar(50),
	@calle					varchar(50),
	@numeroExterior			varchar(50),
	@colonia				varchar(50),
	@municipio				varchar(50),
	@cp						varchar(50),
	@estado					varchar(50),
	@fechaAlta				varchar(50),
	@activo					varchar(50),
	@idTipoCliente			int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Cliente sin modificaciones',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeCliente			bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if exists ( select 1 from clientes where nombres like @nombres and apellidoPaterno like @apellidoPaterno and apellidoMaterno like @apellidoMaterno )
					begin
						select @existeCliente = cast(1 as bit)
					end
					
				-- si es modificacion
				if	( (@idCliente > 0) )
					begin
						
						if not exists ( select 1 from Clientes where idCliente = @idCliente ) 
						begin
							select @mensaje = 'El Cliente no existe.'
							raiserror (@mensaje, 11, -1)
						end

						if ( (@existeCliente = cast(1 as bit)) and ( (@activo is null) or (@activo = 0) ) )
							begin
								select	@activo = activo
								from	Clientes
								where	idCliente = @idCliente
							end
							
						update	Clientes 
						set		nombres = @nombres,
								apellidoPaterno = @apellidoPaterno,
								apellidoMaterno = @apellidoMaterno,
								telefono = @telefono,
								correo = @correo,
								rfc = @rfc,
								calle = @calle,
								numeroExterior = @numeroExterior,
								colonia = @colonia,
								municipio = @municipio,
								cp = @cp,
								estado = @estado,
								fechaAlta = @fechaAlta,
								activo = @activo,
								idTipoCliente = @idTipoCliente
						where	idCliente = @idCliente
						
						select	@mensaje = 'Cliente modificado correctamente.'
					end
				-- si es nuevo
				else
					begin
						if ( @existeCliente = cast(0 as bit) )
							begin
								select @activo = cast(1 as bit)
								--select @idCliente = coalesce( max(idLineaProducto) + 1, 1) from clientes
								insert into Clientes (nombres,apellidoPaterno,apellidoMaterno,telefono,correo,rfc,calle,numeroExterior,colonia,municipio,cp,estado,fechaAlta,activo,idTipoCliente) 
								values (@nombres,@apellidoPaterno,@apellidoMaterno,@telefono,@correo,@rfc,@calle,@numeroExterior,@colonia,@municipio,@cp,@estado,@fechaAlta,@activo,@idTipoCliente) 
								select @mensaje = 'Cliente agregado correctamente.'
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

grant exec on SP_INSERTA_ACTUALIZA_CLIENTES to public
go



