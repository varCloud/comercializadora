using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View(new Sesion());
        }

       [HttpPost]
        public ActionResult ValidarUsuario(Sesion sesion)
        {

            return View("Login",sesion); 
        }
    }
}