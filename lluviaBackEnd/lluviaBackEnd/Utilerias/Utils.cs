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

        public static void GenerarQRSAT(Comprobante c, string path)
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
                img.Save(path + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
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
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:"+ TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            string titulosCabeceras = "style='font-weight:bold;  color:3b3b3b;text-align:center'";
            string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
            string color1 = "bgcolor='#edeceb' style='color:7b7b7b;text-align:center;font-size:8px;' ";
            string color2 = "style='color:7b7b7b; text-align:center; font-size:8px;'";
            string centradas = "style='text-align:center;'";
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
                string html = "<br/>";

                html += @"<table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                           <tr " + cabeceraTablas + @">
                            <td colspan='2' >Información general </td>    
                        </tr>
                         <tr " + titulosCabeceras + @">
                                <td>Folio</td>
                                <td>Fecha de Timbre</td>
                        </tr>
                        <tr " + centradas + @">
                                <td>" + c.Folio + @"</td>
                               <td>"  + c.Fecha+ @"</td>
                        </tr>
                        </table>";


                string html1 = @"<table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                        <tr "+ cabeceraTablas + @">
                            <td colspan='4' >Datos del Emisor </td>    
                        </tr>
                        <tr "+ titulosCabeceras + @">
                            <td>RFC </td> 
                            <td>Tipo de comprobante </td>
                            <td>Lugar de expedición </td>
                            <td>Regimen Fiscal </td>
                        </tr>
                        <tr " + centradas + @">
                            <td>" + c.Emisor.Rfc + @" </td> 
                            <td>" + c.TipoDeComprobante + @" </td>   
                            <td>" + c.LugarExpedicion + @"</td>    
                            <td>" + c.Emisor.RegimenFiscal + @"</td>
                        </tr>
                        </table>";

                string html2 = @"<table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                        <tr " + cabeceraTablas + @">
                            <td colspan='3'>Información del Pago</td>    
                        </tr>
                       <tr " + titulosCabeceras + @">
                            <td>Forma de pago </td>
                            <td>Metodo de pago </td> 
                            <td>Moneda </td>
                        </tr>
                        <tr " + centradas + @">
                                <td>" + c.MetodoPago + @" </td>   
                                <td>" + c.FormaPago + @" </td>  
                                <td>" + c.Moneda + @"</td>    
                        </tr>
                        </table>";

                string DatosDelCliente = @"
                       <table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                            <tr " + cabeceraTablas + @">
                                <td colspan='4'>Datos del cliente</td>    
                            </tr>
                            <tr  " + titulosCabeceras + @">
                                <td>Cliente</td>
                                <td>R.F.C </td>
                                <td>Uso CFDI </td>
                                <td>Domiclio</td>
                            </tr>
                            <tr " + centradas + @">
                                <td>" + c.Receptor.Rfc + @"</td>
                                <td>" + c.Receptor.Rfc + @"</td>
                                <td>" + c.MetodoPago + @" </td>  
                                <td>CALLE GALEANA No. 59, COLONIA LA MAGDALENA, C.P. 60080, URUAPAN, MICHOACAN, MEXICO </td> 
                            </tr>
                            <tr>
                                <td  width='15%'></td>
                                <td  width='15%'></td> 
                                <td  width='15%'></td> 
                                <td  width='55%'></td> 
                            </tr>
                        </table>";

                string html4 = @" 
                        <table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                        <tr " + cabeceraTablas + @">
                            <td colspan='8'>Productos</td>    
                        </tr>
                        <tr "+ titulosCabeceras + @">
                            <td>Cantidad </td>
                            <td>Unidad</td>
                            <td>Clave Unidad</td> 
                            <td>Concepto </td>
                            <td>Valor Unitario</td>
                            <td>Desc.</td>
                            <td>IVA</td>
                            <td>Importe</td>
                        </tr>";
                int i = 0;
                foreach (var item in c.Conceptos)
                {
                    html4 += "<tr " + (i % 2 == 0 ? color1 : color2)+" >";
                    html4 += "<td>" + item.Cantidad + "</td>";
                    html4 += "<td>" + item.Unidad + "</td>";
                    html4 += "<td>" + item.ClaveUnidad + "</td>";
                    html4 += "<td>" + item.Descripcion + "</td>";
                    html4 += "<td>" + item.ValorUnitario.ToString("c2") + "</td>";
                    html4 += "<td>$0.00</td>";
                    html4 += "<td>"+item.Impuestos.Traslados.Traslado.Importe.ToString("C2")+"</td>";
                    html4 += "<td>"+item.Importe.ToString("C2")+"</td>";
                    html4 += "</tr>";
                    i++;
                }
                html4 += @"
                    <tr> 
                        <td colspan='5'></td>
                        <td colspan='2'>Subtotal:</td>
                        <td>" + c.SubTotal.ToString("C2")+ @"</td>
                    </tr>
                    <tr> 
                        <td colspan='5'></td>
                        <td colspan='2'>Impuestos Trasladados:</td>
                        <td>" + c.Impuestos.TotalImpuestosTrasladados.ToString("C2") + @"</td>
                    </tr>
                     <tr> 
                        <td colspan='5'></td>
                        <td colspan='2'>Total:</td>
                        <td>" + c.Total.ToString("C2") + @"</td>
                    </tr>
                    <tr>
                        <td  width='10%'></td>
                        <td  width='10%'></td>
                        <td  width='10%'></td>
                        <td  width='30%'></td>
                        <td  width='10%'></td>
                        <td  width='8%'></td> 
                        <td  width='10%'></td> 
                        <td  width='10%'></td> 
                    </tr>";
                html4 += "</table>";


                html += html1 + html2 + DatosDelCliente + html4;

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