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
using System.Threading;
using System.IO;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class PedidosEspecialesV2Controller : Controller
    {
        Int64 idPedidoEspecial = 0;

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

                        return View(pedidoEspecial);
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


        public ActionResult _ObtenerEntregarPedidos(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
                //pedidosEspecialesV2.idPedidoEspecial = 0;
                //pedidosEspecialesV2.fechaIni = new DateTime(1900, 01, 01);
                //pedidosEspecialesV2.fechaFin = new DateTime(1900, 01, 01);

                notificacion = new PedidosEspecialesV2DAO().ConsultaPedidosEspeciales(pedidosEspecialesV2);

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

        [HttpPost]
        public ActionResult ObtenerProductosPedidoEspecial(int idPedidoEspecial)
        {
            try
            {
                List<Producto> productosPedidoEspecial = new PedidosEspecialesV2DAO().ConsultaPedidoEspecialDetalle(idPedidoEspecial); ;
                return Json(productosPedidoEspecial, JsonRequestBehavior.AllowGet);
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
        public ActionResult PedidosEnRuta(PedidosEspecialesV2 pedidoEspecial)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0, 0, 5);
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult _ObtenerPedidosEnRuta(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
                notificacion = new PedidosEspecialesV2DAO().ConsultaPedidosEnRuta(pedidosEspecialesV2);
                ViewBag.lstPedidos = notificacion.Modelo;
                ViewBag.mensaje = notificacion.Mensaje;

                return PartialView("_ObtenerPedidosEnRuta");

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
                pedidoEspecial.lstProductos = new List<Producto>();
                pedidoEspecial.lstProductos = new PedidosEspecialesV2DAO().ConsultaPedidoEspecialDetalle(pedidoEspecial.idPedidoEspecial);

                List<SelectListItem> listUsuariosRuteo = new SelectList(new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = 5 }), "idUsuario", "nombreCompleto").ToList();
                ViewBag.listUsuariosRuteo = listUsuariosRuteo;

                List<SelectListItem> listUsuariosTaxi = new SelectList(new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = 10 }), "idUsuario", "nombreCompleto").ToList();
                ViewBag.listUsuariosTaxi = listUsuariosTaxi;

                Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
                formasPago = new VentasDAO().ObtenerFormasPago();
                ViewBag.lstFormasPago = formasPago.Modelo;

                Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
                usoCFDI = new VentasDAO().ObtenerUsoCFDI();
                ViewBag.lstUsoCFDI = usoCFDI.Modelo;

                ViewBag.lstClientes = new ClienteDAO().ObtenerClientes(new Cliente() { idCliente = 0 });

                List<Cliente> cliente = new List<Cliente>();
                cliente = new ClienteDAO().ObtenerClientes(new Cliente { idCliente = pedidoEspecial.idCliente });
                ViewBag.cliente = cliente[0];

                ViewBag.comisionBancaria = usuario.comisionBancaria;
                ViewBag.pedidoEspecial = pedidoEspecial;

                ViewBag.cajaAbierta = new PedidosEspecialesV2DAO().ValidaAperturaCajas(usuario.idUsuario).Estatus == 200 ? true : false;


                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarConfirmacion(List<Producto> productos, int idPedidoEspecial, int idEstatusPedidoEspecial, int idUsuarioEntrega, string numeroUnidadTaxi,
                                                int idEstatusCuentaPorCobrar, float montoPagado, bool aCredito, bool aCreditoConAbono,
                                                int aplicaIVA, string idMetodoPago, int idFactFormaPago, int idFactUsoCFDI, string observacionesPedidoRuta, int idUsuarioRuteo)
        {
            try
            {
                Notificacion<PedidosEspecialesV2> result = new Notificacion<PedidosEspecialesV2>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                idUsuarioEntrega = UsuarioActual.idUsuario;
                result = new PedidosEspecialesV2DAO().GuardarConfirmacion(productos, idPedidoEspecial, idEstatusPedidoEspecial, idUsuarioEntrega, numeroUnidadTaxi, idEstatusCuentaPorCobrar,
                                                                          montoPagado, aCredito, aCreditoConAbono, aplicaIVA, idFactFormaPago, idFactUsoCFDI, observacionesPedidoRuta, idUsuarioRuteo);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public ActionResult GuardarPedidoEspecial(List<Producto> productos, int tipoRevision, int idCliente, int idEstatusPedidoEspecial, int idPedidoEspecial, int idPedidoEspecialMayoreo_)
        {
            try
            {
                Notificacion<PedidosEspecialesV2> result = new Notificacion<PedidosEspecialesV2>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                result = new PedidosEspecialesV2DAO().GuardarPedidoEspecial(productos, tipoRevision, idCliente, UsuarioActual.idUsuario, idEstatusPedidoEspecial, UsuarioActual.idEstacion, idPedidoEspecial, idPedidoEspecialMayoreo_);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult CancelarrPedidoEspecial(int idPedidoEspecial)
        {
            try
            {
                Notificacion<PedidosEspecialesV2> result = new Notificacion<PedidosEspecialesV2>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                result = new PedidosEspecialesV2DAO().CancelarPedidoEspecial(idPedidoEspecial);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //[HttpPost]
        public ActionResult ConsultaExistenciasAlmacen(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new PedidosEspecialesV2DAO().ConsultaExistenciasAlmacen(producto);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult ConsultaDatosTicketPedidoEspecialV2(int idPedidoEspecial)
        {
            try
            {

                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
                //PedidosEspecialesV2 pedido = new PedidosEspecialesV2();

                notificacion = new PedidosEspecialesV2DAO().ObtenerEntregarPedidos(new PedidosEspecialesV2() { idPedidoEspecial = idPedidoEspecial, fechaIni = new DateTime(1900, 01, 01), fechaFin = new DateTime(1900, 01, 01) });
                //pedido = notificacion;
                return Json(notificacion, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarIVAPedido(PedidosEspecialesV2 pedido)
        {
            try
            {
                Notificacion<PedidosEspecialesV2> notificacion = new Notificacion<PedidosEspecialesV2>();
                notificacion = new PedidosEspecialesV2DAO().GuardarIVAPedido(pedido);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /************** REIMPRIMIR TICKETS ALMACENES********************/


        int indiceProducto = 0;
        int paginaActual = 0;
        int productosporPagina = 30;
        int paginas = 0;
        int indexProducto = 0;
        Boolean control = false;

        [HttpPost]
        public JsonResult imprimirTicketAlmacenes(Int64 idPedidoEspecial, Int16 copias = 0)
        {
            try
            {
                Notificacion<List<dynamic>> result = new Notificacion<List<dynamic>>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                result = new PedidosEspecialesV2DAO().consultaTicketPedidoEspecial(idPedidoEspecial);
                var response = ImprimirTicketAlamacenes(result, copias);
                VerTicketAlmacenes(idPedidoEspecial);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Notificacion<String> ImprimirTicketAlamacenes(dynamic listPedidosEspeciales, Int16 copias)
        {
            Notificacion<String> notificacion;
            List<dynamic> totalProductos = new List<dynamic>();
            try
            {
                if (copias == 0)
                {
                    copias = 1;
                }

                notificacion = new Notificacion<String>();
                notificacion.Mensaje = "Imprimiedo tickets por almacen.";
                notificacion.Estatus = 200;
                //PrintDocument pd = new PrintDocument();
                for (int i = 0; i < copias; i++)
                {
                    foreach (var pedidoEspecial in listPedidosEspeciales.Modelo)
                    {
                        using (PrintDocument pd = new PrintDocument())
                        {
                            pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString();
                            //pd.PrinterSettings.Copies = copias;
                            //Notificacion<List<Ticket>> _notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = this.idVenta });
                            //PaperSize ps = new PaperSize("", 285, 540);
                            pd.PrintPage += (_sender, args) => pd_PrintPage(null, args, pedidoEspecial);
                            pd.PrintController = new StandardPrintController();
                            pd.DefaultPageSettings.Margins.Left = 10;
                            pd.DefaultPageSettings.Margins.Right = 0;
                            pd.DefaultPageSettings.Margins.Top = 0;
                            pd.DefaultPageSettings.Margins.Bottom = 0;
                            //pd.DefaultPageSettings.PaperSize = ps;
                            pd.Print();
                            pd.Dispose();
                            this.indexProducto = 0;
                            this.paginaActual = 0;
                        }
                        Thread.Sleep(100);
                        totalProductos.Add(pedidoEspecial);
                    }
                    Thread.Sleep(100);
                }


                return notificacion;
            }
            catch (InvalidPrinterException ex)
            {
                notificacion = new Notificacion<String>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return notificacion;
            }
            catch (Exception ex)
            {
                notificacion = new Notificacion<String>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                return notificacion;
            }

        }

        void pd_PrintPage(object sender, PrintPageEventArgs e, dynamic notificacion)
        {
            try
            {


                int ancho = 258;
                int espaciado = 14;
                Rectangle datosIndex = new Rectangle(2, 285, 15, 82);
                Rectangle datosProducto = new Rectangle(20, 285, 145, 82);
                Rectangle datosCantidad = new Rectangle(167, 285, 30, 82);
                Rectangle datosPrecioU = new Rectangle(205, 285, 30, 82);
                Rectangle datosPrecio = new Rectangle(235, 285, 48, 82);

                // TOTALES
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
                int marginLeft = 2;
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

                    postTicketY = (logo.Y + logo.Height + espaciado);


                    //DATOS FISCALES y DOMICILIO;

                    Rectangle datos = new Rectangle(5, postTicketY, ancho, 82);
                    //e.Graphics.DrawString("RFC:" + "GACL7905178F2" + ",\n" + "Calle Macarena #82" + '\n' + "Inguambo" + '\n' + "Uruapan, Michoacán" + '\n' + "C.p. 58000", font, drawBrush, datos, centrado);
                    //postTicketY = datos.Y + datos.Height + espaciado;

                    postTicketY += espaciado;
                    e.Graphics.DrawString("Ticket:" + notificacion[0].idPedidoEspecial.ToString(), font, drawBrush, marginLeft, postTicketY, izquierda);
                    postTicketY += espaciado;
                    e.Graphics.DrawString("Fecha:" + notificacion[0].fechaAlta.ToString("dd-MM-yyyy"), font, drawBrush, marginLeft, postTicketY, izquierda);
                    postTicketY += espaciado;
                    e.Graphics.DrawString("Hora:" + notificacion[0].fechaAlta.ToShortTimeString(), font, drawBrush, marginLeft, postTicketY, izquierda);
                    postTicketY += espaciado;


                    Rectangle datosEnca = new Rectangle(0, postTicketY, 295, 82);
                    e.Graphics.DrawString("____________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString(" TICKET PARA DESPACHADORES " + " \n", font, drawBrush, datosEnca, centrado);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("____________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;

                    e.Graphics.DrawString("  Almacen Enviado: " + notificacion[0].descAlmacen.ToString() + " \n", Bold, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;

                    e.Graphics.DrawString("  Cliente: " + notificacion[0].nombre.ToString() + " \n", Bold, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;

                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("    Descripcion                                                        Cantidad       " + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 9;
                    //e.Graphics.DrawString("                                                                          Unitario       " + " \n", font, drawBrush, datosEnca, izquierda);
                    //datosEnca.Y += 6;
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    //datosIndex = new Rectangle(2, datosEnca.Y, 15, 85);
                    datosProducto = new Rectangle(2, datosEnca.Y, 200, 85);
                    datosCantidad = new Rectangle(220, datosEnca.Y, 30, 85);

                }
                else
                {
                    //datosIndex = new Rectangle(2, 15, 15, 85);
                    datosProducto = new Rectangle(2, 15, 200, 85);
                    datosCantidad = new Rectangle(220, 15, 30, 85);
                }

                for (int i = indexProducto; i < ((List<dynamic>)notificacion).ToList().Count; i++)
                {
                    //e.Graphics.DrawString((indexProducto + 1).ToString() + " \n", font, drawBrush, datosIndex, izquierda);
                    e.Graphics.DrawString(notificacion[i].descripcion.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(notificacion[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    datosIndex.Y += espaciado + 5;
                    datosProducto.Y += espaciado + 5;
                    datosCantidad.Y += espaciado + 5;
                    indexProducto++;
                    if (datosProducto.Y >= 1092)
                    {
                        this.paginaActual++;
                        e.HasMorePages = true;
                        return;
                    }
                }
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


        public ActionResult VerTicketAlmacenes(Int64 idPedidoEspecial)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                var result = new PedidosEspecialesV2DAO().consultaTicketPedidoEspecial(idPedidoEspecial);
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";
                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTicketDespachadoresPDF(result));
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket para despachadores: " + idPedidoEspecial.ToString(); ;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult VerTodosTicketsPedidoEspecial(int idPedidoEspecial)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                // tickets de almacenes
                Notificacion<List<dynamic>> almacenes = new Notificacion<List<dynamic>>();
                almacenes = new PedidosEspecialesV2DAO().consultaTicketPedidoEspecial(idPedidoEspecial);

                // consultar cantidad tickets normales y devoluciones
                List<Notificacion<dynamic>> normales = new List<Notificacion<dynamic>>();
                List<Notificacion<dynamic>> devoluciones = new List<Notificacion<dynamic>>();
                Notificacion<dynamic> numTicketsNormalesDevoluciones = new Notificacion<dynamic>();
                numTicketsNormalesDevoluciones = new PedidosEspecialesV2DAO().ObtieneTicketsPedidoEspecial(idPedidoEspecial);

                for (int i = 0; i < numTicketsNormalesDevoluciones.Modelo.Count; i++)
                {
                    Notificacion<dynamic> ticket = new PedidosEspecialesV2DAO().ObtieneDetalleTicketPedidoEspecial(idPedidoEspecial, numTicketsNormalesDevoluciones.Modelo[i].idTipoTicketPedidoEspecial, 0, true);

                    if (numTicketsNormalesDevoluciones.Modelo[i].idTipoTicketPedidoEspecial == 1) // normal
                    {
                        normales.Add(ticket);
                    }
                    if (numTicketsNormalesDevoluciones.Modelo[i].idTipoTicketPedidoEspecial == 2) // devolucion
                    {
                        devoluciones.Add(ticket);
                    }
                }

                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ticket generado correctamente.";
                string pdfCodigos = Convert.ToBase64String(Utilerias.Utils.GeneraTodosTicketsPDFPedidosEspeciales(idPedidoEspecial, almacenes, normales, devoluciones));
                ViewBag.pdfBase64 = pdfCodigos;
                ViewBag.title = "Ticket: " + idPedidoEspecial.ToString();
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /************** REIMPRIMIR TICKET  GENERAL********************/
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

                Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
                formasPago = new VentasDAO().ObtenerFormasPago();
                ViewBag.lstFormasPago = formasPago.Modelo;

                Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
                usoCFDI = new VentasDAO().ObtenerUsoCFDI();
                ViewBag.lstUsoCFDI = usoCFDI.Modelo;

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



        //        [HttpPost]
        //        public ActionResult ImprimeTicketPedidoEspecial(PedidosEspecialesV2 pedido)
        //        {
        //            Notificacion<Ventas> notificacion;
        //            try
        //            {

        //                notificacion = new Notificacion<Ventas>();
        //                notificacion.Mensaje = "Se envio el ticket a la impresora.";
        //                notificacion.Estatus = 200;

        //                this.idPedidoEspecial = pedido.idPedidoEspecial;

        //                using (PrintDocument pd = new PrintDocument())
        //                {
        //                    if (pedido.ticketVistaPrevia)
        //                    {
        //                        notificacion.Mensaje = "Abriendo Ticket.";
        //                        string nombreImpresora = string.Empty;
        //                        foreach (String strPrinter in PrinterSettings.InstalledPrinters)
        //                        {
        //                            if (strPrinter.Contains("PDF"))
        //                            {
        //                                nombreImpresora = strPrinter;
        //                            }
        //                        }

        //                        if (nombreImpresora == string.Empty)
        //                        {
        //                            notificacion.Mensaje = "No se encontro impresora PDF para previsualizar ticket.";
        //                            notificacion.Estatus = -1;
        //                            pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
        //                        }
        //                        else
        //                        {
        //                            pd.PrinterSettings = new PrinterSettings
        //                            {
        //                                PrinterName = nombreImpresora, //"Microsoft XPS Document Writer",
        //                                PrintToFile = true,
        //                                PrintFileName = System.Web.HttpContext.Current.Server.MapPath("~") + "\\Tickets\\" + pedido.idPedidoEspecial.ToString() + "_preview.pdf"
        //                            };
        //                        }
        //                    }
        //                    else
        //                    {
        //                        pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
        //                    }
        //                    Notificacion<List<Ticket>> _notificacion = new PedidosEspecialesV2DAO().ObtenerTicketsPedidoEspecialV2(new Ticket() { idPedidoEspecial = this.idPedidoEspecial });
        //                    pd.PrintPage += (_sender, args) => pd_PrintPage(null, args, _notificacion);
        //                    pd.PrintController = new StandardPrintController();
        //                    pd.DefaultPageSettings.Margins.Left = 10;
        //                    pd.DefaultPageSettings.Margins.Right = 0;
        //                    pd.DefaultPageSettings.Margins.Top = 0;
        //                    pd.DefaultPageSettings.Margins.Bottom = 0;
        //                    pd.Print();
        //                    pd.Dispose();
        //                }

        //                return Json(notificacion, JsonRequestBehavior.AllowGet);

        //            }
        //            catch (InvalidPrinterException ex)
        //            {
        //                notificacion = new Notificacion<Ventas>();
        //                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //                notificacion.Estatus = -1;
        //                return Json(notificacion, JsonRequestBehavior.AllowGet);
        //            }
        //            catch (Exception ex)
        //            {
        //                notificacion = new Notificacion<Ventas>();
        //                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //                notificacion.Estatus = -1;
        //                return Json(notificacion, JsonRequestBehavior.AllowGet);
        //            }

        //        }

        //        void pd_PrintPage(object sender, PrintPageEventArgs e, Notificacion<List<Ticket>> notificacion)
        //        {
        //            try
        //            {
        //                List<Ticket> lstTickets = notificacion.Modelo;
        //                if (this.paginaActual < _lstTickets.Count())
        //                    lstTickets = _lstTickets[this.paginaActual];
        //                float monto = 0;
        //                float montoIVA = 0;
        //                float montoComisionBancaria = 0;
        //                float montoAhorro = 0;
        //                float montoPagado = 0;
        //                float suCambio = 0;

        //                float montoPagadoAgregarProductos = 0;
        //                float montoAgregarProductos = 0;
        //                float suCambioAgregarProductos = 0;
        //                float cantidadTotalDeArticulos = 0;

        //                int ancho = 258;
        //                int espaciado = 14;
        //                Rectangle datosIndex = new Rectangle(2, 285, 15, 82);
        //                Rectangle datosProducto = new Rectangle(20, 285, 145, 82);
        //                Rectangle datosCantidad = new Rectangle(167, 285, 30, 82);
        //                Rectangle datosPrecioU = new Rectangle(205, 285, 30, 82);
        //                Rectangle datosPrecio = new Rectangle(235, 285, 48, 82);

        //                TOTALES
        //               montoPagadoAgregarProductos = notificacion.Modelo[0].montoPagadoAgregarProductos;
        //                montoAgregarProductos = notificacion.Modelo[0].montoAgregarProductos;
        //                Configuración Global
        //                GraphicsUnit units = GraphicsUnit.Pixel;
        //                e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
        //                e.Graphics.InterpolationMode = InterpolationMode.High;
        //                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

        //                Configuración Texto
        //                StringFormat centrado = new StringFormat();
        //                centrado.Alignment = StringAlignment.Center;//Cetrado
        //                StringFormat izquierda = new StringFormat();
        //                izquierda.Alignment = StringAlignment.Near; //Izquierda
        //                StringFormat derecha = new StringFormat();
        //                derecha.Alignment = StringAlignment.Far; //Izquierda

        //                Tipo y tamaño de letra
        //                Font font = new Font("Arial", 6.8F, FontStyle.Regular, GraphicsUnit.Point);
        //                Font Bold = new Font("Arial", 6.8F, FontStyle.Bold, GraphicsUnit.Point);
        //                Font BoldWester = new Font("Arial", 13, FontStyle.Bold, GraphicsUnit.Point);

        //                Color de texto
        //                SolidBrush drawBrush = new SolidBrush(Color.Black);
        //                Logos
        //                if (paginaActual == 0)
        //                {
        //                    Se pinta logo
        //                    Se pinta logo
        //                    int poslogoY = 15;
        //                    int postTicketY = 0;
        //                    Image newImage = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\assets\\img\\logo_lluvia_150.jpg");
        //                    Rectangle logo = new Rectangle(80, poslogoY, 280, 81);
        //                    e.Graphics.DrawImage(newImage, logo, 0, 0, 380.0F, 120.0F, units);

        //                    postTicketY = (logo.Y + logo.Height + espaciado);


        //                    postTicketY = postTicketY + 100 + espaciado;
        //                    Rectangle datos = new Rectangle(5, postTicketY, ancho, 82);
        //                    e.Graphics.DrawString("RFC:" + "COVO781128LJ1" + ",\n" + "Calle Macarena #82" + '\n' + "Inguambo" + '\n' + "Uruapan, Michoacán" + '\n' + "C.p. 58000", font, drawBrush, datos, centrado);

        //                    e.Graphics.DrawString("Ticket: PE" + notificacion.Modelo[0].idPedidoEspecial.ToString(), font, drawBrush, 40, 181, izquierda);
        //                    e.Graphics.DrawString("Fecha:" + notificacion.Modelo[0].fechaAlta.ToString("dd-MM-yyyy"), font, drawBrush, 150, 181, izquierda);
        //                    e.Graphics.DrawString("Hora:" + notificacion.Modelo[0].fechaAlta.ToShortTimeString(), font, drawBrush, 150, 191, izquierda);

        //                    postTicketY = datos.Y + datos.Height + espaciado;
        //                    Rectangle datosEnca = new Rectangle(0, postTicketY, 295, 82);

        //                    e.Graphics.DrawString("  Cliente: " + notificacion.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
        //                    datosEnca.Y += 14;
        //                    e.Graphics.DrawString("  Forma de Pago: " + notificacion.Modelo[0].descFormaPago.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
        //                    datosEnca.Y += 14;

        //                    e.Graphics.DrawString("______________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
        //                    datosEnca.Y += 14;
        //                    e.Graphics.DrawString("#    Descripcion                              Cantidad     Precio       Precio" + " \n", font, drawBrush, datosEnca, izquierda);
        //                    datosEnca.Y += 9;
        //                    e.Graphics.DrawString("                                                                          Unitario       " + " \n", font, drawBrush, datosEnca, izquierda);
        //                    datosEnca.Y += 6;
        //                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
        //                    datosEnca.Y += 14;
        //                    datosIndex = new Rectangle(2, datosEnca.Y, 15, 82);
        //                    datosProducto = new Rectangle(20, datosEnca.Y, 145, 82);
        //                    datosCantidad = new Rectangle(167, datosEnca.Y, 30, 82);
        //                    datosPrecioU = new Rectangle(205, datosEnca.Y, 30, 82);
        //                    datosPrecio = new Rectangle(235, datosEnca.Y, 48, 82);


        //                }
        //                else
        //                {
        //                    datosIndex = new Rectangle(2, 15, 15, 82);
        //                    datosProducto = new Rectangle(20, 15, 145, 82);
        //                    datosCantidad = new Rectangle(177, 15, 30, 82);
        //                    datosPrecioU = new Rectangle(205, 15, 30, 82);
        //                    datosPrecio = new Rectangle(235, 15, 48, 82);
        //                }

        //                for (int i = indexProducto; i < lstTickets.Count(); i++)
        //                {
        //                    e.Graphics.DrawString((indexProducto + 1).ToString() + " \n", font, drawBrush, datosIndex, izquierda);
        //                    e.Graphics.DrawString(lstTickets[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
        //                    e.Graphics.DrawString(lstTickets[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
        //                    e.Graphics.DrawString(lstTickets[i].precioVenta.ToString() + " \n", font, drawBrush, datosPrecioU, izquierda);
        //                    e.Graphics.DrawString((lstTickets[i].monto + lstTickets[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);


        //                    /*monto += lstTickets[i].monto;
        //                    montoIVA += lstTickets[i].montoIVA;
        //                    montoComisionBancaria += lstTickets[i].montoComisionBancaria;
        //                    montoAhorro += lstTickets[i].ahorro;
        //                    */
        //                    if (lstTickets[i].descProducto.ToString().Length >= 23)
        //                    {
        //                        datosIndex.Y += espaciado + 10;
        //                        datosProducto.Y += espaciado + 10;
        //                        datosCantidad.Y += espaciado + 10;
        //                        datosPrecioU.Y += espaciado + 10;
        //                        datosPrecio.Y += espaciado + 10;
        //                    }
        //                    else
        //                    {

        //                        datosIndex.Y += espaciado;
        //                        datosProducto.Y += espaciado;
        //                        datosCantidad.Y += espaciado;
        //                        datosPrecioU.Y += espaciado;
        //                        datosPrecio.Y += espaciado;
        //                    }

        //                    si hay descuentos por mayoreo o rango de precios
        //                    if (lstTickets[i].ahorro > 0)
        //                    {
        //                        e.Graphics.DrawString("     └Descuento por mayoreo" + " \n", font, drawBrush, datosProducto, izquierda);
        //                        e.Graphics.DrawString("-" + (lstTickets[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
        //                        datosProducto.Y += espaciado;
        //                        datosCantidad.Y += espaciado;
        //                        datosPrecioU.Y += espaciado;
        //                        datosPrecio.Y += espaciado;
        //                        datosIndex.Y += espaciado;
        //                    }


        //                    Console.WriteLine("indexProducto: " + indexProducto);
        //                    indexProducto++;
        //                    if (datosProducto.Y >= 1092)
        //                    {
        //                        this.paginaActual++;
        //                        e.HasMorePages = true;
        //                        return;
        //                    }

        //                }

        //                monto = notificacion.Modelo.Sum(x => x.monto);
        //                montoIVA = notificacion.Modelo.Sum(x => x.montoIVA);
        //                montoComisionBancaria = notificacion.Modelo.Sum(x => x.montoComisionBancaria);
        //                montoAhorro = notificacion.Modelo.Sum(x => x.ahorro);
        //                cantidadTotalDeArticulos = notificacion.Modelo.Sum(x => x.cantidad);
        //                int posXFooter = 285;
        //                if (montoAgregarProductos > 0)
        //                {
        //                    monto -= montoAgregarProductos;
        //                }

        //                suCambioAgregarProductos = montoPagadoAgregarProductos - montoAgregarProductos;
        //                montoPagado = notificacion.Modelo[0].montoPagado;
        //                suCambio = montoPagado - monto - montoIVA - montoComisionBancaria;

        //                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);

        //                if (indexProducto == lstTickets.Count())
        //                {
        //                    datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);
        //                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
        //                    datosfooter1.Y += espaciado;
        //                    indexProducto++;

        //                }

        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                if (indexProducto == lstTickets.Count() + 1)
        //                {
        //                    e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
        //                    datosfooter1.Y += espaciado;
        //                    indexProducto++;

        //                }

        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                if (indexProducto == lstTickets.Count() + 2)
        //                {
        //                    if (montoComisionBancaria > 0)
        //                    {
        //                        e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                        e.Graphics.DrawString((montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
        //                        datosfooter1.Y += espaciado;
        //                        indexProducto++;

        //                    }
        //                }

        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                if (indexProducto == lstTickets.Count() + 3)
        //                    if (montoIVA > 0)
        //                    {
        //                        e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                        e.Graphics.DrawString((montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
        //                        datosfooter1.Y += espaciado;
        //                        indexProducto++;

        //                    }


        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                if (indexProducto == lstTickets.Count() + 4)
        //                {
        //                    e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    e.Graphics.DrawString((monto + montoIVA + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
        //                    datosfooter1.Y += espaciado;
        //                    indexProducto++;

        //                }

        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                if (indexProducto == lstTickets.Count() + 5)
        //                {
        //                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    datosfooter1.Y += espaciado;
        //                    indexProducto++;

        //                }

        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                if (indexProducto == lstTickets.Count() + 6)
        //                {
        //                    e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    e.Graphics.DrawString((montoPagado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
        //                    datosfooter1.Y += espaciado;
        //                    indexProducto++;

        //                }

        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                if (indexProducto == lstTickets.Count() + 7)
        //                {
        //                    e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    e.Graphics.DrawString((suCambio).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
        //                    datosfooter1.Y += espaciado;
        //                    indexProducto++;

        //                }
        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;

        //                if (montoAgregarProductos > 0)
        //                {

        //                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    datosfooter1.Y += espaciado;
        //                    if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                    e.Graphics.DrawString("  COMPLEMENTOS:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    datosfooter1.Y += espaciado;
        //                    if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                    e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    e.Graphics.DrawString((montoPagadoAgregarProductos).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
        //                    datosfooter1.Y += espaciado;
        //                    if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                    e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
        //                    e.Graphics.DrawString((suCambioAgregarProductos).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
        //                    datosfooter1.Y += espaciado;
        //                    if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                }
        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                if (indexProducto == lstTickets.Count() + 8)
        //                {
        //                    datosfooter1.Y += espaciado;
        //                    Rectangle totalArt = new Rectangle(5, datosfooter1.Y, ancho, 82);
        //                    e.Graphics.DrawString("  CANTIDAD DE ARTICULOS COMPRADOS: " + cantidadTotalDeArticulos.ToString(), font, drawBrush, totalArt, centrado);
        //                    datosfooter1.Y += espaciado;
        //                    indexProducto++;
        //                }


        //                if (indexProducto == lstTickets.Count() + 9)
        //                {
        //                    if (montoAhorro > 0)
        //                    {
        //                        datosfooter1.Y += espaciado;
        //                        Rectangle datosAhorro = new Rectangle(0, datosfooter1.Y, 280, 82);
        //                        e.Graphics.DrawString("******* USTED AHORRO:  " + (montoAhorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " *******", font, drawBrush, datosAhorro, centrado);
        //                        datosfooter1.Y += espaciado;
        //                        indexProducto++;

        //                    }
        //                    else
        //                    {
        //                        indexProducto++;
        //                    }
        //                }
        //                datosfooter1.Y += espaciado;
        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;

        //                if (indexProducto >= lstTickets.Count() + 10)
        //                {

        //                    Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));
        //                    e.Graphics.DrawImage(imagenCodigoTicket, -50, datosfooter1.Y, 400, 60);
        //                    datosfooter1.Y += espaciado + 30 + espaciado;
        //                    indexProducto++;
        //                }

        //                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
        //                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y, 280, 82);
        //                if (indexProducto >= lstTickets.Count() + 11)
        //                {
        //                    datosfooter1.Y += espaciado;
        //                    datosfooter2 = new Rectangle(0, datosfooter1.Y, 280, 82);
        //                    e.Graphics.DrawString("********  GRACIAS POR SU PREFERENCIA.  ********", font, drawBrush, datosfooter2, centrado);
        //                    datosfooter1.Y += espaciado;
        //                    datosfooter2.Y += espaciado;
        //                }


        //                Se pinta codigo de barras en ticket
        //                Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));

        //                datosfooter1.Y += 40;
        //                Rectangle posImgCodigoTicket = new Rectangle(0, datosfooter1.Y, 400, 120);
        //                e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 380.0F, 120.0F, units);

        //                datosfooter1.Y += espaciado;
        //                datosfooter2.Y += espaciado;

        //                para mas espaciado al final del ticket
        //                e.Graphics.DrawString("", font, drawBrush, 0, datosfooter2.Y, centrado);
        //                datosfooter1.Y += espaciado;
        //                datosfooter2.Y += espaciado;
        //                e.Graphics.DrawImage
        //                }



        //            }
        //            catch (InvalidPrinterException ex)
        //            {
        //                notificacion = new Notificacion<Ventas>();
        //                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //                notificacion.Estatus = -1;
        //                return Json(notificacion, JsonRequestBehavior.AllowGet);
        //    }
        //            catch (Exception ex)
        //            {
        //                notificacion = new Notificacion<Ventas>();
        //                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
        //                notificacion.Estatus = -1;
        //                return Json(notificacion, JsonRequestBehavior.AllowGet);
        //}

        //        }



        #region CuentasPorCobrar
        public ActionResult ConsultarCuentasPorCobrar()
        {
            try
            {
                Sesion usuario = (Sesion)Session["UsuarioActual"];
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

                VentasDAO ventasDAO = new VentasDAO();
                Notificacion<List<FormaPago>> formasPago = new Notificacion<List<FormaPago>>();
                formasPago = ventasDAO.ObtenerFormasPago();
                ViewBag.lstFormasPago = formasPago.Modelo;

                Notificacion<List<UsoCFDI>> usoCFDI = new Notificacion<List<UsoCFDI>>();
                usoCFDI = ventasDAO.ObtenerUsoCFDI();
                ViewBag.lstUsoCFDI = usoCFDI.Modelo;

                ViewBag.comisionBancaria = usuario.comisionBancaria;

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
        public ActionResult RealizarAbonoPedidosEspeciales(AbonoCliente abono)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                abono.idUsuario = UsuarioActual.idUsuario;
                Notificacion<string> resultAbono = new PedidosEspecialesV2DAO().RealizarAbonoPedidoEspecial(abono);
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
                List<Cliente> clientes = new ClienteDAO().ObtenerClientes(new Cliente() { idCliente = idCliente });
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

        [HttpPost]
        public ActionResult ImprimeTicketAbono(Int64 idAbonoCliente)
        {
            Notificacion<Ventas> notificacion;
            try
            {

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;

                int copias = 2;

                for (int i = 0; i < copias; i++)
                {

                    using (PrintDocument pd = new PrintDocument())
                    {
                        pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                        Notificacion<dynamic> _notificacion = new PedidosEspecialesV2DAO().ObtieneAbonosCliente(idAbonoCliente, 0);
                        pd.PrintPage += (_sender, args) => pd_PrintPageAbonos(null, args, _notificacion);
                        pd.PrintController = new StandardPrintController();
                        pd.DefaultPageSettings.Margins.Left = 10;
                        pd.DefaultPageSettings.Margins.Right = 0;
                        pd.DefaultPageSettings.Margins.Top = 0;
                        pd.DefaultPageSettings.Margins.Bottom = 0;
                        pd.Print();
                        pd.Dispose();
                        this.indexProducto = 0;
                        this.paginaActual = 0;
                    }

                    Thread.Sleep(100);
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

        void pd_PrintPageAbonos(object sender, PrintPageEventArgs e, Notificacion<dynamic> ticket)
        {
            try
            {
                //List<Ticket> lstTickets = notificacion.Modelo;
                //if (this.paginaActual< _lstTickets.Count())
                //    lstTickets = _lstTickets[this.paginaActual];

                int ancho = 258;
                int espaciado = 14;

                // TOTALES
                //montoPagadoAgregarProductos = notificacion.Modelo[0].montoPagadoAgregarProductos;
                //montoAgregarProductos = notificacion.Modelo[0].montoAgregarProductos;
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

                //Se pinta logo 
                //Se pinta logo 
                int poslogoY = 15;
                int postTicketY = 0;
                Image newImage = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\assets\\img\\logo_lluvia_150.jpg");
                Rectangle logo = new Rectangle(80, poslogoY, 280, 81);
                e.Graphics.DrawImage(newImage, logo, 0, 0, 380.0F, 120.0F, units);

                postTicketY = (logo.Y + logo.Height + espaciado);

                string titulo = "\nCOMPROBANTE DE ABONO" + "\n" + " PEDIDOS ESPECIALES"; ;

                //postTicketY = postTicketY + 100 + espaciado;
                Rectangle datos = new Rectangle(5, postTicketY, ancho, 82);
                e.Graphics.DrawString(titulo, Bold, drawBrush, datos, centrado);

                e.Graphics.DrawString("Ticket Abono:" + ticket.Modelo[0].idAbonoCliente.ToString(), font, drawBrush, 40, 161, izquierda);
                e.Graphics.DrawString("Fecha:" + ticket.Modelo[0].fechaAbono, font, drawBrush, 150, 161, izquierda);
                e.Graphics.DrawString("Hora:" + ticket.Modelo[0].horaAbono, font, drawBrush, 150, 171, izquierda);

                postTicketY = datos.Y + datos.Height + espaciado;
                Rectangle datosEnca = new Rectangle(0, postTicketY, 295, 82);

                e.Graphics.DrawString("  Cliente: " + ticket.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;
                e.Graphics.DrawString("  Usuario: " + ticket.Modelo[0].nombreUsuario.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;
                e.Graphics.DrawString("  Forma de Pago: " + ticket.Modelo[0].descFormaPago.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                int posXFooter = 285;

                Rectangle datosfooter1 = new Rectangle(0, datosEnca.Y, 295, 15);

                datosfooter1 = new Rectangle(0, datosEnca.Y, 295, 15);
                e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  MONTO ABONO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].monto).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].monto).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].montoComision).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                //e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                //e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].monto).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                //datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].montoTotal).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].montoPagado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].cambio).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SALDO ANTERIOR:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].saldoAntesOperacion).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  ABONO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString("-" + Convert.ToSingle(ticket.Modelo[0].monto).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SALDO ACTUAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].saldoDespuesOperacion).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                datosfooter1.Y += espaciado;


                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y, 280, 82);

                datosfooter1.Y += espaciado;
                datosfooter2 = new Rectangle(0, datosfooter1.Y, 280, 82);

                e.Graphics.DrawString("********  GRACIAS POR SU PREFERENCIA.  ********", font, drawBrush, datosfooter2, centrado);

                datosfooter1.Y += espaciado;
                datosfooter2.Y += espaciado;



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
                //e.Graphics.DrawImage
                //}



            }
            catch (InvalidPrinterException ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticket.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticket.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticket.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticket.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }

        }


        #endregion


        #region IngresoEfectivo

        [HttpPost]
        public ActionResult IngresoEfectivo(float montoIngresoEfectivo, int idTipoIngresoEfectivo)
        {
            try
            {
                Notificacion<string> notificacion = new Notificacion<string>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                notificacion = new PedidosEspecialesV2DAO().IngresoEfectivo(UsuarioActual.idUsuario, montoIngresoEfectivo, idTipoIngresoEfectivo);
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ImprimirTicketIngresoEfectivo(int idIngresoEfectivo)
        {
            Notificacion<Ventas> notificacion;
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;


                //PrintDocument pd = new PrintDocument();
                using (PrintDocument pd = new PrintDocument())
                {
                    pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                    Notificacion<dynamic> _notificacion = new PedidosEspecialesV2DAO().ObtenerIngresosEfectivo(new IngresoEfectivo { idIngreso = idIngresoEfectivo });
                    pd.PrintPage += (_sender, args) => pd_PrintPageIngresoEfectivo(null, args, _notificacion);
                    pd.PrintController = new StandardPrintController();
                    pd.DefaultPageSettings.Margins.Left = 10;
                    pd.DefaultPageSettings.Margins.Right = 0;
                    pd.DefaultPageSettings.Margins.Top = 0;
                    pd.DefaultPageSettings.Margins.Bottom = 0;
                    pd.Print();
                    pd.Dispose();
                    this.indexProducto = 0;
                    this.paginaActual = 0;
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



        void pd_PrintPageIngresoEfectivo(object sender, PrintPageEventArgs e, Notificacion<dynamic> ticketIngreso)
        {

            try
            {
                string titulo = string.Empty;

                if ((EnumTipoIngresoEfectivo)ticketIngreso.Modelo[0].idTipoIngreso == EnumTipoIngresoEfectivo.AperturaCajas)
                {
                    titulo = "\nCOMPROBANTE DE " + "\n" + "APERTURA DE CAJAS" + "\n" + " PEDIDOS ESPECIALES";
                }
                else
                {
                    titulo = "\nCOMPROBANTE DE " + "\n" + "INGRESO DE EFECTIVO" + "\n" + "PEDIDOS ESPECIALES";
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
                e.Graphics.DrawString(ticketIngreso.Modelo[0].nombreUsuario.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (ticketIngreso.Modelo[0].nombreUsuario.ToString().Length >= 27)
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
                e.Graphics.DrawString(ticketIngreso.Modelo[0].fechaAlta.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (ticketIngreso.Modelo[0].nombreUsuario.ToString().Length >= 27)
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
                e.Graphics.DrawString(ticketIngreso.Modelo[0].Sucursal.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (ticketIngreso.Modelo[0].nombreUsuario.ToString().Length >= 27)
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
                e.Graphics.DrawString(ticketIngreso.Modelo[0].Almacen.ToString(), font, drawBrush, datosTikcet, izquierda);

                if (ticketIngreso.Modelo[0].nombreUsuario.ToString().Length >= 27)
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
                e.Graphics.DrawString("" + Convert.ToSingle(ticketIngreso.Modelo[0].monto).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, datosTikcet, izquierda);

                if (ticketIngreso.Modelo[0].nombreUsuario.ToString().Length >= 27)
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
                ticketIngreso.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticketIngreso.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticketIngreso.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticketIngreso.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }



        }

        public ActionResult _IngresosRetirosEfectivo()
        {
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult ValidaCajaAbierta()
        {
            try
            {
                Sesion usuario = (Sesion)Session["UsuarioActual"];
                Boolean cajaAbierta = new PedidosEspecialesV2DAO().ValidaAperturaCajas(usuario.idUsuario).Estatus == 200 ? true : false;
                return Json(cajaAbierta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion

        #region RetiroExcesoEfectivo
        [HttpPost]
        public ActionResult ObtenerRetirosEfectivo(Retiros retiro)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                retiro.idUsuario = UsuarioActual.idUsuario;
                Notificacion<dynamic> retirosEfectivo = new PedidosEspecialesV2DAO().ObtenerRetirosEfectivo(retiro);

                return Json(JsonConvert.SerializeObject(retirosEfectivo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ObtieneInfoCierre()
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<dynamic> infoCierre = new PedidosEspecialesV2DAO().ObtieneInfoCierre(new Cierre { idUsuario = UsuarioActual.idUsuario, idEstacion = UsuarioActual.idEstacion });

                return Json(JsonConvert.SerializeObject(infoCierre), JsonRequestBehavior.AllowGet);
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
                Notificacion<string> notificacion = new Notificacion<string>();
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                Retiros retiros = new Retiros();
                retiros.idEstacion = usuario.idEstacion;
                retiros.idUsuario = usuario.idUsuario;
                retiros.montoRetiro = montoRetiro;
                notificacion = new PedidosEspecialesV2DAO().RetirarExcesoEfectivo(retiros);
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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


        public ActionResult ImprimeTicketRetiro(Retiros retiro)
        {
            Notificacion<Retiros> notificacion = new Notificacion<Retiros>();
            try
            {

                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;


                //PrintDocument pd = new PrintDocument();
                using (PrintDocument pd = new PrintDocument())
                {
                    pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                    Notificacion<dynamic> _notificacion = new PedidosEspecialesV2DAO().ObtenerRetirosEfectivo(retiro);
                    pd.PrintPage += (_sender, args) => pd_PrintPageRetiro(null, args, _notificacion);
                    pd.PrintController = new StandardPrintController();
                    pd.DefaultPageSettings.Margins.Left = 10;
                    pd.DefaultPageSettings.Margins.Right = 0;
                    pd.DefaultPageSettings.Margins.Top = 0;
                    pd.DefaultPageSettings.Margins.Bottom = 0;
                    pd.Print();
                    pd.Dispose();
                    this.indexProducto = 0;
                    this.paginaActual = 0;
                }

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
        void pd_PrintPageRetiro(object sender, PrintPageEventArgs e, Notificacion<dynamic> retiro)
        {

            try
            {
                string titulo = string.Empty;
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                titulo = "\nCOMPROBANTE DE RETIRO" + "\n" + "POR EXCESO DE EFECTIVO" + "\n" + " PEDIDOS ESPECIALES"; ;

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

                e.Graphics.DrawString("  Usuario: " + retiro.Modelo[0].nombreUsuario.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("  Fecha: " + retiro.Modelo[0].fechaAlta.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("  Sucursal: " + retiro.Modelo[0].descripcionSucursal.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("  Almacen: " + retiro.Modelo[0].descripcionAlmacen.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("  Estacion: " + retiro.Modelo[0].nombreEstacion.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;

                e.Graphics.DrawString("================================================" + " \n", font, drawBrush, datosEnca, izquierda);
                datosEnca.Y += 14;


                e.Graphics.DrawString("  MONTO RETIRO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(Convert.ToSingle(retiro.Modelo[0].montoRetiro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, 266, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;


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
                retiro.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                retiro.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<Ventas>();
                retiro.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                retiro.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region devoluciones
        [HttpPost]
        public ActionResult RealizaDevolucionPedidoEspecial(List<ProductosDevueltosPedidoEspecial> productos, Int64 idPedidoEspecial, float montoDevuelto, string motivoDevolucion)
        {
            try
            {
                Notificacion<string> notificacion = new Notificacion<string>();
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                notificacion = new PedidosEspecialesV2DAO().RealizaDevolucionPedidoEspecial(productos, idPedidoEspecial, montoDevuelto, usuario.idUsuario, motivoDevolucion);
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult _ObtenerTicketsPedidoEspecial(Int64 idPedidoEspecial)
        {
            try
            {
                ViewBag.idPedidoEspecial = idPedidoEspecial;
                Notificacion<dynamic> ticketsPedidoEspecial = new PedidosEspecialesV2DAO().ObtieneTicketsPedidoEspecial(idPedidoEspecial);

                return PartialView(ticketsPedidoEspecial);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        [HttpPost]
        public ActionResult ImprimeTicket(Int64 idPedidoEspecial, int idTipoTicketPedidoEspecial, int idTicketPedidoEspecial = 0, Boolean ticketFinal = true)
        {
            Notificacion<Ventas> notificacion;
            try
            {

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;


                this.idPedidoEspecial = idPedidoEspecial;

                //PrintDocument pd = new PrintDocument();
                using (PrintDocument pd = new PrintDocument())
                {
                    pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                    Notificacion<dynamic> _notificacion = new PedidosEspecialesV2DAO().ObtieneDetalleTicketPedidoEspecial(idPedidoEspecial, idTipoTicketPedidoEspecial, idTicketPedidoEspecial, ticketFinal);
                    if (idTipoTicketPedidoEspecial == 1)
                        pd.PrintPage += (_sender, args) => pd_PrintPageTicketOriginal(null, args, _notificacion, ticketFinal); //ticket original
                    if (idTipoTicketPedidoEspecial == 2) //ticket devoluciones
                        pd.PrintPage += (_sender, args) => pd_PrintPageDevoluciones(null, args, _notificacion);
                    pd.PrintController = new StandardPrintController();
                    pd.DefaultPageSettings.Margins.Left = 10;
                    pd.DefaultPageSettings.Margins.Right = 0;
                    pd.DefaultPageSettings.Margins.Top = 0;
                    pd.DefaultPageSettings.Margins.Bottom = 0;
                    pd.Print();
                    pd.Dispose();
                    this.indexProducto = 0;
                    this.paginaActual = 0;
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

        void pd_PrintPageTicketOriginal(object sender, PrintPageEventArgs e, Notificacion<dynamic> ticket, Boolean ticketFinal)
        {
            try
            {
                //List<Ticket> lstTickets = notificacion.Modelo;
                //if (this.paginaActual< _lstTickets.Count())
                //    lstTickets = _lstTickets[this.paginaActual];

                float TotalMonto = 0;
                float TotalIVA = 0;
                float TotalComisionBancaria = 0;
                float TotalAhorro = 0;
                float TotalPagado = 0;
                float Cambio = 0;
                float cantidadTotalArticulos = 0;

                int ancho = 258;
                int espaciado = 14;
                Rectangle datosIndex = new Rectangle(2, 285, 15, 82);
                Rectangle datosProducto = new Rectangle(20, 285, 145, 82);
                Rectangle datosCantidad = new Rectangle(167, 285, 30, 82);
                Rectangle datosPrecioU = new Rectangle(205, 285, 30, 82);
                Rectangle datosPrecio = new Rectangle(235, 285, 48, 82);

                // TOTALES
                //montoPagadoAgregarProductos = notificacion.Modelo[0].montoPagadoAgregarProductos;
                //montoAgregarProductos = notificacion.Modelo[0].montoAgregarProductos;
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

                    postTicketY = (logo.Y + logo.Height + espaciado);


                    //postTicketY = postTicketY + 100 + espaciado;
                    Rectangle datos = new Rectangle(5, postTicketY, ancho, 82);
                    e.Graphics.DrawString("RFC:" + "COVO781128LJ1" + ",\n" + "Calle Macarena #82" + '\n' + "Inguambo" + '\n' + "Uruapan, Michoacán" + '\n' + "C.p. 58000", font, drawBrush, datos, centrado);

                    e.Graphics.DrawString("Ticket PE:" + ticket.Modelo[0].idPedidoEspecial.ToString(), font, drawBrush, 40, 181, izquierda);
                    e.Graphics.DrawString("Fecha:" + ticket.Modelo[0].fechaTicket, font, drawBrush, 150, 181, izquierda);
                    e.Graphics.DrawString("Hora:" + ticket.Modelo[0].horaTicket, font, drawBrush, 150, 191, izquierda);

                    postTicketY = datos.Y + datos.Height + espaciado;
                    Rectangle datosEnca = new Rectangle(0, postTicketY, 295, 82);

                    e.Graphics.DrawString("  Cliente: " + ticket.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Dirección: " + ticket.Modelo[0].direccion.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 28;
                    e.Graphics.DrawString("  Forma de Pago: " + ticket.Modelo[0].descFormaPago.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
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

                for (int i = indexProducto; i < ticket.Modelo.Count; i++)
                {
                    e.Graphics.DrawString((indexProducto + 1).ToString() + " \n", font, drawBrush, datosIndex, izquierda);
                    e.Graphics.DrawString(ticket.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(ticket.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].precioVenta).ToString() + " \n", font, drawBrush, datosPrecioU, izquierda);
                    e.Graphics.DrawString((Convert.ToSingle(ticket.Modelo[i].monto) + Convert.ToSingle(ticket.Modelo[i].ahorro)).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);


                    /*monto += lstTickets[i].monto;
                    montoIVA += lstTickets[i].montoIVA;
                    montoComisionBancaria += lstTickets[i].montoComisionBancaria;
                    montoAhorro += lstTickets[i].ahorro;
                    */
                    if (ticket.Modelo[i].descProducto.ToString().Length >= 23)
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
                    if (ticket.Modelo[i].ahorro > 0)
                    {
                        e.Graphics.DrawString("     └Descuento por mayoreo" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString("-" + (ticket.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
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


                    //monto = monto + Convert.ToSingle(ticket.Modelo[i].monto);
                    //montoIVA = montoIVA + Convert.ToSingle(ticket.Modelo[i].montoIVA);
                    //montoComisionBancaria = montoComisionBancaria + Convert.ToSingle(ticket.Modelo[i].montoComisionBancaria);
                    //montoAhorro = montoAhorro + Convert.ToSingle(ticket.Modelo[i].ahorro);
                    //cantidadTotalDeArticulos = cantidadTotalDeArticulos + Convert.ToSingle(ticket.Modelo[i].cantidad);

                }

                int posXFooter = 285;
                for (int i = 0; i < ticket.Modelo.Count; i++)
                {
                    TotalMonto = TotalMonto + Convert.ToSingle(ticket.Modelo[i].monto);
                    TotalIVA = TotalIVA + Convert.ToSingle(ticket.Modelo[i].montoIVA);
                    TotalComisionBancaria = TotalComisionBancaria + Convert.ToSingle(ticket.Modelo[i].montoComisionBancaria);
                    TotalAhorro = TotalAhorro + Convert.ToSingle(ticket.Modelo[i].ahorro);
                    cantidadTotalArticulos = cantidadTotalArticulos + Convert.ToSingle(ticket.Modelo[i].cantidad);

                }

                TotalPagado = Convert.ToSingle(ticket.Modelo[0].montoPagado);
                Cambio = TotalPagado == 0 ? 0 : TotalPagado - TotalMonto - TotalIVA - TotalComisionBancaria;

                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);

                if (indexProducto == ticket.Modelo.Count)
                {
                    datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + 1)
                {
                    e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString(TotalMonto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + 2)
                {
                    //if (montoComisionBancaria > 0)
                    //{

                    e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((TotalComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                    //}
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + 3)
                //if (montoIVA > 0)
                {

                    e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((TotalIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }


                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + 4)
                {
                    e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((TotalMonto + TotalIVA + TotalComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + 5)
                {
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + 6)
                {
                    e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((TotalPagado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + 7)
                {
                    e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((Cambio).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }
                if (this.insertaPagina(datosfooter1.Y, ref e)) return;

                if (indexProducto == ticket.Modelo.Count + 8)
                {
                    datosfooter1.Y += espaciado;
                    Rectangle totalArt = new Rectangle(5, datosfooter1.Y, ancho, 82);
                    e.Graphics.DrawString("  CANTIDAD DE ARTICULOS COMPRADOS: " + cantidadTotalArticulos.ToString(), font, drawBrush, totalArt, centrado);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                }


                if (indexProducto == ticket.Modelo.Count + 9)
                {
                    if (TotalAhorro > 0)
                    {
                        datosfooter1.Y += espaciado;
                        Rectangle datosAhorro = new Rectangle(0, datosfooter1.Y, 280, 82);
                        e.Graphics.DrawString("******* USTED AHORRO:  " + (TotalAhorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " *******", font, drawBrush, datosAhorro, centrado);
                        datosfooter1.Y += espaciado;
                        indexProducto++;

                    }
                    else
                    {
                        indexProducto++;
                    }
                }
                datosfooter1.Y += espaciado;
                if (this.insertaPagina(datosfooter1.Y, ref e)) return;

                if (indexProducto >= ticket.Modelo.Count + 10)
                {

                    Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(ticket.Modelo[0].codigoBarras.ToString()));
                    e.Graphics.DrawImage(imagenCodigoTicket, -50, datosfooter1.Y, 400, 60);
                    datosfooter1.Y += espaciado + 30 + espaciado;
                    indexProducto++;
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y, 280, 82);
                if (indexProducto >= ticket.Modelo.Count + 11)
                {
                    datosfooter1.Y += espaciado;
                    datosfooter2 = new Rectangle(0, datosfooter1.Y, 280, 82);
                    if (ticketFinal)
                    {
                        e.Graphics.DrawString("********  GRACIAS POR SU PREFERENCIA.  ********", font, drawBrush, datosfooter2, centrado);
                    }
                    else
                    {
                        e.Graphics.DrawString("******  ESTE TICKEET ES SOLO INFORMATIVO  ******", font, drawBrush, datosfooter2, centrado);
                    }
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
                //e.Graphics.DrawImage
                //}



            }
            catch (InvalidPrinterException ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticket.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticket.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticket.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticket.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }

        }

        void pd_PrintPageDevoluciones(object sender, PrintPageEventArgs e, Notificacion<dynamic> ticketDevolucion)
        {
            //Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
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

                Rectangle datosIndex = new Rectangle(2, 285, 15, 82);
                Rectangle datosProducto = new Rectangle(5, 285, 145, 82);
                Rectangle datosCantidad = new Rectangle(150, 285, 30, 82);
                Rectangle datosPrecioU = new Rectangle(190, 285, 30, 82);
                Rectangle datosPrecio = new Rectangle(220, 285, 48, 82);
                Rectangle datosEnca = new Rectangle(0, 215, 280, 82);



                if (paginaActual == 0)
                {

                    //Se pinta logo 
                    Rectangle logo = new Rectangle(80, 15, 280, 81);
                    e.Graphics.DrawImage(newImage, logo, 0, 0, 380.0F, 120.0F, units);

                    Rectangle datos = new Rectangle(5, 110, ancho, 82);
                    e.Graphics.DrawString("*******************************************************************" + "\n" + "***************** TICKET DE DEVOLUCIÓN *****************" + '\n' + "*******************************************************************", font, drawBrush, datos, centrado);

                    e.Graphics.DrawString("Ticket PE:" + ticketDevolucion.Modelo[0].idPedidoEspecial.ToString(), font, drawBrush, 40, 181, izquierda);
                    e.Graphics.DrawString("Fecha:" + ticketDevolucion.Modelo[0].fechaTicket, font, drawBrush, 150, 181, izquierda);
                    e.Graphics.DrawString("Hora:" + ticketDevolucion.Modelo[0].horaTicket, font, drawBrush, 150, 191, izquierda);


                    e.Graphics.DrawString("  Cliente: " + ticketDevolucion.Modelo[0].nombreCliente.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Dirección: " + ticketDevolucion.Modelo[0].direccion.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 28;
                    e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("#     Descripcion                                 Art.          Precio       Precio" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 9;
                    e.Graphics.DrawString("                                                 Devueltos    Unitario       " + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 6;
                    e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosEnca, izquierda);

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




                for (int i = indexProducto; i < ticketDevolucion.Modelo.Count; i++)
                {
                    e.Graphics.DrawString((i + 1).ToString() + " \n", font, drawBrush, datosIndex, izquierda);
                    e.Graphics.DrawString(ticketDevolucion.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(ticketDevolucion.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticketDevolucion.Modelo[i].precioVenta).ToString() + " \n", font, drawBrush, datosPrecioU, izquierda);
                    e.Graphics.DrawString((Convert.ToSingle(ticketDevolucion.Modelo[i].monto)).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);


                    if (ticketDevolucion.Modelo[i].descProducto.ToString().Length >= 23)
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

                    indexProducto++;
                    if (datosProducto.Y >= 1092)
                    {
                        this.paginaActual++;
                        e.HasMorePages = true;
                        return;
                    }
                }

                float montoTotal = 0;
                float TotalComisionBancaria = 0;
                float cantidadTotalArticulos = 0;
                for (int i = 0; i < ticketDevolucion.Modelo.Count; i++)
                {
                    montoTotal += Convert.ToSingle(ticketDevolucion.Modelo[i].monto);
                    TotalComisionBancaria += Convert.ToSingle(ticketDevolucion.Modelo[i].montoComisionBancaria);
                    cantidadTotalArticulos += Convert.ToSingle(ticketDevolucion.Modelo[i].cantidad);
                }


                int posXFooter = 285;

                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 280, 82);

                if (indexProducto == ticketDevolucion.Modelo.Count)
                {
                    datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticketDevolucion.Modelo.Count + 1)
                {
                    e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString(montoTotal.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticketDevolucion.Modelo.Count + 2)
                {
                    //if (montoComisionBancaria > 0)
                    //{
                    e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((TotalComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                    //}
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticketDevolucion.Modelo.Count + 3)
                {
                    e.Graphics.DrawString("  TOTAL DEVUELTO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoTotal + TotalComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado * 2;
                    indexProducto++;
                }

                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticketDevolucion.Modelo.Count + 4)
                {
                    e.Graphics.DrawString("___________________________________________________", font, drawBrush, datosfooter2, centrado);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;
                    indexProducto++;
                }


                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticketDevolucion.Modelo.Count + 5)
                {
                    e.Graphics.DrawString("FIRMA DEL CLIENTE", font, drawBrush, datosfooter2, centrado);
                    datosfooter1.Y += espaciado * 2;
                    datosfooter2.Y += espaciado * 2;
                    indexProducto++;
                }


                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == ticketDevolucion.Modelo.Count + 6)
                {
                    datosfooter1.Y += espaciado;
                    datosfooter1.Y += espaciado;
                    Rectangle totalArt = new Rectangle(5, datosfooter1.Y, ancho, 82);
                    e.Graphics.DrawString("  CANTIDAD DE ARTICULOS DEVUELTOS: " + cantidadTotalArticulos.ToString(), font, drawBrush, totalArt, centrado);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                }


                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto >= ticketDevolucion.Modelo.Count + 7)
                {
                    Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(ticketDevolucion.Modelo[0].codigoBarras.ToString()));
                    datosfooter1.Y += 40;
                    Rectangle posImgCodigoTicket = new Rectangle(0, datosfooter1.Y, 400, 120);
                    e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 380.0F, 120.0F, units);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;
                    indexProducto++;
                }


                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto >= ticketDevolucion.Modelo.Count + 8)
                {
                    // para mas espaciado al final del ticket
                    e.Graphics.DrawString("", font, drawBrush, 0, datosfooter2.Y, centrado);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;
                    indexProducto++;
                }

            }
            catch (InvalidPrinterException ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticketDevolucion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticketDevolucion.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticketDevolucion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticketDevolucion.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult VerTicket(Int64 idPedidoEspecial, int idTipoTicketPedidoEspecial, int idTicketPedidoEspecial = 0, Boolean ticketFinal = true)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<dynamic> ticket = new PedidosEspecialesV2DAO().ObtieneDetalleTicketPedidoEspecial(idPedidoEspecial, idTipoTicketPedidoEspecial, idTicketPedidoEspecial, ticketFinal);
                string pdf = "";
                if (idTipoTicketPedidoEspecial == 1) //ticket original
                    pdf = Convert.ToBase64String(Utilerias.Utils.GeneraTicketPedidoEspecial(ticket));
                if (idTipoTicketPedidoEspecial == 2) //ticket devolucion
                    pdf = Convert.ToBase64String(Utilerias.Utils.GeneraTicketDevolucionPedidoEspecial(ticket));
                return Json(pdf, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CierreCajas

        public ActionResult CierreCajas()
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                Notificacion<dynamic> cierreDia = new PedidosEspecialesV2DAO().ObtieneCierreDia(usuario.idUsuario, usuario.idEstacion, DateTime.MinValue, 0);
                return View(cierreDia);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ObtenerConfiguracionPedidosEspeciales(EnumTipoConfig tipoConfig)
        {
            try
            {
                Notificacion<dynamic> notificacion = new PedidosEspecialesV2DAO().ObtieneConfiguracionPedidosEspeciales(tipoConfig);
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ValidarContrasena(string usuario, string contrasena)
        {
            try
            {
                Notificacion<Sesion> n = new VentasDAO().ValidaUsuario(usuario, contrasena, EnumTipoValidaUsr.AutorizarCierre);
                return Json(n, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult RealizaCierreEstacion(float efectivoEntregadoEnCierre)
        {
            try
            {
                Notificacion<string> notificacion = new Notificacion<string>();
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                notificacion = new PedidosEspecialesV2DAO().RealizaCierreEstacion(UsuarioActual.idUsuario, UsuarioActual.idEstacion, efectivoEntregadoEnCierre);
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ImprimeTicketCierreCajas(Int64 idCierrePedidoEspecial)
        {
            Notificacion<Ventas> notificacion;
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;


                //PrintDocument pd = new PrintDocument();
                using (PrintDocument pd = new PrintDocument())
                {
                    pd.PrinterSettings.PrinterName = WebConfigurationManager.AppSettings["impresora"].ToString(); // @"\\DESKTOP-M7HANDH\EPSON";
                    Notificacion<dynamic> _notificacion = new PedidosEspecialesV2DAO().ObtieneCierreDia(UsuarioActual.idUsuario, UsuarioActual.idEstacion, DateTime.MinValue, idCierrePedidoEspecial);
                    pd.PrintPage += (_sender, args) => pd_PrintPageCierreDia(null, args, _notificacion);
                    pd.PrintController = new StandardPrintController();
                    pd.DefaultPageSettings.Margins.Left = 10;
                    pd.DefaultPageSettings.Margins.Right = 0;
                    pd.DefaultPageSettings.Margins.Top = 0;
                    pd.DefaultPageSettings.Margins.Bottom = 0;
                    pd.Print();
                    pd.Dispose();
                    this.indexProducto = 0;
                    this.paginaActual = 0;
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

        void pd_PrintPageCierreDia(object sender, PrintPageEventArgs e, Notificacion<dynamic> ticket)
        {
            try
            {
                int ancho = 258;
                int espaciado = 14;
                Rectangle datosIndex = new Rectangle(2, 285, 15, 82);
                Rectangle datosProducto = new Rectangle(20, 285, 145, 82);
                Rectangle datosCantidad = new Rectangle(167, 285, 30, 82);
                Rectangle datosPrecioU = new Rectangle(205, 285, 30, 82);
                Rectangle datosPrecio = new Rectangle(235, 285, 48, 82);

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

                    postTicketY = (logo.Y + logo.Height + espaciado);

                    Rectangle datos = new Rectangle(5, postTicketY, ancho, 82);
                    e.Graphics.DrawString("COMPROBANTE POR CIERRE DE CAJA DEL DIA", Bold, drawBrush, datos, centrado);

                    postTicketY = datos.Y + espaciado + espaciado + espaciado;
                    Rectangle datosEnca = new Rectangle(0, postTicketY, 295, 82);
                    e.Graphics.DrawString("  Ticket cierre: " + ticket.Modelo[0].idCierrePedidoEspecial.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Fecha: " + ticket.Modelo[0].fechaAlta.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Usuario: " + ticket.Modelo[0].nombreUsuario.ToString().ToUpper() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Sucursal: " + ticket.Modelo[0].descripcionSucursal.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Almacen: " + ticket.Modelo[0].descripcionAlmacen.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;
                    e.Graphics.DrawString("  Estacion: " + ticket.Modelo[0].nombreEstacion.ToString() + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;

                    e.Graphics.DrawString("================================================" + " \n", font, drawBrush, datosEnca, izquierda);
                    datosEnca.Y += 14;


                    datosProducto = new Rectangle(5, datosEnca.Y, 180, 82);
                    datosPrecio = new Rectangle(220, datosEnca.Y, 48, 82);


                }
                else
                {

                    datosProducto = new Rectangle(5, 15, 180, 82);
                    datosPrecio = new Rectangle(220, 15, 48, 82);
                }

                Rectangle datosfooterTitulo = new Rectangle(0, datosProducto.Y, 295, 15);
                string titulo = "";
                for (int i = indexProducto; i < ticket.Modelo.Count; i++)
                {
                    e.Graphics.DrawString(ticket.Modelo[i].descripcion.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString("" + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;


                    if (Convert.ToInt32(ticket.Modelo[i].idAlmacen) == 0)
                    {
                        titulo = "Abonos";
                    }
                    else
                    {
                        titulo = "Ventas";
                    }


                    e.Graphics.DrawString(" " + titulo + " contado" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasContado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString(" " + titulo + " tarjeta de crédito" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasTC).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString(" " + titulo + " transferencias bancarias" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasTransferencias).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    e.Graphics.DrawString(" " + titulo + " otras formas de pago" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasOtrasFormasPago).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    if (Convert.ToInt32(ticket.Modelo[i].idAlmacen) > 0)
                    {

                        e.Graphics.DrawString(" " + titulo + " crédito" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasCredito).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosPrecio.Y += espaciado;


                        e.Graphics.DrawString(" " + titulo + " devoluciones" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].MontoDevoluciones).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosPrecio.Y += espaciado;

                    }


                    e.Graphics.DrawString("Total efectivo" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].TotalEfectivo).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;


                    indexProducto++;
                    if (datosProducto.Y >= 1092)
                    {
                        this.paginaActual++;
                        e.HasMorePages = true;
                        return;
                    }

                }


                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count)
                {
                    e.Graphics.DrawString("Resumen General" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString("" + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }


                if (indexProducto == ticket.Modelo.Count + 1)
                {
                    for (int i = 0; i < ticket.Modelo.Count; i++)
                    {
                        e.Graphics.DrawString(" " + ticket.Modelo[i].descripcionSaldo.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString("" + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosPrecio.Y += espaciado;

                        e.Graphics.DrawString("  " + "Efectivo" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasContado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosPrecio.Y += espaciado;

                        e.Graphics.DrawString("  " + "TDC o transferencias" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasTDCTransferencias).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosPrecio.Y += espaciado;

                        e.Graphics.DrawString("  " + "Otras formas de pago" + " \n", font, drawBrush, datosProducto, izquierda);
                        e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasOtrasFormasPago).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                        datosProducto.Y += espaciado;
                        datosPrecio.Y += espaciado;

                        if (Convert.ToInt32(ticket.Modelo[i].idAlmacen) > 0)
                        {
                            e.Graphics.DrawString("  " + "Ventas a crédito" + " \n", font, drawBrush, datosProducto, izquierda);
                            e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[i].VentasCredito).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                            datosProducto.Y += espaciado;
                            datosPrecio.Y += espaciado;

                        }

                        indexProducto++;
                        if (datosProducto.Y >= 1092)
                        {
                            this.paginaActual++;
                            e.HasMorePages = true;
                            return;
                        }

                    }

                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;

                    indexProducto++;
                }




                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 2)
                {
                    e.Graphics.DrawString("Ingresos efectivo" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].MontoIngresosEfectivo).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }


                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 3)
                {
                    e.Graphics.DrawString("Retiros efectivo" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].MontoRetirosEfectivo).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }


                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 4)
                {
                    e.Graphics.DrawString("Saldo final en caja efectivo" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].MontoCierreEfectivo).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }


                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 5)
                {
                    e.Graphics.DrawString("Saldo final en caja tarjeta de crédito y transferencias" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToSingle(ticket.Modelo[0].MontoCierreTC).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado + 10;
                    datosPrecio.Y += espaciado + 10;
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }

                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 6)
                {
                    e.Graphics.DrawString("No. devoluciones" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToInt32(ticket.Modelo[0].noDevoluciones) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }

                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 7)
                {
                    e.Graphics.DrawString("Tickets" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString("" + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }

                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 8)
                {
                    e.Graphics.DrawString(" Efectivo" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToInt32(ticket.Modelo[0].NoTicketsEfectivo) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }

                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 9)
                {
                    e.Graphics.DrawString(" Crédito" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToInt32(ticket.Modelo[0].NoTicketsCredito) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }

                if (this.insertaPagina(datosProducto.Y, ref e)) return;
                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 10)
                {
                    e.Graphics.DrawString(" En resguardo" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(Convert.ToInt32(ticket.Modelo[0].NoPedidosEnResguardo) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosPrecio.Y += espaciado;
                    indexProducto++;
                }


                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);

                if (indexProducto == ticket.Modelo.Count + ticket.Modelo.Count + 11)
                {
                    datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;

                }

                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                //if (indexProducto == ticket.Modelo.Count + 1)
                //{
                //    e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                //    e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                //    datosfooter1.Y += espaciado;
                //    indexProducto++;

                //}

                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                //if (indexProducto == ticket.Modelo.Count + 2)
                //{
                //    if (montoComisionBancaria > 0)
                //    {
                //        e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                //        e.Graphics.DrawString((montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                //        datosfooter1.Y += espaciado;
                //        indexProducto++;

                //    }
                //}

                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                //if (indexProducto == ticket.Modelo.Count + 3)
                //    if (montoIVA > 0)
                //    {
                //        e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                //        e.Graphics.DrawString((montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                //        datosfooter1.Y += espaciado;
                //        indexProducto++;

                //    }


                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                //if (indexProducto == ticket.Modelo.Count + 4)
                //{
                //    e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                //    e.Graphics.DrawString((monto + montoIVA + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                //    datosfooter1.Y += espaciado;
                //    indexProducto++;

                //}

                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                //if (indexProducto == ticket.Modelo.Count + 5)
                //{
                //    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                //    datosfooter1.Y += espaciado;
                //    indexProducto++;

                //}

                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                //if (indexProducto == ticket.Modelo.Count + 6)
                //{
                //    e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                //    e.Graphics.DrawString((montoPagado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                //    datosfooter1.Y += espaciado;
                //    indexProducto++;

                //}

                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                //if (indexProducto == ticket.Modelo.Count + 7)
                //{
                //    e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                //    e.Graphics.DrawString((suCambio).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                //    datosfooter1.Y += espaciado;
                //    indexProducto++;

                //}
                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;

                //if (indexProducto == ticket.Modelo.Count + 8)
                //{
                //    datosfooter1.Y += espaciado;
                //    Rectangle totalArt = new Rectangle(5, datosfooter1.Y, ancho, 82);
                //    e.Graphics.DrawString("  CANTIDAD DE ARTICULOS COMPRADOS: " + cantidadTotalDeArticulos.ToString(), font, drawBrush, totalArt, centrado);
                //    datosfooter1.Y += espaciado;
                //    indexProducto++;
                //}


                //if (indexProducto == ticket.Modelo.Count + 9)
                //{
                //    if (montoAhorro > 0)
                //    {
                //        datosfooter1.Y += espaciado;
                //        Rectangle datosAhorro = new Rectangle(0, datosfooter1.Y, 280, 82);
                //        e.Graphics.DrawString("******* USTED AHORRO:  " + (montoAhorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " *******", font, drawBrush, datosAhorro, centrado);
                //        datosfooter1.Y += espaciado;
                //        indexProducto++;

                //    }
                //    else
                //    {
                //        indexProducto++;
                //    }
                //}
                //datosfooter1.Y += espaciado;
                //if (this.insertaPagina(datosfooter1.Y, ref e)) return;

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto >= ticket.Modelo.Count + ticket.Modelo.Count + 12)
                {
                    e.Graphics.DrawString("================================================" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado + 50;
                    indexProducto++;
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto >= ticket.Modelo.Count + ticket.Modelo.Count + 13)
                {
                    e.Graphics.DrawString("___________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                }


                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto >= ticket.Modelo.Count + ticket.Modelo.Count + 14)
                {
                    Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 5, 280, 82);
                    e.Graphics.DrawString("  AUTORIZO  ", font, drawBrush, datosfooter2, centrado);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;

                    e.Graphics.DrawString("-", font, drawBrush, 0, datosfooter2.Y + 30, centrado);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;
                    e.Graphics.DrawString("", font, drawBrush, 0, datosfooter2.Y, centrado);
                    datosfooter1.Y += espaciado;
                    datosfooter2.Y += espaciado;
                    indexProducto++;
                }

            }
            catch (InvalidPrinterException ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticket.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticket.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //notificacion = new Notificacion<Ventas>();
                ticket.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                ticket.Estatus = -1;
                //return Json(notificacion, JsonRequestBehavior.AllowGet);
            }

        }



        #endregion

    }

}
