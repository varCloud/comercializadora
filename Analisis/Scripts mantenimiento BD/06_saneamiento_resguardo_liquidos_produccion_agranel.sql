/* ============================================================================
   SANEAMIENTO - Inventario fantasma en resguardo (Almacen Liquidos) generado
   por la regresion del fix del 2026-09-09 en SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL.

   Contexto: entre el deploy del 2026-09-09 y el fix del 2026-09-12 (ver
   Analisis/Scripts administracion de produccion agranel/SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL.sql,
   bloque "Modificado 2026/09/12"), cada aprobacion de consumo normal de MPL
   (idEstatusProduccionAgranel=3) en Almacen Liquidos (idAlmacen=2) agrego,
   ademas del descuento correcto de la ubicacion tecnica (1004), la misma
   cantidad a la ubicacion de resguardo (1000/1000/1000, idUbicacion=3895).

   Esa cantidad es 100% fantasma: la mercancia real ya habia salido del
   anaquel fisico al momento de "agregar" el consumo (ver InventarioDetalleLog
   idTipoMovInventario=29 y 30 contra la ubicacion real, no la de resguardo).
   La ubicacion de resguardo del Almacen Liquidos no tiene ningun otro uso
   legitimo (verificado: en la copia local restaurada de produccion, previa al
   09-09, su saldo total era 0). Confirmado con el cliente: hay que sanear
   estos ultimos procesos que se enviaron a resguardo, el resultado debe
   quedar en cero.

   Alcance: SOLO la ubicacion de resguardo del Almacen Liquidos (idUbicacion
   3895). No toca la ubicacion tecnica (1004) ni ningun otro almacen.
   ============================================================================ */
set nocount on;

declare @idUbicacionResguardoLiquidos int = 3895; -- Almacen Liquidos (idAlmacen=2), resguardo 1000/1000/1000
declare @idUsuarioCorreccion int = 0; -- <<< AJUSTAR: poner aqui tu idUsuario antes de correr el PASO 3

/* ---------------------------------------------------------------------------
   PASO 1 - VERIFICACION PREVIA (solo lectura).
   --------------------------------------------------------------------------- */

print '=== 1a. Confirmar la ubicacion de resguardo de Almacen Liquidos ===';
select * from Ubicacion where idUbicacion = @idUbicacionResguardoLiquidos;

print '';
print '=== 1b. Productos con saldo fantasma actual en resguardo (esto es lo que se va a poner en 0) ===';
select idProducto, cantidad, fechaAlta, fechaActualizacion
from InventarioDetalle
where idUbicacion = @idUbicacionResguardoLiquidos and cantidad <> 0
order by fechaActualizacion;

print '';
print '=== 1c. Total a corregir ===';
select count(*) productosAfectados, sum(cantidad) cantidadTotalFantasma
from InventarioDetalle
where idUbicacion = @idUbicacionResguardoLiquidos and cantidad <> 0;

print '';
print '=== 1d. Rastro de como se genero (movimientos tipo 31 desde el deploy del bug) ===';
select idInventarioDetalleLOG, idProducto, cantidad, cantidadActual, idUsuario, fechaAlta, idProcesoProduccionAgranel
from InventarioDetalleLog
where idUbicacion = @idUbicacionResguardoLiquidos
  and idTipoMovInventario = 31
  and fechaAlta >= '2026-09-09'
order by fechaAlta;


/* ---------------------------------------------------------------------------
   PASO 2 - NUEVO TIPO DE MOVIMIENTO PARA LA CORRECCION.
   Siguiendo la convencion existente en CatTipoMovimientoInventario (un tipo
   dedicado por evento de negocio, ver script 05).
   Descomentar SOLO despues de revisar el PASO 1.
   --------------------------------------------------------------------------- */

/*
begin try
    begin transaction;

        insert into CatTipoMovimientoInventario (descripcion, operacion)
        values
            ('Actualizacion de Inventario(correccion: reversion de mercancia enviada por error a resguardo en aprobacion de produccion agranel)', -1);

    commit transaction;
    print '2 OK - catalogo de movimientos actualizado.';
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR en 2 - se revirtio todo.';
    print error_message();
end catch;
go
*/


/* ---------------------------------------------------------------------------
   PASO 3 - CORRECCION.
   Por cada producto con saldo fantasma en resguardo: registra el movimiento
   de reversion (con la cantidad exacta que se quita, para trazabilidad) y
   pone el saldo en 0.
   Idempotente: correrlo dos veces es seguro (la segunda vez el filtro
   cantidad<>0 no encuentra filas).
   Descomentar SOLO despues de correr el PASO 2 y de tener el backup
   verificado. Ajustar @idUsuarioCorreccion arriba antes de correr.
   --------------------------------------------------------------------------- */

/*
begin try
    begin transaction;

        declare @idTipoMovCorreccion int =
            (select idTipoMovInventario from CatTipoMovimientoInventario
             where descripcion like 'Actualizacion de Inventario(correccion: reversion de mercancia enviada por error a resguardo%');

        if (@idTipoMovCorreccion is null)
        begin
            raiserror('No se encontro el tipo de movimiento de correccion - corre primero el PASO 2', 16, 1);
        end

        insert into InventarioDetalleLog
            (idUbicacion, idProducto, cantidad, cantidadActual, idTipoMovInventario, idUsuario, fechaAlta, idVenta, idPedidoInterno, idProcesoProduccionAgranel)
        select
            idUbicacion, idProducto, cantidad, 0, @idTipoMovCorreccion, @idUsuarioCorreccion, dbo.FechaActual(), 0, 0, 0
        from InventarioDetalle
        where idUbicacion = @idUbicacionResguardoLiquidos and cantidad <> 0;

        update InventarioDetalle
        set cantidad = 0, fechaActualizacion = dbo.FechaActual()
        where idUbicacion = @idUbicacionResguardoLiquidos and cantidad <> 0;

        print '3 OK - ' + cast(@@rowcount as varchar) + ' productos saneados en resguardo de Almacen Liquidos.';

    commit transaction;
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR en 3 - se revirtio todo.';
    print error_message();
end catch;
go
*/


/* ---------------------------------------------------------------------------
   PASO 4 - VERIFICACION POST-APLICACION (solo lectura).
   --------------------------------------------------------------------------- */

/*
print '=== 4a. No debe quedar ningun producto con saldo en resguardo de Almacen Liquidos ===';
select count(*) pendientes, sum(cantidad) cantidadPendiente
from InventarioDetalle
where idUbicacion = 3895 and cantidad <> 0;

print '';
print '=== 4b. Movimientos de correccion insertados ===';
select * from InventarioDetalleLog
where idUbicacion = 3895 and idTipoMovInventario = (select idTipoMovInventario from CatTipoMovimientoInventario where descripcion like 'Actualizacion de Inventario(correccion%')
order by fechaAlta desc;
*/


/* ---------------------------------------------------------------------------
   PASO 5 - ROLLBACK, por si hay que deshacerlo.
   El PASO 3 SI queda trazado: cada fila de InventarioDetalleLog insertada en
   ese paso tiene la cantidad exacta que se quito de cada producto, así que
   revertir es sumar esa misma cantidad de vuelta en InventarioDetalle. Aun
   asi, preferir restaurar desde el backup si hay dudas.
   --------------------------------------------------------------------------- */

/*
delete from CatTipoMovimientoInventario
where descripcion like 'Actualizacion de Inventario(correccion: reversion de mercancia enviada por error a resguardo%';
*/
