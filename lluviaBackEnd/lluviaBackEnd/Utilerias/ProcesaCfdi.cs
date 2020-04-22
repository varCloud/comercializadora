using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Diagnostics;
using System.Configuration;
using System.Xml.XPath;
using System.Xml.Xsl;
using lluviaBackEnd.Models.Facturacion;
using lluviaBackEnd.servicioTimbrarPruebas;
using System.Net;

namespace lluviaBackEnd.Utilerias
{

    public static class ProcesaCfdi
    {

        private static string fileName;
        public static string pathArchivoSAT = string.Empty;

        public static string SerializaXML33(Comprobante comprobante)
        {
            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(Comprobante));
                MemoryStream ms = new MemoryStream();
                Encoding utf8EncodingWithNoByteOrderMark = new UTF8Encoding(false);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(ms, utf8EncodingWithNoByteOrderMark);

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                // se agregan los prefijos para en espacio de nombres 
                // este espacio de nombres existe en la definicion de las clases 
                // ejem [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
                // esto lo que hace es de tener <nombre>VICTOR</nombre>  a <cfdi:nombre></cfdi:nombre>
                // recuerda que debes de tener definido en namespace en la definicion de la clase 
                ns.Add("cfdi", "http://www.sat.gob.mx/cfd/3");

                //a todos las propiedades o clases que tengan [XmlAttributeAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
                //adjuntara el xsi por ejemplo "schemaLocation="http://www.sat.gob.mx/cfd/3 " a "xsi:schemaLocation="http://www.sat.gob.mx/cfd/3 "
                //es lo mismo que el de arriba
                ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");


                xmlTextWriter.Formatting = Formatting.Indented;
                serializer.Serialize(xmlTextWriter, comprobante, ns);
                //serializer.Serialize(xmlTextWriter, library, ns);

                ms = (MemoryStream)xmlTextWriter.BaseStream;
                string resultXml = Encoding.UTF8.GetString(ms.ToArray());


                //StringWriter strWriter = new StringWriter();
                //XmlSerializer serializer = new XmlSerializer(typeof(Comprobante));
                //serializer.Serialize(strWriter, comprobante);
                //string resultXml = strWriter.ToString();
                //strWriter.Close();

