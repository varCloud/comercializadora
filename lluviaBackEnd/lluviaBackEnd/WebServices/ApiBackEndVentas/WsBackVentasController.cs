using lluviaBackEnd.Controllers;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace lluviaBackEnd.WebServices.ApiBackEndVentas
{
    public class WsBackVentasController : ApiController
    {
        [HttpPost]
        public Notificacion<Ventas> CancelaVenta(Ventas ventas)
        {
            Notificacion<String> n = new Notificacion<string>();
            var j = new VentasController().CancelaVenta(ventas);
            return (Notificacion<Ventas>)j.Data;
        }
    }
}
