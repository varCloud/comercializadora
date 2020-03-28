
use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_TIPOS_CLIENTES
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_TIPOS_CLIENTES' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_TIPOS_CLIENTES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza los diferentes tipos de clientes del sistema
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_TIPOS_CLIENTES

  @idTipoCliente		int,
  @descripcion			varchar(50),
  @descuento			money,
  @activo				bit

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Tipo de Cliente sin modificaciones',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeTipoCliente		bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if exists ( select 1 from CatTipoCliente where idTipoCliente = @idTipoCliente )
					begin
						select @existeTipoCliente = cast(1 as bit)
					end
					
				-- si es modificacion
				if	( (@idTipoCliente > 0) )
					begin
						
						if not exists ( select 1 from CatTipoCliente where idTipoCliente = @idTipoCliente ) 
						begin
							select @mensaje = 'El Tipo de Cliente no existe.'
							raiserror (@mensaje, 11, -1)
						end

						if ( @existeTipoCliente = cast(1 as bit)) 
							begin
							
								update	CatTipoCliente 
								set		descripcion = @descripcion,
										descuento = @descuento,
										activo = @activo
								where	idTipoCliente = @idTipoCliente
						
								select	@mensaje = 'Tipo de Cliente Modificado Correctamente.'
							
							end

					end
				-- si es nuevo
				else
					begin
						if ( @existeTipoCliente = cast(0 as bit) )
							begin
								select @activo = cast(1 as bit)
								
								insert into CatTipoCliente (descripcion,descuento,activo)
								values (@descripcion,@descuento,@activo)

								select @mensaje = 'Tipo de Cliente Agregado Correctamente.'
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

grant exec on SP_INSERTA_ACTUALIZA_TIPOS_CLIENTES to public
go

