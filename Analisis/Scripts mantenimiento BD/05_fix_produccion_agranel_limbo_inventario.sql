/* ============================================================================
   FIX - Inventario "fantasma" en Producción a Granel (consumo de MPL).

   Bug: al aprobar el consumo de MPL con cantidad menor a la solicitada
   (SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL), dos casos nunca tocaban el
   inventario:
     - Rechazo Completo (idEstatusProduccionAgranel=4, cantidadAtendida=0):
       la reserva completa se quedaba para siempre en la ubicacion tecnica de
       produccion agranel (1004 por almacen) y nunca regresaba al anaquel de
       origen.
     - Rechazo Parcial (idEstatusProduccionAgranel=5): solo la porcion
       aceptada se movia a resguardo; la diferencia rechazada se quedaba
       fantasma en 1004 para siempre (no existia concepto de merma en este
       proceso).

   Confirmado con el cliente (2026-09-09): dentro de este asistente de
   consumo de MPL, rechazo completo = regresa al almacen de origen; rechazo
   parcial = la diferencia es merma (aqui no aplica el caso "sobrante que se
   regresa" porque no es un pedido por bitacora a otro almacen).

   IMPORTANTE sobre el historico: se investigo la ubicacion tecnica (1004)
   para los ~148 registros historicos afectados y la gran mayoria (134) ya NO
   tiene ahi el saldo fantasma - fue absorbido en distintos momentos por
   ajustes GENERICOS de "sobrante sin ubicacion" (idTipoMovInventario=6, sin
   relacion al proceso), es decir la mercancia ya salio del sistema por otra
   via. Descontarla otra vez de 1004 en este punto seria un error (estariamos
   descontando materia prima que ya no tenemos). Por eso el PASO 3 de este
   script SOLO sanea el bookkeeping de ProcesoProduccionAgranel
   (cantidadRestante=0) para los 148 registros historicos - NO mueve
   inventario para ninguno de ellos. El inventario ya refleja la realidad
   actual (0 o casi 0 en 1004 para la mayoria de los productos afectados).

   El fix de la SP (PASO 2) es el que importa hacia adelante: todo consumo de
   MPL que se apruebe o cancele a partir de este deploy va a mover el
   inventario correctamente, sin generar mas fantasmas.

   Definicion completa del SP corregido, para referencia/diff:
   Analisis/Scripts administracion de produccion agranel/SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL.sql

   Probado contra copia local restaurada de produccion (localhost\SQLEXPRESS01)
   simulando alta + rechazo completo y alta + rechazo parcial dentro de una
   transaccion revertida - ver sesion 2026-09-09.
   ============================================================================ */
set nocount on;

/* ---------------------------------------------------------------------------
   PASO 1 - VERIFICACION PREVIA (solo lectura).
   --------------------------------------------------------------------------- */

print '=== 1a. Catalogo de movimientos: los tipos 34/35/36 no deben existir todavia ===';
select * from CatTipoMovimientoInventario where idTipoMovInventario >= 34;

print '';
print '=== 1b. Inventario de registros historicos afectados (idEstatusProduccionAgranel 4/5, cantidadRestante>0) ===';
select idEstatusProduccionAgranel, count(*) filas, sum(cantidadRestante) totalCantidadRestante
from ProcesoProduccionAgranel
where idEstatusProduccionAgranel in (4,5) and cantidadRestante > 0
group by idEstatusProduccionAgranel;

print '';
print '=== 1c. De esos, cuantos todavia tienen saldo fantasma real en su ubicacion (1004) ===';
print '     (informativo - el PASO 3 NO va a mover este inventario, ver nota arriba)';
select
    sum(case when ID.cantidad >= PPA.cantidadRestante then 1 else 0 end) con_saldo_suficiente,
    sum(case when ID.cantidad < PPA.cantidadRestante or ID.cantidad is null then 1 else 0 end) ya_absorbido_por_otra_via
from ProcesoProduccionAgranel PPA
left join InventarioDetalle ID on ID.idProducto = PPA.idProducto and ID.idUbicacion = PPA.idUbicacion
where PPA.idEstatusProduccionAgranel in (4,5) and PPA.cantidadRestante > 0;


