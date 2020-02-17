using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lluviaBackEndEntidades
{
    public class Usuario
    {

        public int idUsuario { get; set; }
        public int idRol { get; set; }
        public string nombre  { get; set; }
        public string apellidoPaterno  { get; set; }
        public string apellidoMaterno  { get; set; }
        public string telefono { get; set; }
        public string contrasena { get; set; }
        public int idAlmacen { get; set; }
        public int idSucursal { get; set; }

    }
}
