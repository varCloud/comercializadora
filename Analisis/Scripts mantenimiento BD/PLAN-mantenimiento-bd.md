# Plan de mantenimiento de la base de datos — lluviaBackEnd

**Fecha del análisis:** 2026-08-03
**Base analizada:** `DB_A57E86_comercializadora` (copia restaurada localmente desde el backup de producción, servidor `SQL5063.site4now.net` en SmarterASP)
**Cuota total del hosting:** 10 GB (todas las bases del plan)
**Tamaño actual de esta base:** 3.0 GB asignados / **2.1 GB usados realmente** (856 MB ya libres dentro del archivo)

---

## 0. Contexto y objetivo

La base tiene más de 5 años en productivo. La empresa quiere mantener viva solo la información reciente porque el tamaño está creciendo (~15%/año). La duda inicial del usuario era resetear los `IDENTITY` de las tablas y empezar "de cero", con la complicación de que el folio de las facturas del SAT se deriva del `idVenta`.

**Política de retención acordada:** año actual + año anterior en la base viva (2025 + 2026). El resto se archiva, no se borra — por obligación fiscal (Art. 30 CFF: conservar contabilidad 5 años).

---

## 1. Descartado: resetear los IDENTITY

Se midieron las **100 columnas identity** de la base (26 `bigint`, 74 `int`, **cero** `smallint`/`tinyint`).

| Tabla | Valor actual | Tope del tipo | % consumido |
|---|---|---|---|
| InventarioDetalleLog | 5,372,485 | 2,147,483,647 (`int`) | 0.25% |
| VentasDetalle | 3,566,433 | 2,147,483,647 (`int`) | 0.17% |
| Ventas | 769,946 | 2,147,483,647 (`int`) | 0.036% |

Con crecimiento compuesto del 15% anual, la tabla más consumida (`InventarioDetalleLog`) se agota en **~41 años**. No hay ninguna columna `smallint`/`tinyint` (que sí se agotarían rápido). Se verificó también que no hay "fuga" de identity por reinicios de SQL Server en hosting compartido: el hueco entre `max(idVenta)` y el conteo real de filas es de solo ~7,400 en 5 años (<1%).

**Conclusión:** el problema real es **espacio en disco**, no desbordamiento de identity. Resetear identity no resuelve el espacio y sí introduce un riesgo real: colisión de folios contra CFDIs ya timbrados ante el SAT. Se descarta por completo.

Query de vigilancia para el futuro (correr cuando se quiera, no bloquea nada):

```sql
select
    t.name as tabla, ty.name as tipo, ident_current(t.name) as actual,
    cast(cast(ident_current(t.name) as float) * 100.0 /
        case ty.name when 'tinyint' then 255.0 when 'smallint' then 32767.0
                      when 'int' then 2147483647.0 else 9223372036854775807.0 end
    as decimal(10,4)) as pct_consumido
from sys.tables t
join sys.identity_columns c on t.object_id = c.object_id
join sys.types ty on c.user_type_id = ty.user_type_id
where ty.name <> 'bigint'
order by pct_consumido desc;
```

Alerta solo si alguna columna pasa de 50% consumido — hoy la máxima es 0.25%.

---

## 2. El folio del CFDI: ya se usa folio alfanumérico en producción

Duda del usuario: si se cambian los ids de `Ventas`, ¿el SAT acepta un folio compuesto?

**Hallazgo en el propio código** — [`FacturaController.cs`](../../lluviaBackEnd/lluviaBackEnd/Controllers/FacturaController.cs) (líneas 166 y 329):

```csharp
comprobante.Folio = factura.folio = (factura.idVenta.Equals("0") || string.IsNullOrEmpty(factura.idVenta)
    ? "PE" + factura.idPedidoEspecial
    : factura.idVenta);
```

