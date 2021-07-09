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
using System.IO;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class VentasController : Controller
    {
        int idVenta = 0;
        int idDevolucion = 0;
        int idComplemento = 0;
        Retiros retiro = new Retiros();
        int idIngresoEfectivo = 0;
        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Nueva Venta
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_Ventas)]
        public ActionResult Ventas(Ventas venta)
        {
            try
            {
                
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                ViewBag.cajaAbierta = new VentasDAO().ValidaAperturaCajas(usuario.idUsuario).status == 200 ? true : false;
                ViewBag.cierreCaja = new VentasDAO().ValidaCierreCajas(usuario.idUsuario).status != 200 ? true : false;

                if (ViewBag.cajaAbierta)
                {
                    if (ViewBag.cierreCaja)
                        return RedirectToAction("CierreCajas");
                    else
                    {
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
                        ViewBag.comisionBancaria = usuario.comisionBancaria;
                        return View();
                    }
                }
                else
                {
                    return View("AperturaCajas");
                }


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
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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
        public ActionResult GuardarVenta(List<Ventas> venta, int idCliente, int formaPago, int usoCFDI, int idVenta, int aplicaIVA, int numClientesAtendidos, int tipoVenta, string motivoDevolucion, int idPedidoEspecial = 0,Int64 idVentaComplemento=0, float montoTotalVenta=0, float montoPagado = 0)
        {
            try
            {
                Notificacion<Ventas> result = new Notificacion<Ventas>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                result = new VentasDAO().GuardarVenta(venta, idCliente, formaPago, usoCFDI, idVenta, UsuarioActual.idUsuario, UsuarioActual.idEstacion, aplicaIVA, numClientesAtendidos, tipoVenta, motivoDevolucion, idPedidoEspecial,idVentaComplemento,montoTotalVenta, montoPagado);
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
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                //notificacion = new ReportesDAO().ObtenerVentas(new Models.Ventas() { idVenta = 0, fechaFin = DateTime.Now, fechaIni =DateTime.Now ,tipoConsulta = 2, idAlmacen = UsuarioActual.idRol == 1 ? 0 : UsuarioActual.idAlmacen });
                ViewBag.lstProductos = new ProductosDAO().ObtenerListaProductos(new Producto() { idProducto = 0,idUsuario= UsuarioActual.idUsuario });
                //ViewBag.lstVentas = notificacion.Modelo;
                ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);

                List<SelectListItem> listUsuarios = new SelectList(new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = 3,idAlmacen= UsuarioActual.idRol==1 ? 0 : UsuarioActual.idAlmacen }), "idUsuario", "nombreCompleto").ToList();
                ViewBag.lstUsuarios = listUsuarios;
               

                Sesion usuario = Session["UsuarioActual"] as Sesion;
                ViewBag.devolucionesPermitidas = usuario.devolucionesPermitidas;
                ViewBag.agregarProductosPermitidos = usuario.agregarProductosPermitidos;

                Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
                formasPago = new VentasDAO().ObtenerFormasPago();
                ViewBag.lstFormasPago = formasPago.Modelo;

                Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
                usoCFDI = new VentasDAO().ObtenerUsoCFDI();
                ViewBag.lstUsoCFDI = usoCFDI.Modelo;

                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult _ObtenerVentas(Ventas ventas)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                ventas.tipoConsulta = 2;
                ventas.idAlmacen = UsuarioActual.idRol == 1 ? 0 : UsuarioActual.idAlmacen;
                
                notificacion = new VentasDAO().ObtenerVentasEditar(ventas);

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
        public JsonResult CancelaVenta(Ventas venta)
        {
            try
            {
                if (Session != null)
                {
                    Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                    venta.idUsuario = UsuarioActual.idUsuario;
                }


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
        //  Consultar Ventas Canceladas
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ConsultaVentasCanceladas()
        {
            try
            {              
             
                ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);

                Sesion usuario = Session["UsuarioActual"] as Sesion;              
                Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
                formasPago = new VentasDAO().ObtenerFormasPago();
                ViewBag.lstFormasPago = formasPago.Modelo;
                Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
                usoCFDI = new VentasDAO().ObtenerUsoCFDI();
                ViewBag.lstUsoCFDI = usoCFDI.Modelo;

                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult _ObtenerVentasCanceladas(Ventas ventas)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                ventas.tipoConsulta = 2;
                ventas.estatusVenta = 2;
                ventas.idAlmacen = usuario.idRol == 1 ? 0 : usuario.idAlmacen;                
                notificacion = new ReportesDAO().ObtenerVentas(ventas);
                return PartialView(notificacion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Herramientas
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

        public ActionResult _CierreDia()
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                Notificacion<Cierre> cierre = new VentasDAO().ConsultaInfoCierre(new Cierre() { idEstacion = usuario.idEstacion, idUsuario = usuario.idUsuario, idAlmacen = usuario.idAlmacen });
                Notificacion<List<Retiros>> retirosExcesoEfectivo = new VentasDAO().ConsultaRetirosEfectivo(new Retiros() { idEstacion = usuario.idEstacion, idAlmacen = usuario.idAlmacen, idUsuario = usuario.idUsuario });
                Notificacion<List<Retiros>> retirosDia = new VentasDAO().ConsultaRetiros(new Retiros() { idEstacion = usuario.idEstacion, idAlmacen = usuario.idAlmacen, idUsuario = usuario.idUsuario });

                List<Retiros> retiros = new List<Retiros>();
                if (retirosExcesoEfectivo.Estatus == 200)
                    retiros.AddRange(retirosExcesoEfectivo.Modelo);
                if (retirosDia.Estatus == 200)
                    retiros.AddRange(retirosDia.Modelo);

                return PartialView(new Tuple<Notificacion<Cierre>, List<Retiros>>(cierre, retiros));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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

        [HttpPost]
        public ActionResult BuscaVentaCodigoBarras(string codigoBarras)
        {
            try
            {
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                notificacion = new VentasDAO().BuscaVentaCodigoBarras(codigoBarras);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Impresion de 

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        public ActionResult ImprimeTicket(Ventas venta)
        {
            Notificacion<Ventas> notificacion;
            try
            {

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;

                this.idVenta = venta.idVenta;

                //PrintDocument pd = new PrintDocument();
                using (PrintDocument pd = new PrintDocument())
                {
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
                    Notificacion<List<Ticket>> _notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = this.idVenta });
                    PaperSize ps = new PaperSize("", 285, 540);
                    pd.PrintPage +=  (_sender, args) => pd_PrintPage(null, args, _notificacion );
                    pd.PrintController = new StandardPrintController();
                    pd.DefaultPageSettings.Margins.Left = 10;
                    pd.DefaultPageSettings.Margins.Right = 0;
                    pd.DefaultPageSettings.Margins.Top = 0;
                    pd.DefaultPageSettings.Margins.Bottom = 0;
                    pd.DefaultPageSettings.PaperSize = ps;
                    pd.Print();
                    pd.Dispose();
                }

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
        int indiceProducto = 0;
        int paginaActual = 0;
        int productosporPagina = 30;
        int paginas = 0;
        int indexProducto = 0;
        Boolean control = false;
        void pd_PrintPage(object sender, PrintPageEventArgs e, Notificacion<List<Ticket>> notificacion)
        {
            try
            {
                List<Ticket> lstTickets = notificacion.Modelo;
                //if (this.paginaActual< _lstTickets.Count())
                //    lstTickets = _lstTickets[this.paginaActual];
                float monto = 0;
                float montoIVA = 0;
                float montoComisionBancaria = 0;
                float montoAhorro = 0;
                float montoPagado = 0;
                float suCambio = 0;

                float montoPagadoAgregarProductos = 0;
                float montoAgregarProductos = 0;
                float suCambioAgregarProductos = 0;
                float cantidadTotalDeArticulos = 0;

                int ancho = 258;
                int espaciado = 14;
                Rectangle datosIndex = new Rectangle(2, 285, 15, 82);
                Rectangle datosProducto = new Rectangle(20, 285, 145, 82);
                Rectangle datosCantidad = new Rectangle(167, 285, 30, 82);
                Rectangle datosPrecioU = new Rectangle(205, 285, 30, 82);
                Rectangle datosPrecio = new Rectangle(235, 285, 48, 82);

                // TOTALES
                montoPagadoAgregarProductos = notificacion.Modelo[0].montoPagadoAgregarProductos;
                montoAgregarProductos = notificacion.Modelo[0].montoAgregarProductos;
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
                //Logos
                if (paginaActual == 0)
                {
                    //Se pinta logo 
                    //Se pinta logo 
                    int poslogoY = 15;
                    int postTicketY = 0;
                    Image newImage = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\assets\\img\\logo_lluvia_150.jpg");
                    Rectangle logo = new Rectangle(80, poslogoY, 280, 81);
                    e.Graphics.DrawImage(newImage, logo, 0, 0, 380.0F, 120.0F, units);

                    Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));

                    postTicketY = (logo.Y + logo.Height + espaciado);
                    //Rectangle posImgCodigoTicket = new Rectangle(0, postTicketY, 1000, 65);
                    //e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 10.0F, .10F, units);
                    e.Graphics.DrawImage(imagenCodigoTicket, 0, postTicketY, 300, 60);

                    postTicketY = postTicketY + 100 + espaciado;
                    Rectangle datos = new Rectangle(5, postTicketY, ancho, 82);
                    e.Graphics.DrawString("RFC:" + "COVO781128LJ1" + ",\n" + "Calle Macarena #82" + '\n' + "Inguambo" + '\n' + "Uruapan, Michoacán" + '\n' + "C.p. 58000", font, drawBrush, datos, centrado);

                    e.Graphics.DrawString("Ticket:" + notificacion.Modelo[0].idVenta.ToString(), font, drawBrush, 40, 181, izquierda);
                    e.Graphics.DrawString("Fecha:" + notificacion.Modelo[0].fechaAlta.ToString("dd-MM-yyyy"), font, drawBrush, 150, 181, izquierda);
                    e.Graphics.DrawString("Hora:" + notificacion.Modelo[0].fechaAlta.ToShortTimeString(), font, drawBrush, 150, 191, izquierda);

                    postTicketY = datos.Y + datos.Height + espaciado;
                    Rectangle datosEnca = new Rectangle(0, postTicketY, 295, 82);

                    e.Graphics.DrawString("  Cliente: " + notificacion.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Forma de Pago: " + notificacion.Modelo[0].descFormaPago.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;

                    e.Graphics.DrawString("______________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("#    Descripcion                              Cantidad     Precio       Precio" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 9;
                    e.Graphics.DrawString("                                                                          Unitario       " + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 6;
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    datosIndex = new Rectangle(2, datosEnca.Y, 15, 82);
                    datosProducto = new Rectangle(20, datosEnca.Y, 145, 82);
                    datosCantidad = new Rectangle(167, datosEnca.Y, 30, 82);
                    datosPrecioU = new Rectangle(205, datosEnca.Y, 30, 82);
                    datosPrecio = new Rectangle(235, datosEnca.Y, 48, 82);


                }
                else
                {
                    datosIndex = new Rectangle(2, 15, 15, 82);
                    datosProducto = new Rectangle(20, 15, 145, 82);
                    datosCantidad = new Rectangle(177, 15, 30, 82);
                    datosPrecioU = new Rectangle(205, 15, 30, 82);
                    datosPrecio = new Rectangle(235, 15, 48, 82);
                }

                for (int i = indexProducto; i < lstTickets.Count(); i++)
                {
                    e.Graphics.DrawString((indexProducto+1).ToString() + " \n", font, drawBrush, datosIndex, izquierda);
                    e.Graphics.DrawString(lstTickets[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(lstTickets[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString(lstTickets[i].precioVenta.ToString() + " \n", font, drawBrush, datosPrecioU, izquierda);
                    e.Graphics.DrawString((lstTickets[i].monto + lstTickets[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);


                    monto += lstTickets[i].monto;
                    montoIVA += lstTickets[i].montoIVA;
                    montoComisionBancaria += lstTickets[i].montoComisionBancaria;
                    montoAhorro += lstTickets[i].ahorro;

                    if (lstTickets[i].descProducto.ToString().Length >= 23)
                    {
                        datosIndex.Y += espaciado + 10;
                        datosProducto.Y += espaciado + 10;
                        datosCantidad.Y += espaciado + 10;
                        datosPrecioU.Y += espaciado + 10;
                        datosPrecio.Y += espaciado + 10;
                    }
                    else
                    {

                        datosIndex.Y += espaciado;
                        datosProducto.Y += espaciado;
                        datosCantidad.Y += espaciado;
                        datosPrecioU.Y += espaciado;
                        datosPrecio.Y += espaciado;
                    }

                    // si hay descuentos por mayoreo o rango de precios
                    if (lstTickets[i].ahorro > 0)
                    {
                        e.Graphics.DrawString("     └Descuento por mayoreo" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString("-" + (lstTickets[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosCantidad.Y += espaciado;
                        datosPrecioU.Y += espaciado;
                        datosPrecio.Y += espaciado;
                        datosIndex.Y += espaciado;
                    }


                    Console.WriteLine("indexProducto: " + indexProducto);
                    indexProducto++;
                    if (datosProducto.Y >= 1092)
                    {
                        this.paginaActual++;
                        e.HasMorePages = true;
                        return;
                    }

                }
               
                monto += notificacion.Modelo.Sum(x => x.monto);
                montoIVA += notificacion.Modelo.Sum(x => x.montoIVA);
                montoComisionBancaria += notificacion.Modelo.Sum(x => x.montoComisionBancaria);
                montoAhorro += notificacion.Modelo.Sum(x => x.ahorro);
                cantidadTotalDeArticulos += notificacion.Modelo.Sum(x => x.cantidad);
                int posXFooter = 285;
                if (montoAgregarProductos > 0)
                {
                    monto -= montoAgregarProductos;
                }

                suCambioAgregarProductos = montoPagadoAgregarProductos - montoAgregarProductos;
                montoPagado = notificacion.Modelo[0].montoPagado;
                suCambio = montoPagado - monto - montoIVA - montoComisionBancaria;

                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);

                if (indexProducto == lstTickets.Count())
                {
                    datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 1)
                {
                    e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 2)
                {
                    //if (montoComisionBancaria > 0)
                    //{
                    e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    
                    //}
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 3)
                //if (montoIVA > 0)
                {
                    e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    
                }


                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 4)
                {
                    e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((monto + montoIVA + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 5)
                {
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 6)
                {
                    e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoPagado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 7)
                {
                    e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((suCambio).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    
                }
                if (this.insertaPagina(datosfooter1.Y, ref e)) return;

                if (montoAgregarProductos > 0)
                {

                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    datosfooter1.Y += espaciado;
                    if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                    e.Graphics.DrawString("  COMPLEMENTOS:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    datosfooter1.Y += espaciado;
                    if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                    e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoPagadoAgregarProductos).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                    e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((suCambioAgregarProductos).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                }
                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 8)
                {
                    datosfooter1.Y += espaciado;
                    Rectangle totalArt = new Rectangle(5, datosfooter1.Y, ancho, 82);
                    e.Graphics.DrawString("  CANTIDAD DE ARTICULOS COMPRADOS: "+cantidadTotalDeArticulos.ToString(), font, drawBrush, totalArt, centrado);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                }


                if (indexProducto == lstTickets.Count() + 9)
                {
                    if (montoAhorro > 0)
                    {
                        datosfooter1.Y += espaciado;
                        Rectangle datosAhorro = new Rectangle(0, datosfooter1.Y, 280, 82);
                        e.Graphics.DrawString("******* USTED AHORRO:  " + (montoAhorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " *******", font, drawBrush, datosAhorro, centrado);
                        datosfooter1.Y += espaciado;
                        indexProducto++;
                        
                    }
                    else {
                        indexProducto++;
                    }
                }
                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
                if (indexProducto >= lstTickets.Count() + 10)
                {
                    datosfooter1.Y += espaciado;
                    datosfooter2 = new Rectangle(0, datosfooter1.Y , 280, 82);
                    e.Graphics.DrawString("********  GRACIAS POR SU PREFERENCIA.  ********", font, drawBrush, datosfooter2, centrado);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;
                }


                //Se pinta codigo de barras en ticket
                //Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));

                //datosfooter1.Y += 40;
                //Rectangle posImgCodigoTicket = new Rectangle(0, datosfooter1.Y, 400, 120);
                //e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 380.0F, 120.0F, units);

                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;

                // para mas espaciado al final del ticket
                e.Graphics.DrawString("", font, drawBrush, 0, datosfooter2.Y, centrado);
                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;
                //}



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
        public bool insertaPagina(int y, ref PrintPageEventArgs e)
        {
            if (y >= 1092)
            {
                this.paginaActual++;
                e.HasMorePages = true;
                return true;

            }
            return false;
        }

        public Image ByteArrayToImage(byte[] data)
        {
            MemoryStream bipimag = new MemoryStream(data);
            Image imag = new Bitmap(bipimag);
            return imag;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Impresion de Ticket de Venta Cancelada
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        public ActionResult ImprimeTicketVentaCancelada(Ventas venta)
        {
            Notificacion<Ventas> notificacion;
            try
            {

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;

                this.idVenta = venta.idVenta;
               
                //PrintDocument pd = new PrintDocument();
                using (PrintDocument pd = new PrintDocument())
                {
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
                    pd.PrintPage += new PrintPageEventHandler(pd_PrintPageVentaCancelada);
                    pd.PrintController = new StandardPrintController();
                    pd.DefaultPageSettings.Margins.Left = 10;
                    pd.DefaultPageSettings.Margins.Right = 0;
                    pd.DefaultPageSettings.Margins.Top = 0;
                    pd.DefaultPageSettings.Margins.Bottom = 0;
                    pd.DefaultPageSettings.PaperSize = ps;
                    pd.Print();
                    pd.Dispose();
                }

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

        void pd_PrintPageVentaCancelada(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
                notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = this.idVenta, estatusVenta = EnumEstatusVenta.Cancelada });

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
                e.Graphics.DrawString("VENTA CANCELADA", Bold, drawBrush, datos, centrado);

                e.Graphics.DrawString("Ticket:" + notificacion.Modelo[0].idVenta.ToString(), font, drawBrush, 40, 136, izquierda);
                e.Graphics.DrawString("Fecha:" + notificacion.Modelo[0].fechaAlta.ToString("dd-MM-yyyy"), font, drawBrush, 150, 136, izquierda);
                e.Graphics.DrawString("Hora:" + notificacion.Modelo[0].fechaAlta.ToShortTimeString(), font, drawBrush, 150, 146, izquierda);

                Rectangle datosProducto = new Rectangle(5, 240, 180, 82);
                Rectangle datosCantidad = new Rectangle(190, 240, 30, 82);
                Rectangle datosPrecio = new Rectangle(220, 240, 48, 82);

                Rectangle datosEnca = new Rectangle(0, 170, 280, 82);

                e.Graphics.DrawString("  Cliente: " + notificacion.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;
                e.Graphics.DrawString("  Forma de Pago: " + notificacion.Modelo[0].descFormaPago.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;
                e.Graphics.DrawString("  Descripcion                                             Cantidad       Precio" + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 8;
                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                //datosEnca.Y += 14;

                float monto = 0;
                float montoIVA = 0;
                float montoComisionBancaria = 0;
                float montoAhorro = 0;

                for (int i = 0; i < notificacion.Modelo.Count(); i++)
                {
                    e.Graphics.DrawString(notificacion.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString((notificacion.Modelo[i].monto + notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);

                    monto += notificacion.Modelo[i].monto;
                    montoIVA += notificacion.Modelo[i].montoIVA;
                    montoComisionBancaria += notificacion.Modelo[i].montoComisionBancaria;
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
                }

                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 280, 82);
                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                if (montoComisionBancaria > 0)
                {
                    e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }

                if (montoIVA > 0)
                {
                    e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }

                e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString((monto + montoIVA + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;


                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
                e.Graphics.DrawString("********  GRACIAS POR SU PREFERENCIA.  ********", font, drawBrush, datosfooter2, centrado);
                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;

                //Se pinta codigo de barras en ticket
                Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));

                datosfooter1.Y += 40;
                Rectangle posImgCodigoTicket = new Rectangle(0, datosfooter1.Y, 400, 120);
                e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 380.0F, 120.0F, units);

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


                Rectangle datosProducto = new Rectangle(5, 240, 180, 82);
                Rectangle datosPrecio = new Rectangle(220, 240, 48, 82);

                Rectangle datosEnca = new Rectangle(0, 160, 280, 82);

                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 280, 82);

                e.Graphics.DrawString("  Usuario: " + notificacion.Modelo[0].nombreUsuario.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("  Fecha: " + notificacion.Modelo[0].fechaAlta.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("  Sucursal: " + notificacion.Modelo[0].descripcionSucursal.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("  Almacen: " + notificacion.Modelo[0].descripcionAlmacen.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("  Estacion: " + notificacion.Modelo[0].nombreEstacion.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("================================================" + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;


                if (this.retiro.tipoRetiro == EnumTipoRetiro.RetirosCierreDia)
                {
                    e.Graphics.DrawString("No. Ventas" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].totalVentas + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString("Apertura de Caja" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].montoApertura.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString("Ingresos de Efectivo" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].montoIngresosEfectivo.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString("Monto Total Ventas Contado" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].montoVentasContado.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString("Monto Total Ventas Tarjeta" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].montoVentasTarjeta.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString("Monto Total Ventas Canceladas" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].montoVentasCanceladas.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString("No. Productos Devueltos" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].ProductosDevueltos + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString("Monto Total Devoluciones" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].MontoTotalDevoluciones.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString("Monto Total Retiros" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].retirosExcesoEfectivo.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    datosfooter1.Y = datosProducto.Y;

                    e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado;

                    e.Graphics.DrawString("  SALDO TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].montoCierre.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;

                    e.Graphics.DrawString("  SALDO EN CAJA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].montoRetiro.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }
                else
                {
                    e.Graphics.DrawString("  MONTO RETIRO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[0].montoRetiro.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }

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

        public ActionResult VerTicket(int idVenta)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                Notificacion<List<Ticket>> ticket = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta });
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";
                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTicketPDF(ticket.Modelo));
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket: " + idVenta.ToString(); ;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult VerTodosTickets(int idVenta)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                Notificacion<List<Ticket>> ticket = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta });
                Notificacion<List<Ticket>> numeroDevoluciones = new VentasDAO().ObtenerTicketsDevolucionComplemento(idVenta, TipoTicket.Devolucion);
                Notificacion<List<Ticket>> numeroComplementos = new VentasDAO().ObtenerTicketsDevolucionComplemento(idVenta, TipoTicket.Complemento);
                List<List<Ticket>> devoluciones = new List<List<Ticket>>();
                List<List<Ticket>> complementos = new List<List<Ticket>>();                

                if ( numeroDevoluciones.Modelo != null ) 
                { 
                    foreach (Ticket dev in numeroDevoluciones.Modelo) { 
                        Notificacion<List<Ticket>> d = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta, idDevolucion = dev.idDevolucion, tipoVenta = EnumTipoVenta.Devolucion });
                        d.Modelo[0].tipoVenta = EnumTipoVenta.Devolucion;
                        devoluciones.Add(d.Modelo);
                    }
                }

                if ( numeroComplementos.Modelo != null )  
                {
                    foreach (Ticket com in numeroComplementos.Modelo)
                    {
                        Notificacion<List<Ticket>> c = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta, idComplemento = com.idComplemento, tipoVenta = EnumTipoVenta.AgregarProductosVenta });
                        c.Modelo[0].tipoVenta = EnumTipoVenta.AgregarProductosVenta;
                        complementos.Add(c.Modelo);
                    }
                }

                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";                
                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTodosTicketsPDF(ticket.Modelo, devoluciones, complementos));                
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket: " + idVenta.ToString();
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult VerTicketVentaCancelada(int idVenta)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                Notificacion<List<Ticket>> ticket = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta,estatusVenta=EnumEstatusVenta.Cancelada });
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";
                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTicketCancelacionPDF(ticket.Modelo));
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket: " + idVenta.ToString(); ;
                return View("VerTicket");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult VerTicketDevolucion(int idVenta, int idDevolucion)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                Notificacion<List<Ticket>> ticket = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta, idDevolucion = idDevolucion, tipoVenta = EnumTipoVenta.Devolucion });
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";

                if (ticket.Modelo.Count > 0)
                    ticket.Modelo[0].tipoVenta = EnumTipoVenta.Devolucion;

                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTicketDevolucionComplemento(ticket.Modelo));
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket: " + idVenta.ToString(); ;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult VerTicketComplemento(int idVenta, int idComplemento)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                Notificacion<List<Ticket>> ticket = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta, idComplemento = idComplemento, tipoVenta = EnumTipoVenta.AgregarProductosVenta });
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";
                
                if( ticket.Modelo.Count > 0 )
                    ticket.Modelo[0].tipoVenta = EnumTipoVenta.AgregarProductosVenta;

                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTicketDevolucionComplemento(ticket.Modelo));
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket: " + idVenta.ToString(); ;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult VerTicketDespachadores(int idVenta)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                Notificacion<List<Ticket>> ticket = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta, tipoVenta = EnumTipoVenta.ProductosLiquidos });
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";
                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTicketDespachadoresPDF(ticket.Modelo));
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket para despachadores: " + idVenta.ToString(); ;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Impresion de ticket de devolucion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ImprimeTicketDevolucion(Ventas venta)
        {
            Notificacion<Ventas> notificacion;
            try
            {

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;

                this.idVenta = venta.idVenta;
                this.idDevolucion = venta.idDevolucion;

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";

                PaperSize ps = new PaperSize("", 285, 540);
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPageDevoluciones);
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

        void pd_PrintPageDevoluciones(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
                notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta, idDevolucion = idDevolucion, tipoVenta = EnumTipoVenta.Devolucion });

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
                e.Graphics.DrawString("*******************************************************************" + "\n" + "***************** TICKET DE DEVOLUCIÓN *****************" + '\n' + "*******************************************************************", font, drawBrush, datos, centrado);

                e.Graphics.DrawString("Ticket:" + notificacion.Modelo[0].idVenta.ToString(), font, drawBrush, 40, 181, izquierda);
                e.Graphics.DrawString("Fecha:" +  notificacion.Modelo[0].fechaAlta.ToString("dd-MM-yyyy"), font, drawBrush, 150, 181, izquierda);
                e.Graphics.DrawString("Hora:" +   notificacion.Modelo[0].fechaAlta.ToShortTimeString(), font, drawBrush, 150, 191, izquierda);

                Rectangle datosProducto = new Rectangle(5, 285, 145, 82);
                Rectangle datosCantidad = new Rectangle(150, 285, 30, 82);
                Rectangle datosPrecioU = new Rectangle(190, 285, 30, 82);
                Rectangle datosPrecio = new Rectangle(220, 285, 48, 82);

                Rectangle datosEnca = new Rectangle(0, 215, 280, 82);

                e.Graphics.DrawString("  Cliente: " + notificacion.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;
                e.Graphics.DrawString("  Descripcion                                 Art.          Precio       Precio" + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 9;
                e.Graphics.DrawString("                                                 Devueltos    Unitario       " + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 6;
                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                //datosEnca.Y += 14;

                float monto = 0;
                float montoComisionBancaria = 0;

                for (int i = 0; i < notificacion.Modelo.Count(); i++)
                {
                    e.Graphics.DrawString(notificacion.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[i].precioVenta.ToString() + " \n", font, drawBrush, datosPrecioU, izquierda);
                    e.Graphics.DrawString((notificacion.Modelo[i].monto + notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);

                    monto += notificacion.Modelo[i].monto;
                    montoComisionBancaria += notificacion.Modelo[i].montoComisionBancaria;

                    if (notificacion.Modelo[i].descProducto.ToString().Length >= 23)
                    {
                        datosProducto.Y += espaciado + 10;
                        datosCantidad.Y += espaciado + 10;
                        datosPrecioU.Y += espaciado + 10;
                        datosPrecio.Y += espaciado + 10;
                    }
                    else
                    {
                        datosProducto.Y += espaciado;
                        datosCantidad.Y += espaciado;
                        datosPrecioU.Y += espaciado;
                        datosPrecio.Y += espaciado;
                    }

                }



                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 280, 82);
                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                if (montoComisionBancaria > 0)
                {
                    e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }


                e.Graphics.DrawString("  TOTAL DEVUELTO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString((monto + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado * 2;


                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
                e.Graphics.DrawString("___________________________________________________", font, drawBrush, datosfooter2, centrado);
                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;
                e.Graphics.DrawString("FIRMA DEL CLIENTE", font, drawBrush, datosfooter2, centrado);
                datosfooter1.Y += espaciado * 2;
                datosfooter2.Y += espaciado * 2;

                Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));

                datosfooter1.Y += 40;
                Rectangle posImgCodigoTicket = new Rectangle(0, datosfooter1.Y, 400, 120);
                e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 380.0F, 120.0F, units);

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
        //  Impresion de ticket de complemento 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ImprimeTicketComplemento(Ventas venta)
        {
            Notificacion<Ventas> notificacion;
            try
            {
                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;

                this.idVenta = venta.idVenta;
                this.idComplemento = venta.idComplemento;

                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";

                PaperSize ps = new PaperSize("", 285, 540);
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPageComplementos);
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

        void pd_PrintPageComplementos(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
                notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = idVenta, idComplemento = idComplemento, tipoVenta = EnumTipoVenta.AgregarProductosVenta });

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
                e.Graphics.DrawString("*******************************************************************" + "\n" + "*************** TICKET DE COMPLEMENTO ***************" + '\n' + "*******************************************************************", font, drawBrush, datos, centrado);

                e.Graphics.DrawString("Ticket:" + notificacion.Modelo[0].idVenta.ToString(), font, drawBrush, 40, 181, izquierda);
                e.Graphics.DrawString("Fecha:" +  notificacion.Modelo[0].fechaAlta.ToString("dd-MM-yyyy"), font, drawBrush, 150, 181, izquierda);
                e.Graphics.DrawString("Hora:" +   notificacion.Modelo[0].fechaAlta.ToShortTimeString(), font, drawBrush, 150, 191, izquierda);

                Rectangle datosProducto = new Rectangle(5, 285, 145, 82);
                Rectangle datosCantidad = new Rectangle(150, 285, 30, 82);
                Rectangle datosPrecioU = new Rectangle(190, 285, 30, 82);
                Rectangle datosPrecio = new Rectangle(220, 285, 48, 82);

                Rectangle datosEnca = new Rectangle(0, 215, 280, 82);

                e.Graphics.DrawString("  Cliente: " + notificacion.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;
                e.Graphics.DrawString("  Descripcion                                 Art.          Precio       Precio" + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 9;
                e.Graphics.DrawString("                                                 Agregados    Unitario       " + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 6;
                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                //datosEnca.Y += 14;

                float monto = 0;
                float montoComisionBancaria = 0;

                for (int i = 0; i < notificacion.Modelo.Count(); i++)
                {
                    e.Graphics.DrawString(notificacion.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[i].precioVenta.ToString() + " \n", font, drawBrush, datosPrecioU, izquierda);
                    e.Graphics.DrawString((notificacion.Modelo[i].monto + notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);

                    monto += notificacion.Modelo[i].monto;
                    montoComisionBancaria += notificacion.Modelo[i].montoComisionBancaria;

                    if (notificacion.Modelo[i].descProducto.ToString().Length >= 23)
                    {
                        datosProducto.Y += espaciado + 10;
                        datosCantidad.Y += espaciado + 10;
                        datosPrecioU.Y += espaciado + 10;
                        datosPrecio.Y += espaciado + 10;
                    }
                    else
                    {
                        datosProducto.Y += espaciado;
                        datosCantidad.Y += espaciado;
                        datosPrecioU.Y += espaciado;
                        datosPrecio.Y += espaciado;
                    }

                }



                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 280, 82);
                e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                if (montoComisionBancaria > 0)
                {
                    e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }


                e.Graphics.DrawString("  TOTAL COMPLEMENTO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString((monto + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado * 2;

                
                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
                e.Graphics.DrawString("___________________________________________________", font, drawBrush, datosfooter2, centrado);
                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;
                e.Graphics.DrawString("FIRMA DEL CLIENTE", font, drawBrush, datosfooter2, centrado);
                datosfooter1.Y += espaciado * 2;
                datosfooter2.Y += espaciado * 2;

                Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));

                datosfooter1.Y += 40;
                Rectangle posImgCodigoTicket = new Rectangle(0, datosfooter1.Y, 400, 120);
                e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 380.0F, 120.0F, units);

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
        //  Impresion de ticket despachadores
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult ImprimeTicketDespachadores(Ventas venta)
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
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPageDespachadores);
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

        void pd_PrintPageDespachadores(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
                notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = this.idVenta, tipoVenta = EnumTipoVenta.ProductosLiquidos });

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
                e.Graphics.DrawString("*******************************************************************" + "\n" + "TICKET PARA DESPACHADORES" + '\n' + "*******************************************************************", font, drawBrush, datos, centrado);

                e.Graphics.DrawString("Ticket:" + notificacion.Modelo[0].idVenta.ToString(), font, drawBrush, 40, 181, izquierda);
                e.Graphics.DrawString("Fecha:" + notificacion.Modelo[0].fechaAlta.ToString("dd-MM-yyyy"), font, drawBrush, 150, 181, izquierda);
                e.Graphics.DrawString("Hora:" + notificacion.Modelo[0].fechaAlta.ToShortTimeString(), font, drawBrush, 150, 191, izquierda);

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

                for (int i = 0; i < notificacion.Modelo.Count(); i++)
                {
                    e.Graphics.DrawString(notificacion.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString((notificacion.Modelo[i].monto + notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);

                    monto += notificacion.Modelo[i].monto;
                    montoIVA += notificacion.Modelo[i].montoIVA;


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

                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
                e.Graphics.DrawString("********  GRACIAS POR SU PREFERENCIA.  ********", font, drawBrush, datosfooter2, centrado);
                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;

                //Se pinta codigo de barras en ticket
                Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));

                datosfooter1.Y += 40;
                Rectangle posImgCodigoTicket = new Rectangle(0, datosfooter1.Y, 400, 120);
                e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 380.0F, 120.0F, units);

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
        //  Ingresos de Efectivo
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        public ActionResult IngresoEfectivo(float montoIngresoEfectivo, int idTipoIngresoEfectivo)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Result result = new VentasDAO().IngresoEfectivo(UsuarioActual.idUsuario, montoIngresoEfectivo, idTipoIngresoEfectivo);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ValidaAperturaCajas()
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Result result = new VentasDAO().ValidaAperturaCajas(UsuarioActual.idUsuario);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _IngresoEfectivo(int idTipoIngresoEfectivo)
        {
            try
            {
                ViewBag.idTipoIngresoEfectivo = idTipoIngresoEfectivo;
                return PartialView();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult AperturaCajas()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult ImprimeTicketIngresosEfectivo(int idIngresoEfectivo)
        {
            Notificacion<Retiros> notificacion;
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                notificacion = new Notificacion<Retiros>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;
                PrintDocument pd = new PrintDocument();
                //string nombreImpresora = string.Empty;
                //foreach (String strPrinter in PrinterSettings.InstalledPrinters)
                //{
                //    if (strPrinter.Contains("PDF"))
                //    {
                //        nombreImpresora = strPrinter;
                //    }
                //}
                //pd.PrinterSettings = new PrinterSettings
                //{
                //    PrinterName = nombreImpresora, //"Microsoft XPS Document Writer",
                //    PrintToFile = true,
                //    PrintFileName = System.Web.HttpContext.Current.Server.MapPath("~") + "\\Tickets\\" + "IngresoEfectivo" + "_preview.pdf"
                //};
                pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                this.idIngresoEfectivo = idIngresoEfectivo;
                PaperSize ps = new PaperSize("", 285, 540);
                pd.PrintPage += new PrintPageEventHandler(pd_PrintPageIngresoEfectivo);
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


        void pd_PrintPageIngresoEfectivo(object sender, PrintPageEventArgs e)
        {
            Notificacion<List<IngresoEfectivo>> notificacion = new Notificacion<List<IngresoEfectivo>>();
            try
            {
                string titulo = string.Empty;
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                IngresoEfectivo ingresoEfectivo = new IngresoEfectivo();
                ingresoEfectivo.idIngreso = idIngresoEfectivo;
                notificacion = new VentasDAO().ConsultaIngresosEfectivo(ingresoEfectivo);
                ingresoEfectivo = notificacion.Modelo[0];

                if ((EnumTipoIngresoEfectivo)ingresoEfectivo.idTipoIngreso == EnumTipoIngresoEfectivo.AperturaCajas)
                {
                    titulo = "\nCOMPROBANTE DE " + "\n" + "APERTURA DE CAJAS";
                }
                else
                {
                    titulo = "\nCOMPROBANTE DE " + "\n" + "INGRESO DE EFECTIVO";
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
                e.Graphics.DrawString(ingresoEfectivo.nombreUsuario.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (ingresoEfectivo.nombreUsuario.ToString().Length >= 27)
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
                e.Graphics.DrawString(ingresoEfectivo.fechaAlta.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (ingresoEfectivo.nombreUsuario.ToString().Length >= 27)
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
                e.Graphics.DrawString(ingresoEfectivo.Sucursal.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (ingresoEfectivo.nombreUsuario.ToString().Length >= 27)
                {
                    datosDescripcion.Y += espaciado + 10;
                    datosTikcet.Y += espaciado + 10;
                }
                else
                {
                    datosDescripcion.Y += espaciado;
                    datosTikcet.Y += espaciado;
                }

                e.Graphics.DrawString("Almacen: ", font, drawBrush, datosDescripcion, izquierda);
                e.Graphics.DrawString(ingresoEfectivo.Almacen.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (ingresoEfectivo.nombreUsuario.ToString().Length >= 27)
                {
                    datosDescripcion.Y += espaciado + 10;
                    datosTikcet.Y += espaciado + 10;
                }
                else
                {
                    datosDescripcion.Y += espaciado;
                    datosTikcet.Y += espaciado;
                }

                //e.Graphics.DrawString("Estación: ", font, drawBrush, datosDescripcion, izquierda);
                //e.Graphics.DrawString(notificacion.Modelo[0].nombreEstacion.ToString(), font, drawBrush, datosTikcet, izquierda);

                //if (notificacion.Modelo[0].nombreUsuario.ToString().Length >= 27)
                //{
                //    datosDescripcion.Y += espaciado + 10;
                //    datosTikcet.Y += espaciado + 10;
                //}
                //else
                //{
                //    datosDescripcion.Y += espaciado;
                //    datosTikcet.Y += espaciado;
                //}


                e.Graphics.DrawString("Monto: ", font, drawBrush, datosDescripcion, izquierda);
                e.Graphics.DrawString("" + notificacion.Modelo[0].monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, datosTikcet, izquierda);

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


                //e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                //datosfooter1.Y += espaciado;

                //Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 5, 280, 82);

                //e.Graphics.DrawString("  AUTORIZO  ", font, drawBrush, datosfooter2, centrado);
                //datosfooter1.Y += espaciado;
                //datosfooter2.Y += espaciado;

                // para mas espaciado al final del ticket
                //e.Graphics.DrawString("-", font, drawBrush, 0, datosfooter2.Y + 30, centrado);
                //datosfooter1.Y += espaciado;
                //datosfooter2.Y += espaciado;
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

        public ActionResult _ExcesoEfectivo()
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                List<ExcesoEfectivo> excesoEfectivos = new VentasDAO().excesoEfectivo((usuario.idRol == 1 ? 0 : usuario.idUsuario));
                return PartialView(excesoEfectivos);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult _ObtenerDetalleTickets(Ventas ventas)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];

                Notificacion<List<Ticket>> devoluciones = new Notificacion<List<Ticket>>();
                Notificacion<List<Ticket>> complementos = new Notificacion<List<Ticket>>();
                int idVenta = ventas.idVenta;

                devoluciones = new lluviaBackEnd.DAO.VentasDAO().ObtenerTicketsDevolucionComplemento(ventas.idVenta, TipoTicket.Devolucion);
                complementos = new lluviaBackEnd.DAO.VentasDAO().ObtenerTicketsDevolucionComplemento(ventas.idVenta, TipoTicket.Complemento);

                ViewBag.devoluciones = devoluciones;
                ViewBag.complementos = complementos;
                ViewBag.idVenta = idVenta;

                return PartialView("_ObtenerDetalleTickets");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





    }


}
