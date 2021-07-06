using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerProductos
    {
        public int idProducto { get; set; }
        public string descripcion { get; set; }
        public int idLineaProducto { get; set; }
        public string articulo { get; set; }
    }
}