                return resultXml;

            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public static string GeneraCadenaOriginal33(string strXml)
        {
            //string path = ObtenerPath();
            string fileXslt = ConfigurationManager.AppSettings["cadenaOriginalXslt"].ToString();

            try
            {
                byte[] byteArrayXml = Encoding.UTF8.GetBytes(strXml);
                MemoryStream stream = new MemoryStream(byteArrayXml);

                //Cargar el XML
                //StreamReader reader = new StreamReader(path + fileXml);
                StreamReader reader = new StreamReader(stream);
                XPathDocument myXPathDoc = new XPathDocument(reader);

                //Cargando el XSLT
                XslCompiledTransform myXslTrans = new XslCompiledTransform();
                //myXslTrans.Load(Path.Combine(ProcesaCfdi.pathArchivoSAT, fileXslt));
                myXslTrans.Load(fileXslt);

                StringWriter str = new StringWriter();
                XmlTextWriter myWriter = new XmlTextWriter(str);

                //Aplicando transformacion
                myXslTrans.Transform(myXPathDoc, null, myWriter);

                //string p = myWriter.ToString() ;
                //Resultado
                string result = str.ToString();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Dictionary<string, string> ObtenerCertificado()
        {
            Dictionary<string, string> certificados = null;
            try
            {
                X509Certificate2 publicCert = new X509Certificate2(lluviaBackEnd.Resource.CerProductivo);
                byte[] data = FromHex(publicCert.GetSerialNumberString());
                string NoCertificado = Encoding.ASCII.GetString(data);
                Debug.WriteLine("no certificado :" + NoCertificado);
                data = null;
                data = FromHex(publicCert.GetPublicKeyString());
                string Certificado = Convert.ToBase64String(publicCert.GetRawCertData());
                Debug.WriteLine("certificado :" + Certificado);
                certificados = new Dictionary<string, string>();
                certificados.Add("NoCertificado", NoCertificado);
                certificados.Add("Certificado", Certificado);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return certificados;

        }

        private static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        public static string GeneraSello(string strCadenaOriginal)
        {
            string strSello = string.Empty;
            try
            {
                //PARA GENERAR EL SELLO SE UTILIZA EL MISMO CERTIFICADO  DE PRODUCTIVO LA MISMA CONTRASEÑA Y EL MISMO ARCHIVO.KEY 
                //LO UNICO QUE CAMBIE ES EN EL WEB SERVICE HAY QUE  APUNTAR AL WEB SERVICE DE PRUEBAS
                //CON LAS CONTRASEÑAS DE PRUEBAS
                string strLlavePwd = ConfigurationManager.AppSettings["claveGeneraSello"].ToString();
                System.Security.SecureString passwordSeguro = new System.Security.SecureString();
                passwordSeguro.Clear();
                foreach (char c in strLlavePwd.ToCharArray())
                    passwordSeguro.AppendChar(c);

                byte[] llavePrivadaBytes = lluviaBackEnd.Resource.keyProductivo;// ARCHIVO .KEY
                RSACryptoServiceProvider rsa = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, passwordSeguro);
                //SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
                SHA256CryptoServiceProvider hash265 = new SHA256CryptoServiceProvider();
                byte[] bytesFirmados = rsa.SignData(System.Text.Encoding.UTF8.GetBytes(strCadenaOriginal), hash265);
                strSello = Convert.ToBase64String(bytesFirmados);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strSello;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static XmlElement GenerateXmlSignature(XmlDocument originalXmlDocument)
        {
            string strLlavePwd = ConfigurationManager.AppSettings["claveGeneraSello"].ToString();
            X509Certificate2 cert = new X509Certificate2(Resource.archivopfx, strLlavePwd);
            RSACryptoServiceProvider Key = cert.PrivateKey as RSACryptoServiceProvider;
            SignedXml signedXml = new SignedXml(originalXmlDocument) { SigningKey = Key };
            Reference reference = new Reference() { Uri = String.Empty };
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            KeyInfoX509Data kdata = new KeyInfoX509Data(cert);
            kdata.AddIssuerSerial(cert.Issuer, cert.SerialNumber);
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(kdata);
            signedXml.KeyInfo = keyInfo;
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            return signedXml.GetXml();
        }

        public static object TimbrarEdifact(string xmlSerializadoSAT)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                servicioTimbrarPruebas.timbrarCFDIPortTypeClient timbrar = new servicioTimbrarPruebas.timbrarCFDIPortTypeClient();
                respuestaTimbrado respuesta = timbrar.timbrarCFDI("", "", xmlSerializadoSAT);
                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /*
        public static string ObtenerPath()
        {
            string path = @"ArchivosSAT\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                System.IO.File.WriteAllBytes(Path.Combine(Path.GetFullPath(path), "cer.cer"), Robot_Facturacion_Electronica_Dinamico.Properties.Resources.cer);
                System.IO.File.WriteAllBytes(Path.Combine(Path.GetFullPath(path), "key.cer"), Robot_Facturacion_Electronica_Dinamico.Properties.Resources.key);
                System.IO.File.WriteAllBytes(Path.Combine(Path.GetFullPath(path), "CerProductivo.cer"), Robot_Facturacion_Electronica_Dinamico.Properties.Resources.CerProductivo);
                System.IO.File.WriteAllBytes(Path.Combine(Path.GetFullPath(path), "keyProductivo.key"), Robot_Facturacion_Electronica_Dinamico.Properties.Resources.keyProductivo);
            }
            pathArchivoSAT = Path.GetFullPath(path);
            return Path.GetFullPath(path);
        }

        public static string MD5Hash(string pass)
        {
            MD5 md5 = MD5.Create("MD5");
            md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(pass));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            foreach (byte b in result)
            {
                strBuilder.Append(b.ToString("x2"));
            }
            return strBuilder.ToString();
        }

        public static byte[] ObtenerCFDQRCodeBytes(string resultXml)
        {
            Robot_Facturacion_Electronica_Dinamico.ServiceReference1.MassiveReceptionControllerImplClient servicioPrueba = null;
            Robot_Facturacion_Electronica_Dinamico.stoTimbradoProductivo.MassiveReceptionControllerImplClient servicioPrductivo = null;
            byte[] resul = null;

            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            System.Net.ServicePointManager.SecurityProtocol=
            SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => {
                return true; 
            };


            if (ConfigurationManager.AppSettings["Prueba"].ToString().Equals("0"))
            {
                try
                {
                    servicioPrueba = new Robot_Facturacion_Electronica_Dinamico.ServiceReference1.MassiveReceptionControllerImplClient();
                    byte[] xmlByte = Encoding.UTF8.GetBytes(resultXml);
                    resul = servicioPrueba.StampCFDQRCodeBytes(xmlByte, ConfigurationManager.AppSettings["usuarioWebServiceSTOPruebas"].ToString(), MD5Hash(ConfigurationManager.AppSettings["contraWebServiceSTOPruebas"].ToString()));

                }
                catch (Exception ex)
                {
                    throw new Exception("-- Al consumir WCF  :" + ex.Message, ex);
                }
            }
            else // SI ENTRA AQUI ES PRODUCTIVOCommerce
            {
                
                try
                {
                    servicioPrductivo = new Robot_Facturacion_Electronica_Dinamico.stoTimbradoProductivo.MassiveReceptionControllerImplClient();
                    byte[] xmlByte = Encoding.UTF8.GetBytes(resultXml);
                    resul = servicioPrductivo.StampCFDQRCodeBytes(xmlByte, ConfigurationManager.AppSettings["usuarioWebServiceSTOProd"].ToString(), MD5Hash(ConfigurationManager.AppSettings["contraWebServiceSTOProd"].ToString()));

                }
                catch (Exception ex)
                {
                    throw new Exception("-- Al consumir WCF  :" + ex.Message, ex);
                }
 
            }
            return resul;
        }

        public static byte[] CancelarMassCancelCFD(List<string> uuids,byte[] certificado)
        {
            Robot_Facturacion_Electronica_Dinamico.ServiceReference1.MassiveReceptionControllerImplClient servicioPrueba = null;
            Robot_Facturacion_Electronica_Dinamico.stoTimbradoProductivo.MassiveReceptionControllerImplClient servicioPrductivo = null;
            byte[] resul = null;

            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            System.Net.ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => {
                return true;
            };


            if (ConfigurationManager.AppSettings["Prueba"].ToString().Equals("0"))
            {
                try
                {
                    servicioPrueba = new Robot_Facturacion_Electronica_Dinamico.ServiceReference1.MassiveReceptionControllerImplClient();
                 
                    resul = servicioPrueba.MassCancelCFDi(uuids.ToArray(), 
                        ConfigurationManager.AppSettings["codigoFiscal"],certificado,
                        ConfigurationManager.AppSettings["CancelarPassCertPruebas"],
                        ConfigurationManager.AppSettings["usuarioWebServiceSTOPruebas"].ToString(), 
                        MD5Hash(ConfigurationManager.AppSettings["contraWebServiceSTOPruebas"].ToString()));

                }
                catch (Exception ex)
                {
                    throw new Exception("-- Al consumir WCF  :" + ex.Message, ex);
                }
            }
            else // SI ENTRA AQUI ES PRODUCTIVOCommerce
            {

                try
                {
                    servicioPrductivo = new Robot_Facturacion_Electronica_Dinamico.stoTimbradoProductivo.MassiveReceptionControllerImplClient();
                    resul = servicioPrductivo.MassCancelCFDi(uuids.ToArray(),
                        ConfigurationManager.AppSettings["codigoFiscal"], certificado,
                        ConfigurationManager.AppSettings["CancelarPassCertProductivo"],
                        ConfigurationManager.AppSettings["usuarioWebServiceSTOProd"].ToString(),
                        MD5Hash(ConfigurationManager.AppSettings["contraWebServiceSTOProd"].ToString()));
                }
                catch (Exception ex)
                {
                    throw new Exception("-- Al consumir WCF  :" + ex.Message, ex);
                }

            }
            return resul;
        }
           
        public static string GeneraCadenaOriginal(string strXml)
        {
            string path = ObtenerPath();
            string fileXslt = ConfigurationManager.AppSettings["nombreArchivoCO"].ToString();

            try
            {
                byte[] byteArrayXml = Encoding.UTF8.GetBytes(strXml);
                MemoryStream stream = new MemoryStream(byteArrayXml);

                //Cargar el XML
                //StreamReader reader = new StreamReader(path + fileXml);
                StreamReader reader = new StreamReader(stream);
                XPathDocument myXPathDoc = new XPathDocument(reader);

                //Cargando el XSLT
                XslCompiledTransform myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(Path.Combine(ProcesaCfdi.pathArchivoSAT, fileXslt));

                StringWriter str = new StringWriter();
                XmlTextWriter myWriter = new XmlTextWriter(str);

                //Aplicando transformacion
                myXslTrans.Transform(myXPathDoc, null, myWriter);

                //string p = myWriter.ToString() ;
                //Resultado
                string result =  str.ToString();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GeneraSello(string strCadenaOriginal)
        {
            string strSello = string.Empty;
            try
            {
                //PARA GENERAR EL SELLO SE UTILIZA EL MISMO CERTIFICADO  DE PRODUCTIVO LA MISMA CONTRASEÑA Y EL MISMO ARCHIVO.KEY 
                //LO UNICO QUE CAMBIE ES EN EL WEB SERVICE HAY QUE  APUNTAR AL WEB SERVICE DE PRUEBAS
                //CON LAS CONTRASEÑAS DE PRUEBAS
                string strLlavePwd = ConfigurationManager.AppSettings["claveGeneraSello"].ToString();
                System.Security.SecureString passwordSeguro = new System.Security.SecureString();
                passwordSeguro.Clear();
                foreach (char c in strLlavePwd.ToCharArray())
                    passwordSeguro.AppendChar(c);

                byte[] llavePrivadaBytes = Robot_Facturacion_Electronica_Dinamico.Properties.Resources.keyProductivo;// ARCHIVO .KEY
                RSACryptoServiceProvider rsa = opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, passwordSeguro);
                //SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
                SHA256CryptoServiceProvider hash265 = new SHA256CryptoServiceProvider();
                byte[] bytesFirmados = rsa.SignData(System.Text.Encoding.UTF8.GetBytes(strCadenaOriginal), hash265);
                strSello = Convert.ToBase64String(bytesFirmados);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strSello;
        }

        public static string ObtnerFolder(TipoFacturacion tipofac, string contador,DateTime fechaTimbreRegistros)
        {
            string ruta = string.Empty;
            try 
            {
                string haberOPrestamo = tipofac == TipoFacturacion.Haberes ? "Haberes" : "Prestamos";
                ruta = Path.Combine(ConfigurationManager.AppSettings["xmlTimbres"].ToString());
                DateTime fecha = fechaTimbreRegistros;
                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);

                string[] directorio = System.IO.Directory.GetDirectories(ruta);

                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fecha.Month).ToUpper();
                ruta = Path.Combine(ruta, fecha.Year.ToString());
                if (directorio.ToList().Exists(p => p.Equals(ruta)))
                {
                    if(tipofac == TipoFacturacion.Prestamos)
                        ruta = Path.Combine(ruta, nombreMes, haberOPrestamo, fecha.Day.ToString(), contador);
                    else
                        ruta = Path.Combine(ruta, nombreMes, haberOPrestamo, contador);
                    if (!Directory.Exists(ruta))
                        Directory.CreateDirectory(ruta);
                }
                else
                {
                    if (tipofac == TipoFacturacion.Prestamos)
                        ruta = Path.Combine(ruta, nombreMes, haberOPrestamo, fecha.Day.ToString(), contador);
                    else
                        ruta = Path.Combine(ruta, nombreMes, haberOPrestamo, contador);

                    Directory.CreateDirectory(ruta);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AL OBTENER LA RUTA DEL PDF", ex);
            }
            return ruta;
        }

        public static string DescomprimirZipCadena(byte[] ArrayBytes)
        {
            using (MemoryStream inputStream = new MemoryStream(ArrayBytes))
            {
                using (DeflateStream gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(gzip, System.Text.Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public static void ProgresoBarra(int porcentaje, Color fondo, Color Barra, PictureBox barra)
        {
            try
            {
                Brush colorTexto;
                int pbComplete = 0;
                pbComplete = porcentaje;
                Bitmap bmp = new Bitmap(barra.Width, barra.Height);
                Double pbUnit = barra.Width / 100.0;
                Graphics g = Graphics.FromImage(bmp);
                //clear graphics
                g.Clear(fondo);
                //126, 204, 51 verde caja
                //Color.FromArgb(126,204,51)
                //if (porcentaje > 50)
                //    colorTexto = Brushes.White;
                //else
                colorTexto = Brushes.Black;

                g.FillRectangle(new SolidBrush(Barra), new Rectangle(0, 0, (int)(pbComplete * pbUnit), barra.Width));
                //draw % complete
                g.DrawString(pbComplete + "%", new Font("Arial", barra.Height / 2, FontStyle.Bold), colorTexto, new PointF(barra.Width / 2 - barra.Height, barra.Height / 10));
                //load bitmap in picturebox picboxPB
                barra.Image = bmp;
                barra.Update();
                barra.Invalidate();
                barra.Refresh();

            }
            catch (Exception ex)
            {
                throw new Exception("AL aumentar la barra", ex);
            }
        }
        */

    }
}
