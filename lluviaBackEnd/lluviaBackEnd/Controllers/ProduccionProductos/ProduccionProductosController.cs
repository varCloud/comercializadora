using lluviaBackEnd.DAO.ProduccionProductos;
using lluviaBackEnd.DAO.ProductosAgranelAEnvasar;
using lluviaBackEnd.Models;
using lluviaBackEnd.Models.ProduccionProductos;
using lluviaBackEnd.WebServices.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers.ProduccionProductos
{
    public class ProduccionProductosController : Controller
    {
        // GET: ProductosAgranelAEnvasar
        public ActionResult ProduccionProductos()
        {
            ViewBag.data = new ProduccionProductosDAO().ObtenerProductos().Modelo;
            ViewBag.combinaciones = (new ProduccionProductosDAO().ObtenerCombinaciones()).Modelo;
            return View(new ProduccionProductosModel());
        }

        public ActionResult _ObtenerCombinacionProduccionProductos()
        {
            try
            {
                Notificacion<List<ProduccionProductosModel>> notificacion = new ProduccionProductosDAO().ObtenerCombinaciones();
                return this.PartialView("_ObtenerCombinacionProduccionProductos", notificacion.Modelo);
            }
            catch (Exception exception)
            {
                return base.Json(WsUtils<string>.RegresaExcepcion(exception, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DesactivarCombinacionProduccionProductos(int idProductoProduccion)
        {
            try
            {
                Notificacion<string> data = new ProduccionProductosDAO().DesactivarCombinacionProductosProduccionProductos(idProductoProduccion);
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return base.Json(WsUtils<string>.RegresaExcepcion(exception, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GuardarRelacionProductos(ProduccionProductosModel request)
        {
            try
            {
                Notificacion<string> data = new ProduccionProductosDAO().AgregarCombinacionesProduccionProductos(request);
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return base.Json(WsUtils<string>.RegresaExcepcion(exception, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerCombinacionProduccionProductos()
        {
            try
            {
                Notificacion<List<ProduccionProductosModel>> notificacion = new ProduccionProductosDAO().ObtenerCombinaciones();
                return base.Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return base.Json(WsUtils<string>.RegresaExcepcion(exception, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerProductos()
        {
            try
            {
                Notificacion<Dictionary<string, object>> notificacion = new ProduccionProductosDAO().ObtenerProductos();
                return base.Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return base.Json(WsUtils<string>.RegresaExcepcion(exception, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }
    }
}