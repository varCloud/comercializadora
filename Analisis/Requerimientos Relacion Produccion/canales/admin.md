# Canal admin


ESTADO: Codigo de pantalla Relacion Produccion completo (controlador, DAO, modelos, vistas, JS, menu) y columna `origen` en MPL Individual. Compila (csc con stub de Sesion; las vistas Razor no se pudieron compilar sin Visual Studio, revision estatica). Integracion contra BD pendiente: canales/sps.md aun no dice APLICADO EN LOCAL.
ESTADO: Supuesto de lectura del SP: primer result set con columnas `status` (o `estatus`) y `mensaje`; el DAO acepta ambos nombres.
ESTADO: `CostoProduccionAgranelPorProducto.idProcesoProduccionAgranel` pasa a Int64? (NULL en filas de formula) y se agrega `idProcesoProduccion` Int64? y `origen` string. El SP de MPL individual debe devolver esas columnas (origen al final).
ESTADO: SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_FORMULA se llama con @idProcesoProduccionFormula (NULL en alta; id al editar/ver). Set 2 productos linea 20, Set 3 MPL linea 12, columnas idProducto, descripcion, idUnidadMedida, unidadMedida.
ESTADO: SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA recibe el XML como DbType.Xml: <materiasPrimas><materiaPrima idProducto="907" cantidad="10"/></materiasPrimas> (cantidad en cultura invariante).
ARCHIVOS NUEVOS (para el .csproj):
<Compile Include="Controllers\ProcesoProduccionFormula\ProcesoProduccionFormulaController.cs" />
<Compile Include="DAO\ProcesoProduccionFormulaDAO.cs" />
<Compile Include="Models\ProcesoProduccionFormula\ProcesoProduccionFormulaModel.cs" />
<Content Include="js\EvtProcesoProduccionFormula.js" />
<Content Include="Views\ProcesoProduccionFormula\ProcesoProduccionFormula.cshtml" />
<Content Include="Views\ProcesoProduccionFormula\_ObtenerFormulas.cshtml" />
