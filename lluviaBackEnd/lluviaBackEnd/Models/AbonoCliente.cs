using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class AbonoCliente
    {
        public int idCliente { get; set; }
        public int idUsuario { get; set; }
        public float montoAbono { get; set; }
        public float montoIVA { get; set; }
        public float montoComision { get; set; }
        public Boolean requiereFactura { get; set; }
        public int idFactFormaPago { get; set; }
        public int idFactUsoCFDI { get; set; }
        public Int64 idPedidoEspecial { get; set; }

    }
}