using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Web.Configuration;
using lluviaBackEnd.Filters;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using lluviaBackEnd.Utilerias;
using Newtonsoft.Json;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class PedidosEspecialesV2Controller : Controller
    {

        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_PedidosEspeciales)]
        public ActionResult PedidosEspeciales(PedidosEspecialesV2 pedidoEspecial)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                // falta validar
                ViewBag.cajaAbierta = true; //new PedidosEspecialesV2DAO().ValidaAperturaCajas(usuario.idUsuario).status == 200 ? true : false;
                // falta validar
                ViewBag.cierreCaja = false; //new PedidosEspecialesV2DAO().ValidaCierreCajas(usuario.idUsuario).status != 200 ? true : false;

                if (ViewBag.cajaAbierta)
                {
                    if (ViewBag.cierreCaja)
                        return RedirectToAction("CierreCajas");
                    else
                    {
                        //Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                        //notificacion = new ProductosDAO().ObtenerProductosPorUsuario(new Models.Producto() { idProducto = 0, idUsuario = usuario.idUsuario, activo = true });
                        //ViewBag.lstProductos = notificacion.Modelo;

                        Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
                        formasPago = new VentasDAO().ObtenerFormasPago();
                        ViewBag.lstFormasPago = formasPago.Modelo;

                        Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
                        usoCFDI = new VentasDAO().ObtenerUsoCFDI();
                        ViewBag.lstUsoCFDI = usoCFDI.Modelo;

                        ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
                        ViewBag.lstClientes = new ClienteDAO().ObtenerClientes(new Cliente() { idCliente = 0 });
                        ViewBag.lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes(0, 0);

                        ViewBag.pedidoEspecial = pedidoEspecial;
                        ViewBag.comisionBancaria = usuario.comisionBancaria;

                        return View();
                    }
                }
                else
                {
                    return View("AperturaCajas");
                }



                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult CierreCajas()
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                ViewBag.mostrarEfectivoEntregado = false;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult Cotizaciones()
        {
            try
            {
                Notificacion<dynamic> cotizaciones = new PedidosEspecialesV2DAO().ObtenerCotizaciones();
                return View(cotizaciones);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ConsultarPedidosEspeciales()
        {
            try
            {
                Notificacion<dynamic> clientes = new PedidosEspecialesV2DAO().ObtenerClientesPedidosEspeciales();
                Notificacion<dynamic> usuarios = new PedidosEspecialesV2DAO().ObtenerUsuariosPedidosEspeciales();
                Notificacion<dynamic> estatus = new PedidosEspecialesV2DAO().ObtenerEstatusPedidosEspeciales();

                var listClientes = new List<SelectListItem>();
                foreach (var cliente in clientes.Modelo)
                {
                    var item = new SelectListItem()
                    {
                        Text = cliente.nombreCliente,
                        Value = cliente.idCliente + ""
                    };

                    listClientes.Add(item);

                }


                var listUsuarios = new List<SelectListItem>();
                foreach (var usuario in usuarios.Modelo)
                {
                    var item = new SelectListItem()
                    {
                        Text = usuario.nombreUsuario,
                        Value = usuario.idUsuario + ""
                    };

                    listUsuarios.Add(item);

                }


                var listEstatus = new List<SelectListItem>();
                foreach (var est in estatus.Modelo)
                {
                    var item = new SelectListItem()
                    {
                        Text = est.descripcion,
                        Value = est.idEstatusPedidoEspecial + ""
                    };

                    listEstatus.Add(item);

                }

                ViewBag.listClientes = listClientes;
                ViewBag.listUsuarios = listUsuarios;
                ViewBag.listEstatus = listEstatus;


                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult BuscarPedidosEspeciales(DateTime? fechaIni, DateTime? fechaFin, Int64 idCliente = 0, Int64 idUsuario = 0, int idEstatusPedidoEspecial = 0)
        {
            try
            {

                Notificacion<dynamic> pedidosEspeciales = new PedidosEspecialesV2DAO().ObtenerPedidosEspeciales(fechaIni, fechaFin, idCliente, idUsuario, idEstatusPedidoEspecial);

                return Json(JsonConvert.SerializeObject(pedidosEspeciales), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



    }

}
