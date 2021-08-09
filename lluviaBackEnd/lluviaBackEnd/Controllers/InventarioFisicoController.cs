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
                InventarioFisico inventarioFisico = new InventarioFisico();
                inventarioFisico.TipoInventario = EnumTipoInventarioFisico.General;
                return View(inventarioFisico);
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

        public ActionResult _ObtenerInventarioFisico(InventarioFisico inventarioFisico)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                inventarioFisico.Sucursal.idSucursal = usuarioSesion.idSucursal;
                return PartialView(new InventarioFisicoDAO().ObtenerInventarioFisico(inventarioFisico));
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
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos(usuarioSesion.idUsuario).Where(x => x.Value != "").ToList();
                ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                inventarioFisico.Sucursal.idSucursal = usuarioSesion.idSucursal;
                ViewBag.listInventarioFisico = new SelectList(new InventarioFisicoDAO().ObtenerInventarioFisico(inventarioFisico), "idInventarioFisico", "Nombre").ToList();
                inventarioFisico.Sucursal.idSucursal = 0;
                inventarioFisico = new InventarioFisicoDAO().ObtenerInventarioFisico(inventarioFisico).First();
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