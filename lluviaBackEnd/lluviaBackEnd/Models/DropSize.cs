using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class DropSize
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public EnumTipoMargenBruto tipoMargenBruto { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public int TotalClientes { get; set; }
        public float TotalVentas { get; set; }
        public int TotalProductos { get; set; }
        public float dropSizeVentas { get; set; }
        public float dropSizeCantidad { get; set; }


    }
}