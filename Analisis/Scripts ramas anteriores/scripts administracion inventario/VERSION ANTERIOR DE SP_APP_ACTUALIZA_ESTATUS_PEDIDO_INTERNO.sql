USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_APP_ACTUALIZA_ESTATUS_PEDIDO_INTERNO]    Script Date: 31/07/2022 11:48:10 p. m. ******/
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
@cantidadAtendida  float = null
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
					dbo.redondear(coalesce(@cantidadAtendida,0))
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
							   end,
				notificado = 0
				WHERE idPedidoInterno =@idPedidoInterno

				--INSERTAMOS LA ACTuALIAzaCION DE LA CANTIDAD QUE SE ATENDIO YA QUE PUEDE SER QUE NO SE ENVIE LA QUE SE SOLICITO
				update PedidosInternosDetalle set cantidadAtendida = dbo.redondear(@cantidadAtendida)
				where idPedidoInterno =@idPedidoInterno
		
					declare 
					@_IdAlmacenDestino int,
					@idProducto int ,
					@cantidadPedidoInterno float ,
					@idUbicacion int,
					@cantidadActualInventario  float,
					@idTipoMonInventario int = 7, -- Salida de mercancia por pedido interno
					@cantidadDespuesDeOperacion float = 0
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
					VALUES ( @idUbcacion,@idProducto,dbo.redondear(@cantidadPedidoInterno) ,dbo.redondear(@cantidadDespuesDeOperacion),@idTipoMonInventario /* Salida pedido */,@idUsuario,dbo.FechaActual(),@idPedidoInterno)

					--ACTUALIZAMOS LA CANTIDAD EN INVENTARIO DETALLE--
					update InventarioDetalle set cantidad = dbo.redondear(@cantidadDespuesDeOperacion)
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

