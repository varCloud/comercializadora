/* ============================================================================
   DIAGNOSTICO - Solo lectura. Correr antes y despues de cada fase.
   No modifica nada. Seguro en produccion en cualquier momento.
   ============================================================================ */
set nocount on;

print '=== 1. Tamano total de la base (MB) ===';
select
    cast(sum(case when type = 0 then size end) * 8.0 / 1024 as decimal(10,1)) as datos_mb,
    cast(sum(case when type = 1 then size end) * 8.0 / 1024 as decimal(10,1)) as log_mb
from sys.database_files;

print '';
print '=== 2. Top 20 tablas por espacio ===';
select top 20
    t.name as tabla,
    p.rows as filas,
    cast(sum(a.total_pages) * 8.0 / 1024 as decimal(10,1)) as mb
from sys.tables t
join sys.indexes i on t.object_id = i.object_id
join sys.partitions p on i.object_id = p.object_id and i.index_id = p.index_id
join sys.allocation_units a on p.partition_id = a.container_id
where i.index_id in (0, 1)
group by t.name, p.rows
order by sum(a.total_pages) desc;

print '';
print '=== 3. Consumo de identity (alerta si pct_consumido > 50) ===';
select
    t.name as tabla,
    ty.name as tipo,
    ident_current(t.name) as actual,
    cast(cast(ident_current(t.name) as float) * 100.0 /
        case ty.name
            when 'tinyint'  then 255.0
            when 'smallint' then 32767.0
            when 'int'      then 2147483647.0
            else 9223372036854775807.0
        end as decimal(10,4)) as pct_consumido
from sys.tables t
join sys.identity_columns c on t.object_id = c.object_id
join sys.types ty on c.user_type_id = ty.user_type_id
where ty.name <> 'bigint'
order by pct_consumido desc;
