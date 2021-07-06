using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class TipoCliente
    {
        public int idTipoCliente { get; set; }
        public string descripcion { get; set; }
        public float descuento { get; set; }
        public Boolean activo { get; set; }
        public int contador { get; set; }
    }
}