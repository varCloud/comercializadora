# Canal api

ESTADO: Implementados los 3 servicios de la seccion 4 con rutas, campos y JSON exactos del contrato (POST api/WsProcesoProduccion/{BuscarProductosFormula|AgregarProcesoProduccion|ObtenerHistorialProcesoProduccion}). Compilado con csc (Roslyn de VS) contra bin/ sin errores. Sin prueba contra BD: canales\sps.md aun no dice "APLICADO EN LOCAL".
ESTADO: Notas de forma para la HH (no cambian el contrato): cantidades/lotes viajan como numero JSON (double en C#); fechaIni/fechaFin son opcionales (null = hoy en el SP); en errores/vacio `Modelo` es lista vacia `[]` (no null) salvo excepcion no controlada (Estatus -1, Modelo null). En AgregarProcesoProduccion con estatus 200, `Modelo` = [] y el idProcesoProduccion generado va en `id` de la Notificacion. Con estatus != 200 por faltantes, `Modelo` trae la lista de faltantes.
ESTADO: Validacion en servidor antes del SP: idUsuario > 0, idAlmacen > 0, idProducto > 0, lotes > 0 (Estatus -1 y mensaje).
ESTADO: El DAO lee la fila de estatus sin importar si el SP la nombra status/estatus ni mayusculas; si no encuentra el nombre, toma columnas 1 y 2. Dependencia: el set 3 de SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA debe traer `idProcesoProduccionFormula` (contrato) para anidar las MPL.
ARCHIVOS NUEVOS (para el .csproj):
  <Compile Include="DAO\ProcesoProduccionDAO.cs" />
  <Compile Include="WebServices\WsProcesoProduccionController.cs" />
  <Compile Include="WebServices\Modelos\Request\RequestAgregarProcesoProduccion.cs" />
  <Compile Include="WebServices\Modelos\Request\RequestBuscarProductosFormulaProcesoProduccion.cs" />
  <Compile Include="WebServices\Modelos\Request\RequestObtenerHistorialProcesoProduccion.cs" />
  <Compile Include="WebServices\Modelos\Response\ResponseProcesoProduccion.cs" />
