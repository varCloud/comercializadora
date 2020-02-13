using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Sesion
    {
        [Display(Name ="Usuario")]
        [Required(ErrorMessage ="Este campo no puede estar vacio")]
        public int NumeroUsuario { get; set; }
        [Display(Name = "Constraseña")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Contrasena { get; set; }
    }
}