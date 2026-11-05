# Contrato de implementación: Proceso Producción (fórmulas)

Documento compartido por los 4 agentes. Fuente de requisitos: `requerimientos.md` (misma carpeta). Si algo de este contrato choca con la realidad del código o de la BD, NO lo resuelvas por tu cuenta: anótalo en tu canal (sección 9) y avisa en tu reporte final.

Rama de trabajo en ambos repositorios: `feature/ProcesoProduccion`. Nadie hace commit, push ni cambia de rama.

## 0. Reglas generales

- Entorno de pruebas: SOLO la BD local `localhost\SQLEXPRESS01`, base `DB_A57E86_comercializadora` (usuario `sa`; la contraseña está en la línea comentada de `lluviaBackEnd/lluviaBackEnd/Web.config` con `localhost`). Prohibido conectarse a la BD remota o ejecutar scripts contra producción. No modificar `Web.config`.
- `sqlcmd -S "localhost\SQLEXPRESS01" -U sa -P <pwd> -C -d DB_A57E86_comercializadora`. Los scripts `.sql` existentes están en Windows-1252: preservar la codificación; los nuevos se guardan en UTF-8 con BOM o ASCII sin acentos en identificadores.
- Convención de SPs: primer result set = fila de estatus (`select 200 status, 'mensaje'` o `-1`), el segundo en adelante son datos. Se lee con `QueryMultiple` + `ReadFirst`, y los datos solo si estatus == 200 (ver `DAO/ProductosAgranelAEnvasar/ProductosAgranelAEnvasarDAO.cs`).
- Cantidades: tipo `float`, redondeadas con `dbo.redondear(...)` como el resto del inventario. Fechas con `dbo.FechaActual()`.
- Idioma: identificadores y SPs en español como el resto; textos de UI en español; comentarios cortos.
- El proyecto C# es no-SDK: los archivos nuevos requieren `<Compile Include>` en `lluviaBackEnd.csproj`. **Los agentes NO editan el .csproj**: listan sus archivos nuevos en su canal y el orquestador los registra al final.
- `Web.config`, `.csproj`, `Models/Enumeraciones.cs` (ya contiene `Puede_visualizar_Relacion_Produccion = 15`) los toca solo el orquestador.

## 1. Reparto de propiedad (cada archivo tiene un solo dueño)

| Agente | Es dueño de |
|---|---|
| SPs | `Analisis/Scripts Proceso Produccion Formula/*.sql` (carpeta nueva) y todo cambio en la BD local |
| Admin | `Controllers/ProcesoProduccionFormula/`, `Views/ProcesoProduccionFormula/`, `js/EvtProcesoProduccionFormula.js`, `Models/ProcesoProduccionFormula/`, `DAO/ProcesoProduccionFormulaDAO.cs`, `Views/Shared/_Layout.cshtml`, y los cambios de la columna `origen` en el reporte MPL Individual (modelo, vista parcial, JS) |
| Api | `WebServices/WsProcesoProduccionController.cs`, `WebServices/Modelos/Request/*ProcesoProduccion*`, `WebServices/Modelos/Response/*ProcesoProduccion*` (o la carpeta que ya use el proyecto), `DAO/ProcesoProduccionDAO.cs` |
| HH | todo `androidMobileLluvia` (repo aparte, rama `feature/ProcesoProduccion`) |

