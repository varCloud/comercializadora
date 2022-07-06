using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestAgregarProductoInventarioLiquidos 
    {
        public int idProducto { get; set; }
        public int idUsuario { get; set; }
        public float cantidad { get; set; }
        public int idAlmacen { get; set; }
    }
}