/* ============================================================================
   FASE 2 (Etapa A) - Persistir serie + folio del CFDI y protegerlos con un
   indice unico.

   Para que sea seguro resetear el IDENTITY de Ventas / PedidosEspeciales, el
   folio ya timbrado tiene que dejar de derivarse del id. Este script solo
   agrega columnas, hace backfill y crea los indices: NO cambia el
   comportamiento del sistema. El timbrado sigue calculando el folio como hoy
   hasta que salga la Etapa B (ver PLAN-fase2-folio-serie.md).

   OJO: al terminar la Etapa A, todo timbrado nuevo grabaria serie='' y
   folio='' -> el segundo choca contra el indice unico. La Etapa B tiene que
   desplegarse junto con esto o inmediatamente despues.

   REQUISITO: backup completo de la base, verificado restaurandolo, antes del
   PASO 2.

   Ejecutar el PASO 1 primero. Solo si sale limpio, ejecutar el PASO 2.
   ============================================================================ */
set nocount on;

/* ---------------------------------------------------------------------------
   PASO 1 - VERIFICACION (solo lectura).
   Las consultas 1a a 1e deben salir en cero. Si alguna trae filas, el indice
   unico va a fallar y hay que resolver esos casos antes de seguir.
   --------------------------------------------------------------------------- */

print '=== 1a. Duplicados de la llave de negocio (debe ser 0 y 0) ===';
select
    (select count(*) from (select idVenta from Facturas
                           group by idVenta having count(*) > 1) a) as dup_idVenta,
    (select count(*) from (select idPedidoEspecial from FacturasPedidosEspeciales
                           group by idPedidoEspecial having count(*) > 1) b) as dup_idPedidoEspecial;

print '';
print '=== 1b. Llaves nulas (debe ser 0 y 0) ===';
select
    (select count(*) from Facturas where idVenta is null) as facturas_sin_idVenta,
    (select count(*) from FacturasPedidosEspeciales where idPedidoEspecial is null) as pe_sin_idPedidoEspecial;

print '';
print '=== 1c. UUID duplicados (debe ser 0 y 0) ===';
select
    (select count(*) from (select UUID from Facturas
                           where UUID is not null and UUID <> ''
                           group by UUID having count(*) > 1) a) as dup_uuid_facturas,
    (select count(*) from (select UUID from FacturasPedidosEspeciales
                           where UUID is not null and UUID <> ''
                           group by UUID having count(*) > 1) b) as dup_uuid_pe;

print '';
print '=== 1d. Folios que no caben en varchar(40) - limite del SAT (debe ser 0 y 0) ===';
select
    (select count(*) from Facturas
     where len(cast(idVenta as varchar(50))) > 40) as folio_largo_facturas,
    (select count(*) from FacturasPedidosEspeciales
     where len('PE' + cast(idPedidoEspecial as varchar(50))) > 40) as folio_largo_pe;

print '';
print '=== 1e. Colision cruzada: folio de venta que empiece con PE (debe ser 0) ===';
select count(*) as colision_cruzada
from Facturas
where cast(idVenta as varchar(50)) like 'PE%';

print '';
print '=== 1f. Las columnas ya existen? (si sale algo, el script ya se corrio) ===';
select object_name(c.object_id) as tabla, c.name as columna
from sys.columns c
where c.object_id in (object_id('Facturas'),
                      object_id('FacturasPedidosEspeciales'),
                      object_id('FactConfiguracionComprobante'))
  and c.name in ('serie', 'folio', 'Serie');

print '';
print '=== 1g. Inventario de lo que se va a tocar ===';
select 'Facturas' as tabla, count(*) as filas,
       min(year(fecha)) as anio_min, max(year(fecha)) as anio_max
from Facturas
union all
select 'FacturasPedidosEspeciales', count(*), min(year(fecha)), max(year(fecha))
from FacturasPedidosEspeciales;

print '';
print '=== 1h. FacturasAbonos - se espera 0 filas (tabla muerta, no lleva folio) ===';
select count(*) as filas_facturas_abonos from FacturasAbonos;


