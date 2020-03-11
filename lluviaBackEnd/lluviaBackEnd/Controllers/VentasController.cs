using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.Controllers
{
    public class VentasController : Controller
    {
        // GET: Ventas

        public ActionResult Ventas()
        {
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
            notificacion = new VentasDAO().ObtenerVentas(new Models.Ventas() { idVenta = 0 });
            //ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            //ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.lstVentas = notificacion.Modelo;
            return View();
        }



        public ActionResult ObtenerVentas(Ventas ventas)
        {
            try
            {
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                notificacion = new VentasDAO().ObtenerVentas(ventas);
                Ventas p = new Ventas();
                p = notificacion.Modelo[0];
                return Json(p, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _ObtenerVentas(Ventas ventas)
        {
            try
            {
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                notificacion = new VentasDAO().ObtenerVentas(ventas);
                ViewBag.lstCompras = notificacion.Modelo;
                return PartialView("_ObtenerVentas");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BuscarVentas(Ventas ventas)
        {
            try
            {
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                notificacion = new VentasDAO().ObtenerVentas(ventas);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstVentas = notificacion.Modelo;
                    return PartialView("_BuscarVentas");
                }
                else
                {
                    ViewBag.titulo = "Mensaje: ";
                    ViewBag.mensaje = notificacion.Mensaje;
                    return PartialView("_SinResultados");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarVentas(List<Ventas> ventas)
        {
            try
            {
                Notificacion<Ventas> result = new Notificacion<Ventas>();
                //result = new VentasDAO().GuardarVentas(ventas);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ActualizarEstatusVentas(Ventas ventas)
        {
            try
            {
                Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
                notificacion = new VentasDAO().ActualizarEstatusVentas(ventas);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }


}
