truncate table InventarioPromedioProducto

declare @fechaIni date='20210430',
        @fechaFin date=cast(dbo.FechaActual() as date),
		@difDias int,
		@cont int=1,
		@fecha date

select @difDias=datediff(DAY,@fechaIni,@fechaFin);

create table #dias(
id int,
fecha date
)

select @fecha=@fechaIni

while @cont<=@difDias
begin

INSERT INTO #dias(id,fecha)
select @cont,@fecha

select @fecha=dateadd(day, 1, @fecha)
select @cont=@cont+1;

end;



--se obtienen los dias faltantes de calcular el inventario promedio
select d.id,d.fecha 
INTO #DIAS_FALTANTES_INVENTARIO_PROMEDIO
from #dias d
left join (
select cast(fecha as date) fecha
from [dbo].[InventarioPromedioProducto] 
where cast(fecha as date)>=cast(@fechaIni as date) and cast(fecha as date)<=cast(@fechaFin as date)
group by cast(fecha as date)) i on cast(d.fecha as date)=cast(i.fecha as date)
where i.fecha is null
order by fecha

select count(1) dias_faltantes from #DIAS_FALTANTES_INVENTARIO_PROMEDIO
select @difDias dias

if exists(select 1 from #DIAS_FALTANTES_INVENTARIO_PROMEDIO)
BEGIN
    
	create table #ESTATUS_INVENTARIO_PROMEDIO(
	status int,
	mensaje	varchar(255),
	error_line	varchar(255),
	error_procedure varchar(255)
	)

	--se calculan los dias faltantes
	--declare @fecha date
	while exists(select 1 from #DIAS_FALTANTES_INVENTARIO_PROMEDIO)
	begin
	  
		DELETE #ESTATUS_INVENTARIO_PROMEDIO	
 
		select @fecha=min(fecha) from #DIAS_FALTANTES_INVENTARIO_PROMEDIO

		select @fecha

		INSERT INTO #ESTATUS_INVENTARIO_PROMEDIO
		EXEC SP_TAREA_PROGRAMADA_INVENTARIO_PROM @fecha

		if exists(select 1 from #ESTATUS_INVENTARIO_PROMEDIO where status=200)
		begin
		delete from #DIAS_FALTANTES_INVENTARIO_PROMEDIO where cast(fecha as date)=cast(@fecha as date)
		end
		else
		break;
	end

	drop table #ESTATUS_INVENTARIO_PROMEDIO

END;

drop table #dias,#DIAS_FALTANTES_INVENTARIO_PROMEDIO

				

