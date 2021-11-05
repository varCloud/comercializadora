IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_OBTENER_BALCANCE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES')
DROP PROCEDURE SP_OBTENER_BALCANCE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Obtener cargos y abonos de las cuentas por cobrar
status			200 = ok
				-1	= error
*/

CREATE proc [dbo].[SP_OBTENER_BALCANCE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES]
@idCliente int=null
as

	begin -- principal
	
		begin try

			begin --declaraciones 

				declare @status					int = 200,
						@mensaje				varchar(255) = '',
						@error_line				varchar(255) = '',
						@error_procedure		varchar(255) = ''
			end  --declaraciones 

			begin -- principal
				
			
				select idPedidoEspecial,CAST(fechaAlta AS DATE) fecha,SaldoInicial cargo,saldoActual
				into #CARGOS
				from PedidosEspecialesCuentasPorCobrar where saldoActual>0

				select a.idPedidoEspecial,CAST(a.fechaAlta AS DATE) fecha,SUM(a.monto) abono
				INTO #ABONOS
				from 
				PedidosEspecialesAbonosCuentasPorCobrar a
				join PedidosEspecialesCuentasPorCobrar b on a.idCuentaPorCobrar=b.idCuentaPorCobrar where saldoActual>0
				group by a.idPedidoEspecial,CAST(a.fechaAlta AS DATE)

				CREATE TABLE #BALANCE_CTAS(
				fecha DATE,
				idPedidoEspecial BIGINT,
				cargo float,
				abono float,
				entregado_por VARCHAR(200),
				saldoCliente float
				)

				INSERT INTO #BALANCE_CTAS(fecha,idPedidoEspecial,cargo,abono,entregado_por,saldoCliente)

				SELECT fecha,idPedidoEspecial, 0 cargo,0 abono,'' entregado_por, 0 saldoCliente				
				FROM (
				SELECT idPedidoEspecial,fecha
				from #CARGOS
				UNION 
				SELECT idPedidoEspecial,fecha
				from #ABONOS) a
				GROUP BY a.idPedidoEspecial,a.fecha

				update a set cargo=c.cargo,saldoCliente=c.saldoActual 
				from #BALANCE_CTAS a
				join #CARGOS c on a.idPedidoEspecial=c.idPedidoEspecial and a.fecha=c.fecha

				update a set abono=c.abono from #BALANCE_CTAS a
				join #ABONOS c on a.idPedidoEspecial=c.idPedidoEspecial and a.fecha=c.fecha

					
				if not exists (select *  from #BALANCE_CTAS)
				begin
					select	
							@status = -1,
							@mensaje = 'No se encontraron cuentas por cobrar'
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
				select  CONVERT(VARCHAR(10),fecha,103) fecha,idPedidoEspecial,cargo,abono,entregado_por, saldoCliente
				from #BALANCE_CTAS

			DROP TABLE #CARGOS,#ABONOS,#BALANCE_CTAS
					
		end -- reporte de estatus
		

	end  -- principal
