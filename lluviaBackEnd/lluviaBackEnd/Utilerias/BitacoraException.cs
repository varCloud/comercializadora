using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Utilerias
{
    public class BitacoraException<T> where T : class
    {
        public BitacoraException()
        {

        }
        public BitacoraException(String numUsuarioBitacora, T value, string Metodo = "")
        {
            this.FechaBitacora = System.DateTime.Now.ToShortDateString();
            this.HoraBitacora = System.DateTime.Now.ToShortTimeString();
            this.numUsuarioBitacora = numUsuarioBitacora;
            this.Metodo = Metodo;
            this.Parametros = value;
            //this.ex = ex;
        }

        public String FechaBitacora { get; set; }

        public String HoraBitacora { get; set; }
        public string numUsuarioBitacora { get; set; }

        public string Metodo { get; set; }
        //public string  ex { get; set; }
        public T Parametros { get; set; }



    }

}