use DB_A552FA_comercializadora
go

-- se crea procedimiento SP_CONSULTA_COMPRAS
if exists (select * from sysobjects where name like 'SP_CONSULTA_COMPRAS' and xtype = 'p' and db_name() = 'DB_A552FA_comercializadora')
	drop proc SP_CONSULTA_COMPRAS
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta las compras hechas a los proveedores
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_COMPRAS

	@idProducto				int = null,
	@descProducto			varchar(300) = null,
	@idProveedor			int = null,
	@idLineaProducto		int = null,
	@idUsuario				int = null,
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
					#Compras
						(
							idCompra					int,
							idProducto					int,
							descripcionProducto			varchar(300),
							idProveedor					int,
							descripcionProveedor		varchar(300),
							idUsuario					int,
							nombreUsuario				varchar(300),
							fechaAlta					datetime,
							cantidad					int,
							idLineaProducto				int,
							descripcionLineaProducto	varchar(300)
						)
						
			end  --declaraciones 

			begin -- principal
				
				-- validaciones
					if (	(@idProducto is null) and 
							(@idProveedor is null) and
							(@idUsuario is null) and
							(@fechaIni is null) and
							(@fechaFin is null)
						)
					begin
						select	@mensaje = 'Debe elejir al menos un criterio para la búsqueda de la Compra.',
								@valido = cast(0 as bit)						
						raiserror (@mensaje, 11, -1)
					end

				-- si son todos
					if (	(@idProducto = 0) and 
							(@idProveedor = 0) and
							(@idLineaProducto = 0) and
							(@idUsuario = 0) and
							(@fechaIni = '19000101') and
							(@fechaFin = '19000101')
						)
					begin

						insert into #Compras (idCompra,idProducto,descripcionProducto,idProveedor,descripcionProveedor,idUsuario,nombreUsuario,fechaAlta,cantidad,idLineaProducto,descripcionLineaProducto)
						select	top 50 c.idCompra, c.idProducto, p.descripcion as descripcionProducto, c.idProveedor, pro.nombre as descripcionProveedor, c.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario, c.fechaAlta, c.cantidad,lp.idLineaProducto,lp.descripcion
						from	Compras c
									inner join Productos p
										on p.idProducto = c.idProducto
									inner join Usuarios u
										on u.idUsuario = c.idUsuario
									inner join Proveedores pro
										on pro.idProveedor = c.idProveedor					
									inner join LineaProducto lp
										on lp.idLineaProducto = p.idLineaProducto											

						order by idCompra desc

					end
				-- si es por busqueda
				else 
					begin

						insert into #Compras (idCompra,idProducto,descripcionProducto,idProveedor,descripcionProveedor,idUsuario,nombreUsuario,fechaAlta,cantidad,idLineaProducto,descripcionLineaProducto)
						select	c.idCompra, c.idProducto, p.descripcion as descripcionProducto, c.idProveedor, pro.nombre as descripcionProveedor, c.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario, c.fechaAlta, c.cantidad,lp.idLineaProducto,lp.descripcion
						from	Compras c
									inner join Productos p
										on p.idProducto = c.idProducto
									inner join Usuarios u
										on u.idUsuario = c.idUsuario
									inner join Proveedores pro
										on pro.idProveedor = c.idProveedor
									inner join LineaProducto lp
										on lp.idLineaProducto = p.idLineaProducto											

						where	c.idProducto =	case
													when @idProducto is null then c.idProducto
													when @idProducto = 0 then c.idProducto
													else @idProducto
												end

							and p.descripcion like	case
														when @descProducto is null then p.descripcion
														else '%' + @descProducto + '%'
													end

							and c.idProveedor =	case
														when @idProveedor is null then c.idProveedor
														when @idProveedor = 0 then c.idProveedor
														else @idProveedor
													end

							and c.idUsuario =	case
														when @idUsuario is null then c.idUsuario
														when @idUsuario = 0 then c.idUsuario
														else @idUsuario
													end

							and lp.idLineaProducto =	case
															when @idLineaProducto is null then lp.idLineaProducto
															when @idLineaProducto = 0 then lp.idLineaProducto
															else @idLineaProducto
														end

							and cast(c.fechaAlta as date) >=	case
																	when @fechaIni is null then cast(c.fechaAlta as date)
																	when @fechaIni = 0 then cast(c.fechaAlta as date)
																	when @fechaIni = '19000101' then cast(c.fechaAlta as date)
																	else cast(@fechaIni as date)
																end

							and cast(c.fechaAlta as date) <=	case
																	when @fechaFin is null then cast(c.fechaAlta as date)
																	when @fechaFin = 0 then cast(c.fechaAlta as date)
																	when @fechaFin = '19000101' then cast(c.fechaAlta as date)
																	else cast(@fechaFin as date)
																end

					end

				
				if not exists ( select 1 from #Compras )
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
					select	* from	#Compras order by idCompra desc 
				end
				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_COMPRAS to public
go



