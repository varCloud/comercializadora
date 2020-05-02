using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerUbicacionProductoInventario
    {
        public int idAlamacen { get; set; }
        public int idProducto { get; set; }

        /* 1.- sin acomodar , 2.- Acomodado  , 3 Todo*/
        public int EstatusProducto { get; set; }
    }


}