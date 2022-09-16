using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Response
{
    public class ResponseObtenerProductosProduccionAgranel
    {
        public Int64 idProcesoProduccionAgranel { get; set; }
        public int idEstatusProduccionAgranel { get; set; }
        public DateTime fechaAlta { get; set; }
        public float cantidad { get; set; }
        public Producto producto { get; set; }
    }
}
