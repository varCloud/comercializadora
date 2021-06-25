using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestActualizaEstatusProductoCompra
    {
        public int idAlmacen { get; set; }
        public int idUsuario { get; set; }
        public int idCompra { get; set; }
        public List<ProductosCompra> Productos { get; set; }
    }

    public class ProductosCompra {

        public int idCompraDetalle { get; set; }

        public float cantidad { get; set; }
        public int precio { get; set; }
        public float cantidadRecibida { get; set; }
        public float cantidadDevuelta { get; set; }
        public string observaciones { get; set; }

        public int idProducto { get; set; }
        /// <summary>
        ///1	Cantidad Correcta
        ///2	Cantidad Mayor a la solicitada
        ///3	Cantidad Menor a la solicitada
        ///4	Devolucion paracial del producto
        ///5	Devolucion completa del producto
        /// </summary>
        public int idEstatusProductoCompra { get; set; }

    }

}