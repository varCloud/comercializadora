using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace lluviaBackEnd.Filters
{
    public class SessionTimeout : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            /************************************************************************
             SI LA SESION ES DIFERENTE DE NULL PASA A EJECUTAR LA SIGUIENTE ACCION
             SI LA SESION ES IGUAL A NULL ENTRA A LA SIGUIENTE FUNCION LO CUAL LO REDIRIGE
             AL LOGIN
            ****************************************************************************/
            return httpContext.Session["UsuarioActual"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary()
            {
                { "action", "SesionExpirada" },
                { "controller", "SesionExpirada" }
            });
        }
    }

}