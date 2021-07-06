using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RegistraProductoInventarioFisico
    {
        public int idInventarioFisico { get; set; }
        public int idProducto { get; set; }
        public int idUbicacion { get; set; }
        public int idUsuario { get; set; }
        public float cantidadEnFisico { get; set; }
    }
}