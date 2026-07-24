using System;

namespace lluviaBackEnd.Models
{
    public class CostoProduccionAgranelPorProducto
    {
        public Int64 idProcesoProduccionAgranel { get; set; }
        public int idProducto { get; set; }
        public string codigoBarras { get; set; }
        public string descripcionProducto { get; set; }
        public float cantidad { get; set; }
        public float cantidadAceptada { get; set; }
        public float cantidadRestante { get; set; }
        public DateTime fechaAlta { get; set; }
        public DateTime fechaUltimaActualizacion { get; set; }
        public int idEstatusProduccionAgranel { get; set; }
        public string descripcionEstatus { get; set; }
        public float ultimoCostoCompra { get; set; }
        public float costoProduccion { get; set; }
    }
}
