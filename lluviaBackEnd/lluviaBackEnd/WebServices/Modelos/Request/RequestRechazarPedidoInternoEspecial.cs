using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestRechazarPedidoInternoEspecial
    {

        public int idPedidoInterno { get; set; }
        public int idUsuario { get; set; }
        public string observacionGeneral { get; set; }
    }
}