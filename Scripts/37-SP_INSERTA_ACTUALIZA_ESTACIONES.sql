use DB_A57E86_comercializadora
go

-- se crea procedimiento SP_INSERTA_ACTUALIZA_ESTACIONES
if exists (select * from sysobjects where name like 'SP_INSERTA_ACTUALIZA_ESTACIONES' and xtype = 'p' and db_name() = 'DB_A57E86_comercializadora')
	drop proc SP_INSERTA_ACTUALIZA_ESTACIONES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		inserta y actualiza las diferentes estaciones del sistema
status			200 = ok
				-1	= error
*/

create proc SP_INSERTA_ACTUALIZA_ESTACIONES

@idEstacion		int,
@idAlmacen		int,
@macAdress		varchar(250),
@nombre			varchar(50),
@numero			int,
@configurado	bit,
@idUsuario		int
--@fechaAlta		datetime,
--@idStatus		int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Estacion sin modificaciones',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@existeEstacion			bit = cast(0 as bit)

			end  --declaraciones 

			begin -- principal
				
				if exists ( select 1 from Estaciones where nombre like @nombre )
					begin
						select @existeEstacion = cast(1 as bit)
					end
					
				-- si es modificacion
				if	( (@idEstacion > 0) )
					begin
						
						if not exists ( select 1 from Estaciones where idEstacion = @idEstacion ) 
						begin
							select @mensaje = 'La Estacion no existe.'
							raiserror (@mensaje, 11, -1)
						end

						if @idAlmacen = 0
							select @idAlmacen = idAlmacen from Estaciones where idEstacion	= @idEstacion

						if @nombre is null
							select @nombre = nombre from Estaciones where idEstacion = @idEstacion

						if @numero = 0
							select @numero = numero from Estaciones where idEstacion = @idEstacion

						if @idUsuario = 0
							select @idUsuario = idUsuario from Estaciones where idEstacion = @idEstacion

						update	Estaciones 
						set		idAlmacen	= @idAlmacen,
								macAdress	= @macAdress,
								nombre		= @nombre,
								numero		= @numero,
								configurado = @configurado,
								idUsuario	= @idUsuario,
								fechaAlta	= getdate()
						where	idEstacion	= @idEstacion
						
						select	@mensaje = 'Estacion modificada correctamente.'
					end
				-- si es nuevo
				else
					begin
						if ( @existeEstacion = cast(0 as bit) )
							begin
								
								insert into Estaciones (idAlmacen,macAdress,nombre,numero,configurado,idUsuario,fechaAlta,idStatus) 
								values (@idAlmacen,@macAdress,@nombre,@numero,@configurado,@idUsuario,getdate(),cast(1 as bit)) 
								select @mensaje = 'Estacion agregada correctamente.'
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

grant exec on SP_INSERTA_ACTUALIZA_ESTACIONES to public
go



