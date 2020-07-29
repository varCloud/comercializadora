use DB_A57E86_lluviadesarrollo
go

-- se crea procedimiento SP_CONSULTA_COMPLEMENTOS_VENTA
if exists (select 1 from sysobjects where name like 'SP_APP_REGISTRA_PRODUCTO_INVENTARIO_FISICO' and xtype = 'p' and db_name() = 'DB_A57E86_lluviadesarrollo')
	drop proc SP_APP_REGISTRA_PRODUCTO_INVENTARIO_FISICO
go

/*

Autor			VIC
UsuarioRed		auhl373453
Fecha			2020/07/23
Objetivo		Consulta productos agregados/devueltos de una venta
status			200 = ok
				-1	= error
*/

create proc SP_APP_REGISTRA_PRODUCTO_INVENTARIO_FISICO
@idInventarioFisico int ,
@idProducto int,
@idUbicacion int,
@idUsuario int,
@cantidadEnFisico int
as
	begin -- procedimiento
		begin try -- try principal
				BEGIN-- DECLARACIONES
					DECLARE
						@cantidadAAjustar int,	
						@cantidadActualSistema int,
						@tipoMovimientoInventario int = 13
				END

				--OBTENEMOS LA CANTIDAD ACTUAL DEL SISTEM
				select @cantidadActualSistema = cantidad from InventarioDetalle where idUbicacion = @idUbicacion  and idProducto = @idProducto 

				--CALCULAMOS LA CANTIDAD A AJUSTAR
				SET @cantidadAAjustar = coalesce(@cantidadActualSistema,0) - @cantidadEnFisico

				--OBTENEMOS EL TIPO DE MOVIMIENTO YA QUE LA AFECTACION PUEDE SER POSITIVO O NEGATIVO 
				IF @cantidadAAjustar < 0
				BEGIN
					SET @tipoMovimientoInventario = 14
				END

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
												)
										VALUES
											  (
											    @idInventarioFisico,
												@idProducto,
												@idUbicacion,
												@idUsuario,
												@cantidadActualSistema,
												@cantidadEnFisico,
												@cantidadAAjustar,
												dbo.FechaActual()
											  )
				SELECT 200 Estatus , 'OK' Mensaje
		end try -- fin del try 
		begin catch
			SELECT -1 Estatus, error_message() Mensaje,error_line() Errorline
		end catch

	end -- fin de procedimiento