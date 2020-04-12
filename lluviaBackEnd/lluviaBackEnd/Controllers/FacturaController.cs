using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.Models.Facturacion;
using lluviaBackEnd.servicioTimbrarPruebas;
using lluviaBackEnd.Utilerias;
using lluviaBackEnd.WebServices.Modelos;

namespace lluviaBackEnd.Controllers
{
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
        public JsonResult GenerarFactura(Factura factura)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                string pathFactura = Utils.ObtnerFolder() + '/';
                FacturaDAO facturacionDAO = new FacturaDAO();

                Comprobante comprobante = facturacionDAO.ObtenerConfiguracionComprobante();
                comprobante.Folio = factura.folio = factura.idVenta;
                comprobante.Emisor.Rfc = "CMV980925LQ7";
                comprobante.Emisor.Nombre = "CAJA MORELIA VALLADOLID S.C. DE A.P. DE R.L. DE C.V.";
                comprobante.Emisor.RegimenFiscal = 603;

                comprobante = facturacionDAO.ObtenerComprobante(factura.idVenta, comprobante);
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
                servicioTimbrarPruebas.timbrarCFDIPortTypeClient timbrar = new servicioTimbrarPruebas.timbrarCFDIPortTypeClient();
                respuestaTimbrado respuesta = timbrar.timbrarCFDI("", "", ProcesaCfdi.Base64Encode(Utilerias.ProcesaCfdi.SerializaXML33(comprobante)));

                if (respuesta.codigoResultado.Equals("100"))
                {

                    string xmlTimbradoDecodificado = ProcesaCfdi.Base64Decode(respuesta.documentoTimbrado);
                    System.IO.File.WriteAllText(pathFactura + "Timbre_" + factura.idVenta + ".xml", xmlTimbradoDecodificado);
                    Comprobante comprobanteTimbrado = Utilerias.ManagerSerealization<Comprobante>.DeserializeToObject(xmlTimbradoDecodificado);
                    Utils.GenerarQRSAT(comprobanteTimbrado, pathFactura + ("Qr_" + factura.idVenta + ".jpg"));
                    Utils.GenerarFactura(comprobanteTimbrado, pathFactura, factura.idVenta);
                    factura.estatusFactura = EnumEstatusFactura.Facturada;
                    factura.mensajeError = "OK";
                    factura.fechaTimbrado = comprobanteTimbrado.Complemento.TimbreFiscalDigital.FechaTimbrado;
                    factura.UUID = comprobanteTimbrado.Complemento.TimbreFiscalDigital.UUID;

                }
                else
                {
                    factura.estatusFactura = EnumEstatusFactura.Error;
                    factura.mensajeError = respuesta.codigoResultado + " |" + respuesta.codigoDescripcion;
                    Utilerias.ManagerSerealization<Comprobante>.Serealizar(comprobante, (pathFactura + ("Comprobante_" + factura.idVenta)));
                    Utilerias.ManagerSerealization<respuestaTimbrado>.Serealizar(respuesta, pathFactura + ("respuesta_" + factura.idVenta));
                }
                notificacion = new FacturaDAO().GuardarFactura(factura);
                return Json(notificacion, JsonRequestBehavior.AllowGet); ;
            }
            catch (Exception ex)
            {

                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error"), JsonRequestBehavior.AllowGet);
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