Rutas relativas del WMS: `E:\Documents\GitHub\comercializadora\lluviaBackEnd\lluviaBackEnd\`. HH: `E:\Documents\BlueCloud\Proyectos\androidMobileLluvia\`.

## 2. Tablas nuevas (estándar de `ProcesoProduccionAgranel` y `VentasDetalle`)

PK `PK_<Tabla>`, FKs `FK_<Tabla>_<Referenciada>` sin cascada, `fechaAlta` y `fechaUltimaActualizacion` datetime.

**ProcesoProduccionFormula** (la fórmula)
- `idProcesoProduccionFormula` bigint identity PK
- `idProducto` int NOT NULL (producto a producir, línea 20) FK Productos
- `cantidadLote` float NOT NULL (> 0)
- `idUnidadMedida` int NOT NULL (copiada de `Productos.idUnidadMedida` al guardar; no la captura el usuario)
- `activo` bit NOT NULL default 1
- `idUsuario` int NOT NULL FK Usuarios, `fechaAlta`, `fechaUltimaActualizacion`
- Índice único filtrado sobre `idProducto` WHERE `activo = 1` (una sola fórmula activa por producto)

**ProcesoProduccionFormulaDetalle**
- `idProcesoProduccionFormulaDetalle` bigint identity PK
- `idProcesoProduccionFormula` bigint FK
- `idProducto` int FK Productos (materia prima, línea 12)
- `cantidad` float NOT NULL (> 0, por lote)
- `idUnidadMedida` int NOT NULL (copiada del producto)
- Único (`idProcesoProduccionFormula`, `idProducto`)

**ProcesoProduccion** (cada producción realizada; no se puede revertir ni editar)
- `idProcesoProduccion` bigint identity PK
- `idProcesoProduccionFormula` bigint FK
- `idProducto` int FK (producto producido)
- `lotes` float NOT NULL (> 0) — SUPUESTO: admite decimales (la HH ya tiene selector decimal). Pendiente de confirmar con el usuario.
- `cantidadLote` float (copia al momento), `cantidadProducida` float (= lotes × cantidadLote), `idUnidadMedida` int
- `idUsuario` int FK, `idAlmacen` int FK, `fechaAlta`

**ProcesoProduccionDetalle**
- `idProcesoProduccionDetalle` bigint identity PK
- `idProcesoProduccion` bigint FK
- `idProducto` int FK (materia prima)
- `cantidadPorLote` float (copia), `cantidadConsumida` float (= cantidadPorLote × lotes), `idUnidadMedida` int

**Columna nueva** `InventarioDetalleLog.idProcesoProduccion bigint NULL` con descripción (`sp_addextendedproperty` `MS_Description`): "Id del registro de producción por fórmula (ProcesoProduccion) que originó este movimiento. NULL si no proviene de esa función. No confundir con idProcesoProduccionAgranel." Script idempotente. Verificado: sin triggers ni INSERT sin lista de columnas (local y producción).

**Tipos de movimiento nuevos** en `CatTipoMovimientoInventario` (id identity; NO hardcodear ids, los SPs los resuelven por descripción exacta):
- `Actualizacion de Inventario(carga de producto terminado por proceso de produccion de formula)` operacion = 1
- `Actualizacion de Inventario(salida de materia prima por proceso de produccion de formula)` operacion = -1

**Permiso**: `CatModulos` idModulo = 15, descripción `Relacion Produccion` (verificar si la tabla es identity); filas en `PermisosRolPorModulo` para todos los roles (`CatRoles`) con `tienePermiso = 1` solo para `idRol = 1`, 0 en los demás.

## 3. Stored procedures (nombres y firmas fijos)

### Para el WMS (agente Admin los consume)
- `SP_OBTENER_PROCESO_PRODUCCION_FORMULAS` (sin parámetros). Set 2: una fila por fórmula activa: `idProcesoProduccionFormula, idProducto, descripcionProducto, idUnidadMedida, unidadMedida, cantidadLote, fechaAlta`.
- `SP_OBTENER_PROCESO_PRODUCCION_FORMULA_DETALLE @idProcesoProduccionFormula bigint`. Set 2: `idProcesoProduccionFormulaDetalle, idProcesoProduccionFormula, idProducto, descripcionProducto, idUnidadMedida, unidadMedida, cantidad`.
- `SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_FORMULA` (sin parámetros). Set 2 = productos línea 20 activos SIN fórmula activa (más el de la fórmula en edición lo resuelve el DAO pasando `@idProcesoProduccionFormula bigint = null`); Set 3 = productos línea 12 activos (materias primas). Columnas: `idProducto, descripcion, idUnidadMedida, unidadMedida`.
- `SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA @idProcesoProduccionFormula bigint (0 = alta), @idProducto int, @cantidadLote float, @idUsuario int, @xmlMateriasPrimas xml`. El XML: `<materiasPrimas><materiaPrima idProducto="907" cantidad="10"/>...</materiasPrimas>`. Valida: lote > 0, al menos una MPL, sin MPL repetida, MPL de línea 12, producto de línea 20, sin otra fórmula activa del mismo producto. Una transacción; al editar reemplaza el detalle. Devuelve `status, mensaje`.
- `SP_DESACTIVAR_PROCESO_PRODUCCION_FORMULA @idProcesoProduccionFormula bigint, @idUsuario int` (baja lógica). Devuelve `status, mensaje`.

### Para la HH vía servicios web (agente Api los consume)
- `SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA @idUsuario int, @descripcion varchar(200) = null, @codigoBarras varchar(50) = null`. Set 2: productos línea 20 con fórmula activa: `idProducto, codigoBarras, descripcion, idUnidadMedida, unidadMedida, idProcesoProduccionFormula, cantidadLote`. Set 3: materias primas de esas fórmulas: `idProcesoProduccionFormula, idProducto, descripcion, idUnidadMedida, unidadMedida, cantidad`. Si no hay resultados, `status = -1`.
- `SP_APP_AGREGAR_PROCESO_PRODUCCION @idProducto int, @lotes float, @idUsuario int, @idAlmacen int`. Una sola transacción (modelo: `SP_APP_AGREGAR_PRODUCTO_INVENTARIO_LIQUIDOS_ENVASADO`):
  - Validaciones sin mover nada: lotes > 0; fórmula activa existe; no hay inventario físico activo; el producto existe en el almacén (`dbo.ExisteProductoEnAlmancen`); inventario suficiente de CADA MPL en el almacén (`dbo.ExisteCantidadProductoEnAlmancen`). Si falta alguna: `status = -1`, mensaje y, como Set 2, la lista de faltantes `idProducto, descripcion, requerido, disponible, unidadMedida`.
  - Calcula una sola vez (`dbo.redondear`) `cantidadProducida` y `cantidadConsumida` por MPL; inserta `ProcesoProduccion` y `ProcesoProduccionDetalle`.
  - Descuenta cada MPL de cualquier ubicación del almacén, de mayor a menor cantidad (sin resguardo 1002; sin ubicación 0-0-0 salvo que sea donde esté la existencia, como en Envasado), actualizando `InventarioDetalle`, `InventarioGeneral` y los logs `InventarioDetalleLog` (con `idProcesoProduccion`, tipo salida MPL) e `InventarioGeneralLog`.
  - Da de alta el producto terminado en la ubicación 0-0-0 (sin acomodar) del almacén (créala si no existe), con tipo carga de producto terminado e `idProcesoProduccion`.
  - Devuelve `status = 200` y `mensaje`; en Set 2 el `idProcesoProduccion`.
- `SP_APP_OBTENER_PROCESO_PRODUCCION_HISTORIAL @idUsuario int, @fechaIni datetime = null, @fechaFin datetime = null`. Set 2: `idProcesoProduccion, idProducto, descripcionProducto, lotes, cantidadProducida, unidadMedida, fechaAlta` solo del usuario; si no hay fechas, hoy. Orden por fecha descendente. `status = -1` si vacío.

### Reportes (ALTER de SPs existentes)
- `SP_CONSULTA_COSTO_PRODUCCION` y `SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO`: sumar con `UNION ALL` el consumo de `ProcesoProduccionDetalle` (unido a `ProcesoProduccion` por fecha): cantidad solicitada = aceptada = `cantidadConsumida`, estatus mostrado "Procesado correctamente" (3), costo con `dbo.obtenerPrecioCompra`. En el reporte individual agregar al final la columna `origen varchar(30)` ('Consumo MPL' | 'Produccion por formula'); para filas de fórmula `idProcesoProduccionAgranel` = NULL y se expone `idProcesoProduccion`. Antes de alterar, el agente SPs guarda resultados de referencia con fechas fijas y demuestra que, sin datos de fórmula, el resultado es idéntico al original.

## 4. Servicios web (agente Api) — controlador `WsProcesoProduccionController`, rutas `api/WsProcesoProduccion/<Action>`, todas POST

| Action | Request (`RequestXxx`) | Response |
|---|---|---|
| `BuscarProductosFormula` | `{ idUsuario, descripcion, codigoBarras }` | `Notificacion<List<ProductoFormula>>`; cada `ProductoFormula` = `idProducto, codigoBarras, descripcion, idUnidadMedida, unidadMedida, idProcesoProduccionFormula, cantidadLote, materiasPrimas: List<MateriaPrimaFormula{idProducto, descripcion, idUnidadMedida, unidadMedida, cantidad}>` |
| `AgregarProcesoProduccion` | `{ idProducto, lotes, idUsuario, idAlmacen }` | `Notificacion<List<MateriaPrimaFaltante>>` (`idProducto, descripcion, requerido, disponible, unidadMedida`); `Modelo` vacío cuando estatus 200 |
| `ObtenerHistorialProcesoProduccion` | `{ idUsuario, fechaIni, fechaFin }` | `Notificacion<List<ProcesoProduccionHistorial>>` (`idProcesoProduccion, idProducto, descripcionProducto, lotes, cantidadProducida, unidadMedida, fechaAlta`) |

Reglas: respuesta `Models.Notificacion<T>` (Estatus, Mensaje, Modelo); errores con `WsUtils<T>.RegresaExcepcion`; DAO con Dapper y `QueryMultiple`/`ReadFirst`; JSON en camelCase tal cual las propiedades C#. En el servidor validar `lotes > 0` y que `idUsuario`/`idAlmacen` sean válidos antes de llamar al SP. DAO propio `DAO/ProcesoProduccionDAO.cs`, separado del DAO del admin.

## 5. Admin (WMS) — pantalla "Relación Producción"

- Controlador `ProcesoProduccionFormulaController` (acciones análogas a `ProductosAgranelAEnvasarController`: vista principal, obtener catálogos, `_ObtenerFormulas` parcial, guardar, desactivar, obtener detalle). Vista, parcial y JS siguen el patrón `Ajax.BeginForm` + `PartialView` + DataTables (PDF/Excel con títulos "Relación Producción", no "Clientes").
- Menú: ítem "Relación Producción" en el menú Productos de `_Layout.cshtml`, junto a "Relación Líquidos", visible solo con `sesion.permisosModulo.Exists(p => p.idModulo == (int)EnumRolesPermisos.Puede_visualizar_Relacion_Produccion && p.tienePermiso)`.
- `[PermisoAttribute]` NO protege (su chequeo está comentado): cada acción debe validar el permiso 15 explícitamente contra `Session["UsuarioActual"]` y rechazar (JSON/redirect) si no lo tiene.
- Formulario: producto a producir (línea 20 sin fórmula), unidad automática (solo lectura), cantidad del lote, y tabla de materias primas (línea 12) con unidad automática, cantidad por lote, agregar y quitar; sin repetir MPL. Sin modal duplicado ni campos ligados al mismo id (defecto del original).
- Reporte MPL Individual: mostrar la columna `origen` nueva (modelo `CostoProduccionAgranelPorProducto`, parcial y JS `EvtMPLIndividual.js`), sin romper la exportación.

## 6. HH (Android) — repo `androidMobileLluvia`

- Sigue `CLAUDE.md` del repo (Java puro, Activities, Retrofit `Call<T>`.enqueue, SharedPreferences). Sin dependencias nuevas.
- Nueva `ProduccionFormulaActivity` + `activity_produccion_formula.xml` copiando `LiquidosActivity`/`activity_liquidos.xml`: pestañas Agregar e Historial; búsqueda por nombre (mín. 3 caracteres) o código; muestra unidad y cantidad del lote; selector de lotes decimal > 0; muestra "Cantidad a producir = lotes × lote" y la lista de MPL a descontar (cantidad × lotes); botón "GUARDAR PRODUCCIÓN" con confirmación que avisa que es definitivo; bloqueo con inventario físico activo (`validarInventarioFisico`); si el servidor devuelve faltantes, mostrarlos.
- Título y menú del nuevo ítem: **"PRODUCCIÓN"**. Visible para roles 13 y 1. Ícono: lo agrega el usuario (poner un drawable/placeholder temporal y avisarlo).
- Renombrar la pantalla actual `action_liquidos`/`titulo_liquidos` a **"PROD LIQUIDOS"** (etiqueta de menú y título de pantalla).
- Agregar el ítem en `res/layout/menu.xml`, `strings.xml`, el manifest y el `Menu()` + `switch` de TODAS las Activities que tienen drawer (~12).
- En `API.java` agregar los 3 `@POST("api/WsProcesoProduccion/...")` con clases `Request*`/`Notificacion*` en `Entidades/` con los mismos nombres de campo que la sección 4.
- NO cambiar `BASE_URL` en el repo (la prueba contra el local la hace el usuario). Existen cambios previos sin commitear en `app/build.gradle`, `LiquidosActivity.java` y `LoginActivity.java`: respetarlos, no revertirlos ni mezclarlos.
- Git: usar `git -c safe.directory=E:/Documents/BlueCloud/Proyectos/androidMobileLluvia ...` solo para lectura.

## 7. Dependencias entre agentes (quién espera a quién)

1. SPs publica primero (en su canal) las tablas/SPs ya aplicados en la BD local. Api y Admin escriben su código contra este contrato y, al terminar, verifican con la BD local solo si el canal del agente SPs dice `APLICADO EN LOCAL`; si no, lo dejan como pendiente de integración.
2. Admin necesita de SPs: los 4 SPs de la sección 3 (WMS) y el cambio de `SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO` (columna `origen`).
3. Api necesita de SPs: los 3 SPs de la sección 3 (HH).
4. HH necesita de Api: rutas, request y response de la sección 4. Cualquier cambio de forma lo anuncia Api en su canal ANTES de hacerlo.
5. Si un agente necesita cambiar algo del contrato, lo escribe en su canal bajo `PROPUESTA DE CAMBIO` y lo reporta; el orquestador decide y lo replica a los demás.

## 8. Entregable de cada agente (reporte final)

Archivos creados y modificados (rutas completas), qué verificó y cómo, qué NO pudo verificar, supuestos tomados, y dependencias pendientes. No inventar resultados: si algo no se ejecutó, decirlo.

## 9. Canales

Carpeta `Analisis/Requerimientos Relacion Produccion/canales/`. Cada agente escribe SOLO en su archivo (`sps.md`, `admin.md`, `api.md`, `hh.md`) y puede leer los demás. Formato: líneas con prefijo `ESTADO:`, `APLICADO EN LOCAL:`, `PROPUESTA DE CAMBIO:`, `BLOQUEO:`, `ARCHIVOS NUEVOS (para el .csproj):`.
