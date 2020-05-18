using ImageMagick;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using lluviaBackEnd.Models.Facturacion;
using System;
using System.Collections.Generic;
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

    }
}