﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Factura
    {

        public int idFactura { get; set; }
        public int idVenta { get; set; }


        //[Display(Name = "nombre")]
        //[Required(ErrorMessage = "Este campo no puede estar vacio")]
        //public string  nombre { get; set; }

        //[Display(Name = "descripcion")]
        //[Required(ErrorMessage = "Este campo no puede estar vacio")]
        //public string  descripcion { get; set; }

        //[Display(Name = "telefono")]
        //[Required(ErrorMessage = "Este campo no puede estar vacio")]
        //public string  telefono { get; set; }

        //[Display(Name = "direccion")]
        //[Required(ErrorMessage = "Este campo no puede estar vacio")]
        //public string direccion { get; set; }


        //public Boolean activo { get; set; }

    }
}