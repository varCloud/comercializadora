using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Ubicacion
    {
        public int idProducto { get; set; }
        public string descripcionProducto { get; set; }
        public string codigoBarras { get; set; }
        public Int64 cantidad { get; set; }
        public DateTime fechaAlta { get; set; }
        public int idUbicacion { get; set; }
        public int idSucursal { get; set; }
        public string descripcionSucursal{ get; set; }
        public int idAlmacen { get; set; }
        public int idPasillo { get; set; }

        public string descripcionPasillo { get; set; }

        public int idRaq { get; set; }
        public string descripcionRaq { get; set; }
        public int idPiso { get; set; }

        public string descripcionPiso { get; set; }

        public int idPisoRaq { get; set; }
        public string descripcionAlmacen { get; set; }

        public bool fraccion { get; set; }

        /// <summary>
        /// solo se usa para cuando existe un inventario fisico
        /// </summary>
        public int ajustado { get; set; }

        public int cantidadEnFisico { get; set; }
    }
}