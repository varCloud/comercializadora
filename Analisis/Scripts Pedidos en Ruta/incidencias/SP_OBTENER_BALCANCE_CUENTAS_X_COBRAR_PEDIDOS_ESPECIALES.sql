USE [DB_A57E86_lluviadesarrollo]
GO
/****** Object:  StoredProcedure [dbo].[SP_OBTENER_BALCANCE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES]    Script Date: 06/05/2022 09:31:15 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
UsuarioRed		aoaj720209
Fecha			2021/09/15
Objetivo		Obtener cargos y abonos de las cuentas por cobrar
status			200 = ok
				-1	= error
*/

ALTER proc [dbo].[SP_OBTENER_BALCANCE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES]
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
				
			
				CREATE TABLE 
					#BALANCE_CTAS
						(
							fecha DATE,
							idPedidoEspecial BIGINT,
							cargo float,
							abono float,
							entregado_por VARCHAR(200),
							saldoCliente float
						)


				INSERT INTO #BALANCE_CTAS(fecha,idPedidoEspecial,cargo,abono,entregado_por,saldoCliente)
				SELECT a.fecha,a.idPedidoEspecial,a.cargo,a.abono,entregado_por,saldoCliente			
				FROM	(
							select	a.idPedidoEspecial,a.fechaAlta fecha,a.SaldoInicial cargo,ISNULL(b.monto,0) abono,a.saldoActual saldoCliente,
									ISNULL(case when u.idRol=10 then 'Taxi unidad' + p.numeroUnidadTaxi else u.nombre + ' ' + u.apellidoPaterno + ' ' + u.apellidoMaterno end,'') entregado_por
							from	PedidosEspecialesCuentasPorCobrar a
										join PedidosEspeciales p 
											on a.idPedidoEspecial = p.idPedidoEspecial
										left join Usuarios u 
											on p.idUsuarioEntrega = u.idUsuario
										left join PedidosEspecialesAbonosCuentasPorCobrar b 
											on a.idCuentaPorCobrar = b.idCuentaPorCobrar and coalesce(b.EsAbonoInicial,cast(0 as bit))=1
							where	saldoActual > 0 
								and	a.idCliente = coalesce(@idCliente,a.idCliente)
								and	p.idEstatusPedidoEspecial not in (9)  -- no considerar los estatus 9 - pedido en ruta

							UNION

							select a.idPedidoEspecial,a.fechaAlta fecha,0 cargo,a.monto abono,0 saldoCliente,'' entregado_por
							from 
							PedidosEspecialesAbonosCuentasPorCobrar a
							join PedidosEspecialesCuentasPorCobrar b on a.idCuentaPorCobrar=b.idCuentaPorCobrar
							join PedidosEspeciales pe on pe.idPedidoEspecial = a.idPedidoEspecial
							where 
							--saldoActual>0
							/*and*/ coalesce(a.EsAbonoInicial,cast(0 as bit))=0 and a.idCliente=coalesce(@idCliente,a.idCliente)		
							and pe.liquidado = cast(0 as bit)
						) a				

					
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
				from #BALANCE_CTAS order by cast(fecha as date) desc

			DROP TABLE #BALANCE_CTAS
					
		end -- reporte de estatus
		

	end  -- principal
