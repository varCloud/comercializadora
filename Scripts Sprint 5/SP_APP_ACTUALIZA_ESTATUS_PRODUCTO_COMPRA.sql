use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_COMPLEMENTOS_VENTA
if exists (select 1 from sysobjects where name like 'SP_APP_ACTUALIZA_ESTATUS_PRODUCTO_COMPRA' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_APP_ACTUALIZA_ESTATUS_PRODUCTO_COMPRA
go

/*

Autor			VIC
UsuarioRed		auhl373453
Fecha			2020/07/23
Objetivo		Consulta productos agregados/devueltos de una venta
status			200 = ok
				-1	= error
*/

create proc SP_APP_ACTUALIZA_ESTATUS_PRODUCTO_COMPRA
	@productos XML ,
    @idAlmacen int ,
	@idUsuario int ,
	@idCompra int


as
	begin -- procedimiento
		begin try -- try principal
			
			IF OBJECT_ID('tempdb..#ProductosRecibidos') IS NOT NULL
			DROP TABLE #ProductosRecibidos
			create table #ProductosRecibidos
			(	
						indice int identity(1,1),	
						idCompraDetalle		int ,		
						idProducto			int ,		
						idEstatusProductoCompra	int ,		
						cantidadRecibida	int ,		
						cantidadDevuelta	int ,		
						observaciones		varchar(500)
			)

			insert into  #ProductosRecibidos
			SELECT  
					P.value('idCompraDetalle[1]', 'INT') AS idCompraDetalle,
					P.value('idProducto[1]', 'INT') AS idProducto,
					P.value('idEstatusProductoCompra[1]', 'INT') AS idEstatusProductoCompra,
					P.value('cantidadRecibida[1]', 'INT') AS cantidadRecibida,
					P.value('cantidadDevuelta[1]', 'INT') AS cantidadDevuelta,
					P.value('observaciones[1]', 'VARCHAR(500)') AS observaciones
			FROM  @productos.nodes('//ArrayOfProductosCompra/ProductosCompra') AS x(P)

			begin -- validaciones

				if exists (select 1 from #ProductosRecibidos PR left join ComprasDetalle C on PR.idCompraDetalle = C.idCompraDetalle
					where C.idCompraDetalle is null and C.idCompra is null)
				begin
					select -1 Estatus , 'La compra contiene un producto que no le pertenece' Mensaje
				end

				if exists (select 1 from Compras where idStatusCompra !=2)
				begin
					select -1 Estatus , 'Esta compra ya ha sido procesada' Mensaje
				end
			end


			
			declare  	@tran_name varchar(32) = 'PRODUCTO_COMPRA',
						@tran_count int = @@trancount,
						@tran_scope bit = 0

			if @tran_count = 0
				begin tran @tran_name
			else
				save tran @tran_name
				
			select @tran_scope = 1
				

				BEGIN-- DECLARACIONES
					DECLARE
					@indice int = 1,
					@max int = 0,
					@cantidadDevuelta int = 0,
					@cantidadRecibida int = 0,
					@idProducto int = 0,
					@cantidadActual int  =0,
					@cantidadTotal int = 0,
					@idTipoMovInventario int = 12		
				END

				SELECT @max = max(indice) from #ProductosRecibidos
				
				WHILE(@indice <= @max)
					BEGIN -- BEGIN WHILE

					   SELECT 
							@idProducto =idProducto 
							,@cantidadRecibida =cantidadRecibida 
							,@cantidadDevuelta =cantidadDevuelta
					   FROM 
							#ProductosRecibidos 
					   WHERE indice = @indice

					   DECLARE
					   @idUbicacion int = 0

					   SELECT  @idUbicacion = idUbicacion FROM Ubicacion WHERE idAlmacen =@idAlmacen and idPasillo = 0  and idRaq =0 and idPiso = 0

					   -- SI LA QUERY DE ARRIBA NO ME REGRESA RESULTADO QUIERE DECIR QUE LA UBICACION NO EXISTE Y POR LO TANTO HAY QUE INSERTARLA
					   IF coalesce(@idUbicacion,0) = 0
					   BEGIN
							INSERT INTO Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) VALUES (@idAlmacen,0,0,0)
							SELECT  @idUbicacion = idUbicacion FROM Ubicacion WHERE idAlmacen =@idAlmacen and idPasillo = 0  and idRaq =0 and idPiso = 0
					   END

					   -- INSERTAMOS EN INVENTARIO  E INVENTARIO DETALLE UNA VEZ QUE INSERTAMOS LA UBICACION EN 0
					   -- LO CUAL QUIERE DECIR QUE AUN NO ESTA ACOMODADA
					   IF EXISTS (SELECT 1 FROM  InventarioDetalle ID where ID.idUbicacion =  @idUbicacion and idProducto = @idProducto )
					   BEGIN
							select  @cantidadActual = isnull(ID.cantidad,0) 
							from InventarioDetalle ID 
							where ID.idUbicacion =  @idUbicacion and idProducto = @idProducto

							set @cantidadTotal = @cantidadRecibida + isnull(@cantidadActual,0)

						    --INSERTAMOS EN INVENTARIO DETALLE
							insert into InventarioDetalleLog (
															idUbicacion
															,idProducto
															,cantidad
															,cantidadActual
															,idTipoMovInventario
															,idUsuario
															,fechaAlta
															,idCompra
															)
													VALUES(
															@idUbicacion,
															@idProducto,
															@cantidadRecibida,
															@cantidadTotal,
															@idTipoMovInventario,
															@idUsuario,
															dbo.FechaActual(),
															@idCompra
															)

								UPDATE InventarioDetalle set cantidad =  @cantidadTotal , fechaActualizacion = dbo.FechaActual()
								WHERE  idProducto = @idProducto and idUbicacion  = @idUbicacion
						END
					    ELSE -- si no existe el producto en inventario detalle , es decir si es la primera vez que insertamos el producto  a inventario
						BEGIN
							-- INSERTAMOS EN INVENTARIO DETALLE
							INSERT INTO InventarioDetalleLog (
															idUbicacion
															,idProducto
															,cantidad
															,cantidadActual
															,idTipoMovInventario
															,idUsuario
															,fechaAlta
															,idCompra
															)
											VALUES         (
														@idUbicacion,
														@idProducto,
														@cantidadRecibida,
														@cantidadRecibida,
														@idTipoMovInventario,
														@idUsuario,
														dbo.FechaActual(),
														@idCompra
													)

							INSERT INTO InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
							VALUES (@idProducto , @cantidadRecibida , dbo.FechaActual() ,@idUbicacion, dbo.FechaActual())
						END
						--PREPARAMOS LAS VARIABLES PARA EL InventarioGeneralLog
						declare 
						@cantidadActInvGeneral int,
						@cantidadDespuesDeOperacionInvGeneral int 

						--	obtenemos la cantidad actual del inventario general
						SELECT @cantidadActInvGeneral=cantidad from InventarioGeneral where idProducto = @idProducto 
						--	sumamos la cantidad que que recibio en inv general
						SET @cantidadDespuesDeOperacionInvGeneral = coalesce(@cantidadActInvGeneral,0)+@cantidadRecibida

						INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
						values(@idProducto,@cantidadRecibida,@cantidadDespuesDeOperacionInvGeneral,dbo.FechaActual(),@idTipoMovInventario)
		

						-- INSERTAMOS LOS VALORES EN EL INVENTARIO GENERAL
						IF exists (select 1 from InventarioGeneral where idProducto = @idProducto)
						BEGIN
								UPDATE InventarioGeneral
								SET
									cantidad = (cantidad + @cantidadRecibida),
									fechaUltimaActualizacion = dbo.FechaActual()
								WHERE
									idProducto = @idProducto
						END
						ELSE
						BEGIN 
								INSERT InventarioGeneral (idProducto,cantidad,fechaUltimaActualizacion)
								VALUES(@idProducto ,@cantidadRecibida , dbo.FechaActual() )
						END
						SET @indice = @indice +1 
					END -- FIN DEL WHILE
                
				-- ACTULIZAMOS EL ESTATUS DE LOS PRODUCTOS DE LAS COMPRAS
				UPDATE CD
				SET 
					 CD.cantidadRecibida  = PR.cantidadRecibida
					,CD.observaciones = PR.observaciones
					,CD.idEstatusProductoCompra = PR.idEstatusProductoCompra
					,CD.fechaRecibio =  dbo.FechaActual()
					,CD.cantidadDevuelta = coalesce(PR.cantidadDevuelta,0)
					,cd.idUsuarioRecibio = @idUsuario
				FROM 
				ComprasDetalle CD  
				JOIN #ProductosRecibidos PR ON CD.idCompra = @idCompra AND  CD.idProducto = PR.idProducto  and CD.idCompraDetalle = PR.idCompraDetalle

				--
				--SELECT * FROM CatStatusCompra
				--ACTUALIZAMOS EL ESTATUS GENERAL DE LA COMPRA AL ESTATUS DE FINALIZADA
				UPDATE Compras SET idStatusCompra= 3 WHERE idCompra = @idCompra

			--VALIDAMOS SI LA TRANSACCION SE GENERO AQUI , AQUIMISMO SE HACE EL COMMIT	
		    if @tran_count = 0	
			begin -- si la transacción se inició dentro de este ámbito
						
				commit tran @tran_name
				select @tran_scope = 0
						
			end -- si la transacción se inició dentro de este ámbito

			select 200 Estatus , 'OK' Mensaje 
			DROP TABLE #ProductosRecibidos
						
		end try -- fin del try 
		begin catch
			SELECT -1 Estatus, error_message() Mensaje,error_line() Errorline

			if @tran_scope = 1
				rollback tran @tran_name
		end catch

	end -- fin de procedimiento