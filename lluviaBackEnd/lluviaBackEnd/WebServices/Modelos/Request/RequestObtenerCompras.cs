using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerCompras
    {
        [Display(Name = "idCompra")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idCompra { get; set; }

        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }

        public Proveedor proveedor { get; set; }

        public Status statusCompra { get; set; }

        public Usuario usuario { get; set; }

    }
}