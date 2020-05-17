using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestActualizarEstatusPedidoInterno
    {
        public int idPedidoInterno { get; set; }
        public int idUsuario { get; set; }
        /// <summary>
        /// 1	Pedido Realizado
        ///2	Pedido Enviado ó Atendido
        ///3	Pedido Rechazado
        ///4	Pedido Finalizado
        /// </summary>
        public int idEstatusPedidoInterno { get; set; }
        public int idAlmacenOrigen { get; set; }
        public int idAlmacenDestino { get; set; }
    }
}