using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerProductosAjustados
    {
        public int idAlmacen { get; set; }
        public int idProducto { get; set; }
        public int idInventarioFisico { get; set; }
    }
}