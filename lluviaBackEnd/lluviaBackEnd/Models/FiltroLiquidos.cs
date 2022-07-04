using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class FiltroLiquidos
    {
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public Int64 idUbicacion { get; set; }
        public string descripcionUbicacion { get; set; }
        public Int64 idProducto { get; set; }
        public string  descripcionProducto { get; set; }
        public Int64 idUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public DateTime fechaAlta { get; set; }
        public Int64 idRol { get; set; }
        public string descripcionRol { get; set; }
        public Int64 idTipoMovimiento { get; set; }
        public string descTipoMovInventario { get; set; }
        public int id { get; set; }
    }
}