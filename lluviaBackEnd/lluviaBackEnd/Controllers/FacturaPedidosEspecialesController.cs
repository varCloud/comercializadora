using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.Utilerias;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    public class FacturaPedidosEspecialesController : Controller
    {
        // GET: FacturaPedidosEspeciales
        public ActionResult FacturaPedidosEspeciales()
        {
            List<SelectListItem> listUsuarios;
            Sesion usuario = Session["UsuarioActual"] as Sesion;
            if (usuario.idRol == 1)
            {
                listUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
            }
            else
                listUsuarios = new UsuarioDAO().ObtenerUsuarios(usuario.idUsuario).Where(x => x.Value != "0").ToList();
            ViewBag.listUsuarios = listUsuarios;
            return View();
        }

        public ActionResult _ObtenerFacturasPE(Factura factura)
        {
            try
            {
                Notificacion<List<Factura>> notificacion = new FacturaDAO().ObtenerFacturasPedidosEspeciales(factura);
                return PartialView(notificacion);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult ObtenerDetalleFacturaPE(Factura f)
        {
            try
            {
                Notificacion<dynamic> notificacion = new FacturaDAO().ObtenerDetalleFacturaPE(f);
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



    }
}