- Las facturas de **pedidos especiales** ya llevan folio alfanumérico (`"PE12345"`) y se timbran así desde hace años. Es la prueba empírica de que **el SAT sí acepta folios alfanuméricos** (CFDI 4.0: `Folio` opcional hasta 40 caracteres, `Serie` opcional hasta 25).
- El identificador fiscal único ante el SAT es el **UUID**, no el folio. El SAT no valida unicidad de Serie+Folio; es responsabilidad del contribuyente.

**Dos problemas detectados que hay que resolver antes de tocar `Ventas`:**

1. **No se usa `Serie`.** `SP_FACTURACION_OBTENER_CONFIGURACION_COMPROBANTE` no devuelve columna `Serie` — se emite sin ella. Es el campo natural para discriminar ejercicio o sucursal (ej. serie `"2026"`).
2. **La tabla `Facturas` no guarda el folio.** Sus columnas son `idFactura, idVenta, idUsuarioFacturacion, fechaTimbrado, fecha, UUID, idEstatusFactura, msjErrorFacturacion, fechaCancelacion, idUsuarioCancelacion, msjErrorCancelacion, pathArchivoFactura`. El folio se **recalcula desde `idVenta`** cada vez que se necesita, en vez de persistirse. Mientras `idVenta` no cambie no hay problema, pero significa que no hay registro de qué folio se mandó realmente al SAT, y que si algún día se archiva/borra la venta, el folio deja de ser reproducible.

---

## 3. Diagnóstico de tamaño

### Top tablas por espacio (3.0 GB asignados totales)

| Tabla | Filas | MB | Naturaleza |
|---|---|---|---|
| VentasDetalle | 3,548,336 | 515.5 | Transaccional |
| **InventarioDetalleLog** | 5,364,501 | **483.1** | Bitácora |
| **InventarioGeneralLog** | 4,326,269 | **243.9** | Bitácora |
| Ventas | 761,560 | 107.9 | Transaccional |
| PedidosEspecialesMovimientosDeMercancia | 705,976 | 53.9 | Transaccional |
| **InventarioDetalleLog_resp20220220** | 597,837 | **45.6** | Basura |
| **InventarioDetalleLogRespPE2** | 526,890 | **40.6** | Basura |
| **InventarioDetalleLogRespPE** | 521,161 | **39.7** | Basura |
| TicketsPedidosEspecialesDetalle | 278,643 | 39.3 | Transaccional |
| MovimientosDeMercancia | 439,116 | 30.8 | Transaccional |
| ... (ver `00_diagnostico.sql` para el top 20 completo) | | | |

- **Bitácoras de inventario** (`InventarioDetalleLog` + `InventarioGeneralLog`): 727 MB (24% de la base). **Sin tablas hijas por FK** — nada depende de ellas.
- **Tablas de respaldo manual muertas** (`*_resp*`, `*RespPE*`, 10 tablas en total, ~128 MB). Verificado que **ningún stored procedure ni código C#** las referencia.
- **Espacio ya libre sin borrar nada:** el archivo de datos está en 3000 MB asignados pero solo 2143.5 MB usados → **856.5 MB libres dentro del archivo**, recuperables con un `DBCC SHRINKFILE` sin tocar una sola fila.

### Volumen y crecimiento

| Año | Ventas | Facturas |
|---|---|---|
| 2021 | 82,598 | 656 |
| 2022 | 130,993 | 2,848 |
| 2023 | 145,967 | 3,098 |
| 2024 | 158,300 | 3,286 |
| 2025 | 164,072 | 3,759 |
| 2026 (parcial) | 79,630 | 1,914 |

Crecimiento ~15%/año. 97 foreign keys en total en la base — el grafo de relaciones transaccionales es denso (`Ventas` es padre de `VentasDetalle`, `Complementos`, `ComplementosDetalle`, `Devoluciones`, `DevolucionesDetalle`, `ClientesAtendidosRuta`, `PedidosEspecialesDetalle`).

### Restricción legal

Las facturas **no se archivan por antigüedad libremente**: Art. 30 CFF obliga a conservar contabilidad 5 años. `Facturas`, `Complementos` y los XML en `pathArchivoFactura` se **archivan a una base histórica**, nunca se borran sin más.

