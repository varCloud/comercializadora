using lluviaBackEnd.Controllers;
using lluviaBackEnd.Models;
using lluviaBackEnd.Models.Facturacion;
using lluviaBackEnd.Utilerias;
using lluviaBackEnd.WebServices.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace lluviaBackEnd.WebServices
{
    public class WsFacturaController : ApiController
    {
        [System.Web.Http.HttpPost]
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

        public Notificacion<dynamic> ReenviarFactura(Factura f)
        {
            Notificacion<dynamic> n = new Notificacion<dynamic>();
            try
            {
                var j = new FacturaController().ReenviarFactura(f);
                return j;

            }
            catch (Exception ex)
            {

                return WsUtils<dynamic>.RegresaExcepcion(ex, null);
            }
            return n;
        }

        [System.Web.Http.HttpGet]
        public Notificacion<dynamic> ObtenerPeticionesPendientes(string rfc)
        {
            Notificacion<dynamic> n = new Notificacion<dynamic>();
            try
            {
                cancelaCFDI4Prod.enviaAcuseCancelacion obtener = new cancelaCFDI4Prod.enviaAcuseCancelacion();          
                n.Modelo = obtener.obtenerPeticionesPendientes(rfc);
                return n;

            }
            catch (Exception ex)
            {

                return WsUtils<dynamic>.RegresaExcepcion(ex, null);
            }
            return n;
        }

        [System.Web.Http.HttpPost]
        public Notificacion<dynamic> ObtenerEstatusFactura(Factura factura)
        {
            Notificacion<dynamic> n = new Notificacion<dynamic>();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                string pathServer = Utils.ObtnerFolder() + @"/";
                string path = pathServer + @"Timbre_329074_638344774457284015.xml";
                Comprobante comprobanteTimbrado = Utilerias.ManagerSerealization<Comprobante>.DeseralizarXMLFromPath(path);
                //string expresionImpresa = new FacturaController().ObtenerComprobanteCFDI4(factura);
                //Utils.ExpresionImpresa(comprobanteTimbrado);
                ConsultaEstatusFactura4.ConsultaCFDIServiceClient consultaCFDI = new ConsultaEstatusFactura4.ConsultaCFDIServiceClient();
                n.Modelo = consultaCFDI.Consulta(Utils.ExpresionImpresa(comprobanteTimbrado));
                return n;

            }
            catch (Exception ex)
            {

                return WsUtils<dynamic>.RegresaExcepcion(ex, null);
            }
            return n;
        }

    }
}