/* ---------------------------------------------------------------------------
   PASO 2 - FIX DE LA STORED PROCEDURE + CATALOGO.
   Descomentar SOLO despues de revisar el PASO 1 y de tener el backup
   verificado.
   --------------------------------------------------------------------------- */

/*
begin try
    begin transaction;

        ---------------------------------------------------------------- 2.1
        -- Nuevos tipos de movimiento, uno por cada evento de negocio nuevo,
        -- siguiendo la convencion existente en CatTipoMovimientoInventario.
        insert into CatTipoMovimientoInventario (descripcion, operacion)
        values
            ('Actualizacion de Inventario(salida de mercancia de produccion agranel por rechazo completo/cancelacion)', -1),
            ('Actualizacion de Inventario(regreso de mercancia a almacen origen por rechazo completo/cancelacion en produccion agranel)', 1),
            ('Actualizacion de Inventario(salida de mercancia por merma en rechazo parcial de produccion agranel)', -1);

    commit transaction;
    print '2.1 OK - catalogo de movimientos actualizado.';
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR en 2.1 - se revirtio todo.';
    print error_message();
end catch;
go

-- 2.2 - ALTER de la stored procedure. Ejecutar completo (requiere su propio
-- batch por el CREATE/ALTER). Ver el archivo de la SP para la version
-- documentada con "create proc"; aqui va como ALTER porque el objeto ya
-- existe en la base.
--
-- >>> Pegar aqui el cuerpo de
-- >>> Analisis/Scripts administracion de produccion agranel/SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL.sql
-- >>> cambiando "create proc [dbo].SP_APP_..." por "ALTER PROCEDURE [dbo].SP_APP_..."
go
*/


/* ---------------------------------------------------------------------------
   PASO 3 - SANEAMIENTO DEL HISTORICO (bookkeeping unicamente).
   NO mueve inventario. Ver nota al inicio del script sobre por que.
   Idempotente: correrlo dos veces es seguro (la segunda vez el filtro no
   encuentra filas).
   --------------------------------------------------------------------------- */

/*
begin try
    begin transaction;

        update ProcesoProduccionAgranel
        set cantidadRestante = 0,
            fechaUltimaActualizacion = dbo.FechaActual()
        where idEstatusProduccionAgranel in (4,5)
          and cantidadRestante > 0;

        print '3.1 OK - ' + cast(@@rowcount as varchar) + ' registros historicos saneados (cantidadRestante=0).';

    commit transaction;
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR en 3.1 - se revirtio todo.';
    print error_message();
end catch;
go
*/


/* ---------------------------------------------------------------------------
   PASO 4 - VERIFICACION POST-APLICACION (solo lectura).
   --------------------------------------------------------------------------- */

/*
print '=== 4a. No deben quedar registros con cantidadRestante>0 en estatus 4/5 ===';
select count(*) pendientes
from ProcesoProduccionAgranel
where idEstatusProduccionAgranel in (4,5) and cantidadRestante > 0;

print '';
print '=== 4b. Catalogo con los 3 tipos nuevos ===';
select * from CatTipoMovimientoInventario where idTipoMovInventario >= 34;

print '';
print '=== 4c. Definicion de la SP actualizada (revisar a ojo que trae el fix) ===';
exec sp_helptext 'SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL';
*/


/* ---------------------------------------------------------------------------
   PASO 5 - ROLLBACK, por si hay que deshacerlo.
   El saneamiento del PASO 3 solo toca cantidadRestante; NO es reversible de
   forma automatica porque no queda registro del valor previo (a proposito:
   ese valor previo era el bug). Restaurar desde el backup si hace falta
   deshacer el PASO 3.
   --------------------------------------------------------------------------- */

/*
-- Revertir la SP a la version anterior (sin el fix) si hiciera falta:
-- ver el historial de git de
-- Analisis/Scripts administracion de produccion agranel/SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL.sql

delete from CatTipoMovimientoInventario where idTipoMovInventario in (34,35,36);
*/
