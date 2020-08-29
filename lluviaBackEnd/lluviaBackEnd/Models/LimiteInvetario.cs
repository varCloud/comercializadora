using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class LimiteInvetario
    {
        public int idProducto { get; set; }
        public int idAlmacen { get; set; }
        public int idLimiteInventario { get; set; }

        public int minimo { get; set; }
        public int maximo { get; set; }
        public string descripcion { get; set; }
        public string descripcionAlmacen { get; set; }
        public int cantidadInventario { get; set; }

        public Status estatusInventario { get; set; }

    }
}