use [DB_A57E86_lluviadesarrollo]
go

-- se crea procedimiento SP_ACEPTA_PEDIDO_ESPECIAL
if exists (select * from sysobjects where name like 'SP_ACEPTA_PEDIDO_ESPECIAL' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_ACEPTA_PEDIDO_ESPECIAL
go

/*

Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2020/09/10
Objetivo		Acepta el pedido especial
status			200 = ok
				-1	= error
*/

create proc SP_ACEPTA_PEDIDO_ESPECIAL

	@idPedidoEspecial		int,
	@idUsuario				int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = 'Se acepto el pedido especial correctamente,',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@tran_name				varchar(32) = 'aceptaPedidoEspecial',
						@tran_count				int = @@trancount,
						@tran_scope				bit = 0,		
						@fecha					datetime,
						@idAlmacenDestino		int = 0,
						@idUbicacionDestino		int= 0,
						@idEstatusPedidoInterno int=4, --pedido finalizado
						@cantidadAtendida		int,			
						@idEstatusPedidoActual	int = 0

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
	
			-- validaciones
			if not exists ( select 1 from PedidosInternos where idPedidoInterno = @idPedidoEspecial )
				begin
					select @mensaje = 'No existe el pedido especial.'
					raiserror (@mensaje, 11, -1)
				end
			
			select  @idEstatusPedidoActual = IdEstatusPedidoInterno from PedidosInternos where idPedidoInterno = @idPedidoEspecial

			if (@idEstatusPedidoActual = 4)
				begin
					select @mensaje = 'El pedido ya ha sido aceptado.'
					raiserror (@mensaje, 11, -1)
				end			

			if (@idEstatusPedidoActual not in (2))
				begin
					select @mensaje = 'No se puede finalizar el pedido sin ser atendido.'
					raiserror (@mensaje, 11, -1)
				end			




			-- universo de productos afectados
			select	idProducto, sum(cantidad) as cantidad
			into	#pedidos
			from	PedidosInternosDetalle 
			where	idPedidoInterno = @idPedidoEspecial
			group by idProducto

			select	@idAlmacenDestino = idAlmacenDestino 
			from	PedidosInternos 
			where	idPedidoInterno = @idPedidoEspecial


			-- se agregan existencias a almacen destino en la ubicacion sin ubicacion
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
			from #pedidos a
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
						select idProducto, cantidad from #pedidos
					)A
			where InventarioGeneral.idProducto = A.idProducto


				--se inserta el InventarioDetalleLog
			insert into InventarioDetalleLog (idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idPedidoInterno)
			select	@idUbicacionDestino, t.idProducto, t.cantidad, id.cantidad, cast(18 as int) as idTipoMovInventario,
					@idUsuario as idUsuario, dbo.FechaActual() as fechaAlta, @idPedidoEspecial as idPedidoInterno
			from	#pedidos t
						inner join InventarioDetalle id
						on t.idProducto = id.idProducto										
			where	t.cantidad > 0
				and id.idUbicacion = @idUbicacionDestino
							





			-- insertamos en movimientos de mercancia
			insert into MovimientosDeMercancia 
				(
					idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoInterno,idUsuario,
					fechaAlta,idEstatusPedidoInterno,observaciones,cantidadAtendida
				)
							
			select	pi_.idAlmacenOrigen, pi_.idAlmacenDestino, pid.idProducto, pid.cantidad, @idPedidoEspecial as idPedidoEspecial, @idUsuario as idUsuario,
					@fecha as fechaAlta, @idEstatusPedidoInterno as idEstatusPedidoInterno, pi_.observacion, pid.cantidadAtendida
			from	PedidosInternos pi_
						inner join PedidosInternosDetalle pid
							on pi_.idPedidoInterno = pid.idPedidoInterno
			where	pi_.idPedidoInterno = @idPedidoEspecial



			-- pedidos internos log
			insert into PedidosInternosLog (idPedidoInterno,idAlmacenOrigen,idAlmacenDestino,idUsuario,IdEstatusPedidoInterno,fechaAlta)
			select	@idPedidoEspecial as idPedidoInterno, idAlmacenOrigen, idAlmacenDestino, @idUsuario as idUsuario, @idEstatusPedidoInterno as IdEstatusPedidoInterno, @fecha as fechaAlta
			from	PedidosInternos pi_


			-- actualizacion de status 				
			update	PedidosInternos 
			set		IdEstatusPedidoInterno = @idEstatusPedidoInterno
			where	idPedidoInterno = @idPedidoEspecial

			
			-- se actualiza la cantidad aceptada en internos detalle 
			update	PedidosInternosDetalle
			set		cantidadAceptada = cantidadAtendida
			where	idPedidoInterno = @idPedidoEspecial



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

grant exec on SP_ACEPTA_PEDIDO_ESPECIAL to public
go



