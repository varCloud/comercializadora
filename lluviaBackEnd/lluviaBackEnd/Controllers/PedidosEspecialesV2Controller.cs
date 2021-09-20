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



    }

}
