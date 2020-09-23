use [DB_A57E86_lluviadesarrollo]
go

-- se crea procedimiento SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL
if exists (select * from sysobjects where name like 'SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/09/10
Objetivo		acepta o guarda el pedido especial
status			200 = ok
				-1	= error
*/

create proc SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL

	@xml					AS XML, 
	@idPedidoEspecial		int,
	@idUsuario				int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se actualizo el pedido especial correctamente,',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@tran_name				varchar(32) = 'actualizaPedidoEspecial',
						@tran_count				int = @@trancount,
						@tran_scope				bit = 0,		
						@idAlmacenOrigen		int = 0,		
						@idUbicacionOrigen		int = 0,
						@idUbicacionDestino		int = 0,
						@fecha					datetime

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
					#idPedidoInternoDetalles 
						(  
							id						int identity(1,1),
							idPedidoInternoDetalle	int
						)

				create table 
					#cantidadesAtendidas
						(  
							id						int identity(1,1),
							cantidadAtendida		int
						)

				create table 
					#cantidadesAceptadas
						(  
							id						int identity(1,1),
							cantidadAceptada		int
						)


				create table 
					#cantidadesRechazadas
						(  
							id						int identity(1,1),
							cantidadRechazada		int
						)

				create table 
					#observaciones
						(  
							id						int identity(1,1),
							observacion				varchar(300)
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

				insert into #idPedidoInternoDetalles (idPedidoInternoDetalle)
				SELECT Producto.idPedidoInternoDetalle.value('.','NVARCHAR(200)') AS idPedidoInternoDetalle FROM @xml.nodes('//idPedidoInternoDetalle') as Producto(idPedidoInternoDetalle)
				
				insert into #cantidadesAtendidas (cantidadAtendida)
				SELECT Producto.cantidadAtendida.value('.','NVARCHAR(200)') AS cantidadAtendida FROM @xml.nodes('//cantidadAtendida') as Producto(cantidadAtendida)

				insert into #cantidadesAceptadas (cantidadAceptada)
				SELECT Producto.cantidadAceptada.value('.','NVARCHAR(200)') AS cantidadAceptada FROM @xml.nodes('//cantidadAceptada') as Producto(cantidadAceptada)

				insert into #cantidadesRechazadas (cantidadRechazada)
				SELECT Producto.cantidadRechazada.value('.','NVARCHAR(200)') AS cantidadRechazada FROM @xml.nodes('//cantidadRechazada') as Producto(cantidadRechazada)

				insert into #observaciones (observacion)
				SELECT Producto.observacion.value('.','NVARCHAR(200)') AS observacion FROM @xml.nodes('//observacion') as Producto(observacion)
				
				select	@idAlmacenOrigen = idAlmacenOrigen ,
						@idUbicacionDestino = idAlmacenDestino
				from	PedidosInternos 
				where	idPedidoInterno = @idPedidoEspecial
				
				-- universo de pedidos especiales detalle
				select	@idPedidoEspecial as idPedidoInterno, pe.idPedidoInternoDetalle, p.idProducto, c.cantidad, cat.cantidadAtendida, cac.cantidadAceptada, car.cantidadRechazada,
						case
							when o.observacion = '0' then null
							else o.observacion
						end as observacion
				into	#pedido
				from	#idProductos p 
							inner join #cantidades c
								on p.id = c.id
							inner join #idPedidoInternoDetalles pe
								on pe.id = c.id
							inner join #cantidadesAceptadas cac
								on cac.id = c.id
							inner join #cantidadesAtendidas cat
								on cat.id = c.id
							inner join #cantidadesRechazadas car
								on car.id = c.id
							inner join #observaciones o
								on o.id = p.id
								
				--select * from #pedido

				begin  -- validaciones

					if not exists ( select 1 from PedidosInternos where idPedidoInterno = @idPedidoEspecial and IdEstatusPedidoInterno = 2 )
						begin
							select @mensaje = 'No es posible aceptar o rechazar el pedido si no ha sido atendido.'
							raiserror (@mensaje, 11, -1)
						end

					if exists	(
									select	1
									from	#pedido
									where	cantidadAtendida <> (cantidadAceptada+cantidadRechazada)
								)
						begin
							select @mensaje = 'No estan aceptados/rechazados todos los productos atendidos.'
							raiserror (@mensaje, 11, -1)
						end


				end  -- validaciones



				-- actualizamos estatus de PedidosInternos y PedidosInternosLog
				-- si se rechazo toda
				if	( 
						(select sum(cantidadAtendida) from #pedido ) =
						(select sum(cantidadRechazada) from #pedido ) 
					)
					begin
						update	PedidosInternos
						set		IdEstatusPedidoInterno = 3 --Pedido Rechazado
						where	idPedidoInterno = @idPedidoEspecial

						insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta)
						select	@idPedidoEspecial, idAlmacenDestino, idAlmacenOrigen, @idUsuario, cast(3 as int) as IdEstatusPedidoInterno, @fecha
						from	PedidosInternos
						where	idPedidoInterno = @idPedidoEspecial
					end
				else
					begin
						update	PedidosInternos
						set		IdEstatusPedidoInterno = 4 --Pedido Finalizado - puede tener productos rechazados
						where	idPedidoInterno = @idPedidoEspecial

						insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta)
						select	@idPedidoEspecial, idAlmacenDestino, idAlmacenOrigen, @idUsuario, cast(4 as int) as IdEstatusPedidoInterno, @fecha
						from	PedidosInternos
						where	idPedidoInterno = @idPedidoEspecial
					end

				
				--PedidosInternosDetalle
				update	PedidosInternosDetalle
				set		PedidosInternosDetalle.cantidadAceptada = a.cantidadAceptada,
						PedidosInternosDetalle.cantidadRechazada = a.cantidadRechazada						
				from	(
							select idPedidoInternoDetalle, idProducto, cantidadAtendida, cantidadAceptada, cantidadRechazada, observacion from #pedido
						)A
				where	PedidosInternosDetalle.idPedidoInternoDetalle = a.idPedidoInternoDetalle
				
				
				if exists ( select 1 from PedidosInternosDetalle where idPedidoInterno = @idPedidoEspecial and cantidadRechazada < 0 )
					begin
							select @mensaje = 'No se pueden aceptar/rechazar mas productos de los que fueron atendidos.'
							raiserror (@mensaje, 11, -1)
					end


				--MovimientosDeMercancia
				-- las aceptadas
				insert into MovimientosDeMercancia (idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoInterno,idUsuario,fechaAlta,idEstatusPedidoInterno,observaciones,cantidadAtendida)
				select	idAlmacenDestino, idAlmacenOrigen,  p.idProducto, p.cantidadAceptada, p.idPedidoInterno , @idUsuario, @fecha, cast(4 as int) as IdEstatusPedidoInterno, 
						p.observacion, p.cantidadAtendida
				from	PedidosInternos pi_
							inner join #pedido p
								on p.idPedidoInterno = pi_.idPedidoInterno
				where	pi_.idPedidoInterno = @idPedidoEspecial
					and	cantidadAceptada > 0

				-- las rechazadas 
				insert into MovimientosDeMercancia (idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoInterno,idUsuario,fechaAlta,idEstatusPedidoInterno,observaciones,cantidadAtendida)
				select	idAlmacenDestino,idAlmacenOrigen , p.idProducto, p.cantidadRechazada, p.idPedidoInterno , @idUsuario, @fecha, cast(3 as int) as IdEstatusPedidoInterno, 
						p.observacion, p.cantidadAtendida
				from	PedidosInternos pi_
							inner join #pedido p
								on p.idPedidoInterno = pi_.idPedidoInterno
				where	pi_.idPedidoInterno = @idPedidoEspecial
					and	cantidadRechazada > 0



				-- se agregann existencias a almacen origen en la ubicacion sin ubicacion
				if not exists	(
									select	1
									from	Ubicacion 
									where	idAlmacen = @idAlmacenOrigen
										and	idPasillo = 0
										and idRaq = 0
										and idPiso = 0
								)
					begin
						insert into Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) values (@idAlmacenOrigen, 0, 0, 0)
					end

				select	@idUbicacionOrigen = idUbicacion 
				from	Ubicacion 
				where	idAlmacen = @idAlmacenOrigen
					and	idPasillo = 0
					and idRaq = 0
					and idPiso = 0

								
				select	idProducto , @idUbicacionOrigen as idUbicacionOrigen, cantidadAceptada, cantidadRechazada
				into	#ubicacionesOrigen
				from	#pedido
	

				-- si no existen las ubicaciones de los productos en almacen origen
				if  exists	(
								select	ud.idProducto as idProductoDestino
								from	#ubicacionesOrigen ud
											left join InventarioDetalle id
												on	id.idProducto = ud.idProducto 
												and	id.idUbicacion = ud.idUbicacionOrigen
								where	id.idProducto is null 
							)
					begin
						insert into InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
						select	uo.idProducto as idProductoDestino, 0 as cantidad, @fecha, @idUbicacionOrigen, @fecha
						from	#ubicacionesOrigen uo
									left join InventarioDetalle id
										on	id.idProducto = uo.idProducto 
										and	id.idUbicacion = uo.idUbicacionOrigen
						where	id.idProducto is null 
					end


				----se actualiza inventario_general_log aceptadas
				--INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
				--select	a.idProducto,sum(a.cantidadAceptada),b.cantidad + sum(a.cantidadAceptada),dbo.FechaActual(),18 
				--from	#pedido a
				--			join InventarioGeneral b 
				--				on a.idProducto=b.idProducto
				--where	a.cantidadAceptada > 0
				--group by a.idProducto,b.cantidad
					
				----se actualiza inventario_general_log rechazadas
				--INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
				--select	a.idProducto,sum(a.cantidadRechazada),b.cantidad + sum(a.cantidadRechazada),dbo.FechaActual(),18 
				--from	#pedido a
				--			join InventarioGeneral b 
				--				on a.idProducto=b.idProducto
				--where	a.cantidadRechazada > 0
				--group by a.idProducto,b.cantidad

				---- se actualiza el inventario general 
				--update	InventarioGeneral
				--set		InventarioGeneral.cantidad = InventarioGeneral.cantidad + A.cantidad,
				--		fechaUltimaActualizacion = dbo.FechaActual()
				--from	(
				--			select idProducto, (cantidadAceptada + cantidadRechazada ) as cantidad from #pedido
				--		)A
				--where InventarioGeneral.idProducto = A.idProducto

				
				-- se actualiza inventario detalle aceptada
				update	InventarioDetalle
				set		cantidad = a.cantidadFinal,
						fechaActualizacion = dbo.FechaActual()
				from	(
							select	p.idProducto, ( p.cantidadAceptada + id.cantidad ) as cantidadFinal, id.idUbicacion
							from	#ubicacionesOrigen p
										inner join InventarioDetalle id 
											on id.idProducto = p.idProducto 
							where	id.idUbicacion = @idUbicacionOrigen
						)A
				where	InventarioDetalle.idProducto = a.idProducto
					and	InventarioDetalle.idUbicacion = a.idUbicacion

				-- se actualiza inventario detalle rechazada
				update	InventarioDetalle
				set		cantidad = a.cantidadFinal,
						fechaActualizacion = dbo.FechaActual()
				from	(
							select	p.idProducto, ( p.cantidadRechazada + id.cantidad ) as cantidadFinal, id.idUbicacion
							from	#ubicacionesOrigen p
										inner join InventarioDetalle id 
											on id.idProducto = p.idProducto 
							where	id.idUbicacion = @idUbicacionDestino
						)A
				where	InventarioDetalle.idProducto = a.idProducto
					and	InventarioDetalle.idUbicacion = a.idUbicacion


				--se inserta el InventarioDetalleLog aceptadas
				insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
				select	@idUbicacionOrigen, t.idProducto, cantidadAceptada, id.cantidad, cast(8 as int) as idTipoMovInventario,
						@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
				from	#pedido t
							inner join InventarioDetalle id
							on t.idProducto = id.idProducto										
				where	t.cantidadAceptada > 0
					and id.idUbicacion = @idUbicacionOrigen
							

				--se inserta el InventarioDetalleLog rechazadas
				insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
				select	@idUbicacionOrigen, t.idProducto, cantidadRechazada, id.cantidad, cast(11 as int) as idTipoMovInventario,
						@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
				from	#pedido t
							inner join InventarioDetalle id
							on t.idProducto = id.idProducto										
				where	t.cantidadRechazada > 0
					and id.idUbicacion = @idUbicacionDestino
													

		
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

grant exec on SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL to public
go



