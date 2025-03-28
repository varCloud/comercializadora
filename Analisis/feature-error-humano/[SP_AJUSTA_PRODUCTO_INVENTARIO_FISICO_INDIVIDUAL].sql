USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_AJUSTA_PRODUCTO_INVENTARIO_FISICO_INDIVIDUAL]    Script Date: 9/2/2023 12:43:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			JESSI
Fecha			2021/08/04
Objetivo		Ajustar inventario fisico de producto
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_AJUSTA_PRODUCTO_INVENTARIO_FISICO_INDIVIDUAL]
@idProducto int,
@idUbicacion int,
@idUsuario int,
@cantidadEnFisico float,
@errorHumano int =0
as
	begin -- procedimiento
		begin try -- try principal
				BEGIN-- DECLARACIONES
					DECLARE	
					    @status int = 200,
						@mensaje varchar(255) = '',
						@cantidadAAjustar float,	
						@cantidadActualSistema float,
						@tipoMovimientoInventario int = 13,
						@idInventarioFisico int

						declare	@tran_name varchar(32) = 'AFECTA INVENTARIO FISICO INDIVIDUAL',
						@tran_count int = @@trancount,
						@tran_scope bit = 0
				END

				--VALIDACIONES
					IF(@idUbicacion IN (select idUbicacion from Ubicacion where idPasillo in (1000,1001,1002,1003,1004) and idPiso in (1000,1001,1002,1003,1004)))
					BEGIN
						SELECT @status=-1,@mensaje='No es posible ajustar el inventario, el producto se encuentra en un proceso interno'
						RETURN
					END
				

				--FIN VALIDACIONES
				begin -- inicio

					if @tran_count = 0
						begin tran @tran_name
					else
						save tran @tran_name
				
					select @tran_scope = 1
				
				end -- inicio



				--OBTENEMOS LA CANTIDAD ACTUAL DEL SISTEMA
				select @cantidadActualSistema = cantidad from InventarioDetalle where idUbicacion = @idUbicacion  and idProducto = @idProducto 

				--CALCULAMOS LA CANTIDAD A AJUSTAR
				SET @cantidadAAjustar = ABS(coalesce(@cantidadActualSistema,0) - @cantidadEnFisico)

				--OBTENEMOS EL TIPO DE MOVIMIENTO YA QUE LA AFECTACION PUEDE SER INCREMENTO O DECREMENTO 
				IF @cantidadActualSistema>@cantidadEnFisico 
				BEGIN
					SET @tipoMovimientoInventario = 14
				END

				--insertamos el inventario fisico en caso de que no exista alguno del dia
				IF NOT EXISTS(select 1 from InventarioFisico where cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idTipoInventarioFisico=2)
				begin
				    
					DECLARE @idSucursal int;
					select @idSucursal=idSucursal from Usuarios where idUsuario=@idUsuario;
				   
					INSERT INTO InventarioFisico(nombre,idUsuario,idSucursal,fechaAlta,fechaInicio,FechaFin,observaciones,idEstatusInventarioFisico,idTipoInventarioFisico)
					VALUES('Inventario Fisico Individual',@idUsuario,@idSucursal,dbo.FechaActual(),dbo.FechaActual(),dbo.FechaActual(),'',3,2);
				end				
				
				select @idInventarioFisico=idInventarioFisico from InventarioFisico where cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idTipoInventarioFisico=2

				update InventarioFisico set FechaFin=dbo.FechaActual() where idInventarioFisico=@idInventarioFisico

				/*IF EXISTS (SELECT 1 FROM AjusteInventarioFisico WHERE  idUbicacion = @idUbicacion  AND idProducto =@idProducto and idInventarioFisico=@idInventarioFisico)
				BEGIN
					

						UPDATE AjusteInventarioFisico SET
						idUsuario  = @idUsuario,
						fechaAlta = dbo.FechaActual(),
						ajustado = 1,
						cantidadAAjustar = dbo.redondear(@cantidadAAjustar),
						cantidadEnFisico =  dbo.redondear(@cantidadEnFisico)						
						wHERE  idUbicacion = @idUbicacion  AND idProducto =@idProducto and idInventarioFisico=@idInventarioFisico
				END
				ELSE
				BEGIN*/
					INSERT INTO AjusteInventarioFisico
												(
												idInventarioFisico
												,idProducto
												,idUbicacion
												,idUsuario
												,cantidadActual
												,cantidadEnFisico
												,cantidadAAjustar
												,fechaAlta
												,ajustado
												,errorHumano
												)
										VALUES
											  (
											    @idInventarioFisico,
												@idProducto,
												@idUbicacion,
												@idUsuario,
												 dbo.redondear(@cantidadActualSistema),
												 dbo.redondear(@cantidadEnFisico),
												 dbo.redondear(@cantidadAAjustar),
												dbo.FechaActual()
												,1
												,@errorHumano
											  )
				--END

				INSERT INTO InventarioDetalleLog(idUbicacion,idProducto,cantidad,cantidadActual,idTipoMovInventario,idUsuario,fechaAlta,idVenta,idPedidoInterno,idInventarioFisico)
				SELECT @idUbicacion,@idProducto,dbo.redondear(@cantidadAAjustar),dbo.redondear(@cantidadEnFisico),@tipoMovimientoInventario,@idUsuario,dbo.FechaActual(),0,0,@idInventarioFisico

				UPDATE InventarioDetalle set cantidad=dbo.redondear(@cantidadEnFisico),fechaActualizacion=dbo.FechaActual() 
				where idProducto=@idProducto and idUbicacion=@idUbicacion

				UPDATE g set cantidad=d.cantidad,fechaUltimaActualizacion=dbo.FechaActual()
				from InventarioGeneral g
				join (select sum(cantidad) cantidad,idProducto from InventarioDetalle group by idProducto) d on g.idProducto=d.idProducto
				where g.idProducto=@idProducto
		
		        begin -- commit
				
					if @tran_count = 0
					
						begin -- si la transacción se inició dentro de este ámbito
						
							commit tran @tran_name
							select @tran_scope = 0
						
						end -- si la transacción se inició dentro de este ámbito
				
				end -- commit

				SELECT @status=200,@mensaje='El inventario del producto ha sido ajustado de manera correcta'
		end try -- fin del try 
		begin catch
			SELECT @status=-1, @mensaje=error_message();
			-- revertir transacción si es necesario
			if @tran_scope = 1
				rollback tran @tran_name
		end catch

		select	@status Estatus,					
				@mensaje Mensaje

	end -- fin de procedimiento