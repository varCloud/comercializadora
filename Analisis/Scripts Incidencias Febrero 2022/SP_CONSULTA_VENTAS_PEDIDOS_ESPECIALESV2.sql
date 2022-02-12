--use DB_A57E86_lluviadesarrollo
--go

-- se crea procedimiento SP_CONSULTA_VENTAS_PEDIDOS_ESPECIALESV2
if exists (select * from sysobjects where name like 'SP_CONSULTA_VENTAS_PEDIDOS_ESPECIALESV2' and xtype = 'p' )
	drop proc SP_CONSULTA_VENTAS_PEDIDOS_ESPECIALESV2
go

/*

Autor			Ernesto Aguilar
Fecha			2022/02/07
Objetivo		CONSULTA LAS VENTAS REALIZADAS DE LOS PEDIDOS ESPECIALES PARA VISTE DE REPORTES

*/

create proc SP_CONSULTA_VENTAS_PEDIDOS_ESPECIALESV2

	--@idLineaProducto		int = null,
	@idCliente				int = null,
	@idUsuario				int = null,
	@fechaIni				datetime = null,
	@fechaFin				datetime = null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status							int = 200,
						@error_message						varchar(255) = '',
						@error_line						varchar(255) = '',
						@error_procedure				varchar(255) = '',
						@valido							bit = cast(1 as bit),
						@hoy							datetime,
						@top							bigint = 0x7fffffffffffffff --valor màximo


				create table
					#PedidosEspeciales
						(
							contador					int identity(1,1),
							fechaAlta					datetime,
							sucursal					varchar(300),
							tienda						varchar(300),
							idUsuario					int,
							nombreUsuario				varchar(300),
							idPedidoEspecial			int ,
							idCliente					int,
							nombreCliente				varchar(300),
							codigoBarrasTicket			varchar(100),
							linea						varchar(300),
							producto					varchar(300),
							cantidad					float,
							precioVenta					float,							
							montoTotal					money,
							montoIVA					money,
							ultimoCostoCompra			money,
							idFactFormaPago				int,
							descripcionFactFormaPago	varchar(50),
							idFactura					int, 
							idEstatusFactura			int,
							descripcionEstatusFactura	varchar(300),
							rutaFactura					varchar(max),
							ganancia					money,	
							fechaCancelacion			datetime
						)			
						
						
			end  --declaraciones 

			begin -- principal

				-- validaciones
					if (	
							(@idCliente is null) and 
							(@idUsuario is null) and 
							(@fechaIni is null) and 
							(@fechaFin is null) 
						)
					begin
						select	@error_message = 'Debe elejir al menos un criterio para la búsqueda de la Venta del Pedido Especial.',
								@valido = cast(0 as bit)						
						raiserror (@error_message, 11, -1)
					end


				-- validamos si la columnas de las fecha vienen en null o el formatobase seteamos la fecha original
					if  convert(varchar , @fechaIni, 112) = '19000101'
					   set @fechaIni = dbo.FechaActual()
					if  convert(varchar , @fechaFin, 112) = '19000101'
					  set @fechaFin = dbo.FechaActual()


					insert into 
						#PedidosEspeciales 
							(
								idPedidoEspecial,idCliente,nombreCliente,cantidad,fechaAlta,idUsuario,nombreUsuario,idFactura,
								idEstatusFactura,descripcionEstatusFactura,rutaFactura,codigoBarrasTicket,montoTotal,montoIVA,
								idFactFormaPago,descripcionFactFormaPago, fechaCancelacion, sucursal, tienda, precioVenta, producto, 
								ganancia, ultimoCostoCompra, linea
							)

					select	top (@top) p.idPedidoEspecial, p.idCliente, (cl.nombres + ' ' + cl.apellidoPaterno + ' ' + cl.apellidoMaterno) as nombreCliente, ped.cantidad,
							p.fechaAlta, p.idUsuario,  (u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno) as nombreUsuario, f.idFacturaPedidoEspecial, f.idEstatusFactura,
							s.descripcion, f.pathArchivoFactura+'/'+'Factura_'+cast(p.idPedidoEspecial as varchar)+'.pdf' as rutaFactura, p.codigoBarras,
							ped.monto, ped.montoIva, p.idFactFormaPago, formaPago.descripcion, p.fechaCancelacion, suc.sucursal, suc.tienda, ped.precioVenta, ped.producto, 
							case when coalesce(ped.ultimoCostoCompra,0)=0 then 0 else (coalesce(ped.precioVenta,0)-coalesce(ped.ultimoCostoCompra,0)) end as ganancia,
							ped.ultimoCostoCompra, ped.linea
					from	PedidosEspeciales p
								join	(
											select	idPedidoEspecial, cantidad, monto, coalesce(montoIva, 0.0) as montoIva, det.precioVenta,
													pro.idProducto, pro.descripcion as producto, pro.ultimoCostoCompra, lp.descripcion as linea
											from	PedidosEspecialesDetalle det
														join Productos pro
															on pro.idProducto = det.idProducto
														join LineaProducto lp
															on lp.idLineaProducto = pro.idLineaProducto	
										)ped
											on ped.idPedidoEspecial = p.idPedidoEspecial
								join	(
											select	ped_.idPedidoEspecial, a_.descripcion as tienda, sucu_.descripcion as sucursal
											from	PedidosEspecialesDetalle ped_
														join Almacenes a_
															on a_.idAlmacen = ped_.idAlmacenOrigen
														join CatSucursales sucu_
															on sucu_.idSucursal = a_.idSucursal
											group by ped_.idPedidoEspecial, a_.descripcion, sucu_.descripcion											
										) suc
											on suc.idPedidoEspecial = p.idPedidoEspecial
								join Usuarios u
									on u.idUsuario = p.idUsuario
								left join Clientes cl
									on p.idCliente = cl.idCliente								
								left join FacturasPedidosEspeciales f
									on f.idPedidoEspecial = p.idPedidoEspecial
								left join FacCatEstatusFactura s
									on s.idEstatusFactura = f.idEstatusFactura
								left join FactCatFormaPago formaPago 
									on p.idFactFormaPago = formaPago.id		
								
					where	p.idCliente =	case
												when @idCliente is null then p.idCliente
												when @idCliente = 0 then p.idCliente
												else @idCliente
											end

						and p.idUsuario =	case
												when @idUsuario is null then p.idUsuario
												when @idUsuario = 0 then p.idUsuario
												else @idUsuario
											end

						and cast(p.fechaAlta as date) >=	case
																when @fechaIni is null then cast(p.fechaAlta as date)
																when @fechaIni = 0 then cast(p.fechaAlta as date)
																when @fechaIni = '19000101' then cast(p.fechaAlta as date)
																else cast(@fechaIni as date)
															end

						and cast(p.fechaAlta as date) <=	case
																when @fechaFin is null then cast(p.fechaAlta as date)
																when @fechaFin = 0 then cast(p.fechaAlta as date)
																when @fechaFin = '19000101' then cast(p.fechaAlta as date)
																else cast(@fechaFin as date)
															end

					order by p.fechaAlta desc

				
				if not exists ( select 1 from #PedidosEspeciales )
					begin
						select	@valido = cast(0 as bit),
								@status = -1,
								@error_message = 'No se encontraron ventas con esos términos de búsqueda.'
					end

			end -- principal

		end try

		begin catch 
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@error_message = error_message()

		end catch

		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_message error_message


		-- si todo bien
		if ( @valido = cast(1 as bit) )
		begin
			select	fechaAlta, 					
					sucursal, 
					tienda,
					idUsuario,
					nombreUsuario,
					idPedidoEspecial,
					idCliente,
					nombreCliente,
					codigoBarrasTicket,
					linea,
					producto,
					cantidad,
					precioVenta,
					montoTotal,
					montoIVA,
					ultimoCostoCompra,
					ganancia,
					idFactFormaPago,
					descripcionFactFormaPago,
					idFactura,
					idEstatusFactura,
					descripcionEstatusFactura,
					rutaFactura
			from	#PedidosEspeciales
			order by fechaalta
		end

		drop table #PedidosEspeciales
					
		end -- reporte de estatus


	end  -- principal
go

grant exec on SP_CONSULTA_VENTAS_PEDIDOS_ESPECIALESV2 to public
go
