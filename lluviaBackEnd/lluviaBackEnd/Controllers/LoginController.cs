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
                    ModelState.AddModelError(string.Empty, "You failed the CAPTCHA.");
                }

                loginDAO = new LoginDAO();
                loginDAO.ValidaUsuario(sesion);

                if ( sesion.usuarioValido )
                {
                    //return RedirectToAction("index", "DashBoard");
                    //return Redirect("~/DashBoard/index");
                    return Json(sesion, JsonRequestBehavior.AllowGet);

                    //return View("Inicio");
                }

                //return View("Login", sesion);
            }
            catch (Exception ex)
            {
                throw /*new FaultException(ex.Message)*/;
            }
            return View("Login");        

        }


        public  bool ReCaptchaPassed(string gRecaptchaResponse)
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