USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_GUARDA_PEDIDO_ESPECIAL_V2]    Script Date: 20/09/2021 02:42:12 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- se crea procedimiento SP_GUARDA_PEDIDO_ESPECIAL_V2
if exists (select * from sysobjects where name like 'SP_GUARDA_PEDIDO_ESPECIAL_V2' and xtype = 'p' )
	drop proc SP_GUARDA_PEDIDO_ESPECIAL_V2
go


/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/20
Objetivo		guarda el pedido especial
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_GUARDA_PEDIDO_ESPECIAL_V2]

	@xml						AS XML, 
	@tipoRevision				int,  --1 - ticket  /  2 - Hand Held
	@idCliente					int,
	@idUsuario					int,
	@idEstatusPedidoEspecial	int,
	@idEstacion					int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status							int = 200,
						@mensaje						varchar(255) = 'Se registro el pedido especial correctamente.',
						@error_line						varchar(255) = '',
						@error_procedure				varchar(255) = '',
						@tran_name						varchar(32) = 'REALIZA_PEDIDO_ESP',
						@tran_count						int = @@trancount,
						@tran_scope						bit = 0,						
						@totalProductos					float = 0,
						@montoTotal						money = 0,
						@descuento						money = 0.0,
						@ini							int = 0, 
						@fin							int = 0,
						@fecha							datetime,
						--@cantProductosLiq				float = 0,
						@idComisionBancaria				int = 0,
						@porcentajeComisionBancaria		money = 0.0,
						@tolerancia						money = 1.0,
						@montoAgregarProductos			money = 0,
						@ini_							int = 0,
						@fin_							int = 0,
						@idPedidoEspecial				int = 0,
						@idAlmacenSolicita				int = 0,
						@idUbicacion					int = 0,
						@idRaqResguardo					int = 0,
						@idPisoResguardo				int = 0,
						@idPasilloResguardo				int = 0

					
				create table
					#PedidosEspeciales
						(	
							contador					int identity(1,1),
							idPedidoEspecial			bigint,
							idCliente					int,
							cantidad					float,
							fechaAlta					datetime,
							montoTotal					money,
							idUsuario					int,
							idEstatusPedidoEspecial		int,
							idEstacion					int,
							observaciones				varchar(500),
							codigoBarras				varchar(250),
							idTipoPago					int,
							idUsuarioEntrega			int,
							numeroUnidadTaxi			varchar(100)
						)

				create table
					#PedidosEspecialesDetalle
						(
							idPedidoEspecialDetalle			bigint,
							idPedidoEspecial				bigint,
							idVenta							int,
							idProducto						int,
							idUbicacion						int,
							idAlmacenOrigen					int,
							idAlmacenDestino				int,
							fechaAlta						datetime,
							cantidad						float,
							monto							money,
							cantidadActualInvGeneral		float,
							cantidadAnteriorInvGeneral		float,
							precioIndividual				money,
							precioMenudeo					money,
							precioRango						money,
							precioVenta						money,
							idTicketMayoreo					int,
							observaciones					varchar(255),
							ultimoCostoCompra				money,
							cantidadAceptada				float,
							cantidadAtendida				float,
							cantidadRechazada				float,
							idEstatusPedidoEspecialDetalle	int,
							notificado						bit
						)

				create table 
					#cantidades 
						(  
							id			int identity(1,1),
							cantidad	float
						)

				create table 
					#idProductos 
						(  
							id			int identity(1,1),
							idProducto	int
						)

				create table 
					#idAlmacenes
						(  
							id			int identity(1,1),
							idAlmacen	int
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

				
				select @fecha = coalesce(@fecha, dbo.FechaActual())

				insert into #cantidades (cantidad)
				SELECT Pedidos.cantidad.value('.','NVARCHAR(200)') AS cantidad FROM @xml.nodes('//cantidad') as Pedidos(cantidad) 

				insert into #idProductos (idProducto)
				SELECT Pedidos.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @xml.nodes('//idProducto') as Pedidos(idProducto)

				insert into #idAlmacenes (idAlmacen)
				SELECT Pedidos.idAlmacen.value('.','NVARCHAR(200)') AS idAlmacen FROM @xml.nodes('//idAlmacen') as Pedidos(idAlmacen)

				
				-- validamos si el cliente tiene descuento por aplicar
				select	@descuento = t.descuento
				from	clientes c
						inner join CatTipoCliente t
							on c.idTipoCliente = t.idTipoCliente
				where	c.idCliente = @idCliente				


				-- almacen que solicita el pedido
				select	@idAlmacenSolicita = idAlmacen 
				from	Usuarios 
				where	idUsuario = @idUsuario


				-- obtenemos ubicacion para resguardo
				select @idPisoResguardo = idPiso from CatPiso where descripcion like 'Resguardo'
				select @idPasilloResguardo = idPasillo from CatPasillo where descripcion like 'Resguardo'
				select @idRaqResguardo = idRaq from CatRaq where descripcion like 'Resguardo'


				-- universo de venta de productos
				select	p.idProducto, 
						c.cantidad,
						a.idAlmacen,
						cast(0 as int) as contador, 
						cast(0 as money) as costo, 
						cast(0 as money) as precioPorProducto,
						coalesce(p.precioIndividual, 0) as precioIndividual, 
						coalesce(p.precioMenudeo, 0) as precioMenudeo, 
						cast(0 as money) as precioRango, 
						--@aplicaIVA as aplicaIVA ,  
						cast(0 as money) as montoIva, 
						cast(0 as money) as montoComisionBancaria,
						cast(0 as money) as montoVenta, 
						cast(0 as money) as precioVenta, 
						cast(0 as money) as porcentajeDescuentoCliente
				into	#pedidos
				from	#cantidades c
						inner join #idProductos p_
							on c.id = p_.id
						inner join Productos p
							on p.idProducto = p_.idProducto
						inner join #idAlmacenes a
							on a.id = c.id

							
					select @totalProductos = sum(cantidad) from #pedidos

				
					-- primero precios individual y menudeo
					update	#pedidos
					set		costo =	case
										when (@totalProductos ) >= 6 then cantidad * precioMenudeo
										else cantidad * precioIndividual
									end,
							precioVenta =	case
												when (@totalProductos) >= 6 then precioMenudeo
												else precioIndividual
											end

					-- se actualiza el precio de venta y el precio del rango con que se hizo la venta en caso q exista un precio de rango
					update	#pedidos
					set		#pedidos.precioVenta = a.precioRango,
							#pedidos.precioRango = a.precioRango
					from	(
								select	v.idProducto, v.cantidad, ppp.costo as precioRango, ppp.min, ppp.max
								from	#pedidos v
											inner join ProductosPorPrecio ppp
												on ppp.idProducto = v.idProducto
											inner join (select idProducto,max(max) maxCantRango 
														from ProductosPorPrecio 
														where
														activo = cast(1 as bit)
														group by idProducto) maxRango 
												on ppp.idProducto=maxRango.idProducto
											--left join #VentaComplemento vc on v.idProducto=vc.idProducto 
								where	ppp.activo = cast(1 as bit)
									--and	v.cantidad between ppp.min and ppp.max
									and	(v.cantidad ) >= min and (v.cantidad ) <= case when (v.cantidad ) > maxCantRango and max = maxCantRango
									then (v.cantidad ) else max end

							)A
					where	#pedidos.idProducto = a.idProducto


					-- si hay descuento
					if ( @descuento > 0.0 )
					begin
						update	#pedidos set #pedidos.porcentajeDescuentoCliente = @descuento
						update	#pedidos set #pedidos.precioVenta = #pedidos.precioVenta - (#pedidos.precioVenta * ( @descuento / 100 ))
					end


					-- actualizamos el precio final
					update	#pedidos 
					set		#pedidos.costo = precioVenta * cantidad,
							#pedidos.montoVenta = costo

					
					-- si no existe el producto en el inventario general
					if  exists	( 
									select 1 from #pedidos v 
										left join InventarioGeneral g 
											on v.idProducto=g.idProducto
									where g.idProducto is null
								)
					begin
						select @mensaje = 'El producto no se encuentra en el inventario.'
						raiserror (@mensaje, 11, -1)
					end


					-- si el producto no tiene un precio asignado en tabla de productos 
					if exists ( select 1 from #pedidos where montoVenta <= 0 )
					begin
						select @mensaje = 'El producto no tiene un precio asignado.'
						raiserror (@mensaje, 11, -1)
					end

					
					-- si todo bien
						select	@montoTotal = Round(sum(montoVenta+montoIva+montoComisionBancaria),2,0) 
						from	#pedidos


					if ( @tipoRevision = 1 )  --ticket
						begin

							-- obtenemos la ubicacion especial para cuando es revision por ticket
							if not exists	(
												select	idUbicacion
												from	Ubicacion 
												where	idAlmacen = @idAlmacenSolicita
													and idPasillo = @idPasilloResguardo --27
													and idRaq = @idRaqResguardo --26
													and idPiso = @idPisoResguardo --10
											)
							begin 						
								insert into Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) values (@idAlmacenSolicita, @idPasilloResguardo, @idRaqResguardo, @idPisoResguardo)
							end


							-- id ubicacion de resguardo
							select	@idUbicacion = idUbicacion
							from	Ubicacion 
							where	idAlmacen = @idAlmacenSolicita
								and idPasillo = @idPasilloResguardo --27
								and idRaq = @idRaqResguardo --26
								and idPiso = @idPisoResguardo --10


							if not exists ( select 1 from InventarioDetalle where idUbicacion = @idUbicacion )
							begin
								insert into InventarioDetalle ( idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion )
								select	idProducto, 0 as cantidad, @fecha as fechaAlta, @idUbicacion as idUbicacion, @fecha as fechaActualizacion
								from	#pedidos
								group by idProducto
							end

						end
					else
						begin
							
							select @idUbicacion = null

						end



					----------------------------------------------------------------------------------------------------------------------------------------------------------
					----------------------------------------------------------------------------------------------------------------------------------------------------------
					----------------------------------------------------------------------------------------------------------------------------------------------------------
					----------------------------------------------------------------------------------------------------------------------------------------------------------
					----------------------------------------------------------------------------------------------------------------------------------------------------------
					
					-- inserta en tablas fisicas
						insert into 
							PedidosEspeciales
								(
									idCliente,cantidad,fechaAlta,montoTotal,idUsuario,idEstatusPedidoEspecial,
									idEstacion,observaciones,codigoBarras,idTipoPago,idUsuarioEntrega,numeroUnidadTaxi
								)

						select	@idCliente as idCliente, @totalProductos as cantidad, @fecha as fechaAlta, @montoTotal as montoTotal, 
								@idUsuario as idUsuario, @idEstatusPedidoEspecial as idEstatusPedidoEspecial,@idEstacion as idEstacion, 
								null as observaciones, null as codigoBarras, null as idTipoPago, null as idUsuarioEntrega, null as numeroUnidadTaxi

						select @idPedidoEspecial = max(idPedidoEspecial)  from PedidosEspeciales

					-- se inserta el detalle de los productos que se vendieron
					insert into
						PedidosEspecialesDetalle
							(
								idPedidoEspecial,idProducto,idUbicacion,idAlmacenOrigen,idAlmacenDestino,fechaAlta,cantidad,monto,
								cantidadActualInvGeneral,cantidadAnteriorInvGeneral,precioIndividual,precioMenudeo,precioRango,precioVenta,
								idTicketMayoreo,observaciones,ultimoCostoCompra,cantidadAceptada,cantidadAtendida,cantidadRechazada,
								idEstatusPedidoEspecialDetalle,notificado
							)
					select	@idPedidoEspecial as idPedidoEspecial, p.idProducto, @idUbicacion as idUbicacion, @idAlmacenSolicita as idAlmacenOrigen, idAlmacen as idAlmacenDestino , 
							@fecha as fechaAlta, p.cantidad, costo as monto, cast(0 as int) as cantidadActualInvGeneral, ig.cantidad as cantidadAnteriorInvGeneral, 
							pro.precioIndividual,pro.precioMenudeo,precioRango,precioVenta,0 as idTicketMayoreo, null as observaciones, pro.ultimoCostoCompra as ultimoCostoCompra, p.cantidad as cantidadAceptada, 
							p.cantidad as cantidadAtendida, 0 as cantidadRechazada, 1 as idEstatusPedidoEspecialDetalle, null as notificado
					from	#pedidos p
								join InventarioGeneral ig
									on ig.idProducto = p.idProducto
								join Productos pro
									on pro.idProducto = p.idProducto


					if ( @tipoRevision = 1 ) -- ticket
						begin
						
							-- calculamos las existencias del inventario despues de la venta
							select idProducto, idAlmacen, cast((sum(cantidad)) as float) as cantidad into #totProductos from #pedidos group by idProducto, idAlmacen

							select	distinct 
									idInventarioDetalle, id.idProducto, p.idAlmacen, id.cantidad, fechaAlta, 
									id.idUbicacion, fechaActualizacion, cast(0 as float) as cantidadDescontada, 
									cast(0 as float) as cantidadFinal
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
							where	--u.idUsuario = @idUsuario
									p.idProducto = id.idProducto
								and	p.idAlmacen = a.idAlmacen
								and id.cantidad > 0
								and	ub.idPiso not in (9) -- se pidio que no se vendieran productos que estuvieran en el piso #9


							if not exists ( select 1 from #tempExistencias)
								begin
									select @mensaje = 'No se realizo el pedido especial, no se cuenta con suficientes existencias en el inventario.'
									raiserror (@mensaje, 11, -1)
								end


							-- se calcula de que ubicaciones se van a descontar los productos
							select @ini = min(idProducto), @fin= max(idProducto) from #totProductos

							while ( @ini <= @fin )
								begin
									declare	@cantidadProductos as float = 0
									select	@cantidadProductos = cantidad from #totProductos where idProducto = @ini

									while ( @cantidadProductos > 0 )
										begin
											declare @cantidadDest as float = 0, @idInventarioDetalle as int = 0
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
															update	#tempExistencias set cantidadDescontada = @cantidadProductos 
															where	idProducto = @ini and idInventarioDetalle = @idInventarioDetalle
															select	@cantidadProductos = 0 
														end
													else
														begin
															update	#tempExistencias set cantidadDescontada = @cantidadDest
															where	idProducto = @ini and idInventarioDetalle = @idInventarioDetalle
															select	@cantidadProductos = @cantidadProductos - @cantidadDest						
														end
												end 
										end  -- while ( @cantidadProductos > 0 )

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




								--select '#totProductos', * from #totProductos

								---------------------------------------------------------------------------------------------------------------------------------------------------------
								-- InventarioGeneral y InventarioGeneralLog 
								---------------------------------------------------------------------------------------------------------------------------------------------------------

								---------------------------------------------------------------------------------------------------------------------------------------------------------
								--- destino
								---------------------------------------------------------------------------------------------------------------------------------------------------------
								--se actualiza inventario general log salida de mercancia de destino
								insert into InventarioGeneralLog (idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
								select	a.idProducto, sum(a.cantidad), b.cantidad - sum(a.cantidad), dbo.FechaActual(), 17 
								from	#totProductos a
											join InventarioGeneral b on a.idProducto=b.idProducto
								group by a.idProducto,b.cantidad
					
								-- se actualiza el inventario general - quitar cantidades de destino (quien surte)
								update	InventarioGeneral
								set		fechaUltimaActualizacion = dbo.FechaActual(),
										InventarioGeneral.cantidad = InventarioGeneral.cantidad - a.cantidad
								from	(
											select idProducto, cantidad from #totProductos
										)A
								where InventarioGeneral.idProducto = A.idProducto

								---------------------------------------------------------------------------------------------------------------------------------------------------------
								-- origen
								---------------------------------------------------------------------------------------------------------------------------------------------------------
								--se actualiza inventario general log entrada de mercancia a almacen origen
								insert into InventarioGeneralLog (idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
								select	a.idProducto, sum(a.cantidad), b.cantidad + sum(a.cantidad), dbo.FechaActual(), 18
								from	#totProductos a
											join InventarioGeneral b on a.idProducto=b.idProducto
								group by a.idProducto,b.cantidad


								-- se actualiza el inventario general - insertar cantidades a origen (quien pide )
								update	InventarioGeneral
								set		fechaUltimaActualizacion = dbo.FechaActual(),
										InventarioGeneral.cantidad = InventarioGeneral.cantidad + a.cantidad
								from	(
											select idProducto, cantidad from #totProductos
										)A
								where InventarioGeneral.idProducto = A.idProducto




								---------------------------------------------------------------------------------------------------------------------------------------------------------
								--  InventarioDetalle y InventarioDetalleLog 
								---------------------------------------------------------------------------------------------------------------------------------------------------------						
								---------------------------------------------------------------------------------------------------------------------------------------------------------
								--- destino
								---------------------------------------------------------------------------------------------------------------------------------------------------------
								--select '#tempExistencias', * from #tempExistencias

								-- se inserta el InventarioDetalleLog
								insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta)
								select	idUbicacion, idProducto, cantidadDescontada, cantidadFinal, cast(17 as int) as idTipoMovInventario,
										@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, 0 as idVenta
								from	#tempExistencias
								where	cantidadDescontada > 0


								-- se actualiza inventario detalle
								update	InventarioDetalle
								set		InventarioDetalle.cantidad = InventarioDetalle.cantidad - a.cantidadDescontada, 
										fechaActualizacion = dbo.FechaActual()
								from	(
											select	idProducto, idUbicacion, sum(cantidadDescontada) as cantidadDescontada
											from	#tempExistencias
											where	cantidadDescontada > 0
											group by idProducto, idUbicacion
										)A
								where	InventarioDetalle.idUbicacion = a.idUbicacion
									and	InventarioDetalle.idProducto = a.idProducto 
								--InventarioDetalle.idInventarioDetalle = a.idInventarioDetalle



								---------------------------------------------------------------------------------------------------------------------------------------------------------
								--- origen
								---------------------------------------------------------------------------------------------------------------------------------------------------------
								-- se inserta el InventarioDetalleLog
								insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta)
								select	id.idUbicacion, id.idProducto, tempExistencias.cantidadDescontada, id.cantidad + tempExistencias.cantidadDescontada, 
										cast(18 as int) as idTipoMovInventario, @idUsuario as idUsuario, @fecha as fechaAlta, cast(0 as int) as idVenta
								from	
										(
											select	idProducto, cantidadDescontada
											from	#tempExistencias
											where	cantidadDescontada > 0.0
											group by idProducto, cantidadDescontada
										)  tempExistencias
											join InventarioDetalle id
												on id.idProducto = tempExistencias.idProducto and id.idUbicacion = @idUbicacion
											

								-- se actualiza inventario detalle
								update	InventarioDetalle
								set		InventarioDetalle.cantidad = InventarioDetalle.cantidad + a.cantidadDescontada, 
										fechaActualizacion = dbo.FechaActual()
								from	(
											select	idProducto, idUbicacion, sum(cantidadDescontada) as cantidadDescontada
											from	#tempExistencias
											where	cantidadDescontada > 0
											group by idProducto, idUbicacion
										)A
								where	InventarioDetalle.idUbicacion = @idubicacion
									and	InventarioDetalle.idProducto = a.idProducto 




								-- actualizamos como quedo el el inventario general actual despues de operaciones
								update	PedidosEspecialesDetalle
								set		PedidosEspecialesDetalle.cantidadActualInvGeneral = a.cantidad
								from	(
											select	ig.idProducto, ig.cantidad
											from	InventarioGeneral ig
														join #tempExistencias t
															on t.idProducto = ig.idProducto
											where	cantidadDescontada > 0
										)A
								where	PedidosEspecialesDetalle.idProducto = a.idProducto



								---------------------------------------------------------------------------------------------------------------------------------------------------------
								--PedidosEspecialesMovimientosDeMercancia
								---------------------------------------------------------------------------------------------------------------------------------------------------------
								-- estatus de solicitado
								insert into 
									PedidosEspecialesMovimientosDeMercancia 
										(
											idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoEspecial,idUsuario,
											fechaAlta,idEstatusPedidoEspecialDetalle,observaciones,cantidadAtendida
										)
								select	@idAlmacenSolicita as idAlmacenOrigen, ped.idAlmacenDestino, ped.idProducto, ped.cantidad, ped.idPedidoEspecial, @idUsuario as idUsuario,
										dbo.FechaActual() as fechaAlta, ped.idEstatusPedidoEspecialDetalle, ped.observaciones, ped.cantidadAtendida
								from	PedidosEspecialesDetalle ped
								where	ped.idPedidoEspecial = @idPedidoEspecial

								/*
								select * from MovimientosDeMercancia
								select * from PedidosEspecialesMovimientosDeMercancia
								select * from CatEstatusPedidoEspecial
								select * from CatEstatusPedidoEspecialDetalle
								*/

						
						end




						-- si es revision por ticket se cambia estatus a atendido
						if ( @tipoRevision = 1 )
							begin
								print 'es revision ticket'

								update	PedidosEspecialesDetalle
								set		idEstatusPedidoEspecialDetalle = 2	--Atendidos
								where	idPedidoEspecial = @idPedidoEspecial

								update	PedidosEspeciales
								set		idEstatusPedidoEspecial = 3	  --En resguardo
								where	idPedidoEspecial = @idPedidoEspecial

								-- movimientos de mercancia con estatus de atendido
								insert into 
									PedidosEspecialesMovimientosDeMercancia 
										(
											idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoEspecial,idUsuario,
											fechaAlta,idEstatusPedidoEspecialDetalle,observaciones,cantidadAtendida
										)
								select	@idAlmacenSolicita as idAlmacenOrigen, ped.idAlmacenDestino, ped.idProducto, ped.cantidad, ped.idPedidoEspecial, @idUsuario as idUsuario,
										dbo.FechaActual() as fechaAlta, ped.idEstatusPedidoEspecialDetalle, ped.observaciones, ped.cantidadAtendida
								from	PedidosEspecialesDetalle ped
								where	ped.idPedidoEspecial = @idPedidoEspecial

							end 



						--select @cantProductosLiq=sum(a.cantidad) from #totProductos a join Productos b on a.idProducto=b.idProducto and b.idLineaProducto in (19,20)

				begin -- commit de transaccion
					if @tran_count = 0
						begin -- si la transacción se inició dentro de este ámbito
							commit tran @tran_name
							select @tran_scope = 0
						end -- si la transacción se inició dentro de este ámbito
				end -- commit de transaccion
					
				drop table #pedidos
				drop table #PedidosEspeciales
				drop table #PedidosEspecialesDetalle
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
					@idPedidoEspecial as idPedidoEspecial
					--,
					--coalesce(@cantProductosLiq,0) as cantProductosLiq,
					--coalesce(@idDevolucion , 0)	  as idDevolucion,
					--coalesce(@idComplemento ,0)   as idComplemento

		end -- reporte de estatus

	end  -- principal

grant exec on SP_GUARDA_PEDIDO_ESPECIAL_V2 to public
go