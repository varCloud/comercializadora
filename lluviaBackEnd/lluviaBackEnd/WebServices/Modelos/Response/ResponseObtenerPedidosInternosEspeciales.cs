using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Response
{
    public class ResponseObtenerPedidosInternosEspeciales
    {
        public string descripcionPedido { get; set; }
        public int idEstatusPedidoInterno { get; set; }
        public string descripcionEstatus { get; set; }
        public int idPedidoInterno { get; set; }
        public DateTime fechaAlta { get; set; }
        public Almacen almacenOrigen { get; set; }
        public Almacen almacenDestino { get; set; }

        public string usuarioSolicito { get; set; }
        public int idUsuarioSolicito { get; set; }
    }
}