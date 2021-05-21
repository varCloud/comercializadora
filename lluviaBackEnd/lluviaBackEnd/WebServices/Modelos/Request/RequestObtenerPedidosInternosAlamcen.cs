using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerPedidosInternosAlamcen
    {
        public int idAlmacenDestino { get; set; }
        public int idAlmacenOrigen { get; set; }
        public int idEstatusPedido { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int idPedidoInterno { get; set; }
        public int idTipoPedidoInterno { get; set; }


    }
}