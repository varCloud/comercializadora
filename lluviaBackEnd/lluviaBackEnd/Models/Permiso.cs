using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Permiso
    {
        public int idPermiso { get; set; }
        public string modulo { get; set; }
        public string descripcion { get; set; }
        public int idModulo { get; set; }
        public Boolean tienePermiso { get; set; }
    }
}