/* ============================================================================
   POST-PURGA - Devolver el espacio liberado al disco y dejar los indices sanos.

   Correr DESPUES de 01 y 02, en ventana de mantenimiento.

   ORDEN IMPORTANTE: primero encoger, despues reconstruir indices.
   El shrink reacomoda paginas y fragmenta brutalmente los indices; si se
   reconstruye antes de encoger, el trabajo se tira a la basura.

   NOTA SMARTERASP: en hosting compartido puede que el usuario de la base no
   tenga permiso para DBCC SHRINKFILE. Si sale error de permisos, hay que
   pedirle el shrink a soporte. La purga igual sirve: el espacio queda libre
   DENTRO del archivo y la base deja de crecer, aunque el .mdf no se encoja.
   ============================================================================ */
set nocount on;

/* ---------------------------------------------------------------------------
   PASO 1 - Cuanto espacio quedo libre dentro de los archivos (solo lectura)
   --------------------------------------------------------------------------- */
select
    name                                                              as archivo,
    cast(size * 8.0 / 1024 as decimal(10,1))                          as tamano_mb,
    cast(fileproperty(name, 'SpaceUsed') * 8.0 / 1024 as decimal(10,1)) as usado_mb,
    cast((size - fileproperty(name, 'SpaceUsed')) * 8.0 / 1024 as decimal(10,1)) as libre_mb
from sys.database_files;


/* ---------------------------------------------------------------------------
   PASO 2 - Encoger. Descomentar cuando haya ventana.
   Dejar ~15% de holgura para que la base no tenga que crecer en cada venta.
   --------------------------------------------------------------------------- */

/*
-- Sustituir 'NOMBRE_LOGICO' por el valor de la columna "archivo" del paso 1.
-- El segundo parametro es el tamano objetivo en MB.
dbcc shrinkfile (N'NOMBRE_LOGICO', 2600);
*/


/* ---------------------------------------------------------------------------
   PASO 3 - Reconstruir indices fragmentados. Descomentar despues del shrink.
   Solo toca los que estan mal (>30% fragmentacion y mas de 1000 paginas),
   para no perder una hora reconstruyendo catalogos de 5 filas.
   --------------------------------------------------------------------------- */

/*
declare @sql nvarchar(max) = N'';

select @sql = @sql + N'alter index ' + quotename(i.name)
                   + N' on ' + quotename(schema_name(t.schema_id)) + N'.' + quotename(t.name)
                   + N' rebuild;' + char(10)
from sys.dm_db_index_physical_stats(db_id(), null, null, null, 'LIMITED') s
join sys.indexes i on s.object_id = i.object_id and s.index_id = i.index_id
join sys.tables  t on i.object_id = t.object_id
where s.avg_fragmentation_in_percent > 30
  and s.page_count > 1000
  and i.name is not null;

print @sql;      -- revisar la lista antes de ejecutarla
-- exec sp_executesql @sql;
*/


/* ---------------------------------------------------------------------------
   PASO 4 - Actualizar estadisticas. Esto SI conviene correrlo siempre al final:
   despues de borrar 6.5M de filas las estadisticas viejas producen planes malos
   y el sistema se siente lento sin razon aparente.
   --------------------------------------------------------------------------- */

/*
exec sp_updatestats;
*/
