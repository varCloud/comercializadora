using ImageMagick;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using lluviaBackEnd.Models;
using lluviaBackEnd.Models.Facturacion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
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
                //img.Save(ObtnerFolderCodigos() + "barras_" + cadena + "_.jpg");
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
                //img.Save(ObtnerFolderCodigos() + "QR_" + cadena + "_.jpg");
                return ms.ToArray();
            }
        }


        public static byte[] GenerarCodigoBarrasGuardado(string cadena)
        {
            System.Drawing.Image img = null;
            using (var ms = new MemoryStream())
            {
                var writer = new ZXing.BarcodeWriter() { Format = BarcodeFormat.CODE_128 };
                writer.Options.Height = 80;
                writer.Options.Width = 280;
                writer.Options.PureBarcode = false;
                img = writer.Write(cadena);
                //img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                img.Save(ObtnerFolderCodigos() + "barras_" + cadena.Replace("/","_") + "_.jpg");
                return ms.ToArray();
            }
        }

        public static byte[] GenerarQRGuardado(string cadena)
        {
            System.Drawing.Image img = null;
            using (var ms = new MemoryStream())
            {
                var writer = new BarcodeWriter() { Format = BarcodeFormat.QR_CODE };
                writer.Options.Height = 200;
                writer.Options.Width = 200;
                img = writer.Write(cadena);
                //img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                img.Save(ObtnerFolderCodigos() + "QR_" + cadena.Replace("/","_") + "_.jpg");
                return ms.ToArray();
            }
        }


        public static byte[] GenerarQR(string cadena, string nombreArchivo)
        {
            System.Drawing.Image img = null;
            using (var ms = new MemoryStream())
            {
                var writer = new BarcodeWriter() { Format = BarcodeFormat.QR_CODE };
                writer.Options.Height = 200;
                writer.Options.Width = 200;
                img = writer.Write(cadena);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                img.Save(ObtnerFolderCodigos() + "QR_" + nombreArchivo + "_.jpg");
                return ms.ToArray();
            }
        }

        public static byte[] GenerarCodigoBarras(string cadena, string nombreArchivo)
        {
            System.Drawing.Image img = null;
            using (var ms = new MemoryStream())
            {
                var writer = new ZXing.BarcodeWriter() { Format = BarcodeFormat.CODE_128 };
                writer.Options.Height = 80;
                writer.Options.Width = 150;
                writer.Options.PureBarcode = false;
                img = writer.Write(cadena);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                img.Save(ObtnerFolderCodigos() + "barras_" + nombreArchivo + "_.jpg");
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
                writer.Options.Height = 150;
                writer.Options.Width = 150;
                img = writer.Write(cadena);
                img.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                /*using (var images = new MagickImageCollection(ms.ToArray()))
                {
                    images.AppendHorizontally().Format = MagickFormat.Jpeg;
                    images.AppendHorizontally().Quality = 0;
                    images.AppendHorizontally().Write(Path.Combine(path));
                }*/
                
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

        public static string ObtnerAnoMesFolder()
        {
            string ruta = string.Empty;
            try
            {
                //ruta = HttpContext.Current.Server.MapPath("~" + WebConfigurationManager.AppSettings["pathFacturas"].ToString());
                DateTime fecha = System.DateTime.Now;                
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fecha.Month).ToUpper();
                ruta = Path.Combine(ruta, fecha.Year.ToString());
                ruta = Path.Combine(ruta, nombreMes);
            }
            catch (Exception ex)
            {
                throw new Exception("call ObtnerAnoMesFolder()", ex);
            }
            return ruta;
        }

        public static void GenerarFactura(Comprobante c, string path ,string idVenta)
        {
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:"+ TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            string titulosCabeceras = "style='font-weight:bold;  color:3b3b3b;text-align:center'";
            string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
            string color1 = "bgcolor='#edeceb' style='color:7b7b7b;text-align:center;font-size:7px;' ";
            string color2 = "style='color:7b7b7b; text-align:center; font-size:7px;'";
            string centradas = "style='text-align:center;'";
            string titulosCabecerasAbre = "bgcolor='7D7D7D' style='font-weight:bold;  color:white;text-align:center'";
            string tituloIndividual = "style='font-weight:bold;  color:3b3b3b;'";
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
                                <td>Fecha</td>
                        </tr>
                        <tr " + centradas + @">
                                <td>" + c.Folio + @"</td>
                               <td>"  + c.Fecha+ @"</td>
                        </tr>
                        </table>";

               string DatosDelCliente = @"
                       <table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                            <tr " + cabeceraTablas + @">
                                <td colspan='4'>Datos del cliente</td>    
                            </tr>
                            <tr>
                                <td "+ tituloIndividual+@" >Cliente</td> <td colspan ='3'>" + c.Receptor.Nombre + @"</td>           
                            </tr>
                            <tr>
                                 <td " + tituloIndividual + @" >R.F.C </td> <td>" + c.Receptor.Rfc + @"</td> <td " + tituloIndividual + @">Uso CFDI </td><td>" + c.Receptor.UsoCFDI+" - "+c.Addenda.descripcionUsoCFDI + @" </td>  
                            </tr>
                            <tr>
                                <td " + tituloIndividual + @" >Domiclio</td> <td colspan ='3'>CALLE GALEANA No. 59, COLONIA LA MAGDALENA, C.P. 60080, URUAPAN, MICHOACAN, MEXICO </td> 
                            </tr>
                            <tr>
                                <td  width='15%'></td>
                                <td  width='35%'></td> 
                                <td  width='15%'></td> 
                                <td  width='35%'></td> 
                            </tr>
                        </table>";


                string DatosDelEmisor = @"<table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                        <tr "+ cabeceraTablas + @">
                            <td colspan='3' >Datos del Emisor </td>    
                        </tr>
                        <tr "+ titulosCabeceras + @">
                            <td>RFC </td> 
                            <td>Tipo de comprobante </td>
                            <td>Lugar de expedición </td>
                          
                        </tr>
                        <tr " + centradas + @">
                            <td>" + c.Emisor.Rfc + @" </td> 
                            <td>" + c.TipoDeComprobante+" - "+c.Addenda.descripcionTipoComprobante + @" </td>   
                            <td>" + c.LugarExpedicion + @"</td>    
                        </tr>
                        </table>";

                string informacionDelPago = @"<table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                        <tr " + cabeceraTablas + @">
                            <td colspan='3'>Información del Pago</td>    
                        </tr>
                       <tr " + titulosCabeceras + @">
                            <td>Forma de pago </td>
                            <td>Metodo de pago </td> 
                            <td>Moneda </td>
                        </tr>
                        <tr " + centradas + @">
                                <td>" + c.FormaPago +" - "+c.Addenda.descripcionFormaPago+@" </td>   
                                <td>" + c.MetodoPago + "-  Pago en una sola exhibición" + @" </td>  
                                <td>" + c.Moneda + @"- Peso Mexicano</td>    
                        </tr>
                        </table>";

  

                string html4 = @" 
                        <table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                        <tr " + cabeceraTablas + @">
                            <td colspan='9'>Productos</td>    
                        </tr>
                        <tr "+ titulosCabeceras + @">
                            <td>Cantidad </td>
                            <td>Unidad</td>
                            <td>Clave Unidad</td> 
                            <td>Clave Producto / Servicio</td> 
                            <td>Concepto </td>
                            <td>Valor Unitario</td>
                            <td>Desc.</td>
                            <td>IVA</td>
                            <td>Importe</td>
                        </tr>";
                int i = 0;
                foreach (var item in c.Addenda.conceptosAddenda)
                {
                    html4 += "<tr " + (i % 2 == 0 ? color1 : color2)+" >";
                    html4 += "<td>" + item.Cantidad + "</td>";
                    html4 += "<td>" + item.Unidad+"</td>";
                    html4 += "<td>" + item.ClaveUnidad + " - " + item.DescripcionClaveUnidad + " </td>";
                    html4 += "<td>" + item.ClaveProdserv+" - "+item.DescripcionClaveProdServ + "</td>";
                    html4 += "<td>" + item.Descripcion+"</td>";
                    html4 += "<td>" + item.ValorUnitario.ToString("c2") + "</td>";
                    html4 += "<td>$0.00</td>";
                    html4 += "<td>"+item.IVA.ToString("C2")+"</td>";
                    html4 += "<td>"+item.Importe.ToString("C2")+"</td>";
                    html4 += "</tr>";
                    i++;
                }
                html4 += @"
                    <tr> 
                        <td colspan='6'></td>
                        <td colspan='2' " + tituloIndividual + @">Subtotal:</td>
                        <td>" + c.SubTotal.ToString("C2")+ @"</td>
                    </tr>
                    <tr> 
                        <td colspan='6'></td>
                        <td colspan='2' " + tituloIndividual + @">Impuestos Trasladados:</td>
                        <td>" + c.Impuestos.TotalImpuestosTrasladados.ToString("C2") + @"</td>
                    </tr>
                     <tr> 
                        <td colspan='6'></td>
                        <td colspan='2' " + tituloIndividual + @">Total:</td>
                        <td>" + c.Total.ToString("C2") + @"</td>
                    </tr>
                    <tr>
                        <td  width='10%'></td>
                        <td  width='10%'></td>
                        <td  width='10%'></td>
                        <td  width='13%'></td>
                        <td  width='10%'></td>
                        <td  width='10%'></td> 
                        <td  width='9%'></td> 
                        <td  width='9%'></td> 
                        <td  width='9%'></td> 
                    </tr>";
                html4 += "</table>";


                string html5 = @" 
                        <table width='100%' " + cssTabla + @"  CELLPADDING='0' >
                        <tr " + cabeceraTablas + @">
                            <td colspan='2'> ESTE DOCUMENTO ES UNA REPRESENTACIÓN IMPRESA DE UN CFDI</td>    
                        </tr>
                        <tr " + titulosCabecerasAbre + @">
                            <td>Numero de Certificado </td>
                            <td>Lugar de Expedicion</td>
                        </tr>
                         <tr " + centradas + @">
                            <td>" + c.Complemento.TimbreFiscalDigital.NoCertificadoSAT+ @" </td>
                            <td>MORELIA" + " " + "MICHOACÁN" + @"</td>
                        </tr>
                        <tr " + titulosCabecerasAbre + @">
                            <td>No. de serie del certificado del sat </td>
                            <td>Folio Fiscal</td>
                        </tr>
                         <tr " + centradas + @">
                            <td>" + c.NoCertificado + @" </td>
                            <td>"+c.Complemento.TimbreFiscalDigital.UUID+ @"</td>
                        </tr>
                        <tr " + titulosCabecerasAbre + @">
                            <td>Fecha y hora de certificación </td>
                            <td>Régimen fiscal</td>
                        </tr>
                         <tr " + centradas + @">
                            <td>" + c.Complemento.TimbreFiscalDigital.FechaTimbrado + @" </td>
                            <td>" + c.Emisor.RegimenFiscal+" Incorporación fiscal"+" "+ @"</td>
                        </tr>
                    </table>";

               string QR = @"<table  width='100%'>
                    <tr>
                        <td width='80%'>
                            <table width='100%' height='100%'   style='font-size:8px;font-family:Arial;color:7b7b7b;'" + @"  CELLPADDING='0' >
                                <tr>
                                    <td style='color:black;' ><b>CADENA ORIGINAL DEL COMPLEMENTO DE CERTIFICACIÓN DIGITAL DEL SAT </b></td>
                                </tr>
                                <tr>
                                    <td  style='white-space: nowrap'>"+ CadenaComplementos(c.Complemento.TimbreFiscalDigital) + @"</td>
                                </tr>
                                <tr>
                                    <td style='color:black;'><b>SELLO DIGITAL EMISOR</b></td>
                                </tr>
                                <tr>
                                    <td> " + c.Sello + @"</td>
                                </tr>
                                <tr>
                                    <td style='color:black;' ><b>SELLO DIGITAL SAT</b></td>
                                </tr>
                                <tr>
                                    <td> " + c.Complemento.TimbreFiscalDigital.SelloSAT + @"</td>
                                </tr>
                            </table>
                        </td>
                        <td width='20%'>
                                        <img src='" + Path.Combine(path, "Qr_"+idVenta+".jpg") + @"' width = '110' height = '110' align='right' />
                        </td> 
                   </tr>
                </table>";


                html += DatosDelEmisor + informacionDelPago + DatosDelCliente + html4 + html5+QR;

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
                using (FileStream fs = File.Create(Path.Combine(path,"Factura_"+idVenta+".pdf")))
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

        private static string CadenaComplementos(TimbreFiscalDigital t)
        {

            string x = string.Empty;
            if (t != null)
            {
                x = "|" + t.Version.ToString().Replace("|", "") + "|" + t.UUID.ToString().Replace("|", "") + "|" + t.FechaTimbrado.ToString().Replace("|", "") + "|" + t.SelloCFD.ToString().Replace("|", "") + "|" + t.NoCertificadoSAT.ToString().Replace("|", "") + "|";

                x = x.Replace("\n", " ").ToString().Replace("\r", " ").ToString().Replace("\t", " ").Replace(" ", "");
                x = "|" + x + "|";
            }
            return x.Replace(" ", "").ToString().Insert(95, " &nbsp;");

        }
               
        public static string ObtnerFolderCodigos()
        {
            string ruta = string.Empty;
            try
            {
                ruta = HttpContext.Current.Server.MapPath("~" + WebConfigurationManager.AppSettings["pathPdfCodigos"].ToString());
                DateTime fecha = System.DateTime.Now;

                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);
            }
            catch (Exception ex)
            {
                throw new Exception("AL OBTENER LA RUTA DEL PDF", ex);
            }
            return ruta;
        }

        public static byte[] GenerarImprimibleCodigos(string path, string articulo, string producto)
        {
            byte[] content = null;
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:" + TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
            Document document = new Document(PageSize.A4, 30, 30, 15, 15);
            MemoryStream memStream = new MemoryStream();
            MemoryStream memStreamReader = new MemoryStream();
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, memStream);
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "Códigos del Producto: ";
            Utilerias.Utils.GenerarQRGuardado(articulo);
            Utilerias.Utils.GenerarCodigoBarrasGuardado(articulo);
            articulo = articulo.Replace("/", "_");
            int renglonesQR = 5;
            int renglonesBarras = 10;

            //PDFWriter.PageEvent = eventos;
            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                string html = "<br/>";

                // codigos QR
                html += @"<table width='100%' " + cssTabla + @"  CELLPADDING='1' >
                        <tr " + cabeceraTablas + @">
                            <td colspan='4' >Producto: " + producto + @" </td>    
                        </tr>";

                for (int i = 0; i < renglonesQR; i++)
                {
                    html += @"<tr>";
                    html += @"   <td><img src='" + Path.Combine(path, "QR_" + articulo + "_.jpg") + @"' width = '150' height = '150' align='right' /></td>";
                    html += @"   <td><img src='" + Path.Combine(path, "QR_" + articulo + "_.jpg") + @"' width = '150' height = '150' align='right' /></td>";
                    html += @"   <td><img src='" + Path.Combine(path, "QR_" + articulo + "_.jpg") + @"' width = '150' height = '150' align='right' /></td>";
                    html += @"   <td><img src='" + Path.Combine(path, "QR_" + articulo + "_.jpg") + @"' width = '150' height = '150' align='right' /></td>";
                    html += @"</tr>";

                }

                html += "</table> <br><br> ";


                // codigos de barras
                html += @"<table width='100%' " + cssTabla + @"  CELLPADDING='6' >
                        <tr " + cabeceraTablas + @">
                            <td colspan='3' >Producto: " + producto + @" </td>    
                        </tr>";

                for (int i = 0; i < renglonesBarras; i++)
                {
                    html += @"<tr>";
                    html += @"   <td><img src='" + Path.Combine(path, "barras_" + articulo + "_.jpg") + @"' width = '196' height = '56' align='left' /></td>";
                    html += @"   <td><img src='" + Path.Combine(path, "barras_" + articulo + "_.jpg") + @"' width = '196' height = '56' align='left' /></td>";
                    html += @"   <td><img src='" + Path.Combine(path, "barras_" + articulo + "_.jpg") + @"' width = '196' height = '56' align='left' /></td>";
                    //html += @"   <td><img src='" + Path.Combine(path, "barras_" + articulo + "_.jpg") + @"' width = '210' height = '60' align='right' /></td>";
                    html += @"</tr>";

                }

                html += "</table>";

                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("LLUVIA");
                document.AddTitle("Codigos: " + producto);
                document.AddCreator("Victor Adrian Reyes");
                document.AddSubject("Codigos de Productos");
                document.CloseDocument();
                document.Close();

                content = memStream.ToArray();
                DeleteFile(ObtnerFolderCodigos() + "QR_" + articulo + "_.jpg");
                DeleteFile(ObtnerFolderCodigos() + "barras_" + articulo + "_.jpg");


                /* using (FileStream fs = File.Create(Path.Combine(path, "Codigos_" + articulo + ".pdf")))
                 {
                     fs.Write(content, 0, (int)content.Length);
                 }*/

            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return "Codigos_" + articulo + ".pdf";
            return content;
        }

        public static void DeleteFile(string nameFile)
        {
            try
            {
                if (File.Exists(nameFile))
                    File.Delete(nameFile);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string ObtnerFolderTickets()
        {
            string ruta = string.Empty;
            try
            {
                ruta = HttpContext.Current.Server.MapPath("~" + WebConfigurationManager.AppSettings["pathPdfTickets"].ToString());
                //DateTime fecha = System.DateTime.Now;

                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);
            }
            catch (Exception ex)
            {
                throw new Exception("AL OBTENER LA RUTA DEL PDF", ex);
            }
            return ruta;
        }


        public static byte[] GeneraTicketPDF(List<Ticket> tickets)
        {
            byte[] content = null;

            //string pathPdfTickets = ObtnerFolderTickets() + @"/";
            string rutaPDF = string.Empty;
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:" + TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            //string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
            Document document = new Document(PageSize.A4, 0, 0, 10, 0);
            MemoryStream memStream = new MemoryStream();
            MemoryStream memStreamReader = new MemoryStream();
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, memStream);
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "Ver Ticket: ";
            string nombreArchivo = string.Empty;
            string path = Utils.ObtnerFolderCodigos() + @"/";

            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                string html = "";
                float monto = 0;
                float montoIVA = 0;
                float montoComisionBancaria = 0;
                float montoAhorro = 0;
                float montoPagado = 0;
                float suCambio = 0;

                html +=
                  @"<table  width='100%'>
                    <tr>
                        <td width='35%'>
                            <table width='100%' height='100%'   style='font-size:6.8px;font-family:Arial;color:7b7b7b;'" + @"  CELLPADDING='0' >
                                <tr>
                                    <td><img src='" + System.Web.HttpContext.Current.Server.MapPath("~") + "\\assets\\img\\logo_lluvia_150.jpg" + @"' width = '78' height = '63' align='center' /></td>
                                </tr>                                
                                
                                <tr><td style='color:black; text-align:center;'>RFC:COVO781128LJ1, </td></tr>
                                <tr><td style='color:black; text-align:center;'>Calle Macarena #82 </td></tr>
                                <tr><td style='color:black; text-align:center;'>Inguambo </td></tr>
                                <tr><td style='color:black; text-align:center;'>Uruapan, Michoacán </td></tr>
                                <tr><td style='color:black; text-align:center;'>C.p. 58000 </td></tr>
                                <tr><td style='color:black; text-align:center;'><br></td></tr>
                                
                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>                                            
                                            <td style='color:black; text-align:center;'>Ticket: " + tickets[0].idVenta.ToString() + @"</td>
                                            <td>" +  tickets[0].fechaAlta.ToString("dd-MM-yyyy") + @"</td>
                                          </tr>
                                          <tr>
                                            <td></td>
                                            <td>Hora: " + tickets[0].fechaAlta.ToShortTimeString() + @"</td>
                                          </tr>
                                          <tr>
                                            <td colspan=""2"">Cliente: " + tickets[0].nombreCliente.ToString().ToUpper() + @"</td>
                                          </tr>
                                          <tr>
                                            <td colspan=""2"">Forma de Pago: " + tickets[0].descFormaPago.ToString() + @"</td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr><td style='color:black; '>____________________________________________________</td></tr>
                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='50%'>Descripcion</td>
                                            <td width='25%' style='color:black; text-align:center;'>Cantidad</td>
                                            <td width='25%' style='color:black; text-align:center;'>Precio</td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr><td style='color:black; '>____________________________________________________</td></tr>";


                                for (int i = 0; i < tickets.Count(); i++)
                                {
                                    monto += tickets[i].monto;
                                    montoIVA += tickets[i].montoIVA;
                                    montoComisionBancaria += tickets[i].montoComisionBancaria;
                                    montoAhorro += tickets[i].ahorro;

                                    html+= @"   <tr>
                                                    <td style='color:black; '> 
                                                        <table>
                                                          <tr>
                                                            <td width='60%'>" + tickets[i].descProducto.ToString() + @"</td>
                                                            <td width='15%'>" + tickets[i].cantidad.ToString() + @"</td>
                                                            <td width='15%' style='color:black; text-align:right;'>" + (tickets[i].monto + tickets[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                                            <td width='10%' style='color:black; text-align:left;'></td>
                                                          </tr>
                                                        </table>
                                                    </td>
                                                </tr>";

                                    if(tickets[i].ahorro > 0)
                                    {
                                        html += @"   <tr>
                                                        <td style='color:black; '> 
                                                            <table>
                                                              <tr>
                                                                <td width='60%'>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; -Descuento por mayoreo" + @"</td>
                                                                <td width='15%'></td>
                                                                <td width='15%' style='color:black; text-align:right;'>" + "-" + (tickets[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                                                <td width='10%' style='color:black; text-align:left;'></td>
                                                              </tr>
                                                            </table>
                                                        </td>
                                                    </tr>";
                                    }

                                }


                html += @"
                                <tr><td style='color:black; '>____________________________________________________</td></tr>

                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='65%'>SUBTOTAL:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>";

                if (montoComisionBancaria > 0)
                {
                    html += @"   <tr>
                                    <td style='color:black; '> 
                                        <table>
                                            <tr>
                                            <td width='65%'>COMISIÓN BANCARIA:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + (montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>";
                }

                if (montoIVA > 0)
                {
                    html+= @"   <tr>
                                    <td style='color:black; '> 
                                        <table>
                                            <tr>
                                            <td width='65%'>I.V.A:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + (montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>";
                }

                montoPagado = tickets[0].montoPagado;
                suCambio = montoPagado - monto - montoIVA - montoComisionBancaria;


                        html+=@" <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='65%'>TOTAL:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + (monto + montoIVA + montoComisionBancaria).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr><td style='color:black; '>____________________________________________________</td></tr>

                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='65%'>RECIBIDO:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + montoPagado.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='65%'>SU CAMBIO:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + suCambio.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>";

                                if (montoAhorro > 0)
                                {
                                    html+= @"
                                                <tr><td style='color:black; text-align:center;'><br></td></tr>
                                                <tr><td style='color:black; text-align:center;'>******* USTED AHORRO:  " + (montoAhorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @" ******* </td></tr>
                                                <tr><td style='color:black; text-align:center;'>******** GRACIAS POR SU PREFERENCIA. ******** </td></tr>";
                                }
                                else
                                {
                                    html += @"  <tr><td style='color:black; text-align:center;'><br></td></tr>
                                                <tr><td style='color:black; text-align:center;'>******** GRACIAS POR SU PREFERENCIA. ******** </td></tr>";

                                }
                // se agrega el codigo de barras en el ticket 
                nombreArchivo = tickets[0].codigoBarras.ToString();
                Utilerias.Utils.GenerarCodigoBarras(tickets[0].codigoBarras.ToString(), nombreArchivo);


                html += @"
                          <tr>
                            <td style='text-align:center;'  align='center' >
                                <div  align='center' style='text-align:center;' >
                                    <br>
                                    <img src='" + Path.Combine(path, "barras_" + nombreArchivo + "_.jpg") + @"' width = '90' height = '30' align='center' style='text-align:center;' />
                                </div>
                            </td>
                         </tr>
                         ";                


                html += @"
                            </table>
                        </td>
                        <td width='65%'>                                        
                        </td> 
                   </tr>
                </table>";


                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("LLUVIA");
                document.AddTitle("Ticket: " + tickets[0].idVenta.ToString());
                document.AddCreator("Victor Adrian Reyes");
                document.AddSubject("Visualizacion de Ticket");
                document.CloseDocument();

                document.Close();
                content = memStream.ToArray();
                DeleteFile(Path.Combine(path, "barras_" + nombreArchivo + "_.jpg"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return content;
        }




        public static byte[] GeneraTicketDevolucion(List<Ticket> tickets)
        {
            byte[] content = null;

            string rutaPDF = string.Empty;
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:" + TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            Document document = new Document(PageSize.A4, 0, 0, 10, 0);
            MemoryStream memStream = new MemoryStream();
            MemoryStream memStreamReader = new MemoryStream();
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, memStream);
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "Ticket de Devolución: ";
            string nombreArchivo = string.Empty;
            string path = Utils.ObtnerFolderCodigos() + @"/";

            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                string html = "";
                float monto = 0;

                html +=
                  @"<table  width='100%'>
                    <tr>
                        <td width='35%'>
                            <table width='100%' height='100%'   style='font-size:6.8px;font-family:Arial;color:7b7b7b;'" + @"  CELLPADDING='0' >
                                <tr>
                                    <td><img src='" + System.Web.HttpContext.Current.Server.MapPath("~") + "\\assets\\img\\logo_lluvia_150.jpg" + @"' width = '78' height = '63' align='center' /></td>
                                </tr>                                
                                
                                <tr><td style='color:black; text-align:center;'>**************************************************************</td></tr>
                                <tr><td style='color:black; text-align:center;'>*************** TICKET DE DEVOLUCIÓN ***************</td></tr>
                                <tr><td style='color:black; text-align:center;'>**************************************************************</td></tr>
                                <tr><td style='color:black; text-align:center;'><br></td></tr>
                                
                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>                                            
                                            <td style='color:black; text-align:center;'>Ticket: " + tickets[0].idVenta.ToString() + @"</td>
                                            <td>" + tickets[0].fechaAlta.ToString("dd-MM-yyyy") + @"</td>
                                          </tr>
                                          <tr>
                                            <td></td>
                                            <td>Hora: " + tickets[0].fechaAlta.ToShortTimeString() + @"</td>
                                          </tr>
                                          <tr>
                                            <td colspan=""2"">Cliente: " + tickets[0].nombreCliente.ToString().ToUpper() + @"</td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr><td style='color:black; '>____________________________________________________</td></tr>
                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='50%'>Descripcion</td>
                                            <td width='25%' style='color:black; text-align:center;'>Art. Devueltos</td>
                                            <td width='25%' style='color:black; text-align:center;'>Precio</td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr><td style='color:black; '>____________________________________________________</td></tr>";


                for (int i = 0; i < tickets.Count(); i++)
                {
                    monto += tickets[i].monto;

                    html += @"   <tr>
                                                    <td style='color:black; '> 
                                                        <table>
                                                          <tr>
                                                            <td width='60%'>" + tickets[i].descProducto.ToString() + @"</td>
                                                            <td width='15%'>" + tickets[i].cantidad.ToString() + @"</td>
                                                            <td width='15%' style='color:black; text-align:right;'>" + (tickets[i].monto).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                                            <td width='10%' style='color:black; text-align:left;'></td>
                                                          </tr>
                                                        </table>
                                                    </td>
                                                </tr>";

                }

                html += @"   <tr>
                                                    <td style='color:black; '> 
                                                        <table>
                                                          <tr><td style='color:black;'>____________________________________________________</td></tr>
                                                        </table>
                                                        <table>
                                                          <tr>
                                                            <td width='60%'>TOTAL DEVUELTO:</td>
                                                            <td width='15%'> </td>
                                                            <td width='15%' style='color:black; text-align:right;'>" + (monto).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                                            <td width='10%' style='color:black; text-align:left;'></td>
                                                          </tr>
                                                        </table>
                                                    </td>
                                                </tr>";

                //html += @" <tr>
                //                    <td style='color:black; '> 
                //                        <table>
                //                          <tr><td style='color:black;'>____________________________________________________</td></tr>
                //                          <tr>
                //                            <td width='55%'>TOTAL DEVUELTO:</td>
                //                            <td width='25%' style='color:black; text-align:right;'>" + (monto).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                //                            <td width='00%' style='color:black; text-align:left;'> </td>
                //                          </tr>
                //                        </table>
                //                    </td>
                //                </tr>";


                html += @"
                                <tr><td style='color:black; text-align:center;'><br></td></tr>
                                <tr><td style='color:black; text-align:center;'><br></td></tr>
                                <tr><td style='color:black; text-align:center;'><br></td></tr>
                                <tr><td style='color:black; text-align:center;'>_________________________________________________</td></tr>
                                <tr><td style='color:black; text-align:center;'>FIRMA DEL CLIENTE </td></tr>";

                // se agrega el codigo de barras en el ticket 
                nombreArchivo = tickets[0].codigoBarras.ToString();
                Utilerias.Utils.GenerarCodigoBarras(tickets[0].codigoBarras.ToString(), nombreArchivo);


                html += @"
                          <tr>
                            <td style='text-align:center;'  align='center' >
                                <div  align='center' style='text-align:center;' >
                                    <br>
                                    <img src='" + Path.Combine(path, "barras_" + nombreArchivo + "_.jpg") + @"' width = '90' height = '30' align='center' style='text-align:center;' />
                                </div>
                            </td>
                         </tr>
                         ";


                html += @"
                            </table>
                        </td>
                        <td width='65%'>                                        
                        </td> 
                   </tr>
                </table>";


                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("LLUVIA");
                document.AddTitle("Ticket: " + tickets[0].idVenta.ToString());
                document.AddCreator("Victor Adrian Reyes");
                document.AddSubject("Ticket de Devolución");
                document.CloseDocument();
                document.Close();
                content = memStream.ToArray();
                DeleteFile(Path.Combine(path, "barras_" + nombreArchivo + "_.jpg"));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return content;
        }

        public static byte[] GeneraTicketDespachadoresPDF(List<Ticket> tickets)
        {
            byte[] content = null;

            //string pathPdfTickets = ObtnerFolderTickets() + @"/";
            string rutaPDF = string.Empty;
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:" + TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            //string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
            Document document = new Document(PageSize.A4, 0, 0, 10, 0);
            MemoryStream memStream = new MemoryStream();
            MemoryStream memStreamReader = new MemoryStream();
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, memStream);
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "Ver Ticket para Despachadores: ";

            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                string html = "";
                float monto = 0;
                float montoIVA = 0;  

                html +=
                  @"<table  width='100%'>
                    <tr>
                        <td width='35%'>
                            <table width='100%' height='100%'   style='font-size:6.8px;font-family:Arial;color:7b7b7b;'" + @"  CELLPADDING='0' >
                                <tr>
                                    <td><img src='" + System.Web.HttpContext.Current.Server.MapPath("~") + "\\assets\\img\\logo_lluvia_150.jpg" + @"' width = '78' height = '63' align='center' /></td>
                                </tr>                                
                                
                                <tr><td style='color:black; text-align:center;'>**************************************************************</td></tr>
                                <tr><td style='color:black; text-align:center;'>TICKET PARA DESPACHADORES</td></tr>
                                <tr><td style='color:black; text-align:center;'>**************************************************************</td></tr>
                                <tr><td style='color:black; text-align:center;'><br></td></tr>
                                
                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>                                            
                                            <td style='color:black; text-align:center;'>Ticket: " + tickets[0].idVenta.ToString() + @"</td>
                                            <td>" + tickets[0].fechaAlta.ToString("dd-MM-yyyy") + @"</td>
                                          </tr>
                                          <tr>
                                            <td></td>
                                            <td>Hora: " + tickets[0].fechaAlta.ToShortTimeString() + @"</td>
                                          </tr>
                                          <tr>
                                            <td colspan=""2"">Cliente: " + tickets[0].nombreCliente.ToString().ToUpper() + @"</td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr><td style='color:black; '>____________________________________________________</td></tr>
                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='50%'>Descripcion</td>
                                            <td width='25%' style='color:black; text-align:center;'>Cantidad</td>
                                            <td width='25%' style='color:black; text-align:center;'>Precio</td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr><td style='color:black; '>____________________________________________________</td></tr>";


                for (int i = 0; i < tickets.Count(); i++)
                {
                    monto += tickets[i].monto;
                    montoIVA += tickets[i].montoIVA;                

                    html += @"   <tr>
                                                    <td style='color:black; '> 
                                                        <table>
                                                          <tr>
                                                            <td width='60%'>" + tickets[i].descProducto.ToString() + @"</td>
                                                            <td width='15%'>" + tickets[i].cantidad.ToString() + @"</td>
                                                            <td width='15%' style='color:black; text-align:right;'>" + (tickets[i].monto + tickets[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                                            <td width='10%' style='color:black; text-align:left;'></td>
                                                          </tr>
                                                        </table>
                                                    </td>
                                                </tr>";

                }


                html += @"
                                <tr><td style='color:black; '>____________________________________________________</td></tr>

                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='65%'>SUBTOTAL:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>";

                if (montoIVA > 0)
                {
                    html += @"   <tr>
                                    <td style='color:black; '> 
                                        <table>
                                            <tr>
                                            <td width='65%'>I.V.A:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + (montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>";
                }

                html += @" <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='65%'>TOTAL:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + (monto + montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>";


                    html += @"  <tr><td style='color:black; text-align:center;'><br></td></tr>
                                                <tr><td style='color:black; text-align:center;'>******** GRACIAS POR SU PREFERENCIA. ******** </td></tr>";

        


                html += @"
                            </table>
                        </td>
                        <td width='65%'>                                        
                        </td> 
                   </tr>
                </table>";


                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("LLUVIA");
                document.AddTitle("Ticket: " + tickets[0].idVenta.ToString());
                document.AddCreator("Victor Adrian Reyes");
                document.AddSubject("Visualizacion de Ticket");
                document.CloseDocument();

                document.Close();
                content = memStream.ToArray();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return content;
        }




        public static Dictionary<string,object> GenerarUbicaciones(List<Ubicacion> ubicaciones, string path)
        {
            byte[] content = null;
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:" + TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
            Document document = new Document(PageSize.A4, 30, 30, 15, 15);
            MemoryStream memStream = new MemoryStream();
            MemoryStream memStreamReader = new MemoryStream();
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, memStream);
            PDFWriter.CloseStream = false;
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "Ubicaciones: ";
            string ubica = string.Empty;
            string nombreArchivo = string.Empty;
            int renglonesQR = 4;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                string html = "<br/>";
                string tds = string.Empty;
                html += @"<table width='100%' " + cssTabla + @"  CELLPADDING='1' border='0'>";
                
                int i = 0;
                int renglones = (ubicaciones.Count / 4) + 1;
                for (int c = 0; c < renglones; c++) {
                    if (i < ubicaciones.Count)
                    {

                        for (int indexCol = 0; indexCol < 4; indexCol++)
                        {
                            if (i < ubicaciones.Count)
                            {
                                ubica = "{\"idAlmacen\": \"" + ubicaciones[i].idAlmacen.ToString() + "\", \"idPasillo\": \"" + ubicaciones[i].idPasillo.ToString() + "\", \"Pasillo\": \"" + ubicaciones[i].descripcionPasillo.ToString().Trim() + "\", \"idRack\": \"" + ubicaciones[i].idRaq.ToString() + "\", \"Rack\": \"" + ubicaciones[i].descripcionRaq.ToString() + "\", \"idPiso\": \"" + ubicaciones[i].idPiso.ToString() + "\", \"Piso\": \"" + ubicaciones[i].descripcionPiso.ToString() + "\"}";
                                nombreArchivo = "A" + ubicaciones[i].idAlmacen.ToString() + "P" + ubicaciones[i].idPiso.ToString() + "P" + ubicaciones[i].descripcionPasillo.ToString() + "R" + ubicaciones[i].idRaq.ToString() + "";
                                Utilerias.Utils.GenerarQR(ubica, nombreArchivo);


                                tds += @"<td style='text-align:center;'  align='center' ><div  align='center' style='text-align:center;' ><img src='" + Path.Combine(path, "QR_" + nombreArchivo + "_.jpg") + @"' width = '120' height = '120' align='center' style='text-align:center;' /></div>";
                                tds += @"<p style='color:black; text-align:center;' >Almacen:" + ubicaciones[i].descripcionAlmacen.ToString() + @" Pasillo: " + ubicaciones[i].descripcionPasillo.ToString() + ", Rack: " + ubicaciones[i].idRaq.ToString() + ", Piso: " + ubicaciones[i].idPiso.ToString() + "</p>";
                                tds += @"</td>";
                                i++;
                            }
                            else {
                                tds += @"<td>";
                                tds += @"</td>";
                            }

                        }
                        html += @" <tr>" + tds + "</tr>";
                        tds = string.Empty;
                    }
                    
                }
                
                //for (int i = 0; i < ubicaciones.Count(); i++)
                //{
                //    html += @"<tr>";
                //    html += @"<td style='color:black; text-align:left;'>Almacen:" + ubicaciones[i].descripcionAlmacen.ToString() + @" Pasillo: " + ubicaciones[i].descripcionPasillo.ToString() + ", Rack: " + ubicaciones[i].idRaq.ToString() + ", Piso: " + ubicaciones[i].idPiso.ToString() + @"</td>";
                //    //html += @"   <td style='color:black; text-align:right;'> Pasillo: " + ubicaciones[i].descripcionPasillo.ToString() + ", Rack: " + ubicaciones[i].idRaq.ToString() + ", Piso: " + ubicaciones[i].idPiso.ToString() + @"</td>";
                //    //html += @"   <td style='color:black; text-align:right;'> Pasillo: " + ubicaciones[i].descripcionPasillo.ToString() + ", Rack: " + ubicaciones[i].idRaq.ToString() + ", Piso: " + ubicaciones[i].idPiso.ToString() + @"</td>";
                //    //html += @"   <td style='color:black; text-align:right;'> Pasillo: " + ubicaciones[i].descripcionPasillo.ToString() + ", Rack: " + ubicaciones[i].idRaq.ToString() + ", Piso: " + ubicaciones[i].idPiso.ToString() + @"</td>";
                //    html += @"</tr>";

                //    //}
                //}
                html += "</table>";
                        //if (i == ubicaciones.Count() - 1)
                        //{
                        //    html += "</table> <br><br><br><br><br> ";
                        //}
                        //else
                        //{
                        //    html += "</table> <br><br><br><br><br><br> ";
                        //}



                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("LLUVIA");
                document.AddTitle("Ubicaciones: ");
                document.AddCreator("Victor Adrian Reyes");
                document.AddSubject("Ubicaciones de Productos");
                document.CloseDocument();
                document.Close();
                content = memStream.ToArray();
                
                for (int l = 0; l < ubicaciones.Count(); l++)
                {
                   // DeleteFile(ObtnerFolderCodigos() + "QR_" + "A" + ubicaciones[l].idAlmacen.ToString() + "P" + ubicaciones[l].idPiso.ToString() + "P" + ubicaciones[l].descripcionPasillo.ToString() + "R" + ubicaciones[l].idRaq.ToString() + "" + "_.jpg");
                }

                memStream.Write(content, 0, content.Length);
                memStream.Position = 0;
                string nombreArchivoPDF = "Ubicaciones_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                using (FileStream fs = File.Create(Path.Combine(path, nombreArchivoPDF)))
                 {
                    fs.Write(content, 0, (int)content.Length);
                    fs.Flush();
                 }
                dic.Add("nombreArchivoPDF", nombreArchivoPDF);
                dic.Add("content", content);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dic;
        }

        public static Dictionary<string, object> GenerarCodigosBarras(List<Producto> productos, string path)
        {
            byte[] content = null;
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:" + TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            string cssTabla2 = @"style='text-align:center;font-size:14px;font-family:Arial; color:#3E3E3E'";
            string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
            Document document = new Document(PageSize.A4, 30, 30, 15, 15);
            MemoryStream memStream = new MemoryStream();
            MemoryStream memStreamReader = new MemoryStream();
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, memStream);
            PDFWriter.CloseStream = false;
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "Códigos de Barras: ";
            //string ubica = string.Empty;
            string nombreArchivo = string.Empty;
            int renglonesQR = 4;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                string html = "<br/>";
                string tds = string.Empty;
                html += @"<table width='100%' " + cssTabla + @"  CELLPADDING='0' border='1'>";

                int i = 0;
                int renglones = (productos.Count / 3) + 1;
                for (int c = 0; c < renglones; c++)
                {
                    if (i < productos.Count)
                    {

                        for (int indexCol = 0; indexCol < 2; indexCol++)
                        {
                            if (i < productos.Count)
                            {
                                //ubica = "{\"idAlmacen\": \"" + ubicaciones[i].idAlmacen.ToString() + "\", \"idPasillo\": \"" + ubicaciones[i].idPasillo.ToString() + "\", \"Pasillo\": \"" + ubicaciones[i].descripcionPasillo.ToString().Trim() + "\", \"idRack\": \"" + ubicaciones[i].idRaq.ToString() + "\", \"Rack\": \"" + ubicaciones[i].descripcionRaq.ToString() + "\", \"idPiso\": \"" + ubicaciones[i].idPiso.ToString() + "\", \"Piso\": \"" + ubicaciones[i].descripcionPiso.ToString() + "\"}";
                                nombreArchivo = "Br_" + productos[i].idProducto.ToString() + "_" + productos[i].codigoBarras.ToString().Replace("/", "").Replace("'",""); //"A" + ubicaciones[i].idAlmacen.ToString() + "P" + ubicaciones[i].idPiso.ToString() + "P" + ubicaciones[i].descripcionPasillo.ToString() + "R" + ubicaciones[i].idRaq.ToString() + "";
                                Utilerias.Utils.GenerarCodigoBarras(productos[i].codigoBarras, nombreArchivo);

                                tds += @"<td style='text-align:center;'  align='center' >";

                                tds += @"<table width='100%' " + cssTabla + @"  CELLPADDING='5' border='0'> 
                                            <tr> 
                                                <td style='text-align:center;' align='center' > <p style='color:black; text-align:center;' >" + productos[i].descripcion.ToString() + "</p> <br>  ";
                                tds +=              @"<div align='center' style='text-align:center;'><img src='" + Path.Combine(path, "barras_" + nombreArchivo + "_.jpg") + @"' width='125' height='51' align='center' style='text-align:center;' /></div>" +

                                               @"</td>  " +
                                               @"<td style='text-align:center;' align='center' >
                                                    <table width='100%' " + cssTabla2 + @"  CELLPADDING='5' border='0'> 
                                                        <tr>
                                                            <td>
                                                               <div align='right' style='text-align:center;' font-size: '14px'>  Menudeo: $ " + productos[i].precioIndividual.ToString() + @" </div>
                                                            </td>
                                                        </tr>    
                                                        <tr>
                                                            <td>
                                                               <div align='left' style='text-align:center;' font-size: 14px>  Mayoreo: $ " + productos[i].precioMenudeo.ToString() + @" </div>

                                                                
                                                            </td>
                                                        </tr>    
                                                    </table>" +
                                               @"</td>  
                                            </tr> 
                                        </table>";

                                //tds += @"<p style='color:black; text-align:center;' >" + productos[i].descripcion.ToString() + "</p><br>";
                                //tds += @"<div align='center' style='text-align:center;'><img src='" + Path.Combine(path, "barras_" + nombreArchivo + "_.jpg") + @"' width='125' height='51' align='center' style='text-align:center;' /></div>";
                                //tds += @"<table width='100%' " + cssTabla + @"  CELLPADDING='5' border='0'> <tr> <td style='text-align:center;' align='center' > <p style='color:black; text-align:center;' >$ Menudeo:" + productos[i].precioIndividual.ToString() + "</p>   </td>  <td style='text-align:center;' align='center' > <p style='color:black; text-align:center;' >$ Mayoreo: " + productos[i].precioMenudeo.ToString() + "</p>   </td>  </tr> </table>";
                                tds += @"</td>";

                                i++;
                            }
                            else
                            {
                                tds += @"<td>";
                                tds += @"</td>";
                            }

                        }
                        html += @" <tr>" + tds + "</tr>";
                        tds = string.Empty;
                    }

                }

                html += "</table>";
              
                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("LLUVIA");
                document.AddTitle("Códigos de Barras: ");
                document.AddCreator("Victor Adrian Reyes");
                document.AddSubject("Códigos de Barras de Productos");
                document.CloseDocument();
                document.Close();
                content = memStream.ToArray();
                memStream.Write(content, 0, content.Length);
                memStream.Position = 0;
                string nombreArchivoPDF = "Codigos_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                using (FileStream fs = File.Create(Path.Combine(path, nombreArchivoPDF)))
                {
                    fs.Write(content, 0, (int)content.Length);
                    fs.Flush();
                }
                dic.Add("nombreArchivoPDF", nombreArchivoPDF);
                dic.Add("content", new byte[8]);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dic;
        }

        public static byte[] GeneraTicketPDFPedidoEspecial(PedidosEspeciales pedido )
        {
            byte[] content = null;

            //string pathPdfTickets = ObtnerFolderTickets() + @"/";
            string rutaPDF = string.Empty;
            string TamañoLetra = "10px";
            string cssTabla = @"style='text-align:center;font-size:" + TamañoLetra + ";font-family:Arial; color:#3E3E3E'";
            //string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
            Document document = new Document(PageSize.A4, 0, 0, 10, 0);
            MemoryStream memStream = new MemoryStream();
            MemoryStream memStreamReader = new MemoryStream();
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, memStream);
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "Pedido Especial: ";

            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                string html = "";
                float monto = 0;
                float montoIVA = 0;
                float montoAhorro = 0;

                html +=
                  @"<table  width='100%'>
                    <tr>
                        <td width='35%'>
                            <table width='100%' height='100%'   style='font-size:6.8px;font-family:Arial;color:7b7b7b;'" + @"  CELLPADDING='0' >
                                <tr>
                                    <td><img src='" + System.Web.HttpContext.Current.Server.MapPath("~") + "\\assets\\img\\logo_lluvia_150.jpg" + @"' width = '78' height = '63' align='center' /></td>
                                </tr>                                
                                
                                <tr><td style='color:black; text-align:center;'>************************************************************************** </td></tr>
                                <tr><td style='color:black; text-align:center;'>**                          PEDIDO ESPECIAL                             ** </td></tr>
                                <tr><td style='color:black; text-align:center;'>************************************************************************** </td></tr>
                                <tr><td style='color:black; text-align:center;'><br><br></td></tr>
                                
                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>                                            
                                            <td style='color:black; text-align:center;'># Pedido: " + pedido.idPedidoEspecial.ToString() + @"</td>
                                            <td>" + pedido.fechaAlta.ToString("dd-MM-yyyy") + @"</td>
                                          </tr>
                                          <tr>
                                            <td></td>
                                            <td>Hora: " + pedido.fechaAlta.ToShortTimeString() + @"</td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr><td style='color:black; '>____________________________________________________</td></tr>
                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='50%'>Descripcion</td>
                                            <td width='25%' style='color:black; text-align:center;'>Cantidad</td>
                                            <td width='25%' style='color:black; text-align:center;'>Precio</td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr><td style='color:black; '>____________________________________________________</td></tr>";


                for (int i = 0; i < pedido.lstPedidosInternosDetalle.Count(); i++)
                {
                    monto += pedido.lstPedidosInternosDetalle[i].monto;
                    montoIVA += pedido.lstPedidosInternosDetalle[i].montoIVA;
                    montoAhorro += pedido.lstPedidosInternosDetalle[i].ahorro;

                    html += @"   <tr>
                                                    <td style='color:black; '> 
                                                        <table>
                                                          <tr>
                                                            <td width='60%'>" + pedido.lstPedidosInternosDetalle[i].descProducto.ToString() + @"</td>
                                                            <td width='15%'>" + pedido.lstPedidosInternosDetalle[i].cantidad.ToString() + @"</td>
                                                            <td width='15%' style='color:black; text-align:right;'>" + ( pedido.lstPedidosInternosDetalle[i].monto + pedido.lstPedidosInternosDetalle[i].ahorro ).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                                            <td width='10%' style='color:black; text-align:left;'></td>
                                                          </tr>
                                                        </table>
                                                    </td>
                                                </tr>";

                    if (pedido.lstPedidosInternosDetalle[i].cantidad > 0) //ahorro
                    {
                        html += @"   <tr>
                                                        <td style='color:black; '> 
                                                            <table>
                                                              <tr>
                                                                <td width='60%'>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; -Descuento por mayoreo" + @"</td>
                                                                <td width='15%'></td>
                                                                <td width='15%' style='color:black; text-align:right;'>" + "-" + (pedido.lstPedidosInternosDetalle[i].ahorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                                                <td width='10%' style='color:black; text-align:left;'></td>
                                                              </tr>
                                                            </table>
                                                        </td>
                                                    </tr>";
                    }

                }


                html += @"
                                <tr><td style='color:black; '>____________________________________________________</td></tr>

                                <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='65%'>SUBTOTAL:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + monto.ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>";

                //if (montoIVA > 0)
                //{
                //    html += @"   <tr>
                //                    <td style='color:black; '> 
                //                        <table>
                //                            <tr>
                //                            <td width='65%'>I.V.A:</td>
                //                            <td width='25%' style='color:black; text-align:right;'>" + (montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                //                            <td width='10%' style='color:black; text-align:left;'></td>
                //                            </tr>
                //                        </table>
                //                    </td>
                //                </tr>";
                //}

                html += @" <tr>
                                    <td style='color:black; '> 
                                        <table>
                                          <tr>
                                            <td width='65%'>TOTAL:</td>
                                            <td width='25%' style='color:black; text-align:right;'>" + (monto + montoIVA).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @"</td>
                                            <td width='10%' style='color:black; text-align:left;'></td>
                                          </tr>
                                        </table>
                                    </td>
                                </tr>";

                if (montoAhorro > 0)
                {
                    html += @"
                                                <tr><td style='color:black; text-align:center;'><br></td></tr>
                                                <tr><td style='color:black; text-align:center;'>******* USTED AHORRO:  " + (montoAhorro).ToString("C2", CultureInfo.CreateSpecificCulture("en-US")) + @" ******* </td></tr>
                                                <tr><td style='color:black; text-align:center;'>******** GRACIAS POR SU PREFERENCIA. ******** </td></tr>";
                }
                else
                {
                    html += @"  <tr><td style='color:black; text-align:center;'><br></td></tr>
                                                <tr><td style='color:black; text-align:center;'>******** GRACIAS POR SU PREFERENCIA. ******** </td></tr>";

                }


                html += @"
                            </table>
                        </td>
                        <td width='65%'>                                        
                        </td> 
                   </tr>
                </table>";


                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("LLUVIA");
                document.AddTitle("Ticket: " + pedido.idPedidoEspecial.ToString());
                document.AddCreator("Victor Adrian Reyes");
                document.AddSubject("Visualizacion de Ticket");
                document.CloseDocument();

                document.Close();
                content = memStream.ToArray();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return content;
        }


        public static bool mercanciaAcomodada(string idPasillo, string idRaq, string idPiso)
        {
            return (idPasillo == "0" && idRaq == "0" && idPiso == "0") || idPasillo == "ninguno" && idRaq == "ninguno" && idPiso == "ninguno" ? true : false;
        }

        public static string EscribirLog(string mensaje)
        {
            string resultado = "";
            try
            {
                string fechaHora = "Fecha :" + System.DateTime.Now.ToShortDateString() + "\n" + "Hora :" + System.DateTime.Now.ToShortTimeString() + "\n" + mensaje;
                fechaHora = fechaHora.Length > 32766 ? fechaHora.Substring(0, 32750) : fechaHora;

                //mensaje = "*****************************************************************************************" + "\n" + mensaje + "\n" + "\n" + "\n";

                string folder = Path.Combine(ConfigurationManager.AppSettings["pathLog"].ToString());
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                System.IO.File.AppendAllText(Path.Combine(folder, @"Log" + System.DateTime.Now.ToString("yyyyMMdd") + ".txt"), mensaje + "\n" + "\n" + "\n");
            }
            catch (Exception ex)
            {
                resultado = ex.Message;
            }
            return resultado;
        }



    }
}