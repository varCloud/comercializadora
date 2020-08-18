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
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_InventarioFisico)]
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

        [HttpPost]
        public ActionResult ActualizaEstatusInventarioFisico(InventarioFisico inventarioFisico)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                inventarioFisico.Usuario.idUsuario = usuarioSesion.idUsuario;
                Notificacion<string> result = new InventarioFisicoDAO().ActualizaEstatusInventarioFisico(inventarioFisico);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult _ObtenerInventarioFisico()
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                return PartialView(new InventarioFisicoDAO().ObtenerInventarioFisico(usuarioSesion.idSucursal, 0, 0));
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult _InventarioFisico(InventarioFisico inventarioFisico)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos().Where(x => x.Value != "").ToList();
                ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                ViewBag.listInventarioFisico = new SelectList(new InventarioFisicoDAO().ObtenerInventarioFisico(usuarioSesion.idSucursal, inventarioFisico.idInventarioFisico, 0), "idInventarioFisico", "Nombre").ToList();

                inventarioFisico = new InventarioFisicoDAO().ObtenerInventarioFisico(0, inventarioFisico.idInventarioFisico, 0).First();
                return PartialView(inventarioFisico);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public ActionResult _ObtenerAjusteInventario(AjusteInventarioFisico ajusteInventario)
        {
            try
            {
                return PartialView(new InventarioFisicoDAO().ObtenerAjusteInventario(ajusteInventario));
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}