using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Response
{
    public class ResponseObtenerPedidosInternos
    {
        public int idEstatusPedidoInterno { get; set; }
        public string descripcionEstatus { get; set; }
        public int idPedidoInterno { get; set; }
        public int cantidadAtendida { get; set; }
        public DateTime fechaAlta { get; set; }
        public DateTime fechaAtendido { get; set; }
        public string observacion { get; set; }

        public string usuarioSolicito{ get; set; }
        public string usuarioAtendio { get; set; }
        public Almacen almacenOrigen { get; set; }
        public Almacen almacenDestino { get; set; }
        public Producto producto { get; set; }

    }
}