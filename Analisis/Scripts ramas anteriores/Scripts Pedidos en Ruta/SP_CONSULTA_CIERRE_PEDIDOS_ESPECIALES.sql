--USE [DB_A57E86_comercializadora]
--GO
/****** Object:  StoredProcedure [dbo].[SP_CONSULTA_CIERRE_PEDIDOS_ESPECIALES]    Script Date: 25/03/2022 02:30:19 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Consultar cierre de pedidos especiales
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_CONSULTA_CIERRE_PEDIDOS_ESPECIALES]
@idUsuario int,
@idEstacion int,
@fecha datetime=null,
@idCierrePedidoEspecial bigint=null

as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = '',
						@idEstatusRetiro		int = 4,		--pendiente;
						@idCategoria			int,
						@nombreCategoria		varchar(250)

			end  --declaraciones 

			begin -- principal

			    --obtenemos el usuario y fecha
				if (@idCierrePedidoEspecial is not null) 
				begin
				    select @idUsuario= idUsuario,@fecha=fechaAlta 
					from PedidosEspecialesCierres where idCierrePedidoEspecial=@idCierrePedidoEspecial
				end

			    select @fecha=coalesce(@fecha,dbo.fechaActual())


				--si existe un registro como pendiente lo eliminamos para volver a hacer el calculo
				if exists(select 1 from PedidosEspecialesCierres where idUsuario=@idUsuario and idEstatusRetiro=@idEstatusRetiro and cast(fechaAlta as date)=cast(@fecha as date))
				begin
					select @idCierrePedidoEspecial=idCierrePedidoEspecial from PedidosEspecialesCierres where idUsuario=@idUsuario and idEstatusRetiro=@idEstatusRetiro and cast(fechaAlta as date)=cast(@fecha as date)
					delete from PedidosEspecialesCierresDetalle where idCierrePedidoEspecial=@idCierrePedidoEspecial
					delete from PedidosEspecialesCierres where idCierrePedidoEspecial=@idCierrePedidoEspecial
				end



				if not exists(select 1 from PedidosEspecialesCierres where idUsuario=@idUsuario and cast(fechaAlta as date)=cast(@fecha as date))
				begin

					INSERT INTO PedidosEspecialesCierres(idUsuario,idEstacion,fechaAlta,idEstatusRetiro)
					select @idUsuario,@idEstacion,@fecha,@idEstatusRetiro


					select @idCierrePedidoEspecial=idCierrePedidoEspecial from PedidosEspecialesCierres where idUsuario=@idUsuario and cast(fechaAlta as date)=cast(@fecha as date)


					--select a.idAlmacen,REPLACE(REPLACE(a.Descripcion,'Tienda',''),'Almacen','') nombre,0 cierre 
					--into #AlmacenesLineaProducto
					--from AlmacenesXLineaProducto l
					--join Almacenes a on l.idAlmacen=a.idAlmacen
					--where l.activo=1 and l.idAlmacen in (3,4,5)
					--group by a.idAlmacen,a.Descripcion

					--declare @idAlmacen int,@nombreAlmacen varchar(250)

					select	idCategoria, descripcion, 0 cierre 
					into	#Categorias
					from	categorias


					while exists(select 1 from #Categorias where cierre = 0)
					begin

						--select @idAlmacen=min(idAlmacen) from #AlmacenesLineaProducto where cierre=0
						--select @nombreAlmacen=nombre from #AlmacenesLineaProducto where idAlmacen=@idAlmacen

						select @idCategoria=min(idCategoria) from #Categorias where cierre=0
						select @nombreCategoria=descripcion from #Categorias where idCategoria=@idCategoria

						--consultamos todos los pedidos realizados 
						select	p.idFactFormaPago,p.idEstatusPedidoEspecial,t.idTipoTicketPedidoEspecial,det.cantidad,det.montoTotal,P.idPedidoEspecial
						into	#productosVendidos
						from	TicketsPedidosEspeciales t
									join PedidosEspeciales p 
										on t.idPedidoEspecial=p.idPedidoEspecial
									join TicketsPedidosEspecialesDetalle det 
										on t.idTicketPedidoEspecial=det.idTicketPedidoEspecial
									join PedidosEspecialesDetalle pdet 
										on det.idPedidoEspecialDetalle=pdet.idPedidoEspecialDetalle
						where	cast(t.fechaAlta as date) = cast(@fecha as date) 
							and t.idUsuario=@idUsuario
							and pdet.idAlmacenDestino in (select idAlmacen from AlmacenesPorCategorias where idCategoria = @idCategoria)
						--and dbo.ExisteProductoEnAlmancen(@idAlmacen,det.idProducto)=1

						--select * from #productosVendidos
						
						
						insert into PedidosEspecialesCierresDetalle(idCierrePedidoEspecial,idAlmacen,descripcion)
						select @idCierrePedidoEspecial,@idCategoria,'Ventas ' + @nombreCategoria

						--ventas de contado
						update PedidosEspecialesCierresDetalle set 
						VentasContado=(select ISNULL(sum(montoTotal),0) from #productosVendidos where idFactFormaPago=1 and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1),
						NoVentasContado=(select ISNULL(sum(cantidad),0) from #productosVendidos where idFactFormaPago=1 and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1)
						where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

						--ventas de tarjeta de credito
						update PedidosEspecialesCierresDetalle set 
						VentasTC=(select ISNULL(sum(montoTotal),0) from #productosVendidos where idFactFormaPago in (4,18) and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1),
						NoVentasTC=(select ISNULL(sum(cantidad),0) from #productosVendidos where idFactFormaPago in (4,18) and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1)
						where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

						--Ventas transferencias bancarias
						update PedidosEspecialesCierresDetalle set 
						VentasTransferencias=(select ISNULL(sum(montoTotal),0) from #productosVendidos where idFactFormaPago in (3) and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1),
						NoVentasTransferencias=(select ISNULL(sum(cantidad),0) from #productosVendidos where idFactFormaPago in (3) and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1)
						where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

						--Ventas otras formas de pago
						update PedidosEspecialesCierresDetalle set 
						VentasOtrasFormasPago=(select ISNULL(sum(montoTotal),0) from #productosVendidos where idFactFormaPago not in (1,4,18,3) and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1),
						NoVentasOtrasFormasPago=(select ISNULL(sum(cantidad),0) from #productosVendidos where idFactFormaPago not in (1,4,18,3) and idEstatusPedidoEspecial in(4,6) and idTipoTicketPedidoEspecial=1)
						where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

						--Ventas crédito
						update PedidosEspecialesCierresDetalle set 
						VentasCredito=(select ISNULL(sum(montoTotal),0) from #productosVendidos where idEstatusPedidoEspecial in(5,7) and idTipoTicketPedidoEspecial=1),
						NoVentasCredito=(select ISNULL(sum(cantidad),0) from #productosVendidos where idEstatusPedidoEspecial in(5,7) and idTipoTicketPedidoEspecial=1)
						where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria
						
						-- Ventas de pedidos especiales en ruta que no han sido liquidados
						update PedidosEspecialesCierresDetalle set 
						MontoPedidosEnRuta=(select ISNULL(sum(montoTotal),0) from #productosVendidos where idEstatusPedidoEspecial in(9) and idTipoTicketPedidoEspecial=3),
						NoPedidosEnRuta=(select ISNULL(sum(cantidad),0) from #productosVendidos where idEstatusPedidoEspecial in(9) and idTipoTicketPedidoEspecial=3)
						where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

						
						--MontoDevoluciones
						update PedidosEspecialesCierresDetalle set 
						MontoDevoluciones=(select ISNULL(sum(montoTotal),0) from #productosVendidos where idTipoTicketPedidoEspecial=2),
						NoDevoluciones=(select ISNULL(sum(cantidad),0) from #productosVendidos where idTipoTicketPedidoEspecial=2)
						where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

						--Total
						update p set 
							TotalEfectivo= dbo.redondear(VentasContado-MontoDevoluciones),
							TotalCreditoTransferencias=(VentasTC + VentasTransferencias + VentasOtrasFormasPago)
						from	PedidosEspecialesCierresDetalle p 
						where	idCierrePedidoEspecial = @idCierrePedidoEspecial 
							and idAlmacen = @idCategoria

						drop table #productosVendidos;

						--update #AlmacenesLineaProducto set cierre=1 where idAlmacen=@idAlmacen
						update #Categorias set cierre=1 where idCategoria=@idCategoria

					end  -- while exists(select 1 from #Categorias where cierre=0)

					--------Abonos

					select @idCategoria=0

					--consultamos todos los abonos
					select idFactFormaPago,montoTotal
					into #abonosPedidosEspeciales
					from [dbo].[PedidosEspecialesAbonoClientes]
					where cast(fechaAlta as date)=cast(@fecha as date) and idUsuario=@idUsuario
					and activo=1

					insert into PedidosEspecialesCierresDetalle(idCierrePedidoEspecial,idAlmacen,descripcion)
					select @idCierrePedidoEspecial,@idCategoria,'Ingresos por pagos crédito'

					--abonos de contado
					update PedidosEspecialesCierresDetalle set 
					VentasContado=(select ISNULL(sum(montoTotal),0) from #abonosPedidosEspeciales where idFactFormaPago=1 ),
					NoVentasContado=(select ISNULL(count(1),0) from #abonosPedidosEspeciales where idFactFormaPago=1 )
					where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

					--abonos de tarjeta de credito
					update PedidosEspecialesCierresDetalle set 
					VentasTC=(select ISNULL(sum(montoTotal),0) from #abonosPedidosEspeciales where idFactFormaPago in (4,18) ),
					NoVentasTC=(select ISNULL(count(1),0) from #abonosPedidosEspeciales where idFactFormaPago in (4,18) )
					where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

					--abonos transferencias bancarias
					update PedidosEspecialesCierresDetalle set 
					VentasTransferencias=(select ISNULL(sum(montoTotal),0) from #abonosPedidosEspeciales where idFactFormaPago in (3)),
					NoVentasTransferencias=(select ISNULL(count(1),0) from #abonosPedidosEspeciales where idFactFormaPago in (3))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

					--abonos otras formas de pago
					update PedidosEspecialesCierresDetalle set 
					VentasOtrasFormasPago=(select ISNULL(sum(montoTotal),0) from #abonosPedidosEspeciales where idFactFormaPago not in (1,4,18,3)),
					NoVentasOtrasFormasPago=(select ISNULL(count(1),0) from #abonosPedidosEspeciales where idFactFormaPago not in (1,4,18,3))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

					--total de ingresos por pagos crédito
					update p set 
						TotalEfectivo= VentasContado-MontoDevoluciones,
						TotalCreditoTransferencias=(VentasTC + VentasTransferencias + VentasOtrasFormasPago)
					from PedidosEspecialesCierresDetalle p where idCierrePedidoEspecial=@idCierrePedidoEspecial and idAlmacen=@idCategoria

					update PedidosEspecialesCierres set 
					MontoIngresosEfectivo=(select ISNULL(sum(monto),0) from PedidosEspecialesIngresosEfectivo where idUsuario=@idUsuario and cast(fechaAlta as date)=cast(@fecha as date))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

					update PedidosEspecialesCierres set 
					MontoRetirosEfectivo=(select ISNULL(sum(montoRetiro),0) from PedidosEspecialesRetirosExcesoEfectivo where idUsuario=@idUsuario and cast(fechaAlta as date)=cast(@fecha as date))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

					update PedidosEspecialesCierres set 
					MontoCierreEfectivo=((select sum(TotalEfectivo) from PedidosEspecialesCierresDetalle where idCierrePedidoEspecial=@idCierrePedidoEspecial)+MontoIngresosEfectivo)-MontoRetirosEfectivo
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

					update PedidosEspecialesCierres set 
					MontoCierreTC=(select sum(TotalCreditoTransferencias) from PedidosEspecialesCierresDetalle where idCierrePedidoEspecial=@idCierrePedidoEspecial)
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

					update PedidosEspecialesCierres set 
					noDevoluciones=(select count(1) from TicketsPedidosEspeciales where idTipoTicketPedidoEspecial=2 and idUsuario=@idUsuario and casT(fechaAlta as date)=cast(@fecha as date))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

					update PedidosEspecialesCierres set 
					NoTicketsEfectivo=(select count(1) from TicketsPedidosEspeciales t join PedidosEspeciales ped on t.idPedidoEspecial=ped.idPedidoEspecial where t.idTipoTicketPedidoEspecial=1 and ped.idFactFormaPago=1 and ped.idEstatusPedidoEspecial in (4,6) and t.idUsuario=@idUsuario and casT(t.fechaAlta as date)=cast(@fecha as date))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

					update PedidosEspecialesCierres set 
					NoTicketsCredito=(select count(1) from TicketsPedidosEspeciales t join PedidosEspeciales ped on t.idPedidoEspecial=ped.idPedidoEspecial where t.idTipoTicketPedidoEspecial=1 and ped.idEstatusPedidoEspecial in(5,7) and t.idUsuario=@idUsuario and casT(t.fechaAlta as date)=cast(@fecha as date))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

					update PedidosEspecialesCierres set 
					NoPedidosEnResguardo=(select count(1) from PedidosEspeciales ped where ped.idEstatusPedidoEspecial=3 and ped.idUsuario=@idUsuario and casT(ped.fechaAlta as date)=cast(@fecha as date))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

					update PedidosEspecialesCierres set 
					NoPedidosEnRuta=(select count(1) from PedidosEspeciales ped where ped.idEstatusPedidoEspecial=9 and ped.idUsuario=@idUsuario and casT(ped.fechaAlta as date)=cast(@fecha as date))
					where idCierrePedidoEspecial=@idCierrePedidoEspecial

				end
				else
				begin
					select @idCierrePedidoEspecial=idCierrePedidoEspecial from PedidosEspecialesCierres where idUsuario=@idUsuario and cast(fechaAlta as date)=cast(@fecha as date)
				end
					

			end -- principal

		end try

		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@mensaje = error_message()
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@mensaje mensaje
           
		    if(@status=200)
				select  
					ped.idCierrePedidoEspecial, 
					CONVERT(VARCHAR(10),ped.fechaAlta,103) + ' ' + CONVERT(VARCHAR(20),ped.fechaAlta,114) fechaAlta,
					CONVERT(VARCHAR(10),ped.fechaAlta,103) fechaTicket,
					CONVERT(VARCHAR(20),ped.fechaAlta,114) horaTicket,
					MontoIngresosEfectivo,
					MontoRetirosEfectivo,
					MontoCierreEfectivo,
					MontoCierreTC,
					EfectivoEntregadoEnCierre,
					ped.noDevoluciones,
					NoTicketsEfectivo,
					NoTicketsCredito,
					NoPedidosEnResguardo,
					ped.NoPedidosEnRuta,
					case when ped.idEstatusRetiro in (1,2) then cast(1 as bit) else cast(0 as bit) end cajaCerrada,
					det.idAlmacen,
					det.descripcion,
					'Saldo ' + replace(det.descripcion,'Ventas','') descripcionSaldo,
					det.VentasContado,
					det.VentasTC,
					det.VentasTransferencias,
					det.VentasOtrasFormasPago,
					det.VentasCredito,
					COALESCE(det.VentasTC,0) + COALESCE(det.VentasTransferencias,0) VentasTDCTransferencias,
					det.MontoDevoluciones,
					det.TotalEfectivo,
					det.TotalCreditoTransferencias,
					us.nombre + ' ' + us.apellidoPaterno + ' ' + us.apellidoMaterno + ' ' nombreUsuario,
					suc.descripcion descripcionSucursal,
					al.descripcion descripcionAlmacen,
					est.nombre + ' ' + cast(est.idEstacion as varchar) nombreEstacion
					from PedidosEspecialesCierres ped
				join PedidosEspecialesCierresDetalle det on ped.idCierrePedidoEspecial=det.idCierrePedidoEspecial
				join Usuarios us on ped.idUsuario=us.idUsuario
				join Almacenes al on us.idAlmacen=al.idAlmacen
				join Estaciones est on ped.idEstacion=est.idEstacion
				join CatSucursales suc on al.idSucursal=suc.idSucursal
				where ped.idCierrePedidoEspecial=@idCierrePedidoEspecial
					
		end -- reporte de estatus
		

	end  -- principal