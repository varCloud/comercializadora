
/****** Object:  StoredProcedure [dbo].[SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL]    Script Date: 01/09/2022 10:27:53 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/10/13
Objetivo		Aprobar los pedidos especiales
status			200 = ok
				-1	= error
"Se quedan en la ubicacion 1004en proceso de produccioón agranel" despues se pueden mover mediante la HH o ajuste de inventario

Modificado		2026/09/09
Objetivo		Fix inventario "fantasma": al Rechazo Completo (estatus 4) la mercancia
				reservada en la ubicacion tecnica (1004) nunca regresaba al anaquel de
				origen; al Rechazo Parcial (estatus 5) la diferencia no atendida nunca
				se descontaba (no existia concepto de merma en este proceso). Confirmado
				con el cliente: rechazo completo = regresa al almacen de origen; rechazo
				parcial = la diferencia es merma (dentro de este asistente de consumo de
				MPL no hay caso de "sobrante que regresa" en rechazo parcial).

Modificado		2026/09/12
Objetivo		Regresion introducida por el fix del 2026/09/09: al desplegarse el script
				completo se reactivo un bloque heredado (de cuando este SP era para
				"pedidos especiales") que movia la cantidad de consumo normal aceptado
				(estatus 3) y la porcion aceptada de un rechazo parcial (estatus 5) a la
				ubicacion de resguardo (1000). Verificado contra InventarioDetalleLog de
				produccion que ese paso NUNCA se ejecuto entre oct-2022 y 2026 (el SP
				realmente desplegado durante esos ~4 anios terminaba en el descuento de
				1004). Confirmado con el cliente (caso LEES 70, 200kg aceptados de 200):
				el consumo normal debe terminar en el descuento de 1004, sin pasar por
				resguardo. Se elimina ese bloque; la logica de rechazo completo/parcial
				del fix anterior no se modifica.

*/