/* ---------------------------------------------------------------------------
   PASO 2 - DDL + BACKFILL + INDICES.
   Descomentar SOLO despues de que el PASO 1 salio limpio y de tener el backup
   verificado.

   Sobre serie = '' en el backfill: las facturas historicas se timbraron SIN
   atributo Serie (SP_FACTURACION_OBTENER_CONFIGURACION_COMPROBANTE nunca lo
   devolvio). Grabar '' registra lo que realmente se mando al SAT. NULL no
   sirve porque SQL Server considera iguales dos NULL en un indice unico.
   --------------------------------------------------------------------------- */

/*
begin try
    begin transaction;

        ---------------------------------------------------------------- 2.1
        -- Columnas nuevas, nullables por ahora
        alter table dbo.Facturas
            add serie varchar(25) null,
                folio varchar(40) null;

        alter table dbo.FacturasPedidosEspeciales
            add serie varchar(25) null,
                folio varchar(40) null;

    commit transaction;
    print '2.1 OK - columnas agregadas.';
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR en 2.1 - se revirtio todo.';
    print error_message();
end catch;
go

begin try
    begin transaction;

        ---------------------------------------------------------------- 2.2
        -- Backfill replicando exactamente la regla de FacturaController.cs:166
        update dbo.Facturas
            set serie = '',
                folio = cast(idVenta as varchar(40));

        update dbo.FacturasPedidosEspeciales
            set serie = '',
                folio = 'PE' + cast(idPedidoEspecial as varchar(38));

        ---------------------------------------------------------------- 2.3
        -- Ya sin nulos, se blinda
        alter table dbo.Facturas
            alter column serie varchar(25) not null;
        alter table dbo.Facturas
            alter column folio varchar(40) not null;

        alter table dbo.FacturasPedidosEspeciales
            alter column serie varchar(25) not null;
        alter table dbo.FacturasPedidosEspeciales
            alter column folio varchar(40) not null;

    commit transaction;
    print '2.2/2.3 OK - backfill aplicado y columnas NOT NULL.';
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR en 2.2/2.3 - se revirtio todo.';
    print error_message();
end catch;
go

begin try
    begin transaction;

        ---------------------------------------------------------------- 2.4
        -- Separacion de espacios de folio entre las dos tablas.
        -- Sin esto, la unicidad cruzada depende de que nadie cambie el
        -- prefijo 'PE' por descuido.
        alter table dbo.Facturas
            add constraint CK_Facturas_folio_no_PE
            check (folio not like 'PE%');

        alter table dbo.FacturasPedidosEspeciales
            add constraint CK_FacturasPE_folio_PE
            check (folio like 'PE%');

        ---------------------------------------------------------------- 2.5
        -- EL INDICE UNICO. Esto es lo que impide que un id reciclado despues
        -- del reseed pise un CFDI ya timbrado.
        create unique index UQ_Facturas_serie_folio
            on dbo.Facturas (serie, folio);

        create unique index UQ_FacturasPE_serie_folio
            on dbo.FacturasPedidosEspeciales (serie, folio);

        ---------------------------------------------------------------- 2.6
        -- Indices de apoyo: hoy estas tablas NO tienen ningun indice sobre su
        -- llave de negocio, asi que cada upsert/cancelacion/reimpresion hace
        -- scan completo.
        create index IX_Facturas_idVenta
            on dbo.Facturas (idVenta);

        create index IX_FacturasPE_idPedidoEspecial
            on dbo.FacturasPedidosEspeciales (idPedidoEspecial);

    commit transaction;
    print '2.4/2.5/2.6 OK - checks e indices creados.';
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR en 2.4/2.5/2.6 - se revirtio todo.';
    print error_message();
end catch;
go

begin try
    begin transaction;

        ---------------------------------------------------------------- 2.7
        -- Serie vigente. Tabla de un solo renglon.
        -- Arranca en '' para no cambiar comportamiento: mientras valga '',
        -- los CFDI se siguen emitiendo sin Serie, igual que hoy.
        -- Se pone 'A' (o el ciclo que toque) en el momento del corte, junto
        -- con el DBCC CHECKIDENT. Ver Etapa D del plan.
        alter table dbo.FactConfiguracionComprobante
            add Serie varchar(25) not null constraint DF_FactConfig_Serie default '';

    commit transaction;
    print '2.7 OK - columna Serie agregada a FactConfiguracionComprobante.';
end try
begin catch
    if @@trancount > 0 rollback transaction;
    print 'ERROR en 2.7 - se revirtio todo.';
    print error_message();
end catch;
go
*/


