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

        /// <summary>
        ///este paramatro es necesario cuando se atendio un pedido para darle salida al producto del alamcen que lo atendio
        ///por tal razon solo es necesario cuando el @idEstatusPedidoInterno = 2
        /// </summary>
        public int idUbicacion { get; set; }

        public string observacion { get; set; }
    }
}