using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos
{
    public static class WsUtils<T> where T : class
    {
        public static Notificacion<T> RegresaExcepcion(Exception ex, T Modelo)
        {
            return new Notificacion<T>()
            {
                Estatus = -1,
                Mensaje = ex.Message,
                Modelo = Modelo
            };
        }
    }
}