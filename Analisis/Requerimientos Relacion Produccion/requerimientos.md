# Relación Producción — Requerimientos (borrador)

Estado: borrador, sin implementar. Modelo base: pantalla "Relación Líquidos" (`ProductosAgranelAEnvasar`) y formulario "Producción LIQ." de la HH (`LiquidosActivity`).

## 1. Objetivo

Definir la fórmula de producción de cada producto líquido (qué materias primas y cuánto de cada una se necesita para producir un lote) y usarla en la HH para que, al registrar lotes producidos, el sistema descuente las materias primas y dé de alta el producto terminado.

## 2. WMS — pantalla "Relación Producción"

### 2.1 Datos

**Encabezado (una fórmula por producto)**
- Producto a producir: productos activos de la línea 20 (LIQUIDOS).
- Unidad de medida del producto: se toma automáticamente de `Productos.idUnidadMedida` (nombre en `CatUnidadMedida`); no es editable (confirmado). Ej.: SUAVI AZUL (822) = Litro.
- Cantidad a producir (lote): decimal mayor a 0. Ejemplo: SUAVI AZUL, Litro, 210.

**Detalle (N materias primas por fórmula)**
- Materia prima: productos activos de la línea 12 (Linea MPL).
- Unidad de medida: se toma automáticamente del producto (`Productos.idUnidadMedida`); no es editable ni se convierte.
- Cantidad de MPL por lote: decimal mayor a 0. Ejemplo: Pasta para suavizante 10 kg, Fragancia suavi azul 1.5 kg, Formol 0.4, Color azul 0.005.

### 2.2 Reglas
- Un producto a producir solo puede tener una fórmula activa (restricción de unicidad).
- Una misma materia prima **no se repite** dentro de la misma fórmula (confirmado).
- Una fórmula debe tener al menos una materia prima.
- Alta, **edición y eliminación** (confirmado). La eliminación es baja lógica (`activo = 0`), igual que Relación Líquidos, porque las producciones ya registradas guardan su propia copia de las cantidades y no deben cambiar. Al eliminar la fórmula se puede volver a crear una nueva para el mismo producto.
- Listado en tabla DataTables con exportación a PDF y Excel; los títulos deben decir "Relación Producción" (no copiar el "Clientes" de la pantalla original).
- Seguir el patrón existente: `Ajax.BeginForm`, `PartialView` para la tabla, lógica en SPs, respuesta `Notificacion<T>` (primer result set = status).
- Entrada de menú junto a "Relación Líquidos" en el menú Productos de `_Layout.cshtml`.
- **Permiso propio** nuevo en `EnumRolesPermisos` (`Models/Enumeraciones.cs`), asignado únicamente al rol Administrador (`idRol` 1). El menú se oculta con `sesion.permisosModulo.Exists(...)` y el alta del permiso se hace en `PermisosRolPorModulo`/catálogo de módulos. Recordar que `[PermisoAttribute]` no bloquea de verdad el acceso por URL, así que la acción del controlador debe validar el permiso explícitamente.

## 3. HH (androidMobileLluvia) — sección de producción por fórmula

### 3.1 Flujo
1. El operador busca o escanea el producto a producir (línea 20), como en "Producción LIQ.".
2. Si el producto no tiene fórmula, la HH lo indica y no permite guardar.
3. El operador captura el **número de lotes** (mayor a 0).
4. La HH muestra la cantidad resultante = lotes × cantidad del lote (ej. 2 lotes = 420 L) y el consumo de cada MPL (cantidad × lotes).
5. Al guardar, el backend ejecuta todo en una sola transacción.

### 3.1.1 Pestañas
- **Agregar:** captura descrita arriba.
- **Historial:** igual que "Producción LIQ." (filtro por fecha inicio/fin, lista de producciones del usuario). Debe mostrar producto, lotes, cantidad producida y fecha. Cada usuario ve sus propias producciones (confirmado).

