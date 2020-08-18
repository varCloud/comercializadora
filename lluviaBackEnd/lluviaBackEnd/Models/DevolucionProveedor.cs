using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class DevolucionProveedor
    {
        public int IdDevolucion { get; set; }
        public DateTime Fecha { get; set; }

        public int idCompra { get; set; }
        public Producto producto { get; set; }
        public Usuario usuario { get; set; }
        public Proveedor proveedor { get; set; }

        public string Observaciones { get; set; }

        public DevolucionProveedor()
        {
            producto = new Producto();
            usuario = new Usuario();
            proveedor = new Proveedor();
        }

    }
}