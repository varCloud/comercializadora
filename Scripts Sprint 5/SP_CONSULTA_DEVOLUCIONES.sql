use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_DEVOLUCIONES
if exists (select * from sysobjects where name like 'SP_CONSULTA_DEVOLUCIONES' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_CONSULTA_DEVOLUCIONES
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta las devoluciones de productos en las ventas
status			200 = ok
				-1	= error
*/

create proc SP_CONSULTA_DEVOLUCIONES

	@idVenta		int = null,
	@idAlmacen		int = null,
	@idUsuario		int = null,
	@fechaIni		datetime = null,
	@fechaFin		datetime = null
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
				
				
				select	v.idVenta, v.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
						v.idCliente, c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno as nombreCliente,
						u.idSucursal, u.idAlmacen, vd.idProducto, p.descripcion as descripcionProducto ,vd.cantidad, vd.monto,
						vd.precioIndividual, vd.precioMenudeo, vd.precioRango, vd.precioVenta, vd.montoIva, 
						vd.productosDevueltos, v.fechaAlta, a.Descripcion as descAlmacen
				into	#VentasDetalle
				from	VentasDetalle vd
							inner join Ventas v 
								on v.idVenta = vd.idVenta
							inner join Usuarios u
								on u.idUsuario = v.idUsuario
							inner join Almacenes a 
								on a.idAlmacen = u.idAlmacen
							inner join Clientes c
								on c.idCliente = v.idCliente
							inner join Productos p
								on p.idProducto = vd.idProducto
				where	vd.idVenta = case
										when @idVenta is null then vd.idVenta
										when @idVenta = 0 then vd.idVenta
										else @idVenta
								  end
					and	u.idAlmacen = case
										when @idAlmacen is null then u.idAlmacen
										when @idAlmacen = 0 then u.idAlmacen
										else @idAlmacen
								  end
					and	v.idUsuario = case
										when @idUsuario is null then v.idUsuario
										when @idUsuario = 0 then v.idUsuario
										else @idUsuario
								  end

					and cast(v.fechaAlta as date) >=	case
															when @fechaIni is null then cast(v.fechaAlta as date)
															when @fechaIni = 0 then cast(v.fechaAlta as date)
															when @fechaIni = '19000101' then cast(v.fechaAlta as date)
															else cast(@fechaIni as date)
														end

					and cast(v.fechaAlta as date) <=	case
															when @fechaFin is null then cast(v.fechaAlta as date)
															when @fechaFin = 0 then cast(v.fechaAlta as date)
															when @fechaFin = '19000101' then cast(v.fechaAlta as date)
															else cast(@fechaFin as date)
														end

					and	vd.productosDevueltos > 0
				order by vd.idVenta


				if not exists (	select 1 from #VentasDetalle )
				begin
					select	@mensaje = 'No existen devoluciones para esos terminos de búsqueda.',
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

		--reporte de estatus
			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
							

		-- si todo ok
			if exists ( select 1 from #VentasDetalle )
			begin
				select	*
				from	#VentasDetalle
			end

				
		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_CONSULTA_DEVOLUCIONES to public
go



