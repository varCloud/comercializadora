# Canal hh

ESTADO: HH terminada (seccion 6). `./gradlew.bat assembleDebug` compila OK (solo warnings de source/target 8). No probado en dispositivo ni contra servidor.
ESTADO: Menu "PRODUCCION" (action_produccion_formula) agregado en menu.xml y en Menu()+switch de las 12 Activities con drawer (verificado con grep: las 12 que tenian action_liquidos_granel tienen ahora tambien el item) mas la nueva ProduccionFormulaActivity. Visible solo roles 13 y 1.
ESTADO: titulo_liquidos renombrado a "PROD LIQUIDOS" (etiqueta de menu y titulo de pantalla).
ESTADO: ICONO TEMPORAL: res/drawable/ic_menu_produccion_formula.xml (matraz gris). El usuario debe reemplazarlo por el definitivo (mismo nombre o cambiar android:icon en menu.xml).
ESTADO: Request del historial: fechaIni "yyyy-MM-dd" y fechaFin "yyyy-MM-dd 23:59:59" (para incluir el dia final). Nota para Api/SPs: el SP debe aceptar ese datetime.
ESTADO: Respuesta de AgregarProcesoProduccion: la HH ignora `id`; estatus 200 = exito; estatus != 200 con Modelo no vacio = dialogo de faltantes; si no, Toast con Mensaje.
ARCHIVOS NUEVOS (androidMobileLluvia/app/src/main/):
  java/bluecloud/com/mx/comercializadoralluvia/ProduccionFormulaActivity.java
  java/.../Entidades/ProductoFormula.java, MateriaPrimaFormula.java, MateriaPrimaFaltante.java, ProcesoProduccionHistorial.java
  java/.../Entidades/RequestBuscarProductosFormula.java, RequestAgregarProcesoProduccion.java, RequestObtenerHistorialProcesoProduccion.java
  java/.../Entidades/NotificacionBuscarProductosFormula.java, NotificacionAgregarProcesoProduccion.java, NotificacionObtenerHistorialProcesoProduccion.java
  res/layout/activity_produccion_formula.xml
  res/drawable/ic_menu_produccion_formula.xml
ARCHIVOS MODIFICADOS: AndroidManifest.xml, res/layout/menu.xml, res/values/strings.xml, ServiciosWeb/API.java (3 @POST), y Menu()/switch de: Bitacora, Compras, Informe, Inventario, Liquidos, LiquidosGranel, Main, Mayoreo, SinAcomodar, Traperos, TraperosProduccion, Verificador (Activity.java).
