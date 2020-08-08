using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Ubicacion
    {
        public int idProducto { get; set; }
        public Int64 cantidad { get; set; }
        public DateTime fechaAlta { get; set; }

        public int idUbicacion { get; set; }

        public int idSucursal { get; set; }
        public int idAlmacen { get; set; }
        public int idPasillo { get; set; }

        public int idRaq { get; set; }
        public int idPiso { get; set; }

        public int idPisoRaq { get; set; }

        public string descripcionAlmacen { get; set; }
    }
}