---

## 4. Plan de acción por fases

### Fase 0 — Eliminar tablas muertas (~128 MB, riesgo cero)

Script: [`01_fase0_drop_tablas_muertas.sql`](01_fase0_drop_tablas_muertas.sql)

Verificado contra la copia local:
- Ningún `sys.sql_modules` (stored procedures) referencia estas tablas.
- Ninguna FK entra o sale de ellas.

10 tablas a eliminar, listadas una por una (nunca `DROP` dinámico por patrón, para no arriesgar borrar la tabla viva por error de `LIKE`):
`InventarioDetalleLog_resp20220220`, `InventarioDetalleLogRespPE2`, `InventarioDetalleLogRespPE`, `InventarioDetalle_resp20220220`, `InventarioDetalleRespPE2`, `InventarioDetalleRespPE`, `PedidosEspecialesMovimientosDeMercanciaRespPE2`, `PedidosEspecialesMovimientosDeMercanciaRespPE`, `InventarioGeneral_resp20220220`, `Estaciones_resp`.

### Fase 1 — Purgar bitácoras de inventario anteriores al cutoff (~490 MB)

Script: [`02_fase1_purga_bitacoras.sql`](02_fase1_purga_bitacoras.sql)

Cutoff: `2025-01-01` (política año actual + anterior).

| Tabla | Filas totales | Filas a borrar (< 2025) |
|---|---|---|
| InventarioDetalleLog | 5,364,501 | 3,622,335 |
| InventarioGeneralLog | 4,326,269 | 2,935,529 |

**Detalle técnico importante:** `InventarioGeneralLog` no tiene índice sobre `fechaAlta`, así que borrar filtrando por fecha escanearía las 4.3M filas en cada lote. Se verificó que el id (PK, índice clustered) y la fecha van en **orden perfectamente correlacionado** (cero filas desordenadas en ambas tablas), así que el borrado se hace por rango de PK en vez de por fecha — mismo resultado, mucho más rápido. El script revalida esta correlación en producción antes de ejecutar cualquier borrado.

Ejecución: lotes de 5,000 filas, cada uno en su propia transacción corta, con pausa entre lotes para no bloquear el POS en vivo. Es reanudable: si se corta a la mitad, lo ya borrado queda borrado y se puede volver a correr el script.

**Pre-requisito verificado:** `recovery model` de la base es `SIMPLE` (no `FULL`), lo cual hace segura la purga masiva sin inflar el log de transacciones. Confirmar este mismo dato en producción antes de purgar (puede diferir del restore local).

### Fase 1.5 — Reclamar espacio

Script: [`03_fase1_post_reclamar_espacio.sql`](03_fase1_post_reclamar_espacio.sql)

Orden obligatorio: **primero shrink, después reindex** (si se reindexa antes, el shrink fragmenta todo de nuevo y se tira el trabajo).

1. `DBCC SHRINKFILE` — recupera el espacio físico del archivo `.mdf`. **Ya hay 856 MB libres disponibles sin haber purgado nada todavía**, solo por espacio muerto de borrados anteriores.
2. Reconstrucción selectiva de índices con >30% de fragmentación y >1000 páginas (evita perder tiempo en catálogos pequeños).
3. `sp_updatestats` — obligatorio después de borrar millones de filas; si no se corre, el optimizador usa estadísticas viejas y el sistema se siente lento sin causa aparente.

**Nota SmarterASP:** en hosting compartido el usuario de la base puede no tener permiso para `DBCC SHRINKFILE`. Si sale error de permisos, hay que pedirlo a soporte. La purga igual sirve aunque no se pueda encoger el archivo: el espacio queda libre *dentro* del archivo y la base deja de crecer.

### Fase 2 — Blindar el folio del CFDI (prerequisito antes de archivar `Ventas`)

No tiene script aún — pendiente de implementar en C#/SQL. Pasos:

