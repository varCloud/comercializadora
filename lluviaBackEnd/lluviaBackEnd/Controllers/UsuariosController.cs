using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        public ActionResult Usuarios()
        {
            //return View();
            List<Models.Usuario> LstUsuario = new UsuarioDAO().ObtenerUsuarios(new Models.Usuario() { idRol = 1 });
            ViewBag.lstSocio = LstUsuario;
            return View(new Usuario());
        }
    }
}