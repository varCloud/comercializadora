using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class EstacionesController : Controller
    {


        public ActionResult Estaciones()
        {
            Notificacion<List<Estacion>> notificacion = new Notificacion<List<Estacion>>();
            notificacion = new EstacionesDAO().ObtenerEstaciones(new Estacion() { idEstacion = 0 });
            ViewBag.lstEstaciones = notificacion.Modelo;
            ViewBag.lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes();
            return View();
        }

        [HttpPost]
        public ActionResult _ObtenerEstaciones()
        {
            Notificacion<List<Estacion>> notificacion = new Notificacion<List<Estacion>>();
            notificacion = new EstacionesDAO().ObtenerEstaciones(new Estacion() { idEstacion = 0 });

            if ( notificacion.Modelo == null ) {
                ViewBag.titulo = "Mensaje: ";
                ViewBag.mensaje = notificacion.Mensaje;
                return PartialView("_SinResultados");
            }
            else {
                return PartialView("_ObtenerEstaciones", notificacion.Modelo);

            }

        }

        [HttpPost]
        public ActionResult ObtenerEstacion(Estacion estacion)
        {
            try
            {
                Notificacion<List<Estacion>> notificacion = new Notificacion<List<Estacion>>();
                notificacion = new EstacionesDAO().ObtenerEstaciones(new Estacion() { idEstacion = estacion.idEstacion });
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarEstacion(Estacion estacion)
        {
            try
            {
                Notificacion<Estacion> notificacion = new Notificacion<Estacion>();
                notificacion = new EstacionesDAO().GuardarEstacion(estacion);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult EliminarEstacion(Estacion estacion)
        {
            try
            {
                Notificacion<Estacion> notificacion = new Notificacion<Estacion>();
                notificacion = new EstacionesDAO().EliminarEstacion(estacion);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
     







    }
}