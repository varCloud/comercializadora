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
        public float costoInventarioPromedio { get; set; }
        public int diasPeriodo { get; set; }
        public float costoVentas { get; set; }
        public float TotalVentas { get; set; }
        public float diasPromedioInventario { get; set; }





    }
}