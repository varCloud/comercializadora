using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Web.Configuration;
using lluviaBackEnd.Filters;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using lluviaBackEnd.Utilerias;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class PedidosEspecialesV2Controller : Controller
    {

        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_PedidosEspeciales)]
        public ActionResult PedidosEspeciales(PedidosEspeciales pedidoEspecial)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;

                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



    }

}
