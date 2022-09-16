using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class CostoProduccionAgranelMensual
    {
       
            public string descripcion { get; set; }
            public DateTime UltimoDiaMesActual { get; set; }

            public DateTime UltimoDiaMesCalculo { get; set; }

            public float totalCantidadAceptada { get; set; }

            public float totalPorcCostoProduccion { get; set; }

            public float totalCostoProduccion { get; set; }

            public float promedioCantidadAceptada { get; set; }
            public float promedioPorcCostoProduccion { get; set; }
            public float promedioCostoProduccion { get; set; }
        
    }
}