using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Precio
    {

        public int contador { get; set; }
        public int idProducto { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float costo { get; set; }
        public Boolean activo { get; set; }
        public int idTipoPrecio { get; set; }
        public int cantidad { get; set; }
    }
}