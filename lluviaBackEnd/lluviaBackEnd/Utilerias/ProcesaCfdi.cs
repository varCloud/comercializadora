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
using AutoMapper;
using lluviaBackEnd.Models.Facturacion.Produccion;
using log4net;

namespace lluviaBackEnd.Utilerias
{

    public static class ProcesaCfdi
    {

        private static readonly ILog log4netRequest = LogManager.GetLogger("LogLluvia");
    

        private static string fileName;
        public static string pathArchivoSAT = string.Empty;

        public static string SerializaXML33(Comprobante comprobante , bool esFacturacion4 = false)
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
                if (esFacturacion4)
                    ns.Add("cfdi", "http://www.sat.gob.mx/cfd/4");
                else
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
                X509Certificate2 publicCert = new X509Certificate2(lluviaBackEnd.Resource.cerLLuvia2022_cer);
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
                string strLlavePwd = ConfigurationManager.AppSettings["claveGeneraSellolluvia"].ToString();
                System.Security.SecureString passwordSeguro = new System.Security.SecureString();
                passwordSeguro.Clear();
                foreach (char c in strLlavePwd.ToCharArray())
                    passwordSeguro.AppendChar(c);

                byte[] llavePrivadaBytes = lluviaBackEnd.Resource.keyLLuvia2022_key;// ARCHIVO .KEY
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
            try
            {
                string strLlavePwd = ConfigurationManager.AppSettings["claveGeneraSellolluvia"].ToString();
                X509Certificate2 cert = new X509Certificate2(Resource.archivo2022_pfx, strLlavePwd);
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
            catch (Exception ex)
            {
                log4netRequest.Debug(ex.Message + "" + ex.StackTrace);
                throw ex;
            }
        }

