GO
IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE object_id = OBJECT_ID(N'dbo.SP_APP_AGREGAR_PRODUCTO_INVENTARIO_TRAPEADORES_FINALIZADOS'))
BEGIN
   DROP PROCEDURE SP_APP_AGREGAR_PRODUCTO_INVENTARIO_TRAPEADORES_FINALIZADOS
END


GO
/****** Object:  StoredProcedure [dbo].[SP_APP_AGREGAR_PRODUCTO_INVENTARIO_LIQUIDOS_ENVASADO]    Script Date: 10/4/2023 11:37:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_APP_AGREGAR_PRODUCTO_INVENTARIO_TRAPEADORES_FINALIZADOS]
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
			@idTipoMovInventario int  = 32, /*32	Actualizacion de Inventario(carga de proceso de produccion de trapeadores finalizados) */
			@cantidadTotal float = 0,
			@idUnidadMedida int = 0,
			@cantidadUnidadMedida  float =0,
			@idProductoMatra int = 0,
			@idProductoBaston int =0,
			@valorUnidadMedida float =0,
			@cantidadReal float =0
			

			
			select @idProductoBaston = MPP.idProductoMateriaProduccion  
			from MateriasPrimasProduccion MPP join Productos P on P.idProducto = MPP.idProductoMateriaProduccion  
	        where  idProductoProduccion = @idProducto and P.idLineaProducto = 26 -- 26 - LINEA BASTON

			select 
				@idProductoMatra = MPP.idProductoMateriaProduccion , 
				@cantidadUnidadMedida = MPP.valorUnidadMedida ,
				@valorUnidadMedida =  MPP.valorUnidadMedida,
				@idUnidadMedida = MPP.idUnidadMedidad
				from MateriasPrimasProduccion MPP join Productos P on P.idProducto = MPP.idProductoMateriaProduccion  
	        where  idProductoProduccion = @idProducto and P.idLineaProducto = 21 -- 21 - LINEA MATRA

			SELECT @cantidadReal = dbo.redondear(@cantidad * @valorUnidadMedida)

			-- VALIDAMOS LOS ID DE LOS PRODUCTOS BASTON Y MATRA PARA ANALIZAR SI PODEMOS AGREGAR A INVENTARIO
			SELECT @idProductoBaston = COALESCE(@idProductoBaston,0) , @idProductoMatra =COALESCE(@idProductoMatra,0)

			if (@idProductoBaston = 0)
			begin 
				select -1 Estatus , 'No existe relación  entre el baston y el producto a producir que deseas agregar ' Mensaje
				return
			end
				
			
			if (@idProductoMatra = 0)
			begin 
				select -1 Estatus , 'No existe relación entre el producto de la linea matra  y el producto a producir que deseas agregar ' Mensaje
				return
			end
			
			if dbo.[ExisteCantidadProductoEnAlmancen](@idAlmacen,@idProductoMatra) < @cantidadReal
			begin
					select -1 Estatus , 'No existe suficiente inventario del producto de la linea matra granel para convertir a envase.' Mensaje
					return
			end

			if dbo.[ExisteProductoEnAlmancen](@idAlmacen,@idProducto) = 0
			begin
					select -1 Estatus , 'el producto no puede ser agregado a este almacen.' Mensaje
					return
			end

			
			if not exists (select 1 from InventarioDetalle ID join Ubicacion U on ID.idUbicacion  = U.idUbicacion
			where U.idPiso=1005	 and  U.idPasillo =1005 and U.idRaq = 1005 and idProducto = @idProductoBaston and coalesce(ID.cantidad,0 ) > @cantidad)
			begin
					select -1 Estatus , 'No existen suficientes bastones en la ubicacion de resguardo para baston' Mensaje
					return
			end
			
			create table #productos_matra(
					id int identity(1,1),
					idProducto int,
					cantidad float,
					idAlamcen int, 
					idUbicacion int,
			)

			insert into #productos_matra (idProducto ,cantidad ,idAlamcen ,idUbicacion)
			select idProducto, SUM(ID.cantidad) cantidad , idAlmacen,ID.idUbicacion  
			FROM 
				InventarioDetalle ID join Ubicacion U on ID.idUbicacion = U.idUbicacion  
			where 
				idProducto = @idProductoMatra 
				and idAlmacen = @idAlmacen 
				and ID.cantidad > 0
				and (U.idPasillo not in (1000,1001,1002,1003,1004,1005)
					and U.idRaq not in (1000,1001,1002,1003,1004,1005)
					and U.idPiso not in (1000,1001,1002,1003,1004,1005)
					)
				
			group by ID.idUbicacion , idAlmacen, idProducto order by cantidad desc
			
			if not exists (select 1 from #productos_matra)
			begin
				select -1 Estatus , 'no existen productos matras en ubicaciones disponibles' Mensaje
				drop table #productos_matra;
				return
			end

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
										dbo.redondear(@cantidad),
										dbo.redondear(@cantidadTotal),
										@idTipoMovInventario,
										@idUsuario,
										dbo.FechaActual()
									)
						
						 update InventarioDetalle set cantidad =   dbo.redondear(@cantidadTotal) , fechaActualizacion = dbo.FechaActual()
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
					@idTipoMovInventarioEnvase int = 33, /*Actualizacion de Inventario(salida de mercancia por conversion de linea matra a trapeadores)*/
					@cantidadDescuentoAgranel float =0
					select @idUbicacionEnvase = idUbicacion from Ubicacion 
					where idPasillo =1005 and idRaq =1005 and idPiso = 1005
						
					select  @cantidadActualEnvase = isnull(ID.cantidad,0) 
					from InventarioDetalle ID 
					where ID.idUbicacion =  @idUbicacionEnvase and idProducto = @idProductoBaston

					set @cantidadDespuesDeOPeracionEnvase = dbo.redondear(isnull(@cantidadActualEnvase,0) - isnull(@cantidad,0))

					INSERT INTO InventarioDetalleLog (idUbicacion,idProducto
													,cantidad,cantidadActual
													,idTipoMovInventario,idUsuario
													,fechaAlta)
					VALUES(
						@idUbicacion,@idProductoBaston,
						@cantidad,@cantidadDespuesDeOPeracionEnvase,
						@idTipoMovInventarioEnvase,@idUsuario,
						dbo.FechaActual()
					)
					UPDATE InventarioDetalle set cantidad =  @cantidadDespuesDeOPeracionEnvase , fechaActualizacion = dbo.FechaActual()
					where  idProducto = @idProductoBaston and idUbicacion  = @idUbicacionEnvase

					UPDATE InventarioGeneral SET cantidad = (cantidad - @cantidad),fechaUltimaActualizacion = dbo.FechaActual()
					WHERE idProducto = @idProductoBaston
					DECLARE
					@ini int = 0,
					@fin int = 0
					select @ini = min(id), @fin= max(id) from #productos_matra
					WHILE ( @ini <= @fin )
					BEGIN
						DECLARE
							@cantidadActualAgranel float = 0,
							@cantidadDespuesDeOperacionAgranel float = 0,
							@idUbicacionAgranel int = 0
						
							--SET DE VALORES
							SELECT @idUbicacionAgranel = idUbicacion , @cantidadActualAgranel = cantidad from #productos_matra where id = @ini
							--AGRANEL
							select  @cantidadActualAgranel = isnull(ID.cantidad,0) 
							from InventarioDetalle ID 
							where ID.idUbicacion =  @idUbicacionAgranel
								and idProducto = @idProductoMatra
							
							PRINT CAST (@cantidadActualAgranel AS VARCHAR)
							PRINT CAST (@cantidadReal AS VARCHAR)
							if(@cantidadActualAgranel > @cantidadReal)
							BEGIN
								PRINT 'ENTRE AL IF'
								set @cantidadDespuesDeOperacionAgranel = isnull(@cantidadActualAgranel,0) - isnull(@cantidadReal,0)
								SET @cantidadDescuentoAgranel = @cantidadReal;
								SET @ini = @fin +1
							END
							ELSE
							BEGIN
								PRINT 'ENTRE AL ELSE'
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
								@idUbicacionAgranel,@idProductoMatra,
								@cantidadDescuentoAgranel,@cantidadDespuesDeOperacionAgranel,
								@idTipoMovInventarioEnvase,@idUsuario,
								dbo.FechaActual()
							)
							 UPDATE InventarioDetalle set cantidad =  @cantidadDespuesDeOperacionAgranel , fechaActualizacion = dbo.FechaActual()
							 where  idProducto = @idProductoMatra and idUbicacion  = @idUbicacionAgranel

							 UPDATE InventarioGeneral SET cantidad = (cantidad - @cantidadDescuentoAgranel),fechaUltimaActualizacion = dbo.FechaActual()
							 WHERE idProducto = @idProductoMatra

							 SET @ini = @ini +1;
					END
				  END
				  drop table #productos_matra;
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
