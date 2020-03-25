using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestAgreagrProductoInventario
    {
        public int idProveedor { get; set; }
        public int idProducto { get; set; }
        public int idUsuario { get; set; }

        public int cantidad { get; set; }
    }
}