        public static object TimbrarEdifact40(string xmlSerializadoSAT)
        {
            try
            {
                servicioTimbradoProductivo.respuestaTimbrado respuesta = null;
                wsPruevas40.respuestaTimbrado respuestaPruebas = null;

                System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                if (ConfigurationManager.AppSettings["FacturarPro"].ToString().Equals("1"))
                {
                    servicioTimbradoProductivo.timbrarCFDIPortTypeClient timbrar = new servicioTimbradoProductivo.timbrarCFDIPortTypeClient();
                    respuesta = timbrar.timbrarCFDI("", "", xmlSerializadoSAT);
                    return respuesta;
                }
                else
                {
                    wsPruevas40.timbrarCFDIPortTypeClient timbrar = new wsPruevas40.timbrarCFDIPortTypeClient();
                    respuestaPruebas = timbrar.timbrarCFDI("", "", xmlSerializadoSAT);
                    ///////////////////MAPER///////////////////
                    //var configuration = new MapperConfiguration(cfg =>
                    //{
                    //    cfg.CreateMap<PrintDocumentolluvia.servicioTimbrarPruebas.respuestaTimbrado, PrintDocumentolluvia.servicioTimbradoProductivo.respuestaTimbrado>();

                    //});

                    //configuration.AssertConfigurationIsValid();
                    //var mapper = configuration.CreateMapper();
                    //respuesta = mapper.Map<PrintDocumentolluvia.wsPruevas40.respuestaTimbrado>(respuestaPruebas);
                    //System.IO.File.WriteAllText(pathfileOutputResponse.Replace("@@", "Response") + ".xml", SerializerManager<wsPruevas40.respuestaTimbrado>.SerealizarObjtecToString(respuestaPruebas));
                    return respuestaPruebas;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static object TimbrarEdifact(string xmlSerializadoSAT)
        {
            try
            {
                servicioTimbradoProductivo.respuestaTimbrado respuesta = null;
                servicioTimbrarPruebas.respuestaTimbrado respuestaPruebas = null;

                System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };
                if (ConfigurationManager.AppSettings["FacturarPro"].ToString().Equals("1"))
                {
                    servicioTimbradoProductivo.timbrarCFDIPortTypeClient timbrar = new servicioTimbradoProductivo.timbrarCFDIPortTypeClient();
                    respuesta = timbrar.timbrarCFDI("", "", xmlSerializadoSAT);
                    return respuesta;
                }
                else
                {
                    servicioTimbrarPruebas.timbrarCFDIPortTypeClient timbrar = new servicioTimbrarPruebas.timbrarCFDIPortTypeClient();
                    respuestaPruebas = timbrar.timbrarCFDI("", "", xmlSerializadoSAT);
                    ///////////////////MAPER///////////////////
                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<servicioTimbrarPruebas.respuestaTimbrado, servicioTimbradoProductivo.respuestaTimbrado>();

                    });

                    configuration.AssertConfigurationIsValid();
                    var mapper = configuration.CreateMapper();
                    respuesta = mapper.Map<servicioTimbradoProductivo.respuestaTimbrado>(respuestaPruebas);
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static string CancelarFacturaEdifact(string documentoOriginal) {
            string result = string.Empty;
            try
            {
                log4netRequest.Debug("CancelarFacturaEdifact");
                XmlDocument originalXmlDocument = new XmlDocument() { PreserveWhitespace = false };
                originalXmlDocument.LoadXml(documentoOriginal);

                // --------------------------------------------------------------------------
                // III. GENERAR ELEMENTO <Signature> USANDO EL CSD DEL EMISOR
                // --------------------------------------------------------------------------

                XmlElement signatureElement = ProcesaCfdi.GenerateXmlSignature(originalXmlDocument);

                // --------------------------------------------------------------------------
                // IV. INCRUSTAR EL ELEMENTO <Signature> DENTRO DEL DOCUMENTO XML ORIGINAL
                // --------------------------------------------------------------------------

                originalXmlDocument.DocumentElement.AppendChild(originalXmlDocument.ImportNode(signatureElement, true));

                // --------------------------------------------------------------------------
                // V. EL XML RESULTANTE ES LA SOLICITUD FIRMADA DE CANCELACIÓN
                // --------------------------------------------------------------------------
                //Debug.WriteLine(originalXmlDocument.OuterXml);

                if (ConfigurationManager.AppSettings["FacturarPro"].ToString().Equals("1"))
                {
                    lluviaBackEnd.Models.Facturacion.Produccion.enviaAcuseCancelacion envia = new Models.Facturacion.Produccion.enviaAcuseCancelacion();
                    result = envia.CallenviaAcuseCancelacion(originalXmlDocument.OuterXml);

                }
                else
                {
                    lluviaBackEnd.Models.Facturacion.enviaAcuseCancelacion enviaCancelacion = new lluviaBackEnd.Models.Facturacion.enviaAcuseCancelacion();
                    result = enviaCancelacion.CallenviaAcuseCancelacion(originalXmlDocument.OuterXml);
                }
            }
            catch (Exception ex)
            {
                log4netRequest.Debug(ex.Message + "" + ex.StackTrace);
                throw ex;
            }
                return result;
        }


        public static AcuseCancelacionProductivoResponseWs ObtnerAcuseCancelacionFactura(string xmlCancelado) {

            AcuseCancelacionProductivoResponseWs cancelacion = null;
            AcuseCancelacionPruebasResponseWS cancelacionPruebas = null;
            try
            {
                log4netRequest.Debug("ObtnerAcuseCancelacionFactura");
                if (ConfigurationManager.AppSettings["FacturarPro"].ToString().Equals("1"))
                {
                     cancelacion = ManagerSerealization<AcuseCancelacionProductivoResponseWs>.DeserializeXMLStringToObject(xmlCancelado);
                     return cancelacion;
                }
                else
                {
                    cancelacionPruebas = ManagerSerealization<AcuseCancelacionPruebasResponseWS>.DeserializeXMLStringToObject(xmlCancelado);
                }
                ///////////////////MAPER///////////////////
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<AcuseCancelacionPruebasResponseWS, AcuseCancelacionProductivoResponseWs>();
                    cfg.CreateMap<Folios, AcuseFolios>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.Signature, lluviaBackEnd.Models.Facturacion.Produccion.Signature>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureSignedInfo, lluviaBackEnd.Models.Facturacion.Produccion.SignatureSignedInfo>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureKeyInfo, lluviaBackEnd.Models.Facturacion.Produccion.SignatureKeyInfo>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureSignedInfoCanonicalizationMethod, lluviaBackEnd.Models.Facturacion.Produccion.SignatureSignedInfoCanonicalizationMethod>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureSignedInfoSignatureMethod, lluviaBackEnd.Models.Facturacion.Produccion.SignatureSignedInfoSignatureMethod>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureSignedInfoReference, lluviaBackEnd.Models.Facturacion.Produccion.SignatureSignedInfoReference>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureSignedInfoReferenceTransforms, lluviaBackEnd.Models.Facturacion.Produccion.SignatureSignedInfoReferenceTransforms>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureSignedInfoReferenceTransformsTransform, lluviaBackEnd.Models.Facturacion.Produccion.SignatureSignedInfoReferenceTransformsTransform>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureSignedInfoReferenceDigestMethod, lluviaBackEnd.Models.Facturacion.Produccion.SignatureSignedInfoReferenceDigestMethod>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureKeyInfoKeyValue, lluviaBackEnd.Models.Facturacion.Produccion.SignatureKeyInfoKeyValue>();
                    cfg.CreateMap<lluviaBackEnd.Models.Facturacion.SignatureKeyInfoKeyValueRSAKeyValue, lluviaBackEnd.Models.Facturacion.Produccion.SignatureKeyInfoKeyValueRSAKeyValue>();
                });
                

                configuration.AssertConfigurationIsValid();
                var mapper = configuration.CreateMapper();
                cancelacion = mapper.Map<AcuseCancelacionProductivoResponseWs>(cancelacionPruebas);
                return cancelacion;


            }
            catch (Exception ex)
            {
                log4netRequest.Debug(ex.Message + "" + ex.StackTrace);
                throw ex;
            }
        }
    }
}
