using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class DetallePedidoInterno
    {
        public int idPedidoInternoDetalle { get; set; }
        public int idProducto { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public int cantidadAtendida { get; set; }

        public string observacion { get; set; }
        public string observacionRechazaSolicita { get; set; }
        public string observacionAtendio { get; set; }
        public string observacionRechazaAtendio { get; set; }
        public string observacionFinalizado { get; set; }
        public string usuarioAtendio { get; set; }
        public string usuarioSolicito { get; set; }

        public string usuarioRechaza { get; set; }

        public string usuarioAutoriza { get; set; }

        public string fechaAlta { get; set; }
        public string fechaAtendido { get; set; }

        public string fechaRechazado { get; set; }
        public string fechaAutoriza { get; set; }
        public string fechaRechazaSolicita { get; set; }

    }
}