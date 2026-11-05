using System;
using System.Collections.Generic;

namespace lluviaBackEnd.WebServices.Modelos.Response
{
    public class MateriaPrimaFormula
    {
        public int idProducto { get; set; }
        public string descripcion { get; set; }
        public int idUnidadMedida { get; set; }
        public string unidadMedida { get; set; }
        public double cantidad { get; set; }
    }

    public class ProductoFormula
    {
        public int idProducto { get; set; }
        public string codigoBarras { get; set; }
        public string descripcion { get; set; }
        public int idUnidadMedida { get; set; }
        public string unidadMedida { get; set; }
        public long idProcesoProduccionFormula { get; set; }
        public double cantidadLote { get; set; }
        public List<MateriaPrimaFormula> materiasPrimas { get; set; }
    }

    public class MateriaPrimaFaltante
    {
        public int idProducto { get; set; }
        public string descripcion { get; set; }
        public double requerido { get; set; }
        public double disponible { get; set; }
        public string unidadMedida { get; set; }
    }

    public class ProcesoProduccionHistorial
    {
        public long idProcesoProduccion { get; set; }
        public int idProducto { get; set; }
        public string descripcionProducto { get; set; }
        public double lotes { get; set; }
        public double cantidadProducida { get; set; }
        public string unidadMedida { get; set; }
        public DateTime fechaAlta { get; set; }
    }
}
