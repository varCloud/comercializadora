GO
/****** Object:  StoredProcedure [dbo].[SP_REALIZA_VENTA]    Script Date: 12/09/2020 11:39:27 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		guarda la venta en el sistema
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_REALIZA_VENTA]

	@xml					AS XML, 
	@idCliente				int,
	@idFactFormaPago		int,
	@idFactUsoCFDI			int,
	@idVenta				int,
	@idUsuario				int,
	@idEstacion				int,
	@aplicaIVA				int,
	@numClientesAtendidos	int,
	@tipoVenta				int,  -- 1-Normal / 2-Devolucion / 3-Agregar Productos a la venta
	@motivoDevolucion		varchar(255),
	@idPedidoEspecial       int=0

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se registro la venta correctamente.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@tran_name				varchar(32) = 'REALIZA_VTA',
						@tran_count				int = @@trancount,
						@tran_scope				bit = 0,						
						@totalProductos			int = 0,
						@montoTotal				money = 0,
						@descuento				money = 0.0,
						@ini					int = 0, 
						@fin					int = 0,
						@fecha					datetime,
						@cantProductosLiq       int=0


				create table
					#Ventas
						(	
							contador			int identity(1,1),
							idCliente			int,
							cantidad			float,
							fechaAlta			datetime,
							montoTotal			money,
							idUsuario			int,
							idStatusVenta		int,
							idFactFormaPago		int
						)

				create table
					#VentasDetalle
						(
							idVentaDetalle					int,
							idVenta							int,
							idProducto						int,
							cantidad						float,
							contadorProductosPorPrecio		int,
							monto							money,
							cantidadActualInvGeneral		float,
							cantidadAnteriorInvGeneral		float						
						)

				create table 
					#cantidades 
						(  
							id			int identity(1,1),
							cantidad	int
						)

				create table 
					#idProductos 
						(  
							id			int identity(1,1),
							idProducto	int
						)

				create table
					#productosDevueltos
						(
							id					int identity(1,1),
							productosDevueltos	int	
						)

				create table
					#idVentaDetalle
						(
							id					int identity(1,1),
							idVentaDetalle		int	
						)

			end  --declaraciones 

			begin -- principal

				begin -- inicio transaccion
					if @tran_count = 0
						begin tran @tran_name
					else
						save tran @tran_name
					select @tran_scope = 1
				end -- inicio transaccion

				select @idFactFormaPago = coalesce(@idFactFormaPago, '1')
				select @fecha = coalesce(@fecha, dbo.FechaActual())

				insert into #cantidades (cantidad)
				SELECT Ventas.cantidad.value('.','NVARCHAR(200)') AS cantidad FROM @xml.nodes('//cantidad') as Ventas(cantidad) 

				insert into #idProductos (idProducto)
				SELECT Ventas.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @xml.nodes('//idProducto') as Ventas(idProducto)

				insert into #productosDevueltos (productosDevueltos)
				SELECT Ventas.productosDevueltos.value('.','NVARCHAR(200)') AS productosDevueltos FROM @xml.nodes('//productosDevueltos') as Ventas(productosDevueltos)

				insert into #idVentaDetalle (idVentaDetalle)
				SELECT Ventas.idVentaDetalle.value('.','NVARCHAR(200)') AS idVentaDetalle FROM @xml.nodes('//idVentaDetalle') as Ventas(idVentaDetalle)
				
				-- validamos si el cliente tiene descuento por aplicar
				select	@descuento = t.descuento
				from	clientes c
						inner join CatTipoCliente t
							on c.idTipoCliente = t.idTipoCliente
				where	c.idCliente = @idCliente				

				-- universo de venta de productos
				select	p.idProducto, c.cantidad, cast(0 as int) as contador, cast(0 as money) as costo, cast(0 as money) as precioPorProducto,
						coalesce(p.precioIndividual, 0) as precioIndividual, coalesce(p.precioMenudeo, 0) as precioMenudeo, 
						cast(0 as money) as precioRango, @aplicaIVA as aplicaIVA ,  cast(0 as money) as  montoIva, 
						cast(0 as money) as  montoVenta, cast(0 as money) as  precioVenta, cast(0 as money) as porcentajeDescuentoCliente
				into	#vta
				from	#cantidades c
						inner join #idProductos p_
							on c.id = p_.id
						inner join Productos p
							on p.idProducto = p_.idProducto
							

				if ( @tipoVenta = 1 )
				begin  --  1-Normal 

					select @totalProductos = sum(cantidad) from #vta
				
					-- primero precios individual y menudeo
					update	#vta
					set		costo =	case
										when @totalProductos >= 12 then cantidad * precioMenudeo
										else cantidad * precioIndividual
									end,
							precioVenta =	case
												when @totalProductos >= 12 then precioMenudeo
												else precioIndividual
											end

					-- se actualiza el precio de venta y el precio del rango con que se hizo la venta en caso q exista un precio de rango
					update	#vta
					set		#vta.precioVenta = a.precioRango,
							#vta.precioRango = a.precioRango
					from	(
								select	v.idProducto, v.cantidad, ppp.costo as precioRango, ppp.min, ppp.max
								from	#vta v
											inner join ProductosPorPrecio ppp
												on ppp.idProducto = v.idProducto
											inner join (select idProducto,max(max) maxCantRango 
														from ProductosPorPrecio 
														where
														activo = cast(1 as bit)
														group by idProducto) maxRango 
												on ppp.idProducto=maxRango.idProducto
								where	ppp.activo = cast(1 as bit)
									--and	v.cantidad between ppp.min and ppp.max
									and	v.cantidad>=min and v.cantidad<=case when v.cantidad>maxCantRango and max=maxCantRango
									then v.cantidad else max end

							)A
					where	#vta.idProducto = a.idProducto


					-- si hay descuento
					if ( @descuento > 0.0 )
					begin
						update	#vta set #vta.porcentajeDescuentoCliente = @descuento
						update	#vta set #vta.precioVenta = #vta.precioVenta - (#vta.precioVenta * ( @descuento / 100 ))
					end

					-- actualizamos el precio final
					update	#vta 
					set		#vta.costo = precioVenta * cantidad

					if ( @aplicaIVA = 1 )
						begin
							update	#vta
							set		montoIva = costo * 0.16
						end

					update	#vta
					set		montoVenta = costo --+ montoIva

					-- si existe el producto en el inventario general
					if  exists	( 
										select 1 from #vta v left join InventarioGeneral g on v.idProducto=g.idProducto
										where g.idProducto is null
									)
					begin
						select @mensaje = 'El producto no se encuentra en el inventario.'
						raiserror (@mensaje, 11, -1)
					end

					-- si el producto no tiene un precio asignado en tabla de productos 
					if exists ( select 1 from #vta where montoVenta <= 0 )
					begin
						select @mensaje = 'El producto no tiene un precio asignado.'
						raiserror (@mensaje, 11, -1)
					end

					----validamos pedido especial
					if(@idPedidoEspecial>0)
					begin					
					   
					   declare @idEstatusPedidoInterno int					   

					  if not exists(select 1 from PedidosInternos where idPedidoInterno=@idPedidoEspecial and idTipoPedidoInterno=2)
					    raiserror('El número de ticket de pedido especial no existe', 11, -1)

					   select @idEstatusPedidoInterno=idEstatusPedidoInterno from PedidosInternos where idPedidoInterno=@idPedidoEspecial
					  
					  if (@idEstatusPedidoInterno!=4)
					  begin
					     declare @msg varchar(100)
						 select @msg='El pedido especial se encuentra en estatus de ' + coalesce((select descripcion from catestatuspedidointerno where idEstatusPedidoInterno=@idEstatusPedidoInterno),'')
					     raiserror(@msg, 11, -1)
					  end

					  if exists(select 1 from PedidosInternos where idPedidoInterno=@idPedidoEspecial and idAlmacenOrigen<>(select idAlmacen from usuarios where idUsuario=@idUsuario))
					    raiserror('El pedido especial no pertenece a tu almacen', 11, -1)

					end
				
					-- si todo bien
						select @montoTotal = sum(montoVenta+montoIva) from #vta

					-- inserta en tablas fisicas
						insert into Ventas (idCliente,cantidad,fechaAlta,montoTotal,idUsuario,idStatusVenta,idFactFormaPago,idFactUsoCFDI,idEstacion)
						select	@idCliente as idCliente, @totalProductos as cantidad , dbo.FechaActual() as fechaAlta, 
								@montoTotal as montoTotal, @idUsuario as idUsuario, 1 as idStatusVenta,
								@idFactFormaPago as idFactFormaPago, @idFactUsoCFDI as idFactUsoCFDI, @idEstacion as idEstacion 

						select @idVenta = max(idVenta)  from Ventas


					-- se inserta el detalle de los productos que se vendieron
						insert into 
							VentasDetalle 
								(
									idVenta,idProducto,cantidad,contadorProductosPorPrecio,monto,cantidadActualInvGeneral,cantidadAnteriorInvGeneral,
									precioIndividual,precioMenudeo,precioRango,montoIVA,precioVenta
								)
						select	@idVenta as idVenta, v.idProducto, v.cantidad, v.contador, v.montoVenta, i.cantidad - v.cantidad as cantidadActualInvGeneral,
								i.cantidad as cantidadAnteriorInvGeneral, v.precioIndividual, v.precioMenudeo, v.precioRango, v.montoIva, v.precioVenta
						from	#vta v
								inner join InventarioGeneral i
									on v.idProducto = i.idProducto



					-- calculamos las existencias del inventario despues de la venta
					select idProducto, sum(cantidad) as cantidad into #totProductos from #vta group by idProducto

					select	idInventarioDetalle, id.idProducto, id.cantidad, fechaAlta, id.idUbicacion, 
							fechaActualizacion, cast(0 as int) as cantidadDescontada, 
							cast(0 as int) as cantidadFinal
					into	#tempExistencias 
					from	Usuarios u
								inner join Almacenes a
									on a.idAlmacen = u.idAlmacen
								inner join Ubicacion ub
									on ub.idAlmacen = a.idAlmacen
								inner join InventarioDetalle id
									on id.idUbicacion = ub.idUbicacion
								inner join #totProductos p
									on p.idProducto = id.idProducto
					where	u.idUsuario = @idUsuario
						and	id.cantidad > 0

					if not exists ( select 1 from #tempExistencias)
						begin
							select @mensaje = 'No se realizo la venta, no se cuenta con suficientes existencias en el inventario.'
							raiserror (@mensaje, 11, -1)
						end


					-- se calcula de que ubicaciones se van a descontar los productos
					select @ini = min(idProducto), @fin= max(idProducto) from #totProductos

					while ( @ini <= @fin )
						begin
							declare	@cantidadProductos as int = 0
							select	@cantidadProductos = cantidad from #totProductos where idProducto = @ini

							while ( @cantidadProductos > 0 )
								begin
									declare @cantidadDest as int = 0, @idInventarioDetalle as int = 0
									select	@cantidadDest = coalesce(max(cantidad), 0) from #tempExistencias where idProducto = @ini and cantidadDescontada = 0
									select	@idInventarioDetalle = idInventarioDetalle from #tempExistencias where idProducto = @ini and cantidadDescontada = 0 and cantidad = @cantidadDest

									-- si ya no hay ubicaciones con existencias a descontar
									if ( @cantidadDest = 0 )
										begin
											update  #tempExistencias set cantidadDescontada = (select cantidad from #totProductos where idProducto = @ini)
											where	idProducto = @ini
											select @cantidadProductos = 0
										end
									else
										begin
											if ( @cantidadDest >= @cantidadProductos )
												begin 
													update #tempExistencias set cantidadDescontada = @cantidadProductos 
													where idProducto = @ini and idInventarioDetalle = @idInventarioDetalle
													select @cantidadProductos = 0 
												end
											else
												begin
													update	#tempExistencias set cantidadDescontada = @cantidadDest
													where	 idProducto = @ini and idInventarioDetalle = @idInventarioDetalle
													select @cantidadProductos = @cantidadProductos - @cantidadDest						
												end
										end 
								end

							select @ini = min(idProducto) from #totProductos where idProducto > @ini

						end  -- while ( @ini <= @fin )

						update	#tempExistencias
						set		cantidadFinal = cantidad - cantidadDescontada

						-- si el inventario de los productos vendidos queda negativo se paso de productos = rollback
						if  exists	( select 1 from #tempExistencias where cantidadFinal < 0 )
						begin
							select @mensaje = 'No se realizo la venta, no se cuenta con suficientes existencias en el inventario.'
							raiserror (@mensaje, 11, -1)
						end

						--se actualiza inventario_general_log
						INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
						select a.idProducto,sum(a.cantidad),b.cantidad-sum(a.cantidad),dbo.FechaActual(),1 
						from #totProductos a
						join InventarioGeneral b on a.idProducto=b.idProducto
						group by a.idProducto,b.cantidad
					

						-- se actualiza inventario detalle
						update	InventarioDetalle
						set		cantidad = a.cantidadFinal,
								fechaActualizacion = dbo.FechaActual()
						from	(
									select	idInventarioDetalle, idProducto, cantidad, idUbicacion, cantidadDescontada, cantidadFinal
									from	#tempExistencias
								)A
						where	InventarioDetalle.idInventarioDetalle = a.idInventarioDetalle

						-- se actualiza el inventario general
						update	InventarioGeneral
						set		InventarioGeneral.cantidad = InventarioGeneral.cantidad - A.cantidad,
								fechaUltimaActualizacion = dbo.FechaActual()
						from	(
									select idProducto, cantidad from #totProductos
								)A
						where InventarioGeneral.idProducto = A.idProducto


						-- se inserta el InventarioDetalleLog
						insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta)
						select	idUbicacion, idProducto, cantidadDescontada, cantidadFinal, cast(1 as int) as idTipoMovInventario,
								@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idVenta as idVenta
						from	#tempExistencias
						where	cantidadDescontada > 0

						-- si tiene clientes atendidos de ruta
						if ( @numClientesAtendidos > 0)
							begin 
							
								insert into	
									ClientesAtendidosRuta
										(cantidad,idVenta,idUsuario,idEstacion,fechaAlta)
								values
									(@numClientesAtendidos, @idVenta, @idUsuario, @idEstacion, @fecha)

							end

					  select @cantProductosLiq=sum(a.cantidad) from #totProductos a join Productos b on a.idProducto=b.idProducto and b.idLineaProducto in (19,20)
					    
					--pedidos especiales actualizamos el estatus
					if(@idPedidoEspecial>0) 
					begin

						UPDATE  pedidosInternos set idEstatusPedidoInterno=6,idVenta=@idVenta where idPedidoInterno=@idPedidoEspecial				

						INSERT INTO pedidosInternosLog(idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,fechaAlta,idEstatusPedidoInterno)
						select @idPedidoEspecial,idAlmacenOrigen,0 idAlmacenDestino,@idUsuario,dbo.FechaActual(),6
						from pedidosInternos where idPedidoInterno=@idPedidoEspecial
					
					end 

				end --  1-Normal


				if ( @tipoVenta = 2 )
				begin  --  2-Devolucion 

					select @mensaje= 'Se realizaron las devoluciones correctamente.'
					
					-- universo de devoluciones
					select	p.idProducto, c.cantidad, 
							coalesce(dev.productosDevueltos, 0) as productosDevueltos,
							coalesce(vd.idVentaDetalle, 0) as idVentaDetalle , coalesce(idVenta.idVentaDetalle, 0) as idVentaDetalle_temp
					into	#devoluciones
					from	#cantidades c
							inner join #idProductos p_
								on c.id = p_.id
							inner join Productos p
								on p.idProducto = p_.idProducto
							left join #productosDevueltos dev
								on dev.id = c.id
							left join #idVentaDetalle idVenta
								on idVenta.id = c.id	
							left join VentasDetalle vd 
								on vd.idVentaDetalle = idVenta.idVentaDetalle
					
					--select '#devoluciones',(cantidad - productosDevueltos) as diff, * from #devoluciones

					if not exists ( select 1 from Ventas where idVenta = @idVenta and idStatusVenta = 1 )
					begin
						select @mensaje = 'No existe la venta que quiere modificar.'
						raiserror (@mensaje, 11, -1)
					end

					if exists ( select 1 from #devoluciones where (cantidad - productosDevueltos) < 0 )
					begin
						select @mensaje = 'No puede regresar mas productos de los que se vendieron.'
						raiserror (@mensaje, 11, -1)
					end

					if exists ( select 1 from VentasDetalle where idVenta = @idVenta and montoIva > 0 )
					begin
						select @mensaje = 'No puede hacer una devolucion en una venta facturada.'
						raiserror (@mensaje, 11, -1)
					end

					if	(
							(select coalesce(devoluciones,0) from Ventas where idVenta = @idVenta) >=
							(select cantidad from ModificacionesPermitidasVenta where id = 1)
						)
					begin
						select @mensaje = 'No es posible hacer mas devoluciones a esta venta.'
						raiserror (@mensaje, 11, -1)
					end					
					
					-- actualizar productos regresados a VentasDetalle
					update	VentasDetalle
					set		VentasDetalle.idEstatusProductoVenta = 1, --Devuelto,
							VentasDetalle.productosDevueltos = a.productosDevueltos,
							VentasDetalle.cantidad = a.cantidad - a.productosDevueltos,
							VentasDetalle.monto = ( a.cantidad - a.productosDevueltos ) * VentasDetalle.precioVenta
					from	(
								select	idVentaDetalle, idProducto, cantidad, productosDevueltos
								from	#devoluciones 
							)A
					where	VentasDetalle.idVentaDetalle = a.idVentaDetalle
						and	VentasDetalle.idVenta = @idVenta

						
					-- actualizamos tabla de ventas
					update	Ventas
					set		Ventas.cantidad = a.cantidad,
							Ventas.montoTotal = a.montoTotal,
							Ventas.devoluciones = coalesce(Ventas.devoluciones, 0) + 1,
							Ventas.observaciones = @motivoDevolucion
					from	(
								select	sum(cantidad)  as cantidad, sum(monto) as montoTotal, idVenta
								from	VentasDetalle
								where	idVenta = @idVenta
								group by idVenta
							)A
					where	Ventas.idVenta = a.idVenta


					-- actualizamos InventarioGeneralLog
					insert into InventarioGeneralLog (idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
					select	d.idProducto, sum(productosDevueltos) as cantidad, 
							( ig.cantidad + sum(productosDevueltos) ) as cantidadDespuesDeOperacion,
							@fecha as fechaAlta, 15 as idTipoMovInventario
					from	#devoluciones d
								inner join InventarioGeneral ig
									on ig.idProducto = d.idProducto
					group by d.idProducto, ig.cantidad

					-- actualizamos InventarioGeneral
					update	InventarioGeneral
					set		InventarioGeneral.cantidad = InventarioGeneral.cantidad + a.totProductosDevueltos
					from	(
								select	idProducto, sum(productosDevueltos) as totProductosDevueltos
								from	#devoluciones
								group by idProducto
							)A
					where	InventarioGeneral.idProducto = a.idProducto



					-- actualizamos InventarioDetalleLog

					--ubicaciones de donde se saco el producto
					select	idl.idProducto, idl.idUbicacion, idl.cantidad, idl.idTipoMovInventario, u.idAlmacen
					into	#tempUbicacionesDevoluciones
					from	InventarioDetalleLog idl
								inner join Ubicacion u
									on u.idUbicacion = idl.idUbicacion
					where	idl.idVenta = @idVenta
						and	idTipoMovInventario = 1 -- las que se vendieron 
					group by idl.idProducto, idl.idUbicacion, idl.cantidad, idl.idTipoMovInventario, u.idAlmacen

					select	t.* , u.idUbicacion as idUbicacionRegresar
					into	#tempUbicacionesDevoluciones_
					from	#tempUbicacionesDevoluciones t
								left join Ubicacion u
									on t.idAlmacen = u.idAlmacen
					where	u.idPasillo = 0
						and	u.idRaq = 0
						and u.idPiso = 0

					--	verificamos si existe el id sin ubicacion
					if exists	(
									select 1 from #tempUbicacionesDevoluciones_ where idUbicacionRegresar is null
								)
					begin
						insert into Ubicacion (idAlmacen, idPasillo, idRaq, idPiso)
						select idAlmacen, 0,0,0 from #tempUbicacionesDevoluciones_
					end

					-- inserta los registros que se regresaron
					insert	into InventarioDetalleLog (idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idVenta)
					select	temp.idUbicacionRegresar, temp.idProducto, devueltos.productosDevueltos, actuales.cantidad + devueltos.productosDevueltos as cantidadActual,
							15 as idTipoMovInventario, @idUsuario as idUsuario, @fecha as fechaAlta, @idVenta as idVenta
					from	#tempUbicacionesDevoluciones_ temp
								join (
										select	idProducto, sum(productosDevueltos) as productosDevueltos 
										from	VentasDetalle 
										where	idVenta = @idVenta 
											and productosDevueltos > 0
										group by idProducto
								
									 )devueltos on devueltos.idProducto = temp.idProducto
								join InventarioDetalle actuales
									on actuales.idProducto = temp.idProducto and actuales.idUbicacion = temp.idUbicacionRegresar


					-- actualizamos InventarioDetalle
					update	InventarioDetalle 
					set		InventarioDetalle.cantidad = a.cantidad,
							InventarioDetalle.fechaActualizacion  = @fecha
					from	(
								select	temp.idProducto, actuales.cantidad + devueltos.productosDevueltos as cantidad, temp.idUbicacionRegresar
								from	#tempUbicacionesDevoluciones_ temp
											join (
													select	idProducto, sum(productosDevueltos) as productosDevueltos 
													from	VentasDetalle 
													where	idVenta = @idVenta 
														and productosDevueltos > 0
													group by idProducto								
												 )devueltos on devueltos.idProducto = temp.idProducto
											join InventarioDetalle actuales
												on actuales.idProducto = temp.idProducto and actuales.idUbicacion = temp.idUbicacionRegresar
							)A
					where	InventarioDetalle.idUbicacion = a.idUbicacionRegresar
						and	InventarioDetalle.idProducto = a.idProducto

					drop table #devoluciones

				end

				
				if ( @tipoVenta = 3 )
				begin  --  3-Agregar Productos a la venta

					select @mensaje= 'Se agregaron los productos a la venta correctamente.'
					
					 --universo de productos agregados
					select	c.id, c.cantidad, p_.idProducto, idVenta.idVentaDetalle,
							cast(0 as money) as costo, 
							cast(0 as money) as precioVenta,
							p.precioIndividual,
							p.precioMenudeo,
							cast(0 as money) as precioRango,
							cast(0 as money) as montoVenta,
							cast(0 as money) as porcentajeDescuentoCliente							
					into	#productosAgregados
					from	#cantidades c
							inner join #idProductos p_
								on c.id = p_.id
							inner join Productos p
								on p.idProducto = p_.idProducto
							left join #idVentaDetalle idVenta
								on idVenta.id = c.id	
							

					if not exists ( select 1 from Ventas where idVenta = @idVenta and idStatusVenta = 1 )
					begin
						select @mensaje = 'No existe la venta que quiere modificar.'
						raiserror (@mensaje, 11, -1)
					end

					if exists ( select 1 from VentasDetalle where idVenta = @idVenta and montoIva > 0 )
					begin
						select @mensaje = 'No puede agregar productos a una venta facturada.'
						raiserror (@mensaje, 11, -1)
					end

					if	(
							(select coalesce(productosAgregados,0) from Ventas where idVenta = @idVenta) >=
							(select cantidad from ModificacionesPermitidasVenta where id = 2)
						)
					begin
						select @mensaje = 'No es posible agregar mas productos a esta venta.'
						raiserror (@mensaje, 11, -1)
					end					
					
					select @totalProductos = sum(cantidad) from #productosAgregados
				
					-- primero precios individual y menudeo
					update	#productosAgregados
					set		costo =	case
										when @totalProductos >= 12 then cantidad * precioMenudeo
										else cantidad * precioIndividual
									end,
							precioVenta =	case
												when @totalProductos >= 12 then precioMenudeo
												else precioIndividual
											end

					-- se actualiza el precio de venta y el precio del rango con que se hizo la venta en caso q exista un precio de rango
					update	#productosAgregados
					set		#productosAgregados.precioVenta = a.precioRango,
							#productosAgregados.precioRango = a.precioRango
					from	(
								select	v.idProducto, v.cantidad, ppp.costo as precioRango, ppp.min, ppp.max
								from	#productosAgregados v
											inner join ProductosPorPrecio ppp
												on ppp.idProducto = v.idProducto
											inner join (select idProducto,max(max) maxCantRango 
																	from ProductosPorPrecio 
																	where
																	activo = cast(1 as bit)
																	group by idProducto) maxRango 
															on ppp.idProducto=maxRango.idProducto
											where	ppp.activo = cast(1 as bit)
												--and	v.cantidad between ppp.min and ppp.max
												and	v.cantidad>=min and v.cantidad<=case when v.cantidad>maxCantRango and max=maxCantRango
												then v.cantidad else max end
							)A
					where	#productosAgregados.idProducto = a.idProducto


					-- si hay descuento
					if ( @descuento > 0.0 )
					begin
						update	#productosAgregados set #productosAgregados.porcentajeDescuentoCliente = @descuento
						update	#productosAgregados set #productosAgregados.precioVenta = #productosAgregados.precioVenta - (#productosAgregados.precioVenta * ( @descuento / 100 ))
					end

					-- actualizamos el precio final
					update	#productosAgregados 
					set		#productosAgregados.costo = precioVenta * cantidad
										
					update	#productosAgregados
					set		montoVenta = costo 

					-- si existe el producto en el inventario general
					if  exists	( 
									select 1 from #productosAgregados v left join InventarioGeneral g on v.idProducto=g.idProducto
									where g.idProducto is null
								)
					begin
						select @mensaje = 'El producto no se encuentra en el inventario.'
						raiserror (@mensaje, 11, -1)
					end

					-- si el producto no tiene un precio asignado en tabla de productos 
					if exists ( select 1 from #productosAgregados where montoVenta <= 0 )
					begin
						select @mensaje = 'El producto no tiene un precio asignado.'
						raiserror (@mensaje, 11, -1)
					end
				
					-- si todo bien
						select @montoTotal = sum(montoVenta) from #productosAgregados


					-- actualizar productos agregados a VentasDetalle
					insert into 
						VentasDetalle (
										idVenta,idProducto,cantidad,contadorProductosPorPrecio,monto,cantidadActualInvGeneral,
										cantidadAnteriorInvGeneral,precioIndividual,precioMenudeo,precioRango,precioVenta,
										montoIva,idEstatusProductoVenta,productosDevueltos
									  )
					select	@idVenta, pa.idProducto, pa.cantidad, 0, (pa.cantidad * p.precioMenudeo) as monto, (ig.cantidad + pa.cantidad) as cantidadActualInvGeneral,
							ig.cantidad as cantidadAnteriorInvGeneral, p.precioIndividual, p.precioMenudeo, 0 as precioRango,  p.precioMenudeo as precioVenta,
							0 as montoIva, 2 as idEstatusProductoVenta, 0 as productosDevueltos
					from	#productosAgregados pa
								join Productos p
									on p.idProducto = pa.idProducto
								join InventarioGeneral ig
									on ig.idProducto = pa.idProducto
					where	pa.idVentaDetalle = 0


					-- actualizamos tabla de ventas
					update	Ventas
					set		Ventas.cantidad = a.cantidad,
							Ventas.montoTotal = a.montoTotal,
							Ventas.productosAgregados = coalesce(Ventas.productosAgregados, 0) + 1							
					from	(
								select	sum(cantidad)  as cantidad, sum(monto) as montoTotal, idVenta
								from	VentasDetalle
								where	idVenta = @idVenta
								group by idVenta
							)A
					where	Ventas.idVenta = a.idVenta

					
					-- actualizamos InventarioGeneralLog
					insert into InventarioGeneralLog (idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)					
					select	pa.idProducto, pa.cantidad, (ig.cantidad + pa.cantidad) as cantidadDespuesDeOperacion, @fecha, 16 as idTipoMovInventario
					from	#productosAgregados pa
								join InventarioGeneral ig
									on ig.idProducto = pa.idProducto
					where	pa.idVentaDetalle = 0					
					

					-- actualizamos InventarioGeneral
					update	InventarioGeneral
					set		InventarioGeneral.cantidad = InventarioGeneral.cantidad + a.totProductosAgregados
					from	(
								select	idProducto, sum(cantidad) as totProductosAgregados
								from	#productosAgregados
								group by idProducto
							)A
					where	InventarioGeneral.idProducto = a.idProducto

					
					-- calculamos las existencias del inventario despues de la venta
					select	idProducto, sum(cantidad) as cantidad 
					into	#totProductosAgregados
					from	#productosAgregados 
					where	idVentaDetalle = 0 -- que sean nuevos
					group by idProducto

					select	idInventarioDetalle, id.idProducto, id.cantidad, fechaAlta, id.idUbicacion, 
							fechaActualizacion, cast(0 as int) as cantidadDescontada, 
							cast(0 as int) as cantidadFinal
					into	#tempExistenciasAgregadas 
					from	Usuarios u
								inner join Almacenes a
									on a.idAlmacen = u.idAlmacen
								inner join Ubicacion ub
									on ub.idAlmacen = a.idAlmacen
								inner join InventarioDetalle id
									on id.idUbicacion = ub.idUbicacion
								inner join #totProductosAgregados p
									on p.idProducto = id.idProducto
					where	u.idUsuario = @idUsuario
						and	id.cantidad > 0

					if not exists ( select 1 from #tempExistenciasAgregadas)
						begin
							select @mensaje = 'No se pudo agregar productos a la venta, no se cuenta con suficientes existencias en el inventario.'
							raiserror (@mensaje, 11, -1)
						end


					-- se calcula de que ubicaciones se van a descontar los productos
					select @ini = min(idProducto), @fin= max(idProducto) from #totProductosAgregados

					while ( @ini <= @fin )
						begin
							declare	@cantidadProductosDevueltos as int = 0
							select	@cantidadProductosDevueltos = cantidad from #totProductosAgregados where idProducto = @ini

							while ( @cantidadProductosDevueltos > 0 )
								begin
									declare @cantidadDestDev as int = 0, @idInventarioDetalleDev as int = 0
									select	@cantidadDestDev = coalesce(max(cantidad), 0) from #tempExistenciasAgregadas where idProducto = @ini and cantidadDescontada = 0
									select	@idInventarioDetalleDev = idInventarioDetalle from #tempExistenciasAgregadas where idProducto = @ini and cantidadDescontada = 0 and cantidad = @cantidadDestDev

									-- si ya no hay ubicaciones con existencias a descontar
									if ( @cantidadDestDev = 0 )
										begin
											update  #tempExistenciasAgregadas set cantidadDescontada = (select cantidad from #totProductosAgregados where idProducto = @ini)
											where	idProducto = @ini
											select @cantidadProductosDevueltos = 0
										end
									else
										begin
											if ( @cantidadDestDev >= @cantidadProductosDevueltos )
												begin 
													update #tempExistenciasAgregadas set cantidadDescontada = @cantidadProductosDevueltos 
													where idProducto = @ini and idInventarioDetalle = @idInventarioDetalleDev
													select @cantidadProductosDevueltos = 0 
												end
											else
												begin
													update	#tempExistenciasAgregadas set cantidadDescontada = @cantidadDestDev
													where	 idProducto = @ini and idInventarioDetalle = @idInventarioDetalleDev
													select @cantidadProductosDevueltos = @cantidadProductosDevueltos - @cantidadDestDev
												end
										end 
								end

							select @ini = min(idProducto) from #totProductosAgregados where idProducto > @ini

						end  -- while ( @ini <= @fin )

						update	#tempExistenciasAgregadas
						set		cantidadFinal = cantidad - cantidadDescontada


						-- si el inventario de los productos vendidos queda negativo se paso de productos = rollback
						if  exists	( select 1 from #tempExistenciasAgregadas where cantidadFinal < 0 )
						begin
							select @mensaje = 'No se pudieron agregar productos a la venta, no se cuenta con suficientes existencias en el inventario.'
							raiserror (@mensaje, 11, -1)
						end

										
						-- se actualiza inventario detalle
						update	InventarioDetalle
						set		cantidad = a.cantidadFinal,
								fechaActualizacion = dbo.FechaActual()
						from	(
									select	idInventarioDetalle, idProducto, cantidad, idUbicacion, cantidadDescontada, cantidadFinal
									from	#tempExistenciasAgregadas
								)A
						where	InventarioDetalle.idInventarioDetalle = a.idInventarioDetalle

					
						-- se inserta el InventarioDetalleLog
						insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta)
						select	idUbicacion, idProducto, cantidadDescontada, cantidadFinal, cast(1 as int) as idTipoMovInventario,
								@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idVenta as idVenta
						from	#tempExistenciasAgregadas
						where	cantidadDescontada > 0

		                select @cantProductosLiq=sum(a.cantidad) from #totProductosAgregados a join Productos b on a.idProducto=b.idProducto and b.idLineaProducto in (19,20)
					      

					

				end




				begin -- commit de transaccion
					if @tran_count = 0
						begin -- si la transacción se inició dentro de este ámbito
							commit tran @tran_name
							select @tran_scope = 0
						end -- si la transacción se inició dentro de este ámbito
				end -- commit de transaccion
					
				drop table #Ventas
				drop table #VentasDetalle
				drop table #cantidades
				drop table #idProductos
				
			end -- principal

		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message()

			-- revertir transacción si es necesario
			if @tran_scope = 1
				rollback tran @tran_name
					
		end catch

		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje,
					@idVenta as idVenta,
					coalesce(@cantProductosLiq,0) cantProductosLiq

		end -- reporte de estatus

	end  -- principal

GO
/****** Object:  StoredProcedure [dbo].[SP_ELIMINA_VENTA]    Script Date: 22/09/2020 11:49:25 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		elimina la venta seleccionada
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_ELIMINA_VENTA]

	@idVenta		int,
	@idUsuario		int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Venta eliminada correctamente.',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@valido					bit = cast(1 as bit)

			end  --declaraciones 

			begin -- principal
				
				if not exists ( select 1 from Ventas where idVenta = @idVenta )
					begin
						select @mensaje = 'No Existe la venta seleccionada.'
						raiserror (@mensaje, 11, -1)
						select @valido = cast(0 as bit)
					end

				if  exists (select 1 from Ventas where idVenta = @idVenta and idStatusVenta=2)
				begin
						select @mensaje = 'La venta ya se encuentra cancelada.'
						raiserror (@mensaje, 11, -1)
						select @valido = cast(0 as bit)
				end

				-- se actualiza el status en tbl de ventas
				update	Ventas 
				set		idStatusVenta = 2
				where	idVenta = @idVenta

				--insertamos en inventario_general_log
				insert into InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
		        select v.idProducto,v.cantidad,i.cantidad+v.cantidad,dbo.FechaActual(),3
				from VentasDetalle v
				inner join InventarioGeneral i on v.idProducto = i.idProducto
					where	v.idVenta = @idVenta


				-- actualizamos el inventario general
				update	InventarioGeneral
				set		cantidad = InventarioGeneral.cantidad + A.cantidadSumada,
						fechaUltimaActualizacion = dbo.FechaActual()
				from	(
							select	v.idProducto, sum(v.cantidad) as cantidadSumada 
							from	VentasDetalle v
										inner join InventarioGeneral i
											on v.idProducto = i.idProducto
							where	v.idVenta = @idVenta
							group by v.idProducto
						) A
				where	InventarioGeneral.idProducto = A.idProducto


				-- revisamos si existen las ubicaciones de sin acomodar por cada idAlmacen que se afecto en la venta
				if exists	(
								select	idl.idUbicacion, u.idAlmacen, sin_.idAlmacen as idAlmacenSin
								from	InventarioDetalleLog idl
										    join Ubicacion u
												on u.idUbicacion = idl.idUbicacion
											left join Ubicacion sin_
												on	sin_.idAlmacen = u.idAlmacen
												and	sin_.idPasillo = 0
												and sin_.idRaq = 0
												and sin_.idPiso = 0
								where	idVenta = @idVenta
									and	sin_.idAlmacen is null	
							)

				begin
	
					insert into Ubicacion (idAlmacen, idPasillo, idRaq, idPiso)
					select	u.idAlmacen, 0 as idPasillo, 0 as idRaq, 0 as idPiso
					from	InventarioDetalleLog idl
							join Ubicacion u
									on u.idUbicacion = idl.idUbicacion
								left join Ubicacion sin_
									on	sin_.idAlmacen = u.idAlmacen
									and	sin_.idPasillo = 0
									and sin_.idRaq = 0
									and sin_.idPiso = 0
					where	idVenta = @idVenta
						and	sin_.idAlmacen is null	
                   group by u.idAlmacen

					insert into InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
					select	idl.idProducto, 0 as cantidad, dbo.FechaActual() as fechaAlta, sin_.idUbicacion, dbo.FechaActual() as fechaActualizacion 
					from	InventarioDetalleLog idl
								 join Ubicacion u
									on u.idUbicacion = idl.idUbicacion
								 join Ubicacion sin_
									on	sin_.idAlmacen = u.idAlmacen
									and	sin_.idPasillo = 0
									and sin_.idRaq = 0
									and sin_.idPiso = 0
					where	idVenta = @idVenta
					group by idl.idProducto,sin_.idUbicacion					

				end


				-- actualizamos el InventarioDetalle
				update	InventarioDetalle
				set		InventarioDetalle.cantidad = a.cantidad_total
				from	(
				
							select	idl.idProducto, sin_.idUbicacion, id.cantidad + sum(idl.cantidad) as cantidad_total
							from	InventarioDetalleLog idl
									join Ubicacion u
											on u.idUbicacion = idl.idUbicacion
									join Ubicacion sin_
											on	sin_.idAlmacen = u.idAlmacen
											and	sin_.idPasillo = 0
											and sin_.idRaq = 0
											and sin_.idPiso = 0
										inner join InventarioDetalle id
											on	id.idProducto = idl.idProducto
											and	id.idUbicacion = sin_.idUbicacion 
							where	idVenta = @idVenta
							group by idl.idProducto, sin_.idUbicacion,id.cantidad 

						)A
				where	InventarioDetalle.idProducto = a.idProducto
					and	InventarioDetalle.idUbicacion = a.idUbicacion

				
				-- insertamos el InventarioDetalleLog
				insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta)
				select	sin_.idUbicacion, idl.idProducto, sum(idl.cantidad), id.cantidad, 3 as idTipoMovInventario, 
						@idUsuario, dbo.FechaActual() as fechaAlta, @idVenta
						--idl.idVenta, idl.idProducto, sin_.idUbicacion, id.cantidad , idl.cantidad, *
				from	InventarioDetalleLog idl
						join Ubicacion u
								on u.idUbicacion = idl.idUbicacion
						join Ubicacion sin_
								on	sin_.idAlmacen = u.idAlmacen
								and	sin_.idPasillo = 0
								and sin_.idRaq = 0
								and sin_.idPiso = 0
						inner join InventarioDetalle id
								on	id.idProducto = idl.idProducto
								and	id.idUbicacion = sin_.idUbicacion 
				where	idVenta = @idVenta
				group by sin_.idUbicacion, idl.idProducto,id.cantidad



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

			--if ( @idVenta = cast(1 as bit) )
			--	begin
					
			--	end

				
		end -- reporte de estatus

	end  -- principal

GO

/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_VENTAS]    Script Date: 12/09/2020 11:58:22 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

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
	@tipoConsulta			int = null

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
					#Ventas	
						(
							contador					int identity(1,1),
							idVenta						int ,
							idCliente					int,
							nombreCliente				varchar(300),
							cantidad					bigint,
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
							costo						float,
							codigoBarras				varchar(100),
							tipoCliente					varchar(50),
							descSucursal				varchar(100),
							productosDevueltos			int,
							productosAgregados			int,
							cantProductosLiq			int
						)			

			end  --declaraciones 

			begin -- principal 
				
				-- validaciones
					if (	
							(@idProducto is null) and 
							(@descProducto is null) and 
							(@idCliente is null) and 
							(@idUsuario is null) and 
							(@fechaIni is null) and 
							(@fechaFin is null) 
						)
					begin
						select	@mensaje = 'Debe elejir al menos un criterio para la búsqueda de la Venta.',
								@valido = cast(0 as bit)						
						raiserror (@mensaje, 11, -1)
					end

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

						insert into #Ventas (idVenta,idCliente,nombreCliente,cantidad,fechaAlta,idUsuario,nombreUsuario,idProducto,descripcionProducto,idLineaProducto,descripcionLineaProducto,idFactura,idEstatusFactura,descripcionEstatusFactura,fechaTimbrado,rutaFactura,precio,costo,codigoBarras,tipoCliente,descSucursal,productosDevueltos,productosAgregados,cantProductosLiq)
						select	top 50 v.idVenta,v.idCliente, cl.nombres + ' ' + cl.apellidoPaterno + ' ' + cl.apellidoMaterno as nombreCliente
								,vd.cantidad, v.fechaAlta, v.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
								p.idProducto,p.descripcion, lp.idLineaProducto, lp.descripcion, f.idFactura, f.idEstatusFactura, s.descripcion,f.fechaTimbrado,f.pathArchivoFactura,
								vd.precioVenta,[dbo].[obtenerPrecioCompra](vd.idProducto,cast(v.fechaAlta as date)),
								p.codigoBarras,t.descripcion,suc.descripcion, v.devoluciones, v.productosAgregados,
								case when p.idLineaProducto in (19,20) then vd.cantidad else 0 end cantProductosLiq
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
						where	v.idStatusVenta = 1
						order by v.fechaAlta desc
					end
				-- si es por busqueda
				else 
					begin

						insert into #Ventas (idVenta,idCliente,nombreCliente,cantidad,fechaAlta,idUsuario,nombreUsuario,idProducto,descripcionProducto,idLineaProducto,descripcionLineaProducto,idFactura,idEstatusFactura,descripcionEstatusFactura,fechaTimbrado,rutaFactura,precio,costo,codigoBarras,tipoCliente,descSucursal,productosDevueltos,productosAgregados,cantProductosLiq)
						select	 v.idVenta,v.idCliente, cl.nombres + ' ' + cl.apellidoPaterno + ' ' + cl.apellidoMaterno as nombreCliente
								,vd.cantidad, v.fechaAlta, v.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
								p.idProducto,p.descripcion, lp.idLineaProducto, lp.descripcion, f.idFactura, f.idEstatusFactura, s.descripcion,f.fechaTimbrado,f.pathArchivoFactura,
								vd.precioVenta,[dbo].[obtenerPrecioCompra](vd.idProducto,cast(v.fechaAlta as date)),
								p.codigoBarras,t.descripcion,suc.descripcion, v.devoluciones, v.productosAgregados,
								case when p.idLineaProducto in (19,20) then vd.cantidad else 0 end cantProductosLiq
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
							and	v.idStatusVenta = 1
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
							select	ROW_NUMBER() OVER(ORDER BY idVenta DESC) AS contador,
									idVenta,idCliente,
									case
										when nombreCliente is null then 'PÚBLICO EN GENERAL' 
										else nombreCliente
									end as nombreCliente,
									SUM(cantidad) cantidad,fechaAlta,idUsuario,nombreUsuario,idFactura, idEstatusFactura, descripcionEstatusFactura,
									rutaFactura+'/'+'Factura_'+cast(idVenta as varchar)+'.pdf' as rutaFactura,
									productosDevueltos, productosAgregados,sum(cantProductosLiq) cantProductosLiq
							from	#Ventas 
							group by idVenta,idCliente,nombreCliente,fechaAlta,idUsuario,nombreUsuario,idFactura,idEstatusFactura,descripcionEstatusFactura,fechaTimbrado,rutaFactura,productosDevueltos, productosAgregados
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
									codigoBarras,tipoCliente,descSucursal,productosDevueltos,productosAgregados,cantProductosLiq						
							from	#Ventas 
							order by idVenta desc 
						end
				end
				
		end -- reporte de estatus

	end  -- principal


GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_TICKET]    Script Date: 12/09/2020 10:44:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los datos del ticket del idVenta
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_TICKET]
	@idVenta		int,
	@tipoVenta		int  -- 1-Normal / 2-Devolucion / 3-Agregar Productos a la venta / 4- Productos Liquidos

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


			if not exists ( select 1 from Ventas where idVenta = @idVenta and idStatusVenta = 1 )
			begin
				select	@mensaje = 'No existe la venta seleccionada.',
						@status = -1,
						@valido = cast(0 as bit)
			end	

			if ( @tipoVenta = 2 )
			begin
				if not exists ( select 1 from VentasDetalle where idVenta = @idVenta and productosDevueltos > 0	  )
				begin
					select	@mensaje = 'No existen devoluciones para la venta seleccionada.',
							@status = -1,
							@valido = cast(0 as bit)
				end	
			end

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
			if ( @valido = cast(1 as bit) )
				begin
					
					if ( @tipoVenta = 2 )
						begin

							select	ROW_NUMBER() OVER(ORDER BY v.idVenta DESC) AS contador,	
									v.idVenta,d.idVentaDetalle, d.idProducto, d.productosDevueltos as cantidad, 
									(d.precioVenta * d.productosDevueltos) as monto,
									p.descripcion as descProducto, v.idCliente, 
									case
										when c.nombres is null then 'PÚBLICO EN GENERAL' 
										else c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno
									end as nombreCliente,
									u.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
									v.fechaAlta, d.precioVenta, d.idVentaDetalle, d.idEstatusProductoVenta, d.productosDevueltos
							from	Ventas v 
										inner join VentasDetalle d
											on v.idVenta = d.idVenta
										left join Clientes c
											on c.idCliente = v.idCliente
										inner join Usuarios u
											on u.idUsuario = v.idUsuario
										inner join Productos p
											on p.idProducto = d.idProducto
							where	v.idVenta = @idVenta
								and d.productosDevueltos > 0

						end
                    else if ( @tipoVenta = 4 )
						begin
						
							select	ROW_NUMBER() OVER(ORDER BY v.idVenta DESC) AS contador,	
									v.idVenta, d.idProducto, d.cantidad, d.contadorProductosPorPrecio, d.monto as monto, d.montoIVA,
									coalesce (((d.precioIndividual - d.precioVenta) * d.cantidad ) , 0 )as ahorro , 
									p.descripcion as descProducto, v.idCliente, 
									case
										when c.nombres is null then 'PÚBLICO EN GENERAL' 
										else c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno
									end as nombreCliente,
									u.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
									d.cantidadActualInvGeneral, d.cantidadAnteriorInvGeneral, v.fechaAlta, d.precioVenta, d.idVentaDetalle,
									d.idEstatusProductoVenta, d.productosDevueltos
							from	Ventas v 
										inner join VentasDetalle d
											on v.idVenta = d.idVenta
										left join Clientes c
											on c.idCliente = v.idCliente
										inner join Usuarios u
											on u.idUsuario = v.idUsuario
										inner join Productos p
											on p.idProducto = d.idProducto
							where	v.idVenta = @idVenta
								and	d.cantidad > 0 and p.idLineaProducto in (19,20)

						end
					else
						begin
						
							select	ROW_NUMBER() OVER(ORDER BY v.idVenta DESC) AS contador,	
									v.idVenta, d.idProducto, d.cantidad, d.contadorProductosPorPrecio, d.monto as monto, d.montoIVA,
									coalesce (((d.precioIndividual - d.precioVenta) * d.cantidad ) , 0 )as ahorro , 
									p.descripcion as descProducto, v.idCliente, 
									case
										when c.nombres is null then 'PÚBLICO EN GENERAL' 
										else c.nombres + ' ' + c.apellidoPaterno + ' ' + c.apellidoMaterno
									end as nombreCliente,
									u.idUsuario, u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno as nombreUsuario,
									d.cantidadActualInvGeneral, d.cantidadAnteriorInvGeneral, v.fechaAlta, d.precioVenta, d.idVentaDetalle,
									d.idEstatusProductoVenta, d.productosDevueltos
							from	Ventas v 
										inner join VentasDetalle d
											on v.idVenta = d.idVenta
										left join Clientes c
											on c.idCliente = v.idCliente
										inner join Usuarios u
											on u.idUsuario = v.idUsuario
										inner join Productos p
											on p.idProducto = d.idProducto
							where	v.idVenta = @idVenta
								and	d.cantidad > 0

						end

				end
				
		end -- reporte de estatus

	end  -- principal


GO

GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PRODUCTOS]    Script Date: 14/09/2020 11:32:03 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/02/17
Objetivo		Consulta los diferentes clientes del sistema
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_PRODUCTOS]

	@idProducto				int = null,
	@descripcion			varchar(255) = null,
	@idUnidadMedida			int = null,
	@idLineaProducto		int = null,
	@activo					bit = null,
	@articulo				varchar(255) = null,
	@claveProdServ			float = null,
	@fechaIni				datetime = null,
	@fechaFin				datetime = null,
	@idUsuario				int = null,
	@idAlmacen				int = null,
	@idPedidoEspecial		int=null

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
							contador					int identity (1,1),
							idProducto					int,
							descripcion					varchar(100),
							idUnidadMedida				int,
							idLineaProducto				int,
							cantidadUnidadMedida		float,
							codigoBarras				nvarchar(4000),
							fechaAlta					datetime,
							activo						bit,
							articulo					varchar(100),
							idClaveProdServ				float,
							precioIndividual			money,
							precioMenudeo				money,
							DescripcionLinea			varchar(500), 
							DescripcionUnidadMedida		varchar(500), 
							cantidad					int,
							descripcionConExistencias	varchar(500),
							costo						float,
							porcUtilidadIndividual		float,
							porcUtilidadMayoreo			float,
							cantidadRecibida            int
						)
						
			end  --declaraciones 

			begin -- principal

				select @idProducto = coalesce(@idProducto, 0)
				select @idUnidadMedida = coalesce(@idUnidadMedida, 0)
				select @idLineaProducto = coalesce(@idLineaProducto, 0)
				select @activo = cast(1 as bit) --coalesce(@activo, 1)
				select @claveProdServ = coalesce(@claveProdServ, 0)
				select @fechaIni = coalesce(@fechaIni, '19000101')
				select @fechaFin = coalesce(@fechaFin, '19000101')
				select @idUsuario = coalesce(@idUsuario, 0)

				-- universo de productos
				insert into 
					#Productos 
						(
							idProducto,descripcion,idUnidadMedida,idLineaProducto,cantidadUnidadMedida,codigoBarras,
							fechaAlta,activo,articulo,idClaveProdServ,precioIndividual,precioMenudeo,
							DescripcionLinea,DescripcionUnidadMedida,cantidad,descripcionConExistencias,costo,
							porcUtilidadIndividual,porcUtilidadMayoreo,cantidadRecibida
						)
				select	p.idProducto,upper(p.descripcion) as descripcion,p.idUnidadMedida,p.idLineaProducto,cantidadUnidadMedida,codigoBarras,
						p.fechaAlta,p.activo,articulo,claveProdServ,coalesce(precioIndividual, 0) as precioIndividual, coalesce(precioMenudeo, 0) as precioMenudeo,
						l.descripcion as DescripcionLinea, u.descripcion as DescripcionUnidadMedida, coalesce(g.cantidad, 0) as cantidad,
						case
							when g.cantidad is null then upper(p.descripcion) + '  - (S/E)'
							when g.cantidad = 0  then upper(p.descripcion) + '  - (S/E)'
							else upper(p.descripcion) + 
									'  - (E:' + cast(g.cantidad as varchar(500)) + ' / ' +
										 'ME:' + cast(isnull(p.precioIndividual,'') as varchar(500)) + ' / ' +
										 'MA:' + cast(isnull(p.precioMenudeo,'') as varchar(500)) +
										 ')'
						end as descripcionConExistencias,coalesce(p.ultimoCostoCompra,DBO.obtenerPrecioCompra(p.idProducto,GETDATE())) costo,
						porcUtilidadIndividual,porcUtilidadMayoreo,0 cantidadRecibida
				from	Productos p
				inner join LineaProducto l 
							on p.idLineaProducto = l.idLineaProducto
						inner join CatUnidadMedida u
							on p.idUnidadMedida = u.idUnidadMedida
						left join InventarioGeneral g
							on g.idProducto = p.idProducto
				where p.activo = @activo
				order by p.idProducto desc						

				-- select * from #Productos p order by p.idProducto asc						
				-- se actualizan existencias si se consulta con el numero de usuario
				if ( @idUsuario > 0 ) 
					begin
						
						update	#Productos 
						set		cantidad = 0,
								descripcionConExistencias = upper(descripcion) + '  - (S/E)'

						update	#Productos
						set		#Productos.cantidad = a.cantidad,
								#Productos.descripcionConExistencias = a.descripcionConExistencias
						from	(
									select	id.idProducto, 
											coalesce( (sum(id.cantidad)), 0) as cantidad,
											case
												when coalesce( (sum(id.cantidad)), 0) is null then upper(p.descripcion) + '  - (S/E)'
												when coalesce( (sum(id.cantidad)), 0) = 0  then upper(p.descripcion) + '  - (S/E)'
												else upper(p.descripcion) + 
														'  - (E:'  + cast(coalesce( (sum(id.cantidad)), 0) as varchar(500)) + ' / ' +
															 'ME:' + cast(isnull(p.precioIndividual,'') as varchar(500)) + ' / ' +
															 'MA:' + cast(isnull(p.precioMenudeo,'') as varchar(500)) +
															 ')'
											end as descripcionConExistencias
									from	Usuarios u
												inner join Almacenes a
													on a.idAlmacen = u.idAlmacen
												inner join Ubicacion ub
													on ub.idAlmacen = a.idAlmacen
												inner join InventarioDetalle id
													on id.idUbicacion = ub.idUbicacion
												inner join Productos p
													on p.idProducto = id.idProducto
									where	u.idUsuario = @idUsuario
									group by id.idProducto, p.descripcion, p.precioIndividual, p.precioMenudeo
								)A
						where	#Productos.idProducto = a.idProducto

					end

					--si vienes de un pedido especial solo consultamos los productos del pedido especial
					if(coalesce(@idPedidoEspecial,0)>0)
					begin

					   select @idAlmacen=idAlmacen from Usuarios where idUsuario=@idUsuario
					   
					   declare @idEstatusPedidoInterno int					   

					  if not exists(select 1 from PedidosInternos where idPedidoInterno=@idPedidoEspecial and idTipoPedidoInterno=2)
					    raiserror('El número de ticket de pedido especial no existe', 11, -1)

					   select @idEstatusPedidoInterno=idEstatusPedidoInterno from PedidosInternos where idPedidoInterno=@idPedidoEspecial
					  
					  if (@idEstatusPedidoInterno!=4)
					  begin
					     declare @msg varchar(100)
						 select @msg='El pedido especial se encuentra en estatus de ' + coalesce((select descripcion from catestatuspedidointerno where idEstatusPedidoInterno=@idEstatusPedidoInterno),'')
					     raiserror(@msg, 11, -1)
					  end

					  if exists(select 1 from PedidosInternos where idPedidoInterno=@idPedidoEspecial and idAlmacenOrigen<>@idAlmacen)
					    raiserror('El pedido especial no pertenece a tu almacen', 11, -1)

					  
					   UPDATE P SET cantidadRecibida=d.cantidadAceptada 
					   from pedidosInternosDetalle d
					   join #Productos p on d.idProducto=p.idProducto
					   where idPedidoInterno=@idPedidoEspecial and d.cantidadAceptada>0

					   --eliminamos todos los productos que no tengan cantidad aceptada
					   delete #Productos where cantidadRecibida=0

					   if not exists ( select 1 from #Productos )
						begin
							 raiserror('No existe productos para este pedido especial', 11, -1)
						end



					end
					else 
					begin
						if not exists ( select 1 from #Productos )
						begin
							select	@valido = cast(0 as bit),
									@status = -1,
									@mensaje = 'No se encontraron productos con esos términos de búsqueda.'
						end

					end
					
		end -- principal

		end try

		begin catch 
		
			-- captura del error
			select	@status =			-error_state(),
					@error_procedure =	error_procedure(),
					@error_line =		error_line(),
					@mensaje =			error_message(),
					@valido = cast(0 as bit)
					
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
				
					select	* 
					from	#Productos p
					where	p.idProducto =		case
													when @idProducto = 0 then p.idProducto
													else @idProducto
												end

						and p.descripcion like	case
													when @descripcion is null then p.descripcion
													else '%' + @descripcion + '%'
												end

						and p.idUnidadMedida =	case
													when @idUnidadMedida = 0 then p.idUnidadMedida
													else @idUnidadMedida
												end

						and p.idLineaProducto =	case
													when @idLineaProducto = 0 then p.idLineaProducto
													else @idLineaProducto
												end

						and articulo like	case
												when @articulo is null then articulo
												else '%' + @articulo + '%' 
											end

						and cast(p.fechaAlta as date) >=	case
																when @fechaIni = '19000101' then cast(p.fechaAlta as date)
																else cast(@fechaIni as date)
															end

						and cast(p.fechaAlta as date) <=	case
																when @fechaFin = '19000101' then cast(p.fechaAlta as date)
																else cast(@fechaFin as date)
															end

						--and claveProdServ		=	case
						--								when @claveProdServ = 0 then claveProdServ
						--								else @claveProdServ
						--							end

						and p.activo = @activo
						order by p.idProducto
				end
				
		end -- reporte de estatus

	end  -- principal

GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_DETALLE_PEDIDOS_INTERNOS]    Script Date: 23/09/2020 06:30:54 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
Fecha			2020/04/27
Objetivo		Consultar el detalle de pedidos internos
*/

