using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Proveedor
    {

        public int idProveedor { get; set; }


        [Display(Name = "nombre")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string  nombre { get; set; }

        [Display(Name = "descripcion")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string  descripcion { get; set; }

        [Display(Name = "telefono")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string  telefono { get; set; }

        [Display(Name = "direccion")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string direccion { get; set; }

        public int totalPedidosIncompletos { get; set; }
        public int totalPedidosTotales { get; set; }

        public int totalPedidosCompletos { get; set; }
        public int totalPedidosMayorSolicitado { get; set; }
        public float PorcAtendido { get; set; }

        public Boolean activo { get; set; }
        public int contador { get; set; }

        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
    }
}