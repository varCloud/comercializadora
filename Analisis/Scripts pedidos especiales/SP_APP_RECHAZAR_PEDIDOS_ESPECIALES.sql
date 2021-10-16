IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_APP_RECHAZAR_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_APP_RECHAZAR_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/10/13
Objetivo		Rechazar los pedidos especiales que pertecen al almacen al que lo solicitaron
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_APP_RECHAZAR_PEDIDOS_ESPECIALES]
@idPedidoEspecial int ,
@idUsuario int,
@idAlmacen int,
@observaciones varchar(1000) = null
AS
BEGIN
	BEGIN TRY			


			declare  	@tran_name varchar(32) = 'RECHAZA_PEDIDO_ESPECIAL',
						@tran_count int = @@trancount,
						@tran_scope bit = 0
						
			
			if not exists(select 1 from PedidosEspeciales where idPedidoEspecial=@idPedidoEspecial)
				RAISERROR('El pedido especial no existe', 15, 217)

            if not exists(select 1 from PedidosEspeciales where idPedidoEspecial=@idPedidoEspecial and idEstatusPedidoEspecial=1)
				RAISERROR('El pedido especial se encuentra en otro estatus', 15, 217)

            --consultamos los pedidos especiales detalle del almacen
			select * 
			INTO #PedidosEspecialesDetalleRechazados
			from PedidosEspecialesDetalle where idAlmacenDestino = @idAlmacen and idEstatusPedidoEspecialDetalle = 1
			and idpedidoespecial=@idPedidoEspecial
			

			if not exists(select 1 from #PedidosEspecialesDetalleRechazados)
			begin
				RAISERROR('No existen productos para rechazar', 15, 217)				
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
					@idEstatusPedidoEspecialDetalle int=3 --rechazado
				END	

				select @fechaActual=dbo.FechaActual()

	
				--insertamos en la tabla movimientos de mercancia	
				INSERT INTO PedidosEspecialesMovimientosDeMercancia(idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoEspecial,idUsuario,fechaAlta,idEstatusPedidoEspecialDetalle,observaciones,cantidadAtendida,idUbicacion)
				select @idAlmacen,idAlmacenOrigen,idProducto,cantidad,@idPedidoEspecial,@idUsuario,@fechaActual,@idEstatusPedidoEspecialDetalle,@observaciones,0,0
				from #PedidosEspecialesDetalleRechazados;


				--actualizamos en la tabla PedidosEspecialesDetalle
				update d set cantidadAtendida=0,cantidadRechazada=d.cantidad,idEstatusPedidoEspecialDetalle=@idEstatusPedidoEspecialDetalle,notificado=0,observaciones=@observaciones
				from PedidosEspecialesDetalle d
				join #PedidosEspecialesDetalleRechazados p on d.idPedidoEspecialDetalle=p.idPedidoEspecialDetalle

				
				
			--VALIDAMOS SI LA TRANSACCION SE GENERO AQUI , AQUIMISMO SE HACE EL COMMIT	
		    if @tran_count = 0	
			begin -- si la transacción se inició dentro de este ámbito
						
				commit tran @tran_name
				select @tran_scope = 0
						
			end -- si la transacción se inició dentro de este ámbito

			select 200 Estatus , 'OK' Mensaje 
			DROP TABLE #ProductosRecibidos

	END TRY
	BEGIN CATCH
		SELECT -1 Estatus, error_message() Mensaje,error_line() Errorline
		
		if @tran_scope = 1
			rollback tran @tran_name

	END CATCH
	
END

