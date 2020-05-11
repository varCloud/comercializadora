using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Ventas
    {

        public Ventas()
        {
            formaPago = 1;
        }

        [Display(Name = "idVenta")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idVenta { get; set; }

        [Display(Name = "idCliente")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idCliente { get; set; }

        [Display(Name = "nombreCliente")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string nombreCliente { get; set; }

        [Display(Name = "cantidad")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int cantidad { get; set; }

        [Display(Name = "fechaAlta")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public DateTime fechaAlta { get; set; }

        [Display(Name = "idUsuario")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idUsuario { get; set; }

        [Display(Name = "nombreUsuario")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string nombreUsuario { get; set; }

        [Display(Name = "descripcionProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string descripcionProducto { get; set; }

        [Display(Name = "idProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idProducto { get; set; }


        [Display(Name = "idLineaProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idLineaProducto { get; set; }

        public DateTime fechaIni { get; set; }

        public DateTime fechaFin { get; set; }

        public int contador { get; set; }
        public float precio { get; set; }
        public int formaPago { get; set; }
        public int tipoConsulta { get; set; }

        public int idFactura { get; set; }
        public int idEstatusFactura { get; set; }
        public string descripcionEstatusFactura { get; set; }
        public string rutaFactura { get; set; }
        public int usoCFDI { get; set; }
        public float descuento { get; set; }
        public float montoTotal { get; set; }
        public int idStatusVenta { get; set; }
        public int idFactFormaPago { get; set; }
        public int idFactUsoCFDI { get; set; }
    }
}