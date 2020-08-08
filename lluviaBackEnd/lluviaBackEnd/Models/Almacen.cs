using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Almacen
    {

        public int idAlmacen { get; set; }
        public int idSucursal { get; set; }
        public string  descripcion { get; set; }
        public Boolean activo { get; set; }
        public int idTipoAlmacen { get; set; }
    }
}