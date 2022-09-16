using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerProductosProduccionAgranel
    {
        public int idAlmacen { get; set; }
        public int idUsuario { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }

        public int idEstatusProduccionAgranel { get; set; }
    }
}