using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class InventarioFisicoController : Controller
    {
        // GET: InventarioFisico
        public ActionResult InventarioFisico()
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

        [HttpPost]
        public ActionResult GuardarInventarioFisico(InventarioFisico inventarioFisico)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                inventarioFisico.Usuario.idUsuario = usuarioSesion.idUsuario;
                Notificacion<string> result = new InventarioFisicoDAO().InsertaInventarioFisico(inventarioFisico);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        public ActionResult _ObtenerInventarioFisico(InventarioFisico inventarioFisico)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                return PartialView(new InventarioFisicoDAO().ObtenerInventarioFisico(usuarioSesion.idSucursal));
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}