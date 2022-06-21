using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerProductosXLineaProducto
    {
        public int idLineaProducto { get; set; }
        public int idUsuario { get; set; }
        public string descripcion { get; set; }
    }
}