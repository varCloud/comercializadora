using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers.Configuracion
{
    // Modulo "Configuracion" del WMS (solo Administrador). Cada configuracion es una seccion: un partial en
    // Views/Configuracion + una accion POST aqui. Para agregar otra: crear el partial, renderizarlo en Index y su accion.
    [SessionTimeout]
    public class ConfiguracionController : Controller
    {
        private const string MensajeSinPermiso = "No tiene permiso para acceder a la configuracion";
        private static readonly Regex NipValido = new Regex("^[0-9]{4}$", RegexOptions.Compiled);

        // [PermisoAttribute] no bloquea el acceso por URL, por eso cada accion valida el rol contra la sesion.
        // Devuelve null si el usuario no es Administrador.
        private Sesion ObtenerSesionAdministrador()
        {
            Sesion sesion = Session["UsuarioActual"] as Sesion;
            if (sesion == null || sesion.idRol != (int)EnumRoles.Administrador)
            {
                return null;
            }
            return sesion;
        }

        private JsonResult RespuestaSinPermiso()
        {
            return Json(new Notificacion<string>() { Estatus = -1, Mensaje = MensajeSinPermiso, Modelo = "" }, JsonRequestBehavior.AllowGet);
        }

        // GET: Configuracion
        public ActionResult Index()
        {
            if (ObtenerSesionAdministrador() == null)
            {
                return RedirectToAction("SinPermisos", "Login");
            }
            return View();
        }

        // Seccion "NIP de formula de produccion". El NIP nunca se registra ni se devuelve.
        [HttpPost]
        public JsonResult CambiarNipFormula(string nipNuevo, string confirmacionNip)
        {
            Sesion sesion = ObtenerSesionAdministrador();
            if (sesion == null)
            {
                return RespuestaSinPermiso();
            }
            try
            {
                if (nipNuevo == null || !NipValido.IsMatch(nipNuevo))
                {
                    return Json(new Notificacion<string>() { Estatus = -1, Mensaje = "El NIP debe tener exactamente 4 digitos numericos", Modelo = "" }, JsonRequestBehavior.AllowGet);
                }
                if (!string.Equals(nipNuevo, confirmacionNip, StringComparison.Ordinal))
                {
                    return Json(new Notificacion<string>() { Estatus = -1, Mensaje = "La confirmacion del NIP no coincide", Modelo = "" }, JsonRequestBehavior.AllowGet);
                }
                Notificacion<string> notificacion = new ProcesoProduccionFormulaDAO().CambiarNipFormula(sesion.idUsuario, nipNuevo);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                // Sin WsUtils.RegresaExcepcion: no se registra la solicitud (contiene el NIP).
                return Json(new Notificacion<string>() { Estatus = -1, Mensaje = "Ocurrio un error al cambiar el NIP", Modelo = "" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