Nota técnica: el historial actual (`SP_CONSULTA_CARGA_MERCANCIA_LIQUIDOS`) lee `InventarioDetalleLog` filtrado fijo a los tipos de movimiento 26 y 27, unido con `Usuarios`, `CatRoles`, `CatTipoMovimientoInventario`, `Productos` e `InventarioDetalle`/`Ubicacion`/`Almacenes`. Los tipos de movimiento nuevos no aparecerían ahí sin modificarlo, y esa tabla no guarda el número de lotes.

### 3.2 Reglas de inventario (en una transacción; se toma como modelo `SP_APP_AGREGAR_PRODUCTO_INVENTARIO_LIQUIDOS_ENVASADO`)
- Validar antes de mover nada que existe inventario suficiente de **cada** MPL en el almacén del usuario; si falta alguna, **se rechaza toda la producción** (confirmado) e indicar cuáles faltan.
- **Tope de lotes (confirmado):** el número de lotes por producción no puede ser mayor a 100 (100 exacto se acepta). Se valida en la HH, en el servicio web y en el SP. Aparte, el tamaño de la fórmula (`cantidadLote`) tiene un tope de 10,000 en el WMS para atrapar errores de captura.
- **No se puede revertir** una producción ya guardada (confirmado): no hay cancelación ni devolución de MPL desde la HH ni el WMS. La confirmación en la HH debe dejar claro que el guardado es definitivo.
- Descontar cada MPL de cualquier ubicación del almacén, empezando por la de mayor cantidad. Sin ubicación de resguardo (1002).
- Dar de alta el producto terminado (cantidad = lotes × cantidad del lote) en la ubicación 0-0-0 (sin acomodar) del almacén del usuario, creándola si no existe, igual que Envasado. Confirmado.
- Usar **tipos de movimiento nuevos** en `CatTipoMovimientoInventario` (el id es identity; hoy el máximo es 36), sin reutilizar los existentes:
  - `Actualizacion de Inventario(carga de producto terminado por proceso de produccion de formula)` (`operacion = 1`).
  - `Actualizacion de Inventario(salida de materia prima por proceso de produccion de formula)` (`operacion = -1`).
  - Nota: el tipo 28 existente es una salida pero tiene `operacion = 1`; no copiar esa inconsistencia.
- Registrar `InventarioDetalleLog`, `InventarioGeneral` e `InventarioGeneralLog` para cada movimiento.
- Bloquear el guardado si hay inventario físico activo (como en las pantallas actuales; confirmado).

### 3.3 Acceso
- Visible para el rol 13 y para el administrador (`idRol` 1 = Administrador, 13 = Encargado de Producción (líquidos)).
- Agregar el ítem al menú de todas las Activities (el menú está copiado en cada una, ~12) y registrar la nueva Activity en el manifest.
- Nuevo endpoint `api/<Controller>/<Action>` en `API.java` y controlador en `WebServices/`.

## 3.4 Tabla de registro de producciones (nueva)

Cada producción guarda un registro propio para el historial (lotes incluidos), sin sustituir a `InventarioDetalleLog`, que sigue siendo la fuente de verdad del inventario.

Nombres y estándares (tomados de las tablas existentes: `ProcesoProduccionAgranel`, `VentasDetalle`, `ComprasDetalle`):
- Tablas en PascalCase, detalle con sufijo `Detalle`, PK `id<Tabla>` con constraint `PK_<Tabla>`, FKs `FK_<Tabla>_<Referenciada>` (sin cascada), fechas `fechaAlta` y `fechaUltimaActualizacion`, columnas `idUsuario` e `idAlmacen`, PK de procesos `bigint identity`.
- **Encabezado: `ProcesoProduccion`** — `idProcesoProduccion` (bigint identity, PK; es el mismo nombre que la columna nueva de `InventarioDetalleLog`), `idProducto` (producido), `idProcesoProduccionFormula` (fórmula usada), `lotes`, `cantidadLote` (copia al momento), `cantidadProducida`, `idUnidadMedida`, `idUsuario`, `idAlmacen`, `fechaAlta`.
- **Detalle: `ProcesoProduccionDetalle`** — `idProcesoProduccionDetalle` (bigint identity, PK), `idProcesoProduccion` (FK), `idProducto` (materia prima), `cantidadPorLote` (copia), `cantidadConsumida` (total), `idUnidadMedida`.
- Tablas de la fórmula (pantalla Relación Producción, sección 2), decidido: **`ProcesoProduccionFormula`** (encabezado: producto a producir, unidad, cantidad del lote, `activo`) y **`ProcesoProduccionFormulaDetalle`** (materias primas y cantidad por lote). PKs `idProcesoProduccionFormula` e `idProcesoProduccionFormulaDetalle`, mismo estándar de constraints.
- Se guarda una **copia** de las cantidades de la fórmula, porque la fórmula puede editarse después y el historial no debe cambiar.

