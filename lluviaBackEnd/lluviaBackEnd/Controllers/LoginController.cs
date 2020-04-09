using lluviaBackEnd.Models;
using lluviaBackEnd.DAO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.Models.Facturacion;
using lluviaBackEnd.Utilerias;
using lluviaBackEnd.servicioTimbrarPruebas;

namespace lluviaBackEnd.Controllers
{
    public class LoginController : Controller
    {
        public LoginDAO loginDAO;



        // GET: Login
        public ActionResult Login()
        {
            return View(new Sesion());
        }

        [HttpPost]
        public ActionResult ValidarUsuario(Sesion sesion)
        {
            try
            {
                if (!ReCaptchaPassed(sesion.Token))
                {
                    ModelState.AddModelError(string.Empty, "Por favor comunicate con el Administrador");
                    return Json( new Notificacion<object>() {Estatus = -1 , Mensaje = "Error al validar el captcha" }, JsonRequestBehavior.AllowGet);
                }
                Notificacion<Sesion> n = new LoginDAO().ValidaUsuario(sesion);
                if (n.Modelo.usuarioValido)
                {
                    Session["UsuarioActual"] = n.Modelo;
                }
                return Json(n, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, " Login", "ValidarUsuario"));
            }

        }

        [HttpPost]
        public ActionResult Facturar()
        {
            try
            {
                string pathFactura  = Utils.ObtnerFolder()+'/';
                int idVenta = 64;
                FacturacionDAO facturacionDAO = new FacturacionDAO();
                Comprobante comprobante =  facturacionDAO.ObtenerConfiguracionComprobante();
                comprobante.Emisor.Rfc= "CMV980925LQ7";
                comprobante.Emisor.Nombre = "CAJA MORELIA VALLADOLID S.C. DE A.P. DE R.L. DE C.V.";
                comprobante.Emisor.RegimenFiscal = 603;
                

                comprobante = facturacionDAO.ObtenerComprobante(idVenta, comprobante);

                facturacionDAO.ObtenerImpuestosGenerales(ref comprobante);

                facturacionDAO.ObtenerTotal(ref comprobante);

                Dictionary<string, string> certificados = Utilerias.ProcesaCfdi.ObtenerCertificado();
                if(certificados == null)
                    return Json("Error al obtener los certificados", JsonRequestBehavior.AllowGet);
                comprobante.Certificado = certificados["Certificado"];
                comprobante.NoCertificado = certificados["NoCertificado"];
                string xmlSerealizado = Utilerias.ProcesaCfdi.SerializaXML33(comprobante);
                string cadenaOriginal = Utilerias.ProcesaCfdi.GeneraCadenaOriginal33(xmlSerealizado);
                comprobante.Sello = Utilerias.ProcesaCfdi.GeneraSello(cadenaOriginal);
                servicioTimbrarPruebas.timbrarCFDIPortTypeClient timbrar = new servicioTimbrarPruebas.timbrarCFDIPortTypeClient();
                respuestaTimbrado respuesta =timbrar.timbrarCFDI("", "", ProcesaCfdi.Base64Encode(Utilerias.ProcesaCfdi.SerializaXML33(comprobante)));
                Utilerias.ManagerSerealization<Comprobante>.Serealizar(comprobante, (pathFactura +  ("Comprobante_"+idVenta)));
                if (respuesta.codigoResultado.Equals("100"))
                {
                    string xmlTimbradoDecodificado = ProcesaCfdi.Base64Decode(respuesta.documentoTimbrado);
                    System.IO.File.WriteAllText(pathFactura + "Timbre_" + idVenta + ".xml", xmlTimbradoDecodificado);
                    Comprobante comprobanteTimbrado = Utilerias.ManagerSerealization<Comprobante>.DeserializeToObject(xmlTimbradoDecodificado);
                    Utils.GenerarQRSAT(comprobanteTimbrado, pathFactura + ("Qr_" + idVenta));
                    Utils.GenerarFactura(comprobanteTimbrado, pathFactura + "Factura_" + (idVenta + ".pdf"));
                }
                else {
                    Utilerias.ManagerSerealization<respuestaTimbrado>.Serealizar(respuesta, pathFactura + ("respuesta_" + idVenta));
                }
                return Json("facturando",JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public bool ReCaptchaPassed(string gRecaptchaResponse)
        {
            HttpClient httpClient = new HttpClient();
            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret=6LfandgUAAAAACyyGmdqpahF8xXL1haLavqH6X2i&response={gRecaptchaResponse}").Result;
            if (res.StatusCode != HttpStatusCode.OK)
                return false;

            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
                return false;

            return true;
        }
    }
}