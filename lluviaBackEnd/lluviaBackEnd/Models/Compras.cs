using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Compras
    {        

        [Display(Name = "idCompra")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idCompra { get; set; }

        [Display(Name = "fechaAlta")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public DateTime fechaAlta { get; set; }

        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }

        public int totalCantProductos { get; set; }
        public float montoTotal { get; set; }

        public string observaciones { get; set; }

        public List<Producto> listProductos { get; set; }
        public Proveedor proveedor { get; set; }

        public Status statusCompra { get; set; }

        public Usuario usuario { get; set; }

        public Producto producto { get; set; }

        public int totalCantProductosRecibidos { get; set; }
        public float montoTotalRecibido { get; set; }

        public EnumEstadoCompras estadoCompra { get; set; }

        public Compras()
        {
            listProductos = new List<Producto>();
            proveedor = new Proveedor();
            statusCompra = new Status();
            usuario = new Usuario();
            producto = new Producto();
        }

    }
}