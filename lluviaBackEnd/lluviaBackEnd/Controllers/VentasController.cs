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

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class VentasController : Controller
    {
        int idVenta = 0;
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

            Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
            usoCFDI = new VentasDAO().ObtenerUsoCFDI();
            ViewBag.lstUsoCFDI = usoCFDI.Modelo;

            ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
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
        public ActionResult GuardarVenta(List<Ventas> venta, int idCliente, int formaPago, int usoCFDI, int idVenta, int aplicaIVA)
        {
            try
            {
                Notificacion<Ventas> result = new Notificacion<Ventas>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                result = new VentasDAO().GuardarVenta(venta, idCliente, formaPago, usoCFDI, idVenta, UsuarioActual.idUsuario, UsuarioActual.idEstacion, aplicaIVA);
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
            ViewBag.lstProductos = new ProductosDAO().ObtenerListaProductos(new Producto() { idProducto = 0 } );
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
                notificacion = new VentasDAO().ConsultaInfoCierre(new Cierre() { idEstacion=usuario.idEstacion} );
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
                Notificacion<List<Retiros>> p = new VentasDAO().ConsultaRetirosEfectivo(retiros);
                return PartialView(p);
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


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Impresion de Ticket
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ImprimeTicket(Ventas venta )
        {
            Notificacion<Ventas> notificacion;
            try
            {

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";

                this.idVenta = venta.idVenta;

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                PaperSize ps = new PaperSize("", 120, 540);

                pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

                pd.PrintController = new StandardPrintController();
                pd.DefaultPageSettings.Margins.Left = 0;
                pd.DefaultPageSettings.Margins.Right = 0;
                pd.DefaultPageSettings.Margins.Top = 0;
                pd.DefaultPageSettings.Margins.Bottom = 0;

                pd.DefaultPageSettings.PaperSize = ps;

                pd.Print();

                return Json(notificacion, JsonRequestBehavior.AllowGet);

            }
            catch (InvalidPrinterException ex) {
                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora "+ex.Message;
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


        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
                notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = this.idVenta });

                //Logos
                //Image newImage = Image.FromFile(Application.StartupPath + "\\Imagenes\\logo_lluvia_150.jpg");
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
                Rectangle datosPrecio = new Rectangle(225, 270, 40, 82);

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
                    e.Graphics.DrawString(notificacion.Modelo[i].monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
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


                }

                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 280, 82);
                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 267, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                if (montoIVA > 0)
                {
                    e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 267, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }

                e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString((monto + montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 267, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                if ( montoAhorro >= 0 )
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

    }


}
