using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers.ProcesoProduccionFormula
{
    [SessionTimeout]
    public class ProcesoProduccionFormulaController : Controller
    {
        private const string MensajeSinPermiso = "No tiene permiso para administrar las formulas de produccion";

        // [PermisoAttribute] no bloquea el acceso por URL (su chequeo esta comentado), por eso cada accion
        // valida aqui el permiso Puede_visualizar_Relacion_Produccion contra la sesion. Devuelve null si no lo tiene.
        private Sesion ObtenerSesionConPermiso()
        {
            Sesion sesion = Session["UsuarioActual"] as Sesion;
            if (sesion == null || sesion.permisosModulo == null)
            {
                return null;
            }
            bool tienePermiso = sesion.permisosModulo.Exists(p =>
                p.idModulo == (int)EnumRolesPermisos.Puede_visualizar_Relacion_Produccion && p.tienePermiso);
            return tienePermiso ? sesion : null;
        }

        private JsonResult RespuestaSinPermiso()
        {
            return Json(new Notificacion<string>() { Estatus = -1, Mensaje = MensajeSinPermiso, Modelo = "" }, JsonRequestBehavior.AllowGet);
        }

        // GET: ProcesoProduccionFormula
        public ActionResult ProcesoProduccionFormula()
        {
            if (ObtenerSesionConPermiso() == null)
            {
                return RedirectToAction("SinPermisos", "Login");
            }
            return View(new ProcesoProduccionFormulaGuardarModel());
        }

        // GET: ProcesoProduccionFormula/ProcesoProduccionHistorial  (pantalla "Produccion": historial de producciones)
        public ActionResult ProcesoProduccionHistorial()
        {
            if (ObtenerSesionConPermiso() == null)
            {
                return RedirectToAction("SinPermisos", "Login");
            }

            List<SelectListItem> productos = new List<SelectListItem> { new SelectListItem { Text = "Todos", Value = "" } };
            List<SelectListItem> almacenes = new List<SelectListItem> { new SelectListItem { Text = "Todos", Value = "" } };
            try
            {
                Notificacion<List<ProcesoProduccionConsultaProductoModel>> catalogo = new ProcesoProduccionFormulaDAO().ObtenerProductosHistorial();
                if (catalogo.Estatus == 200)
                {
                    productos.AddRange(catalogo.Modelo.Select(p => new SelectListItem { Text = p.descripcion, Value = p.idProducto.ToString() }));
                }
                almacenes.AddRange(new UsuarioDAO().ObtenerAlmacenes(0, 0));
            }
            catch (Exception)
            {
                // Los catalogos son solo filtros opcionales; la pantalla abre con "Todos" y la consulta reporta sus propios errores.
            }
            ViewBag.Productos = productos;
            ViewBag.Almacenes = almacenes;
            return View(new ProcesoProduccionConsultaFiltroModel());
        }

        // Tabla del historial (HTML parcial). Sin permiso responde 403 para que el ajax caiga en su handler de error.
        [HttpPost]
        public ActionResult _ObtenerProcesoProduccionHistorial(ProcesoProduccionConsultaFiltroModel filtro)
        {
            if (ObtenerSesionConPermiso() == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, MensajeSinPermiso);
            }
            try
            {
                Notificacion<ProcesoProduccionConsultaResultadoModel> notificacion = new ProcesoProduccionFormulaDAO().ObtenerHistorial(filtro);
                return PartialView("_ObtenerProcesoProduccionHistorial", notificacion);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        // Tabla de formulas (HTML parcial). Sin permiso responde 403 para que el ajax caiga en su handler de error.
        [HttpPost]
        public ActionResult _ObtenerFormulas()
        {
            if (ObtenerSesionConPermiso() == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, MensajeSinPermiso);
            }
            try
            {
                Notificacion<List<ProcesoProduccionFormulaModel>> notificacion = new ProcesoProduccionFormulaDAO().ObtenerFormulas();
                return PartialView("_ObtenerFormulas", notificacion);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        // Catalogos del formulario; con id > 0 incluye tambien el detalle de la formula (editar / ver).
        [HttpPost]
        public JsonResult ObtenerDatosFormulario(long idProcesoProduccionFormula)
        {
            if (ObtenerSesionConPermiso() == null)
            {
                return RespuestaSinPermiso();
            }
            try
            {
                Notificacion<ProcesoProduccionFormulaCatalogosModel> notificacion = new ProcesoProduccionFormulaDAO().ObtenerCatalogos(idProcesoProduccionFormula);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GuardarFormula(ProcesoProduccionFormulaGuardarModel request)
        {
            Sesion sesion = ObtenerSesionConPermiso();
            if (sesion == null)
            {
                return RespuestaSinPermiso();
            }
            try
            {
                if (request == null)
                {
                    return Json(new Notificacion<string>() { Estatus = -1, Mensaje = "Solicitud invalida", Modelo = "" }, JsonRequestBehavior.AllowGet);
                }
                Notificacion<string> notificacion = new ProcesoProduccionFormulaDAO().GuardarFormula(request, sesion.idUsuario);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DesactivarFormula(long idProcesoProduccionFormula)
        {
            Sesion sesion = ObtenerSesionConPermiso();
            if (sesion == null)
            {
                return RespuestaSinPermiso();
            }
            try
            {
                Notificacion<string> notificacion = new ProcesoProduccionFormulaDAO().DesactivarFormula(idProcesoProduccionFormula, sesion.idUsuario);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
