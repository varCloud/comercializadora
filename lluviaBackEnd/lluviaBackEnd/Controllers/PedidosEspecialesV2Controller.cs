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


        public ActionResult _ObtenerEntregarPedidos(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
                //ventas.tipoConsulta = 2;
                //ventas.idAlmacen = UsuarioActual.idRol == 1 ? 0 : UsuarioActual.idAlmacen;

                notificacion = new PedidosEspecialesV2DAO().ObtenerEntregarPedidos(pedidosEspecialesV2);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstEntregas = notificacion.Modelo;
                    return PartialView("_ObtenerEntregarPedidos");
                }
                else
                {
                    ViewBag.titulo = "Mensaje: ";
                    ViewBag.mensaje = notificacion.Mensaje;
                    return PartialView("_ObtenerEntregarPedidos");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_PedidosEspeciales)]
        public ActionResult EntregarPedido(PedidosEspecialesV2 pedidoEspecial)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;

                ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);
                ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0);

                //List<SelectListItem> listUsuarios = new SelectList(new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = 3, idAlmacen = UsuarioActual.idRol == 1 ? 0 : UsuarioActual.idAlmacen }), "idUsuario", "nombreCompleto").ToList();
                //ViewBag.lstUsuarios = listUsuarios;

                //List<SelectListItem> listUsuarios = new SelectList(new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = 5 }), "idUsuario", "nombreCompleto").ToList();
                //ViewBag.lstUsuarios = listUsuarios;



                //ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
                //ViewBag.lstClientes = new ClienteDAO().ObtenerClientes(new Cliente() { idCliente = 0 });
                //ViewBag.lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                //ViewBag.lstPedidosEspeciales = new PedidosEspecialesV2DAO().ConsultaPedidosEspeciales(pedidoEspecial.idPedidoEspecial);
                //ViewBag.comisionBancaria = usuario.comisionBancaria;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_PedidosEspeciales)]
        public ActionResult ConfirmarProductos(PedidosEspecialesV2 pedidoEspecial)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                //ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
                //ViewBag.lstClientes = new ClienteDAO().ObtenerClientes(new Cliente() { idCliente = 0 });
                //ViewBag.lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes(0, 0);                
                //ViewBag.pedidoEspecial = new PedidosEspecialesV2DAO().ConsultaPedidoEspecial(pedidoEspecial.idPedidoEspecial);
                //ViewBag.comisionBancaria = usuario.comisionBancaria;
                pedidoEspecial.lstProductos = new List<Producto>();
                pedidoEspecial.lstProductos = new PedidosEspecialesV2DAO().ConsultaPedidoEspecialDetalle(pedidoEspecial.idPedidoEspecial);

                List<SelectListItem> listUsuariosRuteo = new SelectList(new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = 5 }), "idUsuario", "nombreCompleto").ToList();
                ViewBag.listUsuariosRuteo = listUsuariosRuteo;

                List<SelectListItem> listUsuariosTaxi = new SelectList(new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = 10 }), "idUsuario", "nombreCompleto").ToList();
                ViewBag.listUsuariosTaxi = listUsuariosTaxi;

                ViewBag.pedidoEspecial = pedidoEspecial;
                return View();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarConfirmacion(List<Producto> productos, int idPedidoEspecial, int idEstatusPedidoEspecial, int idUsuarioEntrega, int numeroUnidadTaxi, int idEstatusCuentaPorCobrar, float montoTotal, float montoTotalcantidadAbonada)
        {
            try
            {
                Notificacion<PedidosEspecialesV2> result = new Notificacion<PedidosEspecialesV2>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                idUsuarioEntrega = UsuarioActual.idUsuario;
                result = new PedidosEspecialesV2DAO().GuardarConfirmacion(productos, idPedidoEspecial, idEstatusPedidoEspecial, idUsuarioEntrega, numeroUnidadTaxi, idEstatusCuentaPorCobrar, montoTotal, montoTotalcantidadAbonada);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public ActionResult GuardarPedidoEspecial(List<Producto> productos, int tipoRevision, int idCliente, int idEstatusPedidoEspecial)
        {
            try
            {
                Notificacion<PedidosEspecialesV2> result = new Notificacion<PedidosEspecialesV2>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                result = new PedidosEspecialesV2DAO().GuardarPedidoEspecial(productos, tipoRevision, idCliente, UsuarioActual.idUsuario, idEstatusPedidoEspecial, UsuarioActual.idEstacion);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /************** REIMPRIMIR TICKETS ALMACENES********************/
        /************** REIMPRIMIR TICKET  GENERAL********************/



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

        public ActionResult BuscarPedidosEspeciales(Filtro filtro)
        {
            try
            {

                Notificacion<dynamic> pedidosEspeciales = new PedidosEspecialesV2DAO().ObtenerPedidosEspeciales(filtro);

                return Json(JsonConvert.SerializeObject(pedidosEspeciales), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ObtenerPedidosEspecialesDetalle(Int64 idPedidoEspecial)
        {
            try
            {

                Notificacion<dynamic> pedidosEspeciales = new PedidosEspecialesV2DAO().ObtenerPedidosEspecialesDetalle(idPedidoEspecial);

                return Json(JsonConvert.SerializeObject(pedidosEspeciales), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region CuentasPorCobrar
        public ActionResult ConsultarCuentasPorCobrar()
        {
            try
            {
                Notificacion<dynamic> clientes = new PedidosEspecialesV2DAO().ObtenerClientesCuentasXCobrar();             

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

                ViewBag.listClientes = listClientes;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult BuscarCuentasPorCobrar(Filtro filtro)
        {
            try
            {

                Notificacion<dynamic> cuentasPorCobrar = new PedidosEspecialesV2DAO().ObtenerCuentasXCobrar(filtro);

                return Json(JsonConvert.SerializeObject(cuentasPorCobrar), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ObtenerCuentasPorCobrarDetalle(Int64 idCliente)
        {
            try
            {

                Notificacion<dynamic> detalleCuentasPorCobrar = new PedidosEspecialesV2DAO().ObtenerCuentasXCobrarDetalle(idCliente);

                return Json(JsonConvert.SerializeObject(detalleCuentasPorCobrar), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult RealizarAbonoPedidosEspeciales(Int64 idCliente,float montoAdeudo)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<string> resultAbono = new PedidosEspecialesV2DAO().RealizarAbonoPedidoEspecial(idCliente, montoAdeudo, UsuarioActual.idUsuario);
                return Json(JsonConvert.SerializeObject(resultAbono), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult PDFDetalleCuentasPorCobrar(Int64 idCliente)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                List<Cliente> clientes=new ClienteDAO().ObtenerClientes(new Cliente() { idCliente = idCliente });
                Cliente cliente = clientes.First();
                Notificacion<dynamic> balance = new PedidosEspecialesV2DAO().ObtenerBalanceCuentasXCobrar(idCliente);
                string pdf = Convert.ToBase64String(Utilerias.Utils.GeneraPDFCuentasPorCobrar(cliente, balance));
                return Json(pdf, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        #endregion
    }

}
