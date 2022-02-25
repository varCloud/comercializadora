--USE [DB_A57E86_comercializadora]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


alter proc

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
					join #CATEGORIAS c on CAST(v.fechaVentaDetalle as date)>=c.fechaIni and  CAST(v.fechaVentaDetalle as date)<=c.fechaFin
					group by  c.id,c.categoria
				  ) total on c.id=total.id

		UPDATE c SET totalPE=coalesce(total.montoTotal,0) FROM #CATEGORIAS c
		left join ( 
					SELECT c.id,SUM(coalesce(PE.montoIva,0)) montoTotal
					FROM PedidosEspecialesDetalle PE
					join #CATEGORIAS c on CAST(PE.fechaAlta as date)>=c.fechaIni and  CAST(PE.fechaAlta as date)<=c.fechaFin
					group by  c.id,c.categoria
				  ) total on c.id=total.id
																					
		select 200 status , 'OK' mensaje
		SELECT * from #CATEGORIAS  order by id
		DROP TABLE #CATEGORIAS
		
	end