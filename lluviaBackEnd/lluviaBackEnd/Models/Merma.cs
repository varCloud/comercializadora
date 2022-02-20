using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Merma
    {        
        public Int64 idReporteMerma { get; set; }
        public Int64 idProducto { get; set; }
        public float inventarioFinalMesAnt { get; set; }
        public float totalCompras { get; set; }
        public float inventarioSistema { get; set; }
        public float merma { get; set; }
        public float porcMerma { get; set; }
        public float ultCostoCompra { get; set; }
        public float costoMerma { get; set; }
        public DateTime UltimoDiaMesCalculo { get; set; }
        public DateTime UltimoDiaMesAnterior { get; set; }
        public DateTime fechaAlta { get; set; }
        public string codigoBarras { get; set; }
        public string descripcionProducto { get; set; }
        public int idLineaProducto { get; set; }
        public string descripcionLinea { get; set; }

        public int AnioCalculo { get; set; }
        public int MesCalculo { get; set; }
        public int idAlmacen { get; set; }


    }
}