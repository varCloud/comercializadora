/* ============================================================================
   FASE 0 - Eliminar tablas de respaldo manual abandonadas.
   Recupera ~128 MB. Ninguna de estas tablas es referenciada por codigo C#
   ni por ningun stored procedure (verificado 2026-08-03).

   REQUISITO: backup completo de la base, verificado restaurandolo, ANTES
   de correr esto. Un DROP TABLE no se deshace.

   Ejecutar el paso 1 primero. Solo si sale limpio, ejecutar el paso 2.
   ============================================================================ */
set nocount on;

/* ---------------------------------------------------------------------------
   PASO 1 - VERIFICACION (solo lectura). Las tres consultas deben salir vacias.
   --------------------------------------------------------------------------- */

print '=== 1a. Modulos SQL que referencian estas tablas (debe salir vacio) ===';
select distinct object_name(m.object_id) as objeto_que_referencia
from sys.sql_modules m
where m.definition like '%RespPE%'
   or m.definition like '%[_]resp20220220%'
   or m.definition like '%Estaciones[_]resp%';

print '';
print '=== 1b. Foreign keys que entran o salen de estas tablas (debe salir vacio) ===';
select
    object_name(fk.parent_object_id)     as tabla_hija,
    object_name(fk.referenced_object_id) as tabla_padre
from sys.foreign_keys fk
where object_name(fk.parent_object_id)     like '%Resp%'
   or object_name(fk.referenced_object_id) like '%Resp%'
   or object_name(fk.parent_object_id)     like '%[_]resp%'
   or object_name(fk.referenced_object_id) like '%[_]resp%';

print '';
print '=== 1c. Inventario de lo que se va a borrar (revisar antes de seguir) ===';
select
    t.name as tabla,
    p.rows as filas,
    cast(sum(a.total_pages) * 8.0 / 1024 as decimal(10,1)) as mb
from sys.tables t
join sys.indexes i on t.object_id = i.object_id
join sys.partitions p on i.object_id = p.object_id and i.index_id = p.index_id
join sys.allocation_units a on p.partition_id = a.container_id
where i.index_id in (0, 1)
  and (t.name like '%[_]resp%' or t.name like '%RespPE%')
group by t.name, p.rows
order by sum(a.total_pages) desc;


/* ---------------------------------------------------------------------------
   PASO 2 - DROP. Descomentar SOLO despues de que el paso 1 salio limpio
   y de tener el backup verificado.

   Se listan una por una a proposito: nada de DROP generado dinamicamente
   sobre un patron de nombre, porque un patron mal escrito borra la tabla viva.
   --------------------------------------------------------------------------- */

/*
begin try
    begin transaction;

        drop table dbo.InventarioDetalleLog_resp20220220;              --  45.6 MB
        drop table dbo.InventarioDetalleLogRespPE2;                    --  40.6 MB
        drop table dbo.InventarioDetalleLogRespPE;                     --  39.7 MB
        drop table dbo.InventarioDetalle_resp20220220;                 --   0.6 MB
        drop table dbo.InventarioDetalleRespPE2;                       --   0.5 MB
        drop table dbo.InventarioDetalleRespPE;                        --   0.5 MB
        drop table dbo.PedidosEspecialesMovimientosDeMercanciaRespPE2; --   0.3 MB
        drop table dbo.PedidosEspecialesMovimientosDeMercanciaRespPE;  --   0.2 MB
        drop table dbo.InventarioGeneral_resp20220220;                 --   0.1 MB
        drop table dbo.Estaciones_resp;                                --   0.1 MB

    commit transaction;
    print 'FASE 0 OK - 10 tablas eliminadas.';
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR - se revirtio todo. Nada fue eliminado.';
    print error_message();
end catch;
*/
