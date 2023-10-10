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

        [HttpGet]
        public JsonResult ObtenerProductos()
        {
            try
            {
                Notificacion<Dictionary<string, object>> notificacion = new ProductosAgranelAEnvasarDAO().ObtenerProductos();
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult ObtenerCombinacionProductos()
        {
            try
            {
                Notificacion<List<ProductosAgranelAEnvasarModel>> notificacion = new ProductosAgranelAEnvasarDAO().ObtenerCombinaciones();
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult _ObtenerCombinacionProductos()
        {
            try
            {
                Notificacion<List<ProductosAgranelAEnvasarModel>> notificacion = new ProductosAgranelAEnvasarDAO().ObtenerCombinaciones();
                return PartialView("_ObtenerCombinacionProductos", notificacion.Modelo);
            }
            catch (Exception ex)
            {

                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GuardarRelacionProductos(ProductosAgranelAEnvasarModel request)
        {
            try
            {
                Notificacion<string> notificacion = new ProductosAgranelAEnvasarDAO().AgregarCombinacionesMPL(request);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult DesactivarCombinacionProductosEnvasadosAgranel(int idRelacionEnvasadoAgranel)
        {
            try
            {
                Notificacion<string> notificacion = new ProductosAgranelAEnvasarDAO().DesactivarCombinacionProductosEnvasadosAgranel(idRelacionEnvasadoAgranel);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }

        }
    }
}