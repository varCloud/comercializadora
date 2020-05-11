using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestActualizarUbicacionInventario
    {
   
        public int idProducto { get; set; }
        public int idUsuario { get; set; }
        public int cantidad { get; set; }
        public int idAlmacen { get; set; }
        public int IdUbicacion { get; set; }
        public int idPiso { get; set; }
        public int idPasillo { get; set; }
        public int idRack { get; set; }
    }
}