﻿using System;
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
    public class PedidosEspecialesController : Controller
    {
        int idPedidoEspecial = 0;
        Retiros retiro = new Retiros();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Nuevo PedidoEspecial
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_PedidosEspeciales)]
        public ActionResult PedidosEspeciales(PedidosEspeciales peeidoEspecial)
        {
            Sesion usuario = Session["UsuarioActual"] as Sesion;

            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductosPorUsuario(new Models.Producto() { idProducto = 0, idUsuario = usuario.idUsuario, activo = true });
            ViewBag.lstProductos = notificacion.Modelo;

            //Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
            //formasPago = new PedidosEspecialesDAO().ObtenerFormasPago();
            //ViewBag.lstFormasPago = formasPago.Modelo;

            //Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
            //usoCFDI = new PedidosEspecialesDAO().ObtenerUsoCFDI();
            //ViewBag.lstUsoCFDI = usoCFDI.Modelo;

            ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
            ViewBag.listAlmacenes = new UsuarioDAO().ObtenerAlmacenes(0, 0);

            ViewBag.lstClientes = new ClienteDAO().ObtenerClientes(new Cliente() { idCliente = 0 });

            ViewBag.peeidoEspecial = peeidoEspecial;

            return View();
        }

     

        public ActionResult ObtenerPreciosDeProductos(List<Precio> precios)
        {
            try
            {
                Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
                notificacion = new PedidosEspecialesDAO().ObtenerPreciosDeProductos(precios);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult GuardarPedidoEspecial(PedidosEspeciales pedido)
        {
            try
            {
                Notificacion<PedidosEspeciales> result = new Notificacion<PedidosEspeciales>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                pedido.idUsuario = UsuarioActual.idUsuario;
                pedido.almacenDestino.idAlmacen = UsuarioActual.idAlmacen;
                result = new PedidosEspecialesDAO().GuardarPedidoEspecial(pedido);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult AceptarPedido(PedidosEspeciales pedido)
        {
            try
            {
                Notificacion<Result> result = new Notificacion<Result>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                pedido.idUsuario = UsuarioActual.idUsuario;
                result = new PedidosEspecialesDAO().AceptarPedido(pedido);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Editar PedidosEspeciales
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ConsultaPedidosEspeciales()
        {
            Notificacion<List<PedidosEspeciales>> notificacion = new Notificacion<List<PedidosEspeciales>>();
            //notificacion = new ReportesDAO().ObtenerPedidosEspeciales(new Models.PedidosEspeciales() { idPedidoEspecial = 0, tipoConsulta = 2 });
            ViewBag.lstProductos = new ProductosDAO().ObtenerListaProductos(new Producto() { idProducto = 0 });
            ViewBag.lstPedidosEspeciales = notificacion.Modelo;
            ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);

            Sesion usuario = Session["UsuarioActual"] as Sesion;
            ViewBag.devolucionesPermitidas = usuario.devolucionesPermitidas;
            ViewBag.agregarProductosPermitidos = usuario.agregarProductosPermitidos;

            Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
            formasPago = new PedidosEspecialesDAO().ObtenerFormasPago();
            ViewBag.lstFormasPago = formasPago.Modelo;

            Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
            usoCFDI = new PedidosEspecialesDAO().ObtenerUsoCFDI();
            ViewBag.lstUsoCFDI = usoCFDI.Modelo;

            return View();
        }

        //public ActionResult _ObtenerPedidosEspeciales(PedidosEspeciales PedidosEspeciales)
        //{
        //    try
        //    {
        //        Notificacion<List<PedidosEspeciales>> notificacion = new Notificacion<List<PedidosEspeciales>>();
        //        //PedidosEspeciales.tipoConsulta = 2;
        //        //notificacion = new ReportesDAO().ObtenerPedidosEspeciales(PedidosEspeciales);

        //        if (notificacion.Modelo != null)
        //        {
        //            ViewBag.lstPedidosEspeciales = notificacion.Modelo;
        //            return PartialView("_ObtenerPedidosEspeciales");
        //        }
        //        else
        //        {
        //            ViewBag.titulo = "Mensaje: ";
        //            ViewBag.mensaje = notificacion.Mensaje;
        //            return PartialView("_SinResultados");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        //[HttpPost]
        //public ActionResult CancelaVenta(PedidosEspeciales venta)
        //{
        //    try
        //    {
        //        Notificacion<PedidosEspeciales> notificacion = new Notificacion<PedidosEspeciales>();
        //        //notificacion = new PedidosEspecialesDAO().CancelaVenta(venta);
        //        return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public ActionResult ConsultaVenta(PedidosEspeciales venta)
        {
            try
            {
                Notificacion<PedidosEspeciales> notificacion = new Notificacion<PedidosEspeciales>();
                //notificacion = new PedidosEspecialesDAO().ConsultaVenta(venta);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_PedidosEspeciales)]
        public ActionResult ConsultaPedidos()
        {
            try
            {
                List<SelectListItem> listEstatus = new SelectList(new BitacoraDAO().ObtenerStatusPedidosInternos().Modelo, "idStatus", "descripcion").ToList();
                List<SelectListItem> listAlmacenes = new UsuarioDAO().ObtenerAlmacenes();
                List<SelectListItem> listProductos = new SelectList(new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 }).Modelo, "idProducto", "descripcion").ToList();
                List<SelectListItem> listUsuarios;
                Sesion usuario = Session["UsuarioActual"] as Sesion;

                if (usuario.idRol == 1)
                {
                    listUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                }
                else
                    listUsuarios = new UsuarioDAO().ObtenerUsuarios(usuario.idUsuario).Where(x => x.Value != "0").ToList();


                ViewBag.listEstatusPedidosInternos = listEstatus;
                ViewBag.listAlmacenes = listAlmacenes;
                ViewBag.listUsuarios = listUsuarios;
                ViewBag.listProductos = listProductos;


                return View(new PedidosEspeciales());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult _ObtenerPedidosEspeciales(PedidosEspeciales pedidosEspeciales)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                pedidosEspeciales.idUsuario = usuario.idUsuario;

                //if (pedidosInternos.usuario.idUsuario == 0 && usuario.idRol != 1)
                //    pedidosInternos.usuario.idUsuario = usuario.idUsuario;


                Notificacion<List<PedidosEspeciales>> p = new PedidosEspecialesDAO().ObtenerPedidosEspeciales(pedidosEspeciales);
                return PartialView(p);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult _DetallePedidoEspecial(int idPedidoInterno)
        {
            try
            {
                Notificacion<List<PedidosEspeciales>> p = new PedidosEspecialesDAO().ObtenerDetallePedidosEspeciales(idPedidoInterno);
                return PartialView(p);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Impresion de 

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ImprimeTicket(PedidosEspeciales pedidoEspecial)
        {
            Notificacion<PedidosEspeciales> notificacion;
            try
            {

                notificacion = new Notificacion<PedidosEspeciales>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;

                this.idPedidoEspecial = pedidoEspecial.idPedidoEspecial;

                PrintDocument pd = new PrintDocument();

                //if (pedidoEspecial.ticketVistaPrevia)
                //{
                //    notificacion.Mensaje = "Abriendo Ticket.";
                //    string nombreImpresora = string.Empty;
                //    foreach (String strPrinter in PrinterSettings.InstalledPrinters)
                //    {
                //        if (strPrinter.Contains("PDF"))
                //        {
                //            nombreImpresora = strPrinter;
                //        }
                //    }

                //    if (nombreImpresora == string.Empty)
                //    {
                //        notificacion.Mensaje = "No se encontro impresora PDF para previsualizar ticket.";
                //        notificacion.Estatus = -1;
                //        pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                //    }
                //    else
                //    {
                //        pd.PrinterSettings = new PrinterSettings
                //        {
                //            PrinterName = nombreImpresora, //"Microsoft XPS Document Writer",
                //            PrintToFile = true,
                //            PrintFileName = System.Web.HttpContext.Current.Server.MapPath("~") + "\\Tickets\\" + venta.idPedidoEspecial.ToString() + "_preview.pdf"
                //        };
                //    }
                //}
                //else
                //{
                    pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                //}

                PaperSize ps = new PaperSize("", 285, 540);
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                pd.PrintController = new StandardPrintController();
                pd.DefaultPageSettings.Margins.Left = 10;
                pd.DefaultPageSettings.Margins.Right = 0;
                pd.DefaultPageSettings.Margins.Top = 0;
                pd.DefaultPageSettings.Margins.Bottom = 0;
                pd.DefaultPageSettings.PaperSize = ps;
                pd.Print();

                return Json(notificacion, JsonRequestBehavior.AllowGet);

            }
            catch (InvalidPrinterException ex)
            {
                notificacion = new Notificacion<PedidosEspeciales>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                notificacion = new Notificacion<PedidosEspeciales>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }

        }

        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
                notificacion = new PedidosEspecialesDAO().ObtenerTicketsPedidosEspeciales(new Ticket() { idVenta = this.idPedidoEspecial });

                    //Logos
                    Image newImage = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("~") + "\\assets\\img\\logo_lluvia_150.jpg");

                    int ancho = 258;
                    int espaciado = 14;

                    //Configuración Global
                    GraphicsUnit units = GraphicsUnit.Pixel;
                    e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
                    e.Graphics.InterpolationMode = InterpolationMode.High;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                    //Configuración Texto
                    StringFormat centrado = new StringFormat();
                    centrado.Alignment = StringAlignment.Center;//Cetrado
                    StringFormat izquierda = new StringFormat();
                    izquierda.Alignment = StringAlignment.Near; //Izquierda
                    StringFormat derecha = new StringFormat();
                    derecha.Alignment = StringAlignment.Far; //Izquierda

                    //Tipo y tamaño de letra
                    Font font = new Font("Arial", 6.8F, FontStyle.Regular, GraphicsUnit.Point);
                    Font Bold = new Font("Arial", 6.8F, FontStyle.Bold, GraphicsUnit.Point);
                    Font BoldWester = new Font("Arial", 13, FontStyle.Bold, GraphicsUnit.Point);

                    //Color de texto
                    SolidBrush drawBrush = new SolidBrush(Color.Black);

                    //Se pinta logo 
                    Rectangle logo = new Rectangle(80, 15, 280, 81);
                    e.Graphics.DrawImage(newImage, logo, 0, 0, 380.0F, 120.0F, units);

                    Rectangle datos = new Rectangle(5, 110, ancho, 82);
                    e.Graphics.DrawString("RFC:" + "COVO781128LJ1" + ",\n" + "Calle Macarena #82" + '\n' + "Inguambo" + '\n' + "Uruapan, Michoacán" + '\n' + "C.p. 58000", font, drawBrush, datos, centrado);

                    e.Graphics.DrawString("Ticket:" + notificacion.Modelo[0].idVenta.ToString(), font, drawBrush, 40, 181, izquierda);
                    e.Graphics.DrawString("Fecha:" + DateTime.Now.ToString("dd-MM-yyyy"), font, drawBrush, 150, 181, izquierda);
                    e.Graphics.DrawString("Hora:" + DateTime.Now.ToShortTimeString(), font, drawBrush, 150, 191, izquierda);

                    Rectangle datosProducto = new Rectangle(5, 270, 180, 82);
                    Rectangle datosCantidad = new Rectangle(190, 270, 30, 82);
                    Rectangle datosPrecio = new Rectangle(220, 270, 48, 82);

                    Rectangle datosEnca = new Rectangle(0, 215, 280, 82);

                    e.Graphics.DrawString("  Cliente: " + notificacion.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;

                    e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Descripcion                                             Cantidad       Precio" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 8;
                    e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    //datosEnca.Y += 14;

                    float monto = 0;
                    float montoIVA = 0;
                    float montoAhorro = 0;

                for (int i = 0; i < notificacion.Modelo.Count(); i++)
                {
                    e.Graphics.DrawString(notificacion.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString((notificacion.Modelo[i].monto + notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);

                    monto += notificacion.Modelo[i].monto;
                    montoIVA += notificacion.Modelo[i].montoIVA;
                    montoAhorro += notificacion.Modelo[i].ahorro;

                    if (notificacion.Modelo[i].descProducto.ToString().Length >= 27)
                    {
                        datosProducto.Y += espaciado + 10;
                        datosCantidad.Y += espaciado + 10;
                        datosPrecio.Y += espaciado + 10;
                    }
                    else
                    {
                        datosProducto.Y += espaciado;
                        datosCantidad.Y += espaciado;
                        datosPrecio.Y += espaciado;
                    }

                    // si hay descuentos por mayoreo o rango de precios
                    if (notificacion.Modelo[i].ahorro > 0)
                    {
                        e.Graphics.DrawString("     └Descuento por mayoreo" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString("-" + (notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosCantidad.Y += espaciado;
                        datosPrecio.Y += espaciado;
                    }

       
                    //// si hay descuentos por mayoreo o rango de precios
                    //if (notificacion.Modelo[i].ahorro > 0)
                    //{
                    //    e.Graphics.DrawString("     -Descuento por mayoreo" + " \n", font, drawBrush, datosProducto, izquierda);
                    //    e.Graphics.DrawString("-" + (notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    //    datosProducto.Y += espaciado;
                    //    datosCantidad.Y += espaciado;
                    //    datosPrecio.Y += espaciado;

                    //}
                }

                    Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 280, 82);
                    e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado;

                    e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;

                    if (montoIVA > 0)
                    {
                        e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                        e.Graphics.DrawString((montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                        datosfooter1.Y += espaciado;
                    }

                    e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((monto + montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;

                    if (montoAhorro > 0)
                    {
                        Rectangle datosAhorro = new Rectangle(0, datosfooter1.Y + 20, 280, 82);
                        e.Graphics.DrawString("******* USTED AHORRO:  " + (montoAhorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " *******", font, drawBrush, datosAhorro, centrado);
                        datosfooter1.Y += espaciado;
                    }

                    Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
                    e.Graphics.DrawString("********  GRACIAS POR SU PREFERENCIA.  ********", font, drawBrush, datosfooter2, centrado);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;

                    // para mas espaciado al final del ticket
                    e.Graphics.DrawString("", font, drawBrush, 0, datosfooter2.Y, centrado);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;

            }
            catch (InvalidPrinterException ex)
            {
                //notificacion = new Notificacion<PedidosEspeciales>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<PedidosEspeciales>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }



        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Impresion de Ticket de Retiro por Exceso de Efectivo
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //public ActionResult ImprimeTicketRetiro(Retiros retiros)
        //{
        //    Notificacion<Retiros> notificacion;
        //    try
        //    {

        //        Sesion usuario = Session["UsuarioActual"] as Sesion;
        //        notificacion = new Notificacion<Retiros>();
        //        if (retiros.tipoRetiro == EnumTipoRetiro.RetirosExcesoEfectivo)
        //            notificacion.Mensaje = "Se envio el ticket de cierre por exceso de efectivo a la impresora.";
        //        else
        //            notificacion.Mensaje = "Se envio el ticket de cierre de día a la impresora.";

        //        notificacion.Estatus = 200;
        //        PrintDocument pd = new PrintDocument();
        //        pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
        //        this.retiro = retiros;
        //        this.retiro.idEstacion = usuario.idEstacion;
        //        PaperSize ps = new PaperSize("", 285, 540);
        //        pd.PrintPage += new PrintPageEventHandler(pd_PrintPageRetiro);
        //        pd.PrintController = new StandardPrintController();
        //        pd.DefaultPageSettings.Margins.Left = 10;
        //        pd.DefaultPageSettings.Margins.Right = 0;
        //        pd.DefaultPageSettings.Margins.Top = 0;
        //        pd.DefaultPageSettings.Margins.Bottom = 0;
        //        pd.DefaultPageSettings.PaperSize = ps;
        //        pd.Print();

        //        return Json(notificacion, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (InvalidPrinterException ex)
        //    {
        //        notificacion = new Notificacion<Retiros>();
        //        notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //        notificacion.Estatus = -1;
        //        return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        notificacion = new Notificacion<Retiros>();
        //        notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //        notificacion.Estatus = -1;
        //        return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }

        //}


        //void pd_PrintPageRetiro(object sender, PrintPageEventArgs e)
        //{
        //    Notificacion<List<Retiros>> notificacion = new Notificacion<List<Retiros>>();
        //    try
        //    {
        //        string titulo = string.Empty;
        //        Sesion usuario = Session["UsuarioActual"] as Sesion;
        //        retiro.idAlmacen = usuario.idAlmacen;
        //        if (usuario.idRol == 1 || usuario.idRol == 2)
        //        {
        //            this.retiro.idUsuario = 0;
        //        }
        //        else
        //        {
        //            this.retiro.idUsuario = usuario.idUsuario;
        //        }


        //        if (this.retiro.tipoRetiro == EnumTipoRetiro.RetirosExcesoEfectivo)
        //        {
        //            notificacion = new PedidosEspecialesDAO().ConsultaRetirosEfectivo(this.retiro);
        //            titulo = "\nCOMPROBANTE DE RETIRO" + "\n" + "POR EXCESO DE EFECTIVO";
        //        }
        //        else
        //        {
        //            notificacion = new PedidosEspecialesDAO().ConsultaRetiros(this.retiro);
        //            titulo = "\nCOMPROBANTE DE RETIRO" + "\n" + "POR CIERRE DE CAJA DEL DIA";
        //        }

        //        //Logos
        //        Image newImage = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("~") + "\\assets\\img\\logo_lluvia_150.jpg");

        //        int ancho = 258;
        //        int espaciado = 14;

        //        //Configuración Global
        //        GraphicsUnit units = GraphicsUnit.Pixel;
        //        e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
        //        e.Graphics.InterpolationMode = InterpolationMode.High;
        //        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

        //        //Configuración Texto
        //        StringFormat centrado = new StringFormat();
        //        centrado.Alignment = StringAlignment.Center;//Cetrado
        //        StringFormat izquierda = new StringFormat();
        //        izquierda.Alignment = StringAlignment.Near; //Izquierda
        //        StringFormat derecha = new StringFormat();
        //        derecha.Alignment = StringAlignment.Far; //Izquierda

        //        //Tipo y tamaño de letra
        //        Font font = new Font("Arial", 6.8F, FontStyle.Regular, GraphicsUnit.Point);
        //        Font Bold = new Font("Arial", 6.8F, FontStyle.Bold, GraphicsUnit.Point);
        //        Font BoldWester = new Font("Arial", 13, FontStyle.Bold, GraphicsUnit.Point);

        //        //Color de texto
        //        SolidBrush drawBrush = new SolidBrush(Color.Black);

        //        //Se pinta logo 
        //        Rectangle logo = new Rectangle(80, 15, 280, 81);
        //        e.Graphics.DrawImage(newImage, logo, 0, 0, 380.0F, 120.0F, units);

        //        Rectangle datos = new Rectangle(5, 110, ancho, 82);
        //        e.Graphics.DrawString(titulo, Bold, drawBrush, datos, centrado);

        //        Rectangle datosDescripcion = new Rectangle(5, 160, 280, 82);
        //        Rectangle datosTikcet = new Rectangle(120, 160, 180, 82);

        //        e.Graphics.DrawString("================================================" + " \n", font, drawBrush, datosDescripcion, izquierda);
        //        datosDescripcion.Y += 14;
        //        datosTikcet.Y += 14;

        //        e.Graphics.DrawString("Usuario: ", font, drawBrush, datosDescripcion, izquierda);
        //        e.Graphics.DrawString(notificacion.Modelo[0].nombreUsuario.ToString(), font, drawBrush, datosTikcet, izquierda);

        //        if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
        //        {
        //            datosDescripcion.Y += espaciado + 10;
        //            datosTikcet.Y += espaciado + 10;
        //        }
        //        else
        //        {
        //            datosDescripcion.Y += espaciado;
        //            datosTikcet.Y += espaciado;
        //        }

        //        e.Graphics.DrawString("Fecha: ", font, drawBrush, datosDescripcion, izquierda);
        //        e.Graphics.DrawString(notificacion.Modelo[0].fechaAlta.ToString(), font, drawBrush, datosTikcet, izquierda);

        //        if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
        //        {
        //            datosDescripcion.Y += espaciado + 10;
        //            datosTikcet.Y += espaciado + 10;
        //        }
        //        else
        //        {
        //            datosDescripcion.Y += espaciado;
        //            datosTikcet.Y += espaciado;
        //        }

        //        e.Graphics.DrawString("Sucursal: ", font, drawBrush, datosDescripcion, izquierda);
        //        e.Graphics.DrawString(notificacion.Modelo[0].descripcionSucursal.ToString(), font, drawBrush, datosTikcet, izquierda);

        //        if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
        //        {
        //            datosDescripcion.Y += espaciado + 10;
        //            datosTikcet.Y += espaciado + 10;
        //        }
        //        else
        //        {
        //            datosDescripcion.Y += espaciado;
        //            datosTikcet.Y += espaciado;
        //        }

        //        e.Graphics.DrawString("Alamcen: ", font, drawBrush, datosDescripcion, izquierda);
        //        e.Graphics.DrawString(notificacion.Modelo[0].descripcionAlmacen.ToString(), font, drawBrush, datosTikcet, izquierda);

        //        if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
        //        {
        //            datosDescripcion.Y += espaciado + 10;
        //            datosTikcet.Y += espaciado + 10;
        //        }
        //        else
        //        {
        //            datosDescripcion.Y += espaciado;
        //            datosTikcet.Y += espaciado;
        //        }

        //        e.Graphics.DrawString("Estación: ", font, drawBrush, datosDescripcion, izquierda);
        //        e.Graphics.DrawString(notificacion.Modelo[0].nombreEstacion.ToString(), font, drawBrush, datosTikcet, izquierda);

        //        if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
        //        {
        //            datosDescripcion.Y += espaciado + 10;
        //            datosTikcet.Y += espaciado + 10;
        //        }
        //        else
        //        {
        //            datosDescripcion.Y += espaciado;
        //            datosTikcet.Y += espaciado;
        //        }


        //        e.Graphics.DrawString("Monto: ", font, drawBrush, datosDescripcion, izquierda);
        //        e.Graphics.DrawString("" + notificacion.Modelo[0].montoRetiro.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, datosTikcet, izquierda);

        //        if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
        //        {
        //            datosDescripcion.Y += espaciado + 10;
        //            datosTikcet.Y += espaciado + 10;
        //        }
        //        else
        //        {
        //            datosDescripcion.Y += espaciado;
        //            datosTikcet.Y += espaciado;
        //        }


        //        Rectangle datosfooter1 = new Rectangle(5, datosDescripcion.Y, 280, 82);
        //        e.Graphics.DrawString("================================================" + " \n", font, drawBrush, datosfooter1, izquierda);
        //        datosfooter1.Y += espaciado + 50;


        //        e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
        //        datosfooter1.Y += espaciado;

        //        Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 5, 280, 82);

        //        e.Graphics.DrawString("  AUTORIZO  ", font, drawBrush, datosfooter2, centrado);
        //        datosfooter1.Y += espaciado;
        //        datosfooter2.Y += espaciado;

        //        // para mas espaciado al final del ticket
        //        e.Graphics.DrawString("-", font, drawBrush, 0, datosfooter2.Y + 30, centrado);
        //        datosfooter1.Y += espaciado;
        //        datosfooter2.Y += espaciado;
        //    }
        //    catch (InvalidPrinterException ex)
        //    {
        //        //notificacion = new Notificacion<PedidosEspeciales>();
        //        notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //        notificacion.Estatus = -1;
        //        //return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        //notificacion = new Notificacion<PedidosEspeciales>();
        //        notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //        notificacion.Estatus = -1;
        //        //return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }



        //}


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Visualizacion de Ticket 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult VerTicket(int idPedidoEspecial)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                Notificacion<List<Ticket>> ticket = new PedidosEspecialesDAO().ObtenerTicketsPedidosEspeciales(new Ticket() { idVenta = idPedidoEspecial });
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";
                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTicketPDF(ticket.Modelo));
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket: " + idPedidoEspecial.ToString(); ;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public ActionResult VerTicketDevolucion(int idPedidoEspecial)
        //{
        //    Notificacion<String> notificacion = new Notificacion<string>();
        //    try
        //    {
        //        Notificacion<List<Ticket>> ticket = new PedidosEspecialesDAO().ObtenerTickets(new Ticket() { idPedidoEspecial = idPedidoEspecial, tipoVenta = EnumTipoVenta.Devolucion }); 
        //        notificacion.Estatus = 200;
        //        notificacion.Mensaje = "Ticket generado correctamente.";
        //        string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTicketDevolucion(ticket.Modelo));
        //        ViewBag.pdfBase64 = pdfCodigos;
        //        ViewBag.title = "Ticket: " + idPedidoEspecial.ToString(); ;
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}






        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////  Impresion de ticket de devolucion
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //public ActionResult ImprimeTicketDevolucion(PedidosEspeciales venta)
        //{
        //    Notificacion<PedidosEspeciales> notificacion;
        //    try
        //    {

        //        notificacion = new Notificacion<PedidosEspeciales>();
        //        notificacion.Mensaje = "Se envio el ticket a la impresora.";
        //        notificacion.Estatus = 200;

        //        this.idPedidoEspecial = venta.idPedidoEspecial;

        //        PrintDocument pd = new PrintDocument();
        //        pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";

        //        PaperSize ps = new PaperSize("", 285, 540);
        //        pd.PrintPage += new PrintPageEventHandler(pd_PrintPageDevoluciones);
        //        pd.PrintController = new StandardPrintController();
        //        pd.DefaultPageSettings.Margins.Left = 10;
        //        pd.DefaultPageSettings.Margins.Right = 0;
        //        pd.DefaultPageSettings.Margins.Top = 0;
        //        pd.DefaultPageSettings.Margins.Bottom = 0;
        //        pd.DefaultPageSettings.PaperSize = ps;
        //        pd.Print();

        //        return Json(notificacion, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (InvalidPrinterException ex)
        //    {
        //        notificacion = new Notificacion<PedidosEspeciales>();
        //        notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //        notificacion.Estatus = -1;
        //        return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        notificacion = new Notificacion<PedidosEspeciales>();
        //        notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //        notificacion.Estatus = -1;
        //        return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }

        //}

        //void pd_PrintPageDevoluciones(object sender, PrintPageEventArgs e)
        //{
        //    Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
        //    try
        //    {
        //        notificacion = new PedidosEspecialesDAO().ObtenerTickets(new Ticket() { idPedidoEspecial = this.idPedidoEspecial, tipoVenta = EnumTipoVenta.Devolucion });

        //        //Logos
        //        Image newImage = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("~") + "\\assets\\img\\logo_lluvia_150.jpg");

        //        int ancho = 258;
        //        int espaciado = 14;

        //        //Configuración Global
        //        GraphicsUnit units = GraphicsUnit.Pixel;
        //        e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
        //        e.Graphics.InterpolationMode = InterpolationMode.High;
        //        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

        //        //Configuración Texto
        //        StringFormat centrado = new StringFormat();
        //        centrado.Alignment = StringAlignment.Center;//Cetrado
        //        StringFormat izquierda = new StringFormat();
        //        izquierda.Alignment = StringAlignment.Near; //Izquierda
        //        StringFormat derecha = new StringFormat();
        //        derecha.Alignment = StringAlignment.Far; //Izquierda

        //        //Tipo y tamaño de letra
        //        Font font = new Font("Arial", 6.8F, FontStyle.Regular, GraphicsUnit.Point);
        //        Font Bold = new Font("Arial", 6.8F, FontStyle.Bold, GraphicsUnit.Point);
        //        Font BoldWester = new Font("Arial", 13, FontStyle.Bold, GraphicsUnit.Point);

        //        //Color de texto
        //        SolidBrush drawBrush = new SolidBrush(Color.Black);

        //        //Se pinta logo 
        //        Rectangle logo = new Rectangle(80, 15, 280, 81);
        //        e.Graphics.DrawImage(newImage, logo, 0, 0, 380.0F, 120.0F, units);

        //        Rectangle datos = new Rectangle(5, 110, ancho, 82);
        //        e.Graphics.DrawString("*******************************************************************" + "\n" + "***************** TICKET DE DEVOLUCIÓN *****************" + '\n' + "*******************************************************************", font, drawBrush, datos, centrado);

        //        e.Graphics.DrawString("Ticket:" + notificacion.Modelo[0].idPedidoEspecial.ToString(), font, drawBrush, 40, 181, izquierda);
        //        e.Graphics.DrawString("Fecha:" + DateTime.Now.ToString("dd-MM-yyyy"), font, drawBrush, 150, 181, izquierda);
        //        e.Graphics.DrawString("Hora:" + DateTime.Now.ToShortTimeString(), font, drawBrush, 150, 191, izquierda);

        //        Rectangle datosProducto = new Rectangle(5, 270, 180, 82);
        //        Rectangle datosCantidad = new Rectangle(190, 270, 30, 82);
        //        Rectangle datosPrecio = new Rectangle(220, 270, 48, 82);

        //        Rectangle datosEnca = new Rectangle(0, 215, 280, 82);

        //        e.Graphics.DrawString("  Cliente: " + notificacion.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
        //        datosEnca.Y += 14;

        //        e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
        //        datosEnca.Y += 14;
        //        e.Graphics.DrawString("  Descripcion                                      Art. Devueltos       Precio" + " \n", font, drawBrush, datosEnca, izquierda);
        //        datosEnca.Y += 8;
        //        e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
        //        //datosEnca.Y += 14;

        //        float monto = 0;

        //        for (int i = 0; i < notificacion.Modelo.Count(); i++)
        //        {
        //            e.Graphics.DrawString(notificacion.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
        //            e.Graphics.DrawString(notificacion.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
        //            e.Graphics.DrawString((notificacion.Modelo[i].monto + notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);

        //            monto += notificacion.Modelo[i].monto;

        //            if (notificacion.Modelo[i].descProducto.ToString().Length >= 27)
        //            {
        //                datosProducto.Y += espaciado + 10;
        //                datosCantidad.Y += espaciado + 10;
        //                datosPrecio.Y += espaciado + 10;
        //            }
        //            else
        //            {
        //                datosProducto.Y += espaciado;
        //                datosCantidad.Y += espaciado;
        //                datosPrecio.Y += espaciado;
        //            }

        //        }

        //        Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 280, 82);
        //        e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
        //        datosfooter1.Y += espaciado;

        //        e.Graphics.DrawString("TOTAL DEVUELTO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //        e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
        //        datosfooter1.Y += espaciado*2;

                
        //        Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
        //        e.Graphics.DrawString("___________________________________________________", font, drawBrush, datosfooter2, centrado);
        //        datosfooter1.Y += espaciado;
        //        datosfooter2.Y += espaciado;
        //        e.Graphics.DrawString("FIRMA DEL CLIENTE", font, drawBrush, datosfooter2, centrado);
        //        datosfooter1.Y += espaciado*2;
        //        datosfooter2.Y += espaciado*2;

        //        // para mas espaciado al final del ticket
        //        e.Graphics.DrawString("", font, drawBrush, 0, datosfooter2.Y, centrado);
        //        datosfooter1.Y += espaciado;
        //        datosfooter2.Y += espaciado;

        //    }
        //    catch (InvalidPrinterException ex)
        //    {
        //        //notificacion = new Notificacion<PedidosEspeciales>();
        //        notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //        notificacion.Estatus = -1;
        //        //return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        //notificacion = new Notificacion<PedidosEspeciales>();
        //        notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //        notificacion.Estatus = -1;
        //        //return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }



        //}





    }


}