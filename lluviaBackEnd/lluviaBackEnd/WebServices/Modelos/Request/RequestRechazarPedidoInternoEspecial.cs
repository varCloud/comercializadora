using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestRechazarPedidoInternoEspecial
    {

        public int idPedidoEspecial { get; set; }
        public int idUsuario { get; set; }
        public int idAlmacen { get; set; }
        public string observaciones { get; set; }
    }
}