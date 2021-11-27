USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2
if exists (select * from sysobjects where name like 'SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2' and xtype = 'p' )
	drop proc SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2
go

/*
Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/09/10
Objetivo		confirma los productos del pedido especial
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2]

	@listaProductos						xml,
	@idPedidoEspecial					int,
	@idEstatusPedidoEspecial			int,	
	@idUsuarioEntrega					int,
	@numeroUnidadTaxi					varchar(255),
	@idEstatusCuentaPorCobrar			int,
	@montoTotal							float,
	@montoTotalcantidadAbonada			float,
	@aCredito							bit,
	@idTipoPago							varchar(100),
	@aplicaIVA							bit,
	@idFactUsoCFDI						int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status						int = 200,
						@mensaje					varchar(255) = '',
						@error_line					varchar(255) = '',
						@error_procedure			varchar(255) = '',
						@fecha						varchar(255) = '',
						@tran_name					varchar(32) = 'CONFIRMA_PRODUCTOS_PEDIDO_ESP',
						@tran_count					int = @@trancount,
						@tran_scope					bit = cast(0 as bit),
						@hayRechazos				bit = cast(0 as bit),
						@hayNoAceptados				bit = cast(0 as bit),
						@valido						bit = cast(1 as bit),
						@idCliente					int = 0,
						@idUsuario					int = 0,
						@saldoInicial				float = 0,
						@idCuentaPorCobrar			bigint = 0,
						@montoIva					float = 0,
						@montoComision				float = 0,
						@porcentajeComision			float = 0,
						@idAbonoCliente				bigint = 0,
						@autorizadoMayoreo			bit = cast(1 as bit),
						@totalProductos				float = 0

				create table 
					#cantidadSolicitada 
						(  
							id						int identity(1,1),
							cantidadSolicitada		float
						)

				create table 
					#cantidadAtendida 
						(  
							id						int identity(1,1),
							cantidadAtendida		float
						)
				create table 
					#cantidadRechazada 
						(  
							id						int identity(1,1),
							cantidadRechazada		float
						)
				create table 
					#cantidadAceptada 
						(  
							id						int identity(1,1),
							cantidadAceptada		float
						)

				create table 
					#idProductos 
						(  
							id			int identity(1,1),
							idProducto	int
						)

				create table 
					#idPedidoEspecialDetalle
						(  
							id							int identity(1,1),
							idPedidoEspecialDetalle		int
						)

				create table 
					#observaciones
						(  
							id							int identity(1,1),
							observaciones				varchar(500)
						)


			end  --declaraciones 

			begin -- principal

				if not exists ( select 1 from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial )
				begin
					if ( @idPedidoEspecial > 0 )
					begin 
						select @mensaje = 'No existe el pedido especial solicitado.'
						select @valido = cast(0 as bit)
						raiserror (@mensaje, 11, -1)					
					end
				end
			
			
				begin -- inicio transaccion
					if @tran_count = 0
						begin tran @tran_name
					else
						save tran @tran_name
					select @tran_scope = 1
				end -- inicio transaccion
				

				select @fecha = dbo.FechaActual()

				insert into #cantidadSolicitada (cantidadSolicitada)
				SELECT Pedidos.cantidadSolicitada.value('.','NVARCHAR(200)') AS cantidadSolicitada FROM @listaProductos.nodes('//cantidadSolicitada') as Pedidos(cantidadSolicitada) 

				insert into #cantidadAtendida (cantidadAtendida)
				SELECT Pedidos.cantidadAtendida.value('.','NVARCHAR(200)') AS cantidadAtendida FROM @listaProductos.nodes('//cantidadAtendida') as Pedidos(cantidadAtendida) 

				insert into #cantidadRechazada  (cantidadRechazada )
				SELECT Pedidos.cantidadRechazada .value('.','NVARCHAR(200)') AS cantidadRechazada  FROM @listaProductos.nodes('//cantidadRechazada ') as Pedidos(cantidadRechazada ) 

				insert into #cantidadAceptada (cantidadAceptada)
				SELECT Pedidos.cantidadAceptada.value('.','NVARCHAR(200)') AS cantidadAceptada FROM @listaProductos.nodes('//cantidadAceptada') as Pedidos(cantidadAceptada) 

				insert into #idProductos (idProducto)
				SELECT Pedidos.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @listaProductos.nodes('//idProducto') as Pedidos(idProducto)

				insert into #idPedidoEspecialDetalle (idPedidoEspecialDetalle)
				SELECT Pedidos.idPedidoEspecialDetalle.value('.','NVARCHAR(200)') AS idAlmacen FROM @listaProductos.nodes('//idPedidoEspecialDetalle') as Pedidos(idPedidoEspecialDetalle)

				insert into #observaciones (observaciones)
				SELECT Pedidos.observaciones.value('.','NVARCHAR(200)') AS observaciones FROM @listaProductos.nodes('//observaciones') as Pedidos(observaciones)
				
				-- universo de venta de productos
				select	p.idProducto, 
						i.idPedidoEspecialDetalle,
						ped.idAlmacenOrigen,
						ped.idAlmacenDestino,
						cs.cantidadSolicitada,
						ca.cantidadAtendida,
						cr.cantidadRechazada,
						cac.cantidadAceptada,
						o.observaciones
				into	#productos
				from	#idProductos p_
						join Productos p
							on p.idProducto = p_.idProducto
						join #idPedidoEspecialDetalle i
							on i.id = p_.id
						join PedidosEspecialesDetalle ped
							on ped.idPedidoEspecialDetalle = i.idPedidoEspecialDetalle
						left join #observaciones o
							on o.id = p_.id
						left join #cantidadSolicitada cs
							on cs.id = p_.id
						left join #cantidadAtendida ca
							on ca.id = p_.id
						left join #cantidadRechazada cr
							on cr.id = p_.id
						left join #cantidadAceptada cac
							on cac.id = p_.id

													   
				select	@idCliente = idCliente,
						@idUsuario = idUsuario
				from	PedidosEspeciales 
				where	idPedidoEspecial = @idPedidoEspecial


				-- si es tarjeta de credito o debito se aplica comision 
				if ( ( @idTipoPago in ( '4', '18' ) ) and ( @aplicaIVA = (cast (0 as bit) ) ) ) 
					begin	
						select @porcentajeComision = porcentaje from Comisiones where idComision = 1 and activo = cast(1 as bit)
					end
				
				select @porcentajeComision = coalesce(@porcentajeComision, 0.0)
				
				select @montoTotalcantidadAbonada = coalesce(@montoTotalcantidadAbonada, 0.0)
				
				select @totalProductos = sum(cantidadAceptada) from #Productos
				select @totalProductos = coalesce(@totalProductos, 0.0)

				
				-- universo de productos aceptados para calculo de precio de venta de pedido especial
				select	p_.idProducto, p_.idPedidoEspecialDetalle, p_.cantidadAceptada,  @autorizadoMayoreo as autorizadoMayoreo, 
						p.precioIndividual, p.precioMenudeo, ped.precioRango, ped.precioVenta
				into	#productosPrecios
				from	#productos p_
							join PedidosEspecialesDetalle ped
								on	ped.idPedidoEspecialDetalle = p_.idPedidoEspecialDetalle 
								and ped.idPedidoEspecial = @idPedidoEspecial
								and	ped.idProducto = p_.idProducto
							join Productos p 
								on p.idProducto = p_.idProducto

				--select * from #productosPrecios
				-- select * from FactCatFormaPago
				-- select * from Comisiones
				--select * from FactCatMetodoPago


				-- acualizamos estatus de pedido especial
				update	PedidosEspeciales
				set		idEstatusPedidoEspecial = @idEstatusPedidoEspecial,
						idUsuarioEntrega = @idUsuarioEntrega,
						numeroUnidadTaxi = @numeroUnidadTaxi												
				where	idPedidoEspecial = @idPedidoEspecial

				
				-- si no se aceptaron todos los productos solicitados 
				-- acualizamos estatus de pedido especial detalle
				if exists ( select 1 from #productos where cantidadSolicitada <> cantidadAceptada )
					begin
					
						update	PedidosEspecialesDetalle
						set		PedidosEspecialesDetalle.observacionesConfirmar = a.observaciones,
								idEstatusPedidoEspecialDetalle = 6		-- Rechazados por el Administrador
						from	(
									select	idProducto, idPedidoEspecialDetalle, observaciones 
									from	#productos
								)A
						where	PedidosEspecialesDetalle.idPedidoEspecialDetalle = a.idPedidoEspecialDetalle


						-- si el total aceptado de productos es < 6 y no esta autorizado para mayoreo se debe actualizar el precio de venta 
						if ( (@totalProductos < 6) and ( @autorizadoMayoreo = cast(0 as bit) ) )
							begin
								
								update	#productosPrecios 
								set		precioVenta = precioIndividual

							end

						-- si hubo cambios en los productos aceptados habra que actualizar cantidades y montos de venta
						update	PedidosEspecialesDetalle
						set		cantidad = a.cantidadAceptada,
								monto =  Round( (a.cantidadAceptada * a.precioVenta),2,0) 
						from	(
									select	idProducto, idPedidoEspecialDetalle, cantidadAceptada, precioIndividual, precioMenudeo, precioRango, precioVenta
									from	#productosPrecios
								)A
						where	PedidosEspecialesDetalle.idPedidoEspecialDetalle = a.idPedidoEspecialDetalle


						update	PedidosEspeciales
						set		montoTotal = a.montoTotal,
								cantidad = a.cantidad
						from	(
									select	sum(monto) as montoTotal, sum(cantidad) as cantidad 
									from	PedidosEspecialesDetalle where idPedidoEspecial = @idPedidoEspecial
								)A
						where	PedidosEspeciales.idPedidoEspecial = @idPedidoEspecial

					end
				else
					begin
					
						update	PedidosEspecialesDetalle
						set		PedidosEspecialesDetalle.idEstatusPedidoEspecialDetalle = 2	--Atendidos
						where	PedidosEspecialesDetalle.idPedidoEspecial = @idPedidoEspecial

					end


				-- se envian a sin acomodar los productos que son rechazados
				if exists ( select 1 from #productos where cantidadRechazada > 0 )
				begin
					select @hayRechazos = cast(1 as bit)
				end

				if exists ( select 1 from #productos where cantidadAceptada <> cantidadSolicitada )
				begin
					select @hayNoAceptados = cast(1 as bit)
				end

				
				if ( @hayRechazos = cast(1 as bit) or @hayNoAceptados = cast(1 as bit) )
					begin

						--verificamos si existe el id sin ubicacion
						select	p.* , u.idUbicacion as idUbicacionRegresar
						into	#tempUbicacionesDevoluciones_
						from	#productos p
									left join Ubicacion u
										on p.idAlmacenOrigen = u.idAlmacen
						where	u.idPasillo = 0
							and	u.idRaq = 0
							and u.idPiso = 0


						-- si no existe insertamos la ubicacion sin acomodar
						if exists	(
										select 1 from #tempUbicacionesDevoluciones_ where idUbicacionRegresar is null
									)
						begin
							insert into Ubicacion (idAlmacen, idPasillo, idRaq, idPiso)
							select idAlmacenOrigen, 0,0,0 from #tempUbicacionesDevoluciones_
						end
						

						-- inserta los registros que se regresaron
						-- rechazados
						insert	into InventarioDetalleLog (idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idVenta)
						select	distinct temp.idUbicacionRegresar, temp.idProducto, rechazados.cantidadRechazada, actuales.cantidad + rechazados.cantidadRechazada as cantidadActual,
								20 as idTipoMovInventario, -- 20	Actualizacion de Inventario(carga de mercancia por pedido especial rechazado)
								@idUsuarioEntrega as idUsuario, cast(@fecha as date) as fechaAlta, cast(0 as int) as idVenta
						from	#tempUbicacionesDevoluciones_ temp
									join (
											select	p.idProducto,  p.cantidadRechazada												
											from	#productos p
											where	p.cantidadRechazada	> 0
										 )rechazados on rechazados.idProducto = temp.idProducto
									join InventarioDetalle actuales
										on actuales.idProducto = temp.idProducto and actuales.idUbicacion = temp.idUbicacionRegresar
					
						-- no aceptados
						insert	into InventarioDetalleLog (idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idVenta)
						select	distinct temp.idUbicacionRegresar, temp.idProducto, rechazados.noAceptados, actuales.cantidad + rechazados.noAceptados as cantidadActual,
								20 as idTipoMovInventario, -- 20	Actualizacion de Inventario(carga de mercancia por pedido especial rechazado)
								@idUsuarioEntrega as idUsuario, cast(@fecha as date) as fechaAlta, cast(0 as int) as idVenta
						from	#tempUbicacionesDevoluciones_ temp
									join (	
											select	p.idProducto,  (p.cantidadAtendida - cantidadAceptada) as noAceptados
											from	#productos p
											where	(p.cantidadAtendida - cantidadAceptada)	> 0
										 )rechazados on rechazados.idProducto = temp.idProducto
									join InventarioDetalle actuales
										on actuales.idProducto = temp.idProducto and actuales.idUbicacion = temp.idUbicacionRegresar
					


						-- actualizamos InventarioDetalle
						-- rechazados
						update	InventarioDetalle 
						set		InventarioDetalle.cantidad = a.cantidad,
								InventarioDetalle.fechaActualizacion  = @fecha
						from	(
									select	temp.idProducto, actuales.cantidad + rechazados.cantidadRechazada as cantidad, temp.idUbicacionRegresar
									from	#tempUbicacionesDevoluciones_ temp
												join (
														select	p.idProducto, p.cantidadRechazada
														from	#productos p
														where	p.cantidadRechazada > 0
													 )rechazados on rechazados.idProducto = temp.idProducto
												join InventarioDetalle actuales
													on actuales.idProducto = temp.idProducto and actuales.idUbicacion = temp.idUbicacionRegresar
								)A
						where	InventarioDetalle.idUbicacion = a.idUbicacionRegresar
							and	InventarioDetalle.idProducto = a.idProducto


							
						-- no aceptados
						update	InventarioDetalle 
						set		InventarioDetalle.cantidad = a.cantidad,
								InventarioDetalle.fechaActualizacion  = @fecha
						from	(
									select	temp.idProducto, actuales.cantidad + rechazados.noAceptados as cantidad, temp.idUbicacionRegresar
									from	#tempUbicacionesDevoluciones_ temp
												join (
														select	p.idProducto,  (p.cantidadAtendida - cantidadAceptada) as noAceptados												
														from	#productos p
														where	(p.cantidadAtendida - cantidadAceptada) > 0
													 )rechazados on rechazados.idProducto = temp.idProducto
												join InventarioDetalle actuales
													on actuales.idProducto = temp.idProducto and actuales.idUbicacion = temp.idUbicacionRegresar
								)A
						where	InventarioDetalle.idUbicacion = a.idUbicacionRegresar
							and	InventarioDetalle.idProducto = a.idProducto




						-- inserta los registros que se regresaron para los movimientos de mercancia
						-- rechazados
						insert into 
							PedidosEspecialesMovimientosDeMercancia 
								(
									idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoEspecial,idUsuario,fechaAlta,
									idEstatusPedidoEspecialDetalle,observaciones,cantidadAtendida,idUbicacionOrigen,idUbicacionDestino
								)
						select	u.idAlmacen as idAlmacenOrigen, ubicacionDestino.idAlmacenOrigen as idAlmacenDestino, idl.idProducto, rechazados.cantidadRechazada as cantidad, idl.idPedidoEspecial,
								idl.idUsuario, @fecha as fechaAlta, cast(5 as int) as idEstatusPedidoEspecialDetalle, -- 5	Atendidos/Incompletos
								rechazados.observaciones, rechazados.cantidadAtendida, idl.idUbicacion as idUbicacionOrigen, ubicacionDestino.idUbicacionOrigen as idUbicacionDestino
						from	InventarioDetalleLog idl
									join Ubicacion u
										on u.idUbicacion = idl.idUbicacion
									join	(
												select	idPedidoEspecial, idAlmacenOrigen, idEstatusPedidoEspecialDetalle, idUbicacionOrigen
												from	PedidosEspecialesMovimientosDeMercancia
												where	idPedidoEspecial = @idPedidoEspecial
													and idEstatusPedidoEspecialDetalle = 1 -- 1	Solcitados
												group by idPedidoEspecial, idAlmacenOrigen, idEstatusPedidoEspecialDetalle, idUbicacionOrigen
											)ubicacionDestino
												on ubicacionDestino.idPedidoEspecial = idl.idPedidoEspecial
									join	(
												select	idProducto,  cantidadRechazada as cantidadRechazada, observaciones, cantidadAtendida
												from	#productos 
												where	cantidadRechazada > 0												
											)rechazados
												on rechazados.idProducto = idl.idProducto
						where	idl.idPedidoEspecial = @idPedidoEspecial
							and	idl.idTipoMovInventario = 18



						-- no aceptados
						insert into 
							PedidosEspecialesMovimientosDeMercancia 
								(
									idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoEspecial,idUsuario,fechaAlta,
									idEstatusPedidoEspecialDetalle,observaciones,cantidadAtendida,idUbicacionOrigen,idUbicacionDestino
								)
						select	u.idAlmacen as idAlmacenOrigen, ubicacionDestino.idAlmacenOrigen as idAlmacenDestino, idl.idProducto, noAceptados.noAceptados as cantidad, idl.idPedidoEspecial,
								idl.idUsuario, @fecha as fechaAlta, cast(6 as int) as idEstatusPedidoEspecialDetalle, -- 6	Rechazados por el Administrador
								noAceptados.observaciones, noAceptados.cantidadAtendida, idl.idUbicacion as idUbicacionOrigen, ubicacionDestino.idUbicacionOrigen as idUbicacionDestino
						from	InventarioDetalleLog idl
									join Ubicacion u
										on u.idUbicacion = idl.idUbicacion
									join	(
												select	idPedidoEspecial, idAlmacenOrigen, idEstatusPedidoEspecialDetalle, idUbicacionOrigen
												from	PedidosEspecialesMovimientosDeMercancia
												where	idPedidoEspecial = @idPedidoEspecial
													and idEstatusPedidoEspecialDetalle = 1 -- 1	Solcitados
												group by idPedidoEspecial, idAlmacenOrigen, idEstatusPedidoEspecialDetalle, idUbicacionOrigen
											)ubicacionDestino
												on ubicacionDestino.idPedidoEspecial = idl.idPedidoEspecial
									join	(
												select	idProducto,  (cantidadSolicitada - cantidadAceptada) as noAceptados, observaciones, cantidadAtendida
												from	#productos 
												where	cantidadAceptada <> cantidadSolicitada										
											)noAceptados
												on noAceptados.idProducto = idl.idProducto
						where	idl.idPedidoEspecial = @idPedidoEspecial
							and	idl.idTipoMovInventario = 18

					end -- if ( @hayRechazos = cast(1 as bit) or @hayNoAceptados = cast(1 as bit) )

					-- se aplica primero la comision
					if (  @idTipoPago in ( '4', '18' ) )
						begin
							
							select @montoComision =  Round((@montoTotalcantidadAbonada * @porcentajeComision),2,0) 
							select @montoComision = coalesce(@montoComision, 0)

						end


					-- se obtiene monto de iva y ademas si es una venta facturada se elimina el monto de comision
					if ( @aplicaIVA = cast(1 as bit) )
						begin
							
							select @montoIva =  Round((@montoTotalcantidadAbonada * 0.16),2,0) 
							select @montoIva = coalesce(@montoIva, 0)
							select @montoComision = 0

						end


					-- Afectar las tablas de PedidosEspecialesCuentasPorCobrar cuando el pedido es a credito
					if ( @aCredito = cast(1 as bit) )
						begin
								
							select @saldoInicial = montoTotal from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial

							insert into 
								PedidosEspecialesCuentasPorCobrar 
									(
										idPedidoEspecial,idCliente,idUsuario,idTipoPago,fechaAlta,SaldoInicial,saldoActual,idEstatusCuentaPorCobrar
									)
							select	@idPedidoEspecial as idPedidoEspecial, @idCliente as idCliente, @idUsuario as idUsuario, @idTipoPago as idTipoPago, 
									@fecha as fechaAlta, @saldoInicial as SaldoInicial, (@saldoInicial - @montoTotalcantidadAbonada) as saldoActual,
									cast(1 as int) as idEstatusCuentaPorCobrar -- 1	En Crédito.
										
							select	@idCuentaPorCobrar = max(idCuentaPorCobrar) from PedidosEspecialesCuentasPorCobrar where idCliente = @idCliente

						end
					else
						begin  -- si fue liquidado
							
							if ( @aplicaIVA = cast(1 as bit) )
								begin
									--select @montoIva =  Round((@montoTotalcantidadAbonada * 0.16),2,0) 
									update	PedidosEspecialesDetalle 
									set		montoIva = Round((monto * 0.16),2,0) 
									where	idPedidoEspecial = @idPedidoEspecial

									update	PedidosEspeciales
									set		montoTotal = a.montoTotal
									from	(
												select	sum(monto + montoIva) as montoTotal
												from	PedidosEspecialesDetalle where idPedidoEspecial = @idPedidoEspecial
											)A
									where	PedidosEspeciales.idPedidoEspecial = @idPedidoEspecial

								end

						end

					-- Afectar las tablas de PedidosEspecialesAbonosCuentasPorCobrar cuando el pedido tiene un monto abonado
					if ( @montoTotalcantidadAbonada > 0.0 )
						begin
								
							insert into 
								PedidosEspecialesAbonoClientes  --a nivel general
									(
										idUsuario,monto,montoIva,montoComision,montoTotal,idCliente,requiereFactura,idFacturaAbono,
										idFactura,idFactFormaPago,idFactUsoCFDI,fechaAlta,activo
									)
							select	@idUsuario as idUsuario, @montoTotalcantidadAbonada as monto, @montoIva as montoIva, @montoComision as montoComision,
									(@montoTotalcantidadAbonada + @montoIva + @montoComision) as  montoTotal, @idCliente as  idCliente,
									@aplicaIVA as requiereFactura, cast(0 as int) as idFacturaAbono, cast(0 as int) as idFactura,
									@idTipoPago as idFactFormaPago, @idFactUsoCFDI as idFactUsoCFDI, @fecha as fechaAlta, cast(1 as bit) as activo

							select	@idAbonoCliente = max(idAbonoCliente) from PedidosEspecialesAbonoClientes where idCliente = @idCliente


							insert into 
								PedidosEspecialesAbonosCuentasPorCobrar  -- abonos a nivel desglose
									(
										monto,fechaAlta,idCliente,idUsuario,idPedidoEspecial,idCuentaPorCobrar,
										EsAbonoInicial,SaldoDespuesOperacion,idAbonoCliente
									)
							select	@montoTotalcantidadAbonada as monto, @fecha as fechaAlta, @idCliente as idCliente, @idUsuario as idUsuario, 
									@idPedidoEspecial as idPedidoEspecial, @idCuentaPorCobrar as idCuentaPorCobrar, cast(1 as bit) as EsAbonoInicial,
									(@saldoInicial - @montoTotalcantidadAbonada) as SaldoDespuesOperacion, @idAbonoCliente as idAbonoCliente

						end




				begin -- commit de transaccion
					if @tran_count = 0
						begin -- si la transacción se inició dentro de este ámbito
							commit tran @tran_name
							select @tran_scope = 0
						end -- si la transacción se inició dentro de este ámbito
				end -- commit de transaccion				

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
							

			

				
			end -- reporte de estatus

	end  -- principal

grant exec on SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2 to public
go