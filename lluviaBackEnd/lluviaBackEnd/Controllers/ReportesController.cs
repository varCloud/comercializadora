using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.Controllers
{
    public class ReportesController : Controller
    {
        //REPORTE INVENTARIO

        public ActionResult Inventario()
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.lstProductos = notificacion.Modelo;
            return View();
        }

        //public ActionResult ObtenerInventario(Producto producto)
        //{
        //    try
        //    {
        //        Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
        //        notificacion = new ProductosDAO().ObtenerProductos(producto);
        //        Producto p = new Producto();
        //        p = notificacion.Modelo[0];
        //        return Json(p, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult _ObtenerInventario(Producto producto)
        //{
        //    try
        //    {
        //        Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
        //        notificacion = new ProductosDAO().ObtenerProductos(producto);
        //        ViewBag.lstProductos = notificacion.Modelo;
        //        return PartialView("_ObtenerProductos");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public ActionResult BuscarInventario(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_Inventario");
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


        //REPORTE COMPRAS

        public ActionResult Compras()
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.lstProductos = notificacion.Modelo;
            return View();
        }

        public ActionResult ObtenerCompras(Producto producto)
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

        public ActionResult _ObtenerCompras(Producto producto)
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

        public ActionResult BuscarCompras(Producto producto)
        {
            try
            {

                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_Compras");
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


        //REPORTE VENTAS

        public ActionResult Ventas()
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.lstProductos = notificacion.Modelo;
            return View();
        }

        public ActionResult ObtenerVentas(Producto producto)
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

        public ActionResult _ObtenerVentas(Producto producto)
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

        public ActionResult BuscarVentas(Producto producto)
        {
            try
            {

                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_Ventas");
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


    }


}
