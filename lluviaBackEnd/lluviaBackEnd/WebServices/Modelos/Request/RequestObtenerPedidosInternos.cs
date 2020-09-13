using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerPedidosInternos
    {
        public int idPedidoInterno { get; set; }
        public int idEstatusPedido { get; set; }
        public int idAlmacenOrigen { get; set; }
        public int idAlmacenDestino { get; set; }
        public int idUsuario { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }

        public int idTipoPedidoInterno { get; set; }

    }
}