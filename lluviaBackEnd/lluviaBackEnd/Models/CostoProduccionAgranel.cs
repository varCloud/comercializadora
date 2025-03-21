﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class CostoProduccionAgranel
    {
        public Int64 idReporteCostoProduccion { get; set; }
        public Int64 idProducto { get; set; }
        public float cantidadSolicitadaMesAnt { get; set; }
        public float cantidadAceptadaFinalMesAnt { get; set; }

        public float porcCostoProduccion { get; set; }
        public float ultCostoCompra { get; set; }
        public float costoProduccionMerma { get; set; }
        public DateTime ultimoDiaMesCalculo { get; set; }
        public DateTime ultimoDiaMesAnterior { get; set; }
        public DateTime fechaAlta { get; set; }
        public string codigoBarras { get; set; }
        public string descripcionProducto { get; set; }
        public int idLineaProducto { get; set; }
        public string descripcionLinea { get; set; }

        public int AnioCalculo { get; set; }
        public int MesCalculo { get; set; }
        public int idAlmacen { get; set; }

        public string descripcionEstatus { get; set; }

        public float cantidad { get; set; }
        public float cantidadAceptada { get; set; }
        public float cantidadRestante { get; set; }
        public string nombreUsuario { get; set; }
    }

    public class FiltroCostoProduccionAgranel
    {

        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public int idUsuario { get; set; }
        public int idEstatusProcesoProduccionAgranel { get; set; }
    }
}