ALTER PROCEDURE [dbo].[SP_CONSULTA_DETALLE_PEDIDOS_INTERNOS]
	@idPedidoInterno int
AS
begin -- procedimiento
		
		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit)
						
			end -- inicio
			
		    
			begin

					SELECT 
					PI.idPedidoInterno,pi.fechaAlta,
					pi.idAlmacenOrigen,ao.Descripcion almacenOrigen,
					pi.idAlmacenDestino,ad.Descripcion almacenDestino,
					pi.idUsuario,coalesce(u.nombre,'') + ' ' + coalesce(u.apellidoPaterno,'') + ' ' + coalesce(u.apellidoMaterno,'') nombreCompleto,
					pi.IdEstatusPedidoInterno idStatus,s.descripcion
					INTO #PEDIDOS_INTERNOS_LOG
					FROM PedidosInternosLog pi					
					join CatEstatusPedidoInterno s on pi.IdEstatusPedidoInterno=s.IdEstatusPedidoInterno
					join Almacenes ao on pi.idAlmacenOrigen=ao.idAlmacen
					left join Almacenes ad on pi.idAlmacenDestino=ad.idAlmacen
					join Usuarios u on pi.idUsuario=u.idUsuario					
					where idPedidoInterno=@idPedidoInterno
					

				if not exists (select 1 from #PEDIDOS_INTERNOS_LOG)
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@error_message = 'No se encontraron coincidencias.'
				end
				

			end
		   

		end try -- try principal
		
		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@error_message = error_message(),
					@error_severity =
						case error_severity()
							when 11 then 'Error en validación'
							when 12 then 'Error en consulta'
							when 13 then 'Error en actualización'
							else 'Error general'
						end
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_severity error_severity,
					@error_message mensaje

				select 
			    idPedidoInterno,fechaAlta,
			    idAlmacenOrigen,idAlmacenOrigen idAlmacen,almacenOrigen Descripcion,
				idAlmacenDestino,idAlmacenOrigen idAlmacen,almacenDestino Descripcion,
				idUsuario,nombreCompleto,
				idStatus,descripcion,
				0 idProducto--,producto descripcion,cantidad
				from #PEDIDOS_INTERNOS_LOG 
			
					
		end -- reporte de estatus
		
	end -- procedimiento
	

GO
/****** Object:  StoredProcedure [dbo].[SP_APP_OBTENER_PEDIDOS_INTERNOS]    Script Date: 07/09/2020 10:54:43 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--drop procedure [SP_APP_OBTENER_PEDIDOS_INTERNOS]

ALTER PROCEDURE [dbo].[SP_APP_OBTENER_PEDIDOS_INTERNOS]
@idEstatusPedido int = null,
@idAlmacenOrigen int = null,
@idAlmacenDestino int = null,
@fechaInicio datetime = null,
@fechaFin datetime = null,
@idPedidoInterno int =  null
/*
1 Pedido Realizado
2 Pedido Enviado ó Atendido
3 Pedido Rechazado
4 Pedido Finalizado
*/
,@idTipoPedidoInterno  int  = 1
AS
BEGIN
		--select * from [dbo].[CatEstatusPedidoInterno]

		select 200 Estatus , 'ok' Mensaje

		SELECT 
		P.IdEstatusPedidoInterno,
		EP.descripcion descripcionEstatus,
		P.idPedidoInterno,
		PD.idProducto , PD.cantidad,Prod.descripcion, 
		P.idAlmacenOrigen, A.Descripcion,
		P.idAlmacenDestino,AB.Descripcion
	
		--* 
		FROM PedidosInternos   P join  PedidosInternosDetalle PD
		on P.idPedidoInterno = PD.idPedidoInterno join Productos Prod
		on PD.idProducto = Prod.idProducto JOIN CatEstatusPedidoInterno EP
		ON EP.IdEstatusPedidoInterno =P.IdEstatusPedidoInterno join Almacenes A
		on P.idAlmacenOrigen = A.idAlmacen join Almacenes AB
		on P.idAlmacenDestino = AB.idAlmacen

		where
		P.IdEstatusPedidoInterno = coalesce(@idEstatusPedido, P.IdEstatusPedidoInterno)
		and P.idAlmacenDestino = coalesce(@idAlmacenDestino , P.idAlmacenDestino)
		and P.idAlmacenOrigen = coalesce(@idAlmacenOrigen , P.idAlmacenOrigen)
		and cast(P.fechaAlta as date) >= coalesce(cast(@fechaInicio as date),cast(P.fechaAlta as date))
		and cast(P.fechaAlta as date) <= coalesce(cast(@fechaFin as date),cast(P.fechaAlta as date))
		and P.idPedidoInterno = coalesce (@idPedidoInterno , P.idPedidoInterno )
		and P.idTipoPedidoInterno =  coalesce (@idTipoPedidoInterno , P.idTipoPedidoInterno)
		order by P.fechaAlta asc
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_PEDIDOS_INTERNOS]    Script Date: 14/09/2020 11:52:06 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
Fecha			2020/04/27
Objetivo		Consultar los pedidos internos
*/

