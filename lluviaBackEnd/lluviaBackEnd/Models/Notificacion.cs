using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lluviaBackEnd.Models
{
   public class Notificacion<T> where T : class
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; }
        public string Errorline { get; set; }
        public T Modelo { get; set; }
    }
}
