/* ============================================================================
   FASE 1 - Purga de bitacoras de inventario anteriores al cutoff.

   Recupera ~490 MB. Politica: conservar ano actual + ano anterior.
   Cutoff 2025-01-01 => se borra 2021..2024.

   Tablas afectadas (ninguna tiene tablas hijas por FK, verificado 2026-08-03):
     - InventarioDetalleLog   3,622,335 de 5,364,501 filas  (~326 MB)
     - InventarioGeneralLog   2,935,529 de 4,326,269 filas  (~166 MB)

   POR QUE SE BORRA POR RANGO DE PK Y NO POR FECHA:
   InventarioGeneralLog NO tiene indice sobre fechaAlta, asi que un
   "delete where fechaAlta < @cut" escanearia 4.3M filas en CADA lote.
   Se calcula una sola vez el id maximo bajo el cutoff y se borra por rango
   de clave primaria, que es el indice clustered. El paso 1 valida que id y
   fecha van en el mismo orden, que es lo que hace correcta esa sustitucion.

   REQUISITO: backup completo verificado. Este backup ES el archivo historico
   de las bitacoras; una vez borradas solo viven ahi.

   CORRER EN HORARIO DE BAJA VENTA. Es reversible solo por restore.
   ============================================================================ */
set nocount on;

declare @cutoff datetime = '2025-01-01';   -- <<< unico parametro a cambiar


/* ---------------------------------------------------------------------------
   PASO 1 - PRE-VUELO (solo lectura). Correr esto solo, revisar, y continuar
   unicamente si "viejos_escapados" y "recientes_atrapados" salen en CERO.
   --------------------------------------------------------------------------- */

print '=== 1a. Modelo de recuperacion y tamano del log ===';
print '    Si esta en FULL sin backups de log, el log crecera durante la purga.';
select
    d.name,
    d.recovery_model_desc,
    cast((select sum(size) * 8.0 / 1024 from sys.database_files where type = 1) as decimal(10,1)) as log_mb
from sys.databases d
where d.database_id = db_id();

print '';
print '=== 1b. Volumen a borrar y validacion de orden id/fecha ===';

declare @maxDetalle int = (select max(idInventarioDetalleLOG) from InventarioDetalleLog where fechaAlta < @cutoff);
declare @maxGeneral int = (select max(idInventarioGeneralLog) from InventarioGeneralLog where fechaAlta < @cutoff);

select
    'InventarioDetalleLog' as tabla,
    @maxDetalle            as max_id_a_borrar,
    (select count(*) from InventarioDetalleLog)                                              as total,
    (select count(*) from InventarioDetalleLog where fechaAlta <  @cutoff)                   as a_borrar,
    (select count(*) from InventarioDetalleLog where idInventarioDetalleLOG <= @maxDetalle
                                                 and fechaAlta >= @cutoff)                   as recientes_atrapados,
    (select count(*) from InventarioDetalleLog where idInventarioDetalleLOG >  @maxDetalle
                                                 and fechaAlta <  @cutoff)                   as viejos_escapados
union all
select
    'InventarioGeneralLog',
    @maxGeneral,
    (select count(*) from InventarioGeneralLog),
    (select count(*) from InventarioGeneralLog where fechaAlta <  @cutoff),
    (select count(*) from InventarioGeneralLog where idInventarioGeneralLog <= @maxGeneral
                                                 and fechaAlta >= @cutoff),
    (select count(*) from InventarioGeneralLog where idInventarioGeneralLog >  @maxGeneral
                                                 and fechaAlta <  @cutoff);


/* ---------------------------------------------------------------------------
   PASO 2 - PURGA POR LOTES. Descomentar despues de validar el paso 1.

   Cada lote es una transaccion propia y corta: si algo falla, solo se pierde
   ese lote y lo ya borrado queda borrado (es idempotente, se puede reanudar
   volviendo a correr el script).

   @lote  = filas por transaccion. 5000 es conservador para hosting compartido.
   @pausa = respiro entre lotes para que el POS no sienta el bloqueo.
   --------------------------------------------------------------------------- */

/*
declare @lote  int      = 5000;
declare @pausa char(8)  = '00:00:01';
declare @borradas int;
declare @acumulado bigint;
declare @inicio datetime2 = sysdatetime();

-- ---------- InventarioDetalleLog ----------
set @acumulado = 0;
set @borradas  = 1;
print 'Purgando InventarioDetalleLog hasta id ' + cast(@maxDetalle as varchar(20)) + ' ...';

while @borradas > 0
begin
    begin transaction;
        delete top (@lote) from InventarioDetalleLog
        where idInventarioDetalleLOG <= @maxDetalle;
        set @borradas = @@rowcount;
    commit transaction;

    set @acumulado += @borradas;

    if @acumulado % 100000 < @lote and @borradas > 0
        raiserror('  InventarioDetalleLog: %I64d filas borradas...', 0, 1, @acumulado) with nowait;

    if @borradas > 0 waitfor delay @pausa;
end
raiserror('InventarioDetalleLog LISTO: %I64d filas.', 0, 1, @acumulado) with nowait;

-- ---------- InventarioGeneralLog ----------
set @acumulado = 0;
set @borradas  = 1;
print 'Purgando InventarioGeneralLog hasta id ' + cast(@maxGeneral as varchar(20)) + ' ...';

while @borradas > 0
begin
    begin transaction;
        delete top (@lote) from InventarioGeneralLog
        where idInventarioGeneralLog <= @maxGeneral;
        set @borradas = @@rowcount;
    commit transaction;

    set @acumulado += @borradas;

    if @acumulado % 100000 < @lote and @borradas > 0
        raiserror('  InventarioGeneralLog: %I64d filas borradas...', 0, 1, @acumulado) with nowait;

    if @borradas > 0 waitfor delay @pausa;
end
raiserror('InventarioGeneralLog LISTO: %I64d filas.', 0, 1, @acumulado) with nowait;

print 'FASE 1 TERMINADA en ' + cast(datediff(minute, @inicio, sysdatetime()) as varchar(10)) + ' minutos.';
print 'Siguiente: 03_fase1_post_reclamar_espacio.sql';
*/