ALTER procedure [dbo].[SP_CONSULTA_PEDIDOS_INTERNOS]

@IdEstatusPedidoInterno int=NULL,
@idAlmancenOrigen int=NULL,
@idAlmacenDestino int=NULL,
@idUsuario int=NULL,
@idProducto int=NULL,
@fechaIni date=NULL,
@fechaFin date=NULL,
@idPedidoInterno int=NULL,
@idTipoPedidoInterno int = null



AS BEGIN

		begin try -- try principal
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit),
						@top bigint=0x7fffffffffffffff--valor màximo
						
			end -- inicio
			
		    
			begin
				
				--select @idTipoPedidoInterno = coalesce(@idTipoPedidoInterno, 1)
				--print @idTipoPedidoInterno

				if(	@IdEstatusPedidoInterno is null and
					@idAlmancenOrigen is null and
					@idAlmancenOrigen is null and
					@idUsuario is null and
					@idProducto is null and
					@fechaIni is null and
					@fechaFin is null and
					@idPedidoInterno is null and
					@idTipoPedidoInterno is null 
					)
					
					begin
						select @top=50
					end


					SELECT top (@top) 
					PI.idPedidoInterno,pi.fechaAlta,
					pi.idAlmacenOrigen,ao.Descripcion almacenOrigen,
					pi.idAlmacenDestino,ad.Descripcion almacenDestino,
					pi.idUsuario,coalesce(u.nombre,'') + ' ' + coalesce(u.apellidoPaterno,'') + ' ' + coalesce(u.apellidoMaterno,'') nombreCompleto,
					pi.IdEstatusPedidoInterno idStatus,s.descripcion,
					pid.idProducto,p.descripcion producto,pid.cantidad
					INTO #PEDIDOS_INTERNOS
					FROM PedidosInternos pi
					join PedidosInternosDetalle pid on pi.idPedidoInterno=pid.idPedidoInterno
					join CatEstatusPedidoInterno s on pi.IdEstatusPedidoInterno=s.IdEstatusPedidoInterno
					join Almacenes ao on pi.idAlmacenOrigen=ao.idAlmacen
					join Almacenes ad on pi.idAlmacenDestino=ad.idAlmacen
					join Usuarios u on pi.idUsuario=u.idUsuario
					join Productos p on pid.idProducto=p.idProducto
					where
					pi.idPedidoInterno=coalesce(@idPedidoInterno,pi.idPedidoInterno) and
					pi.IdEstatusPedidoInterno=coalesce(@IdEstatusPedidoInterno,pi.idestatuspedidointerno)
					and pi.idAlmacenOrigen=coalesce(@idAlmancenOrigen,pi.idAlmacenOrigen)
					and pi.idAlmacenDestino=coalesce(@idAlmacenDestino,pi.idAlmacenDestino)
					and pi.idUsuario=coalesce(@idUsuario,pi.idUsuario)
					and pid.idProducto=coalesce(@idProducto,pid.idProducto)
					and cast(pi.fechaAlta as date) >=coalesce(cast(@fechaIni as date),cast(pi.fechaAlta as date))
					and cast(pi.fechaAlta as date) <=coalesce(cast(@fechaFin as date),cast(pi.fechaAlta as date))
					and idTipoPedidoInterno = coalesce(@idTipoPedidoInterno, 1)
					order by fechaAlta desc

				if not exists (select 1 from #PEDIDOS_INTERNOS)
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@error_message = 'No se encontraron pedidos internos con esos términos de búsqueda.'
				end
				

			end
		   

		end try -- try principal
		
		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@error_message = error_message(),
					@error_severity =
						case error_severity()
							when 11 then 'Error en validación'
							when 12 then 'Error en consulta'
							when 13 then 'Error en actualización'
							else 'Error general'
						end
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_severity error_severity,
					@error_message mensaje

				select 
			    idPedidoInterno,fechaAlta,
			    idAlmacenOrigen,idAlmacenOrigen idAlmacen,almacenOrigen Descripcion,
				idAlmacenDestino,idAlmacenOrigen idAlmacen,almacenDestino Descripcion,
				idUsuario,nombreCompleto,
				idStatus,descripcion,
				idProducto,producto descripcion,cantidad
				from #PEDIDOS_INTERNOS 
				
			
					
		end -- reporte de estatus
		

END

GO
/****** Object:  StoredProcedure [dbo].[SP_APP_RECHAZA_PEDIDO_INTERNO]    Script Date: 22/09/2020 09:09:03 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--drop procedure SP_APP_ACEPTA_PEDIDO_INTERNO
ALTER PROCEDURE [dbo].[SP_APP_RECHAZA_PEDIDO_INTERNO]
@idPedidoInterno int ,
@idUsuario int,
@idAlmacenOrigen int,-- el almacen del usuario que esta logueado en la hand held
@idAlmacenDestino int,
@observacion varchar(max) = null
AS
BEGIN
	BEGIN TRY
	        
			declare @idEstatusPedidoInterno int=5 --pedido rechazado
		
	
			if not exists (select 1 from PedidosInternos where 
			(idAlmacenDestino = @idAlmacenDestino or idAlmacenDestino =  @idAlmacenOrigen) and
			(idAlmacenOrigen = @idAlmacenOrigen  or idAlmacenOrigen = @idAlmacenDestino ) and
			idPedidoInterno = @idPedidoInterno)
			begin
				select -1 Estatus , 'El pedido no corresponde al almacen solicitado' mensaje
				return
			end
			declare
			@idEstatusPedidoActual int = 0
			select  @idEstatusPedidoActual = IdEstatusPedidoInterno from PedidosInternos where @idPedidoInterno =idPedidoInterno

			if (@idEstatusPedidoActual not in (2))
			begin
				select -1 Estatus , 'No se puede rechazar el pedido sin ser atendido' mensaje
				return
			end

			if exists (select  1  from PedidosInternos where IdEstatusPedidoInterno = @idEstatusPedidoInterno and @idPedidoInterno =idPedidoInterno)
			begin
				select -1 Estatus , 'El estatus del pedido actual es el mismo por favor verifica el estatus' mensaje
				return
			end
			

			if EXISTS (select  1  from PedidosInternosLog where IdEstatusPedidoInterno  = @idEstatusPedidoInterno  and  idPedidoInterno =  @idPedidoInterno )

			BEGIN
				select -1 Estatus , 'No es posible rechazar el pedido, este pedido ya fue rechazado' mensaje
				return
			END
			
			BEGIN TRAN 
				--OBTENEMOS LA FECHA MAS QUE NADA LA HORA ACTUAL DE NUESTRA ZONA HORARIA

				declare	@fecha  datetime,@cantidadRechazada int
				select @fecha  = [dbo].[FechaActual]()

				--OBTENEMOS LA CANTIDAD POR LA CUAL SE VA A RECHAZAR
				select @cantidadRechazada=cantidadAtendida from PedidosInternosDetalle where idPedidoInterno=@idPedidoInterno
				

				--INSERTAMOS LA ACTUALIZACION EN LA TABLA DE MOVIMIENTOS DE MERCANCIA
				INSERT INTO  MovimientosDeMercancia 
					(
					 idAlmacenOrigen
					,idAlmacenDestino
					,idProducto
					,cantidad
					,idPedidoInterno
					,idUsuario
					,fechaAlta
					,idEstatusPedidoInterno
					,observaciones
					,cantidadAtendida
					)
					SELECT 
					@idAlmacenOrigen,
					@idAlmacenDestino,
					PD.idProducto,
					PD.cantidad,
					@idPedidoInterno,
					@idUsuario,
					@fecha,
					@idEstatusPedidoInterno,
					coalesce(@observacion,''),
					@cantidadRechazada
					FROM PedidosInternos P join PedidosInternosDetalle PD
					on  P.idPedidoInterno = PD.idPedidoInterno WHERE P.idPedidoInterno = @idPedidoInterno
			
				--insertamos en el log los cambios del pedido interno
				insert into PedidosInternosLog
				(
					idPedidoInterno
					,idAlmacenOrigen
					,idAlmacenDestino
					,idUsuario
					,IdEstatusPedidoInterno
					,fechaAlta
				)select 
					  @idPedidoInterno
					 ,@idAlmacenOrigen
					 ,@idAlmacenDestino
					 ,@idUsuario
					 ,@idEstatusPedidoInterno
					 ,@fecha
				--from PedidosInternos
				--where idPedidoInterno = @idPedidoInterno

				--INSERTAMOS LA ACUTALIACION DEL ESTATUS
				UPDATE PedidosInternos 
				SET IdEstatusPedidoInterno = @idEstatusPedidoInterno,
				observacion = case  when @observacion is null or  @observacion = '' then observacion
								    when @observacion is NOT null AND  @observacion  != '' then @observacion
							   end
				WHERE idPedidoInterno =@idPedidoInterno

				--INSERTAMOS LA ACTuALIAzaCION DE LA CANTIDAD QUE SE ATENDIO YA QUE PUEDE SER QUE NO SE ENVIE LA QUE SE SOLICITO
				update PedidosInternosDetalle set cantidadRechazada = @cantidadRechazada
				where idPedidoInterno =@idPedidoInterno
		
				declare 
				@_IdAlmacenDestino int,
				@idProducto int ,
				@cantidadPedidoInterno int ,
				@cantidadActualInventario  int,
				@idTipoMovInventario int = 10, -- Carga de mercancia por pedido interno rechazado
				@cantidadDespuesDeOperacion int = 0,
				@idUbicacion int

				--INCREMENTAMOS LA MERCANCIA EN EL INVENTARIO DEL ALAMACEN
					
				select  @idProducto = idProducto ,  @cantidadPedidoInterno= isnull(cantidad,0)  
				from PedidosInternosDetalle where idPedidoInterno =@idPedidoInterno

					--SI LA CANTIDAD QUE ATENDIMOS ES DIFERENTE QUE LA QUE SE PIDIO SE SETEA A CANTIDAD YA QUE CON ESA VARIABLE
					--SE HACEN LAS OPERACIONES PARA INVENTARIO DETALLE Y DETALLE LOG,
					--SI ES IGUAL  PUES NO AFECTA CON CUAL VARIABLE REALIZAMOS EL CALCULO
					if (@cantidadRechazada != isnull(@cantidadPedidoInterno,0))
						SET @cantidadPedidoInterno = @cantidadRechazada

					select @idUbicacion= idUbicacion FROM Ubicacion WHERE idAlmacen = @idAlmacenDestino and idPasillo=0 and idRaq=0 and idPiso=0

					--VALIDAMOS QUE EXISTA LA UBICACION DE SIN ACOMODAR Y EN CASO DE QUE NO EXISTE LA INSERTAMOS
					IF (coalesce(@idUbicacion,0)=0)
					BEGIN
						 INSERT INTO Ubicacion(idAlmacen,idPasillo,idRaq,idPiso)
						 select @idAlmacenDestino,0,0,0

						 select @idUbicacion=max(idUbicacion) from Ubicacion where idAlmacen=@idAlmacenDestino				

					END					

					SELECT @cantidadActualInventario = cantidad  FROM InventarioDetalle 
					WHERE idUbicacion = @idUbicacion and idProducto = @idProducto
					
					-- VALIDAMOS QUE EL RESULTADO QUE SE OBTIENE NO SEA NULL 
					SET @cantidadActualInventario = isnull(@cantidadActualInventario, 0)					
	

					SET @cantidadDespuesDeOperacion =  @cantidadActualInventario+@cantidadPedidoInterno
					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE LOG
					INSERT INTO InventarioDetalleLog (  idUbicacion,
														idProducto,
														cantidad,
														cantidadActual,
														idTipoMovInventario,
														idUsuario,
														fechaAlta,
														idPedidoInterno
														)
					VALUES ( @idUbicacion,@idProducto,@cantidadPedidoInterno ,@cantidadDespuesDeOperacion,@idTipoMovInventario,@idUsuario,dbo.FechaActual(),@idPedidoInterno)

					--VALIDACION SI NO EXISTE INVENTARIO DETALLE CON ESE UBICACION LA INSERTAMOS
					IF NOT EXISTS(select 1 from InventarioDetalle where idUbicacion=@idUbicacion and idProducto=@idProducto)
					--INSERTAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					INSERT INTO  InventarioDetalle(idProducto,cantidad,fechaAlta,idUbicacion)
					SELECT @idProducto, @cantidadDespuesDeOperacion,@fecha,@idUbicacion
					ELSE
					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					update InventarioDetalle set cantidad = @cantidadDespuesDeOperacion,fechaActualizacion=@fecha
					where idUbicacion = @idUbicacion and idProducto = @idProducto				

			COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN 
		SELECT -1 Estatus ,ERROR_MESSAGE() Mensaje, ERROR_LINE() LineaError 
	END CATCH
		
	     select 200 Estatus , 'Registro actualizado exitosamente' Mensaje
END

GO
/****** Object:  StoredProcedure [dbo].[SP_APP_OBTENER_PEDIDOS_INTERNOS_X_USUARIO]    Script Date: 07/09/2020 11:01:24 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--drop procedure [SP_APP_OBTENER_PEDIDOS_INTERNOS]

ALTER PROCEDURE [dbo].[SP_APP_OBTENER_PEDIDOS_INTERNOS_X_USUARIO]
@idUsuario int = null,
@idEstatusPedido int = null,
@idAlmacenDestino int = null,
@fechaInicio datetime = null,
@fechaFin datetime = null,
@idPedidoInterno int =  null 

/*
1 Pedido Realizado
2 Pedido Enviado ó Atendido
3 Pedido Rechazado
4 Pedido Finalizado
*/

,@idTipoPedidoInterno  int  = 1
/*
1	Pedido Interno
2	Pedido Especial
*/
AS
BEGIN
		--select * from [dbo].[CatEstatusPedidoInterno]

		select 200 Estatus , 'ok' Mensaje

		SELECT 
		P.idPedidoInterno,
		P.IdEstatusPedidoInterno,
		EP.descripcion descripcionEstatus,
		Pd.cantidadAtendida,
		P.fechaAlta,
		isnull(p.observacion,'') observacion,
		isnull(M4.observaciones,'') observacionRechazaSolicita,
		isnull(MM.observaciones,'') observacionAtendio,
		isnull(M2.observaciones,'') observacionRechazaAtendio,
		isnull(M3.observaciones,'') observacionFinalizado,
		isnull(U.nombre,' ')+' '+isnull(U.apellidoPaterno,'')+' '+isnull(u.apellidoMaterno,'') usuarioAtendio,
		isnull(UU.nombre,' ')+' '+isnull(UU.apellidoPaterno,'')+' '+isnull(UU.apellidoMaterno,'') usuarioSolicito,
		isnull(URechazado.nombre,' ')+' '+isnull(URechazado.apellidoPaterno,'')+' '+isnull(URechazado.apellidoMaterno,'') usuarioRechaza,
		isnull(UAutoriza.nombre,' ')+' '+isnull(UAutoriza.apellidoPaterno,'')+' '+isnull(UAutoriza.apellidoMaterno,'') usuarioAutoriza,
	    MM.fechaAlta as fechaAtendido,
		M2.fechaAlta as fechaRechazado,
		M3.fechaAlta as fechaAutoriza,
		M4.fechaAlta as fechaRechazaSolicita,
		isnull(PD.cantidadAtendida, 0) cantidadAtendida,
		PD.idProducto , PD.cantidad,Prod.descripcion, 
		P.idAlmacenOrigen,A.idAlmacen, A.Descripcion,
		P.idAlmacenDestino,AB.idAlmacen,AB.Descripcion
		--* 
		FROM PedidosInternos   P join  PedidosInternosDetalle PD
		on P.idPedidoInterno = PD.idPedidoInterno join Productos Prod
		on PD.idProducto = Prod.idProducto JOIN CatEstatusPedidoInterno EP
		ON EP.IdEstatusPedidoInterno =P.IdEstatusPedidoInterno join Almacenes A
		on P.idAlmacenOrigen = A.idAlmacen join Almacenes AB
		on P.idAlmacenDestino = AB.idAlmacen LEFT JOIN [dbo].[MovimientosDeMercancia] MM
		on MM.idPedidoInterno = P.idPedidoInterno and  MM.idEstatusPedidoInterno =2 LEFT JOIN Usuarios U
		on U.idUsuario = MM.idUsuario LEFT JOIN  Usuarios UU 
		on UU.idUsuario = P.idUsuario  LEFT JOIN [dbo].[MovimientosDeMercancia] M2
		on M2.idPedidoInterno = P.idPedidoInterno and  M2.idEstatusPedidoInterno =3 LEFT JOIN Usuarios URechazado on URechazado.idUsuario = M2.idUsuario
		  LEFT JOIN [dbo].[MovimientosDeMercancia] M3
		on M3.idPedidoInterno = P.idPedidoInterno and  M3.idEstatusPedidoInterno =4 LEFT JOIN Usuarios UAutoriza on UAutoriza.idUsuario = M3.idUsuario
		LEFT JOIN [dbo].[MovimientosDeMercancia] M4 on M4.idPedidoInterno = P.idPedidoInterno and  M4.idEstatusPedidoInterno =5 
		LEFT JOIN Usuarios URechazaSoclicita on URechazaSoclicita.idUsuario = M4.idUsuario

		where
		P.IdEstatusPedidoInterno = coalesce(@idEstatusPedido, P.IdEstatusPedidoInterno)
		and P.idUsuario = COALESCE(@idUsuario, P.idUsuario)
		and P.idAlmacenDestino = coalesce(@idAlmacenDestino , P.idAlmacenDestino)
		and cast(P.fechaAlta as date) >= coalesce(cast(@fechaInicio as date),cast(P.fechaAlta as date))
		and cast(P.fechaAlta as date) <= coalesce(cast(@fechaFin as date),cast(P.fechaAlta as date))
		and P.idPedidoInterno = coalesce (@idPedidoInterno , P.idPedidoInterno )
		and P.idTipoPedidoInterno = coalesce (@idTipoPedidoInterno , P.idTipoPedidoInterno)
		order by P.fechaAlta asc
END

GO
/****** Object:  StoredProcedure [dbo].[SP_APP_OBTENER_PEDIDOS_INTERNOS_X_ALMACEN]    Script Date: 07/09/2020 10:59:41 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--drop procedure [SP_APP_OBTENER_PEDIDOS_INTERNOS]

ALTER PROCEDURE [dbo].[SP_APP_OBTENER_PEDIDOS_INTERNOS_X_ALMACEN]
@idAlmacenDestino int,
@idEstatusPedido int = null,
@fechaInicio datetime = null,
@fechaFin datetime = null,
@idPedidoInterno int =  null 

/*
1 Pedido Realizado
2 Pedido Enviado ó Atendido
3 Pedido Rechazado
4 Pedido Finalizado
*/
,@idTipoPedidoInterno  int  = 1
AS
BEGIN
		--select * from [dbo].[CatEstatusPedidoInterno]

		select 200 Estatus , 'ok' Mensaje

		SELECT 
		P.IdEstatusPedidoInterno,
		EP.descripcion descripcionEstatus,
		P.idPedidoInterno,
		P.fechaAlta,
		isnull(p.observacion,'') observacion,
		isnull(M4.observaciones,'') observacionRechazaSolicita,
		isnull(MM.observaciones,'') observacionAtendio,
		isnull(M2.observaciones,'') observacionRechazaAtendio,
		isnull(M3.observaciones,'') observacionFinalizado,
		isnull(U.nombre,' ')+' '+isnull(U.apellidoPaterno,'')+' '+isnull(u.apellidoMaterno,'') usuarioSolicito,
		isnull(UU.nombre,' ')+' '+isnull(UU.apellidoPaterno,'')+' '+isnull(UU.apellidoMaterno,'') usuarioAtendio,
		isnull(URechazado.nombre,' ')+' '+isnull(URechazado.apellidoPaterno,'')+' '+isnull(URechazado.apellidoMaterno,'') usuarioRechaza,
		isnull(UAutoriza.nombre,' ')+' '+isnull(UAutoriza.apellidoPaterno,'')+' '+isnull(UAutoriza.apellidoMaterno,'') usuarioAutoriza,
		isnull(URechazaSoclicita.nombre,' ')+' '+isnull(URechazaSoclicita.apellidoPaterno,'')+' '+isnull(URechazaSoclicita.apellidoMaterno,'') usuarioRechazaSoclicita,

		MM.fechaAlta as fechaAtendido,
		M2.fechaAlta as fechaRechazado,
		M3.fechaAlta as fechaAutoriza,
		M4.fechaAlta as fechaRechazaSolicita,
		isnull(PD.cantidadAtendida, 0) cantidadAtendida,
		PD.idProducto , PD.cantidad,Prod.descripcion, 
		P.idAlmacenOrigen,A.idAlmacen, A.Descripcion,
		P.idAlmacenDestino,AB.idAlmacen,AB.Descripcion
		--* 
		FROM PedidosInternos   P join  PedidosInternosDetalle PD
		on P.idPedidoInterno = PD.idPedidoInterno join Productos Prod
		on PD.idProducto = Prod.idProducto JOIN CatEstatusPedidoInterno EP
		ON EP.IdEstatusPedidoInterno =P.IdEstatusPedidoInterno join Almacenes A
		on P.idAlmacenOrigen = A.idAlmacen join Almacenes AB
		on P.idAlmacenDestino = AB.idAlmacen  LEFT JOIN Usuarios U 
		on U.idUsuario = P.idUsuario LEFT JOIN [dbo].[MovimientosDeMercancia] MM
		on MM.idPedidoInterno = P.idPedidoInterno and  MM.idEstatusPedidoInterno =2 LEFT JOIN Usuarios UU 
		on UU.idUsuario = MM.idUsuario  LEFT JOIN [dbo].[MovimientosDeMercancia] M2
		on M2.idPedidoInterno = P.idPedidoInterno and  M2.idEstatusPedidoInterno =3 LEFT JOIN Usuarios URechazado 
		on URechazado.idUsuario = M2.idUsuario  LEFT JOIN [dbo].[MovimientosDeMercancia] M3
		on M3.idPedidoInterno = P.idPedidoInterno and  M3.idEstatusPedidoInterno =4 LEFT JOIN Usuarios UAutoriza 
		on UAutoriza.idUsuario = M3.idUsuario LEFT JOIN [dbo].[MovimientosDeMercancia] M4
		on M4.idPedidoInterno = P.idPedidoInterno and  M4.idEstatusPedidoInterno =5 LEFT JOIN Usuarios URechazaSoclicita 
		on URechazaSoclicita.idUsuario = M4.idUsuario
		
		where
		P.IdEstatusPedidoInterno = coalesce(@idEstatusPedido, P.IdEstatusPedidoInterno)
		and P.idAlmacenDestino = coalesce(@idAlmacenDestino , P.idAlmacenDestino)
		and cast(P.fechaAlta as date) >= coalesce(cast(@fechaInicio as date),cast(P.fechaAlta as date))
		and cast(P.fechaAlta as date) <= coalesce(cast(@fechaFin as date),cast(P.fechaAlta as date))
		and P.idPedidoInterno = coalesce (@idPedidoInterno , P.idPedidoInterno )
		and P.idTipoPedidoInterno = coalesce (@idTipoPedidoInterno , P.idTipoPedidoInterno)
		order by P.fechaAlta asc
END

GO
/****** Object:  StoredProcedure [dbo].[SP_APP_GENERAR_PEDIDO_INTERNO]    Script Date: 22/09/2020 08:28:05 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_APP_GENERAR_PEDIDO_INTERNO]
@idProducto int,
@idUsuario int,
@idAlamacenOrigen int,-- el almacen del usuario que esta logueado en la hand held
@idAlamacenDestino int,
@cantidad int
AS
BEGIN
	
	DECLARE
		@idPedidoInterno int = 0;
	begin try
	--VALIDACIONES
			 
			
			begin tran
				--INSERTAMOS EN LA TABLA DE PEDIDOS INTERNOS
				INSERT INTO [PedidosInternos] 
				(
					idAlmacenOrigen,
					idAlmacenDestino,
					idUsuario,
					IdEstatusPedidoInterno,
					fechaAlta,
					idTipoPedidoInterno
				)
				VALUES
				(
					@idAlamacenOrigen,
					@idAlamacenDestino,
					@idUsuario,
					1, /*1	Pedido Realizado 2	Pedido Enviado ó Atendido 3	Pedido Rechazado 4	Pedido Finalizado */
					dbo.FechaActual(),
					1
				)
				--OBTENEMOS EL ID DE PEDIDO INTERNO GENERADO
				select @idPedidoInterno = max(idPedidoInterno) from [PedidosInternos] where idUsuario = @idUsuario
			

				--insertamos en el log
				INSERT INTO PedidosInternosLog
				(
					 idPedidoInterno
					,idAlmacenOrigen
					,idAlmacenDestino
					,idUsuario
					,IdEstatusPedidoInterno
					,fechaAlta
				)select 
					 @idPedidoInterno
					,@idAlamacenOrigen
					,@idAlamacenDestino
					,@idUsuario
					,1
					,dbo.FechaActual()
				
				


				--INSERTAMOS EL DETALLE DEL PEDIDO INTERNO GENERADO
				INSERT INTO [PedidosInternosDetalle]
				(
				idPedidoInterno,
				idProducto,
				cantidad,
				fechaAlta
				)
				VALUES
				(
				@idPedidoInterno,
				@idProducto,
				@cantidad,
				dbo.FechaActual()
				)

				--INSERTAMOS EL MOVIMIENTO DE LA MERCANCIA 
				INSERT INTO [MovimientosDeMercancia]
				(
					idAlmacenOrigen
					,idAlmacenDestino
					,idProducto
					,cantidad
					,idPedidoInterno
					,idUsuario
					,fechaAlta
					,idEstatusPedidoInterno
					,cantidadAtendida
				)
				VALUES
				(
				@idAlamacenOrigen,
				@idAlamacenDestino,
				@idProducto,
				@cantidad,
				@idPedidoInterno,
				@idUsuario,
				dbo.FechaActual(),
				1,/*1	Pedido Realizado 2	Pedido Enviado ó Atendido 3	Pedido Rechazado 4	Pedido Finalizado */
				@cantidad

				)
			COMMIT TRAN
			SELECT 200 Estatus , 'Pedido registrado exitosamente' Mensaje
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		select 
			-1 Estatus ,
			'Ha ocurrido un error al registrar el pedido ' Mensaje,
			 ERROR_NUMBER() AS ErrorNumber  ,
			 ERROR_MESSAGE() AS ErrorMessage  

	END CATCH

