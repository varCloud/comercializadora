---USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].SP_APP_AGREGAR_PRODUCTO_INVENTARIO_LIQUIDOS_PRODUCCION    Script Date: 17/06/2022 09:56:52 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].SP_APP_AGREGAR_PRODUCTO_INVENTARIO_LIQUIDOS_ENVASADO
@idProducto int,
@cantidad float,
@idUsuario int,
@idAlmacen int
AS
BEGIN
	BEGIN TRY

/*idTipoMovInventario	*/
			declare
			@idUbicacion int ,
			@cantidadActual float = 0,
			@idTipoMovInventario int  = 27 /*27	Actualizacion de Inventario(carga de mercancia por usuario de Envasado de líquidos)*/,
			@cantidadTotal float = 0,
			@idUnidadMedida int = 0,
			@cantidadUnidadMedida  float =0,
			@idProductoAgranel int = 0,
			@idProductoEnvase int =0,
			@valorUnidadMedida int =0,
			@cantidadReal float =0
			print('antes')
			select 
				@idProductoEnvase = idProducoEnvase,
				@idProductoAgranel =idProductoAgranel,
				@valorUnidadMedida = valorUnidadMedida
			from ProductosEnvasadosXAgranel where idProductoEnvasado = @idProducto

			-- VALIDAMOS LOS ID DE LOS PRODUCTOS AGRANEL Y DE ENVASE PARA ANALIZAR SI PODEMOS AGREGAR A INVENTARIO
			SELECT @idProductoEnvase = COALESCE(@idProductoEnvase,0) , @idProductoAgranel =COALESCE(@idProductoAgranel,0)
			SELECT @cantidadReal = dbo.redondear(@cantidad * @valorUnidadMedida);
			
			if (@idProductoEnvase = 0)
			begin 
				select -1 Estatus , 'No existe relación  entre el envase y el producto que deseas agregar ' Mensaje
				return
			end
				
			
			if (@idProductoAgranel = 0)
			begin 
				select -1 Estatus , 'No existe relación entre el producto a granel y el producto que deseas agregar ' Mensaje
				return
			end
			
			if dbo.[ExisteCantidadProductoEnAlmancen](@idAlmacen,@idProductoAgranel) < @cantidadReal
			begin
					declare @productoDesc varchar(100)
					select @productoDesc = idProducto from Productos where idProducto = @idProductoAgranel
					select -1 Estatus , 'No existe suficiente inventario del producto a granel para convertir a envase. idProducto ['+ cast(@productoDesc as varchar) +'] ' Mensaje
					return
			end

			if dbo.[ExisteProductoEnAlmancen](@idAlmacen,@idProducto) = 0
			begin
					select -1 Estatus , 'el producto no puede ser agregado a este almacen.' Mensaje
					return
			end

			
			if not exists (select 1 from InventarioDetalle ID join Ubicacion U on ID.idUbicacion  = U.idUbicacion
			where U.idPasillo =1002 and U.idRaq = 1002 and idProducto = @idProductoEnvase and coalesce(ID.cantidad,0 ) > @cantidad)
			begin
					select -1 Estatus , 'No existen suficientes envases en la ubicacion de resguardo para envase' Mensaje
					return
			end
			


			create table #productos_agranel(
					id int identity(1,1),
					idProducto int,
					cantidad float,
					idAlamcen int, 
					idUbicacion int,
			)

			insert into #productos_agranel (idProducto ,cantidad ,idAlamcen ,idUbicacion)
			select idProducto, SUM(ID.cantidad) cantidad , idAlmacen,ID.idUbicacion  
			FROM InventarioDetalle ID join Ubicacion U on ID.idUbicacion = U.idUbicacion  
			where idProducto = @idProductoAgranel and idAlmacen = @idAlmacen and ID.cantidad > 0
			group by ID.idUbicacion , idAlmacen, idProducto order by cantidad desc

			--select * from #productos_agranel
			-- OBTENEMOS EL DETALLE DEL PRODUCTO
			---SELECT @cantidadUnidadMedida = cantidadUnidadMedida, @idUnidadMedida= idUnidadMedida  from Productos where idProducto = @idProducto


		   BEGIN TRAN
				   BEGIN
				   				
					-- OBTENER LA UBICACION DEL PRODUCTO EN CASO DE QUE EXISTA SI NO LA INSERTAMOS Y LA OBTENEMOS
					IF EXISTS (SELECT 1 FROM Ubicacion U 
					where u.idAlmacen =@idAlmacen and U.idPasillo = 0 and idPiso = 0 and idRaq = 0 )
					begin
						SELECT @idUbicacion = U.idUbicacion FROM Ubicacion U 
						where u.idAlmacen =@idAlmacen and U.idPasillo = 0 and idPiso = 0 and idRaq = 0 
					end
					ELSE
					begin
						INSERT INTO Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) VALUES(@idAlmacen,0,0,0)
						select @idUbicacion  = Max(U.idUbicacion) from Ubicacion U  
						where U.idAlmacen =@idAlmacen and U.idPasillo = 0 and idPiso = 0 and idRaq = 0 
					end
					
					
					-- INSERTAMOS EN INVENTARIO  E INVENTARIO DETALLE UNA VEZ QUE INSERTAMOS LA UBICACION EN 0
					-- LO CUAL QUIERE DECIR QUE AUN NO ESTA ACOMODADA
					IF exists (Select 1 from InventarioDetalle ID where ID.idUbicacion =  @idUbicacion and idProducto = @idProducto )
					BEGIN
					
						select  @cantidadActual = isnull(ID.cantidad,0) 
						from InventarioDetalle ID 
						where ID.idUbicacion =  @idUbicacion and idProducto = @idProducto

						set @cantidadTotal = @cantidad + isnull(@cantidadActual,0)

						-- INSERTAMOS EN INVENTARIO DETALLE
						insert into InventarioDetalleLog (
														idUbicacion
														,idProducto
														,cantidad
														,cantidadActual
														,idTipoMovInventario
														,idUsuario
														,fechaAlta
														)
									VALUES(
										@idUbicacion,
										@idProducto,
										@cantidad,
										@cantidadTotal,
										@idTipoMovInventario,
										@idUsuario,
										dbo.FechaActual()
									)
						
						 update InventarioDetalle set cantidad =  @cantidadTotal , fechaActualizacion = dbo.FechaActual()
						 where  idProducto = @idProducto and idUbicacion  = @idUbicacion
					END
					ELSE -- si no existe el producto en inventario detalle , es decir si es la primera vez que insertamos a inventario
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
															)
							VALUES                  (
														@idUbicacion,
														@idProducto,
														@cantidad,
														@cantidad,
														@idTipoMovInventario,
														@idUsuario,
														dbo.FechaActual()
													)

						INSERT INTO InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
						VALUES (@idProducto , @cantidad , dbo.FechaActual() ,@idUbicacion, dbo.FechaActual())
					END
					

					--INSERTAMOS EN LA TABLA LOG
					declare @cantidadAct int

					select @cantidadAct=cantidad from InventarioGeneral where idProducto = @idProducto 

					INSERT INTO InventarioGeneralLog(idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
					values(@idProducto,@cantidad,coalesce(@cantidadAct,0)+@cantidad,dbo.FechaActual(),@idTipoMovInventario)
		

					-- INSERTAMOS LOS VALORES EN EL INVENTARIO GENERAL
					IF exists (select 1 from InventarioGeneral where idProducto = @idProducto)
					BEGIN
							update InventarioGeneral
							SET 
								cantidad = (cantidad + @cantidad),
								fechaUltimaActualizacion = dbo.FechaActual()
							WHERE 
								idProducto = @idProducto
					END
					ELSE
					BEGIN 
							 insert into InventarioGeneral (idProducto,cantidad,fechaUltimaActualizacion)
							 values(@idProducto ,@cantidad , dbo.FechaActual() )
					END
					-----------------------------------------------------------
					--INSERTAMOS LAS SALIDA DE ENVASES Y DEL LIQUIDO A GRANEL---
					-----------------------------------------------------------
					--ENVASE
					DECLARE
					@cantidadActualEnvase bigint = 0,
					@cantidadDespuesDeOPeracionEnvase bigint = 0,
					@idUbicacionEnvase int = 0,
					@idTipoMovInventarioEnvase int = 28, /*SALIDA DE MERCANCIA POR CONVERSION DE LIQUIDOS AGRANEL A ENVASE*/
					@cantidadDescuentoAgranel float =0
					select @idUbicacionEnvase = idUbicacion from Ubicacion 
					where idPasillo =1002 and idRaq =1002 and idPasillo = 1002
						
					select  @cantidadActualEnvase = isnull(ID.cantidad,0) 
					from InventarioDetalle ID 
					where ID.idUbicacion =  @idUbicacionEnvase and idProducto = @idProductoEnvase

					set @cantidadDespuesDeOPeracionEnvase = dbo.redondear(isnull(@cantidadActualEnvase,0) - isnull(@cantidad,0))

					INSERT INTO InventarioDetalleLog (idUbicacion,idProducto
													,cantidad,cantidadActual
													,idTipoMovInventario,idUsuario
													,fechaAlta)
					VALUES(
						@idUbicacion,@idProductoEnvase,
						@cantidad,@cantidadDespuesDeOPeracionEnvase,
						@idTipoMovInventarioEnvase,@idUsuario,
						dbo.FechaActual()
					)
					UPDATE InventarioDetalle set cantidad =  @cantidadDespuesDeOPeracionEnvase , fechaActualizacion = dbo.FechaActual()
					where  idProducto = @idProductoEnvase and idUbicacion  = @idUbicacionEnvase

					UPDATE InventarioGeneral SET cantidad = (cantidad - @cantidad),fechaUltimaActualizacion = dbo.FechaActual()
					WHERE idProducto = @idProductoEnvase
					
					DECLARE
					@ini int = 0,
					@fin int = 0
					select @ini = min(id), @fin= max(id) from #productos_agranel
					WHILE ( @ini <= @fin )
					BEGIN
						DECLARE
							@cantidadActualAgranel float = 0,
							@cantidadDespuesDeOperacionAgranel float = 0,
							@idUbicacionAgranel int = 0
						
							--SET DE VALORES
							SELECT @idUbicacionAgranel = idUbicacion , @cantidadActualAgranel = cantidad from #productos_agranel where id = @ini
							--AGRANEL
							select  @cantidadActualAgranel = isnull(ID.cantidad,0) 
							from InventarioDetalle ID 
							where ID.idUbicacion =  @idUbicacionAgranel and idProducto = @idProductoAgranel

							if(@cantidadActualAgranel > @cantidadReal)
							BEGIN
								set @cantidadDespuesDeOperacionAgranel = isnull(@cantidadActualAgranel,0) - isnull(@cantidadReal,0)
								SET @cantidadDescuentoAgranel = @cantidadReal;
								SET @ini = @fin +1
							END
							ELSE
							BEGIN
								SELECT 
									@cantidadDespuesDeOperacionAgranel = 0,
									@cantidadReal = dbo.redondear(@cantidadReal - isnull(@cantidadActualAgranel,0)),
									@cantidadDescuentoAgranel = @cantidadActualAgranel

							END

							INSERT INTO InventarioDetalleLog (idUbicacion,idProducto
									,cantidad,cantidadActual
									,idTipoMovInventario,idUsuario
									,fechaAlta)
							VALUES(
								@idUbicacionAgranel,@idProductoAgranel,
								@cantidadDescuentoAgranel,@cantidadDespuesDeOperacionAgranel,
								@idTipoMovInventarioEnvase,@idUsuario,
								dbo.FechaActual()
							)
							 UPDATE InventarioDetalle set cantidad =  @cantidadDespuesDeOperacionAgranel , fechaActualizacion = dbo.FechaActual()
							 where  idProducto = @idProductoAgranel and idUbicacion  = @idUbicacionAgranel

							 UPDATE InventarioGeneral SET cantidad = (cantidad - @cantidadDescuentoAgranel),fechaUltimaActualizacion = dbo.FechaActual()
							 WHERE idProducto = @idProductoAgranel

							 SET @ini = @ini +1;
					END
				  END
				  drop table #productos_agranel;
				COMMIT TRAN
				SELECT 200 Estatus , 'Producto agregado correctamente al inventario' Mensaje

		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		select 
			-1 Estatus ,
			'Ha ocurrido un error al agregar el producto al inventario general' Mensaje,
			 ERROR_NUMBER() AS ErrorNumber  ,
			 ERROR_MESSAGE() AS ErrorMessage  ,
			 ERROR_LINE() as error_lin
	END CATCH

END
