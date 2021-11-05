using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Filtro
    {
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public Int64 idCliente { get; set; }
        public Int64 idUsuario { get; set; }
        public int idEstatusPedidoEspecial { get; set; }
        public string codigoBarras { get; set; }

    }
}