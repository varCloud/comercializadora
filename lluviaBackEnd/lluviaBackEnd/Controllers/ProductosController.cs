using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;


namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class ProductosController : Controller
    {
        // GET: Productos

        public ActionResult Productos()
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstClaveProdServ = new LineaProductoDAO().ObtenerClavesProductos();
            ViewBag.lstClavesUnidad = new LineaProductoDAO().ObtenerClavesUnidad();
            ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();

            ViewBag.lstProductos = notificacion.Modelo;
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerCodigos(string cadena)
        {
            try
            {
                Dictionary<string, object> codigos = new Dictionary<string, object>();
                codigos.Add("barra",Convert.ToBase64String(Utilerias.Utils.GenerarCodigoBarras(cadena)));
                codigos.Add("qr", Convert.ToBase64String(Utilerias.Utils.GenerarQR(cadena)));
                return Json(codigos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ObtenerProductos(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);
                Producto p = new Producto();
                p = notificacion.Modelo[0];
                return Json(p, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _ObtenerProductos(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);
                ViewBag.lstProductos = notificacion.Modelo;
                return PartialView("_ObtenerProductos");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BuscarProductos(Producto producto)
        {
            try
            {

                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_ObtenerProductos");
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
        public ActionResult GuardarProductos(Producto producto)
        {
            try
            {

                Notificacion<Producto> result = new Notificacion<Producto>();
                result = new ProductosDAO().GuardarProducto(producto);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ActualizarEstatusProducto(Producto producto)
        {
            try
            {
                Notificacion<Producto> notificacion = new Notificacion<Producto>();
                notificacion = new ProductosDAO().ActualizarEstatusProducto(producto);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public ActionResult GuardarPrecios(List<Precio> precios, Producto producto)
        {
            try
            {
                Notificacion<Precio> result = new Notificacion<Precio>();
                result = new ProductosDAO().GuardarPrecios(precios, producto);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult ObtenerPrecios(Precio precio)
        {
            try
            {
                Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
                notificacion = new ProductosDAO().ObtenerPrecios(precio);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


}