### Reglas para evitar discrepancias con `InventarioDetalleLog`
1. El registro y los movimientos de inventario se escriben en la **misma transacción**: si falla uno, no queda ninguno.
2. La cantidad producida y la consumida se calculan **una sola vez** en el SP (con `dbo.redondear`) y de esa misma variable salen tanto el registro nuevo como el movimiento de inventario.
3. Cantidades consumidas por MPL = suma de los movimientos de salida en `InventarioDetalleLog` de esa producción (la salida puede repartirse en varias ubicaciones, por eso se suma).
4. Vínculo verificable: nueva columna **`idProcesoProduccion`** en `InventarioDetalleLog` (decidido), más una consulta de conciliación que compare ambos lados y devuelva cero filas cuando todo cuadra.

### Columna `InventarioDetalleLog.idProcesoProduccion`
- Tipo `bigint NULL`, mismo patrón que las columnas existentes `idPedidoEspecial` e `idProcesoProduccionAgranel` (ambas `bigint NULL`). Sin default, sin FK obligatoria sobre datos históricos.
- Solo se llena en los movimientos generados por esta feature (alta del terminado y salidas de MPL); en el resto de movimientos queda `NULL`.
- **Descripción de la columna** (extended property `MS_Description`) para saber después de qué es, por ejemplo: "Id del registro de producción por fórmula (Relación Producción) que originó este movimiento. NULL si el movimiento no proviene de esa función. No confundir con idProcesoProduccionAgranel."
- El script debe ser idempotente (`IF NOT EXISTS` sobre `sys.columns`) y llevar la verificación previa/posterior, como los demás scripts de `Analisis/`.
- Verificado en la BD local: la tabla no tiene triggers y ningún módulo hace `INSERT INTO InventarioDetalleLog VALUES` sin lista de columnas, así que agregar la columna nullable no rompe inserts existentes. Revisar lo mismo contra producción antes de aplicar.
5. El historial en la HH lee la tabla nueva; el inventario real nunca se lee de ella.

## 3.5 Impacto en reportes existentes (revisado en BD local)

