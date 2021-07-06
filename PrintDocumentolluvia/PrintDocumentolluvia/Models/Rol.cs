using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Rol
    {

        public int idRol { get; set; }
        public string  descripcion { get; set; }
        public Boolean activo { get; set; }

    }
}