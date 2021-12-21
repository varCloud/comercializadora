using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class ProductosDevueltosPedidoEspecial
    {
        public Int64 idProducto { get; set; }
        public float cantidad { get; set; }
        public float productosDevueltos { get; set; }
        public Int64 idPedidoEspecialDetalle { get; set; }

    }
}