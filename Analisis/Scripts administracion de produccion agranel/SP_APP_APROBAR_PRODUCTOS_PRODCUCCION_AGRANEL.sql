
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

			/* 1	Ninguno | 2	Alta | 3 Procesado Correctamente | 4.-Rechazo Completo | 5.-Rechazo Parcial */
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
					@idUbicacionFinal int=0,
					@tipoMovimientoInventarioSalida int=30,
					@tipoMovimientoInventarioResguardo int=31,
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
					@idPasilloRackPiso int = 1000

			
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
					IF(@idEstatusProduccionAgranel in (4,3,5) and @cantidadAtendida>0)
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

						--agregamos el inventario a la ubicacion resguardo

						--obtenemos la ubicacion resguardo

						IF NOT EXISTS (select 1 from Ubicacion where idPasillo=@idPasilloRackPiso and idRaq=@idPasilloRackPiso and idPiso=@idPasilloRackPiso and idAlmacen=@idAlmacen)
						BEGIN
						   INSERT INTO Ubicacion(idAlmacen,idPasillo,idRaq,idPiso)
						   SELECT @idAlmacen,@idPasilloRackPiso,@idPasilloRackPiso,@idPasilloRackPiso;
						end;

						SELECT @idUbicacionFinal=idUbicacion from Ubicacion
						where idPasillo=@idPasilloRackPiso and idRaq=@idPasilloRackPiso and idPiso=@idPasilloRackPiso and idAlmacen=@idAlmacen;

						SELECT @cantidadActualSistema=0,@cantidadDespuesOp=0;

						--OBTENEMOS LA CANTIDAD ACTUAL DEL SISTEMA
						SELECT @cantidadActualSistema = cantidad from InventarioDetalle where idUbicacion = @idUbicacionFinal  and idProducto = @idProducto 

						--CALCULAMOS LA CANTIDAD A AJUSTAR
						SET @cantidadDespuesOp = coalesce(@cantidadActualSistema,0) + @cantidadAtendida

						-- VALIDAMOS SI EXISTE REGISTRON EN INVENTARIO DETALLE, SI NO EXISTE ES NECESARIO INSERTARLO
						IF EXISTS (Select 1 from InventarioDetalle ID where ID.idUbicacion =  @idUbicacionFinal and idProducto = @idProducto )
						BEGIN
							UPDATE InventarioDetalle set cantidad =  @cantidadDespuesOp , fechaActualizacion = dbo.FechaActual()
						    WHERE  idProducto = @idProducto and idUbicacion  = @idUbicacionFinal
						END
						ELSE
						BEGIN
							INSERT INTO InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
							VALUES (@idProducto , @cantidadDespuesOp , dbo.FechaActual() ,@idUbicacionFinal, dbo.FechaActual())
						END


						INSERT INTO InventarioDetalleLog(idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta,idPedidoInterno,idProcesoProduccionAgranel)
						SELECT @idUbicacionFinal,@idProducto,dbo.redondear(@cantidadAtendida),dbo.redondear(@cantidadDespuesOp),@tipoMovimientoInventarioResguardo,@idUsuario,dbo.FechaActual(),0,0,@idProcesoProduccionAgranel

						UPDATE InventarioDetalle set cantidad=dbo.redondear(@cantidadDespuesOp),fechaActualizacion=dbo.FechaActual() 
						WHERE idProducto=@idProducto and idUbicacion=@idUbicacionFinal

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
						print '@idUbicacionFinal: '+ISNULL(cast(@idUbicacionFinal as varchar),0);
						print '@cantidadActualSistema: '+ISNULL(cast(@cantidadActualSistema as varchar),0);
						
						-- UNCOMMENT FOR DEBUG
						--select * from ProcesoProduccionAgranel where idProcesoProduccionAgranel= @idProcesoProduccionAgranel
						--select * from InventarioDetalleLog where idProducto = @idProducto order by fechaAlta desc
						--select * from InventarioDetalle where idProducto = @idProducto order by fechaAlta desc
						--select * from InventarioGeneral where idProducto = @idProducto 
					END

					select @indiceActual=@indiceActual+1;

				end -- fin del else

				
			--VALIDAMOS SI LA TRANSACCION SE GENERO AQUI , AQUIMISMO SE HACE EL COMMIT	
		    if @tran_count = 0	
			begin -- si la transacción se inició dentro de este ámbito
						
				commit tran @tran_name
				select @tran_scope = 0
						
			end -- si la transacción se inició dentro de este ámbito

			select 200 Estatus , 'Productos procesados correctamente' Mensaje 
			DROP TABLE #ProductosRecibidos

	END TRY
	BEGIN CATCH
		SELECT -1 Estatus, error_message() Mensaje,error_line() Errorline
		
		if @tran_scope = 1
			rollback tran @tran_name

	END CATCH
	
END

