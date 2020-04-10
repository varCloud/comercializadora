using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class FormaPago
    {
        public int id { get; set; }
        public string formaPago { get; set; }
        public string descripcion { get; set; }
    }
}