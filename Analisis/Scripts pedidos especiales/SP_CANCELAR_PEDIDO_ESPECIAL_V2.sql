--USE [DB_A57E86_lluviadesarrollo]
--GO
/****** Object:  StoredProcedure [dbo].[SP_CANCELAR_PEDIDO_ESPECIAL_V2]    Script Date: 17/09/2021 09:47:45 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- se crea procedimiento SP_CANCELAR_PEDIDO_ESPECIAL_V2
if exists (select * from sysobjects where name like 'SP_CANCELAR_PEDIDO_ESPECIAL_V2' and xtype = 'p' )
	drop proc SP_CANCELAR_PEDIDO_ESPECIAL_V2
go

/*
Autor			Ernesto Aguilar
UsuarioRed		auhl373453
Fecha			2021/12/01
Objetivo		elimina todo el pedido especial
status			200 = ok
				-1	= error
*/

create proc [dbo].[SP_CANCELAR_PEDIDO_ESPECIAL_V2]

	@idPedidoEspecial					int

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status						int = 200,
						@mensaje					varchar(255) = '',
						@error_line					varchar(255) = '',
						@error_procedure			varchar(255) = '',
						@idUsuarioEntrega			int = 0,
						@fecha						datetime,
						@idUbicacionResguardo		int = 0

			end  --declaraciones 

			begin -- principal

				-- validaciones
				if not exists ( select 1 from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial and idEstatusPedidoEspecial in (1,2,3) )
				begin
					if ( @idPedidoEspecial > 0 )
					begin 
						select @mensaje = 'No existe el pedido especial a cancelar.'
						raiserror (@mensaje, 11, -1)					
					end
				end
			
				select @idUsuarioEntrega = idUsuario from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial
				select @fecha = dbo.FechaActual()


				select	@idUbicacionResguardo = u.idUbicacion
				from	PedidosEspecialesDetalle d
							join Almacenes a
								on a.idAlmacen = d.idAlmacenOrigen
							join Ubicacion u
								on u.idAlmacen = a.idAlmacen
				where	d.idPedidoEspecial = @idPedidoEspecial
					and	u.idPasillo = 1000
					and	u.idPiso = 1000
					and	u.idRaq = 1000


				-- si todo bien
				-- ubicaciones para hacer la devolucion de productos
				select	p.idProducto, p.idPedidoEspecialDetalle, p.idUbicacion, p.idAlmacenOrigen as idAlmacenOrigen, u.idUbicacion as idUbicacionRegresar						
				into	#tempUbicacionesDevoluciones_
				from	PedidosEspecialesDetalle p
							left join Ubicacion u
								on u.idAlmacen = p.idAlmacenOrigen
				where	idPedidoEspecial = @idPedidoEspecial
					and	u.idPasillo = 1001
					and	u.idRaq = 1001
					and u.idPiso = 1001
					

				-- si no existe insertamos la ubicacion sin acomodar
				if exists	(
								select 1 from #tempUbicacionesDevoluciones_ where idUbicacionRegresar is null
							)
				begin
					insert into Ubicacion (idAlmacen, idPasillo, idRaq, idPiso)
					select idAlmacenOrigen, 1001,1001,1001 from #tempUbicacionesDevoluciones_
				end
						
				
				-- si no existe registro en inventario detalle para regresar productos
				if exists	(
								select	* 
								from	#tempUbicacionesDevoluciones_ t 
											left join InventarioDetalle id 
												on	id.idProducto = t.idProducto
												and	id.idUbicacion = t.idUbicacionRegresar
								where	id.idProducto is null
							)
				begin
					insert into InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
					select	t.idProducto, cast(0 as float) as cantidad, @fecha as fechaAlta, t.idUbicacionRegresar, @fecha as  fechaActualizacion
					from	#tempUbicacionesDevoluciones_ t 
								left join InventarioDetalle id 
									on	id.idProducto = t.idProducto
									and	id.idUbicacion = t.idUbicacionRegresar
					where	id.idProducto is null
				end



				----------------------------------------------------------------------------------------------------------------------------------
				-- se quitan los productos que se encuentran en ubicacion de resguardo
				----------------------------------------------------------------------------------------------------------------------------------
				insert	into InventarioDetalleLog (idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idVenta, idPedidoEspecial)
				select	distinct @idUbicacionResguardo as idUbicacionResguardo, temp.idProducto, rechazados.cantidadRechazada, actuales.cantidad - rechazados.cantidadRechazada as cantidadActual,
						cast (23 as int ) as idTipoMovInventario, -- 23	Actualizacion de Inventario(salida de mercancia por cancelacion en pedido especial)
						@idUsuarioEntrega as idUsuario, @fecha as fechaAlta, cast(0 as int) as idVenta, @idPedidoEspecial as idPedidoEspecial
				from	#tempUbicacionesDevoluciones_ temp
							join	(
										select	idPedidoEspecialDetalle, idProducto, cantidad as cantidadRechazada
										from	PedidosEspecialesDetalle
										where	idPedidoEspecial = @idPedidoEspecial
									)rechazados on rechazados.idProducto = temp.idProducto
							join InventarioDetalle actuales
								on actuales.idProducto = temp.idProducto and actuales.idUbicacion = @idUbicacionResguardo
					
				update	InventarioDetalle 
				set		InventarioDetalle.cantidad = a.cantidad,
						InventarioDetalle.fechaActualizacion  = @fecha
				from	(
							select	temp.idProducto, actuales.cantidad - rechazados.cantidadRechazada as cantidad, @idUbicacionResguardo as idUbicacionResguardo
							from	#tempUbicacionesDevoluciones_ temp
										join	(
													select	idPedidoEspecialDetalle, idProducto, cantidad as cantidadRechazada
													from	PedidosEspecialesDetalle
													where	idPedidoEspecial = @idPedidoEspecial
												)rechazados on rechazados.idProducto = temp.idProducto
										join InventarioDetalle actuales
											on actuales.idProducto = temp.idProducto and actuales.idUbicacion = @idUbicacionResguardo
						)A
				where	InventarioDetalle.idUbicacion = a.idUbicacionResguardo
					and	InventarioDetalle.idProducto = a.idProducto


				----------------------------------------------------------------------------------------------------------------------------------
				-- se envian los productos a la ubicacion de devolucion de pedido especial
				----------------------------------------------------------------------------------------------------------------------------------
				insert	into InventarioDetalleLog (idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idVenta, idPedidoEspecial)
				select	distinct temp.idUbicacionRegresar, temp.idProducto, rechazados.cantidadRechazada, actuales.cantidad + rechazados.cantidadRechazada as cantidadActual,
						cast (20 as int ) as idTipoMovInventario, -- 20	Actualizacion de Inventario(carga de mercancia por pedido especial rechazado)
						@idUsuarioEntrega as idUsuario, @fecha as fechaAlta, cast(0 as int) as idVenta, @idPedidoEspecial as idPedidoEspecial
				from	#tempUbicacionesDevoluciones_ temp
							join	(
										select	idPedidoEspecialDetalle, idProducto, cantidad as cantidadRechazada
										from	PedidosEspecialesDetalle
										where	idPedidoEspecial = @idPedidoEspecial
									)rechazados on rechazados.idProducto = temp.idProducto
							join InventarioDetalle actuales
								on actuales.idProducto = temp.idProducto and actuales.idUbicacion = temp.idUbicacionRegresar
					
				update	InventarioDetalle 
				set		InventarioDetalle.cantidad = a.cantidad,
						InventarioDetalle.fechaActualizacion  = @fecha
				from	(
							select	temp.idProducto, actuales.cantidad + rechazados.cantidadRechazada as cantidad, temp.idUbicacionRegresar
							from	#tempUbicacionesDevoluciones_ temp
										join	(
													select	idPedidoEspecialDetalle, idProducto, cantidad as cantidadRechazada
													from	PedidosEspecialesDetalle
													where	idPedidoEspecial = @idPedidoEspecial
												)rechazados on rechazados.idProducto = temp.idProducto
										join InventarioDetalle actuales
											on actuales.idProducto = temp.idProducto and actuales.idUbicacion = temp.idUbicacionRegresar
						)A
				where	InventarioDetalle.idUbicacion = a.idUbicacionRegresar
					and	InventarioDetalle.idProducto = a.idProducto


				----------------------------------------------------------------------------------------------------------------------------------
				-- inserta los registros que se regresaron para los movimientos de mercancia
				----------------------------------------------------------------------------------------------------------------------------------
				insert into 
					PedidosEspecialesMovimientosDeMercancia 
						(
							idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoEspecial,idUsuario,fechaAlta,
							idEstatusPedidoEspecialDetalle,observaciones,cantidadAtendida,idUbicacionOrigen,idUbicacionDestino
						)
				select	u.idAlmacen as idAlmacenOrigen, ubicacionDestino.idAlmacenOrigen as idAlmacenDestino, idl.idProducto, rechazados.cantidadRechazada as cantidad, idl.idPedidoEspecial,
						idl.idUsuario, @fecha as fechaAlta, cast(7 as int) as idEstatusPedidoEspecialDetalle, -- 7	Cancelados	
						rechazados.observaciones, rechazados.cantidadAtendida, idl.idUbicacion as idUbicacionOrigen, ubicacionRegresar.idUbicacionRegresar as idUbicacionDestino
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
										select	idPedidoEspecialDetalle, idProducto, cantidad as cantidadRechazada, cantidadAtendida, observaciones
										from	PedidosEspecialesDetalle
										where	idPedidoEspecial = @idPedidoEspecial											
									)rechazados
										on rechazados.idProducto = idl.idProducto
							join	(
										select idProducto, idAlmacenOrigen,	idUbicacionRegresar from #tempUbicacionesDevoluciones_
									)ubicacionRegresar
										on ubicacionRegresar.idProducto = rechazados.idProducto
				where	idl.idPedidoEspecial = @idPedidoEspecial
					and	idl.idTipoMovInventario = 18
					

				-- acualizamos estatus de pedido especial
				update	PedidosEspeciales
				set		idEstatusPedidoEspecial = cast(8 as int)  -- 8	Cancelado
				where	idPedidoEspecial = @idPedidoEspecial

				-- acualizamos estatus de pedido especial detalle
				update	PedidosEspecialesDetalle
				set		idEstatusPedidoEspecialDetalle = 7, -- 7	Cancelados
						cantidadRechazada = cantidadAtendida
				where	idPedidoEspecial = @idPedidoEspecial
				

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
				
			end -- reporte de estatus

	end  -- principal

grant exec on SP_CANCELAR_PEDIDO_ESPECIAL_V2 to public
go