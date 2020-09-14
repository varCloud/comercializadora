use [DB_A57E86_lluviadesarrollo]
go

-- se crea procedimiento SP_GUARDA_PEDIDO_ESPECIAL
if exists (select * from sysobjects where name like 'SP_GUARDA_PEDIDO_ESPECIAL' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_GUARDA_PEDIDO_ESPECIAL
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/09/10
Objetivo		Guarda el pedido especial
status			200 = ok
				-1	= error
*/

create proc SP_GUARDA_PEDIDO_ESPECIAL

	@xml					AS XML, 
	@idCliente				int,
	@idPedidoEspecial		int,
	@idUsuario				int,
	@idAlmacenDestino		int,
	@descripcion			varchar(500)

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se registro el pedido interno correctamente,',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@tran_name				varchar(32) = 'guardaPedidoInterno',
						@tran_count				int = @@trancount,
						@tran_scope				bit = 0,		
						@idAlmacenAtiende		int = 0,
						@nombreAlmacenAtiende	varchar(100),
						@fecha					datetime,
						@idUbicacionDestino		int = 0,
						@ini					int = 0,
						@fin					int = 0,
						@ini_					int = 0,
						@fin_					int = 0

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
				SELECT Producto.cantidad.value('.','NVARCHAR(200)') AS cantidad FROM @xml.nodes('//cantidad') as Producto(cantidad) 

				insert into #idProductos (idProducto)
				SELECT Producto.idProducto.value('.','NVARCHAR(200)') AS idProducto FROM @xml.nodes('//idProducto') as Producto(idProducto)

				
				if ( @idPedidoEspecial = 0 )  -- si es nuevo
					begin

						-- universo de pedidos
						select	p.idProducto, coalesce( sum(c.cantidad), 0 )  as cantidad
						into	#pedidos
						from	#cantidades c
								inner join #idProductos p_
									on c.id = p_.id
								inner join Productos p
									on p.idProducto = p_.idProducto
						group by p.idProducto
							

						-- universo de productos por idAlmacen
						select	id.idProducto, u.idAlmacen, coalesce( (sum(id.cantidad)), 0) as cantidad
						into	#productos
						from	InventarioDetalle id
									inner join Ubicacion u
										on id.idUbicacion =u.idUbicacion
									inner join Productos p
										on p.idProducto = id.idProducto
						where id.idproducto in (select distinct idProducto from #pedidos )
						group by id.idProducto, u.idAlmacen, p.descripcion, p.precioIndividual,p.precioMenudeo
						order by idProducto
				
				
				
						-- buscamos quien tiene inventario para surtirlo
						-- eliminamos lo que no son tienen existencias para surtir el pedido 
						delete	#productos
						where	idAlmacen in	(
													select	pro.idAlmacen
													from	#pedidos p 
																inner join #productos pro 
																	on p.idProducto = pro.idProducto 
													where	pro.cantidad-p.cantidad <0
												)
										

						-- eliminamos las que no contienen todos los productos del pedido
						select @ini = min(idAlmacen) from #productos
						select @fin = max(idAlmacen) from #productos

				
						while ( @ini <= @fin )
							begin
						
								select '#productos'+CAST(@ini as varchar(10)) as _vuelta , * into #tempProductos from #productos  where idAlmacen = @ini
					
								-- si no cuenta con todas las existencias 
								if exists ( select * from #pedidos p left join #tempProductos t on p.idProducto = t.idProducto where t.idProducto is null )
									begin 
										--select 'elimina '+  CAST(@ini as varchar(10))  --para debuguear
										delete from #productos where idAlmacen = @ini
									end

								select @ini = min(idAlmacen) from #productos where idAlmacen > @ini
								drop table #tempProductos

							end

						if not exists ( select 1 from #productos )
							begin
								select @mensaje = 'No hay Almacenes que puedan atender el pedido.'
								raiserror (@mensaje, 11, -1)
							end

						-- seleccionamos el almacen que atendera el pedido (aquel que tenga mas existencias en total)
						select	top 1 @idAlmacenAtiende = idAlmacen
						from	#productos 
						group by idAlmacen
						order by SUM(cantidad) desc
										
										
						-- insertamos los registros del pedido interno
						insert	into PedidosInternos (idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta,observacion,idTipoPedidoInterno,descripcion)
						select	@idAlmacenAtiende, @idAlmacenDestino, @idUsuario, 1 as IdEstatusPedidoInterno, @fecha as fechaAlta, null as observacion, 2 as idTipoPedidoInterno, @descripcion as descripcion

						select @idPedidoEspecial = max(idPedidoInterno) from PedidosInternos where idTipoPedidoInterno = 2
						
						insert	into PedidosInternosDetalle (idPedidoInterno,idProducto, cantidad,fechaAlta,cantidadAtendida,cantidadAceptada,cantidadRechazada) 
						select	@idPedidoEspecial as idPedidoInterno, idProducto, cantidad, @fecha as fechaAlta, 0 as cantidadAtendida, 0 as cantidadAceptada, 0 as cantidadRechazada
						from	#pedidos

						
						-- se insertan los movimientos de los productos al almacen destino
						insert into MovimientosDeMercancia (idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoInterno,idUsuario,fechaAlta,idEstatusPedidoInterno,observaciones,cantidadAtendida)
						select	@idAlmacenAtiende as idAlmacenOrigen, @idAlmacenDestino as idAlmacenDestino, idProducto, cantidad, @idPedidoEspecial as  idPedidoInterno,
								@idUsuario as idUsuario, @fecha as fechaAlta, 1 as idEstatusPedidoInterno, null as observaciones, cantidad as cantidadAtendida
						from	#pedidos
						

						-- calculamos las existencias del inventario despues de la venta
						select idProducto, sum(cantidad) as cantidad into #totProductos from #pedidos group by idProducto

						select	distinct idInventarioDetalle, id.idProducto, id.cantidad, fechaAlta, id.idUbicacion, 
								fechaActualizacion, cast(0 as int) as cantidadDescontada, 
								cast(0 as int) as cantidadFinal, a.idAlmacen
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
						where	a.idAlmacen = @idAlmacenAtiende
							and	id.cantidad > 0


						if not exists ( select 1 from #tempExistencias)
							begin
								select @mensaje = 'No se realizo el pedido, no se cuenta con suficientes existencias en el inventario.'
								raiserror (@mensaje, 11, -1)
							end


						-- se calcula de que ubicaciones se van a descontar los productos
						select @ini_ = min(idProducto), @fin_= max(idProducto) from #totProductos

						while ( @ini_ <= @fin_ )
							begin
								declare	@cantidadProductos as int = 0
								select	@cantidadProductos = cantidad from #totProductos where idProducto = @ini_

								while ( @cantidadProductos > 0 )
									begin
										declare @cantidadDest as int = 0, @idInventarioDetalle as int = 0
										select	@cantidadDest = coalesce(max(cantidad), 0) from #tempExistencias where idProducto = @ini_ and cantidadDescontada = 0
										select	@idInventarioDetalle = idInventarioDetalle from #tempExistencias where idProducto = @ini_ and cantidadDescontada = 0 and cantidad = @cantidadDest

										-- si ya no hay ubicaciones con existencias a descontar
										if ( @cantidadDest = 0 )
											begin
												update  #tempExistencias set cantidadDescontada = (select cantidad from #totProductos where idProducto = @ini_)
												where	idProducto = @ini_
												select @cantidadProductos = 0
											end
										else
											begin
												if ( @cantidadDest >= @cantidadProductos )
													begin 
														update #tempExistencias set cantidadDescontada = @cantidadProductos 
														where idProducto = @ini_ and idInventarioDetalle = @idInventarioDetalle
														select @cantidadProductos = 0 
													end
												else
													begin
														update	#tempExistencias set cantidadDescontada = @cantidadDest
														where	 idProducto = @ini_ and idInventarioDetalle = @idInventarioDetalle
														select @cantidadProductos = @cantidadProductos - @cantidadDest						
													end
											end 
									end

								select @ini_ = min(idProducto) from #totProductos where idProducto > @ini_

							end  -- while ( @ini_ <= @fin_ )

							
							update	#tempExistencias
							set		cantidadFinal = cantidad - cantidadDescontada

							-- si el inventario de los productos vendidos queda negativo se paso de productos = rollback
							if  exists	( select 1 from #tempExistencias where cantidadFinal < 0 )
							begin
								select @mensaje = 'No se realizo la venta, no se cuenta con suficientes existencias en el inventario.'
								raiserror (@mensaje, 11, -1)
							end

							-- se descuentan existencias de almacen origen
							--se actualiza inventario_general_log
							INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
							select a.idProducto,sum(a.cantidad),b.cantidad-sum(a.cantidad),dbo.FechaActual(),17 
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
							insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
							select	idUbicacion, idProducto, cantidadDescontada, cantidadFinal, cast(17 as int) as idTipoMovInventario,
									@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
							from	#tempExistencias
							where	cantidadDescontada > 0


							-- se agregann existencias a almacen destino en la ubicacion sin ubicacion
							if not exists	(
												select	1
												from	Ubicacion 
												where	idAlmacen = @idAlmacenDestino
													and	idPasillo = 0
													and idRaq = 0
													and idPiso = 0
											)
								begin
									insert into Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) values (@idAlmacenDestino, 0, 0, 0)
								end

							select	@idUbicacionDestino = idUbicacion 
							from	Ubicacion 
							where	idAlmacen = @idAlmacenDestino
								and	idPasillo = 0
								and idRaq = 0
								and idPiso = 0

								
							select	idProducto , @idUbicacionDestino as idUbicacionDestino, cantidad
							into	#ubicacionesDestino
							from	#pedidos
	

							-- si no existen las ubicaciones de los productos en almacen destino
							if  exists	(
											select	ud.idProducto as idProductoDestino
											from	#ubicacionesDestino ud
														left join InventarioDetalle id
															on	id.idProducto = ud.idProducto 
															and	id.idUbicacion = ud.idUbicacionDestino
											where	id.idProducto is null 
										)
								begin
									insert into InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
									select	ud.idProducto as idProductoDestino, 0 as cantidad, @fecha, @idUbicacionDestino, @fecha
									from	#ubicacionesDestino ud
												left join InventarioDetalle id
													on	id.idProducto = ud.idProducto 
													and	id.idUbicacion = ud.idUbicacionDestino
									where	id.idProducto is null 
								end


							--se actualiza inventario_general_log
							INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
							select a.idProducto,sum(a.cantidad),b.cantidad + sum(a.cantidad),dbo.FechaActual(),18 
							from #totProductos a
							join InventarioGeneral b on a.idProducto=b.idProducto
							group by a.idProducto,b.cantidad
					

							-- se actualiza inventario detalle
							update	InventarioDetalle
							set		cantidad = a.cantidadFinal,
									fechaActualizacion = dbo.FechaActual()
							from	(
										select	p.idProducto, ( p.cantidad + id.cantidad ) as cantidadFinal, id.idUbicacion
										from	#ubicacionesDestino p
													inner join InventarioDetalle id 
														on id.idProducto = p.idProducto 
										where	id.idUbicacion = @idUbicacionDestino
									)A
							where	InventarioDetalle.idProducto = a.idProducto
								and	InventarioDetalle.idUbicacion = a.idUbicacion

							-- se actualiza el inventario general
							update	InventarioGeneral
							set		InventarioGeneral.cantidad = InventarioGeneral.cantidad + A.cantidad,
									fechaUltimaActualizacion = dbo.FechaActual()
							from	(
										select idProducto, cantidad from #totProductos
									)A
							where InventarioGeneral.idProducto = A.idProducto


							 --se inserta el InventarioDetalleLog
							insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
							select	@idUbicacionDestino, t.idProducto, cantidadDescontada, id.cantidad, cast(18 as int) as idTipoMovInventario,
									@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
							from	#tempExistencias t
										inner join InventarioDetalle id
										on t.idProducto = id.idProducto										
							where	cantidadDescontada > 0
								and id.idUbicacion = @idUbicacionDestino
							
						
						select @nombreAlmacenAtiende = descripcion from Almacenes where idAlmacen = @idAlmacenAtiende
						select @mensaje = @mensaje + ' Sera atendido por: ' + @nombreAlmacenAtiende

						drop table #pedidos
						drop table #productos

					end -- si es nuevo
				else
					begin

						print 'edicion'

					end

				



				begin -- commit de transaccion
					if @tran_count = 0
						begin -- si la transacción se inició dentro de este ámbito
							commit tran @tran_name
							select @tran_scope = 0
						end -- si la transacción se inició dentro de este ámbito
				end -- commit de transaccion
					
				--drop table #Ventas
				--drop table #VentasDetalle
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

		end -- reporte de estatus

	end  -- principal
go

grant exec on SP_GUARDA_PEDIDO_ESPECIAL to public
go



