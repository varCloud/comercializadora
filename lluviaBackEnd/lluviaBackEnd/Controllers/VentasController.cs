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
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Nueva Venta
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Ventas(Ventas venta)
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            ViewBag.lstProductos = notificacion.Modelo;

            Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
            formasPago = new VentasDAO().ObtenerFormasPago();
            ViewBag.lstFormasPago = formasPago.Modelo;

            ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);

            ViewBag.venta = venta;

            return View();
        }

        public ActionResult ObtenerProductoPorPrecio(Precio precio)
        {
            try
            {
                Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
                notificacion = new VentasDAO().ObtenerProductoPorPrecio(precio);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult GuardarVenta(List<Ventas> venta)
        {
            try
            {
                Notificacion<Ventas> result = new Notificacion<Ventas>();
                result = new VentasDAO().GuardarVenta(venta);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Editar Ventas
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public ActionResult ConsultaVentas()
        {
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
            notificacion = new ReportesDAO().ObtenerVentas(new Models.Ventas() { idVenta = 0, tipoConsulta=2 });
            //Notificacion<List<Producto>> notificacionProductos = new Notificacion<List<Producto>>();
            //notificacionProductos = new ProductosDAO().ObtenerProductos(new Producto() { idProducto = 0 });

            ViewBag.lstProductos = new ProductosDAO().ObtenerListaProductos(new Producto() { idProducto = 0 } );
            ViewBag.lstVentas = notificacion.Modelo;
            ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);
            
            return View();
        }

        public ActionResult _ObtenerVentas(Ventas ventas)
        {
            try
            {
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                ventas.tipoConsulta = 2;
                notificacion = new ReportesDAO().ObtenerVentas(ventas);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstVentas = notificacion.Modelo;
                    return PartialView("_ObtenerVentas");
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