/* ---------------------------------------------------------------------------
   PASO 3 - VERIFICACION POST-APLICACION (solo lectura).
   Correr despues del PASO 2. Tambien sirve como chequeo periodico y,
   sobre todo, DESPUES del corte de serie + reseed (Etapa D).
   --------------------------------------------------------------------------- */

/*
print '=== 3a. Todo folio grabado coincide con la regla vieja (debe ser 0 y 0) ===';
select
    (select count(*) from Facturas
     where folio <> cast(idVenta as varchar(40)) and serie = '') as facturas_desalineadas,
    (select count(*) from FacturasPedidosEspeciales
     where folio <> 'PE' + cast(idPedidoEspecial as varchar(38)) and serie = '') as pe_desalineadas;

print '';
print '=== 3b. Sin duplicados de (serie, folio) - lo garantiza el indice, se verifica igual ===';
select 'Facturas' as tabla, serie, folio, count(*) as veces
from Facturas group by serie, folio having count(*) > 1
union all
select 'FacturasPedidosEspeciales', serie, folio, count(*)
from FacturasPedidosEspeciales group by serie, folio having count(*) > 1;

print '';
print '=== 3c. Serie vigente configurada ===';
select Serie as serie_vigente from FactConfiguracionComprobante;

print '';
print '=== 3d. Convivencia de ciclos: facturas por serie ===';
select 'Facturas' as tabla, serie, count(*) as filas,
       min(fecha) as desde, max(fecha) as hasta
from Facturas group by serie
union all
select 'FacturasPedidosEspeciales', serie, count(*), min(fecha), max(fecha)
from FacturasPedidosEspeciales group by serie
order by 1, 2;

print '';
print '=== 3e. POST-RESEED: ids reciclados que ya tienen factura en la serie vieja ===';
--  Debe salir vacio antes del corte. Despues del corte, cada fila aqui es un
--  id que existe en dos series: eso es correcto y esperado, pero confirma que
--  buscar factura por idVenta a secas ya es ambiguo (ver Etapa C del plan).
select f.idVenta, count(distinct f.serie) as series_distintas
from Facturas f
group by f.idVenta
having count(distinct f.serie) > 1;
*/


/* ---------------------------------------------------------------------------
   PASO 4 - ROLLBACK de la Etapa A, por si hay que deshacerla.
   Solo valido mientras la Etapa B (codigo que escribe serie/folio) NO este
   desplegada. Despues de la Etapa B, tirar estas columnas rompe el timbrado.
   --------------------------------------------------------------------------- */

/*
drop index UQ_Facturas_serie_folio on dbo.Facturas;
drop index IX_Facturas_idVenta on dbo.Facturas;
alter table dbo.Facturas drop constraint CK_Facturas_folio_no_PE;
alter table dbo.Facturas drop column serie, folio;

drop index UQ_FacturasPE_serie_folio on dbo.FacturasPedidosEspeciales;
drop index IX_FacturasPE_idPedidoEspecial on dbo.FacturasPedidosEspeciales;
alter table dbo.FacturasPedidosEspeciales drop constraint CK_FacturasPE_folio_PE;
alter table dbo.FacturasPedidosEspeciales drop column serie, folio;

alter table dbo.FactConfiguracionComprobante drop constraint DF_FactConfig_Serie;
alter table dbo.FactConfiguracionComprobante drop column Serie;
*/
