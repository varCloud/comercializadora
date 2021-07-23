using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class MargenBruto
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public float totalVentas { get; set; }
        public float CostoVentas { get; set; }
        public float ContribucionMarginal { get; set; }
        public EnumTipoMargenBruto tipoMargenBruto { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public float margenBruto { get; set; }
        public string codigoBarras { get; set; }

    }
}