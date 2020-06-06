using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using lluviaBackEnd.Models.Facturacion;
using lluviaBackEnd.servicioTimbrarPruebas;

using lluviaBackEnd.Utilerias;
using lluviaBackEnd.WebServices.Modelos;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class FacturaController : Controller
    {
        // GET: FacturasController

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
        public ActionResult CancelarFactura(Factura factura)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                Cancelacion c = new Cancelacion();
                Sesion UsuarioActual =(Sesion) Session["UsuarioActual"];
                factura.idUsuario = UsuarioActual.idUsuario;
                c.Fecha = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                c.RfcEmisor = "CMV980925LQ7";
                c.Folios = new Folios();
                c.Folios.UUID = "38b622d6-e612-4d87-82bf-b49e278f0d8e";

                string pathFactura = Utils.ObtnerFolder() + '/';
                string documentoOriginal = Utilerias.ManagerSerealization<Cancelacion>.SerealizarToString(c);

                XmlDocument originalXmlDocument = new XmlDocument() { PreserveWhitespace = false };
                originalXmlDocument.LoadXml(documentoOriginal);

                // --------------------------------------------------------------------------
                // III. GENERAR ELEMENTO <Signature> USANDO EL CSD DEL EMISOR
                // --------------------------------------------------------------------------

                XmlElement signatureElement = ProcesaCfdi.GenerateXmlSignature(originalXmlDocument);

                // --------------------------------------------------------------------------
                // IV. INCRUSTAR EL ELEMENTO <Signature> DENTRO DEL DOCUMENTO XML ORIGINAL
                // --------------------------------------------------------------------------

                originalXmlDocument.DocumentElement.AppendChild(originalXmlDocument.ImportNode(signatureElement, true));

                // --------------------------------------------------------------------------
                // V. EL XML RESULTANTE ES LA SOLICITUD FIRMADA DE CANCELACIÓN
                // --------------------------------------------------------------------------

                //Debug.WriteLine(originalXmlDocument.OuterXml);

                enviaAcuseCancelacion enviaCancelacion = new enviaAcuseCancelacion();
                string result = enviaCancelacion.CallenviaAcuseCancelacion(originalXmlDocument.OuterXml);
                System.IO.File.WriteAllText(pathFactura + "Cancelacion_" + factura.idVenta + ".xml", result);
                AcuseCancelacionResponseWS cancelacion = ManagerSerealization<AcuseCancelacionResponseWS>.DeserializeXMLStringToObject(result);
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
                return Json(notificacion, JsonRequestBehavior.AllowGet); ;

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
            try
            {
                string pathFactura = WebConfigurationManager.AppSettings["pathFacturas"].ToString() + Utils.ObtnerAnoMesFolder().Replace("\\","/");
                string pathServer = Utils.ObtnerFolder() + @"/";
                
                FacturaDAO facturacionDAO = new FacturaDAO();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                //factura.idVenta = "64";
                factura.idUsuario = UsuarioActual.idUsuario;
                Comprobante comprobante = facturacionDAO.ObtenerConfiguracionComprobante();
                comprobante.Folio = factura.folio = factura.idVenta;
                /*
                comprobante.Emisor.Rfc = "COVO781128LJ1";
                comprobante.Emisor.Nombre = "OSEAS AURELIANO CORNEJO VAZQUEZ";
                comprobante.Emisor.RegimenFiscal = 621;                
                */

                Dictionary<string, object> items = facturacionDAO.ObtenerComprobante(factura.idVenta, comprobante);
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
                respuestaTimbrado respuesta = (respuestaTimbrado) ProcesaCfdi.TimbrarEdifact(ProcesaCfdi.Base64Encode(xmlSerealizado));

                if (respuesta.codigoResultado.Equals("100"))
                {

                    string xmlTimbradoDecodificado = ProcesaCfdi.Base64Decode(respuesta.documentoTimbrado);
                    
                    Comprobante comprobanteTimbrado = Utilerias.ManagerSerealization<Comprobante>.DeserializeXMLStringToObject(xmlTimbradoDecodificado);
                    comprobanteTimbrado.Addenda = new ComprobanteAddenda();
                    comprobanteTimbrado.Addenda.conceptosAddenda = (List<ConceptosAddenda>)items["conceptosAddenda"];
                    comprobanteTimbrado.Addenda.descripcionFormaPago = items["descripcionFormaPago"].ToString();
                    comprobanteTimbrado.Addenda.descripcionUsoCFDI= items["descripcionUsoCFDI"].ToString();
                    comprobanteTimbrado.Addenda.descripcionTipoComprobante = "Ingreso";

                    
                    Utils.GenerarQRSAT(comprobanteTimbrado, pathServer + ("Qr_" + factura.idVenta + ".jpg"));
                    Utils.GenerarFactura(comprobanteTimbrado, pathServer, factura.idVenta);
                    System.IO.File.WriteAllText(pathServer + "Timbre_" + factura.idVenta + ".xml", xmlTimbradoDecodificado);

                    factura.pathFactura = pathFactura;
                    factura.estatusFactura = EnumEstatusFactura.Facturada;
                    factura.mensajeError = "OK";
                    factura.fechaTimbrado = comprobanteTimbrado.Complemento.TimbreFiscalDigital.FechaTimbrado;
                    factura.UUID = comprobanteTimbrado.Complemento.TimbreFiscalDigital.UUID;

                }
                else
                {
                    factura.estatusFactura = EnumEstatusFactura.Error;
                    factura.mensajeError = respuesta.codigoResultado + " |" + respuesta.codigoDescripcion;
                    System.IO.File.WriteAllText(pathFactura + ("Comprobante_" + factura.idVenta+".xml"), xmlSerealizado);
                    Utilerias.ManagerSerealization<respuestaTimbrado>.Serealizar(respuesta, pathFactura + ("respuesta_" + factura.idVenta));
                }
                notificacion = new FacturaDAO().GuardarFactura(factura);
                return Json(notificacion, JsonRequestBehavior.AllowGet); ;
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
                Notificacion<List<Factura>> notificacion=new FacturaDAO().ObtenerFacturas(factura);
                return PartialView(notificacion);
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
