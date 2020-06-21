using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class DiasPromedioInventario
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public EnumTipoMargenBruto tipoMargenBruto { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public float CostoProducto { get; set; }
        public float InventarioPromedioPeriodo { get; set; }
        public float CostoInvPromedio { get; set; }
        public int diasPeriodo { get; set; }
        public float CostoVendido { get; set; }
        public float diasPromedioInventario { get; set; }        
        public float rotacionInventario { get; set; }
        public float TotalVentas { get; set; }

    }
}