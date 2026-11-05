# Canal sps

ESTADO: TERMINADO. Scripts en `Analisis/Scripts Proceso Produccion Formula/` (01..07 + 07a rollback). Pruebas funcionales hechas en BD local con datos de prueba revertidos (tablas de formula/produccion vacias e identities en 0).
APLICADO EN LOCAL: SP_CONSULTA_COSTO_PRODUCCION y SP_CONSULTA_COSTO_PRODUCCION_POR_PRODUCTO con UNION ALL de formula (07). Individual: columnas nuevas AL FINAL `origen` ('Consumo MPL' | 'Produccion por formula') e `idProcesoProduccion` (bigint, NULL en filas de granel); en filas de formula idProcesoProduccionAgranel = NULL.
NOTA: en los scripts con indices filtrados/SPs usar `sqlcmd -I -f 65001` (sqlcmd trae QUOTED_IDENTIFIER OFF y los SPs que escriben en ProcesoProduccionFormula fallarian). Los SPs ya se crean con SET QUOTED_IDENTIFIER ON.
CORRECCION: el set 2 de SP_APP_AGREGAR_PROCESO_PRODUCCION con status -1 sigue existiendo siempre (aunque vacio).

APLICADO EN LOCAL: tablas ProcesoProduccionFormula, ProcesoProduccionFormulaDetalle, ProcesoProduccion, ProcesoProduccionDetalle (01)
APLICADO EN LOCAL: columna InventarioDetalleLog.idProcesoProduccion bigint NULL + MS_Description (02)
APLICADO EN LOCAL: tipos de movimiento 37 (carga producto terminado, +1) y 38 (salida MPL, -1) en CatTipoMovimientoInventario (03; los SPs los resuelven por descripcion)
APLICADO EN LOCAL: CatModulos 15 'Relacion Produccion' + PermisosRolPorModulo (14 filas, tienePermiso=1 solo idRol 1) (04)
APLICADO EN LOCAL: SP_OBTENER_PROCESO_PRODUCCION_FORMULAS, SP_OBTENER_PROCESO_PRODUCCION_FORMULA_DETALLE, SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_FORMULA, SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA, SP_DESACTIVAR_PROCESO_PRODUCCION_FORMULA (05)
APLICADO EN LOCAL: SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA, SP_APP_AGREGAR_PROCESO_PRODUCCION, SP_APP_OBTENER_PROCESO_PRODUCCION_HISTORIAL (06)

NOTAS DE FORMA (leer antes de integrar):
- La fila de estatus de TODOS los SPs trae tres columnas: `status`, `estatus` (mismo valor) y `mensaje`. Asi sirve tanto `r1.status` como `r1.estatus` (dynamic de Dapper distingue mayusculas; los DAOs existentes usan `estatus`/`mensaje` en minuscula; si algun DAO usa `Estatus`/`Mensaje` con mayuscula, usar el tipo `Notificacion` o leer en minuscula).
- SP_APP_BUSCAR_...: sin resultados -> status -1 y NO hay set 2/3. Con resultados: set 2 productos, set 3 MPL con `idProcesoProduccionFormula` (para agrupar).
- SP_APP_AGREGAR_PROCESO_PRODUCCION: status 200 -> set 2 = `idProcesoProduccion`. status -1 -> set 2 SIEMPRE existe, con columnas `idProducto, descripcion, requerido, disponible, unidadMedida` (vacio si el rechazo no es por faltantes: validaciones, inventario fisico activo, error inesperado).
- SP_APP_OBTENER_PROCESO_PRODUCCION_HISTORIAL: sin filas -> status -1 sin set 2.
- SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA: status 200 -> set 2 = `idProcesoProduccionFormula`.
- Cantidades redondeadas a 2 decimales (dbo.redondear). Si cantidad MPL x lotes redondea a 0 (< 0.01, p.ej. 0.005 con 1 lote) el SP rechaza pidiendo mas lotes.
- Inventario fisico activo = InventarioFisico.idEstatusInventarioFisico = 2 en la sucursal del usuario (misma regla que FnValidaInventario).
- Descuento MPL: ubicaciones del almacen con piso < 1000 (excluye resguardo 1002, incluye 0-0-0 porque ExisteCantidadProductoEnAlmancen la cuenta).
