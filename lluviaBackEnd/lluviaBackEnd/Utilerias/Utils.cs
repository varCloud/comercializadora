using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using lluviaBackEnd.Models.Facturacion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using ZXing;

namespace lluviaBackEnd.Utilerias
{
    public static class Utils
    {
        public static byte[] GenerarCodigoBarras(string cadena)
        {
            System.Drawing.Image img = null;
            using (var ms = new MemoryStream())
            {
                var writer = new ZXing.BarcodeWriter() { Format = BarcodeFormat.CODE_128 };
                writer.Options.Height = 80;
                writer.Options.Width = 280;
                writer.Options.PureBarcode = false;
                img = writer.Write(cadena);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static byte[] GenerarQR(string cadena)
        {
            System.Drawing.Image img = null;
            using (var ms = new MemoryStream())
            {
                var writer = new BarcodeWriter() { Format = BarcodeFormat.QR_CODE };
                writer.Options.Height = 200;
                writer.Options.Width = 200;
                img = writer.Write(cadena);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static void GenerarQRSAT(Comprobante c , string path)
        {
            var data = "&id=" + c.Complemento.TimbreFiscalDigital.UUID;
            data = data + "&re=" + c.Emisor.Rfc;
            data = data + "&rr=" + c.Receptor.Rfc;
            data = data + "&tt=" + c.Total.ToString("#.##");
            data = data + "&fe=" + c.Sello.Substring(c.Sello.Length - 8);
            string cadena = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx" + data;
            System.Drawing.Image img = null;
            using (var ms = new MemoryStream())
            {
                var writer = new BarcodeWriter() { Format = BarcodeFormat.QR_CODE };
                writer.Options.Height = 200;
                writer.Options.Width = 200;
                img = writer.Write(cadena);
                img.Save(path+".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        public static string ObtnerFolder()
        {
            string ruta = string.Empty;
            try
            {
                ruta = HttpContext.Current.Server.MapPath("~" + WebConfigurationManager.AppSettings["pathFacturas"].ToString());
                DateTime fecha = System.DateTime.Now;

                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);
                string[] directorio = System.IO.Directory.GetDirectories(ruta);

                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fecha.Month).ToUpper();
                ruta = Path.Combine(ruta, fecha.Year.ToString());
                if (directorio.ToList().Exists(p => p.Equals(ruta)))
                {
                    directorio = Directory.GetDirectories(ruta);
                    ruta = Path.Combine(ruta, nombreMes);
                    if (!Directory.Exists(ruta))
                        Directory.CreateDirectory(ruta);

                }
                else
                {
                    ruta = Path.Combine(ruta, nombreMes);
                    Directory.CreateDirectory(ruta);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("AL OBTENER LA RUTA DEL PDF", ex);
            }
            return ruta;
        }

        public static void GenerarFactura(Comprobante c, string path)
        {
            
                
                Document document = new Document(PageSize.A4, 30, 30, 30, 110);
                MemoryStream memStream = new MemoryStream();
                MemoryStream memStreamReader = new MemoryStream();
                PdfWriter PDFWriter = PdfWriter.GetInstance(document, memStream);
                ItextEvents eventos = new ItextEvents();
                eventos.TituloCabecera = "Articulos de limpieza lluvia";
                PDFWriter.PageEvent = eventos;
                try
                {
                    DateTime fechaActual = System.DateTime.Now;
                    DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                    string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                    string html = "<p align='center' ><b>" + eventos.TituloCabecera + @"</b></p><br />";

                    html += "<p align='right'> " + fechaActual.Day.ToString() + " de " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombreMes.ToLower()) + " del " + fechaActual.Year.ToString() + @"</p><br />";


                    string html1 = @"<table>
                        <tr>
                            <td colspan='4' >Datos del Emisor </td>    
                        </tr>
                        <tr>

                            <td>RFC: </td> 
                            <td>Tipo de comprobante: </td>
                            <td>Lugar de expedición: </td>
                            <td>Regimen Fiscal: </td>
                        </tr>
                        <tr>
                            <td>" + c.TipoDeComprobante + @" </td>   
                            <td>" + c.Emisor.Rfc + @" </td> 
                            <td>" + c.LugarExpedicion+@"</td>    
                            <td>"+c.Emisor.RegimenFiscal+@"</td>
                        </tr>
                        </table>";

                    string html2 = @"<table>
                        <tr>
                            <td colspan='3'>Información del Pago</td>    
                        </tr>
                        <tr>
                           
                            <td>Forma de pago: </td>
                            <td>Metodo de pago: </td><td>" + c.MetodoPago + @" </td>   
                            <td>Moneda: </td>
                        </tr>
                        <tr>
                                <td>" + c.MetodoPago + @" </td>   
                                <td>" + c.FormaPago + @" </td>  
                                <td>" + c.Moneda + @"</td>    
                        </tr>
                        </table>";

                string html3 = @"
                       <table>
                        <tr>
                            <td colspan='3'>Datos del cliente</td>    
                        </tr>
                        <tr>
                           
                            <td>Cliente: </td><td>"+c.Receptor.Rfc+ @"</td>
                            <td>R.F.C: </td><td>" + c.Receptor.Rfc + @"</td>
                            <td>Domiclio: </td><td>" + c.MetodoPago + @" </td>   
                            <td>Uso CFDI: </td><td>" + c.Receptor.UsoCFDI + @" </td> 
                        </tr>
                        </table>";

                    string html4 = @"
                        <tr>
                            <td>4. </td>
                            <td>Nombre del banco que lleva la cuenta de depósito a la vista o de ahorro en la que se realizará
                                el cargo: ____________________________________________________________________ .
                            </td>
                        </tr>";

                    string html5 = @"
                        <tr>
                            <td>5. </td>
                            <td>Cualquiera de los Datos de identificación de la cuenta, siguientes:
                                Número de tarjeta de débito (16 dígitos): _______________________________________;
                            </td>
                        </tr>";
                    string html6 = @"
                        <tr>
                            <td>6. </td>
                            <td>Monto máximo fijo del cargo autorizado por periodo de facturación: $ __________________ .
                                En lugar del monto máximo fijo, tratándose del pago de créditos revolventes asociados a tarjetas
                                de crédito, el titular de la cuenta podrá optar por autorizar alguna de las opciones de cargo
                                siguientes:
                            </td>
                        </tr>";

                    string html6Complemento = @"
                        <tr><td colspan='2'>(Marcar con una X la opción que, en su caso, corresponda)</td> </tr>
                        <tr><td colspan='2'>El importe del pago mínimo del periodo: ( &nbsp; ),</td> </tr>
                        <tr><td colspan='2'>El saldo total para no generar intereses en el periodo ( &nbsp; ), o</td> </tr>
                        <tr><td colspan='2'>Un monto fijo: ( &nbsp; ) (Incluir monto) $ _________________.</td> </tr>
                        ";

                    string html7 = @"
                        <tr>
                            <td>7. </td>
                            <td>Esta autorización es por plazo indeterminado ( &nbsp; ), o vence el: _________________________ .
                            </td>
                        </tr>
                        <tr>
                            <td width='3%'> </td>
                            <td width='97%'> </td>
                        </tr>
                    </table>";
                    html += html1 + html2 + html3 + html4 + html5 + html6 + html6Complemento + html7;

                    html += @"<p  >Estoy enterado de que en cualquier momento podré solicitar la cancelación de la presente
                         domiciliación sin costo a mi cargo.</p>";


                    html += "<br /><p align='center'  >Atentamente, <br /> <br />  Victor Adrian Reyes <br />" +
                        "______________________________________________<br />" +
                        "(NOMBRE O RAZÓN SOCIAL DEL TITULAR DE LA CUENTA)</p>";

                    //html = "<h1>Formato para solicitar la Domiciliación</h1>";
                    document.Open();
                    foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                    {
                        document.Add(E);

                    }
                    document.AddAuthor("LLUVIA");
                    document.AddTitle("Factura");
                    document.AddCreator("Victor Adrian Reyes");
                    document.AddSubject("Factura");
                    document.CloseDocument();
                    document.Close();

                    //PdfReader reader = new PdfReader(memStream.ToArray());
                    //PdfEncryptor.Encrypt(reader, memStreamReader, true, "secret", "secret", PdfWriter.ALLOW_PRINTING);
                    byte[] content = memStream.ToArray();
                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(content, 0, (int)content.Length);
                    }

                    //return content;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            

        }

    }
}