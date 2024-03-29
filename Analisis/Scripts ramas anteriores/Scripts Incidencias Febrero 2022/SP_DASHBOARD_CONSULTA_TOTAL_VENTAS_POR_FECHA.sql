--USE [DB_A57E86_comercializadora]
GO
/****** Object:  StoredProcedure [dbo].[SP_DASHBOARD_CONSULTA_TOTAL_VENTAS_POR_FECHA]    Script Date: 07/02/2022 05:11:51 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
Fecha			2020/05/14
Objetivo		Consultar el total de ventas por fecha
*/

ALTER proc

	[dbo].[SP_DASHBOARD_CONSULTA_TOTAL_VENTAS_POR_FECHA]
	
	@idTipoReporte int,--1 semana,2 mes,3 año
	@idEstacion int=null,
	@fechaConsulta datetime = null

as

	begin -- procedimiento
		
		begin try -- try principal

		   set LANGUAGE Spanish
		
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit),
						@fecha date					
			end -- inicio

			--creamos tabla de categorias
			begin

			
			

				CREATE TABLE #CATEGORIAS(
				id int,
				categoria varchar(100),
				fechaIni date,
				fechaFin date,
				total money, 
				totalPE money default(0)
				)

			end		
		    
			begin

			  --categorias por semana
			  if(@idTipoReporte=1)
			  begin

				--obtenemos el primer dia de la semana
				select @fecha=DATEADD(wk,DATEDIFF(wk,0,coalesce( @fechaConsulta , dbo.FechaActual())),-1);

				with dias as
						(
						select cast(1 as int) id, @fecha  fecha
						union all
						select cast((id) + 1 as int),dateadd(day, 1, fecha)
						from   dias
						where  id < 7
						)
				INSERT INTO #CATEGORIAS  
				select id, DATENAME(dw,fecha) Categoria,fecha fechaIni,fecha fechaFin,0,0
				from   dias dia             
				order by id ASC
				option (maxrecursion 7)

			  end

			  --categorias por mes
			  if(@idTipoReporte=2)
			  begin

				--obtenemos el primero dia del año
				select @fecha=DATEADD(yy, DATEDIFF(yy, 0,coalesce( @fechaConsulta , dbo.FechaActual())), 0);

				with meses as
				(
				select cast(1 as int) id, @fecha  fecha
				union all
				select cast((id) + 1 as int),dateadd(mm, 1, fecha)
				from   meses
				where  id < 12
				)
				INSERT INTO #CATEGORIAS
				select id,DATENAME(mm,fecha) Categoria,fecha fechaIni,cast(DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,fecha)+1,0)) as date) fechaFin,0,0
				from   meses mes             
				order by id ASC
				option (maxrecursion 12)

			  end

			  --categorias por año
			  if(@idTipoReporte=3)
			  begin
			    declare @yearIni int=2020 --año inicio
				select  @fecha=DATEADD(year, DATEDIFF(YEAR,0,(DATEADD(YEAR,-9, coalesce( @fechaConsulta , dbo.FechaActual())))), 0);

				with years as
						(
						select cast(1 as int) id, @fecha  fecha
						union all
						select cast((id) + 1 as int),dateadd(year, 1, fecha)
						from   years
						where  id<10
						)
				INSERT INTO #CATEGORIAS
				select id, year(fecha) Categoria,fecha fechaIni,DATEADD(yy, DATEDIFF(yy, 0, fecha) + 1, -1) fechaFin,0,0
				from   years 
				where year(fecha)>=@yearIni           
				order by id ASC
				option (maxrecursion 10)
			  end

				
				--Actualizamos monto total
				--UPDATE c SET total=coalesce(total.montoTotal,0) FROM #CATEGORIAS c
				--left join ( 
				--SELECT c.id,SUM(coalesce(montoTotal,0)) montoTotal
				----UPDATE c set total=SUM(montoTotal) 
				--FROM Ventas v
				--join #CATEGORIAS c on CAST(v.fechaAlta as date)>=c.fechaIni and  CAST(v.fechaAlta as date)<=c.fechaFin
				--where v.idStatusVenta=1 and v.idEstacion=coalesce(@idEstacion,v.idEstacion)
				--group by  c.id,c.categoria) total on c.id=total.id


				update c SET total = coalesce(montoVentas,0) + coalesce(montoComplementos,0)-coalesce(montoDev,0) 
				from  #CATEGORIAS c
				--ventas
				left join (
					select c.id, coalesce(sum((montoTotal)),0) montoVentas
					from Ventas v
					join #CATEGORIAS c on CAST(v.fechaAlta as date)>=c.fechaIni and  CAST(v.fechaAlta as date)<=c.fechaFin
					where  v.idStatusVenta=1 and v.idEstacion=coalesce(@idEstacion,v.idEstacion)
					group by c.id,c.categoria						
					) total on c.id=total.id
				----complementos
				left join (		
						select c.id,coalesce(sum((montoAgregarProductos)),0)montoComplementos
						from Complementos Com
						join #CATEGORIAS c on CAST(Com.fechaAlta as date)>=c.fechaIni and  CAST(Com.fechaAlta as date)<=c.fechaFin
						where idEstacion=coalesce(@idEstacion,idEstacion)
						group by c.id,c.categoria		
					)
				Com on Com.id=c.id
				----devoluciones
				left join (
						select sum(b.monto + coalesce(b.montoDevueltoComisionBancaria,0))  montoDev,c.id	
						from Devoluciones a
						join DevolucionesDetalle b 	on a.idDevolucion=b.idDevolucion
						join #CATEGORIAS c on CAST(a.fechaAlta as date)>=c.fechaIni and  CAST(a.fechaAlta as date)<=c.fechaFin
						where idEstacion=coalesce(@idEstacion,idEstacion)
						group by  c.id,c.categoria	
					) d on d.id=c.id		
				



				--Actualizamos monto de pedidos especiales
				UPDATE c SET totalPE=coalesce(total.montoTotal,0) FROM #CATEGORIAS c
				left join ( 
							SELECT c.id,SUM(coalesce(T.montoTotal,0)) montoTotal
							from TicketsPedidosEspeciales T join PedidosEspeciales P on T.idPedidoEspecial = P.idPedidoEspecial
								join #CATEGORIAS c on CAST(T.fechaAlta as date)>=c.fechaIni and  CAST(T.fechaAlta as date)<=c.fechaFin
							where 
								T.idTipoTicketPedidoEspecial = 1 
								and P.idEstatusPedidoEspecial in(4,6)
								and P.idEstacion=coalesce(@idEstacion,P.idEstacion)
							group by  c.id,c.categoria
							)
				total on c.id=total.id

				--Actualizamos monto de pedidos especiales devueltos
				UPDATE c SET totalPE= totalPE -coalesce(total.montoTotal,0) FROM #CATEGORIAS c
				left join ( 
							SELECT c.id,SUM(coalesce(T.montoTotal,0)) montoTotal
							from TicketsPedidosEspeciales T join PedidosEspeciales P on T.idPedidoEspecial = P.idPedidoEspecial
								join #CATEGORIAS c on CAST(T.fechaAlta as date)>=c.fechaIni and  CAST(T.fechaAlta as date)<=c.fechaFin
							where 
								T.idTipoTicketPedidoEspecial = 2
								and P.idEstatusPedidoEspecial in(4,6)
								and P.idEstacion=coalesce(@idEstacion,P.idEstacion)
							group by  c.id,c.categoria
							)
				total on c.id=total.id


				----Actualizamos monto de abonos de pedidos especiales 
				UPDATE c SET totalPE=(totalPE + coalesce(total.montoTotal,0)) FROM #CATEGORIAS c
				left join ( 
				SELECT c.id,SUM(coalesce(montoTotal,0)) montoTotal
				FROM PedidosEspecialesAbonoClientes v
				join #CATEGORIAS c on CAST(v.fechaAlta as date)>=c.fechaIni and  CAST(v.fechaAlta as date)<=c.fechaFin
				group by  c.id,c.categoria) total on c.id=total.id
				

				if not exists (select 1 from #CATEGORIAS)
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@error_message = 'No se encontraron categorias.'
				end
				

			end
		   

		end try -- try principal
		
		begin catch -- catch principal
		
			-- captura del error
			select	@status = -error_state(),
					@error_procedure = coalesce(error_procedure(), 'CONSULTA DINÁMICA'),
					@error_line = error_line(),
					@error_message = error_message(),
					@error_severity =
						case error_severity()
							when 11 then 'Error en validación'
							when 12 then 'Error en consulta'
							when 13 then 'Error en actualización'
							else 'Error general'
						end
		
		end catch -- catch principal
		
		begin -- reporte de estatus

			select	@status status,
					@error_procedure error_procedure,
					@error_line error_line,
					@error_severity error_severity,
					@error_message mensaje

			IF(@valido=1)
			begin

			select * from #CATEGORIAS order by id

			DROP TABLE #CATEGORIAS

			end
			
					
		end -- reporte de estatus
		
	end -- procedimiento
	
