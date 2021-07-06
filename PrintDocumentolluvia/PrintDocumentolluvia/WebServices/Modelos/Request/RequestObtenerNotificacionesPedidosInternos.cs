using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerNotificacionesPedidosInternos
    {
        public int idEstatusPedidoInterno { get; set; }
        public int idTipoPedidoInterno { get; set; }

        public int idAlmacenOrigen { get; set; }
        public int idAlmacenDestino { get; set; }
    }
}