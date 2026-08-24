# Fase 2 — Serie + folio persistente e índice único (habilitador del reseteo de IDENTITY)

**Fecha:** 2026-08-06
**Base analizada:** `DB_A57E86_comercializadora` (copia local restaurada de producción)
**Objetivo:** dejar el folio del CFDI grabado y protegido por un índice único, para que reusar
un `idVenta` / `idPedidoEspecial` después de un reseed deje de poder pisar un CFDI ya timbrado.

---

## 1. Las tablas de facturas que existen realmente

| Tabla | Filas | PK (identity) | Ligada a | Folio que se manda al SAT |
|---|---|---|---|---|
| `Facturas` | 15,564 | `idFactura` bigint | `idVenta` | `cast(idVenta as varchar)` |
| `FacturasPedidosEspeciales` | 3,114 | `idFacturaPedidoEspecial` bigint | `idPedidoEspecial` | `'PE' + idPedidoEspecial` |
| `FacturasAbonos` | **0** | `idFacturAbono` bigint | `idPedidoEspecial`, `idFacturaPedidoEspecial` | — |

- **`FacturasAbonos` está muerta**: cero filas y ningún stored procedure ni código C# la referencia.
  Candidata a `DROP` en la Fase 0, no hay que darle serie ni folio.
- `Complementos` / `ComplementosDetalle` **no son CFDI** — son complementos de venta (agregar
  productos a una venta ya hecha). No emiten timbre. Fuera de alcance.
- Las dos tablas vivas son **estructuralmente gemelas**: mismas 12 columnas, cambiando solo la
  llave de negocio (`idVenta` vs `idPedidoEspecial`). Todo lo que se haga en una se hace igual en la otra.

### Distribución por año

| Año | `Facturas` | `FacturasPedidosEspeciales` |
|---|---|---|
| 2020 | 3 | — |
| 2021 | 656 | — |
| 2022 | 2,848 | 537 |
| 2023 | 3,098 | 668 |
| 2024 | 3,286 | 744 |
| 2025 | 3,759 | 767 |
| 2026 (parcial) | 1,914 | 398 |

---

## 2. Hallazgos que cambian el plan

### 2.1 Las tablas de facturas hacen UPSERT por la llave de negocio — este es el riesgo real

`SP_FACTURACION_INSERTA_FACTURA` no inserta siempre; primero busca:

```sql
if exists ( SELECT 1 FROM Facturas WHERE idVenta = @idVenta )
begin
        update Facturas set
        fechaTimbrado = @fechaTimbrado,
        UUID = @UUID,                      -- <<<<< pisa el UUID del CFDI anterior
        idEstatusFactura = @idEstatusFactura,
        ...
        msjErrorCancelacion = 'Refacturada al ' + convert(varchar, dbo.FechaActual(), 22)
        where idVenta = @idVenta
end
```

`SP_FACTURACION_PEDIDOS_ESPECIALES_INSERTA_FACTURA` es idéntico con `idPedidoEspecial`.

Dos consecuencias:

1. **Hoy ya se pierde historia.** Una refacturación sobrescribe el UUID anterior. Solo queda la
   leyenda `'Refacturada al ...'` en `msjErrorCancelacion`, sin rastro del UUID viejo. Por eso hay
   cero duplicados de `idVenta` — no es que no pase, es que la tabla no puede representarlo.
2. **Con el IDENTITY reseteado esto es destructivo.** Si `Ventas` vuelve a emitir `idVenta = 5`,
   la nueva factura entra por la rama `update` y **sobrescribe el CFDI de la venta 5 de 2023**:
   UUID, fecha de timbrado y ruta del PDF. La factura vieja deja de existir en la base aunque el
   CFDI siga vivo ante el SAT.

**Por eso el índice único solo no basta**: hay que cambiar la llave del upsert a `(serie, folio)`.
Con la llave vieja el upsert nunca choca contra el índice — simplemente pisa la fila.

### 2.2 No hay FK entre `Facturas` y `Ventas`

Ni entre `FacturasPedidosEspeciales` y `PedidosEspeciales`. La relación es por convención.
Ventaja: archivar `Ventas` no rompe integridad declarativa. Desventaja: ya hay **4 facturas
huérfanas** (`idVenta` que no existe en `Ventas`). El backfill igual las cubre porque el folio sale
del `idVenta` guardado, no de la venta.

### 2.3 Los únicos índices son las PK

`Facturas` y `FacturasPedidosEspeciales` no tienen ni un índice sobre su llave de negocio. Cada
upsert, cada cancelación y cada reimpresión hace scan completo. Al agregar el índice único conviene
agregar también el índice sobre `idVenta` / `idPedidoEspecial`.

### 2.4 El discriminador venta-vs-pedido es el string del folio