END

GO
/****** Object:  StoredProcedure [dbo].[SP_APP_ACTUALIZA_ESTATUS_PEDIDO_INTERNO]    Script Date: 22/09/2020 08:41:39 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--drop procedure SP_APP_ACTUALIZA_ESTATUS_PEDIDO_INTERNO
ALTER PROCEDURE [dbo].[SP_APP_ACTUALIZA_ESTATUS_PEDIDO_INTERNO]
@idPedidoInterno int ,
@idUsuario int,
@idEstatusPedidoInterno int , 
@idAlmacenOrigen int,-- el almacen del usaurio que esta logueado en la hand held
@idAlmacenDestino int,

/*
los siguientes parametros son necesarios cuando se atendio un pedido para darle salida al producto del alamcen que lo atendio
por tal razon solo es necesario cuando el @idEstatusPedidoInterno = 2 PEDIDO APROBADO
*/
@idUbcacion int = null,
@observacion varchar(max) = null,
@cantidadAtendida  int = null
AS
BEGIN
	BEGIN TRY
	
			if not exists (select 1 from PedidosInternos where 
			(idAlmacenDestino = @idAlmacenDestino or idAlmacenDestino =  @idAlmacenOrigen) and
			(idAlmacenOrigen = @idAlmacenOrigen  or idAlmacenOrigen = @idAlmacenDestino ) and
			idPedidoInterno = @idPedidoInterno)
			begin
				select -1 Estatus , 'El pedido no corresponde al almacen solicitado' mensaje
				return
			end
			declare
			@idEstatusPedidoActual int = 0
			select  @idEstatusPedidoActual = IdEstatusPedidoInterno from PedidosInternos where @idPedidoInterno =idPedidoInterno

			if (@idEstatusPedidoActual = 1 and @idEstatusPedidoInterno = 4)
			begin
				select -1 Estatus , 'No se puede finalizar el pedido sin ser atendido o rechazado' mensaje
				return
			end

			if exists (select  1  from PedidosInternos where IdEstatusPedidoInterno = @idEstatusPedidoInterno and @idPedidoInterno =idPedidoInterno)
			begin
				select -1 Estatus , 'El estatus del pedido actual es el mismo por favor verifica el estatus' mensaje
				return
			end
			

			if EXISTS (select  1  from PedidosInternos where IdEstatusPedidoInterno  = 2 /*Pedido Enviado ó Atendido*/ and  idPedidoInterno =  @idPedidoInterno ) AND
						@idEstatusPedidoInterno = 3
			BEGIN
				select -1 Estatus , 'No es posible cancelar el Pedido, este pedido ya fue atendido' mensaje
				return
			END
			
			BEGIN TRAN 
				--OBTENEMOS LA FECHA MAS QUE NADA LA HORA ACTUAL DE NUESTRA ZONA HORARIA

				declare	@fecha  datetime
				select @fecha  = [dbo].[FechaActual]()

				--INSERTAMOS LA ACTUALIZACION EN LA TABLA DE MOVIMIENTOS DE MERCANCIA
				INSERT INTO  MovimientosDeMercancia 
					(
					 idAlmacenOrigen
					,idAlmacenDestino
					,idProducto
					,cantidad
					,idPedidoInterno
					,idUsuario
					,fechaAlta
					,idEstatusPedidoInterno
					,observaciones,
					cantidadAtendida
					)
					SELECT 
					@idAlmacenOrigen,
					@idAlmacenDestino,
					PD.idProducto,
					PD.cantidad,
					@idPedidoInterno,
					@idUsuario,
					@fecha,
					@idEstatusPedidoInterno,
					coalesce(@observacion,''),
					coalesce(@cantidadAtendida,0)
					FROM PedidosInternos P join PedidosInternosDetalle PD
					on  P.idPedidoInterno = PD.idPedidoInterno WHERE P.idPedidoInterno = @idPedidoInterno
			
				--insertamos en el log las actualizacion del pedido especial
				insert into PedidosInternosLog
				(
					idPedidoInterno
					,idAlmacenOrigen
					,idAlmacenDestino
					,idUsuario
					,IdEstatusPedidoInterno
					,fechaAlta
				)select 
					@idPedidoInterno
					,@idAlmacenOrigen 
					,@idAlmacenDestino 
					,@idUsuario
					,@idEstatusPedidoInterno
				    ,@fecha
				--from PedidosInternos
			 	--where idPedidoInterno = @idPedidoInterno

				--INSERTAMOS LA ACUTALIACION DEL ESTATUS
				UPDATE PedidosInternos 
				SET IdEstatusPedidoInterno = @idEstatusPedidoInterno,
				observacion = case  when @observacion is null or  @observacion = '' then observacion
								    when @observacion is NOT null AND  @observacion  != '' then @observacion
							   end
				WHERE idPedidoInterno =@idPedidoInterno

				--INSERTAMOS LA ACTuALIAzaCION DE LA CANTIDAD QUE SE ATENDIO YA QUE PUEDE SER QUE NO SE ENVIE LA QUE SE SOLICITO
				update PedidosInternosDetalle set cantidadAtendida = @cantidadAtendida
				where idPedidoInterno =@idPedidoInterno
		
					declare 
					@_IdAlmacenDestino int,
					@idProducto int ,
					@cantidadPedidoInterno int ,
					@idUbicacion int,
					@cantidadActualInventario  int,
					@idTipoMonInventario int = 7, -- Salida de mercancia por pedido interno
					@cantidadDespuesDeOperacion int = 0
				--DECREMENTAMOS LA MERCANCIA EN EL INVENTARIO DEL ALAMACEN
				
				IF(@idEstatusPedidoInterno in (2)) --ATENDIDO O ENVIADO
				BEGIN

					
					select  @idProducto = idProducto ,  @cantidadPedidoInterno= isnull(cantidad,0)  
					from PedidosInternosDetalle where idPedidoInterno =@idPedidoInterno

					--SI LA CANTIDAD QUE ATENDIMOS ES DIFERENTE QUE LA QUE SE PIDIO SE SETEA A CANTIDAD YA QUE CON ESA VARIABLE
					--SE HACEN LAS OPERACIONES PARA INVENTARIO DETALLE Y DETALLE LOG,
					--SI ES IGUAL  PUES NO AFECTA CON CUAL VARIABLE REALIZAMOS EL CALCULO
					if (@cantidadAtendida != isnull(@cantidadPedidoInterno,0))
						SET @cantidadPedidoInterno = @cantidadAtendida

					--VALIDAMOS QUE EL ID PRODUCTO EXISTA EN EL INVENTARIO
					IF NOT EXISTS (SELECT 1 FROM InventarioDetalle WHERE idUbicacion = @idUbcacion and idProducto = @idProducto)
					BEGIN
						 RAISERROR('No existe producto cargado en el inventario del alamacen', 15, 217)
					END

					SELECT @cantidadActualInventario = cantidad  FROM InventarioDetalle 
					WHERE idUbicacion = @idUbcacion and idProducto = @idProducto
					
					-- VALIDAMOS QUE EL RESULTADO QUE SE OBTIENE NO SEA NULL PARA PODER  HACER LA RESTA
					SET @cantidadActualInventario = isnull(@cantidadActualInventario, 0)
					
					IF (@cantidadActualInventario < @cantidadPedidoInterno)
					BEGIN
						 RAISERROR('No existe suficiente canditad en el inventario para actualizar el pedido', 15, 217)
					END

					SET @cantidadDespuesDeOperacion =  @cantidadActualInventario-@cantidadPedidoInterno
					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE LOG
					INSERT INTO InventarioDetalleLog (  idUbicacion,
														idProducto,
														cantidad,
														cantidadActual,
														idTipoMovInventario,
														idUsuario,
														fechaAlta,
														idPedidoInterno
														)
					VALUES ( @idUbcacion,@idProducto,@cantidadPedidoInterno ,@cantidadDespuesDeOperacion,@idTipoMonInventario /* Salida pedido */,@idUsuario,dbo.FechaActual(),@idPedidoInterno)

					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					update InventarioDetalle set cantidad = @cantidadDespuesDeOperacion
					where idUbicacion = @idUbcacion and idProducto = @idProducto
				END

			COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN 
		SELECT -1 Estatus ,ERROR_MESSAGE() Mensaje, ERROR_LINE() LineaError 
	END CATCH
		
	     select 200 Estatus , 'Registro actualizado exitosamente' Mensaje
