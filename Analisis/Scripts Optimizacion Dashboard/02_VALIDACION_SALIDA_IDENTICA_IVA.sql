/*
VALIDACION DE SALIDA IDENTICA: SP_DASHBOARD_OBTENER_IVA_ACUMULADO (original vs optimizado)

*** SOLO LECTURA SOBRE LOS DATOS, PERO CREA Y ELIMINA OBJETOS DE APOYO. EJECUTAR PRIMERO EN BD LOCAL/PRUEBAS. ***
Este script:
  1. CREA el objeto SP_DASHBOARD_OBTENER_IVA_ACUMULADO_ORIG con el cuerpo ORIGINAL (el mismo de 01a_ROLLBACK...), y AL FINAL LO ELIMINA (DROP).
  2. CREA procedimientos auxiliares zz_val_ORIG / zz_val_FIX y la tabla global ##VAL_OUT; AL FINAL LOS ELIMINA.
     Los auxiliares se generan a partir de OBJECT_DEFINITION de los SPs a comparar, cambiando SOLO el final (SELECT -> INSERT INTO ##VAL_OUT),
     porque un SP con 2 result sets de distinta forma no se puede capturar con INSERT...EXEC.
  3. No modifica SP_DASHBOARD_OBTENER_IVA_ACUMULADO ni ningun dato.
Ejecutar DESPUES de aplicar 01_ALTER_SP_DASHBOARD_OBTENER_IVA_ACUMULADO.sql (compara _ORIG contra el SP real ya optimizado).
Ejecutar con sqlcmd -f 65001 -I (UTF-8) o completo en una sola sesion de SSMS.

Comprueba:
  A. Texto: ambos SPs son iguales salvo los 2 predicados de fecha (despues de normalizar nombre, espacios en blanco y caracteres no ASCII de comentarios) => mismos parametros, columnas, tipos, orden.
  B. Resultado: por cada juego de parametros, filas de _ORIG vs optimizado con EXCEPT en ambos sentidos (incluye numero de fila => orden) y conteo de filas.
  C. Prueba sintetica de frontera (sin tocar datos): CAST(x as date) BETWEEN ini..fin vs rango semiabierto con valores como 23:59:59.997 y 00:00:00.000.
  D. Informativo: existencia de filas reales cerca de medianoche.
Resultado esperado: linea final 'RESULTADO GLOBAL: PASS'.
Config (ver #cfg abajo): orig/fix = nombres de SP a comparar; create_orig = 1 crea/elimina el _ORIG.
*/
SET NOCOUNT ON
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('tempdb..#cfg') IS NOT NULL DROP TABLE #cfg
CREATE TABLE #cfg(k varchar(30) primary key, v sysname)
INSERT #cfg VALUES ('orig','SP_DASHBOARD_OBTENER_IVA_ACUMULADO_ORIG'),('fix','SP_DASHBOARD_OBTENER_IVA_ACUMULADO'),('create_orig','1')
GO
-- 1. Crear copia del ORIGINAL (cuerpo de 01a)
IF (SELECT v FROM #cfg WHERE k='create_orig')='1'
BEGIN
  DECLARE @n sysname=(SELECT v FROM #cfg WHERE k='orig')
  DECLARE @b nvarchar(max)=N'@idTipoReporte int,--1 semana,2 mes,3 año
		@idEstacion int=null,
		@fechaConsulta datetime = null

as

	begin -- procedimiento
		set LANGUAGE Spanish

		declare
		@fechaIni date =''20210601'',
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
																					
		select 200 status , ''OK'' mensaje
		SELECT * from #CATEGORIAS  order by id
		DROP TABLE #CATEGORIAS
		
	end'
  DECLARE @s nvarchar(max)=N'CREATE OR ALTER proc [dbo].['+@n+N'] '+@b
  EXEC(@s)
  PRINT 'Creado '+@n
END
GO
-- 2. Verificacion de texto (A) y generacion de auxiliares
DECLARE @o sysname=(SELECT v FROM #cfg WHERE k='orig'), @f sysname=(SELECT v FROM #cfg WHERE k='fix')
DECLARE @defO nvarchar(max)=OBJECT_DEFINITION(OBJECT_ID(@o)), @defF nvarchar(max)=OBJECT_DEFINITION(OBJECT_ID(@f))
IF @defO IS NULL OR @defF IS NULL THROW 50001,'No existe el SP orig o fix',1
DECLARE @p1o nvarchar(400)=N'CAST(v.fechaVentaDetalle as date)>=c.fechaIni and  CAST(v.fechaVentaDetalle as date)<=c.fechaFin'
DECLARE @p1n nvarchar(400)=N'v.fechaVentaDetalle >= c.fechaIni and v.fechaVentaDetalle < DATEADD(day,1,c.fechaFin)'
DECLARE @p2o nvarchar(400)=N'CAST(PE.fechaAlta as date)>=c.fechaIni and  CAST(PE.fechaAlta as date)<=c.fechaFin'
DECLARE @p2n nvarchar(400)=N'PE.fechaAlta >= c.fechaIni and PE.fechaAlta < DATEADD(day,1,c.fechaFin)'
DECLARE @st nvarchar(100)=N'select 200 status , ''OK'' mensaje', @se nvarchar(100)=N'SELECT * from #CATEGORIAS  order by id'

-- normaliza: desde @idTipoReporte, predicados -> token, sin espacios/saltos de linea ni caracteres no ASCII (comentarios con ene/acentos)
DECLARE @nO nvarchar(max)=SUBSTRING(@defO,CHARINDEX('@idTipoReporte',@defO),LEN(@defO))
DECLARE @nF nvarchar(max)=SUBSTRING(@defF,CHARINDEX('@idTipoReporte',@defF),LEN(@defF))
SET @nO=REPLACE(REPLACE(REPLACE(REPLACE(@nO,@p1o,'<P1>'),@p1n,'<P1>'),@p2o,'<P2>'),@p2n,'<P2>')
SET @nF=REPLACE(REPLACE(REPLACE(REPLACE(@nF,@p1o,'<P1>'),@p1n,'<P1>'),@p2o,'<P2>'),@p2n,'<P2>')
SELECT @nO=(SELECT STRING_AGG(ch,'') WITHIN GROUP (ORDER BY n) FROM (SELECT TOP (LEN(@nO)) n=ROW_NUMBER() OVER(ORDER BY (SELECT 1)), ch=SUBSTRING(@nO,ROW_NUMBER() OVER(ORDER BY (SELECT 1)),1) FROM sys.all_columns a CROSS JOIN sys.all_columns b) x WHERE UNICODE(ch) BETWEEN 33 AND 127)
SELECT @nF=(SELECT STRING_AGG(ch,'') WITHIN GROUP (ORDER BY n) FROM (SELECT TOP (LEN(@nF)) n=ROW_NUMBER() OVER(ORDER BY (SELECT 1)), ch=SUBSTRING(@nF,ROW_NUMBER() OVER(ORDER BY (SELECT 1)),1) FROM sys.all_columns a CROSS JOIN sys.all_columns b) x WHERE UNICODE(ch) BETWEEN 33 AND 127)
DECLARE @hasP1O int=(LEN(@defO)-LEN(REPLACE(@defO,@p1o,'')))/LEN(@p1o), @hasP2O int=(LEN(@defO)-LEN(REPLACE(@defO,@p2o,'')))/LEN(@p2o)
DECLARE @hasP1N int=(LEN(@defF)-LEN(REPLACE(@defF,@p1n,'')))/LEN(@p1n), @hasP2N int=(LEN(@defF)-LEN(REPLACE(@defF,@p2n,'')))/LEN(@p2n)
DECLARE @hasP1OF int=(LEN(@defF)-LEN(REPLACE(@defF,@p1o,'')))/LEN(@p1o), @hasP2OF int=(LEN(@defF)-LEN(REPLACE(@defF,@p2o,'')))/LEN(@p2o)
SELECT check_A='A. texto',
       orig_tiene_CAST_p1_p2 = CONCAT(@hasP1O,'/',@hasP2O),
       fix_tiene_rango_p1_p2 = CONCAT(@hasP1N,'/',@hasP2N),
       fix_aun_tiene_CAST_p1_p2 = CONCAT(@hasP1OF,'/',@hasP2OF),
       texto_igual_salvo_predicados = CASE WHEN @nO COLLATE Latin1_General_BIN = @nF COLLATE Latin1_General_BIN THEN 'PASS' ELSE 'FAIL' END,
       len_norm_orig=LEN(@nO), len_norm_fix=LEN(@nF)
IF OBJECT_ID('tempdb..#textcheck') IS NOT NULL DROP TABLE #textcheck
SELECT pass = CASE WHEN @nO COLLATE Latin1_General_BIN = @nF COLLATE Latin1_General_BIN AND @hasP1O=1 AND @hasP2O=1 AND @hasP1N=1 AND @hasP2N=1 AND @hasP1OF=0 AND @hasP2OF=0 THEN 1 ELSE 0 END INTO #textcheck

-- auxiliares: mismo cuerpo, el resultado va a ##VAL_OUT
IF OBJECT_ID('tempdb..##VAL_OUT') IS NOT NULL DROP TABLE ##VAL_OUT
CREATE TABLE ##VAL_OUT(seq int identity(1,1), id int, categoria varchar(100), fechaIni date, fechaFin date, total money, totalPE money)
DECLARE @i int=0, @def nvarchar(max), @nm sysname, @cnt int, @cnt2 int
WHILE @i<2
BEGIN
  SET @i+=1
  SET @def=CASE @i WHEN 1 THEN @defO ELSE @defF END
  SET @nm=CASE @i WHEN 1 THEN 'zz_val_ORIG' ELSE 'zz_val_FIX' END
  SET @cnt=(LEN(@def)-LEN(REPLACE(@def,@st,'')))/LEN(@st); SET @cnt2=(LEN(@def)-LEN(REPLACE(@def,@se,'')))/LEN(@se)
  IF @cnt<>1 OR @cnt2<>1 THROW 50002,'No se encontro (exactamente una vez) el SELECT final a sustituir',1
  SET @def='CREATE OR ALTER proc dbo.'+@nm+' '+SUBSTRING(@def,CHARINDEX('@idTipoReporte',@def),LEN(@def))
  SET @def=REPLACE(@def,@st,'declare @zz int')
  SET @def=REPLACE(@def,@se,'INSERT INTO ##VAL_OUT(id,categoria,fechaIni,fechaFin,total,totalPE) SELECT * from #CATEGORIAS order by id')
  EXEC(@def)
END
GO
-- 3. Juegos de parametros (B)
IF OBJECT_ID('tempdb..#params') IS NOT NULL DROP TABLE #params
CREATE TABLE #params(setNo int identity(1,1) primary key, tipo int, estacion int null, fecha datetime null, why varchar(70))
-- llamadas reales del dashboard: tipo 1 Semanal (carga inicial), 2 Mensuales, 3 Anuales; fecha NULL (hoy); estacion NULL (admin) o idEstacion (rol 3)
INSERT #params(tipo,estacion,fecha,why) SELECT t.t, NULL, NULL, 'dashboard: llamada real, fecha=hoy' FROM (VALUES(1),(2),(3)) t(t)
INSERT #params(tipo,estacion,fecha,why) SELECT 1, e.idEstacion, NULL, 'dashboard: rol 3 estacion '+CAST(e.idEstacion as varchar) FROM Estaciones e
INSERT #params(tipo,estacion,fecha,why) SELECT t.t, (SELECT MIN(idEstacion) FROM Estaciones), NULL, 'estacion unica tipo '+CAST(t.t as varchar) FROM (VALUES(2),(3)) t(t)
-- fechas pasadas, frontera de anio, bisiesto
DECLARE @fx TABLE(f datetime, why varchar(50))
INSERT @fx VALUES ('20241231','fin de anio (semana cruza anio)'),('20250101','inicio de anio'),('20240229','dia bisiesto 2024'),('20200229','dia bisiesto 2020'),
                  ('20210601','fechaIni por defecto del SP'),('20230115','semana domingo inicio'),('20260105','anio actual semana 1')
INSERT #params(tipo,estacion,fecha,why) SELECT t.t, NULL, f.f, f.why FROM @fx f CROSS JOIN (VALUES(1),(2),(3)) t(t)
-- fechas con filas reales de hora extrema (ultimas 23:59:xx / primeras 00:00:xx) y extremos de datos
DECLARE @ed TABLE(d date, why varchar(50))
INSERT @ed SELECT TOP 2 CAST(fechaVentaDetalle as date),'VentasDetalle hora mas tardia' FROM VentasDetalle ORDER BY CAST(fechaVentaDetalle as time) DESC
INSERT @ed SELECT TOP 2 CAST(fechaVentaDetalle as date),'VentasDetalle hora mas temprana' FROM VentasDetalle ORDER BY CAST(fechaVentaDetalle as time) ASC
INSERT @ed SELECT TOP 2 CAST(fechaAlta as date),'PedEspDetalle hora mas tardia' FROM PedidosEspecialesDetalle ORDER BY CAST(fechaAlta as time) DESC
INSERT @ed SELECT TOP 2 CAST(fechaAlta as date),'PedEspDetalle hora mas temprana' FROM PedidosEspecialesDetalle ORDER BY CAST(fechaAlta as time) ASC
INSERT @ed SELECT MIN(CAST(fechaVentaDetalle as date)),'VentasDetalle fecha minima' FROM VentasDetalle
INSERT @ed SELECT MAX(CAST(fechaVentaDetalle as date)),'VentasDetalle fecha maxima' FROM VentasDetalle
INSERT @ed SELECT MIN(CAST(fechaAlta as date)),'PedEspDetalle fecha minima' FROM PedidosEspecialesDetalle
INSERT @ed SELECT MAX(CAST(fechaAlta as date)),'PedEspDetalle fecha maxima' FROM PedidosEspecialesDetalle
INSERT #params(tipo,estacion,fecha,why) SELECT DISTINCT t.t, NULL, e.d, e.why FROM @ed e CROSS JOIN (VALUES(1),(2),(3)) t(t) WHERE e.d IS NOT NULL
SELECT COUNT(*) juegos_de_parametros FROM #params

IF OBJECT_ID('tempdb..#RA') IS NOT NULL DROP TABLE #RA
IF OBJECT_ID('tempdb..#RB') IS NOT NULL DROP TABLE #RB
IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T
CREATE TABLE #RA(setNo int, seq int, id int, categoria varchar(100), fechaIni date, fechaFin date, total money, totalPE money)
CREATE TABLE #RB(setNo int, seq int, id int, categoria varchar(100), fechaIni date, fechaFin date, total money, totalPE money)
CREATE TABLE #T(setNo int, msOrig int, msFix int)
DECLARE @s int,@t int,@e int,@d datetime,@t0 datetime2,@mo int,@mf int
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT setNo,tipo,estacion,fecha FROM #params ORDER BY setNo
OPEN c; FETCH c INTO @s,@t,@e,@d
WHILE @@FETCH_STATUS=0
BEGIN
  TRUNCATE TABLE ##VAL_OUT
  SET @t0=SYSDATETIME(); EXEC dbo.zz_val_ORIG @idTipoReporte=@t,@idEstacion=@e,@fechaConsulta=@d; SET @mo=DATEDIFF(ms,@t0,SYSDATETIME())
  INSERT #RA SELECT @s,seq,id,categoria,fechaIni,fechaFin,total,totalPE FROM ##VAL_OUT
  TRUNCATE TABLE ##VAL_OUT
  SET @t0=SYSDATETIME(); EXEC dbo.zz_val_FIX @idTipoReporte=@t,@idEstacion=@e,@fechaConsulta=@d; SET @mf=DATEDIFF(ms,@t0,SYSDATETIME())
  INSERT #RB SELECT @s,seq,id,categoria,fechaIni,fechaFin,total,totalPE FROM ##VAL_OUT
  INSERT #T VALUES(@s,@mo,@mf)
  FETCH c INTO @s,@t,@e,@d
END
CLOSE c; DEALLOCATE c

-- resultado por juego de parametros
SELECT p.setNo,p.tipo,p.estacion,p.fecha,p.why,
  filasOrig=(SELECT COUNT(*) FROM #RA a WHERE a.setNo=p.setNo),
  filasFix=(SELECT COUNT(*) FROM #RB b WHERE b.setNo=p.setNo),
  orig_menos_fix=(SELECT COUNT(*) FROM (SELECT * FROM #RA WHERE setNo=p.setNo EXCEPT SELECT * FROM #RB WHERE setNo=p.setNo) x),
  fix_menos_orig=(SELECT COUNT(*) FROM (SELECT * FROM #RB WHERE setNo=p.setNo EXCEPT SELECT * FROM #RA WHERE setNo=p.setNo) x),
  totalIvaOrig=(SELECT SUM(total+totalPE) FROM #RA a WHERE a.setNo=p.setNo),
  msOrig=t.msOrig, msFix=t.msFix
FROM #params p JOIN #T t ON t.setNo=p.setNo ORDER BY p.setNo

-- tipos de columna (B2): mismas columnas/tipos del result set capturado
SELECT check_B2='B2. columnas de salida', columnas=STRING_AGG(c.name+':'+TYPE_NAME(c.user_type_id)+IIF(c.max_length>0 AND TYPE_NAME(c.user_type_id) LIKE '%char','('+CAST(c.max_length as varchar)+')',''),', ') WITHIN GROUP (ORDER BY c.column_id)
FROM tempdb.sys.columns c WHERE c.object_id=OBJECT_ID('tempdb..##VAL_OUT') AND c.name<>'seq'
SELECT 'B3. dmv first result set' chk, name, system_type_name, error_message FROM sys.dm_exec_describe_first_result_set(N'SELECT * FROM ##VAL_OUT',NULL,0)

SELECT check_B='B. salida',
  juegos=(SELECT COUNT(*) FROM #params),
  filas_comparadas_orig=(SELECT COUNT(*) FROM #RA), filas_comparadas_fix=(SELECT COUNT(*) FROM #RB),
  filas_distintas=(SELECT COUNT(*) FROM (SELECT * FROM #RA EXCEPT SELECT * FROM #RB) x)+(SELECT COUNT(*) FROM (SELECT * FROM #RB EXCEPT SELECT * FROM #RA) x),
  ms_total_orig=(SELECT SUM(msOrig) FROM #T), ms_total_fix=(SELECT SUM(msFix) FROM #T)
SELECT pass = CASE WHEN (SELECT COUNT(*) FROM #RA)=(SELECT COUNT(*) FROM #RB) AND (SELECT COUNT(*) FROM #RA)>0
  AND NOT EXISTS (SELECT * FROM #RA EXCEPT SELECT * FROM #RB) AND NOT EXISTS (SELECT * FROM #RB EXCEPT SELECT * FROM #RA) THEN 1 ELSE 0 END INTO #outcheck

-- 4. Prueba sintetica de frontera (C)
DECLARE @v TABLE(label varchar(40), ts datetime)
INSERT @v VALUES ('dia anterior 23:59:59.997','20240228 23:59:59.997'),('dia inicio 00:00:00.000','20240229 00:00:00.000'),('dia 00:00:00.003','20240229 00:00:00.003'),
 ('dia 12:00','20240229 12:00:00'),('dia 23:59:59.990','20240229 23:59:59.990'),('dia 23:59:59.993','20240229 23:59:59.993'),('dia 23:59:59.997','20240229 23:59:59.997'),
 ('dia siguiente 00:00:00.000','20240301 00:00:00.000'),('fin anio 23:59:59.997','20241231 23:59:59.997'),('inicio anio sig. 00:00:00','20250101 00:00:00.000'),
 ('inicio anio 00:00:00','20240101 00:00:00.000'),('1900-01-01','19000101'),('9999-12-30 23:59:59.997','99991230 23:59:59.997')
DECLARE @rg TABLE(ini date, fin date)
INSERT @rg VALUES ('20240229','20240229'),('20240101','20241231'),('20240201','20240229'),('20240229','20240301'),('20241231','20241231'),('20250101','20250101'),('19000101','19000101')
SELECT check_C='C. frontera sintetica', combinaciones=COUNT(*),
  difieren=SUM(CASE WHEN (CASE WHEN CAST(v.ts as date)>=r.ini AND CAST(v.ts as date)<=r.fin THEN 1 ELSE 0 END) <> (CASE WHEN v.ts>=r.ini AND v.ts<DATEADD(day,1,r.fin) THEN 1 ELSE 0 END) THEN 1 ELSE 0 END),
  tipo_fechaFin='date (DATEADD(day,1,date) devuelve date)'
FROM @v v CROSS JOIN @rg r
SELECT pass = CASE WHEN NOT EXISTS (SELECT 1 FROM @v v CROSS JOIN @rg r WHERE (CASE WHEN CAST(v.ts as date)>=r.ini AND CAST(v.ts as date)<=r.fin THEN 1 ELSE 0 END) <> (CASE WHEN v.ts>=r.ini AND v.ts<DATEADD(day,1,r.fin) THEN 1 ELSE 0 END)) THEN 1 ELSE 0 END INTO #synth

-- 5. Informativo (D): filas reales cerca de medianoche
SELECT check_D='D. filas reales cerca de medianoche (informativo)',
 vd_tarde_23_59=(SELECT COUNT(*) FROM VentasDetalle WHERE CAST(fechaVentaDetalle as time)>='23:59:00'),
 vd_00_00_00=(SELECT COUNT(*) FROM VentasDetalle WHERE CAST(fechaVentaDetalle as time)='00:00:00'),
 ped_tarde_23_59=(SELECT COUNT(*) FROM PedidosEspecialesDetalle WHERE CAST(fechaAlta as time)>='23:59:00'),
 ped_00_00_00=(SELECT COUNT(*) FROM PedidosEspecialesDetalle WHERE CAST(fechaAlta as time)='00:00:00'),
 vd_max_hora=(SELECT MAX(CAST(fechaVentaDetalle as time)) FROM VentasDetalle), ped_max_hora=(SELECT MAX(CAST(fechaAlta as time)) FROM PedidosEspecialesDetalle)

SELECT 'RESULTADO GLOBAL: ' + CASE WHEN (SELECT pass FROM #textcheck)=1 AND (SELECT pass FROM #outcheck)=1 AND (SELECT pass FROM #synth)=1 THEN 'PASS' ELSE 'FAIL' END AS resultado,
  texto=(SELECT pass FROM #textcheck), salida=(SELECT pass FROM #outcheck), frontera=(SELECT pass FROM #synth)
GO
-- 6. Limpieza
IF OBJECT_ID('dbo.zz_val_ORIG') IS NOT NULL DROP PROCEDURE dbo.zz_val_ORIG
IF OBJECT_ID('dbo.zz_val_FIX') IS NOT NULL DROP PROCEDURE dbo.zz_val_FIX
IF OBJECT_ID('tempdb..##VAL_OUT') IS NOT NULL DROP TABLE ##VAL_OUT
IF (SELECT v FROM #cfg WHERE k='create_orig')='1'
BEGIN
  DECLARE @n sysname=(SELECT v FROM #cfg WHERE k='orig')
  IF @n LIKE '%[_]ORIG' AND OBJECT_ID('dbo.'+@n) IS NOT NULL
  BEGIN
    DECLARE @sql nvarchar(300)=N'DROP PROCEDURE [dbo].['+@n+N']'
    EXEC(@sql)
    PRINT 'Eliminado '+@n
  END
END
GO