using System;
using System.Collections.Generic;

namespace lluviaBackEnd.Models
{
    // Filtro de la pantalla "Producción" (historial). Las fechas llegan como texto yyyy-MM-dd y se validan en el DAO.
    public class ProcesoProduccionConsultaFiltroModel
    {
        public string fechaIni { get; set; }
        public string fechaFin { get; set; }
        public int? idProducto { get; set; }
        public int? idAlmacen { get; set; }
    }

    // Fila de produccion (set 2 de SP_OBTENER_PROCESO_PRODUCCION_FORMULA_HISTORIAL)
    public class ProcesoProduccionConsultaModel
    {
        public long idProcesoProduccion { get; set; }
        public DateTime fechaAlta { get; set; }
        public int idProducto { get; set; }
        public string codigoBarras { get; set; }
        public string descripcionProducto { get; set; }
        public double lotes { get; set; }
        public double cantidadLote { get; set; }
        public double cantidadProducida { get; set; }
        public string unidadMedida { get; set; }
        public int idUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public int idAlmacen { get; set; }
        public string descripcionAlmacen { get; set; }
    }

    // Materia prima consumida (set 3 del mismo SP)
    public class ProcesoProduccionConsultaDetalleModel
    {
        public long idProcesoProduccion { get; set; }
        public int idProducto { get; set; }
        public string descripcionProducto { get; set; }
        public double cantidadPorLote { get; set; }
        public double cantidadConsumida { get; set; }
        public string unidadMedida { get; set; }
    }

    public class ProcesoProduccionConsultaResultadoModel
    {
        public List<ProcesoProduccionConsultaModel> producciones { get; set; }
        public List<ProcesoProduccionConsultaDetalleModel> detalle { get; set; }
    }

    // Opcion del filtro de producto (SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_HISTORIAL)
    public class ProcesoProduccionConsultaProductoModel
    {
        public int idProducto { get; set; }
        public string descripcion { get; set; }
    }
}
