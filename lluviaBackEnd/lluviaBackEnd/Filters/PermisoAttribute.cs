using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace lluviaBackEnd.Filters
{
    public class PermisoAttribute :ActionFilterAttribute
    {
        public EnumRolesPermisos Permiso { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            HttpContext context = HttpContext.Current;
            Sesion sesion = (Sesion)context.Session["UsuarioActual"];

            if (Models.Sesion.ExisteInventarioFisicoActivo() && this.Permiso!=EnumRolesPermisos.Puede_visualizar_InventarioFisico)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "InventarioFisicoActivo"
                }));
            }

            //if (!Models.Sesion.TienePermiso(this.Permiso))
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            //    {
            //        controller = "Login",
            //        action = "SinPermisos"
            //    }));
            //}
        }
    }
}