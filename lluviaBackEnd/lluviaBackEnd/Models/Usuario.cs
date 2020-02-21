using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Usuario
    {
        public Boolean usuarioValido { get; set; }
        public int idUsuario { get; set; }
        public string usuario { get; set; }
        public string contrasena { get; set; }
        public int idRol { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string telefono { get; set; }
        public int idAlmacen { get; set; }
        public int idSucursal { get; set; }
        public string Sucursal { get; set; }
        public string Almacen { get; set; }
        public string Rol { get; set; }

    }
}