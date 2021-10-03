using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerNotificacionesPedidosEspeciales
    {
        public int idEstatusPedidoEspecialDetalle { get; set; }
        public int idAlmacenOrigen { get; set; }
        public int idAlmacenDestino { get; set; }
    }
}