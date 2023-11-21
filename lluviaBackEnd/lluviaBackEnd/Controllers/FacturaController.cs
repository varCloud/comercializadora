using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using CsvHelper;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using lluviaBackEnd.Models.Facturacion;
using lluviaBackEnd.Models.Facturacion.Produccion;
using lluviaBackEnd.servicioTimbradoProductivoV4;

using lluviaBackEnd.Utilerias;
using lluviaBackEnd.WebServices.Modelos;
using log4net;
using Newtonsoft.Json;
using static iTextSharp.text.pdf.AcroFields;

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
            return View();
        }


        [HttpPost]
        public JsonResult CancelarFactura(Factura factura)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            CancelarCFDI40 c = null;
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
                    string pathFactura = Utils.ObtnerFolder() + @"/";
                    string documentoOriginal = Utilerias.ManagerSerealization<CancelarCFDI40>.SerealizarToString(c);
                    log4netRequest.Debug("pathFactura : " + pathFactura);
                    log4netRequest.Debug("Documento a cancelar : " + documentoOriginal);
                    string result = ProcesaCfdi.CancelarFacturaEdifact(documentoOriginal);
                    System.IO.File.WriteAllText(pathFactura + "Cancelacion_" + factura.idVenta + timeStamp + ".xml", result);
                    AcuseCancelacionProductivoResponseWs cancelacion = ProcesaCfdi.ObtnerAcuseCancelacionFactura(result);
                    string statusCancelacion = cancelacion.Folios.EstatusUUID.ToString();
                    if (statusCancelacion.Equals("201"))
                    {
                        factura.estatusFactura = EnumEstatusFactura.Pendiente_de_cancelacion;
                        factura.mensajeError = "En proceso de cancelacion.";
                        notificacion = new FacturaDAO().CancelarFactura(factura);
                    }
                    if (statusCancelacion.Equals("202"))
                    {
                        notificacion.Estatus = -1;
                        notificacion.Mensaje = "Factura previamente enviada espere un momento y consulte su estado en el modulo de facturas.";
                    }
                    else
                    {
                        notificacion.Estatus = -1;
                        notificacion.Mensaje = "Espere un momento y vuelva a intentarlo:  [estatus]: "+ statusCancelacion;
                    }
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
                Utils.GenerarFactura(comprobanteTimbrado, pathServer, factura.idVenta, items, "");
                factura.pathArchivoFactura = pathFactura;
                factura.estatusFactura = EnumEstatusFactura.Facturada;
                factura.mensajeError = "OK";
                factura.fechaTimbrado = comprobanteTimbrado.Complemento.TimbreFiscalDigital.FechaTimbrado;
                factura.UUID = comprobanteTimbrado.Complemento.TimbreFiscalDigital.UUID;
                Task.Factory.StartNew(() =>
                {
                    Email.NotificacionPagoReferencia(items["correoCliente"].ToString(), pathServer + "Timbre_" + factura.idVenta + ".xml", factura, "");
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
                FacturaDAO facturacionDAO = new FacturaDAO();
                Sesion UsuarioActual = null;
                if (Session != null)
                    UsuarioActual = (Sesion)Session["UsuarioActual"];
                factura.idUsuario = factura.idUsuario == 0 ? UsuarioActual.idUsuario : factura.idUsuario;
                Comprobante comprobante = facturacionDAO.ObtenerConfiguracionComprobante();
                comprobante.Folio = factura.folio = (factura.idVenta.Equals("0") || string.IsNullOrEmpty(factura.idVenta) ? "PE" + factura.idPedidoEspecial : factura.idVenta);
                items = facturacionDAO.ObtenerComprobante(factura, comprobante);
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

                    string xmlSerealizado = Utilerias.ProcesaCfdi.SerializaXML33(comprobante, true);
                    string cadenaOriginal = Utilerias.ProcesaCfdi.GeneraCadenaOriginal33(xmlSerealizado);
                    comprobante.Sello = Utilerias.ProcesaCfdi.GeneraSello(cadenaOriginal);
                    xmlSerealizado = Utilerias.ProcesaCfdi.SerializaXML33(comprobante, true);

                    //TIMBRAR CON EDI-FACT
                    respuestaTimbrado respuesta = (respuestaTimbrado)ProcesaCfdi.TimbrarEdifact40(ProcesaCfdi.Base64Encode(xmlSerealizado));
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


                        Utils.GenerarQRSAT(comprobanteTimbrado, pathServer + ("Qr_" + comprobante.Folio + timeStamp + ".jpg"));
                        Utils.GenerarFactura(comprobanteTimbrado, pathServer, comprobante.Folio, items, timeStamp);
                        System.IO.File.WriteAllText(pathServer + "Timbre_" + comprobante.Folio + timeStamp + ".xml", xmlTimbradoDecodificado);

                        factura.pathArchivoFactura = pathFactura + "/Factura_" + comprobante.Folio + timeStamp + ".pdf";
                        factura.estatusFactura = EnumEstatusFactura.Facturada;
                        factura.mensajeError = "OK";
                        factura.fechaTimbrado = comprobanteTimbrado.Complemento.TimbreFiscalDigital.FechaTimbrado;
                        factura.UUID = comprobanteTimbrado.Complemento.TimbreFiscalDigital.UUID;

                        Task.Factory.StartNew(() =>
                        {
                            if (!string.IsNullOrEmpty(items["correoCliente"].ToString()))
                                Email.NotificacionPagoReferencia(items["correoCliente"].ToString(), pathServer + "Timbre_" + comprobante.Folio + timeStamp + ".xml", factura, string.Empty);
                        });


                    }
                    else
                    {
                        factura.estatusFactura = EnumEstatusFactura.Error;
                        factura.mensajeError = respuesta.codigoResultado + " |" + respuesta.codigoDescripcion;
                        System.IO.File.WriteAllText(pathServer + ("Comprobante_" + comprobante.Folio + timeStamp + ".xml"), xmlSerealizado);
                        Utilerias.ManagerSerealization<respuestaTimbrado>.Serealizar(respuesta, pathServer + ("respuesta_" + comprobante.Folio));
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
                Factura f = new Factura();
                f.idVenta = idVenta.ToString();
                Notificacion<dynamic> notificacion = new FacturaDAO().ObtenerDetalleFactura(f);
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public Notificacion<dynamic> ReenviarFactura(Factura f)
        {
            try
            {
                Notificacion<dynamic> notificacion = null;
                notificacion = f.idPedidoEspecial == 0 ? new FacturaDAO().ObtenerDetalleFactura(f) : new FacturaDAO().ObtenerDetalleFacturaPE(f);
                string path = System.Web.HttpContext.Current.Server.MapPath("~" + WebConfigurationManager.AppSettings["pathFacturas"].ToString());
                path = (path + notificacion.Modelo.pathArchivoFactura.Replace("Facturas/", "").Replace("Factura_", "Timbre_").Replace("pdf", "xml"));
                Email.NotificacionPagoReferencia(notificacion.Modelo.correo, path, f, f.correoAdicional);
                notificacion.Mensaje = "Factura reenviada exitosamente";
                return notificacion;


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string ObtenerComprobanteCFDI4(Factura factura)
        {
            Dictionary<string, object> items = null;
            FacturaDAO facturacionDAO = new FacturaDAO();
            Comprobante comprobante = facturacionDAO.ObtenerConfiguracionComprobante();
            comprobante.Folio = factura.folio = (factura.idVenta.Equals("0") || string.IsNullOrEmpty(factura.idVenta) ? "PE" + factura.idPedidoEspecial : factura.idVenta);
            items = facturacionDAO.ObtenerComprobante(factura, comprobante);
            if (items["estatus"].ToString().Equals("200"))
            {
                comprobante = (items["comprobante"] as Comprobante);
                return Utils.ExpresionImpresa(comprobante);
            }

            return "";
        }
    }


}