En [`FacturaDAO.cs:57`](../../lluviaBackEnd/lluviaBackEnd/DAO/FacturaDAO.cs#L57) y
[`:221`](../../lluviaBackEnd/lluviaBackEnd/DAO/FacturaDAO.cs#L221):

```csharp
string sp = f.folio.Contains("PE") ? "SP_FACTURACION_PEDIDOS_ESPECIALES_INSERTA_FACTURA"
                                   : "SP_FACTURACION_INSERTA_FACTURA";
```

El sistema decide a qué tabla escribir **leyendo el texto del folio**. Cualquier cambio de formato
de folio (prefijo de serie, ceros a la izquierda, folio con año) rompe el ruteo silenciosamente y
manda facturas de pedido especial a la tabla de ventas. Hay que cambiarlo a `idPedidoEspecial != 0`
antes de tocar el formato del folio. La misma línea está duplicada en
[`PrintDocumentolluvia/DAO/FacturaDAO.cs:189`](../../PrintDocumentolluvia/PrintDocumentolluvia/DAO/FacturaDAO.cs#L189).

### 2.5 `FactConfiguracionComprobante` no tiene columna `Serie`

Columnas actuales: `contador, Version, Moneda, TipoDeComprobante, MetodoPago, LugarExpedicion,
RegimenFiscal, Rfc, Nombre, Telefono, Domicilio, Exportacion`. Tabla de un solo renglón. Es el lugar
natural para la serie vigente.

### 2.6 Estado de partida: limpio

Verificado en la copia local — **cero conflictos** para crear el índice único hoy:

| Verificación | Resultado |
|---|---|
| `idVenta` duplicados en `Facturas` | 0 |
| `idPedidoEspecial` duplicados en `FacturasPedidosEspeciales` | 0 |
| UUID duplicados (ambas tablas) | 0 |
| `idVenta` / `idPedidoEspecial` nulos | 0 |
| `fecha` nula | 0 |
| Longitud máxima del folio derivado | 6 (ventas) / 7 (PE) — el límite del SAT es 40 |

Hay 2 valores numéricos que existen a la vez como `idVenta` y como `idPedidoEspecial`; el prefijo
`PE` es lo único que hoy los separa. Es la razón por la que el prefijo se conserva en el backfill.

---

## 3. Diseño del folio

```
serie  varchar(25) NOT NULL     -- ejercicio / ciclo de folios
folio  varchar(40) NOT NULL     -- lo que se manda en el atributo Folio del CFDI
```

**Regla de oro:** *la serie cambia exactamente cuando se resetea el IDENTITY.* Esa es la única
invariante que hace seguro el reseed. Nada más.

**Backfill histórico: `serie = ''` (cadena vacía, no NULL).**

Las 18,678 facturas existentes se timbraron **sin atributo `Serie`** — `SP_FACTURACION_OBTENER_
CONFIGURACION_COMPROBANTE` nunca lo devolvió, así que el XML salió sin ese nodo. Grabar `''`
registra fielmente lo que se mandó al SAT. Grabar `'A'` sería inventar un dato fiscal que nunca
existió en el CFDI.

NULL no sirve: SQL Server trata varios NULL como iguales en un índice único, así que dos folios sin
serie colisionarían.

A partir del corte, la serie sale de `FactConfiguracionComprobante.Serie` (`'A'` para el primer
ciclo, `'B'` para el que arranca después del primer reseed, y así). El SAT acepta `Serie` de hasta
25 caracteres y no valida unicidad de Serie+Folio — la responsabilidad es del contribuyente, que es
justamente lo que este índice cubre.

**El folio no cambia de formato:** sigue siendo `idVenta` para ventas y `'PE' + idPedidoEspecial`
para pedidos especiales. Cambiar formato y agregar serie a la vez duplica el riesgo sin beneficio;
la serie ya resuelve la colisión.

**Unicidad cruzada entre tablas:** dentro de una serie, `Facturas` da folios numéricos y
`FacturasPedidosEspeciales` da folios que empiezan con `PE` — no se pueden encimar. Se blinda con un
`check` que impide que un folio de `Facturas` empiece con `PE`, para que no dependa de la disciplina
del programador.

---

## 4. Etapas

### Etapa A — Columnas, backfill e índice único *(no cambia comportamiento)*

Script: [`04_fase2_serie_folio.sql`](04_fase2_serie_folio.sql)

1. Pre-vuelo: reconfirmar en **producción** los cero duplicados de la sección 2.6.
2. `alter table` para agregar `serie` / `folio` NULL en ambas tablas.
3. Backfill: `serie = ''`, `folio = cast(idVenta as varchar)` / `'PE' + cast(idPedidoEspecial as varchar)`.
4. Subir a `NOT NULL`.
5. `check` de formato (`Facturas.folio` no empieza con `PE`).
6. Índice único `UQ_<tabla>_serie_folio` sobre `(serie, folio)`.
7. Índice no único sobre `idVenta` / `idPedidoEspecial` (hoy no existe ninguno).
8. `Serie varchar(25) not null default ''` en `FactConfiguracionComprobante`.

Reversible con `drop index` / `drop column`. No toca una sola fila de datos existentes ni requiere
desplegar código. **Se puede hacer hoy.**

### Etapa B — Que el timbrado escriba serie y folio

Sin esto, cada factura nueva entra con `folio = ''` y la segunda choca contra el índice único.
**La Etapa B tiene que salir a producción junto con la Etapa A, o inmediatamente después.**

1. `SP_FACTURACION_OBTENER_CONFIGURACION_COMPROBANTE` devuelve la nueva columna `Serie`; el modelo
   `Comprobante` ya tiene la propiedad `Serie`, solo hay que dejar que se llene.
2. `SP_FACTURACION_INSERTA_FACTURA` recibe `@serie` y `@folio`, los graba, y **cambia la llave del
   upsert**:

   ```sql
   -- antes
   if exists (select 1 from Facturas where idVenta = @idVenta)
       update Facturas set ... where idVenta = @idVenta

   -- después
   if exists (select 1 from Facturas where serie = @serie and folio = @folio)
       update Facturas set ... where serie = @serie and folio = @folio
   ```

   Igual en `SP_FACTURACION_PEDIDOS_ESPECIALES_INSERTA_FACTURA` y en las dos variantes
   `..._INSERTA_FACTURA_CANCELADA...`.
3. `FacturaDAO.GuardarFactura` y `CancelarFactura` mandan `@serie` / `@folio`.
4. Cambiar el ruteo `f.folio.Contains("PE")` por `f.idPedidoEspecial != 0` (sección 2.4).
5. **Espejar todo en `PrintDocumentolluvia`** — `Form1.cs:980` timbra con la misma regla de folio y
   `DAO/FacturaDAO.cs` llama a los mismos SPs. Si se cambia la firma de los SPs y no se espeja, la
   impresora deja de facturar.

### Etapa C — Que las consultas dejen de buscar por `idVenta`

Estos objetos localizan la factura por la llave de negocio y, después del reseed, encontrarían la
factura vieja del id reciclado:

- `SP_FACTURACION_OBTENER_DATOS_FACTURA` / `..._PEDIDO_ESPECIAL` (reimpresión y reenvío)
- `SP_OBTENER_CANCELACION_FACTURA` / `SP_FACTURACION_OBTENER_CANCELACION_FACTURA`
- `SP_FACTURAS_OBTENER_PATH_ARCHIVO`
- `SP_CONSULTA_FACTURAS`, `SP_V2_CONSULTA_FACTURAS`, `SP_FACTURACION_OBTENER_FACTURAS_PEDIDOS_ESPECIALES`
  (agregar `serie` y `folio` a la salida y mostrarlos en la pantalla de facturas)

Regla: mientras la venta viva en la base actual, buscar por `idVenta` sigue funcionando; en cuanto
haya dos ciclos de folios conviviendo, cualquier búsqueda por `idVenta` es ambigua. Todas se
resuelven filtrando además por `serie`, o llevando `idFactura` desde la pantalla.

**Bug aparte detectado:** `FacturaController.RegenerarFactura` usa `items` sin inicializarlo
(`Dictionary<string, object> items = null;` y luego `items["conceptosAddenda"]`) → revienta con
`NullReferenceException` siempre. Además arma la ruta como `"Timbre_" + idVenta + ".xml"`, sin el
timestamp con el que sí se graba el archivo en `GenerarFactura`. Esa acción está muerta hoy; hay que
decidir si se arregla o se elimina antes de que estorbe.

### Etapa D — El reseteo del IDENTITY

Solo después de A, B y C, y **después del archivado de la Fase 3**. Un `DBCC CHECKIDENT` sobre
`Ventas` con las filas viejas todavía adentro provoca violación de PK en la primera venta nueva; el
reseed exige que el rango a reusar esté vacío.

Secuencia del corte:

1. Archivar y sacar de `Ventas` (y sus hijas) todo lo anterior al cutoff — Fase 3.
2. `update FactConfiguracionComprobante set Serie = 'B'` (la serie nueva).
3. `DBCC CHECKIDENT('Ventas', RESEED, 0)` — y lo equivalente en `PedidosEspeciales`.
4. Correr la verificación post-corte del script (`(serie, folio)` sin duplicados, ninguna factura de
   la serie nueva apuntando a un `idVenta` de la serie vieja).

A partir de ahí `idVenta` vuelve a 1 y el folio `1` de la serie `B` convive sin ambigüedad con el
folio `1` de la serie anterior.

---

## 5. Orden y estado

| Etapa | Qué hace | Riesgo | Estado |
|---|---|---|---|
| A | columnas + backfill + índice único | Bajo, reversible | Script listo, DDL comentado |
| B | timbrado graba serie/folio; llave del upsert | Medio — toca timbrado en vivo | Pendiente |
| C | consultas/cancelación/reimpresión por serie | Medio | Pendiente |
| D | corte de serie + reseed | Alto, irreversible | Pendiente, depende de Fase 3 |

**Siguiente paso concreto:** correr el PASO 1 de `04_fase2_serie_folio.sql` contra **producción**
(solo lectura) para confirmar que ahí también hay cero duplicados. Si sale limpio, la Etapa A se
puede aplicar en la misma ventana, porque no cambia comportamiento — pero la Etapa B tiene que estar
lista para desplegarse enseguida.
