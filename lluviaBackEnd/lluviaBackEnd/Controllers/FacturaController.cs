﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using lluviaBackEnd.Models.Facturacion;
using lluviaBackEnd.Models.Facturacion.Produccion;
using lluviaBackEnd.servicioTimbradoProductivo;

using lluviaBackEnd.Utilerias;
using lluviaBackEnd.WebServices.Modelos;
using log4net;
using Newtonsoft.Json;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class FacturaController : Controller
    {
        private readonly ILog log4netRequest;
        public FacturaController()
        { 
            log4netRequest = LogManager.GetLogger("LogLluvia");
        }

        public ActionResult Factura()
        {
            //ViewBag.lstFacturas = new FacturaDAO().ObtenerFacturas(new Models.Factura() { idFactura = 0 });
            return View();
        }

        //    public ActionResult ObtenerFactura(int idFactura)
        //    {
        //        try
        //        {
        //            Models.Factura p = new FacturaDAO().ObtenerFacturas(new Models.Factura() { idFactura = idFactura })[0];
        //            return Json(p, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //    public ActionResult _ObtenerFacturas(int idFactura)
        //    {
        //        try
        //        {
        //            //List<Models.Socio> Lstsocio = new SocioDAO().ObtenerSocios(new Models.Socio() { IdSocio = 0 });
        //            ViewBag.lstFacturas = new FacturaDAO().ObtenerFacturas(new Models.Factura() { idFactura = 0 });
        //            return PartialView("_ObtenerFacturas");
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }


        [HttpPost]
        public JsonResult CancelarFactura(Factura factura)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            Cancelacion c = null;
            Sesion UsuarioActual = null;
            try
            {
                if (Session != null)
                {
                    UsuarioActual = (Sesion)Session["UsuarioActual"];
                    factura.idUsuario = UsuarioActual.idUsuario;
                }
                c = new FacturaDAO().ObtenerCancelacionFactura(factura);
                if (c != null)
                {
                    string timeStamp = "_" + DateTime.Now.Ticks.ToString();
                    log4netRequest.Debug("C es diferente de null");
                    string pathFactura = Utils.ObtnerFolder() + @"/";
                    string documentoOriginal = Utilerias.ManagerSerealization<Cancelacion>.SerealizarToString(c);
                    log4netRequest.Debug("pathFactura : " + pathFactura);
                    log4netRequest.Debug("Documento a cancelar : " +  documentoOriginal);
                    string result = ProcesaCfdi.CancelarFacturaEdifact(documentoOriginal);
                    System.IO.File.WriteAllText(pathFactura + "Cancelacion_" + factura.idVenta+timeStamp + ".xml", result);
                    //pathFactura = @"F:\Documents\comercializadora\lluviaBackEnd\lluviaBackEnd\Facturas\2021\AGOSTO\";
                    //string text = System.IO.File.ReadAllText(pathFactura+ "Cancelacion_35705.xml");
                    AcuseCancelacionProductivoResponseWs cancelacion = ProcesaCfdi.ObtnerAcuseCancelacionFactura(result);
                    if (cancelacion.Folios.EstatusUUID.ToString().Equals("201"))
                    {
                        factura.estatusFactura = EnumEstatusFactura.Cancelada;
                        factura.mensajeError = "Cancelada correctamente";
                    }
                    else
                    {
                        factura.estatusFactura = EnumEstatusFactura.Cancelada;
                        factura.mensajeError = "Ocurrio un error al intentar cancelar la factura, codigo error :" + cancelacion.Folios.EstatusUUID;
                    }
                    notificacion = new FacturaDAO().CancelarFactura(factura);
                }
                else
                {
                    notificacion.Estatus = -1;
                    notificacion.Mensaje = "Ocurrio un error al intentar cancelar la factura";
                }

                return Json(notificacion, JsonRequestBehavior.AllowGet); ;

            }
            catch (Exception ex)
            {
                log4netRequest.Debug(ex.Message + " " + ex.StackTrace);
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult RegenerarFactura(Factura factura)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            Dictionary<string, object> items = null;
            try
            {
                string pathFactura = WebConfigurationManager.AppSettings["pathFacturas"].ToString() + Utils.ObtnerAnoMesFolder().Replace("\\", "/");
                string pathServer = Utils.ObtnerFolder() + @"/";
                FacturaDAO facturacionDAO = new FacturaDAO();
                Sesion UsuarioActual = null;
                if (Session != null)
                    UsuarioActual = (Sesion)Session["UsuarioActual"];
  
                factura.idUsuario = factura.idUsuario == 0 ? UsuarioActual.idUsuario : factura.idUsuario;          
                Comprobante comprobanteTimbrado = SerializerManager<Comprobante>.DeseralizarXML(pathServer + "Timbre_" + factura.idVenta + ".xml");
                comprobanteTimbrado.Addenda = new ComprobanteAddenda();
                comprobanteTimbrado.Addenda.conceptosAddenda = (List<ConceptosAddenda>)items["conceptosAddenda"];
                comprobanteTimbrado.Addenda.descripcionFormaPago = items["descripcionFormaPago"].ToString();
                comprobanteTimbrado.Addenda.descripcionUsoCFDI = items["descripcionUsoCFDI"].ToString();
                comprobanteTimbrado.Addenda.descripcionTipoComprobante = "Ingreso";
                Utils.GenerarQRSAT(comprobanteTimbrado, pathServer + ("Qr_" + factura.idVenta + ".jpg"));
                Utils.GenerarFactura(comprobanteTimbrado, pathServer, factura.idVenta, items , "");
                factura.pathArchivoFactura = pathFactura;
                factura.estatusFactura = EnumEstatusFactura.Facturada;
                factura.mensajeError = "OK";
                factura.fechaTimbrado = comprobanteTimbrado.Complemento.TimbreFiscalDigital.FechaTimbrado;
                factura.UUID = comprobanteTimbrado.Complemento.TimbreFiscalDigital.UUID;
                Task.Factory.StartNew(() =>
                {
                    Email.NotificacionPagoReferencia(items["correoCliente"].ToString(), pathServer + "Timbre_" + factura.idVenta + ".xml", factura);
                });
                
                notificacion = new Notificacion<string>()
                {
                    Estatus = 200,
                    Mensaje = "Factura Generada y enviada"
                };
                return Json(notificacion, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult GenerarFactura(Factura factura)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            Dictionary<string, object> items = null;

            try
            {
                string pathFactura = WebConfigurationManager.AppSettings["pathFacturas"].ToString() + Utils.ObtnerAnoMesFolder().Replace("\\", "/");
                string pathServer = Utils.ObtnerFolder() + @"/";
                log4netRequest.Debug("pathFactura: " + pathFactura);
                log4netRequest.Debug("pathServer : " + pathServer);
                FacturaDAO facturacionDAO = new FacturaDAO();
                Sesion UsuarioActual = null;
                if (Session != null)
                    UsuarioActual = (Sesion)Session["UsuarioActual"];
                //factura.idVenta = "64";
                factura.idUsuario = factura.idUsuario == 0 ? UsuarioActual.idUsuario : factura.idUsuario;
                Comprobante comprobante = facturacionDAO.ObtenerConfiguracionComprobante();
                comprobante.Folio = factura.folio = factura.idVenta;
                /*
                comprobante.Emisor.Rfc = "COVO781128LJ1";
                comprobante.Emisor.Nombre = "OSEAS AURELIANO CORNEJO VAZQUEZ";
                comprobante.Emisor.RegimenFiscal = 621;                
                */
                items = facturacionDAO.ObtenerComprobante(factura.idVenta, comprobante);
                if (items["estatus"].ToString().Equals("200"))
                {
                    comprobante = (items["comprobante"] as Comprobante);
                    facturacionDAO.ObtenerImpuestosGenerales(ref comprobante);
                    facturacionDAO.ObtenerTotal(ref comprobante);

                    Dictionary<string, string> certificados = Utilerias.ProcesaCfdi.ObtenerCertificado();
                    if (certificados == null)
                        return Json("Error al obtener los certificados", JsonRequestBehavior.AllowGet);

                    comprobante.Certificado = certificados["Certificado"];
                    comprobante.NoCertificado = certificados["NoCertificado"];

                    string xmlSerealizado = Utilerias.ProcesaCfdi.SerializaXML33(comprobante);
                    string cadenaOriginal = Utilerias.ProcesaCfdi.GeneraCadenaOriginal33(xmlSerealizado);
                    comprobante.Sello = Utilerias.ProcesaCfdi.GeneraSello(cadenaOriginal);
                    xmlSerealizado = Utilerias.ProcesaCfdi.SerializaXML33(comprobante);

                    //TIMBRAR CON EDI-FACT
                    respuestaTimbrado respuesta = (respuestaTimbrado)ProcesaCfdi.TimbrarEdifact(ProcesaCfdi.Base64Encode(xmlSerealizado));
                    string timeStamp = "_" + DateTime.Now.Ticks.ToString();
                    if (respuesta.codigoResultado.Equals("100"))
                    {

                        string xmlTimbradoDecodificado = ProcesaCfdi.Base64Decode(respuesta.documentoTimbrado);

                        Comprobante comprobanteTimbrado = Utilerias.ManagerSerealization<Comprobante>.DeserializeXMLStringToObject(xmlTimbradoDecodificado);
                        comprobanteTimbrado.Addenda = new ComprobanteAddenda();
                        comprobanteTimbrado.Addenda.conceptosAddenda = (List<ConceptosAddenda>)items["conceptosAddenda"];
                        comprobanteTimbrado.Addenda.descripcionFormaPago = items["descripcionFormaPago"].ToString();
                        comprobanteTimbrado.Addenda.descripcionUsoCFDI = items["descripcionUsoCFDI"].ToString();
                        comprobanteTimbrado.Addenda.descripcionTipoComprobante = "Ingreso";
                        

                        Utils.GenerarQRSAT(comprobanteTimbrado, pathServer + ("Qr_" + factura.idVenta+timeStamp+ ".jpg"));
                        Utils.GenerarFactura(comprobanteTimbrado, pathServer, factura.idVenta, items , timeStamp);
                        System.IO.File.WriteAllText(pathServer + "Timbre_" + factura.idVenta+timeStamp + ".xml", xmlTimbradoDecodificado);

                        factura.pathArchivoFactura = pathFactura + "/Factura_" + factura.idVenta + timeStamp + ".pdf";
                        factura.estatusFactura = EnumEstatusFactura.Facturada;
                        factura.mensajeError = "OK";
                        factura.fechaTimbrado = comprobanteTimbrado.Complemento.TimbreFiscalDigital.FechaTimbrado;
                        factura.UUID = comprobanteTimbrado.Complemento.TimbreFiscalDigital.UUID;

                        Task.Factory.StartNew(() =>
                        {
                            if(!string.IsNullOrEmpty(items["correoCliente"].ToString()))
                                Email.NotificacionPagoReferencia(items["correoCliente"].ToString(), pathServer + "Timbre_" + factura.idVenta+timeStamp + ".xml", factura);
                        });


                    }
                    else
                    {
                        factura.estatusFactura = EnumEstatusFactura.Error;
                        factura.mensajeError = respuesta.codigoResultado + " |" + respuesta.codigoDescripcion;
                        System.IO.File.WriteAllText(pathServer + ("Comprobante_" + factura.idVenta+timeStamp + ".xml"), xmlSerealizado);
                        Utilerias.ManagerSerealization<respuestaTimbrado>.Serealizar(respuesta, pathServer + ("respuesta_" + factura.idVenta));
                        //Email.NotificacionPagoReferencia("var901106@gmail.com");
                    }

                    notificacion = new FacturaDAO().GuardarFactura(factura);
                    notificacion.Mensaje += " " + factura.mensajeError;
                    return Json(notificacion, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    notificacion.Estatus = Convert.ToInt16(items["estatus"]);
                    notificacion.Mensaje = items["mensaje"].ToString();
                    return Json(notificacion, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Facturas()
        {
            try
            {
                List<SelectListItem> listUsuarios;
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                if (usuario.idRol == 1)
                {
                    listUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                }
                else
                    listUsuarios = new UsuarioDAO().ObtenerUsuarios(usuario.idUsuario).Where(x => x.Value != "0").ToList();
                ViewBag.listUsuarios = listUsuarios;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult _ObtenerFacturas(Factura factura)
        {
            try
            {
                Notificacion<List<Factura>> notificacion = new FacturaDAO().ObtenerFacturas(factura);
                return PartialView(notificacion);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult ObtenerDetalleFactura(Int64 idVenta)
        {
            try
            {
                Notificacion<dynamic>  notificacion = new FacturaDAO().ObtenerDetalleFactura(idVenta);                
                return Json(JsonConvert.SerializeObject(notificacion),JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult ReenviarFactura(Int64 idVenta,string correo)
        {
            try
            {
                Notificacion<string> notificacion = new Notificacion<string>();
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Enviado";
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        //    [HttpPost]
        //    public ActionResult ActualizarEstatusFactura(int idFactura, bool activo)
        //    {
        //        try
        //        {
        //            Result result = new FacturaDAO().ActualizarEstatusFactura(idFactura, activo);
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

    }


}
