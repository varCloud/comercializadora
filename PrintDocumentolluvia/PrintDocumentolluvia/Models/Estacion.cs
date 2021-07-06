using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Estacion
    {

        public int contador { get; set; }
        public int idEstacion { get; set; }

        [Display(Name = "IdAlmacen")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idAlmacen { get; set; }

        public string nombreAlmacen { get; set; }
        public string macAdress { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string nombre { get; set; }

        [Display(Name = "Numero")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int numero { get; set; }

        public bool configurado { get; set; }

        public int idUsuario { get; set; }

        public DateTime fechaAlta { get; set; }

        public int idStatus { get; set; }

        public float montoTotalDia { get; set; }
        public float montoTotalSemana { get; set; }
        public float montoTotalMes { get; set; }
        public float montoTotalAnio { get; set; }
        public int idSucursal { get; set; }
    }
}