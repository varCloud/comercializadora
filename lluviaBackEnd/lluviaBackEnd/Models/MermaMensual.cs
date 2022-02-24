using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class MermaMensual
    {
        public string descripcion { get; set; }
        public DateTime UltimoDiaMesActual { get; set; }

        public DateTime UltimoDiaMesCalculo { get; set; }

        public float totalMerma { get; set; }

        public float totalPorcMerma { get; set; }

        public float totalCostoMerma { get; set; }

        public float promedioMerma { get; set; }
        public float promedioPorcMerma { get; set; }
        public float promedioCostoMerma { get; set; }
    }
}