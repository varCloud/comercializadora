using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace lluviaBackEnd.Filters
{
    public class PermisoAttribute 
    {
        public EnumRolesPermisos Permiso { get; set; }

       /* public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (!Models.Sesion.TienePermiso(this.Permiso))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "SinPermisos"
                }));
            }
        }*/
    }
}