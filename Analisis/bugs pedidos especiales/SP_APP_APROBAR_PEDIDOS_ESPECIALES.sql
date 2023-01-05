GO
/****** Object:  StoredProcedure [dbo].[SP_APP_APROBAR_PEDIDOS_ESPECIALES]    Script Date: 12/27/2022 1:25:22 AM ******/
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
*/

ALTER proc [dbo].[SP_APP_APROBAR_PEDIDOS_ESPECIALES]
@xmlProductos xml,
@idPedidoEspecial int,
@idAlmacenOrigen int, --el usuario que esta logueado en la hand held
@idAlmacenDestino int, -- el usuario que solicito
@idUsuario int
AS
BEGIN
	BEGIN TRY			

			
			IF OBJECT_ID('tempdb..#ProductosRecibidos') IS NOT NULL
				DROP TABLE #ProductosRecibidos
			create table #ProductosRecibidos
			(	
						indice int identity(1,1),
						idPedidoEspecialDetalle int,			
						idProducto int ,
						idUbicacion	int,
						cantidadAtendida	float ,			
						--cantidadRechazada float ,
						observaciones varchar(500),
						idEstatusPedidoEspecialDetalle int,
						idEstatusAsignado	int null,
						idUbicacionDestino	int null
			)

			INSERT INTO  #ProductosRecibidos 
						(
						idPedidoEspecialDetalle		
						,idProducto
						,idUbicacion
						,cantidadAtendida	
						--,cantidadRechazada
						,observaciones
						,idEstatusPedidoEspecialDetalle
						)
			SELECT  
					P.value('idPedidoEspecialDetalle[1]', 'INT') AS idPedidoEspecialDetalle,		
					P.value('idProducto[1]', 'INT') AS idProducto,
					P.value('idUbicacion[1]', 'INT') AS idUbicacion,
					P.value('cantidadAtendida[1]', 'float') AS cantidadAtendida,
					--P.value('cantidadRechazada[1]', 'float') AS cantidadRechazada,
					P.value('observaciones[1]', 'varchar(500)') AS observaciones,
					P.value('idEstatusPedidoEspecialDetalle[1]', 'int') AS idEstatusPedidoEspecialDetalle		
			FROM  @xmlProductos.nodes('//ArrayOfProductosPedidoEspecial/ProductosPedidoEspecial') AS x(P)
			

			declare  	@tran_name varchar(32) = 'APRUEBA_PEDIDO_ESPECIAL',
						@tran_count int = @@trancount,
						@tran_scope bit = 0
			
			if not exists(select 1 from PedidosEspeciales where idPedidoEspecial=@idPedidoEspecial)
				RAISERROR('El pedido especial no existe', 15, 217)

            if not exists(select 1 from PedidosEspeciales where idPedidoEspecial=@idPedidoEspecial and idEstatusPedidoEspecial=1)
				RAISERROR('El pedido especial se encuentra en otro estatus', 15, 217)

			if exists (select 1
				from #ProductosRecibidos PR   left  join   PedidosEspecialesDetalle PD 
				on PR.idPedidoEspecialDetalle = PD.idPedidoEspecialDetalle and PD.idPedidoEspecial=@idPedidoEspecial and pd.idAlmacenDestino=@idAlmacenOrigen
				where PD.idPedidoEspecialDetalle is null)
			begin
				RAISERROR('El pedido contiene productos que no corresponden a lo solicitado', 15, 217)				
			end

			if exists(select 1 from #ProductosRecibidos PR  join   PedidosEspecialesDetalle PD 
				on PR.idPedidoEspecialDetalle = PD.idPedidoEspecialDetalle where pr.cantidadAtendida>pd.cantidad)
			begin
				RAISERROR('La cantidad atendida no puede ser mayor que la cantidad solicitada', 15, 217)				
			end

			if exists(select 1 from #ProductosRecibidos PR  join   PedidosEspecialesDetalle PD 
				on PR.idPedidoEspecialDetalle = PD.idPedidoEspecialDetalle where pd.idestatuspedidoespecialdetalle!=1)
			begin
				RAISERROR('El pedido contiene productos que ya fueron atendidos', 15, 217)				
			end

			--asignamos el estatus que le corresponde para validar que vengan en estatus correcto
			UPDATE PR SET idEstatusAsignado=case when COALESCE(pr.cantidadAtendida,0)=0 then 3 else case when COALESCE(pr.cantidadAtendida,0)=pd.cantidad then 2 else 5 end end 
			from #ProductosRecibidos PR  join   PedidosEspecialesDetalle PD on PR.idPedidoEspecialDetalle = PD.idPedidoEspecialDetalle 

			if exists(select 1 from #ProductosRecibidos PR  where idEstatusPedidoEspecialDetalle!=idEstatusAsignado)
			begin
				RAISERROR('El pedido contiene productos en estatus diferente al que le corrresponde', 15, 217)				
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
					@idUbicacionResguardo int=0,
					@tipoMovimientoInventarioSalida int=17,
					@tipoMovimientoInventarioResguardo int=18,
					@indiceActual int=1,
					@indiceMax int,
					@idProducto bigint,
					@idUbicacion bigint,
					@cantidadAtendida float,
					@idPedidoEspecialDetalle bigint,
					@idEstatusPedidoEspecialDetalle int,
					@observaciones varchar(500),
					@cantidadActualSistema float,
					@cantidadDespuesOp float

			
				END	

				select @fechaActual=dbo.FechaActual()

				select @indiceMax=max(indice) from #ProductosRecibidos;

				while @indiceActual<=(select max(indice) from #ProductosRecibidos)
				begin

					SELECT 
						 @idProducto =idProducto 
						,@cantidadAtendida =cantidadAtendida 
						,@idUbicacion = idUbicacion
						,@idPedidoEspecialDetalle = idPedidoEspecialDetalle,
						@idEstatusPedidoEspecialDetalle=idEstatusPedidoEspecialDetalle
						,@observaciones=observaciones
					FROM #ProductosRecibidos 
					WHERE indice = @indiceActual;

					--afectamos el inventario
					IF(@idEstatusPedidoEspecialDetalle in (2,5) and @cantidadAtendida>0)
					begin

					--validamos que exista la cantidad atendida en el inventario
					IF NOT EXISTS(select 1 from InventarioDetalle where idProducto=@idProducto and idUbicacion=@idUbicacion and cantidad>=@cantidadAtendida)
					begin   
						RAISERROR('No existe suficiente cantidad en el inventario', 15, 217)
					end

					--descontamos el inventario de la ubicacion
					select @cantidadActualSistema=0,@cantidadDespuesOp=0;

					--OBTENEMOS LA CANTIDAD ACTUAL DEL SISTEMA
					select @cantidadActualSistema = cantidad from InventarioDetalle where idUbicacion = @idUbicacion  and idProducto = @idProducto 

					--CALCULAMOS LA CANTIDAD A AJUSTAR
					SET @cantidadDespuesOp = coalesce(@cantidadActualSistema,0) - @cantidadAtendida

					INSERT INTO InventarioDetalleLog(idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta,idPedidoInterno,idPedidoEspecial)
					SELECT @idUbicacion,@idProducto,dbo.redondear(@cantidadAtendida),dbo.redondear(@cantidadDespuesOp),@tipoMovimientoInventarioSalida,@idUsuario,dbo.FechaActual(),0,0,@idPedidoEspecial

					UPDATE InventarioDetalle set cantidad=dbo.redondear(@cantidadDespuesOp),fechaActualizacion=dbo.FechaActual() 
					where idProducto=@idProducto and idUbicacion=@idUbicacion

					--agregamos el inventario a la ubicacion resguardo

					--obtenemos la ubicacion resguardo

					IF NOT EXISTS (select 1 from Ubicacion where idPasillo=1000 and idRaq=1000 and idPiso=1000 and idAlmacen=@idAlmacenDestino) 
					begin
					   INSERT INTO Ubicacion(idAlmacen,idPasillo,idRaq,idPiso)
					   select @idAlmacenDestino,1000,1000,1000;
					end;

					select @idUbicacionResguardo=idUbicacion from Ubicacion where idPasillo=1000 and idRaq=1000 and idPiso=1000 and idAlmacen=@idAlmacenDestino;

					select @cantidadActualSistema=0,@cantidadDespuesOp=0;

					--OBTENEMOS LA CANTIDAD ACTUAL DEL SISTEMA
					select @cantidadActualSistema = cantidad from InventarioDetalle where idUbicacion = @idUbicacionResguardo  and idProducto = @idProducto 

					--CALCULAMOS LA CANTIDAD A AJUSTAR
					SET @cantidadDespuesOp = coalesce(@cantidadActualSistema,0) + @cantidadAtendida

					INSERT INTO InventarioDetalleLog(idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta,idPedidoInterno,idPedidoEspecial)
					SELECT @idUbicacionResguardo,@idProducto,dbo.redondear(@cantidadAtendida),dbo.redondear(@cantidadDespuesOp),@tipoMovimientoInventarioResguardo,@idUsuario,dbo.FechaActual(),0,0,@idPedidoEspecial

					--VALIDAR SI EL PRODUCTO EXISTE EN INVENTARIO DETALLE SI NO EXISTE LO INSERTAMOS
					IF NOT EXISTS (SELECT 1 FROM InventarioDetalle WHERE idUbicacion = @idUbicacionResguardo AND idProducto = @idProducto)
					BEGIN
						INSERT INTO InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion) VALUES (@idProducto,0,dbo.FechaActual(),@idUbicacionResguardo,dbo.FechaActual()) 
					END

					UPDATE InventarioDetalle set cantidad=dbo.redondear(@cantidadDespuesOp),fechaActualizacion=dbo.FechaActual() 
					where idProducto=@idProducto and idUbicacion=@idUbicacionResguardo

					UPDATE #ProductosRecibidos set idUbicacionDestino=@idUbicacionResguardo where idPedidoEspecialDetalle=@idPedidoEspecialDetalle

					end

					select @indiceActual=@indiceActual+1;

				end

				--insertamos en la tabla movimientos de mercancia	
				INSERT INTO PedidosEspecialesMovimientosDeMercancia(idAlmacenOrigen,idAlmacenDestino,idProducto,cantidad,idPedidoEspecial,idUsuario,fechaAlta,idEstatusPedidoEspecialDetalle,observaciones,cantidadAtendida,idUbicacionOrigen,idUbicacionDestino)
				select @idAlmacenOrigen,@idAlmacenDestino,d.idProducto,d.cantidad,@idPedidoEspecial,@idUsuario,@fechaActual,@idEstatusPedidoEspecialDetalle,@observaciones,@cantidadAtendida,p.idUbicacion,p.idUbicacionDestino
				from PedidosEspecialesDetalle d
				join  #ProductosRecibidos p on d.idPedidoEspecialDetalle=p.idPedidoEspecialDetalle
				where d.idPedidoEspecial=@idPedidoEspecial;

				--actualizamos en la tabla PedidosEspecialesDetalle
				update d set cantidadAtendida=p.cantidadAtendida,cantidadRechazada=dbo.redondear(d.cantidad-p.cantidadAtendida) ,idEstatusPedidoEspecialDetalle=p.idEstatusPedidoEspecialDetalle,notificado=0,observaciones=p.observaciones
				from PedidosEspecialesDetalle d
				join #ProductosRecibidos p on d.idPedidoEspecialDetalle=p.idPedidoEspecialDetalle

				--si ya fueron atentidos todos los productos del pedido especial cambiamos el estatus a resguardo
				IF NOT EXISTS(SELECT 1 FROM PedidosEspecialesDetalle where idPedidoEspecial=@idPedidoEspecial and idEstatusPedidoEspecialDetalle=1)
				update PedidosEspeciales set idEstatusPedidoEspecial=3 where idPedidoEspecial=@idPedidoEspecial
				
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

