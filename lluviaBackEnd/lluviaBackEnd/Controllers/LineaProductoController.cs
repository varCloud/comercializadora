using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class LineaProductoController : Controller
    {
        // GET: LineaProducto
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_Productos)]
        public ActionResult LineaProducto()
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                ViewBag.lstLineaProducto = new LineaProductoDAO().ObtenerLineaProductos(new Models.LineaProducto() { idLineaProducto = 0 },usuarioSesion.idUsuario);
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }




        public ActionResult ObtenerLineaProducto(int idLineaProducto)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                Models.LineaProducto p = new LineaProductoDAO().ObtenerLineaProductos(new Models.LineaProducto() { idLineaProducto = idLineaProducto },usuarioSesion.idUsuario)[0];
                return Json(p, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _ObtenerLineaProducto(int idLineaProducto)
        {
            try
            {
                //List<Models.Socio> Lstsocio = new SocioDAO().ObtenerSocios(new Models.Socio() { IdSocio = 0 });
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                ViewBag.lstLineaProducto = new LineaProductoDAO().ObtenerLineaProductos(new Models.LineaProducto() { idLineaProducto = 0 },usuarioSesion.idUsuario);
                return PartialView("_ObtenerLineaProducto");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult GuardarLineaProducto(LineaProducto p)
        {
            try
            {

                Result result = new Result();
                result = new LineaProductoDAO().GuardarLineaProducto(p);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ActualizarEstatusLineaProducto(int idLineaProducto, bool activo)
        {
            try
            {
                Result result = new LineaProductoDAO().ActualizarEstatusLineaProducto(idLineaProducto, activo);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult LineasAlmacen(int idAlmacen)
        {
            try
            {
                return Json(new LineaProductoDAO().ObtenerLineasAlmacen(idAlmacen), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }


}