1. `ALTER TABLE Facturas ADD serie varchar(25) NULL, folio varchar(40) NULL;`
2. Backfill de las facturas existentes replicando la regla actual: `folio = isnull(cast(idVenta as varchar), 'PE' + cast(idPedidoEspecial as varchar))`.
3. Modificar `FacturaController.cs` para **persistir** serie y folio al momento de timbrar, en vez de recalcularlos desde `idVenta` cada vez.
4. Introducir `Serie` (ej. año `"2026"` o sucursal) — abre la puerta a archivar ventas sin ambigüedad de folio.
5. Índice único en `(serie, folio)` para blindar contra duplicados por bug.

Esto es lo que permite que, más adelante, una venta archivada/borrada de la base viva no le quite a `Facturas` la capacidad de reimprimir o cancelar un CFDI.

### Fase 3 — Archivado de tablas transaccionales (pendiente, la más grande)

No tiene script aún. Alcance:

1. Base histórica separada (`comercializadora_historico`) en el mismo servidor, o `.bak` descargable si SmarterASP cobra por base adicional.
2. Copiar y luego borrar respetando el orden del grafo de FKs (hijos primero): `VentasDetalle` → `DevolucionesDetalle` → `Devoluciones` → `ComplementosDetalle` → `Complementos` → `ClientesAtendidosRuta` → `PedidosEspecialesDetalle` → `Ventas`.
3. **`Facturas` nunca se borra** (obligación fiscal 5 años) — por eso la Fase 2 es prerequisito: una vez que el folio vive en `Facturas` y no depende de `Ventas`, archivar la venta no rompe la factura.
4. Vista o pantalla de consulta de solo lectura contra el histórico, para cuando se necesite reimprimir o auditar una venta vieja.

### Fase 4 — Que no se repita

1. Job mensual automatizado de purga de bitácoras (repetir la lógica de la Fase 1 con cutoff móvil).
2. Job anual de archivado (Fase 3) al cerrar cada ejercicio.
3. Alerta de espacio al 80% de la cuota de 10 GB del hosting.

---

## 5. Estado actual y siguiente paso

| Fase | Script | Estado |
|---|---|---|
| Diagnóstico | `00_diagnostico.sql` | Listo, probado en local |
| Fase 0 (tablas muertas) | `01_fase0_drop_tablas_muertas.sql` | Verificación probada en local; `DROP` comentado, pendiente de correr en producción |
| Fase 1 (purga bitácoras) | `02_fase1_purga_bitacoras.sql` | Pre-vuelo probado en local; purga comentada, pendiente |
| Fase 1.5 (reclamar espacio) | `03_fase1_post_reclamar_espacio.sql` | Diagnóstico probado en local; shrink/reindex comentado, pendiente |
| Fase 2 (folio persistente) | *(sin script aún)* | Pendiente de diseñar/implementar |
| Fase 3 (archivado transaccional) | *(sin script aún)* | Pendiente, depende de Fase 2 |
| Fase 4 (jobs recurrentes) | *(sin script aún)* | Pendiente, depende de Fases 1–3 |

**Orden recomendado de ejecución:**

1. Backup completo de producción, restaurado y verificado en local.
2. Correr `00_diagnostico.sql` en **producción** para confirmar recovery model y espacio real (puede diferir de la copia local).
3. `03` paso shrink (sin purgar nada): ~856 MB recuperables de inmediato.
4. `01` (tablas muertas): ~128 MB.
5. `02` (purga bitácoras): ~490 MB, en horario de baja venta.
6. `03` completo otra vez (shrink + reindex + estadísticas).

**Proyección de esta primera ronda (Fases 0–1.5):** de 3.0 GB asignados a **~1.5 GB** (de 30% a 15% de la cuota de 10 GB).

La Fase 3 (archivar `Ventas` anteriores a 2025, que son el 68% de los registros históricos) liberaría otros ~500 MB adicionales, pero requiere primero completar la Fase 2.
