﻿using System;
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

      

        public ActionResult BuscarInventario(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ReportesDAO().ObtenerInventario(producto);

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
            Notificacion<List<Compras>> notificacion = new Notificacion<List<Compras>>();
            notificacion = new ReportesDAO().ObtenerCompras(new Models.Compras() { idCompra = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstProveedores = new ProveedorDAO().ObtenerProveedores(0); // todos los proovedores en formato select list
            ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0); // todos los usuarios en formato select list
            ViewBag.lstCompras = notificacion.Modelo;
            return View();
        }

       

        public ActionResult BuscarCompras(Compras compras)
        {
            try
            {

                Notificacion<List<Compras>> notificacion = new Notificacion<List<Compras>>();
                notificacion = new ReportesDAO().ObtenerCompras(compras);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
                    ViewBag.lstProveedores = new ProveedorDAO().ObtenerProveedores(0); // todos los proovedores en formato select list
                    ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0); // todos los usuarios en formato select list
                    ViewBag.lstCompras = notificacion.Modelo;
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
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
            notificacion = new ReportesDAO().ObtenerVentas(new Models.Ventas() { idVenta = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);
            ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
            ViewBag.lstVentas = notificacion.Modelo;
            return View();
        }

      

        public ActionResult BuscarVentas(Ventas ventas)
        {
            try
            {

                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                notificacion = new ReportesDAO().ObtenerVentas(ventas);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
                    ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);
                    ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                    ViewBag.lstVentas = notificacion.Modelo;
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