using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class PedidosInternosDetalle
    {

        public int idPedidoInternoDetalle { get; set; }
        public int idPedidoInterno { get; set; }
        public int contador { get; set; }
        public int idProducto { get; set; }
        public string descProducto { get; set; }
        public int cantidad { get; set; }
        public DateTime fechaAlta { get; set; }
        public int cantidadAtendida { get; set; }
        public int cantidadAceptada { get; set; }
        public int cantidadRechazada { get; set; }
        public string observacion { get; set; }

    }
}