END

GO
/****** Object:  StoredProcedure [dbo].[SP_APP_ACEPTA_PEDIDO_INTERNO]    Script Date: 22/09/2020 09:01:14 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--drop procedure SP_APP_ACEPTA_PEDIDO_INTERNO
ALTER PROCEDURE [dbo].[SP_APP_ACEPTA_PEDIDO_INTERNO]
@idPedidoInterno int ,
@idUsuario int,
@idAlmacenOrigen int,-- el almacen del usaurio que esta logueado en la hand held
@idAlmacenDestino int,
@cantidadAceptada int,
@observacion varchar(max) = null
AS
BEGIN
	BEGIN TRY
	        
			declare @idEstatusPedidoInterno int=4, --pedido finalizado
			@cantidadAtendida int,			
			@idEstatusPedidoActual int = 0
		
	
			if not exists (select 1 from PedidosInternos where 
			(idAlmacenDestino = @idAlmacenDestino or idAlmacenDestino =  @idAlmacenOrigen) and
			(idAlmacenOrigen = @idAlmacenOrigen  or idAlmacenOrigen = @idAlmacenDestino ) and
			idPedidoInterno = @idPedidoInterno)
			begin
				select -1 Estatus , 'El pedido no corresponde al almacen solicitado' mensaje
				return
			end
			
			select  @idEstatusPedidoActual = IdEstatusPedidoInterno from PedidosInternos where @idPedidoInterno =idPedidoInterno

			if (@idEstatusPedidoActual not in (2))
			begin
				select -1 Estatus , 'No se puede finalizar el pedido sin ser atendido' mensaje
				return
			end

			if exists (select  1  from PedidosInternos where IdEstatusPedidoInterno = @idEstatusPedidoInterno and @idPedidoInterno =idPedidoInterno)
			begin
				select -1 Estatus , 'El estatus del pedido actual es el mismo por favor verifica el estatus' mensaje
				return
			end
			

			if EXISTS (select  1  from PedidosInternosLog where IdEstatusPedidoInterno  = @idEstatusPedidoInterno  and  idPedidoInterno =  @idPedidoInterno )

			BEGIN
				select -1 Estatus , 'No es posible aceptar el Pedido, este pedido ya fue aceptado' mensaje
				return
			END

			select @cantidadAtendida=coalesce(cantidadAtendida,0) from PedidosInternosDetalle where idPedidoInterno=@idPedidoInterno
			select @cantidadAtendida=coalesce(@cantidadAtendida,0)
			if @cantidadAceptada>@cantidadAtendida
			BEGIN
				select -1 Estatus , 'La cantidad aceptada no puede ser mayor que la cantidad atendida' mensaje
				return
			END


			
			BEGIN TRAN 
				--OBTENEMOS LA FECHA MAS QUE NADA LA HORA ACTUAL DE NUESTRA ZONA HORARIA

				declare	@fecha  datetime
				select @fecha  = [dbo].[FechaActual]()

				declare @cantidadDif int=0

			    select @cantidadDif=coalesce(@cantidadAtendida,0)-coalesce(@cantidadAceptada,0)
				
				select @cantidadDif=coalesce(@cantidadDif,0)
				--INSERTAMOS LA ACTUALIZACION EN LA TABLA DE MOVIMIENTOS DE MERCANCIA
				INSERT INTO  MovimientosDeMercancia 
					(
					 idAlmacenOrigen
					,idAlmacenDestino
					,idProducto
					,cantidad
					,idPedidoInterno
					,idUsuario
					,fechaAlta
					,idEstatusPedidoInterno
					,observaciones
					,cantidadAtendida					
					)
					SELECT 
					@idAlmacenOrigen,
					@idAlmacenDestino,
					PD.idProducto,
					PD.cantidad,
					@idPedidoInterno,
					@idUsuario,
					@fecha,
					@idEstatusPedidoInterno,
					coalesce(@observacion,''),
					@cantidadAceptada
					FROM PedidosInternos P join PedidosInternosDetalle PD
					on  P.idPedidoInterno = PD.idPedidoInterno WHERE P.idPedidoInterno = @idPedidoInterno
			
				--insertamos en el log el estado anterior 
				insert into PedidosInternosLog
				(
					idPedidoInterno
					,idAlmacenOrigen
					,idAlmacenDestino
					,idUsuario
					,IdEstatusPedidoInterno
					,fechaAlta					
				)select 
					@idPedidoInterno
					,@idAlmacenOrigen
					,@idAlmacenDestino
					,@idUsuario
					,@IdEstatusPedidoInterno
					,@fecha
				--from PedidosInternos
				--where idPedidoInterno = @idPedidoInterno

				--INSERTAMOS LA ACUTALIACION DEL ESTATUS
				UPDATE PedidosInternos 
				SET IdEstatusPedidoInterno = @idEstatusPedidoInterno,
				observacion = case  when @observacion is null or  @observacion = '' then observacion
								    when @observacion is NOT null AND  @observacion  != '' then @observacion
							   end
				WHERE idPedidoInterno =@idPedidoInterno

				--INSERTAMOS LA ACTuALIAzaCION DE LA CANTIDAD QUE SE ATENDIO YA QUE PUEDE SER QUE NO SE ENVIE LA QUE SE SOLICITO
				update PedidosInternosDetalle set cantidadAceptada = @cantidadAceptada,cantidadRechazada=@cantidadDif
				where idPedidoInterno =@idPedidoInterno
		
				declare 
				@_IdAlmacenDestino int,
				@idProducto int ,
				@cantidadPedidoInterno int ,
				@cantidadActualInventario  int,
				@idTipoMovInventario int = 9, -- Carga de mercancia por pedido interno aceptado
				@cantidadDespuesDeOperacion int = 0,
				@idUbicacion int

				--INCREMENTAMOS LA MERCANCIA EN EL INVENTARIO DEL ALAMACEN
					
				select  @idProducto = idProducto ,  @cantidadPedidoInterno= isnull(cantidad,0)  
				from PedidosInternosDetalle where idPedidoInterno =@idPedidoInterno

					--SI LA CANTIDAD QUE ATENDIMOS ES DIFERENTE QUE LA QUE SE PIDIO SE SETEA A CANTIDAD YA QUE CON ESA VARIABLE
					--SE HACEN LAS OPERACIONES PARA INVENTARIO DETALLE Y DETALLE LOG,
					--SI ES IGUAL  PUES NO AFECTA CON CUAL VARIABLE REALIZAMOS EL CALCULO
					if (@cantidadAceptada != isnull(@cantidadPedidoInterno,0))
						SET @cantidadPedidoInterno = @cantidadAceptada

					select @idUbicacion= idUbicacion FROM Ubicacion WHERE idAlmacen = @idAlmacenOrigen and idPasillo=0 and idRaq=0 and idPiso=0

					--VALIDAMOS QUE EXISTA LA UBICACION DE SIN ACOMODAR Y EN CASO DE QUE NO EXISTE LA INSERTAMOS
					IF (coalesce(@idUbicacion,0)=0)
					BEGIN
						 INSERT INTO Ubicacion(idAlmacen,idPasillo,idRaq,idPiso)
						 select @idAlmacenOrigen,0,0,0

						 select @idUbicacion=max(idUbicacion) from Ubicacion where idAlmacen=@idAlmacenOrigen				

					END					

					SELECT @cantidadActualInventario = cantidad  FROM InventarioDetalle 
					WHERE idUbicacion = @idUbicacion and idProducto = @idProducto
					
					-- VALIDAMOS QUE EL RESULTADO QUE SE OBTIENE NO SEA NULL 
					SET @cantidadActualInventario = isnull(@cantidadActualInventario, 0)					
	

					SET @cantidadDespuesDeOperacion =  @cantidadActualInventario+@cantidadPedidoInterno
					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE LOG
					INSERT INTO InventarioDetalleLog (  idUbicacion,
														idProducto,
														cantidad,
														cantidadActual,
														idTipoMovInventario,
														idUsuario,
														fechaAlta,
														idPedidoInterno
														)
					VALUES ( @idUbicacion,@idProducto,@cantidadPedidoInterno ,@cantidadDespuesDeOperacion,@idTipoMovInventario,@idUsuario,dbo.FechaActual(),@idPedidoInterno)

					--VALIDACION SI NO EXISTE INVENTARIO DETALLE CON ESE UBICACION LA INSERTAMOS
					IF NOT EXISTS(select 1 from InventarioDetalle where idUbicacion=@idUbicacion and idProducto=@idProducto)
					--INSERTAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					INSERT INTO  InventarioDetalle(idProducto,cantidad,fechaAlta,idUbicacion)
					SELECT @idProducto, @cantidadDespuesDeOperacion,@fecha,@idUbicacion
					ELSE
					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					update InventarioDetalle set cantidad = @cantidadDespuesDeOperacion,fechaActualizacion=@fecha
					where idUbicacion = @idUbicacion and idProducto = @idProducto	
					
				   -----EN CASO DE HABER SOBRANTE SE REGRESA EL SOBRANTE AL ALMACEN DESTINO
				   if(@cantidadDif>0)
				   begin        
						
					INSERT INTO  MovimientosDeMercancia 
					(
					 idAlmacenOrigen
					,idAlmacenDestino
					,idProducto
					,cantidad
					,idPedidoInterno
					,idUsuario
					,fechaAlta
					,idEstatusPedidoInterno
					,observaciones
					,cantidadAtendida					
					)
					SELECT 
					@idAlmacenOrigen,
					@idAlmacenDestino,
					PD.idProducto,
					PD.cantidad,
					@idPedidoInterno,
					@idUsuario,
					@fecha,
					3 /* Pedido Rechazado (producto rechazado) */,
					coalesce(@observacion,''),
					@cantidadDif
					FROM PedidosInternos P join PedidosInternosDetalle PD
					on  P.idPedidoInterno = PD.idPedidoInterno WHERE P.idPedidoInterno = @idPedidoInterno
						
						select @idTipoMovInventario=10--Actualizacion de Inventario(carga de mercancia por rechazo de pedido interno)

						select @idUbicacion= idUbicacion FROM Ubicacion WHERE idAlmacen = @idAlmacenDestino and idPasillo=0 and idRaq=0 and idPiso=0

						--VALIDAMOS QUE EXISTA LA UBICACION DE SIN ACOMODAR Y EN CASO DE QUE NO EXISTE LA INSERTAMOS
						IF (coalesce(@idUbicacion,0)=0)
						BEGIN
							 INSERT INTO Ubicacion(idAlmacen,idPasillo,idRaq,idPiso)
							 select @idAlmacenDestino,0,0,0

							 select @idUbicacion=max(idUbicacion) from Ubicacion where idAlmacen=@idAlmacenDestino				

						END					

						SELECT @cantidadActualInventario = cantidad  FROM InventarioDetalle 
						WHERE idUbicacion = @idUbicacion and idProducto = @idProducto
					
					-- VALIDAMOS QUE EL RESULTADO QUE SE OBTIENE NO SEA NULL 
					SET @cantidadActualInventario = isnull(@cantidadActualInventario, 0)					
	

					SET @cantidadDespuesDeOperacion =  @cantidadActualInventario+@cantidadDif
					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE LOG
					INSERT INTO InventarioDetalleLog (  idUbicacion,
														idProducto,
														cantidad,
														cantidadActual,
														idTipoMovInventario,
														idUsuario,
														fechaAlta,
														idPedidoInterno
														)
					VALUES (@idUbicacion,@idProducto,@cantidadDif,@cantidadDespuesDeOperacion,@idTipoMovInventario,@idUsuario,dbo.FechaActual(),@idPedidoInterno)

					--VALIDACION SI NO EXISTE INVENTARIO DETALLE CON ESE UBICACION LA INSERTAMOS
					IF NOT EXISTS(select 1 from InventarioDetalle where idUbicacion=@idUbicacion and idProducto=@idProducto)
					--INSERTAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					INSERT INTO  InventarioDetalle(idProducto,cantidad,fechaAlta,idUbicacion)
					SELECT @idProducto, @cantidadDespuesDeOperacion,@fecha,@idUbicacion
					ELSE
					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					update InventarioDetalle set cantidad = @cantidadDespuesDeOperacion,fechaActualizacion=@fecha
					where idUbicacion = @idUbicacion and idProducto = @idProducto	
					
				   
				   end		
				   		

			COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN 
		SELECT -1 Estatus ,ERROR_MESSAGE() Mensaje, ERROR_LINE() LineaError 
	END CATCH
		
	     select 200 Estatus , 'Registro actualizado exitosamente' Mensaje
END
