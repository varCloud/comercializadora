using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestAprobarProductosProduccionAgranel
    {
        public int idUsuario { get; set; }
        public int idAlmacen { get; set; }
        public List<ProductosProduccionAgranel> productos { get; set; }
    }

    public class ProductosProduccionAgranel
    {
        public int idProcesoProduccionAgranel { get; set; }
        public int idProducto { get; set; }
        public int idUbicacion { get; set; }
        public float cantidadAtendida { get; set; }
        public string observaciones { get; set; }
        public int idEstatusProduccionAgranel { get; set; }
    }


}