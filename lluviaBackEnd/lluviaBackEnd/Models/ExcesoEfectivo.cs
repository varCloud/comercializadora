using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class ExcesoEfectivo
    {
        public int idUsuario { get; set; }
        public string Nombre { get; set; }
        public float Ingresos { get; set; }
        public float Retiros { get; set; }
        public float EfectivoDisponible { get; set; }

    }
}