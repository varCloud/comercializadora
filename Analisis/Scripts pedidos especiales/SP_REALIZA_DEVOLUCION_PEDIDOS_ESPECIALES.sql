IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_REALIZA_DEVOLUCION_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_REALIZA_DEVOLUCION_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/10/13
Objetivo		Realiza devoluciones pedidos especiales
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_REALIZA_DEVOLUCION_PEDIDOS_ESPECIALES]
@motivoDevolucion varchar(250),
@idPedidoEspecial bigint,
@montoDevuelto money,
@idUsuario int,
@xmlProductos xml
AS
BEGIN
	BEGIN TRY	

			declare  	@tran_name varchar(32) = 'DEVOLUCION_PEDIDO_ESPECIAL',
						@tran_count int = @@trancount,
						@tran_scope bit = 0,
						@idTipoTicketPedidoEspecial int=2, --ticket devolucion
						@fechaActual datetime=dbo.fechaActual();

			IF OBJECT_ID('tempdb..#ProductosRecibidos') IS NOT NULL
				DROP TABLE #ProductosRecibidos
			create table #ProductosRecibidos
			(	
						indice int identity(1,1),
						idPedidoEspecialDetalle int,			
						idProducto int ,			
						productosDevueltos float,
						cantidad float null,
						montoDevuelto money null,
						comisionBancariaDevuelto money null
			)

			INSERT INTO  #ProductosRecibidos 
						(
						idPedidoEspecialDetalle		
						,idProducto			
						,productosDevueltos
						)
			SELECT  
					P.value('idPedidoEspecialDetalle[1]', 'INT') AS idPedidoEspecialDetalle,		
					P.value('idProducto[1]', 'INT') AS idProducto,
					P.value('productosDevueltos[1]', 'float') AS productosDevueltos						
			FROM  @xmlProductos.nodes('//ArrayOfProductosDevueltosPedidoEspecial/ProductosDevueltosPedidoEspecial') AS x(P)

			delete #ProductosRecibidos where coalesce(productosDevueltos,0)=0

			
			if not exists(select 1 from PedidosEspeciales where idPedidoEspecial=@idPedidoEspecial)
				select 'El pedido especial no existe'

			if exists ( select 1 from FacturasPedidosEspeciales where idPedidoEspecial = @idPedidoEspecial and idEstatusFactura = 1 )
			begin
				select 'No puede hacer una devolucion en un pedido especial facturado.'
			end

			if not exists (select 1 from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial and idEstatusPedidoEspecial in (4,6) )
			begin
				declare @estatus varchar(100)
				select @estatus=e.descripcion from PedidosEspeciales p join CatEstatusPedidoEspecial e on p.idEstatusPedidoEspecial=e.idEstatusPedidoEspecial where idPedidoEspecial=@idPedidoEspecial
				select 'No puede hacer una devolucion en un pedido especial ' + @estatus
			end

			if exists (select 1
				from #ProductosRecibidos PR   left  join   PedidosEspecialesDetalle PD 
				on PR.idPedidoEspecialDetalle = PD.idPedidoEspecialDetalle and PD.idPedidoEspecial=@idPedidoEspecial
				where PD.idPedidoEspecialDetalle is null)
			begin
				select 'El pedido contiene productos que no corresponden al pedido especial'				
			end		
			
			if @tran_count = 0
				begin tran @tran_name
			else
				save tran @tran_name
				
			select @tran_scope = 1

				update a set 
				cantidad=b.cantidad,
				montoDevuelto=dbo.redondear(precioVenta*a.productosDevueltos),
				comisionBancariaDevuelto=case when coalesce(b.montoComisionBancaria,0)>0 then dbo.redondear(a.productosDevueltos*(b.montoComisionBancaria/b.cantidad)) else 0  end
				from #ProductosRecibidos a join PedidosEspecialesDetalle b on a.idPedidoEspecialDetalle=b.idPedidoEspecialDetalle
				where idPedidoEspecial=@idPedidoEspecial


				if exists ( select 1 from #ProductosRecibidos where (cantidad - productosDevueltos) < 0 )
				begin
					select 'No puede regresar mas productos de los que se vendieron.'	
				end

				BEGIN-- DECLARACIONES

					declare 
					@cantidad float,
					@monto money,
					@comisionBancaria money,
					@montoTotal money,
					@idTicketPedidoEspecial bigint,
					@idPedidoEspecialDetalle bigint,
					@idProducto bigint,
					@cantidadActualInvGeneral float,
					@idAlmacen int,
					@idTipoMovInventario int=22,
					@cantidadDevuelta float,
					@idUbicacion int;
			
				END	

				select 
				@cantidad=sum(productosDevueltos),
				@monto=sum(montoDevuelto),
				@comisionBancaria=sum(comisionBancariaDevuelto)
				from #ProductosRecibidos

				select @montoTotal=@monto+@comisionBancaria

				if(@montoDevuelto!=@monto)
				begin
				 select 'El monto total no coincide con lo que se le cobro al cliente ' + cast(@montoDevuelto as varchar) + ' monto total: ' + cast(@montoTotal as varchar)							
				end

				select @idAlmacen=idAlmacen from Usuarios where idUsuario=@idUsuario


				INSERT INTO ticketspedidosespeciales (idTipoTicketPedidoEspecial,idPedidoEspecial,idUsuario,cantidad,monto,comisionBancaria,montoIVA,montoTotal,fechaAlta,observaciones)
				select @idTipoTicketPedidoEspecial,@idPedidoEspecial,@idUsuario,@cantidad,@monto,@comisionBancaria,0,@montoTotal,@fechaActual,@motivoDevolucion

				select @idTicketPedidoEspecial=max(idTicketPedidoEspecial) from TicketsPedidosEspeciales where idPedidoEspecial=@idPedidoEspecial and idTipoTicketPedidoEspecial=@idTipoTicketPedidoEspecial

				while exists(select 1 from #ProductosRecibidos)
				begin

					select @idPedidoEspecialDetalle=min(idPedidoEspecialDetalle) from #ProductosRecibidos

					select @cantidadDevuelta=productosDevueltos from #ProductosRecibidos where idPedidoEspecialDetalle=@idPedidoEspecialDetalle

					select @idProducto=idProducto 
					from PedidosEspecialesDetalle where idPedidoEspecialDetalle=@idPedidoEspecialDetalle

					select @cantidadActualInvGeneral=cantidad from InventarioGeneral where idProducto=@idProducto

					insert into InventarioGeneralLog (idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
					select @idProducto,@cantidadDevuelta,dbo.redondear(@cantidadActualInvGeneral+@cantidadDevuelta),@fechaActual,@idTipoMovInventario

					update InventarioGeneral set cantidad=dbo.redondear(cantidad+@cantidadDevuelta) where idProducto=@idProducto


					if not exists(select 1 from Ubicacion where idAlmacen=@idAlmacen and idPasillo = 0 and idRaq = 0 and idPiso = 0)
					begin
						insert into Ubicacion (idAlmacen, idPasillo, idRaq, idPiso)
						select @idAlmacen, 0,0,0	
					end

					select @idUbicacion=idUbicacion from Ubicacion where idAlmacen=@idAlmacen and idPasillo = 0 and idRaq = 0 and idPiso = 0

					insert into InventarioDetalleLog(idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idPedidoEspecial)
					select @idUbicacion,@idProducto,@cantidadDevuelta,dbo.redondear(cantidad+@cantidadDevuelta),@idTipoMovInventario,@idUsuario,@fechaActual,@idPedidoEspecial 
					from InventarioDetalle where idProducto=@idProducto and idUbicacion=@idUbicacion


					update InventarioDetalle set cantidad=dbo.redondear(cantidad+@cantidadDevuelta) where idProducto=@idProducto and idUbicacion=@idUbicacion

					update a set 
					cantidad=dbo.redondear(a.cantidad-@cantidadDevuelta),
					monto=dbo.redondear(dbo.redondear(a.cantidad-@cantidadDevuelta) * a.precioVenta),
					montoComisionBancaria=dbo.redondear(dbo.redondear(a.cantidad-@cantidadDevuelta)*(a.montoComisionBancaria/a.cantidad)),
					cantidadActualInvGeneral=dbo.redondear(@cantidadActualInvGeneral+@cantidadDevuelta),
					cantidadAnteriorInvGeneral=@cantidadActualInvGeneral
					from PedidosEspecialesDetalle a join #ProductosRecibidos b on a.idPedidoEspecialDetalle=b.idPedidoEspecialDetalle
					where a.idPedidoEspecialDetalle=@idPedidoEspecialDetalle

					insert into TicketsPedidosEspecialesDetalle(idTicketPedidoEspecial,idPedidoEspecial,idPedidoEspecialDetalle,idProducto,cantidad,monto,montoComision,montoTotal,precioVenta,precioIndividual,precioMenudeo,precioRango,cantidadActualInvGeneral,cantidadAnteriorInvGeneral,fechaAlta)
					select @idTicketPedidoEspecial,@idPedidoEspecial,@idPedidoEspecialDetalle,@idProducto,@cantidadDevuelta,montoDevuelto,comisionBancariaDevuelto,dbo.redondear(montoDevuelto+comisionBancariaDevuelto),e.precioVenta,e.precioIndividual,e.precioMenudeo,e.precioRango,e.cantidadActualInvGeneral,e.cantidadAnteriorInvGeneral,@fechaActual
					from #ProductosRecibidos p 
					join PedidosEspecialesDetalle e on p.idPedidoEspecialDetalle=e.idPedidoEspecialDetalle
					where e.idPedidoEspecialDetalle=@idPedidoEspecialDetalle

					delete #ProductosRecibidos where idPedidoEspecialDetalle=@idPedidoEspecialDetalle

				end

				--afectamos la tabla de pedidos especiales
				update p
					set montoTotal=d.montoTotal,cantidad=d.cantidad
				from PedidosEspeciales p
				join (select idPedidoEspecial,sum(cantidad) cantidad,Round(sum(coalesce(monto,0)+coalesce(montoIva,0)+coalesce(montoComisionBancaria,0)),2,0) montoTotal  from PedidosEspecialesDetalle group by idPedidoEspecial) d
				on p.idPedidoEspecial=d.idPedidoEspecial
				where p.idPedidoEspecial=@idPedidoEspecial



				--afectamos la tabla de pedidoespeciales monto y cantidad
				 
				
			--VALIDAMOS SI LA TRANSACCION SE GENERO AQUI , AQUIMISMO SE HACE EL COMMIT	
		    if @tran_count = 0	
			begin -- si la transacción se inició dentro de este ámbito
						
				commit tran @tran_name
				select @tran_scope = 0
						
			end -- si la transacción se inició dentro de este ámbito

			select 200 Estatus , 'Se ha realizado la devolucion de manera correcta' Mensaje 
			DROP TABLE #ProductosRecibidos

	END TRY
	BEGIN CATCH
		SELECT -1 Estatus, error_message() Mensaje,error_line() Errorline
		
		if @tran_scope = 1
			rollback tran @tran_name

	END CATCH
	
END

