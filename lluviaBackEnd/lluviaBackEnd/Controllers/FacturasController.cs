using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.Controllers
{
    public class FacturasController : Controller
    {
        // GET: FacturasController

        public ActionResult Facturas()
        {
            //ViewBag.lstFacturas = new FacturaDAO().ObtenerFacturas(new Models.Factura() { idFactura = 0 });
            return View();
        }




        //    public ActionResult ObtenerFactura(int idFactura)
        //    {
        //        try
        //        {
        //            Models.Factura p = new FacturaDAO().ObtenerFacturas(new Models.Factura() { idFactura = idFactura })[0];
        //            return Json(p, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //    public ActionResult _ObtenerFacturas(int idFactura)
        //    {
        //        try
        //        {
        //            //List<Models.Socio> Lstsocio = new SocioDAO().ObtenerSocios(new Models.Socio() { IdSocio = 0 });
        //            ViewBag.lstFacturas = new FacturaDAO().ObtenerFacturas(new Models.Factura() { idFactura = 0 });
        //            return PartialView("_ObtenerFacturas");
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }


        //    [HttpPost]
        //    public ActionResult GuardarFactura(Factura p)
        //    {
        //        try
        //        {

        //            Result result = new Result();
        //            result = new FacturaDAO().GuardarFactura(p);
        //            return Json(result, JsonRequestBehavior.AllowGet);

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //    [HttpPost]
        //    public ActionResult ActualizarEstatusFactura(int idFactura, bool activo)
        //    {
        //        try
        //        {
        //            Result result = new FacturaDAO().ActualizarEstatusFactura(idFactura, activo);
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

    }


}
