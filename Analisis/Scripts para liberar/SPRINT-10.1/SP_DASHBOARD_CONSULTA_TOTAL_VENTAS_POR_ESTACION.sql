
GO
/****** Object:  StoredProcedure [dbo].[SP_DASHBOARD_CONSULTA_TOTAL_VENTAS_POR_ESTACION]    Script Date: 16/08/2021 21:18:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*

Autor			Jessica Almonte Acosta
Fecha			2020/05/14
Objetivo		Consultar el total de ventas por estacion
*/

ALTER proc

	[dbo].[SP_DASHBOARD_CONSULTA_TOTAL_VENTAS_POR_ESTACION]

	@fechaIni date=null,
	@fechaFin date=null,
	@idEstacion int=null

as

	begin -- procedimiento
		
		begin try -- try principal

	
			begin -- inicio

				-- declaraciones
				declare @status int = 200,
						@error_message varchar(255) = '',
						@error_line varchar(255) = '',
						@error_severity varchar(255) = '',
						@error_procedure varchar(255) = '',
						@valido	bit = cast(1 as bit),
						@fecha date,
						@primerDiaSemana date,						
						@ultimoDiaSemana date,
						@primerDiaAnio date,						
						@ultimoDiaAnio date,
						@primerDiaMes date,						
						@ultimoDiaMes date
											
			end -- inicio

			begin

			select @fecha=dbo.FechaActual()

			select @primerDiaSemana=DATEADD(wk,DATEDIFF(wk,0,@fecha),-1);

			select @ultimoDiaSemana= dateadd(day, 6,@primerDiaSemana);

			select @primerDiaAnio= DATEADD(yy, DATEDIFF(yy, 0, GETDATE()), 0);

			select @ultimoDiaAnio= DATEADD(yy, DATEDIFF(yy, 0, GETDATE()) + 1, -1);

			select @primerDiaMes=DATEADD(D,-(DAY(GETDATE()-1)),GETDATE())
			select @ultimoDiaMes=DATEADD(D,-(DAY(DATEADD(M,1,GETDATE()))),DATEADD(M,1,GETDATE()))

			create table #VENTAS_ANUAL(
			idEstacion int,
			montoTotal money,
			fechaAlta date
			)

			CREATE TABLE #VENTAS_ESTACIONES(
			idEstacion int,
			nombre varchar(500),
			montoTotalDia money,
			montoTotalSemana money,
			montoTotalMes money,
			montoTotalAnio money,
			nombreAlmacen varchar(500)
			)					

			end
					    
			begin
			    
				--consultamos las ventas de una fecha en especifica por estacion
			    IF(@fechaIni is not null and @fechaFin is not null)
				begin

					if (OBJECT_ID('tempdb.dbo.#TotalVentasXEstacion','U')) is not null
					drop table #TotalVentasXEstacion

					CREATE TABLE #TotalVentasXEstacion (idEstacion int not null ,
					nombre varchar(200),
					monto money null)	

				    insert into #TotalVentasXEstacion 
					select * from obtnerMontoVentasXEstacion(cast(@fechaIni as date),cast(@fechaFin as date),@idEstacion)

					INSERT INTO #VENTAS_ESTACIONES
					select v.idEstacion,v.nombre,
					v.monto montoTotalDia,
					0 montoTotalSemana,
					0 montoTotalMes,
					0 montoTotalAnio,
					a.Descripcion nombreAlmacen				 
					FROM estaciones e					 
					join Almacenes a on e.idAlmacen=a.idAlmacen	
					left join #TotalVentasXEstacion v on e.idEstacion=v.idEstacion
					

				end
				else
				begin

				--ventas del dia
				if (OBJECT_ID('tempdb.dbo.#TotalVentasXEstacionDia','U')) is not null
				drop table #TotalVentasXEstacionDia

				CREATE TABLE #TotalVentasXEstacionDia (idEstacion int not null ,
				nombre varchar(200),
				monto money null)	

				insert into #TotalVentasXEstacionDia 
				select * from obtnerMontoVentasXEstacion(cast(@fecha as date),cast(@fecha as date),@idEstacion)

				--ventas semanal
				if (OBJECT_ID('tempdb.dbo.#TotalVentasXEstacionSemanal','U')) is not null
				drop table #TotalVentasXEstacionSemanal

				CREATE TABLE #TotalVentasXEstacionSemanal (idEstacion int not null ,
				nombre varchar(200),
				monto money null)	

				insert into #TotalVentasXEstacionSemanal 
				select * from obtnerMontoVentasXEstacion(cast(@primerDiaSemana as date),cast(@ultimoDiaSemana as date),@idEstacion)

				--ventas mensual
				if (OBJECT_ID('tempdb.dbo.#TotalVentasXEstacionMensual','U')) is not null
				drop table #TotalVentasXEstacionMensual

				CREATE TABLE #TotalVentasXEstacionMensual (idEstacion int not null ,
				nombre varchar(200),
				monto money null)	

				insert into #TotalVentasXEstacionMensual 
				select * from obtnerMontoVentasXEstacion(cast(@primerDiaMes as date),cast(@ultimoDiaMes as date),@idEstacion)

				--ventas anual
				if (OBJECT_ID('tempdb.dbo.#TotalVentasXEstacionAnual','U')) is not null
				drop table #TotalVentasXEstacionAnual

				CREATE TABLE #TotalVentasXEstacionAnual (idEstacion int not null ,
				nombre varchar(200),
				monto money null)	

				insert into #TotalVentasXEstacionAnual 
				select * from obtnerMontoVentasXEstacion(cast(@primerDiaAnio as date),cast(@ultimoDiaAnio as date),@idEstacion)

				INSERT INTO #VENTAS_ESTACIONES
			    select e.idEstacion,e.nombre + ' ' + cast(e.numero as varchar) nombre,
				coalesce(vDia.monto,0) montoTotalDia,
				coalesce(vSemana.monto,0) montoTotalSemana,
				coalesce(vMes.monto,0) montoTotalMes,
				coalesce(vAnio.monto,0) montoTotalAnio,
				a.Descripcion nombreAlmacen				 
				FROM Estaciones e
				join Almacenes a on e.idAlmacen=a.idAlmacen
				left join #TotalVentasXEstacionDia vDia on e.idEstacion=vDia.idEstacion
				left join #TotalVentasXEstacionSemanal vSemana on e.idEstacion=vSemana.idEstacion
				left join #TotalVentasXEstacionMensual vMes on e.idEstacion=vMes.idEstacion
				left join #TotalVentasXEstacionAnual vAnio on e.idEstacion=vAnio.idEstacion
				where e.idEstacion=coalesce(@idEstacion,e.idestacion)
				

				end


				if not exists (select 1 from #VENTAS_ESTACIONES)
				begin
					select	@valido = cast(0 as bit),
							@status = -1,
							@error_message = 'No se encontraron estaciones con ventas.'
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

			select * from #VENTAS_ESTACIONES order by nombre

			drop table #VENTAS_ESTACIONES

			end
			
					
		end -- reporte de estatus
		
	end -- procedimiento
	