alter proc [dbo].SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL
@xmlProductos xml,
@idUsuario int,
@idAlmacen int
AS
BEGIN
	BEGIN TRY



			IF OBJECT_ID('tempdb..#ProductosRecibidos') IS NOT NULL
				DROP TABLE #ProductosRecibidos
			create table #ProductosRecibidos
			(
						indice int identity(1,1),
						idProcesoProduccionAgranel int,
						idProducto int ,
						idUbicacion	int,
						cantidadAtendida	float ,
						observaciones varchar(500),
						idEstatusProduccionAgranel int,
			)

			INSERT INTO  #ProductosRecibidos
						(
						idProcesoProduccionAgranel
						,idProducto
						,idUbicacion
						,cantidadAtendida
						,observaciones
						--,idEstatusProduccionAgranel
						)
			SELECT
					P.value('idProcesoProduccionAgranel[1]', 'INT') AS idProcesoProduccionAgranel,
					P.value('idProducto[1]', 'INT') AS idProducto,
					P.value('idUbicacion[1]', 'INT') AS idUbicacion,
					P.value('cantidadAtendida[1]', 'float') AS cantidadAtendida,
					P.value('observaciones[1]', 'varchar(500)') AS observaciones
					-- P.value('idEstatusProduccionAgranel[1]', 'int') AS idEstatusProduccionAgranel
			FROM  @xmlProductos.nodes('//ArrayOfProductosProduccionAgranel/ProductosProduccionAgranel') AS x(P)


			declare  	@tran_name varchar(32) = 'PRODUCTOS_PRODCUCCION_AGRANEL',
						@tran_count int = @@trancount,
						@tran_scope bit = 0



			if exists(select 1 from #ProductosRecibidos PR  join   ProcesoProduccionAgranel PPA
				on PR.idProcesoProduccionAgranel = PPA.idProcesoProduccionAgranel where pr.cantidadAtendida > PPA.cantidad)
			begin
				RAISERROR('La cantidad atendida no puede ser mayor que la cantidad solicitada', 15, 217)
			end

			IF EXISTS (select 1 from #ProductosRecibidos PR  join ProcesoProduccionAgranel PPA on PR.idProcesoProduccionAgranel = PPA.idProcesoProduccionAgranel
						where PPA.idEstatusProduccionAgranel in (3,4,5))
			BEGIN
				RAISERROR('Existen uno o mas productos que ya fueron aceptados', 15, 217)
			END


			/* 1 Ninguno | 2	Alta | 3 Procesado Correctamente | 4.-Rechazo Completo | 5.-Rechazo Parcial */
			--asignamos el estatus que le corresponde para validar que vengan en estatus correcto
			UPDATE PR
			SET idEstatusProduccionAgranel =
				case when COALESCE(pr.cantidadAtendida,0)=0 then 4
				     when dbo.redondear(COALESCE(pr.cantidadAtendida,0))=PPA.cantidad then 3
					 when dbo.redondear(COALESCE(pr.cantidadAtendida,0)) < PPA.cantidad then 5
					 when dbo.redondear(COALESCE(pr.cantidadAtendida,0)) > PPA.cantidad then 6 /*ERROR*/
					 end
			from #ProductosRecibidos PR  join   ProcesoProduccionAgranel PPA on PR.idProcesoProduccionAgranel = PPA.idProcesoProduccionAgranel


			if exists(select 1 from #ProductosRecibidos PR  where idEstatusProduccionAgranel=6)
			begin
				RAISERROR('El pedido contiene productos en estatus con cantidades atendidas mayores a las solicitadas', 15, 217)
			end

			if @tran_count = 0
				begin tran @tran_name
			else
				save tran @tran_name

			select @tran_scope = 1

			--BEGIN TRAN
				--OBTENEMOS LA FECHA MAS QUE NADA LA HORA ACTUAL DE NUESTRA ZONA HORARIA

				BEGIN-- DECLARACIONES

					DECLARE
					@fechaActual date,
					@tipoMovimientoInventarioSalida int=30,
					@tipoMovimientoSalidaCancelacion int=34,
					@tipoMovimientoEntradaCancelacion int=35,
					@tipoMovimientoMerma int=36,
					@indiceActual int=1,
					@indiceMax int,
					@idProducto bigint,
					@idUbicacion bigint,
					@cantidadAtendida float,
					@idProcesoProduccionAgranel bigint,
					@idEstatusProduccionAgranel int,
					@observaciones varchar(500),
					@cantidadActualSistema float,
					@cantidadDespuesOp float,
					@idUbicacionOrigenCancelada int,
					@cantidadRestanteProceso float


				END

				select @fechaActual=dbo.FechaActual()

				select @indiceMax=max(indice) ,@indiceActual = MIN(indice) from #ProductosRecibidos;

				while @indiceActual<=(select max(indice) from #ProductosRecibidos)
				begin
					--select * from CatEstatusProcesoAgranel

					SELECT
						 @idProducto =idProducto
						,@cantidadAtendida =cantidadAtendida
						,@idUbicacion = idUbicacion
						,@idProcesoProduccionAgranel = idProcesoProduccionAgranel
						,@idEstatusProduccionAgranel=idEstatusProduccionAgranel
						,@observaciones=observaciones
					FROM #ProductosRecibidos
					WHERE indice = @indiceActual;

					--afectamos el inventario
					IF(@idEstatusProduccionAgranel in (4,3,5) and @cantidadAtendida > 0)
					BEGIN

						--validamos que exista la cantidad atendida en el inventario
						IF NOT EXISTS(select 1 from InventarioDetalle where idProducto=@idProducto and idUbicacion=@idUbicacion and cantidad>=@cantidadAtendida)
						BEGIN
							RAISERROR('No existe suficiente cantidad en el inventario', 15, 217)
						end

						--descontamos el inventario de la ubicacion
						SELECT @cantidadActualSistema=0,@cantidadDespuesOp=0;

						--OBTENEMOS LA CANTIDAD ACTUAL DEL SISTEMA
						SELECT @cantidadActualSistema = cantidad from InventarioDetalle where idUbicacion = @idUbicacion  and idProducto = @idProducto

						--CALCULAMOS LA CANTIDAD A AJUSTAR
						SET @cantidadDespuesOp = coalesce(@cantidadActualSistema,0) - @cantidadAtendida

						INSERT INTO InventarioDetalleLog(idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta,idPedidoInterno,idProcesoProduccionAgranel)
						SELECT @idUbicacion,@idProducto,dbo.redondear(@cantidadAtendida),dbo.redondear(@cantidadDespuesOp),@tipoMovimientoInventarioSalida,@idUsuario,dbo.FechaActual(),0,0,@idProcesoProduccionAgranel

						UPDATE InventarioDetalle set cantidad=dbo.redondear(@cantidadDespuesOp),fechaActualizacion=dbo.FechaActual()
						where idProducto=@idProducto and idUbicacion=@idUbicacion

						-- === CORRECCION 2026-09-12: se elimina el envio automatico a la ubicacion de resguardo (1000) ===
						-- Este bloque (agregar a Ubicacion resguardo) es codigo heredado de cuando esta SP
						-- se usaba para "pedidos especiales" (2021/2022). Verificado contra InventarioDetalleLog
						-- de produccion: el tipo de movimiento 31 (entrada a resguardo) solo ocurrio 2 veces en
						-- oct-2022 pese a miles de descuentos (tipo 30) desde entonces hasta 2026 — es decir, la
						-- SP realmente desplegada en produccion durante ~4 anios NUNCA ejecuto este paso; el
						-- descuento de la ubicacion tecnica (1004) era la operacion final del consumo normal de
						-- MPL. El fix del 2026-09-09 partio del script (desincronizado del SP real) y al
						-- desplegarlo completo reactivo este bloque por error, mandando a resguardo cualquier
						-- consumo aceptado (estatus 3) o la porcion aceptada de un rechazo parcial (estatus 5).
						-- Confirmado con el cliente (LEES 70, 200kg aceptados de 200): el consumo normal debe
						-- terminar en el descuento de 1004, sin pasar por resguardo.

						UPDATE ProcesoProduccionAgranel
						SET
							cantidadAceptada = @cantidadAtendida,
							cantidadRestante = dbo.redondear(cantidad) - dbo.redondear(@cantidadAtendida),
							fechaUltimaActualizacion = dbo.FechaActual(),
							idEstatusProduccionAgranel = @idEstatusProduccionAgranel
						WHERE idProcesoProduccionAgranel = @idProcesoProduccionAgranel

						print '@idProducto'+ cast(@idProducto as varchar);
						print '@cantidadAtendida'+cast(@cantidadAtendida as varchar);
						print '@idUbicacion'+cast(@idUbicacion as varchar);
						print '@idProcesoProduccionAgranel'+cast(@idProcesoProduccionAgranel as varchar);
						print '@idEstatusProduccionAgranel'+cast(@idEstatusProduccionAgranel as varchar);
						print '@observaciones'+cast(@observaciones as varchar);
						print '@cantidadDespuesOp'+cast(@cantidadDespuesOp as varchar);

						-- UNCOMMENT FOR DEBUG
						--select * from ProcesoProduccionAgranel where idProcesoProduccionAgranel= @idProcesoProduccionAgranel
						--select * from InventarioDetalleLog where idProducto = @idProducto order by fechaAlta desc
						--select * from InventarioDetalle where idProducto = @idProducto order by fechaAlta desc
						--select * from InventarioGeneral where idProducto = @idProducto
					END
					ELSE
					BEGIN
						UPDATE ProcesoProduccionAgranel
						SET
							cantidadAceptada = @cantidadAtendida,
							cantidadRestante = dbo.redondear(cantidad) - dbo.redondear(@cantidadAtendida),
							fechaUltimaActualizacion = dbo.FechaActual(),
							idEstatusProduccionAgranel = @idEstatusProduccionAgranel
						WHERE idProcesoProduccionAgranel = @idProcesoProduccionAgranel
					END

					-- === FIX 2026-09-09: cierre correcto de inventario para Rechazo Completo y Rechazo Parcial ===
					-- Antes de este fix, ninguno de los dos casos siguientes tocaba el inventario:
					-- la mercancia se quedaba fantasma en la ubicacion tecnica de produccion agranel (1004) para siempre.

					-- Rechazo Completo (idEstatusProduccionAgranel=4): se cancela toda la reserva y regresa al anaquel de origen.
					IF (@idEstatusProduccionAgranel = 4)
					BEGIN
						SELECT @cantidadRestanteProceso = NULL, @idUbicacionOrigenCancelada = NULL;

						SELECT @cantidadRestanteProceso = cantidadRestante
						FROM ProcesoProduccionAgranel
						WHERE idProcesoProduccionAgranel = @idProcesoProduccionAgranel;

						IF (COALESCE(@cantidadRestanteProceso,0) > 0)
						BEGIN
							-- la ubicacion de origen es la misma de donde se desconto al dar de alta el consumo de MPL (tipo 30)
							SELECT @idUbicacionOrigenCancelada = MIN(idUbicacion)
							FROM InventarioDetalleLog
							WHERE idProcesoProduccionAgranel = @idProcesoProduccionAgranel AND idTipoMovInventario = @tipoMovimientoInventarioSalida;

							IF (@idUbicacionOrigenCancelada IS NULL)
							BEGIN
								RAISERROR('No se encontro la ubicacion de origen para regresar la mercancia cancelada', 15, 217)
							END

							IF NOT EXISTS(select 1 from InventarioDetalle where idProducto=@idProducto and idUbicacion=@idUbicacion and cantidad>=@cantidadRestanteProceso)
							BEGIN
								RAISERROR('No existe suficiente cantidad en la ubicacion de produccion agranel para cancelar', 15, 217)
							END

							-- descontamos de la ubicacion tecnica de produccion agranel (1004)
							UPDATE InventarioDetalle SET cantidad = dbo.redondear(cantidad - @cantidadRestanteProceso), fechaActualizacion = dbo.FechaActual()
							WHERE idProducto = @idProducto AND idUbicacion = @idUbicacion;

							INSERT INTO InventarioDetalleLog(idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta,idPedidoInterno,idProcesoProduccionAgranel)
							SELECT @idUbicacion,@idProducto,dbo.redondear(@cantidadRestanteProceso),cantidad,@tipoMovimientoSalidaCancelacion,@idUsuario,dbo.FechaActual(),0,0,@idProcesoProduccionAgranel
							FROM InventarioDetalle WHERE idProducto=@idProducto AND idUbicacion=@idUbicacion;

							-- regresamos la mercancia al anaquel de origen
							IF EXISTS (select 1 from InventarioDetalle where idProducto=@idProducto and idUbicacion=@idUbicacionOrigenCancelada)
							BEGIN
								UPDATE InventarioDetalle SET cantidad = dbo.redondear(cantidad + @cantidadRestanteProceso), fechaActualizacion = dbo.FechaActual()
								WHERE idProducto=@idProducto AND idUbicacion=@idUbicacionOrigenCancelada;
							END
							ELSE
							BEGIN
								INSERT INTO InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
								VALUES (@idProducto, dbo.redondear(@cantidadRestanteProceso), dbo.FechaActual(), @idUbicacionOrigenCancelada, dbo.FechaActual());
							END

							INSERT INTO InventarioDetalleLog(idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta,idPedidoInterno,idProcesoProduccionAgranel)
							SELECT @idUbicacionOrigenCancelada,@idProducto,dbo.redondear(@cantidadRestanteProceso),cantidad,@tipoMovimientoEntradaCancelacion,@idUsuario,dbo.FechaActual(),0,0,@idProcesoProduccionAgranel
							FROM InventarioDetalle WHERE idProducto=@idProducto AND idUbicacion=@idUbicacionOrigenCancelada;
						END
					END

					-- Rechazo Parcial (idEstatusProduccionAgranel=5): la diferencia no atendida se da de baja como merma.
					IF (@idEstatusProduccionAgranel = 5)
					BEGIN
						SELECT @cantidadRestanteProceso = NULL;

						SELECT @cantidadRestanteProceso = cantidadRestante
						FROM ProcesoProduccionAgranel
						WHERE idProcesoProduccionAgranel = @idProcesoProduccionAgranel;

						IF (COALESCE(@cantidadRestanteProceso,0) > 0)
						BEGIN
							IF NOT EXISTS(select 1 from InventarioDetalle where idProducto=@idProducto and idUbicacion=@idUbicacion and cantidad>=@cantidadRestanteProceso)
							BEGIN
								RAISERROR('No existe suficiente cantidad en la ubicacion de produccion agranel para dar de baja la merma', 15, 217)
							END

							UPDATE InventarioDetalle SET cantidad = dbo.redondear(cantidad - @cantidadRestanteProceso), fechaActualizacion = dbo.FechaActual()
							WHERE idProducto = @idProducto AND idUbicacion = @idUbicacion;

							INSERT INTO InventarioDetalleLog(idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta,idPedidoInterno,idProcesoProduccionAgranel)
							SELECT @idUbicacion,@idProducto,dbo.redondear(@cantidadRestanteProceso),cantidad,@tipoMovimientoMerma,@idUsuario,dbo.FechaActual(),0,0,@idProcesoProduccionAgranel
							FROM InventarioDetalle WHERE idProducto=@idProducto AND idUbicacion=@idUbicacion;
						END
					END
					-- === FIN FIX 2026-09-09 ===

					select @indiceActual=@indiceActual+1;

				end -- fin del else


			--VALIDAMOS SI LA TRANSACCION SE GENERO AQUI , AQUIMISMO SE HACE EL COMMIT
		    if @tran_count = 0
			begin -- si la transacci�n se inici� dentro de este �mbito

				commit tran @tran_name
				select @tran_scope = 0

			end -- si la transacci�n se inici� dentro de este �mbito

			select 200 Estatus , 'Productos procesados correctamente' Mensaje
			DROP TABLE #ProductosRecibidos

	END TRY
	BEGIN CATCH
		SELECT -1 Estatus, error_message() Mensaje,error_line() Errorline

		if @tran_scope = 1
			rollback tran @tran_name

	END CATCH

END