- **Consumo de MPL → MPL Agrupado** (`Reportes/CostoProduccionAgranel`, `SP_CONSULTA_COSTO_PRODUCCION`): arma `ReporteCostoProduccion` desde `ProcesoProduccionAgranel` (estatus >= 3). **No lee `InventarioDetalleLog`**, por lo que los movimientos nuevos no aparecen.
- **Consumo de MPL → MPL Individual** (`ProduccionAgranel/MPLIndividual`, `SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO`): lee directamente `ProcesoProduccionAgranel`. Tampoco muestra los movimientos nuevos.
- **Historial de líquidos** (`SP_CONSULTA_CARGA_MERCANCIA_LIQUIDOS` y `SP_V2_CONSULTA_CARGA_MERCANCIA_LIQUIDOS`): filtran fijo los tipos 26 y 27; los tipos nuevos no aparecen salvo que se pida el tipo exacto.
- **Inventario promedio** (`SP_CALCULA_INVENTARIO_PROMEDIO_PRODUCTO`): filtra los tipos 2 y 4; no se ve afectado.
- Conclusión: para ver la producción por fórmula en los reportes de Consumo de MPL hay que **modificar esos SPs** o crear reportes nuevos que lean `ProcesoProduccion`/`ProcesoProduccionDetalle`.
- **Objetivo confirmado:** el consumo de MPL de la producción por fórmula **debe verse en MPL Agrupado y MPL Individual**.
- Hechos que condicionan el diseño: `ProcesoProduccionAgranel` contiene solo productos de la línea 12 (MPL); el costo se calcula como `cantidadAceptada × dbo.obtenerPrecioCompra(...)`; los estatus son 1-2 pendiente, 3 procesado correctamente, 4 rechazo total, 5 rechazo parcial, 6 cancelado por ajuste. La producción por fórmula no tiene flujo de aprobación, así que equivale a estatus 3 (cantidad = cantidad aceptada = `cantidadConsumida`).
- `ReporteCostoProduccion` es un caché mensual: el mes actual se recalcula en cada consulta; los meses cerrados quedan congelados.
- **Alcance (confirmado):** estos reportes muestran únicamente las **materias primas consumidas** (`ProcesoProduccionDetalle`); el producto terminado que se produce **no** debe aparecer en ellos.
- **Mecanismo (decidido):** modificar `SP_CONSULTA_COSTO_PRODUCCION` y `SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO` con un `UNION` hacia `ProcesoProduccionDetalle` (unido a `ProcesoProduccion` para la fecha). No se leerá `InventarioDetalleLog` para estos reportes y no se insertarán filas en `ProcesoProduccionAgranel`.
  - Cantidad solicitada = cantidad aceptada = `cantidadConsumida`; estatus mostrado "Procesado correctamente" (3); costo con `dbo.obtenerPrecioCompra`, igual que hoy.
  - MPL Individual: agregar una columna de origen para distinguir "Consumo MPL" de "Producción por fórmula".
  - MPL Agrupado: incluir las MPL consumidas en el cálculo mensual de `ReporteCostoProduccion` (el mes actual se recalcula en cada consulta; los meses cerrados no cambian).
  - Los SPs se modifican con script `ALTER`, con verificación de que los números existentes no cambian (comparar resultados antes y después con las mismas fechas).

## 3.6 Servicios web para la HH (nuevos)

La HH solo habla con el WMS a través de servicios web (`WebServices/`, rutas `api/<Controller>/<Action>`, respuesta `Notificacion<T>`). Hay que crear servicios nuevos; no se reutilizan los de "Producción LIQ.", que no conocen fórmulas ni descuentan MPL. Propuesta inicial (nombres por definir al implementar, siguiendo `WsInventarioController` / `AdminLiquidosController`):

1. **Buscar producto con fórmula:** recibe descripción o código de barras y el usuario; devuelve los productos de la línea 20 que tienen fórmula activa, con su unidad, cantidad del lote y las materias primas con cantidad por lote. Un producto sin fórmula se informa como tal.
2. **Guardar producción:** recibe producto, número de lotes, usuario y almacén; ejecuta el SP de producción (una sola transacción) y devuelve `Notificacion` con el resultado. Si falta inventario, el mensaje y el detalle indican qué materias primas faltan.
3. **Historial de producciones:** recibe usuario y rango de fechas; devuelve las producciones del usuario (producto, lotes, cantidad producida, fecha) desde `ProcesoProduccion`.

Cada servicio lleva su método en el DAO (Dapper), su SP y su manejo de errores con `WsUtils<T>.RegresaExcepcion`, y su `@POST` + clases `Request*`/`Notificacion*` en la app Android. Validar siempre en el servidor (usuario, almacén, rol y inventario físico activo), no solo en la HH. La pantalla de fórmulas del WMS no usa servicios web: es un controlador MVC con `PartialView`.

## 4. Pendientes por confirmar

