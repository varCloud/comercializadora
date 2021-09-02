using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class PedidosEspecialesV2
    {
        public int idPedidoEspecial { get; set; }
        public int idCliente { get; set; }
        public int idUsuario { get; set; }
    }
}