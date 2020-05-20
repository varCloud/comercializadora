using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Retiros
    {

        public int idRetiro { get; set; }
        public float montoRetiro { get; set; }
        public int idUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public int idEstacion { get; set; }
        public string nombreEstacion { get; set; }
        public DateTime fechaAlta { get; set; }

        //public int MyProperty { get; set; }

    }
}