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

        [Display(Name = "idProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idProducto { get; set; }

        [Display(Name = "descripcionProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string descripcionProducto { get; set; }

        [Display(Name = "idLineaProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idLineaProducto { get; set; }

        [Display(Name = "idProveedor")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idProveedor { get; set; }

        [Display(Name = "descripcionProveedor")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string descripcionProveedor { get; set; }

        [Display(Name = "idUsuario")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idUsuario { get; set; }

        [Display(Name = "nombreUsuario")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string nombreUsuario { get; set; }

        [Display(Name = "fechaAlata")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public DateTime fechaAlata { get; set; }

        [Display(Name = "cantidad")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int cantidad { get; set; }

        [Display(Name = "idEstatusCompra")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idEstatusCompra { get; set; }

        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }

    }
}