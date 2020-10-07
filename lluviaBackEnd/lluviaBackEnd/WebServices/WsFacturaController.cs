using lluviaBackEnd.Controllers;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace lluviaBackEnd.WebServices
{
    public class WsFacturaController : ApiController
    {
        public Notificacion<String> GenerarFactura(Factura f) {
            Notificacion<String> n = new Notificacion<string>();
            
            try
            {
                var j =  new FacturaController().GenerarFactura(f);
                return (Notificacion<String>)j.Data;
            }
            catch (Exception ex)
            {

                return WsUtils<String>.RegresaExcepcion(ex, null);
            }
            return n;
        }

        public Notificacion<String> CancelarFactura(Factura f)
        {
            Notificacion<String> n = new Notificacion<string>();
            try
            {
                var j = new FacturaController().CancelarFactura(f);
                return (Notificacion<String>)j.Data;
            }
            catch (Exception ex)
            {

                return WsUtils<String>.RegresaExcepcion(ex, null);
            }
            return n;
        }
    }
}
