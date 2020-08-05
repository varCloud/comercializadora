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
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_Usuarios)]
        public ActionResult Usuarios()
        {
            //return View();
            //List<Models.Usuario> LstUsuario = new UsuarioDAO().ObtenerUsuarios(new Models.Usuario() { idRol = 1 });
            ViewBag.lstUsuario = new UsuarioDAO().ObtenerUsuarios(new Models.Usuario() { idUsuario = 0 });
            ViewBag.lstRoles = new UsuarioDAO().ObtenerRoles(new Models.Rol() { idRol = 1 });
            //ViewBag.lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes();
            ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
            
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerAlmacenSucursal(int idSucursal = 0, int idTipoAlmacen = 0)
        {
            try
            {

                return Json(new UsuarioDAO().ObtenerAlmacenes(idSucursal, idTipoAlmacen), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult ObtenerUsuario(int idUsuario)
        {
            try
            {
                Models.Usuario usr = new UsuarioDAO().ObtenerUsuarios(new Models.Usuario() { idUsuario = idUsuario })[0];
                return Json(usr, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _ObtenerUsuarios(int idUsuario)
        {
            try
            {
                //List<Models.Socio> Lstsocio = new SocioDAO().ObtenerSocios(new Models.Socio() { IdSocio = 0 });
                ViewBag.lstUsuario = new UsuarioDAO().ObtenerUsuarios(new Models.Usuario() { idUsuario = 0 });
               // ViewBag.lstRoles = new UsuarioDAO().ObtenerRoles(new Models.Rol() { idRol = 1 });
                //ViewBag.lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes();
                //ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
                return PartialView("_ObtenerUsuarios");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult GuardarUsuario(Usuario usr)
        {
            try
            {

                Result result = new Result();
                result = new UsuarioDAO().GuardarUsuario(usr);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ActualizarEstatusUsuario(int idUsuario, bool activo)
        {
            try
            {
               Result result = new UsuarioDAO().ActualizarEstatusUsuario(idUsuario, activo);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ObtenerUsuariosPorAlmacenyRol(Usuario usuario)
        {
            try
            {
                //Sesion usuario_ = (Sesion)Session["UsuarioActual"];
                //List<Usuario> lstUsuarios = new List<Usuario>();
                //lstUsuarios = new UsuarioDAO().ObtenerUsuarios(new Models.Usuario() { idUsuario = 0, idAlmacen = usuario_.idAlmacen, idRol =  });
                return Json(new UsuarioDAO().ObtenerUsuarios(usuario), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}