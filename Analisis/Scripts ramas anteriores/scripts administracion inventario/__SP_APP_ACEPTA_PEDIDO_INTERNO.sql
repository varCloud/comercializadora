--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_APP_ACEPTA_PEDIDO_INTERNO]    Script Date: 30/05/2022 11:30:22 p. m. ******/
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
@cantidadAceptada float,
@observacion varchar(max) = null
AS
BEGIN
	BEGIN TRY
	        
			declare		@idEstatusPedidoInterno			int = 4,  --pedido finalizado
						@cantidadAtendida				float,			
						@idEstatusPedidoActual			int = 0,
						@idUbicacionDestino				int = 0
		
	
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

				declare @cantidadDif float=0
				
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
					dbo.redondear(@cantidadAceptada)
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


				--INSERTAMOS LA ACUTALIACION DEL ESTATUS
				UPDATE PedidosInternos 
				SET IdEstatusPedidoInterno = @idEstatusPedidoInterno,
				observacion = case  when @observacion is null or  @observacion = '' then observacion
								    when @observacion is NOT null AND  @observacion  != '' then @observacion
							   end,
				notificado = 0
				WHERE idPedidoInterno =@idPedidoInterno

				--INSERTAMOS LA ACTuALIAzaCION DE LA CANTIDAD QUE SE ATENDIO YA QUE PUEDE SER QUE NO SE ENVIE LA QUE SE SOLICITO
				update PedidosInternosDetalle set cantidadAceptada = dbo.redondear(@cantidadAceptada),cantidadRechazada=dbo.redondear(@cantidadDif)
				where idPedidoInterno =@idPedidoInterno
		
				declare 
				@_IdAlmacenDestino int,
				@idProducto int ,
				@cantidadPedidoInterno float ,
				@cantidadActualInventario  float,
				@idTipoMovInventario int = 9, -- Carga de mercancia por pedido interno aceptado
				@cantidadDespuesDeOperacion float = 0,
				@idUbicacion int

				--INCREMENTAMOS LA MERCANCIA EN EL INVENTARIO DEL ALAMACEN
					
				select  @idProducto = idProducto ,  @cantidadPedidoInterno= isnull(cantidad,0)  
				from PedidosInternosDetalle where idPedidoInterno =@idPedidoInterno

					--SI LA CANTIDAD QUE ATENDIMOS ES DIFERENTE QUE LA QUE SE PIDIO SE SETEA LA CANTIDAD YA QUE CON ESA VARIABLE
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
					

					declare
					@idUbicacionProceso int = 0

					-- quitamos la mercancia del origen 
					select  @idUbicacionProceso  = idUbicacion from Ubicacion where idAlmacen = @idAlmacenOrigen and idPasillo = 1003 and idRaq = 1003 and idPiso = 1003 

					print '@idUbicacion '+cast(@idUbicacion as varchar )
					print '@idUbicacion '+cast(@idUbicacionProceso as varchar )

					update	InventarioDetalle
					set		cantidad = cantidad - (@cantidadAceptada),
						     fechaActualizacion =  dbo.FechaActual()
					where	idUbicacion = @idUbicacionProceso
						and	idProducto = @idProducto
					
					insert into InventarioDetalleLog ( idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idPedidoInterno )
					select	idUbicacion, @idProducto as idProducto, (@cantidadAceptada) as cantidad,cantidad, cast(7 as int) as idTipoMovInventario, @idUsuario as idUsuario, dbo.FechaActual(), @idPedidoInterno
					from	InventarioDetalle id
					where	id.idUbicacion = @idUbicacionProceso
						and	id.idProducto = @idProducto

					
					--CARGA DE MERCANCIA A LA UBICACION SIN ACOMODAR
					--
					IF NOT EXISTS ( SELECT 1 FROM InventarioDetalle where	idUbicacion = @idUbicacion	and	idProducto = @idProducto)
					BEGIN
						 INSERT INTO InventarioDetalle(idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
						 VALUES (@idProducto,0, dbo.FechaActual(),@idUbicacion , dbo.FechaActual())
					END		

					update	InventarioDetalle
					set		cantidad = cantidad + (@cantidadAceptada),
							fechaActualizacion =  dbo.FechaActual()
					where	idUbicacion = @idUbicacion
						and	idProducto = @idProducto
					
					insert into InventarioDetalleLog ( idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idPedidoInterno )
					select	idUbicacion, @idProducto as idProducto, (@cantidadAceptada) as cantidad,cantidad, cast(@idTipoMovInventario as int) as idTipoMovInventario, @idUsuario as idUsuario, dbo.FechaActual(), @idPedidoInterno
					from	InventarioDetalle id
					where	id.idUbicacion = @idUbicacion
						and	id.idProducto = @idProducto
					
				
					
					select @cantidadDif = coalesce(@cantidadDif, 0)

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
						dbo.redondear(@cantidadDif)
						FROM PedidosInternos P join PedidosInternosDetalle PD
						on  P.idPedidoInterno = PD.idPedidoInterno WHERE P.idPedidoInterno = @idPedidoInterno
						
						update	InventarioDetalle
						set		cantidad = cantidad - (@cantidadDif),
							    fechaActualizacion =  dbo.FechaActual()
						where	idUbicacion = @idUbicacionProceso
							and	idProducto = @idProducto
					
						insert into InventarioDetalleLog ( idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idPedidoInterno )
						select	idUbicacion, @idProducto as idProducto, (@cantidadDif) as cantidad,cantidad, cast(25 as int) as idTipoMovInventario, @idUsuario as idUsuario, dbo.FechaActual(), @idPedidoInterno
						from	InventarioDetalle id
						where	id.idUbicacion = @idUbicacionProceso
						and	id.idProducto = @idProducto


						select @idTipoMovInventario=10--Actualizacion de Inventario(carga de mercancia por rechazo de pedido interno)
						select @idUbicacion= idUbicacion FROM Ubicacion WHERE idAlmacen = @idAlmacenOrigen and idPasillo=0 and idRaq=0 and idPiso=0

						select	@idUbicacionDestino = max(idUbicacion) 
						from	Ubicacion 
						where	idAlmacen = @idAlmacenDestino
							and	idPasillo = 0


						IF (coalesce(@idUbicacionDestino,0)=0)
						BEGIN
							 INSERT INTO Ubicacion(idAlmacen,idPasillo,idRaq,idPiso)
							 select @idAlmacenDestino,0,0,0

							 select @idUbicacionDestino=max(idUbicacion) from Ubicacion where idAlmacen=@idAlmacenDestino				

						END			

						SELECT	@cantidadActualInventario = cantidad  
						FROM	InventarioDetalle 
						WHERE	idUbicacion = @idUbicacionDestino 
							and idProducto = @idProducto
					
						-- VALIDAMOS QUE EL RESULTADO QUE SE OBTIENE NO SEA NULL 
						SET @cantidadActualInventario = isnull(@cantidadActualInventario, 0)					
	

						SET @cantidadDespuesDeOperacion =  @cantidadActualInventario + @cantidadDif
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
						VALUES (@idUbicacionDestino,@idProducto,dbo.redondear(@cantidadDif),dbo.redondear(@cantidadDespuesDeOperacion),@idTipoMovInventario,@idUsuario,dbo.FechaActual(),@idPedidoInterno)

						--VALIDACION SI NO EXISTE INVENTARIO DETALLE CON ESE UBICACION LA INSERTAMOS
						IF NOT EXISTS(select 1 from InventarioDetalle where idUbicacion=@idUbicacionDestino and idProducto=@idProducto)
							--INSERTAMOS LA CANTIDAD EN INVENTARIO DETALLE--
							INSERT INTO  InventarioDetalle(idProducto,cantidad,fechaAlta,idUbicacion)
							SELECT @idProducto, dbo.redondear(@cantidadDespuesDeOperacion),@fecha,@idUbicacionDestino
						ELSE
						--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE--
							update InventarioDetalle set cantidad = dbo.redondear(@cantidadDespuesDeOperacion),fechaActualizacion=@fecha
							where idUbicacion = @idUbicacionDestino and idProducto = @idProducto	

					    --INSERTAMOS EL MOVIMIENTO DE INVENTARIO DETALLE LOG
						--insert into InventarioDetalleLog ( idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idPedidoInterno )
						--select	idUbicacion, @idProducto as idProducto, (@cantidadDif) as cantidad, cantidad, cast(10 as int) as idTipoMovInventario, @idUsuario as idUsuario, dbo.FechaActual(), @idPedidoInterno
						--from	InventarioDetalle id
						--where	id.idUbicacion = @idUbicacionDestino
						--	and	id.idProducto = @idProducto


	
				   end		
				   		

			COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN 
		SELECT -1 Estatus ,ERROR_MESSAGE() Mensaje, ERROR_LINE() LineaError 
	END CATCH
		
	     select 200 Estatus , 'Registro actualizado exitosamente' Mensaje
END
