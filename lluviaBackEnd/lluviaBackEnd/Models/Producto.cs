using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Producto
    {

        public int idProducto { get; set; }


        [Display(Name = "descripcion")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string  descripcion { get; set; }

        [Display(Name = "idUnidadMedida")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idUnidadMedida { get; set; }

        public string DescripcionUnidadMedida { get; set; }

        [Display(Name = "idLineaProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idLineaProducto { get; set; }

        public string DescripcionLinea { get; set; }

        [Display(Name = "cantidadUnidadMedida")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int cantidadUnidadMedida { get; set; }

        public string codigoBarras { get; set; }

        public Boolean activo { get; set; }

        [Display(Name = "articulo")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string articulo { get; set; }

        public string[] idLineaProductoConsulta { get; set; }
        
    }
}