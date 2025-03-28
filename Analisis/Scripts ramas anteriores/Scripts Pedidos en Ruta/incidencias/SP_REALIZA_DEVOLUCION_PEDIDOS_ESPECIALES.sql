--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_REALIZA_DEVOLUCION_PEDIDOS_ESPECIALES]    Script Date: 15/05/2022 09:53:56 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/10/13
Objetivo		Realiza devoluciones pedidos especiales
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_REALIZA_DEVOLUCION_PEDIDOS_ESPECIALES]
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
						@fechaActual datetime=dbo.fechaActual(),
						@Mensaje varchar(500);

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

			if not exists (select 1 from PedidosEspecialesIngresosEfectivo where idUsuario = @idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idTipoIngreso=1)
			begin				
				raiserror ('Por poder realizar una devolución, se requiere que se realize la apertura de cajas.', 11, -1)
			end

			if exists (select 1 from PedidosEspecialesCierres where idUsuario = @idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idEstatusRetiro in (1,2)) 
			begin
				raiserror ('No se puede realizar la devolución, ya que existe un cierre de cajas de hoy', 11, -1)
			end	

			
			if not exists(select 1 from PedidosEspeciales where idPedidoEspecial=@idPedidoEspecial)
				raiserror ('El pedido especial no existe', 11, -1)

			if exists ( select 1 from FacturasPedidosEspeciales where idPedidoEspecial = @idPedidoEspecial and idEstatusFactura = 1 )
			begin
				raiserror ('No puede hacer una devolución en un pedido especial facturado.', 11, -1)
			end

			if not exists (select 1 from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial and idEstatusPedidoEspecial in (4,6) )
			begin
				declare @estatus varchar(100)
				select @estatus='No puede hacer una devolución en un pedido especial ' + e.descripcion from PedidosEspeciales p join CatEstatusPedidoEspecial e on p.idEstatusPedidoEspecial=e.idEstatusPedidoEspecial where idPedidoEspecial=@idPedidoEspecial
				raiserror (@estatus, 11, -1)
			end

			if exists (select 1
				from #ProductosRecibidos PR   left  join   PedidosEspecialesDetalle PD 
				on PR.idPedidoEspecialDetalle = PD.idPedidoEspecialDetalle and PD.idPedidoEspecial=@idPedidoEspecial
				where PD.idPedidoEspecialDetalle is null)
			begin
				raiserror ('El pedido contiene productos que no corresponden al pedido especial', 11, -1)				
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
					raiserror ('No puede regresar mas productos de los que se vendieron.', 11, -1)	
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
				
				if(@montoDevuelto!=@montoTotal)
				begin				 
				 select @Mensaje='El monto total no coincide con lo que se le cobro al cliente ' + cast(@montoDevuelto as varchar) + ' monto total: ' + cast(@montoTotal as varchar)							
				 raiserror (@Mensaje, 11, -1)
				end

				--select @idAlmacen=idAlmacen from Usuarios where idUsuario=@idUsuario


				INSERT INTO ticketspedidosespeciales (idTipoTicketPedidoEspecial,idPedidoEspecial,idUsuario,cantidad,monto,comisionBancaria,montoIVA,montoTotal,fechaAlta,observaciones)
				select @idTipoTicketPedidoEspecial,@idPedidoEspecial,@idUsuario,@cantidad,@monto,@comisionBancaria,0,@montoTotal,@fechaActual,@motivoDevolucion

				select @idTicketPedidoEspecial=max(idTicketPedidoEspecial) from TicketsPedidosEspeciales where idPedidoEspecial=@idPedidoEspecial and idTipoTicketPedidoEspecial=@idTipoTicketPedidoEspecial

				while exists(select 1 from #ProductosRecibidos)
				begin

					select @idPedidoEspecialDetalle=min(idPedidoEspecialDetalle) from #ProductosRecibidos

					select @cantidadDevuelta=productosDevueltos from #ProductosRecibidos where idPedidoEspecialDetalle=@idPedidoEspecialDetalle

					select @idProducto=idProducto,@idAlmacen=idAlmacenDestino 
					from PedidosEspecialesDetalle where idPedidoEspecialDetalle=@idPedidoEspecialDetalle

					select @cantidadActualInvGeneral=cantidad from InventarioGeneral where idProducto=@idProducto

					insert into InventarioGeneralLog (idProducto,cantidad,cantidadDespuesDeOperacion,fechaAlta,idTipoMovInventario)
					select @idProducto,@cantidadDevuelta,dbo.redondear(@cantidadActualInvGeneral+@cantidadDevuelta),@fechaActual,@idTipoMovInventario

					update	InventarioGeneral 
					set		cantidad = dbo.redondear(cantidad + @cantidadDevuelta),
							fechaUltimaActualizacion = @fechaActual
					where	idProducto=@idProducto


					if not exists(select 1 from Ubicacion where idAlmacen=@idAlmacen and idPasillo = 1001 and idRaq = 1001 and idPiso = 1001)
					begin
						insert into Ubicacion (idAlmacen, idPasillo, idRaq, idPiso)
						select @idAlmacen, 1001,1001,1001	
					end

					select @idUbicacion=idUbicacion from Ubicacion where idAlmacen=@idAlmacen and idPasillo = 1001 and idRaq = 1001 and idPiso = 1001

					-- si no existe la ubicacion a devolver el producto
					if not exists ( select 1 from InventarioDetalle where idProducto = @idProducto and idUbicacion = @idUbicacion )
						begin
							insert into InventarioDetalle (idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion) 
							select @idProducto as idProducto, cast(0.0 as float) as cantidad, @fechaActual as fechaAlta, @idUbicacion as idUbicacion , @fechaActual as fechaActualizacion
						end					

					insert into InventarioDetalleLog(idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idPedidoEspecial)
					select @idUbicacion,@idProducto,@cantidadDevuelta,dbo.redondear(cantidad+@cantidadDevuelta),@idTipoMovInventario,@idUsuario,@fechaActual,@idPedidoEspecial 
					from InventarioDetalle where idProducto=@idProducto and idUbicacion=@idUbicacion

					if not exists(select 1 from inventarioDetalle where idUbicacion=@idUbicacion and idProducto=@idProducto)
					begin
						insert into InventarioDetalle(idProducto,cantidad,fechaAlta,idUbicacion,fechaActualizacion)
						select @idProducto,@cantidadDevuelta,@fechaActual,@idUbicacion,@fechaActual
					end
					else
					begin
						update InventarioDetalle set cantidad=dbo.redondear(cantidad+@cantidadDevuelta),fechaActualizacion=@fechaActual where idProducto=@idProducto and idUbicacion=@idUbicacion
					end	
					
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
				update	p
				set		montoTotal=d.montoTotal,cantidad=d.cantidad
				from	PedidosEspeciales p
							join (select idPedidoEspecial,sum(cantidad) cantidad,Round(sum(coalesce(monto,0)+coalesce(montoIva,0)+coalesce(montoComisionBancaria,0)),2,0) montoTotal  from PedidosEspecialesDetalle group by idPedidoEspecial) d
								on p.idPedidoEspecial=d.idPedidoEspecial
				where	p.idPedidoEspecial=@idPedidoEspecial

				
				-- si se devolvieron todos los productos del pedido
				if ( ( select montoTotal from PedidosEspeciales where idPedidoEspecial = @idPedidoEspecial ) = 0.0 )
					begin
						update PedidosEspeciales set idEstatusPedidoEspecial = 8  where idPedidoEspecial = @idPedidoEspecial    --8	Cancelado
					end				

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

