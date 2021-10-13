using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerPedidosEspecialesXAlmacen
    {
        public int idAlmacen { get; set; }
        public int idEstatusPedidoEspecialDetalle { get; set; }
    }
}