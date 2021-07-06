using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Response
{
    public class ResponseObtenerPisoPasilloRaq
    {
        public ResponseObtenerPisoPasilloRaq()
        {
           
        }
        public List<Pasillo> pasillos { get; set; }
        public List<Raq> raqs { get; set; }
        public List<Piso> pisos { get; set; }
    }
}