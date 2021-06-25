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
        public string descripcionPedidoEspecial { get; set; }
        public int idPedidoInterno { get; set; }
        public float cantidadAtendida { get; set; }
        public DateTime fechaAlta { get; set; }
        public DateTime fechaAtendido { get; set; }

        public DateTime fechaRechazado { get; set; }

        public DateTime fechaAutoriza { get; set; }

        public DateTime fechaRechazaSolicita { get; set; }

        
        public string observacion { get; set; }

        public string observacionRechazaSolicita { get; set; }
        public string observacionAtendio { get; set; }
        public string observacionRechazaAtendio { get; set; }
        public string observacionFinalizado { get; set; }

        public string usuarioSolicito { get; set; }
        public string usuarioAtendio { get; set; }
        public string usuarioRechaza { get; set; }
        public string usuarioAutoriza { get; set; }
        public Almacen almacenOrigen { get; set; }
        public Almacen almacenDestino { get; set; }
        public Producto producto { get; set; }

    }
}