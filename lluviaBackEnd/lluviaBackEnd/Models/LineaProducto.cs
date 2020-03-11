using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class LineaProducto
    {

        public int idLineaProducto { get; set; }


        [Display(Name = "descripcion")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string  descripcion { get; set; }

        public Boolean activo { get; set; }
        public int contador { get; set; }

    }
}