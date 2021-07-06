using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class UsoCFDI
    {
        public int id { get; set; }
        public string usoCFDI { get; set; }
        public string descripcion { get; set; }
    }
}