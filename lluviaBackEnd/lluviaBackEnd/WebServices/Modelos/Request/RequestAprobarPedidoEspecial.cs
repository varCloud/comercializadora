using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestAprobarPedidoEspecial
    {
        public int idAlmacenOrigen { get; set; }
        public int idAlmacenDestino { get; set; }
        public int idUsuario { get; set; }
        public int idPedidoInterno { get; set; }

        public List<ProductosPedidoEspecial> Productos { get; set; }

    }


    public class ProductosPedidoEspecial
    {
        public int idPedidoInternoDetalle { get; set; }
        public int idUbicacion { get; set; }
        public int idProducto { get; set; }
        public int cantidadAtendida { get; set; }
        public string Observaciones { get; set; }
        public int cantidadSolicitada { get; set; }

    }
}