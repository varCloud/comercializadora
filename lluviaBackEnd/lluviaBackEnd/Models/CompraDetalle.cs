using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class CompraDetalle
    {

        public int idCompraDetalle { get; set; }
        public int idCompra { get; set; }
        public int cantidad { get; set; }
        public int precio { get; set; }
        public int cantidadRecibida { get; set; }
        public string observaciones { get; set; }

        public Producto producto { get; set; }

        public Usuario usuario { get; set; }

        public Status status { get; set; }


    }
}