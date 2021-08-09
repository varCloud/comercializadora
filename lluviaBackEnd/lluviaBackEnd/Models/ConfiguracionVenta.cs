using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class ConfiguracionVenta
    {
        public int idConfig { get; set; }
        public string descripcion { get; set; }
        public int valor { get; set; }
        public Boolean activo { get; set; }
    }
}