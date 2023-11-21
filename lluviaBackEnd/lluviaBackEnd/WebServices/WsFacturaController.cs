using lluviaBackEnd.Controllers;
using lluviaBackEnd.DAO;
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
using System.Web.Configuration;
using System.Web;
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
        public Notificacion<dynamic> ActualizaEstatusCancelacionFactura(Factura facturaRequest)
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

                Notificacion<dynamic> factura = new FacturaDAO().ObtenerPathXMLFactura(facturaRequest.id, facturaRequest.esPedidoEspecial);
                string path = HttpContext.Current.Server.MapPath("~" + factura.Modelo.pathArchivoFactura.Replace(".pdf", ".xml").Replace("Factura_", "Timbre_"));
                Comprobante comprobanteTimbrado = Utilerias.ManagerSerealization<Comprobante>.DeseralizarXMLFromPath(path);
                ConsultaEstatusFactura4.ConsultaCFDIServiceClient consultaCFDI = new ConsultaEstatusFactura4.ConsultaCFDIServiceClient();
                n.Modelo = consultaCFDI.Consulta(Utils.ExpresionImpresa(comprobanteTimbrado));
                if (n.Modelo != null && n.Modelo.Estado == "Cancelado")
                {
                    Factura f = new Factura();
                    f.idPedidoEspecial = (facturaRequest.esPedidoEspecial ? facturaRequest.id : 0);
                    f.idVenta = (!facturaRequest.esPedidoEspecial ? facturaRequest.id.ToString() : "0");
                    f.estatusFactura = EnumEstatusFactura.Cancelada;
                    f.mensajeError = "Cancelado correctamente | "+ n.Modelo.Estado;
                    new FacturaDAO().CancelarFactura(f);
                }
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
