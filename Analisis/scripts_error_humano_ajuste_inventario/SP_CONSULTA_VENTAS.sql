USE [DB_A57E86_comercializadora]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_VENTAS]    Script Date: 07/08/2024 06:55:06 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

exec SP_CONSULTA_VENTAS
@idProducto = 0,
@descProducto = null,
@idLineaProducto = 0,
@idCliente = 0,
@idUsuario = 0,
@fechaIni = '19000101',
@fechaFin = '19000101',
@tipoConsulta = 2,
@idStatusVenta = 1

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta las ventas hechas a los clientes
status			200 = ok
				-1	= error
				tipoConsulta: 1-para reportes / 2- para modulo de ventas
*/

ALTER proc [dbo].[SP_CONSULTA_VENTAS]

	@idProducto				int = null,
	@descProducto			varchar(300) = null,
	@idLineaProducto		int = null,
	@idCliente				int = null,
	@idUsuario				int = null,
	@fechaIni				datetime = null,
	@fechaFin				datetime = null,
	@tipoConsulta			int = null,
	@idStatusVenta			int=1,
	@idFactFormaPago		int=null,
	@idAlmacen				int=null				   

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status							int = 200,
						@mensaje						varchar(255) = '',
						@error_line						varchar(255) = '',
						@error_procedure				varchar(255) = '',
						@valido							bit = cast(1 as bit),
						@hoy							datetime,
						@top bigint=0x7fffffffffffffff--valor màximo

				create table
					#Ventas	
						(
							contador					int identity(1,1),
							idVenta						int ,
							idCliente					int,
							nombreCliente				varchar(300),
							cantidad					float,
							fechaAlta					datetime,
							idUsuario					int,
							nombreUsuario				varchar(300),
							idProducto					int,
							descripcionProducto			varchar(300),							
							idLineaProducto				int,
							descripcionLineaProducto	varchar(300),
							idFactura					int, 
							idEstatusFactura			int,
							descripcionEstatusFactura	varchar(300),
							fechaTimbrado				datetime,
							rutaFactura					varchar(max),
							precio						float,
							montoIva					float,
							costo						float,
							codigoBarras				varchar(100),
							tipoCliente					varchar(50),
							descSucursal				varchar(100),
							productosDevueltos			int,
							productosAgregados			int,
							cantProductosLiq			int,
							codigoBarrasTicket			varchar(100),
							fechaCancelacion			datetime,
							montoTotal					money,
							idFactFormaPago				int,
							descripcionFactFormaPago	varchar(50),
							puedeHacerComplementos		bit,
							descripcionAlmacen          varchar(1000),
							diasPasadosVentaInicial		int
						)			

				-- para ver hasta cuantos dias pueden hacerse los complementos y devoluciones
				select @hoy = dbo.FechaActual()

			end  --declaraciones 

			begin -- principal 
				
				-- validaciones
					if (	
							(@idProducto is null) and 
							(@descProducto is null) and 
							(@idCliente is null) and 
							(@idUsuario is null) and 
							(@fechaIni is null) and 
							(@fechaFin is null) and
							(@idAlmacen is null) 
						)
					begin
						select	@mensaje = 'Debe elejir al menos un criterio para la búsqueda de la Venta.',
								@valido = cast(0 as bit)						
						raiserror (@mensaje, 11, -1)
					end
				-- validamos si la columnas de las fecha vienen en null o el formatobase seteamos la fecha original
					if  convert(varchar , @fechaIni, 112) = '19000101'
					   set @fechaIni = dbo.FechaActual()
					if  convert(varchar , @fechaFin, 112) = '19000101'
					  set @fechaFin = dbo.FechaActual()

				-- si son todos
					if (	
							(@idProducto is null) and 
							(@descProducto is null) and 
							(@idLineaProducto is null) and 
							(@idCliente is null) and 
							(@idUsuario is null) and 
							(@fechaIni = '19000101') and
							(@fechaFin = '19000101')
						)
					begin
					print 'todo'
						insert into #Ventas (idVenta,idCliente,nombreCliente,cantidad,fechaAlta,idUsuario,nombreUsuario,idProducto,descripcionProducto,idLineaProducto,descripcionLineaProducto,idFactura,idEstatusFactura,descripcionEstatusFactura,fechaTimbrado,rutaFactura,precio,costo,codigoBarras,tipoCliente,descSucursal,productosDevueltos,productosAgregados,cantProductosLiq,codigoBarrasTicket,fechaCancelacion,
						montoTotal,idFactFormaPago,descripcionFactFormaPago,puedeHacerComplementos,descripcionAlmacen,diasPasadosVentaInicial, montoIva)
						select	top 100 v.idVenta,v.idCliente, cl.nombres + ' ' + cl.apellidoPaterno + ' ' + cl.apellidoMaterno as nombreCliente
								,vd.cantidad, v.fechaAlta, v.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
								p.idProducto,p.descripcion, lp.idLineaProducto, lp.descripcion, f.idFactura, f.idEstatusFactura, s.descripcion,f.fechaTimbrado,f.pathArchivoFactura,
								vd.precioVenta,p.ultimoCostoCompra,
								p.codigoBarras,t.descripcion,suc.descripcion, v.devoluciones, v.productosAgregados,
								case when p.idLineaProducto in (19,20) then vd.cantidad else 0 end cantProductosLiq, v.codigoBarras as codigoBarrasTicket,
								v.fechaCancelacion,v.montoTotal,v.idFactFormaPago,formaPago.descripcion descripcionFactFormaPago, v.puedeHacerComplementos
								,a.Descripcion, DATEDIFF(day, v.fechaAlta, @hoy) as diasPasadosVentaInicial, vd.montoIva
						from	Ventas v
									inner join Usuarios u
										on u.idUsuario = v.idUsuario
									left join Clientes cl
										on v.idCliente = cl.idCliente
									inner join VentasDetalle vd
										on vd.idVenta = v.idVenta
									inner join Productos p
										on vd.idProducto = p.idProducto	
									inner join LineaProducto lp
										on lp.idLineaProducto = p.idLineaProducto
									left join Facturas f
										on f.idVenta = v.idVenta
									left join FacCatEstatusFactura s
										on s.idEstatusFactura = f.idEstatusFactura
									left join [dbo].[CatTipoCliente] t on cl.idTipoCliente=t.idTipoCliente
									left join Estaciones e on v.idestacion=e.idestacion 
									left join Almacenes a on e.idAlmacen=a.idAlmacen
									left join CatSucursales suc on a.idSucursal=suc.idSucursal
									left join FactCatFormaPago formaPago on v.idFactFormaPago=formaPago.id
						where	v.idStatusVenta = @idStatusVenta
							and	vd.cantidad > 0
						order by v.fechaAlta desc
					end
				-- si es por busqueda
				else 
					begin
					print '2111'
						insert into #Ventas (idVenta,idCliente,nombreCliente,cantidad,fechaAlta,idUsuario,nombreUsuario,idProducto,descripcionProducto,idLineaProducto,descripcionLineaProducto,idFactura,idEstatusFactura,descripcionEstatusFactura,fechaTimbrado,rutaFactura,precio,costo,codigoBarras,tipoCliente,descSucursal,productosDevueltos,productosAgregados,cantProductosLiq,codigoBarrasTicket,fechaCancelacion,
						montoTotal,idFactFormaPago,descripcionFactFormaPago,puedeHacerComplementos,descripcionAlmacen,diasPasadosVentaInicial,montoIva)
						select	top (@top) v.idVenta,v.idCliente, cl.nombres + ' ' + cl.apellidoPaterno + ' ' + cl.apellidoMaterno as nombreCliente
								,vd.cantidad, v.fechaAlta, v.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
								p.idProducto,p.descripcion, lp.idLineaProducto, lp.descripcion, f.idFactura, f.idEstatusFactura, s.descripcion,f.fechaTimbrado,f.pathArchivoFactura,
								vd.precioVenta,p.ultimoCostoCompra,
								p.codigoBarras,t.descripcion,suc.descripcion, v.devoluciones, v.productosAgregados,
								case when p.idLineaProducto in (19,20) then vd.cantidad else 0 end cantProductosLiq, v.codigoBarras as codigoBarrasTicket,
								v.fechaCancelacion,v.montoTotal,v.idFactFormaPago,formaPago.descripcion descripcionFactFormaPago, v.puedeHacerComplementos
								,a.Descripcion, DATEDIFF(day, v.fechaAlta, @hoy) as diasPasadosVentaInicial, vd.montoIva
						from	Ventas v
									inner join Usuarios u
										on u.idUsuario = v.idUsuario
									left join Clientes cl
										on v.idCliente = cl.idCliente
									inner join VentasDetalle vd
										on vd.idVenta = v.idVenta
									inner join Productos p
										on vd.idProducto = p.idProducto
									inner join LineaProducto lp
										on lp.idLineaProducto = p.idLineaProducto	
									left join Facturas f
										on f.idVenta = v.idVenta
									left join FacCatEstatusFactura s
										on s.idEstatusFactura = f.idEstatusFactura
									left join [dbo].[CatTipoCliente] t on cl.idTipoCliente=t.idTipoCliente
									left join Estaciones e on v.idestacion=e.idestacion 
									left join Almacenes a on e.idAlmacen=a.idAlmacen
									left join CatSucursales suc on a.idSucursal=suc.idSucursal
									left join FactCatFormaPago formaPago on v.idFactFormaPago=formaPago.id									
						where	p.idProducto =	case
													when @idProducto is null then p.idProducto
													when @idProducto = 0 then p.idProducto
													else @idProducto
												end

							and p.descripcion like	case
														when @descProducto is null then p.descripcion
														else '%' + @descProducto + '%'
													end

							and	v.idCliente =	case
													when @idCliente is null then v.idCliente
													when @idCliente = 0 then v.idCliente
													else @idCliente
												end

							and v.idUsuario =	case
													when @idUsuario is null then v.idUsuario
													when @idUsuario = 0 then v.idUsuario
													else @idUsuario
												end

							and v.idFactFormaPago =	case
													when @idFactFormaPago is null then v.idFactFormaPago
													when @idFactFormaPago = 0 then v.idFactFormaPago
													else @idFactFormaPago
												end

							and lp.idLineaProducto =	case
															when @idLineaProducto is null then lp.idLineaProducto
															when @idLineaProducto = 0 then lp.idLineaProducto
															else @idLineaProducto
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
							and u.idAlmacen					=   case 
																	when coalesce(@idAlmacen,0)>0 then @idAlmacen 
																	else u.idAlmacen 
																end
							and	v.idStatusVenta = @idStatusVenta
							and	vd.cantidad > 0
						group by 
							vd.idProducto, p.idLineaProducto, v.idVenta,v.idCliente, cl.nombres ,cl.apellidoPaterno , cl.apellidoMaterno,
							vd.cantidad, v.fechaAlta, v.idUsuario, u.nombre, u.apellidoPaterno, u.apellidoMaterno, p.idProducto, p.descripcion, 
							lp.idLineaProducto, lp.descripcion, f.idFactura, f.idEstatusFactura, s.descripcion,f.fechaTimbrado,f.pathArchivoFactura, 
							vd.precioVenta,p.codigoBarras,t.descripcion,suc.descripcion, v.devoluciones, v.productosAgregados,	vd.cantidad, 
							v.codigoBarras, v.fechaCancelacion,v.montoTotal,v.idFactFormaPago,formaPago.descripcion,p.ultimoCostoCompra, v.puedeHacerComplementos,
							a.Descripcion, vd.montoIva

						order by v.fechaAlta desc

					end

				
				if not exists ( select 1 from #Ventas )
					begin
						select	@valido = cast(0 as bit),
								@status = -1,
								@mensaje = 'No se encontraron ventas con esos términos de búsqueda.'
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

					if ( @tipoConsulta = 2 )
						begin
							select	
									-- top 50
									-- ROW_NUMBER() OVER(ORDER BY idVenta DESC) AS contador,
									idVenta,idCliente,
									case
										when nombreCliente is null then 'PÚBLICO EN GENERAL' 
										else nombreCliente
									end as nombreCliente,
									SUM(cantidad) cantidad,fechaAlta,idUsuario,nombreUsuario,idFactura, idEstatusFactura, descripcionEstatusFactura,
									rutaFactura+'/'+'Factura_'+cast(idVenta as varchar)+'.pdf' as rutaFactura,
									productosDevueltos, productosAgregados,sum(cantProductosLiq) cantProductosLiq,codigoBarrasTicket,fechaCancelacion,
									montoTotal,idFactFormaPago,descripcionFactFormaPago,puedeHacerComplementos
									,descripcionAlmacen, diasPasadosVentaInicial, montoIva
							from	#Ventas 
							group by idVenta,idCliente,nombreCliente,fechaAlta,idUsuario,nombreUsuario,idFactura,idEstatusFactura,descripcionEstatusFactura,fechaTimbrado,rutaFactura,productosDevueltos, productosAgregados, codigoBarrasTicket,fechaCancelacion,
							montoTotal,idFactFormaPago,descripcionFactFormaPago,puedeHacerComplementos,descripcionAlmacen, diasPasadosVentaInicial, montoIva
							order by fechaAlta desc 
						end
					else
						begin
							select	
									contador,					
									idVenta,						
									idCliente,					
									case
										when nombreCliente is null then 'PÚBLICO EN GENERAL' 
										else nombreCliente
									end as nombreCliente,
									cantidad,					
									fechaAlta,					
									idUsuario,					
									nombreUsuario,				
									idProducto,					
									descripcionProducto	,								
									idLineaProducto,				
									descripcionLineaProducto,
									idFactura, 
									idEstatusFactura,
									descripcionEstatusFactura,
									rutaFactura+'/'+'Factura_'+cast(idVenta as varchar)+'.pdf' as rutaFactura,
									precio,costo,case when coalesce(costo,0)=0 then 0 else (coalesce(precio,0)-coalesce(costo,0)) end ganancia,
									year(fechaAlta) as mes, day(fechaAlta) as dia,
									codigoBarras,tipoCliente,descSucursal,productosDevueltos,productosAgregados,cantProductosLiq, codigoBarrasTicket,
									(cantidad*precio) montoTotal,idFactFormaPago,descripcionFactFormaPago,
									puedeHacerComplementos
									,descripcionAlmacen
									,diasPasadosVentaInicial
									,montoIva
							from	#Ventas 
							order by idVenta desc 
						end
				end
				
		end -- reporte de estatus

	end  -- principal


