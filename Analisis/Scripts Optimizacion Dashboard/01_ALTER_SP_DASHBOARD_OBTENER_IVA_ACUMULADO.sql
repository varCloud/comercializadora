/*
Objetivo	Optimizar SP_DASHBOARD_OBTENER_IVA_ACUMULADO (grafico "IVA acumulado" del Dashboard, ~4.7 s en BD local).
			Causa: CAST(columna as date) en el JOIN contra #CATEGORIAS vuelve el predicado NO sargable, por lo que los indices
			index_VentasDetalle_fechaVentaDetalle (3.5M filas) no se usan. Se reescriben SOLO los 2 predicados a rango semiabierto:
				v.fechaVentaDetalle >= c.fechaIni  AND v.fechaVentaDetalle < DATEADD(day,1,c.fechaFin)
				PE.fechaAlta        >= c.fechaIni  AND PE.fechaAlta        < DATEADD(day,1,c.fechaFin)
			Equivalencia: las columnas son datetime y #CATEGORIAS.fechaIni/fechaFin son date, asi que el rango cubre exactamente
			los mismos dias completos (incluye hasta 23:59:59.997 del ultimo dia).
			Sin cambios en parametros, columnas, tipos, orden ni result sets. Misma firma y mismo nombre.
Fecha		2026-11-05
Revertir:	01a_ROLLBACK_SP_DASHBOARD_OBTENER_IVA_ACUMULADO.sql
Validar:	02_VALIDACION_SALIDA_IDENTICA_IVA.sql (despues de aplicar este script; solo BD local/pruebas primero)
Ejecutar con sqlcmd -f 65001 -I (archivo UTF-8) o desde SSMS.
Idempotente: CREATE OR ALTER (se puede ejecutar varias veces).
*/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER proc

	[dbo].[SP_DASHBOARD_OBTENER_IVA_ACUMULADO]
		@idTipoReporte int,--1 semana,2 mes,3 año
		@idEstacion int=null,
		@fechaConsulta datetime = null

as

	begin -- procedimiento
		set LANGUAGE Spanish

		declare
		@fechaIni date ='20210601',
		@fechaFin date = getdate(),
		@fecha date	

		CREATE TABLE #CATEGORIAS(
				id int,
				categoria varchar(100),
				fechaIni date,
				fechaFin date,
				total money, 
				totalPE money default(0)
				)


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
		 		 
		UPDATE c SET total=coalesce(total.montoTotal,0) FROM #CATEGORIAS c
		left join ( 
					SELECT c.id,SUM(coalesce(montoIva,0)) montoTotal
					FROM ventasDetalle v
					join #CATEGORIAS c on v.fechaVentaDetalle >= c.fechaIni and v.fechaVentaDetalle < DATEADD(day,1,c.fechaFin)
					group by  c.id,c.categoria
				  ) total on c.id=total.id

		UPDATE c SET totalPE=coalesce(total.montoTotal,0) FROM #CATEGORIAS c
		left join ( 
					SELECT c.id,SUM(coalesce(PE.montoIva,0)) montoTotal
					FROM PedidosEspecialesDetalle PE
					join #CATEGORIAS c on PE.fechaAlta >= c.fechaIni and PE.fechaAlta < DATEADD(day,1,c.fechaFin)
					group by  c.id,c.categoria
				  ) total on c.id=total.id
																					
		select 200 status , 'OK' mensaje
		SELECT * from #CATEGORIAS  order by id
		DROP TABLE #CATEGORIAS
		
	end
GO
