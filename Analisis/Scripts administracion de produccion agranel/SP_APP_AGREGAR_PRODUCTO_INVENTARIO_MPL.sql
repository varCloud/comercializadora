----USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_APP_AGREGAR_PRODUCTO_INVENTARIO_MPL]    Script Date: 21/07/2022 11:13:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].SP_APP_INVENTARIO_AGREGAR_PRODUCTO_PRODUCCION_AGRANEL
@idProducto int,
@cantidad float,
@idUsuario int,
@idAlmacen int
--@idTipoMovInventario int
AS
BEGIN
	BEGIN TRY

			declare
			@idUbicacion int ,
			@cantidadActual float = 0,
			@idTipoMovInventario int  = 29, /*Actualizacion de Inventario(carga de mercancia por usuario para proceso de produccion agranel (MPL))*/
			@cantidadTotal float = 0,
		    @idPisoRaqPasillo int = 1004   


			if not exists (select 1 from Productos where idProducto = @idProducto and idLineaProducto = 12) /*MPL*/
			BEGIN
				select -1 Estatus , 'El producto debe pertenecer a la linea de MPL' Mensaje
				return
			END

			if (@idAlmacen is null)
			begin 
				select -1 Estatus , 'El almacen es requerido ' Mensaje
				return
			end

			if dbo.[ExisteProductoEnAlmancen](@idAlmacen,@idProducto) = 0
			begin
					select -1 Estatus , 'el producto no puede ser agregado a este almacen ' Mensaje
					return
			end
		
		   BEGIN TRAN
				   BEGIN

					

					-- OBTENER LA UBICACION DEL PRODUCTO EN CASO DE QUE EXISTA SI NO LA INSERTAMOS Y LA OBTENEMOS
					IF EXISTS (SELECT 1 FROM Ubicacion U 
					where u.idAlmacen =@idAlmacen and U.idPasillo = @idPisoRaqPasillo and idPiso = @idPisoRaqPasillo and idRaq = @idPisoRaqPasillo )
					begin
						SELECT @idUbicacion = U.idUbicacion FROM Ubicacion U 
						where u.idAlmacen =@idAlmacen and U.idPasillo = @idPisoRaqPasillo and idPiso = @idPisoRaqPasillo and idRaq = @idPisoRaqPasillo 
					end
					ELSE-- SI NO EXISTE LA UBICACION EN PROCESO DE PRODUCCION AGRANEL LA INSERTAMOS
					begin
						INSERT INTO Ubicacion (idAlmacen,idPasillo,idRaq,idPiso) VALUES(@idAlmacen,@idPisoRaqPasillo,@idPisoRaqPasillo,@idPisoRaqPasillo)
						select @idUbicacion  = Max(U.idUbicacion) from Ubicacion U  
						where U.idAlmacen =@idAlmacen and U.idPasillo = @idPisoRaqPasillo and idPiso = @idPisoRaqPasillo and idRaq = @idPisoRaqPasillo
					end

					--INSERTAMOS LOS VALORS EN LA TABLA PARA LA ADMINISTRACION DEL PROCESO DE PRODUCTOS MPL AGRANEL(PRODUCCION)
					INSERT INTO ProcesoProduccionAgranel (idProducto,idUbicacion,cantidad,cantidadAceptada,cantidadRestante,fechaAlta,fechaUltimaActualizacion,idEstatusProduccionAgranel,idUsuario ,idAlmacen)
					values (@idProducto,@idUbicacion,@cantidad,0,0,dbo.FechaActual(),dbo.FechaActual() , 1 /*ALTA*/ , @idUsuario ,@idAlmacen)
					
					declare @idProcesoProduccionAgranel bigint = 0
					select @idProcesoProduccionAgranel = MAX(idProcesoProduccionAgranel) from ProcesoProduccionAgranel where idProducto = @idProducto
			
					-- INSERTAMOS EN INVENTARIO E INVENTARIO DETALLE UNA VEZ QUE INSERTAMOS LA UBICACION EN 0
					-- LO CUAL QUIERE DECIR QUE AUN NO ESTA ACOMODADA
					if exists (Select 1 from InventarioDetalle ID where ID.idUbicacion =  @idUbicacion and idProducto = @idProducto )
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
														,idProcesoProduccionAgranel
														)
									VALUES(
										@idUbicacion,
										@idProducto,
										@cantidad,
										@cantidadTotal,
										@idTipoMovInventario,
										@idUsuario,
										dbo.FechaActual(),
										@idProcesoProduccionAgranel
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
															,idProcesoProduccionAgranel
															)
							VALUES                  (
														@idUbicacion,
														@idProducto,
														@cantidad,
														@cantidad,
														@idTipoMovInventario,
														@idUsuario,
														dbo.FechaActual(),
														@idProcesoProduccionAgranel
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
							Set 
								cantidad = (cantidad + @cantidad),
								fechaUltimaActualizacion = dbo.FechaActual()
							where 
								idProducto = @idProducto
					END
					ELSE
					BEGIN 
							 insert into InventarioGeneral (idProducto,cantidad,fechaUltimaActualizacion)
							 values(@idProducto ,@cantidad , dbo.FechaActual() )
					END

					

				  END
				COMMIT TRAN
				SELECT 200 Estatus , 'Producto agregado correctamente al inventario' Mensaje

		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		select 
			-1 Estatus ,
			'Ha ocurrido un error al agregar el producto al inventario general' Mensaje,
			 ERROR_NUMBER() AS ErrorNumber  ,
			 ERROR_MESSAGE() AS ErrorMessage  
	END CATCH

END
