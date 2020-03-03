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
        //REPORTE PRODUCTOS

        public ActionResult ReporteProductos()
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.lstProductos = notificacion.Modelo;
            return View();
        }

        public ActionResult ObtenerReporteProductos(Producto producto)
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

        public ActionResult _ObtenerReporteProductos(Producto producto)
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

        public ActionResult BuscarReporteProductos(Producto producto)
        {
            try
            {
                
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if ( notificacion.Modelo != null )
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_BuscarProductos");
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

      



        //REPORTE INVENTARIO

        public ActionResult ReporteInventario()
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.lstProductos = notificacion.Modelo;
            return View();
        }

        public ActionResult ObtenerReporteInventario(Producto producto)
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

        public ActionResult _ObtenerReporteInventario(Producto producto)
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

        public ActionResult BuscarReporteInventario(Producto producto)
        {
            try
            {

                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_BuscarProductos");
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

        public ActionResult ReporteCompras()
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.lstProductos = notificacion.Modelo;
            return View();
        }

        public ActionResult ObtenerReporteCompras(Producto producto)
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

        public ActionResult _ObtenerReporteCompras(Producto producto)
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

        public ActionResult BuscarReporteCompras(Producto producto)
        {
            try
            {

                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_BuscarProductos");
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

        public ActionResult ReporteVentas()
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.lstProductos = notificacion.Modelo;
            return View();
        }

        public ActionResult ObtenerReporteVentas(Producto producto)
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

        public ActionResult _ObtenerReporteVentas(Producto producto)
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

        public ActionResult BuscarReporteVentas(Producto producto)
        {
            try
            {

                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_BuscarProductos");
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
