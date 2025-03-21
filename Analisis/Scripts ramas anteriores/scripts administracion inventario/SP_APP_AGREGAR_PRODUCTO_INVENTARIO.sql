----USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_APP_AGREGAR_PRODUCTO_INVENTARIO]    Script Date: 21/07/2022 11:13:36 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_APP_AGREGAR_PRODUCTO_INVENTARIO]
@idProducto int,
@idProveedor int,
@cantidad float,
@idUsuario int,
@idAlmacen int
--@idTipoMovInventario int
AS
BEGIN
	BEGIN TRY

/*idTipoMovInventario	
1	Venta
2	Resutir
3	Cancelacion
4	Carga de Inventario
5	Actualizacion de Inventario (asignacion de la ubicacion fisica del producto)
6	Actualizacion de Inventario (sobrante del producto sin ubicacion asignada )
7	Actualizacion de Inventario( salida de mercancia por pedido interno)
8	Actualizacion de Inventario( carga de mercancia por pedido interno)
*/
			declare
			@idUbicacion int ,
			@cantidadActual float = 0,
			@idTipoMovInventario int  = 4,
			@cantidadTotal float = 0


			if exists (select 1 from Productos where idProducto = @idProducto and idLineaProducto = 20) /*Liquidos*/
			BEGIN
				if exists (select 1 from Usuarios where idUsuario = @idUsuario and idRol = 13) /* ENCARGADO DE PRODUCCION*/
				begin
						SET @idTipoMovInventario = 26
				end
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
		
			if not exists (select * from Proveedores where idProveedor =@idProveedor)
			begin
					select -1 Estatus , 'El Proveedor aun no se encuentra registrado ' Mensaje
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
			
					-- INSERTAMOS EN INVENTARIO   E INVENTARIO DETALLE UNA VEZ QUE INSERTAMOS LA UBICACION EN 0
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
