use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_PRODUCTOS
if exists (select * from sysobjects where name like 'SP_CONSULTA_PRODUCTOS' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_PRODUCTOS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes clientes del sistema
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_PRODUCTOS

	@idProducto				int = null,
	@descripcion			varchar(255) = null,
	@idUnidadMedida			int = null,
	@idLineaProducto		int = null,
	@activo					bit = null,
	@articulo				varchar(255) = null,
	@fechaIni				datetime = null,
	@fechaFin				datetime = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)

				create table
					#Productos
						(
							idProducto				int identity (1,1),
							descripcion				varchar(100),
							idUnidadMedida			int,
							idLineaProducto			int,
							cantidadUnidadMedida	float,
							codigoBarras			nvarchar(4000),
							fechaAlta				datetime,
							activo					bit,
							articulo				varchar(100)					
						)
						
			end  --declaraciones 

			begin -- principal
				
				-- validaciones
					if (@idProducto is null and @descripcion is null and @idUnidadMedida is null and 
						@idLineaProducto is null and @activo is null and @articulo is null )
					begin
						select	@mensaje = 'Debe elejir al menos un criterio para la búsqueda del Producto.',
								@valido = cast(0 as bit)						
						raiserror (@mensaje, 11, -1)
					end

				-- si son todos
				if	( 
						( @idProducto = 0 ) and 
						( @descripcion is null ) and 
						( @idUnidadMedida = 0 ) and 
						( @idLineaProducto = 0 ) and 
						( @activo = 0 ) and 
						( @articulo is null ) and					
						( @fechaIni = '19000101' ) and					
						( @fechaFin = '19000101' ) 
					)
					begin

						insert into #Productos (descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,fechaAlta,activo,articulo)
						select	top 50 descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,fechaAlta,activo,articulo
						from	Productos
						where	activo = cast(1 as bit)
						order by idProducto desc						

					end
				-- si es por busqueda
				else 
					begin

						insert into #Productos (descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,fechaAlta,activo,articulo)
						select	descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,fechaAlta,activo,articulo
						from	Productos
						where	idProducto =	case
													when @idProducto is null then idProducto
													when @idProducto = 0 then idProducto
													else @idProducto
												end

							and descripcion like	case
														when @descripcion is null then descripcion
														else '%' + @descripcion + '%'
													end

							and idLineaProducto =	case
														when @idLineaProducto is null then idLineaProducto
														when @idLineaProducto = 0 then idLineaProducto
														else @idLineaProducto
													end

							and articulo like	case
													when @articulo is null then articulo
													else '%' + @articulo + '%' 
												end

							and cast(fechaAlta as date) >=	case
																	when @fechaIni is null then cast(fechaAlta as date)
																	when @fechaIni = 0 then cast(fechaAlta as date)
																	when @fechaIni = '19000101' then cast(fechaAlta as date)
																	else cast(@fechaIni as date)
																end

							and cast(fechaAlta as date) <=	case
																	when @fechaFin is null then cast(fechaAlta as date)
																	when @fechaFin = 0 then cast(fechaAlta as date)
																	when @fechaFin = '19000101' then cast(fechaAlta as date)
																	else cast(@fechaFin as date)
																end




							and activo = cast(1 as bit)

					end

				
				if not exists ( select 1 from #Productos )
					begin
						select	@valido = cast(0 as bit),
								@status = -1,
								@mensaje = 'No se encontraron productos con esos términos de búsqueda.'
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

		--reporte de estatus
			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
							

		-- si todo ok
			if ( @valido = 1 )
				begin
				
					select	p.*, l.descripcion as DescripcionLinea, u.descripcion as DescripcionUnidadMedida, g.cantidad
					from	#Productos p
								inner join LineaProducto l 
									on p.idLineaProducto = l.idLineaProducto
								inner join CatUnidadMedida u
									on p.idUnidadMedida = u.idUnidadMedida
								left join InventarioGeneral g
									on g.idProducto = p.idProducto
					order by idProducto desc 

				end
				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_PRODUCTOS to public
go



