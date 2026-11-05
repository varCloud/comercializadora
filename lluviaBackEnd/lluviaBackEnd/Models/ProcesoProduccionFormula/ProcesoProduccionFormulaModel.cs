using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lluviaBackEnd.Models
{
    // Fila del listado de formulas (SP_OBTENER_PROCESO_PRODUCCION_FORMULAS)
    public class ProcesoProduccionFormulaModel
    {
        public long idProcesoProduccionFormula { get; set; }
        public int idProducto { get; set; }
        public string descripcionProducto { get; set; }
        public int idUnidadMedida { get; set; }
        public string unidadMedida { get; set; }
        public double cantidadLote { get; set; }
        public DateTime fechaAlta { get; set; }
    }

    // Materia prima de una formula (SP_OBTENER_PROCESO_PRODUCCION_FORMULA_DETALLE)
    public class ProcesoProduccionFormulaDetalleModel
    {
        public long idProcesoProduccionFormulaDetalle { get; set; }
        public long idProcesoProduccionFormula { get; set; }
        public int idProducto { get; set; }
        public string descripcionProducto { get; set; }
        public int idUnidadMedida { get; set; }
        public string unidadMedida { get; set; }
        public double cantidad { get; set; }
    }

    // Opcion de producto para los catalogos del formulario
    public class ProcesoProduccionFormulaProductoModel
    {
        public int idProducto { get; set; }
        public string descripcion { get; set; }
        public int idUnidadMedida { get; set; }
        public string unidadMedida { get; set; }
    }

    // Catalogos del formulario: productos a producir (linea 20) y materias primas (linea 12)
    public class ProcesoProduccionFormulaCatalogosModel
    {
        public List<ProcesoProduccionFormulaProductoModel> productos { get; set; }
        public List<ProcesoProduccionFormulaProductoModel> materiasPrimas { get; set; }
        // Solo al editar o ver una formula existente
        public List<ProcesoProduccionFormulaDetalleModel> detalle { get; set; }
    }

    // Materia prima enviada por el formulario (hidden inputs materiasPrimas[i].*)
    public class ProcesoProduccionFormulaMateriaPrimaRequest
    {
        public int idProducto { get; set; }
        // Texto: se interpreta con cultura invariante (punto decimal)
        public string cantidad { get; set; }
    }

    // Modelo del formulario (Ajax.BeginForm)
    public class ProcesoProduccionFormulaGuardarModel
    {
        public long idProcesoProduccionFormula { get; set; }

        [Required(ErrorMessage = "Selecciona el producto a producir")]
        public string idProducto { get; set; }

        [Required(ErrorMessage = "Captura la cantidad del lote")]
        public string cantidadLote { get; set; }

        public List<ProcesoProduccionFormulaMateriaPrimaRequest> materiasPrimas { get; set; }
    }
}
