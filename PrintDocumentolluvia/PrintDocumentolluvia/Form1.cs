﻿using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.Utilerias;
using PdfiumViewer;
using RawPrint;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintDocumentolluvia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //7022
        //int idVenta = 1948;

        int idVenta = 7022;
        int indiceProducto = 0;
        int paginaActual = 0;
        int productosporPagina = 30;
        int paginas = 0;
        int indexProducto = 0;
        int pintaFooterIndice = 0;
        Boolean control = false;
        private void button1_Click(object sender, EventArgs e)
        {
            Notificacion<Ventas> notificacion;
            try
            {
                paginaActual = 0;
                productosporPagina = 30;
                paginas = 0;

                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Se envio el ticket a la impresora.";
                notificacion.Estatus = 200;

                using (PrintDocument pd = new PrintDocument())
                {

                    //pd.PrinterSettings.PrinterName = "Microsoft Print to PDF"; // @"\\DESKTOP-M7HANDH\EPSON";
                    pd.PrinterSettings.PrinterName = "EPSON TM-T20III Receipt";
                    //pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                    Notificacion<List<Ticket>> _notificacion = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = this.idVenta });
                    List<List<Ticket>> listadoTickets = new List<List<Ticket>>();

                    if (_notificacion.Modelo.Count() > productosporPagina)
                    {
                        //this.productosporPagina += 6;
                        //calculamos el numero de paginas que vamos a recorrer o impimir
                        float calcularPaginas = _notificacion.Modelo.Count() / Convert.ToSingle(this.productosporPagina);
                        //siempre aumentamos una pagina mas por si la division arroja decimales estar sobrados al recorrer los arreglos
                        this.paginas = Convert.ToInt16(calcularPaginas) + 1;
                        // hacemos un split de las lista total de productos que se van a imprmir con la fuente actual caben  cerca de 20 articulos mas el logo y los datos de la empresa 
                        //esto en la primera pagina como en la segunda pagina ya no se imprime el logo aumentamos la cantidad de productos que se pintan por pagina que en este caso
                        //son 8 productos mas los que caben.
                        var lstTicketsSplit = this.SplitList<Ticket>(_notificacion.Modelo, this.productosporPagina);
                        //agregarmos los primeros 20 a la lista que los va a imprimir
                        //listadoTickets.Add(lstTicketsSplit.First());
                        //generamos una lista con el resto de productos para ahora poder hacer el split con 28 productos cada lista
                        List<Ticket> _listRestoTicket = new List<Ticket>();
                        for (int i = 1; i < lstTicketsSplit.Count(); i++)
                        {
                            _listRestoTicket.AddRange(lstTicketsSplit.ToList()[i]);
                        }
                        //lstTicketsSplit = this.SplitList<Ticket>(_listRestoTicket, (this.productosporPagina + 15));

                        //asignamos las listas de arrary a la listra que vamos a recorrer para pintar el ticjet
                        foreach (var item in lstTicketsSplit) { listadoTickets.Add(item); }
                    }
                    else
                    {
                        listadoTickets.Add(_notificacion.Modelo);
                    }
                    bool esSoloUnTicket = this.control = (listadoTickets.ToList()[0].Count >= 19 && listadoTickets.ToList()[0].Count <= 25 && listadoTickets.ToList().Count == 1);
                    /*if (esSoloUnTicket)
                        pd.PrintPage += (_sender, args) => pd_PrintPageUnTicket(sender, args, _notificacion, listadoTickets , esSoloUnTicket);
                    else*/
                    //pd.PrintPage += (_sender, args) => pd_PrintPageUnTicket(sender, args, _notificacion, listadoTickets, esSoloUnTicket);
                    //pd.PrintPage += (_sender, args) => pd_PrintPage(sender, args, _notificacion, listadoTickets, esSoloUnTicket);

                    pd.PrintController = new StandardPrintController();
                    pd.DefaultPageSettings.Margins.Left = 10;
                    pd.DefaultPageSettings.Margins.Right = 0;
                    pd.DefaultPageSettings.Margins.Top = 0;
                    pd.DefaultPageSettings.Margins.Bottom = 0;
                    PaperSize psize = new PaperSize("Custom", 100, 200);
                    pd.DefaultPageSettings.PaperSize = psize;
                    pd.DefaultPageSettings.PaperSize.Height = 820;
                    pd.DefaultPageSettings.PaperSize.Width = 520;
                    pd.PrintPage += (_sender, args) => pd_PrintPage(sender, args, _notificacion, listadoTickets, esSoloUnTicket);
                    pd.Print();
                    pd.Dispose();
                }
            }

            catch (InvalidPrinterException ex)
            {
                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                notificacion = new Notificacion<Ventas>();
                notificacion.Mensaje = "Por favor revise la conexion de la impresora " + ex.Message;
                notificacion.Estatus = -1;
                Console.WriteLine(ex.Message);
            }
        }
        public IEnumerable<List<T>> SplitList<T>(List<T> bigList, int nSize = 3)
        {
            for (int i = 0; i < bigList.Count; i += nSize)
            {
                yield return bigList.GetRange(i, Math.Min(nSize, bigList.Count - i));
            }
        }

        void pd_PrintPageUnTicket(object sender, PrintPageEventArgs e, Notificacion<List<Ticket>> notificacion, List<List<Ticket>> _lstTickets, bool esSoloUnTicket)
        {

            try
            {
                e.HasMorePages = false;
                List<Ticket> lstTickets = _lstTickets[this.paginaActual];
                float monto = 0;
                float montoIVA = 0;
                float montoComisionBancaria = 0;
                float montoAhorro = 0;
                float montoPagado = 0;
                float suCambio = 0;

                float montoPagadoAgregarProductos = 0;
                float montoAgregarProductos = 0;
                float suCambioAgregarProductos = 0;
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

                //Se pinta logo 
                int poslogoY = 15;
                int postTicketY = 0;
                Image newImage = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\assets\\img\\logo_lluvia_150.jpg");
                Rectangle logo = new Rectangle(80, poslogoY, 280, 81);
                e.Graphics.DrawImage(newImage, logo, 0, 0, 380.0F, 120.0F, units);

                Image imagenCodigoTicket = ByteArrayToImage(Utils.GenerarCodigoBarras(notificacion.Modelo[0].codigoBarras.ToString()));

                postTicketY = (logo.Y + logo.Height + espaciado);
                Rectangle posImgCodigoTicket = new Rectangle(0, postTicketY, 400, 120);
                e.Graphics.DrawImage(imagenCodigoTicket, posImgCodigoTicket, 0, 0, 380.0F, 120.0F, units);

                postTicketY = posImgCodigoTicket.Y + posImgCodigoTicket.Height + espaciado;
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
                //datosEnca.Y += 14;

                montoPagadoAgregarProductos = notificacion.Modelo[0].montoPagadoAgregarProductos;
                montoAgregarProductos = notificacion.Modelo[0].montoAgregarProductos;


                for (int i = 0; i < lstTickets.Count(); i++)
                {
                    e.Graphics.DrawString(indexProducto.ToString() + " \n", font, drawBrush, datosIndex, izquierda);
                    e.Graphics.DrawString(lstTickets[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString(lstTickets[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                    e.Graphics.DrawString(lstTickets[i].precioVenta.ToString() + " \n", font, drawBrush, datosPrecioU, izquierda);
                    e.Graphics.DrawString((lstTickets[i].monto + lstTickets[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    indexProducto++;

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

                }

                int posXFooter = 285;
                if (montoAgregarProductos > 0)
                {
                    monto -= montoAgregarProductos;
                }

                suCambioAgregarProductos = montoPagadoAgregarProductos - montoAgregarProductos;

                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 82);
                e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                if (montoComisionBancaria > 0)
                {
                    e.Graphics.DrawString("  COMISIÓN BANCARIA:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }

                if (montoIVA > 0)
                {
                    e.Graphics.DrawString("  I.V.A:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                }

                e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString((monto + montoIVA + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                montoPagado = notificacion.Modelo[0].montoPagado;
                suCambio = montoPagado - monto - montoIVA - montoComisionBancaria;


                e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString((montoPagado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                e.Graphics.DrawString((suCambio).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                datosfooter1.Y += espaciado;

                if (montoAgregarProductos > 0)
                {

                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    datosfooter1.Y += espaciado;

                    e.Graphics.DrawString("  COMPLEMENTOS:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    datosfooter1.Y += espaciado;

                    e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoPagadoAgregarProductos).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;

                    e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((suCambioAgregarProductos).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;

                }

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
        void pd_PrintPage(object sender, PrintPageEventArgs e, Notificacion<List<Ticket>> notificacion, List<List<Ticket>> _lstTickets, bool esSoloUnTicket)
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
                    e.Graphics.DrawString(indexProducto.ToString() + " \n", font, drawBrush, datosIndex, izquierda);
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
                indexProducto++;
                monto += notificacion.Modelo.Sum(x => x.monto);
                montoIVA += notificacion.Modelo.Sum(x => x.montoIVA);
                montoComisionBancaria += notificacion.Modelo.Sum(x => x.montoComisionBancaria);
                montoAhorro += notificacion.Modelo.Sum(x => x.ahorro);

                int posXFooter = 285;
                if (montoAgregarProductos > 0)
                {
                    monto -= montoAgregarProductos;
                }

                suCambioAgregarProductos = montoPagadoAgregarProductos - montoAgregarProductos;
                montoPagado = notificacion.Modelo[0].montoPagado;
                suCambio = montoPagado - monto - montoIVA - montoComisionBancaria;

                Rectangle datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);
               
                if (indexProducto == lstTickets.Count() )
                {
                    datosfooter1 = new Rectangle(0, datosProducto.Y, 295, 15);
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, datosfooter1, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    pintaFooterIndice++;
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;

                if (indexProducto == lstTickets.Count() +1)
                {
                    e.Graphics.DrawString("  SUBTOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString(monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    pintaFooterIndice++;
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
                    pintaFooterIndice++;
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
                    pintaFooterIndice++;
                }


                if (this.insertaPagina(datosfooter1.Y, ref e)) return;

                if (indexProducto == lstTickets.Count() + 4)
                {
                    e.Graphics.DrawString("  TOTAL:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((monto + montoIVA + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    pintaFooterIndice++;
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;


                if (indexProducto == lstTickets.Count() + 5)
                {
                    e.Graphics.DrawString("_____________________________________________________" + " \n", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    pintaFooterIndice++;
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;

                if (indexProducto == lstTickets.Count() + 6)
                {
                    e.Graphics.DrawString("  RECIBIDO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((montoPagado).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                     pintaFooterIndice++;
                }

                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                if (indexProducto == lstTickets.Count() + 7)
                {
                    e.Graphics.DrawString("  SU CAMBIO:", font, drawBrush, 0, datosfooter1.Y, izquierda);
                    e.Graphics.DrawString((suCambio).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")), font, drawBrush, posXFooter, datosfooter1.Y, derecha);
                    datosfooter1.Y += espaciado;
                    indexProducto++;
                    pintaFooterIndice++;
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
                if (indexProducto == lstTickets.Count() + 8)
                {
                    if (montoAhorro > 0)
                    {
                        Rectangle datosAhorro = new Rectangle(0, datosfooter1.Y + 20, 280, 82);
                        e.Graphics.DrawString("******* USTED AHORRO:  " + (montoAhorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " *******", font, drawBrush, datosAhorro, centrado);
                        datosfooter1.Y += espaciado;
                        indexProducto++;
                        pintaFooterIndice++;
                    }
                }
                if (this.insertaPagina(datosfooter1.Y, ref e)) return;
                Rectangle datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
                if (indexProducto >= lstTickets.Count() + 9)
                {
                    datosfooter2 = new Rectangle(0, datosfooter1.Y + 30, 280, 82);
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

        public void pintaTablaProductos(object sender, ref PrintPageEventArgs e, Notificacion<List<Ticket>> notificacion)
        {

            float monto = 0;
            float montoIVA = 0;
            float montoComisionBancaria = 0;
            float montoAhorro = 0;
            float montoPagado = 0;
            float suCambio = 0;

            float montoPagadoAgregarProductos = 0;
            float montoAgregarProductos = 0;
            float suCambioAgregarProductos = 0;

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

            Rectangle datosProducto = new Rectangle(5, 285, 145, 82);
            Rectangle datosCantidad = new Rectangle(152, 285, 30, 82);
            Rectangle datosPrecioU = new Rectangle(190, 285, 30, 82);
            Rectangle datosPrecio = new Rectangle(220, 285, 48, 82);

            Rectangle datosEnca = new Rectangle(0, 215, 280, 82);
            int itemperpage = 1;
            for (int i = 0; i < notificacion.Modelo.Count(); i++)
            {
                e.Graphics.DrawString(notificacion.Modelo[i].descProducto.ToString() + " \n", font, drawBrush, datosProducto, izquierda);
                e.Graphics.DrawString(notificacion.Modelo[i].cantidad.ToString() + " \n", font, drawBrush, datosCantidad, izquierda);
                e.Graphics.DrawString(notificacion.Modelo[i].precioVenta.ToString() + " \n", font, drawBrush, datosPrecioU, izquierda);
                e.Graphics.DrawString((notificacion.Modelo[i].monto + notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);

                monto += notificacion.Modelo[i].monto;
                montoIVA += notificacion.Modelo[i].montoIVA;
                montoComisionBancaria += notificacion.Modelo[i].montoComisionBancaria;
                montoAhorro += notificacion.Modelo[i].ahorro;

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

                // si hay descuentos por mayoreo o rango de precios
                if (notificacion.Modelo[i].ahorro > 0)
                {
                    e.Graphics.DrawString("     └Descuento por mayoreo" + " \n", font, drawBrush, datosProducto, izquierda);
                    e.Graphics.DrawString("-" + (notificacion.Modelo[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + " \n", font, drawBrush, datosPrecio, derecha);
                    datosProducto.Y += espaciado;
                    datosCantidad.Y += espaciado;
                    datosPrecioU.Y += espaciado;
                    datosPrecio.Y += espaciado;
                }

                if (itemperpage < 20) // check whether  the number of item(per page) is more than 20 or not
                {
                    itemperpage += 1; // increment itemperpage by 1
                    e.HasMorePages = false; // set the HasMorePages property to false , so that no other page will not be added

                }

                else // if the number of item(per page) is more than 20 then add one page
                {
                    itemperpage = 0; //initiate itemperpage to 0 .
                    e.HasMorePages = true; //e.HasMorePages raised the PrintPage event once per page .
                    return;//It will call PrintPage event again
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Notificacion<List<Ticket>> ticket = new VentasDAO().ObtenerTickets(new Ticket() { idVenta = this.idVenta });
                byte[] myByteArray = Utils.GeneraTicketPDF(ticket.Modelo);
                //MemoryStream stream = new MemoryStream(myByteArray);
                Stream stream = new MemoryStream(myByteArray);
                string pdfCodigos = Convert.ToBase64String(myByteArray);

                string Filepath = @"E:/Desktop/123.pdf";
                // The name of the PDF that will be printed (just to be shown in the print queue)
                string Filename = "1.pdf";
                // The name of the printer that you want to use
                // Note: Check step 1 from the B alternative to see how to list
                // the names of all the available printers with C#
                string PrinterName = "EPSON TM-T20III Receipt";
                IPrinter printer = new Printer();
                printer.PrintRawStream(PrinterName, stream, Filename);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //PdfDocument doc = new PdfDocument();
                //doc.LoadFromFile("E:/Desktop/2.pdf");

                ////Use the default printer to print all the pages
                ////doc.PrintDocument.Print();

                ////Set the printer and select the pages you want to print

                //PrintDialog dialogPrint = new PrintDialog();
                //dialogPrint.AllowPrintToFile = true;
                //dialogPrint.AllowSomePages = true;
                //dialogPrint.PrinterSettings.MinimumPage = 1;
                //dialogPrint.PrinterSettings.MaximumPage = doc.Pages.Count;
                //dialogPrint.PrinterSettings.FromPage = 1;
                //dialogPrint.PrinterSettings.ToPage = doc.Pages.Count;

                //if (dialogPrint.ShowDialog() == DialogResult.OK)
                //{
                //    doc.PrintFromPage = dialogPrint.PrinterSettings.FromPage;
                //    doc.PrintToPage = dialogPrint.PrinterSettings.ToPage;
                //    doc.PrinterName = dialogPrint.PrinterSettings.PrinterName;

                //    PrintDocument printDoc = doc.PrintDocument;
                //    printDoc.Print();
                //}
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string Filepath = @"E:/Desktop/1.pdf";
            try
            {
                using (PrintDialog Dialog = new PrintDialog())
                {
                    Dialog.ShowDialog();

                    ProcessStartInfo printProcessInfo = new ProcessStartInfo()
                    {
                        Verb = "print",
                        CreateNoWindow = true,
                        FileName = Filepath,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    Process printProcess = new Process();
                    printProcess.StartInfo = printProcessInfo;
                    printProcess.Start();

                    printProcess.WaitForInputIdle();

                    Thread.Sleep(3000);

                    if (false == printProcess.CloseMainWindow())
                    {
                        printProcess.Kill();
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string printer = "EPSON TM-T20III Receipt";
            string paperName = "Roll Paper 80 x 3276 mm";
            string filename = "E:/Desktop/2.pdf"; ;
            int copies = 1;

            try
            {
                // Create the printer settings for our printer
                var printerSettings = new PrinterSettings
                {
                    PrinterName = printer,
                    Copies = (short)copies,
                };

                // Create our page settings for the paper size selected
                var pageSettings = new PageSettings(printerSettings)
                {
                    Margins = new Margins(0, 0, 0, 0),
                };
                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                {
                    if (paperSize.PaperName == paperName)
                    {
                        pageSettings.PaperSize = paperSize;
                        break;
                    }
                }

                // Now print the PDF document
                using (var document = PdfDocument.Load(filename))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}