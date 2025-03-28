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
<<<<<<< HEAD
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
=======
using lluviaBackEnd.Utilerias;

>>>>>>> 9d82c7f3ceda7d8a089832b3fd1f335f07cbd3e6

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class VentasController : Controller
    {
        int idVenta = 0;
        Retiros retiro = new Retiros();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Nueva Venta
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Ventas(Ventas venta)
        {
            Sesion usuario = Session["UsuarioActual"] as Sesion;

            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            notificacion = new ProductosDAO().ObtenerProductosPorUsuario(new Models.Producto() { idProducto = 0, idUsuario = usuario.idUsuario, activo = true });
            ViewBag.lstProductos = notificacion.Modelo;

            Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
            formasPago = new VentasDAO().ObtenerFormasPago();
            ViewBag.lstFormasPago = formasPago.Modelo;

            Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
            usoCFDI = new VentasDAO().ObtenerUsoCFDI();
            ViewBag.lstUsoCFDI = usoCFDI.Modelo;

            ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
            ViewBag.lstClientes = new ClienteDAO().ObtenerClientes(new Cliente() { idCliente = 0 });

            ViewBag.venta = venta;

            return View();
        }

        //public ActionResult ObtenerProductoPorPrecio(Precio precio)
        //{
        //    try
        //    {
        //        Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
        //        notificacion = new VentasDAO().ObtenerProductoPorPrecio(precio);
        //        return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public ActionResult ObtenerPreciosDeProductos(List<Precio> precios)
        {
            try
            {
                Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
                notificacion = new VentasDAO().ObtenerPreciosDeProductos(precios);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult GuardarVenta(List<Ventas> venta, int idCliente, int formaPago, int usoCFDI, int idVenta, int aplicaIVA, int numClientesAtendidos)
        {
            try
            {
                Notificacion<Ventas> result = new Notificacion<Ventas>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                result = new VentasDAO().GuardarVenta(venta, idCliente, formaPago, usoCFDI, idVenta, UsuarioActual.idUsuario, UsuarioActual.idEstacion, aplicaIVA, numClientesAtendidos);
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
            notificacion = new ReportesDAO().ObtenerVentas(new Models.Ventas() { idVenta = 0, tipoConsulta = 2 });
            ViewBag.lstProductos = new ProductosDAO().ObtenerListaProductos(new Producto() { idProducto = 0 });
            ViewBag.lstVentas = notificacion.Modelo;
            ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);

            Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
            formasPago = new VentasDAO().ObtenerFormasPago();
            ViewBag.lstFormasPago = formasPago.Modelo;

            Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
            usoCFDI = new VentasDAO().ObtenerUsoCFDI();
            ViewBag.lstUsoCFDI = usoCFDI.Modelo;

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


        [HttpPost]
        public ActionResult CancelaVenta(Ventas venta)
        {
            try
            {
                Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
                notificacion = new VentasDAO().CancelaVenta(venta);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ConsultaVenta(Ventas venta)
        {
            try
            {
                Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
                notificacion = new VentasDAO().ConsultaVenta(venta);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarIVA(Ventas venta)
        {
            try
            {
                Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
                notificacion = new VentasDAO().GuardarIVA(venta);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Herramientas
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        public ActionResult ConsultaInfoCierre()
        {
            try
            {
                Notificacion<Cierre> notificacion = new Notificacion<Cierre>();
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                notificacion = new VentasDAO().ConsultaInfoCierre(new Cierre() { idEstacion = usuario.idEstacion, idUsuario = usuario.idUsuario, idAlmacen = usuario.idAlmacen });
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _ObtenerRetiros(Retiros retiros)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                retiros.idEstacion = usuario.idEstacion;
                retiros.idAlmacen = usuario.idAlmacen;

                if (usuario.idRol == 1 || usuario.idRol == 2)
                {
                    retiros.idUsuario = 0;
                }
                else
                {
                    retiros.idUsuario = usuario.idUsuario;
                }

                Notificacion<List<Retiros>> p = new VentasDAO().ConsultaRetirosEfectivo(retiros);
                return PartialView(p);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult _ObtenerRetirosV2(Retiros retiros)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                retiros.idEstacion = usuario.idEstacion;
                retiros.idAlmacen = usuario.idAlmacen;

                if (usuario.idRol == 1 || usuario.idRol == 2)
                {
                    retiros.idUsuario = 0;
                }
                else
                {
                    retiros.idUsuario = usuario.idUsuario;
                }

                Notificacion<List<Retiros>> p = new VentasDAO().ConsultaRetiros(retiros);
                return PartialView(p);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ActionResult Retiros()
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;

                Retiros retiros = new Retiros();
                retiros.fechaAlta = DateTime.Now;
                if ((usuario.idRol != 1) && (usuario.idRol != 2))
                {
                    retiros.idAlmacen = usuario.idAlmacen;
                    retiros.idUsuario = usuario.idUsuario;

                }
                ViewBag.Almacenes = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                List<SelectListItem> listUsuarios = new SelectList(new UsuarioDAO().ObtenerUsuarios(new Usuario { idUsuario = retiro.idUsuario, idAlmacen = retiros.idAlmacen, idRol = 3 }), "idUsuario", "nombreCompleto").ToList();

                ViewBag.Usuarios = listUsuarios;
                return View(retiros);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult _ObtenerRetirosAutorizacion(Retiros retiros)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                ViewBag.idRol = usuario.idRol;

                //if (usuario.idRol == 1 || usuario.idRol == 2)
                //{
                //    retiros.idUsuario = 0;
                //}
                //else
                //{
                //    retiros.idUsuario = usuario.idUsuario;
                //}
                //retiros.idAlmacen = usuario.idAlmacen;

                Notificacion<List<Retiros>> p = new VentasDAO().ConsultaRetiros(retiros);
                return PartialView(p);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult ActualizaEstatusRetiro(Retiros retiros)
        {
            try
            {
                Notificacion<string> notificacion = new Notificacion<string>();
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                retiros.idUsuario = usuario.idUsuario;
                notificacion = new VentasDAO().ActualizaEstatusRetiro(retiros);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public ActionResult RetirarExcesoEfectivo(float montoRetiro)
        {
            try
            {
                Notificacion<Retiros> notificacion = new Notificacion<Retiros>();
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                Retiros retiros = new Retiros();
                retiros.idEstacion = usuario.idEstacion;
                retiros.idUsuario = usuario.idUsuario;
                retiros.montoRetiro = montoRetiro;
                notificacion = new VentasDAO().RetirarExcesoEfectivo(retiros);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult RealizaCierreEstacion(float monto)
        {
            try
            {
                Notificacion<Retiros> notificacion = new Notificacion<Retiros>();
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                Retiros retiros = new Retiros();
                retiros.idEstacion = usuario.idEstacion;
                retiros.idUsuario = usuario.idUsuario;
                retiros.montoRetiro = monto;
                notificacion = new VentasDAO().RealizaCierreEstacion(retiros);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Impresion de Ticket
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ImprimeTicket(Ventas venta)
        {
            Notificacion<Ventas> notificacion;
            try
            {

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;

                this.idVenta = venta.idVenta;

                PrintDocument pd = new PrintDocument();

                if (venta.ticketVistaPrevia)
                {
                    notificacion.Mensaje = "Abriendo Ticket.";
                    string nombreImpresora = string.Empty;
                    foreach (String strPrinter in PrinterSettings.InstalledPrinters)
                    {
                        if (strPrinter.Contains("PDF"))
                        {
                            nombreImpresora = strPrinter;
                        }
                    }

                    if (nombreImpresora == string.Empty)
                    {
                        notificacion.Mensaje = "No se encontro impresora PDF para previsualizar ticket.";
                        notificacion.Estatus = -1;
                        pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                    }
                    else
                    {
                        pd.PrinterSettings = new PrinterSettings
                        {
                            PrinterName = nombreImpresora, //"Microsoft XPS Document Writer",
                            PrintToFile = true,
                            PrintFileName = System.Web.HttpContext.Current.Server.MapPath("~") + "\\Tickets\\" + venta.idVenta.ToString() + "_preview.pdf"
                        };
                    }
                }
                else
                {
                    pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                }

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
                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }

        }

        const int PHYSICALWIDTH = 110;  // Physical Width in device units           
        const int PHYSICALHEIGHT = 111; // Physical Height in device units          

        // This function returns the device capability value specified
        // by the requested index value.
        [DllImport("GDI32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 GetDeviceCaps(IntPtr hdc, int nIndex);

        protected void SaveViaBitmap( PrintPageEventArgs e)
        {
            int width = e.PageBounds.Width;
            int height = e.PageBounds.Height;
            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                // Draw all the graphics using into g (into the bitmap)
                g.DrawLine(Pens.Black, 0, 0, 100, 100);
            }
            e.Graphics.DrawImage(bitmap, 0, 0);
            bitmap.Save("ticket", ImageFormat.Png);
        }
        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
                SaveViaBitmap(e);
                // Retrieve the physical bitmap boundaries from the PrintPage Graphics Device Context
                IntPtr hdc = e.Graphics.GetHdc();
                Int32 PhysicalWidth = GetDeviceCaps(hdc, (Int32)PHYSICALWIDTH);
                Int32 PhysicalHeight = GetDeviceCaps(hdc, (Int32)PHYSICALHEIGHT);
                e.Graphics.ReleaseHdc(hdc);

                // Create a bitmap with PrintPage Graphic's size and resolution
                Bitmap myBitmap = new Bitmap(PhysicalWidth, PhysicalHeight, e.Graphics);
                // Get the new work Graphics to use to draw the bitmap
                Graphics myGraphics = Graphics.FromImage(myBitmap);

                // Draw everything on myGraphics to build the bitmap

                // Transfer the bitmap to the PrintPage Graphics
                e.Graphics.DrawImage(myBitmap, 0, 0);
                myBitmap.Save("tick", ImageFormat.Png);
                // Cleanup 
                myBitmap.Dispose();
                notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = this.idVenta });

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

<<<<<<< HEAD
=======
                    // si hay descuentos por mayoreo o rango de precios
                    if (notificacion.Modelo[i].ahorro > 0 )
                    {
                        e.Graphics.DrawString("     -Descuento por mayoreo" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString("-"+(notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosCantidad.Y += espaciado;
                        datosPrecio.Y += espaciado;
>>>>>>> 9d82c7f3ceda7d8a089832b3fd1f335f07cbd3e6
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
                //notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }



        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Impresion de Ticket de Retiro por Exceso de Efectivo
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ImprimeTicketRetiro(Retiros retiros)
        {
            Notificacion<Retiros> notificacion;
            try
            {

                Sesion usuario = Session["UsuarioActual"] as Sesion;
                notificacion = new Notificacion<Retiros>();
                if (retiros.tipoRetiro == EnumTipoRetiro.RetirosExcesoEfectivo)
                    notificacion.Mensaje = "Se envio el ticket de cierre por exceso de efectivo a la impresora.";
                else
                    notificacion.Mensaje = "Se envio el ticket de cierre de día a la impresora.";

                notificacion.Estatus = 200;
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                this.retiro = retiros;
                this.retiro.idEstacion = usuario.idEstacion;
                PaperSize ps = new PaperSize("", 285, 540);
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPageRetiro);
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
                notificacion = new Notificacion<Retiros>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                notificacion = new Notificacion<Retiros>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }

        }


        void pd_PrintPageRetiro(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Retiros>> notificacion = new Notificacion<List<Retiros>>();
            try
            {
                string titulo = string.Empty;
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                retiro.idAlmacen = usuario.idAlmacen;
                if (usuario.idRol == 1 || usuario.idRol == 2)
                {
                    this.retiro.idUsuario = 0;
                }
                else
                {
                    this.retiro.idUsuario = usuario.idUsuario;
                }


                if (this.retiro.tipoRetiro == EnumTipoRetiro.RetirosExcesoEfectivo)
                {
                    notificacion = new VentasDAO().ConsultaRetirosEfectivo(this.retiro);
                    titulo = "\nCOMPROBANTE DE RETIRO" + "\n" + "POR EXCESO DE EFECTIVO";
                }
                else
                {
                    notificacion = new VentasDAO().ConsultaRetiros(this.retiro);
                    titulo = "\nCOMPROBANTE DE RETIRO" + "\n" + "POR CIERRE DE CAJA DEL DIA";
                }

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
                e.Graphics.DrawString(titulo, Bold, drawBrush, datos, centrado);

                Rectangle datosDescripcion = new Rectangle(5, 160, 280, 82);
                Rectangle datosTikcet = new Rectangle(120, 160, 180, 82);

                e.Graphics.DrawString("================================================" + " \n", font, drawBrush, datosDescripcion, izquierda);
                datosDescripcion.Y += 14;
                datosTikcet.Y += 14;

                e.Graphics.DrawString("Usuario: ", font, drawBrush, datosDescripcion, izquierda);
                e.Graphics.DrawString(notificacion.Modelo[0].nombreUsuario.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
                {
                    datosDescripcion.Y += espaciado + 10;
                    datosTikcet.Y += espaciado + 10;
                }
                else
                {
                    datosDescripcion.Y += espaciado;
                    datosTikcet.Y += espaciado;
                }

                e.Graphics.DrawString("Fecha: ", font, drawBrush, datosDescripcion, izquierda);
                e.Graphics.DrawString(notificacion.Modelo[0].fechaAlta.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
                {
                    datosDescripcion.Y += espaciado + 10;
                    datosTikcet.Y += espaciado + 10;
                }
                else
                {
                    datosDescripcion.Y += espaciado;
                    datosTikcet.Y += espaciado;
                }

                e.Graphics.DrawString("Sucursal: ", font, drawBrush, datosDescripcion, izquierda);
                e.Graphics.DrawString(notificacion.Modelo[0].descripcionSucursal.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
                {
                    datosDescripcion.Y += espaciado + 10;
                    datosTikcet.Y += espaciado + 10;
                }
                else
                {
                    datosDescripcion.Y += espaciado;
                    datosTikcet.Y += espaciado;
                }

                e.Graphics.DrawString("Alamcen: ", font, drawBrush, datosDescripcion, izquierda);
                e.Graphics.DrawString(notificacion.Modelo[0].descripcionAlmacen.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
                {
                    datosDescripcion.Y += espaciado + 10;
                    datosTikcet.Y += espaciado + 10;
                }
                else
                {
                    datosDescripcion.Y += espaciado;
                    datosTikcet.Y += espaciado;
                }

                e.Graphics.DrawString("Estación: ", font, drawBrush, datosDescripcion, izquierda);
                e.Graphics.DrawString(notificacion.Modelo[0].nombreEstacion.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
                {
                    datosDescripcion.Y += espaciado + 10;
                    datosTikcet.Y += espaciado + 10;
                }
                else
                {
                    datosDescripcion.Y += espaciado;
                    datosTikcet.Y += espaciado;
                }


                e.Graphics.DrawString("Monto: ", font, drawBrush, datosDescripcion, izquierda);
                e.Graphics.DrawString("" + notificacion.Modelo[0].montoRetiro.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, datosTikcet, izquierda);

                if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
                {
                    datosDescripcion.Y += espaciado + 10;
                    datosTikcet.Y += espaciado + 10;
                }
                else
                {
                    datosDescripcion.Y += espaciado;
                    datosTikcet.Y += espaciado;
                }


                Rectangle datosfooter1 = new Rectangle(5, datosDescripcion.Y, 280, 82);
                e.Graphics.DrawString("================================================" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado + 50;


                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado;

                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 5, 280, 82);

                e.Graphics.DrawString("  AUTORIZO  ", font, drawBrush, datosfooter2, centrado);
                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;

                // para mas espaciado al final del ticket
                e.Graphics.DrawString("-", font, drawBrush, 0, datosfooter2.Y + 30, centrado);
                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;
            }
            catch (InvalidPrinterException ex)
            {
                //notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }



        }




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Apertura de Cajon
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult AbrirCajon()
        {
            Notificacion<Retiros> notificacion;
            try
            {
                notificacion = new Notificacion<Retiros>();
                notificacion.Mensaje = "Abriendo Cajón del Dinero.";
                notificacion.Estatus = 200;
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString();
                PaperSize ps = new PaperSize("", 285, 540);
                pd.PrintPage += new PrintPageEventHandler(pd_PrintAbreCajon);
                pd.PrintController = new StandardPrintController();
                pd.Print();
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (InvalidPrinterException ex)
            {
                notificacion = new Notificacion<Retiros>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                notificacion = new Notificacion<Retiros>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }

        }


        void pd_PrintAbreCajon(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Retiros>> notificacion = new Notificacion<List<Retiros>>();
            try
            {
                StringFormat centrado = new StringFormat();
                centrado.Alignment = StringAlignment.Center;
                Font Bold = new Font("Arial", 6.8F, FontStyle.Bold, GraphicsUnit.Point);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                Rectangle datos = new Rectangle(5, 0, 0, 0);
                e.Graphics.DrawString(" ", Bold, drawBrush, datos, centrado);
            }
            catch (InvalidPrinterException ex)
            {
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
            }
            catch (Exception ex)
            {
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
            }

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Visualizacion de Ticket 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        [HttpPost]
        public JsonResult VerTicket(int idVenta)
        {
            try
            {
                Notificacion<List<Ticket>> ticket = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta });
                Notificacion<String> notificacion = new Notificacion<string>();                
                notificacion.Modelo = Utils.GeneraTicketPDF(ticket.Modelo);
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";
                return Json(notificacion, JsonRequestBehavior.AllowGet); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }


}
