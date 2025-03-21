--USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_REALIZA_ABONO_PEDIDOS_ESPECIALES]    Script Date: 09/02/2022 22:41:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/10/13
Objetivo		Realiza abono pedidos especiales
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_REALIZA_ABONO_PEDIDOS_ESPECIALES]
@idCliente int ,
@idUsuario int,
@monto money,
@montoIVA money null,
@montoComision money null,
@requiereFactura bit null,
@idFactFormaPago int null,
@idFactUsoCFDI int null,
@idPedidoEspecial bigint null,
@montoRecibido money null
AS
BEGIN
	BEGIN TRY			


			declare  	@tran_name varchar(32) = 'REALIZA_ABONO_ESPECIAL',
						@tran_count int = @@trancount,
						@tran_scope bit = 0
			
			if not exists (select 1 from PedidosEspecialesIngresosEfectivo where idUsuario = @idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idTipoIngreso=1)
			begin				
				raiserror ('Por poder realizar un abono, se requiere que se realize la apertura de cajas.', 11, -1)
			end

			if exists (select 1 from PedidosEspecialesCierres where idUsuario = @idUsuario and cast(fechaAlta as date)=cast(dbo.FechaActual() as date) and idEstatusRetiro in (1,2)) 
			begin
				raiserror ('No se puede realizar el abono, ya que existe un cierre de cajas de hoy', 11, -1)
			end	
			
			if not exists(select 1 from PedidosEspecialesCuentasPorCobrar where idCliente=@idCliente)
				RAISERROR('El cliente no tiene adeudos', 15, 217)			

			if (select sum(saldoActual) from PedidosEspecialesCuentasPorCobrar where idCliente=@idCliente and idPedidoEspecial=coalesce(@idPedidoEspecial,idPedidoEspecial))<@monto
			begin
				RAISERROR('El monto a abonar es mayor que el monto adeudado', 15, 217)				
			end
			
						
			if @tran_count = 0
				begin tran @tran_name
			else
				save tran @tran_name
				
			select @tran_scope = 1

			--BEGIN TRAN 
				--OBTENEMOS LA FECHA MAS QUE NADA LA HORA ACTUAL DE NUESTRA ZONA HORARIA

				BEGIN-- DECLARACIONES

					DECLARE								 
					@fechaActual date,
					@montoPagado money,
					@idCuentaPorCobrar bigint,
					--@idPedidoEspecial bigint,
					@saldoActual money,
					@saldoRestante money,
					@montoTotal	money,
					@idAbonoCliente bigint,
					@saldoInicial money,
					@saldoAntesOperacion money,
					@saldoDespuesOperacion money
					
				END	

				select @fechaActual=dbo.FechaActual()

				select 
				@saldoInicial=sum(saldoInicial),
				@saldoAntesOperacion=sum(saldoActual),
				@saldoDespuesOperacion=dbo.redondear(sum(saldoActual)-@monto) 
				from PedidosEspecialesCuentasPorCobrar where saldoActual>0  and idCliente=@idCliente

				--insertamos en la tabla PedidosEspecialesAbonoClientes
				select @montoTotal=DBO.redondear(@monto + @montoIVA + @montoComision);

				INSERT INTO PedidosEspecialesAbonoClientes(idUsuario,monto,montoIva,montoComision,montoTotal,idCliente,requiereFactura,idFactFormaPago,idFactUsoCFDI,fechaAlta,activo,saldoInicial,saldoAntesOperacion,saldoDespuesOperacion,montoPagado)
				VALUES(@idUsuario,@monto,@montoIVA,@montoComision,@montoTotal,@idCliente,@requiereFactura,@idFactFormaPago,@idFactUsoCFDI,dbo.FechaActual(),1,@saldoInicial,@saldoAntesOperacion,@saldoDespuesOperacion,@montoRecibido)

				select @idAbonoCliente=max(idAbonoCliente) from PedidosEspecialesAbonoClientes where idCliente=@idCliente

				if(coalesce(@idPedidoEspecial,0)>0)
				begin

				    select  @idCuentaPorCobrar=idCuentaPorCobrar,@idPedidoEspecial=idPedidoEspecial,@saldoActual=saldoActual			
					from [dbo].[PedidosEspecialesCuentasPorCobrar]  where idCliente=@idCliente and saldoActual>0 and idPedidoEspecial=@idPedidoEspecial
					order by idCuentaPorCobrar

					--afectamos el saldo actual de la tabla PedidosEspecialesCuentasPorCobrar
					UPDATE PedidosEspecialesCuentasPorCobrar SET saldoActual=dbo.redondear(saldoActual-@monto) 
					where idCuentaPorCobrar=@idCuentaPorCobrar

					--insertamos el abono en la tabla PedidosEspecialesAbonosCuentasPorCobrar
					INSERT INTO PedidosEspecialesAbonosCuentasPorCobrar(monto,fechaAlta,idCliente,idUsuario,idPedidoEspecial,idCuentaPorCobrar,idAbonoCliente,SaldoDespuesOperacion)
					values(@monto,dbo.FechaActual(),@idCliente,@idUsuario,@idPedidoEspecial,@idCuentaPorCobrar,@idAbonoCliente,dbo.redondear(@saldoActual-@monto))

					if exists(select 1 from [PedidosEspecialesCuentasPorCobrar] where idPedidoEspecial=@idPedidoEspecial  and saldoActual=0)
					   update PedidosEspeciales set liquidado=1 where idPedidoEspecial=@idPedidoEspecial

				end
				else
				begin
					while(@monto>0)
					begin

						--consultamos el primer pedido especial
						select top 1 @idCuentaPorCobrar=idCuentaPorCobrar,@idPedidoEspecial=idPedidoEspecial,@saldoActual=saldoActual			
						from [dbo].[PedidosEspecialesCuentasPorCobrar]  where idCliente=@idCliente and saldoActual>0
						order by idCuentaPorCobrar

						if(@saldoActual>@monto)
						begin
						select @montoPagado=@monto
						end
						else
						begin
						select @montoPagado=@saldoActual
						end

						--afectamos el saldo actual de la tabla PedidosEspecialesCuentasPorCobrar
						UPDATE PedidosEspecialesCuentasPorCobrar SET saldoActual=dbo.redondear(saldoActual-@montoPagado) 
						where idCuentaPorCobrar=@idCuentaPorCobrar

						--insertamos el abono en la tabla PedidosEspecialesAbonosCuentasPorCobrar
						INSERT INTO PedidosEspecialesAbonosCuentasPorCobrar(monto,fechaAlta,idCliente,idUsuario,idPedidoEspecial,idCuentaPorCobrar,idAbonoCliente,SaldoDespuesOperacion)
						values(@montoPagado,dbo.FechaActual(),@idCliente,@idUsuario,@idPedidoEspecial,@idCuentaPorCobrar,@idAbonoCliente,dbo.redondear(@saldoActual-@montoPagado))

						if exists(select 1 from [PedidosEspecialesCuentasPorCobrar] where idPedidoEspecial=@idPedidoEspecial  and saldoActual=0)
							update PedidosEspeciales set liquidado=1 where idPedidoEspecial=@idPedidoEspecial

						select @monto=@monto-@montoPagado

					end

				end


			
			--VALIDAMOS SI LA TRANSACCION SE GENERO AQUI , AQUIMISMO SE HACE EL COMMIT	
		    if @tran_count = 0	
			begin -- si la transacción se inició dentro de este ámbito
						
				commit tran @tran_name
				select @tran_scope = 0
						
			end -- si la transacción se inició dentro de este ámbito

			select 200 Estatus , 'OK' Mensaje,@idAbonoCliente id 

	END TRY
	BEGIN CATCH
		SELECT -1 Estatus, error_message() Mensaje,error_line() Errorline
		
		if @tran_scope = 1
			rollback tran @tran_name

	END CATCH
	
END