1. ~~Ubicación del producto terminado~~ — resuelto: 0-0-0 (sin acomodar).
2. ~~Tipos de movimiento~~ — resuelto: crear nuevos (carga de terminado y salida de MPL). Descripciones definidas (ver sección 3.2).
3. ~~Id del rol administrador~~ — resuelto: `CatRoles.idRol = 1` (Administrador). Nombres en el menú de la HH (decidido): el ítem nuevo se llama **"PRODUCCIÓN"** (también como título de su pantalla) y el actual (`action_liquidos`, hoy "PRODUCCIÓN") se renombra **"PROD LIQUIDOS"**. Las dos pantallas conviven. El renombre aplica a la etiqueta del menú **y** al título de la pantalla actual (`titulo_liquidos` en `strings.xml`, hoy "PRODUCCIÓN LIQ." → "PROD LIQUIDOS"). Ícono: **nuevo** para "PRODUCCIÓN" (no reutilizar el de producción actual); lo agrega el usuario, no es parte del alcance de implementación.
4. ~~Permiso en el WMS~~ — resuelto: permiso propio, solo Administrador. Nombre: `Puede_visualizar_Relacion_Produccion`, con un comentario corto en el enum que explique que controla la pantalla Relación Producción (fórmulas de producción). En el enum el último id es 13, pero en `CatModulos` (BD local) el id 14 ya está usado por "Facturacion" (sin valor en el enum). El nuevo debe ser **15**; confirmado también en producción: `CatModulos` llega hasta el id 14 (Facturacion) y el 15 está libre. Alta en `CatModulos` y en `PermisosRolPorModulo` (`tienePermiso = 1` solo para `idRol` 1; el resto de roles en 0, como hacen los permisos existentes).
5. ~~Historial en la HH~~ — resuelto: pestaña Historial como "Producción LIQ.". El registro se persiste en `ProcesoProduccion` y `ProcesoProduccionDetalle` (sección 3.4).
6. ~~SP que llama hoy el rol 13~~ — verificado: HH `LiquidosActivity.agregarInventario` → `api/WsInventario/AgreagrProductoInventario` → `InventarioDAO.AgreagrProductoInventario` → **`SP_APP_AGREGAR_PRODUCTO_INVENTARIO`** (genérico, con `idProveedor = 90` fijo). Para productos de la línea 20 y usuario rol 13 asigna el tipo de movimiento 26; da de alta en la ubicación 0-0-0 (sin acomodar) y no descuenta nada. `SP_APP_AGREGAR_PRODUCTO_INVENTARIO_LIQUIDOS_PRODUCCION` no lo usa la HH (copia huérfana). La pantalla actual se mantiene sin cambios salvo el renombre a "PROD LIQUIDOS".
7. ~~"SUAVI AZUL"~~ — resuelto: existe como `idProducto` 822, línea 20, unidad Litro (id 3), activo. (Mi búsqueda previa lo omitió por un `top 6` mal ordenado, no porque faltara.) Ojo con homónimos: "SUAVI AZUL 1 LT." (2654) es de la línea 27 (envasado) y "SUAVI AZUL CONCENTRADO PARA 5" (3730) es de la línea 20 en Kg; el buscador de la HH debe mostrar la descripción completa y la unidad para no confundirlos.
9. ~~Vínculo con `InventarioDetalleLog`~~ — resuelto: columna `idProcesoProduccion bigint NULL` con descripción.
8. ~~Impacto en `PrintDocumentolluvia`~~ — resuelto: es la app de escritorio de impresión de tickets y timbrado; no tiene producción ni inventario de líquidos, sin cambios.
10. ~~Cancelar o revertir producciones~~ — resuelto: no es posible.
11. ~~Editar o eliminar fórmulas con producciones previas~~ — resuelto: sí; eliminación lógica, el historial conserva su copia.

12. ~~Integración en MPL Agrupado/Individual~~ — resuelto: `UNION` hacia `ProcesoProduccionDetalle` en ambos SPs (ver sección 3.5).

Confirmado en producción: `InventarioDetalleLog` tiene 0 triggers y ningún módulo hace `INSERT` sin lista de columnas; agregar la columna nullable `idProcesoProduccion` es seguro. El id 15 del permiso también está confirmado.

Abiertos: ninguno de requerimientos. (El ícono lo agrega el usuario.) y confirmar contra producción el id 15 del permiso y el estado de `InventarioDetalleLog` (triggers e inserts).
