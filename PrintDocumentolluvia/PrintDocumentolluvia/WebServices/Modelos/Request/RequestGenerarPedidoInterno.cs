using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestGenerarPedidoInterno
    {
        public int idProducto { get; set; }
        public int idUsuario { get; set; }
        public int idAlmacenOrigen { get; set; }
        public int idAlmacenDestino { get; set; }
        public float cantidad { get; set; }
    }
}