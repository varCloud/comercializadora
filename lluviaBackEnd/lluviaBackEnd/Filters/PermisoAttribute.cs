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

            if (!Models.Sesion.ExisteInventarioFisicoActivo())
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