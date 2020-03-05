using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos
{
    public class RequestLogin
    {
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
    }
}