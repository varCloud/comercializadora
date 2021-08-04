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
using System.Configuration;
using System.Web.Configuration;
using System.Net.NetworkInformation;

namespace lluviaBackEnd.Controllers
{
    public class LoginController : Controller
    {
        public LoginDAO loginDAO;



        // GET: Login
        public ActionResult Login()
        {
            try
            {
                string version = string.IsNullOrEmpty(ConfigurationManager.AppSettings["web-app-version"].ToString()) ? "0.0.0" : ConfigurationManager.AppSettings["web-app-version"].ToString();
                ViewBag.appVersion = version;
                return View(new Sesion());
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        [HttpPost]
        public ActionResult ValidarUsuario(Sesion sesion)
        {
            try
            {
                string controladorAccion = "Login/Login";


                if (!ReCaptchaPassed(sesion.Token))
                {
                    ModelState.AddModelError(string.Empty, "Por favor comunicate con el Administrador");
                    return Json(new Notificacion<object>() { Estatus = -1, Mensaje = "Error al validar el captcha" }, JsonRequestBehavior.AllowGet);
                }

                sesion.macAdress = (
                                        from nic in NetworkInterface.GetAllNetworkInterfaces()
                                        where nic.OperationalStatus == OperationalStatus.Up
                                        select nic.GetPhysicalAddress().ToString()
                                    ).FirstOrDefault();


                Notificacion<Sesion> n = new LoginDAO().ValidaUsuario(sesion);
                if (n.Modelo.usuarioValido)
                {
                    Session["UsuarioActual"] = n.Modelo;
                    n.Modelo.configurado = WebConfigurationManager.AppSettings["configurado"].ToString();

                    if (Sesion.ExisteInventarioFisicoActivo() && n.Modelo.idRol != (int)EnumRoles.Administrador)
                    {
                        controladorAccion = "Login/InventarioFisicoActivo/";
                    }
                    else
                    {
                        if (n.Modelo.idRol == (int)EnumRoles.Administrador)
                        {
                            controladorAccion = "Dashboard/Index/";
                        }
                        else if (n.Modelo.idRol == (int)EnumRoles.Cajero)
                        {
                            controladorAccion = "Ventas/Ventas/";
                        }
                        else if (n.Modelo.idRol == (int)EnumRoles.Encargado_de_almacen)
                        {
                            controladorAccion = "Bitacora/Bitacoras/";
                        }
                        
                    }                  

                }
                return Json(new { notificacion = n, controladorAccion = controladorAccion }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
                //return View("Error", new HandleErrorInfo(ex, " Login", "ValidarUsuario"));
            }

        }

        public ActionResult EstacionesDisponibles()
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;

                Notificacion<List<Estacion>> lst = new lluviaBackEnd.DAO.EstacionesDAO().ObtenerEstaciones(new lluviaBackEnd.Models.Estacion() { idEstacion = 0, idAlmacen = usuarioSesion.idAlmacen });
                ViewBag.Notificacion = lst;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }


        [HttpPost]
        public ActionResult SeleccionarEstacion(Estacion estacion)
        {
            try
            {
                Notificacion<Estacion> n = new Notificacion<Estacion>();

                estacion.macAdress = (
                                            from nic in NetworkInterface.GetAllNetworkInterfaces()
                                            where nic.OperationalStatus == OperationalStatus.Up
                                            select nic.GetPhysicalAddress().ToString()
                                        ).FirstOrDefault();

                estacion.configurado = true;

                n = new EstacionesDAO().GuardarEstacion(estacion);

                if (n.Estatus == 200)
                {

                    string key = "configurado";
                    string value = "1";

                    Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                    string valor = config.AppSettings.Settings[key].Value;

                    if (config.AppSettings.Settings[key] == null)
                    {
                        config.AppSettings.Settings.Add(key, value);
                    }
                    else
                    {
                        config.AppSettings.Settings[key].Value = value;
                    }
                    config.Save();
                    ConfigurationManager.RefreshSection("appSettings");
                }

                return Json(n, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
                //return View("Error", new HandleErrorInfo(ex, " Login", "ValidarUsuario"));
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

        public ActionResult SinPermisos()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult InventarioFisicoActivo()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}