using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models.Facturacion.Produccion
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "enviaAcuseCancelacionBinding", Namespace = "http://edifact.com.mx/xsd")]
    public partial class enviaAcuseCancelacion : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback CallenviaAcuseCancelacionOperationCompleted;

        private System.Threading.SendOrPostCallback aceptarRechazarCancelacionOperationCompleted;

        private System.Threading.SendOrPostCallback obtenerPeticionesPendientesOperationCompleted;

        private System.Threading.SendOrPostCallback obtenerRelacionadosOperationCompleted;

        /// <remarks/>
        public enviaAcuseCancelacion()
        {
            this.Url = "https://www.edifactmx-pac.com:443/serviceCFDI/cancelaCFDI.php";
        }

        /// <remarks/>
        public event CallenviaAcuseCancelacionCompletedEventHandler CallenviaAcuseCancelacionCompleted;

        /// <remarks/>
        public event aceptarRechazarCancelacionCompletedEventHandler aceptarRechazarCancelacionCompleted;

        /// <remarks/>
        public event obtenerPeticionesPendientesCompletedEventHandler obtenerPeticionesPendientesCompleted;

        /// <remarks/>
        public event obtenerRelacionadosCompletedEventHandler obtenerRelacionadosCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("https://www.edifactmx-pac.com/serviceCFDI/cancelaCFDI.php/enviaAcuseCancelacion", RequestNamespace = "http://edifact.com.mx/xsd", ResponseNamespace = "http://edifact.com.mx/xsd")]
        [return: System.Xml.Serialization.SoapElementAttribute("ns1:return")]
        public string CallenviaAcuseCancelacion(string xmlFile)
        {
            object[] results = this.Invoke("CallenviaAcuseCancelacion", new object[] {
                    xmlFile});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginCallenviaAcuseCancelacion(string xmlFile, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CallenviaAcuseCancelacion", new object[] {
                    xmlFile}, callback, asyncState);
        }

        /// <remarks/>
        public string EndCallenviaAcuseCancelacion(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void CallenviaAcuseCancelacionAsync(string xmlFile)
        {
            this.CallenviaAcuseCancelacionAsync(xmlFile, null);
        }

        /// <remarks/>
        public void CallenviaAcuseCancelacionAsync(string xmlFile, object userState)
        {
            if ((this.CallenviaAcuseCancelacionOperationCompleted == null))
            {
                this.CallenviaAcuseCancelacionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCallenviaAcuseCancelacionOperationCompleted);
            }
            this.InvokeAsync("CallenviaAcuseCancelacion", new object[] {
                    xmlFile}, this.CallenviaAcuseCancelacionOperationCompleted, userState);
        }

        private void OnCallenviaAcuseCancelacionOperationCompleted(object arg)
        {
            if ((this.CallenviaAcuseCancelacionCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CallenviaAcuseCancelacionCompleted(this, new CallenviaAcuseCancelacionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("https://www.edifactmx-pac.com/serviceCFDI/cancelaCFDI.php/aceptarRechazarCancelac" +
            "ion", RequestNamespace = "http://edifact.com.mx/xsd", ResponseNamespace = "http://edifact.com.mx/xsd")]
        [return: System.Xml.Serialization.SoapElementAttribute("ns1:return")]
        public string aceptarRechazarCancelacion(string xmlFile)
        {
            object[] results = this.Invoke("aceptarRechazarCancelacion", new object[] {
                    xmlFile});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginaceptarRechazarCancelacion(string xmlFile, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("aceptarRechazarCancelacion", new object[] {
                    xmlFile}, callback, asyncState);
        }

        /// <remarks/>
        public string EndaceptarRechazarCancelacion(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void aceptarRechazarCancelacionAsync(string xmlFile)
        {
            this.aceptarRechazarCancelacionAsync(xmlFile, null);
        }

        /// <remarks/>
        public void aceptarRechazarCancelacionAsync(string xmlFile, object userState)
        {
            if ((this.aceptarRechazarCancelacionOperationCompleted == null))
            {
                this.aceptarRechazarCancelacionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnaceptarRechazarCancelacionOperationCompleted);
            }
            this.InvokeAsync("aceptarRechazarCancelacion", new object[] {
                    xmlFile}, this.aceptarRechazarCancelacionOperationCompleted, userState);
        }

        private void OnaceptarRechazarCancelacionOperationCompleted(object arg)
        {
            if ((this.aceptarRechazarCancelacionCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.aceptarRechazarCancelacionCompleted(this, new aceptarRechazarCancelacionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("https://www.edifactmx-pac.com/serviceCFDI/cancelaCFDI.php/obtenerPeticionesPendie" +
            "ntes", RequestNamespace = "http://edifact.com.mx/xsd", ResponseNamespace = "http://edifact.com.mx/xsd")]
        [return: System.Xml.Serialization.SoapElementAttribute("ns1:return")]
        public string obtenerPeticionesPendientes(string rfcReceptor)
        {
            object[] results = this.Invoke("obtenerPeticionesPendientes", new object[] {
                    rfcReceptor});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginobtenerPeticionesPendientes(string rfcReceptor, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("obtenerPeticionesPendientes", new object[] {
                    rfcReceptor}, callback, asyncState);
        }

        /// <remarks/>
        public string EndobtenerPeticionesPendientes(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void obtenerPeticionesPendientesAsync(string rfcReceptor)
        {
            this.obtenerPeticionesPendientesAsync(rfcReceptor, null);
        }

        /// <remarks/>
        public void obtenerPeticionesPendientesAsync(string rfcReceptor, object userState)
        {
            if ((this.obtenerPeticionesPendientesOperationCompleted == null))
            {
                this.obtenerPeticionesPendientesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnobtenerPeticionesPendientesOperationCompleted);
            }
            this.InvokeAsync("obtenerPeticionesPendientes", new object[] {
                    rfcReceptor}, this.obtenerPeticionesPendientesOperationCompleted, userState);
        }

        private void OnobtenerPeticionesPendientesOperationCompleted(object arg)
        {
            if ((this.obtenerPeticionesPendientesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.obtenerPeticionesPendientesCompleted(this, new obtenerPeticionesPendientesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("https://www.edifactmx-pac.com/serviceCFDI/cancelaCFDI.php/obtenerRelacionados", RequestNamespace = "http://edifact.com.mx/xsd", ResponseNamespace = "http://edifact.com.mx/xsd")]
        [return: System.Xml.Serialization.SoapElementAttribute("ns1:return")]
        public string obtenerRelacionados(string xmlFile)
        {
            object[] results = this.Invoke("obtenerRelacionados", new object[] {
                    xmlFile});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginobtenerRelacionados(string xmlFile, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("obtenerRelacionados", new object[] {
                    xmlFile}, callback, asyncState);
        }

        /// <remarks/>
        public string EndobtenerRelacionados(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void obtenerRelacionadosAsync(string xmlFile)
        {
            this.obtenerRelacionadosAsync(xmlFile, null);
        }

        /// <remarks/>
        public void obtenerRelacionadosAsync(string xmlFile, object userState)
        {
            if ((this.obtenerRelacionadosOperationCompleted == null))
            {
                this.obtenerRelacionadosOperationCompleted = new System.Threading.SendOrPostCallback(this.OnobtenerRelacionadosOperationCompleted);
            }
            this.InvokeAsync("obtenerRelacionados", new object[] {
                    xmlFile}, this.obtenerRelacionadosOperationCompleted, userState);
        }

        private void OnobtenerRelacionadosOperationCompleted(object arg)
        {
            if ((this.obtenerRelacionadosCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.obtenerRelacionadosCompleted(this, new obtenerRelacionadosCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    public delegate void CallenviaAcuseCancelacionCompletedEventHandler(object sender, CallenviaAcuseCancelacionCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CallenviaAcuseCancelacionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CallenviaAcuseCancelacionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    public delegate void aceptarRechazarCancelacionCompletedEventHandler(object sender, aceptarRechazarCancelacionCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class aceptarRechazarCancelacionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal aceptarRechazarCancelacionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    public delegate void obtenerPeticionesPendientesCompletedEventHandler(object sender, obtenerPeticionesPendientesCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class obtenerPeticionesPendientesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal obtenerPeticionesPendientesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    public delegate void obtenerRelacionadosCompletedEventHandler(object sender, obtenerRelacionadosCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class obtenerRelacionadosCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal obtenerRelacionadosCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }


    // NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "Acuse")]
    public partial class AcuseCancelacionProductivoResponseWs
    {

        private AcuseFolios foliosField;

        private Signature signatureField;

        private System.DateTime fechaField;

        private string rfcEmisorField;

        private ushort codEstatusField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort CodEstatus
        {
            get
            {
                return this.codEstatusField;
            }
            set
            {
                this.codEstatusField = value;
            }
        }
        /// <remarks/>
        public AcuseFolios Folios
        {
            get
            {
                return this.foliosField;
            }
            set
            {
                this.foliosField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime Fecha
        {
            get
            {
                return this.fechaField;
            }
            set
            {
                this.fechaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RfcEmisor
        {
            get
            {
                return this.rfcEmisorField;
            }
            set
            {
                this.rfcEmisorField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AcuseFolios
    {
        private List<AcuseFoliosFolio> folioField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public List<AcuseFoliosFolio> Folio
        {
            get
            {
                return this.folioField;
            }
            set
            {
                this.folioField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cancelacfd.sat.gob.mx")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://cancelacfd.sat.gob.mx", IsNullable = false)]
    public partial class AcuseFoliosFolio {

        //private string estatusUUIDField;

        private string motivoField;
        private string uUIDField;

        /// <remarks/>
        public string UUID
        {
            get
            {
                return this.uUIDField;
            }
            set
            {
                this.uUIDField = value;
            }
        }


        public string Motivo
        {
            get
            {
                return this.motivoField;
            }
            set
            {
                this.motivoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class Signature
    {

        private SignatureSignedInfo signedInfoField;

        private string signatureValueField;

        private SignatureKeyInfo keyInfoField;

        private string idField;

        /// <remarks/>
        public SignatureSignedInfo SignedInfo
        {
            get
            {
                return this.signedInfoField;
            }
            set
            {
                this.signedInfoField = value;
            }
        }

        /// <remarks/>
        public string SignatureValue
        {
            get
            {
                return this.signatureValueField;
            }
            set
            {
                this.signatureValueField = value;
            }
        }

        /// <remarks/>
        public SignatureKeyInfo KeyInfo
        {
            get
            {
                return this.keyInfoField;
            }
            set
            {
                this.keyInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfo
    {

        private SignatureSignedInfoCanonicalizationMethod canonicalizationMethodField;

        private SignatureSignedInfoSignatureMethod signatureMethodField;

        private SignatureSignedInfoReference referenceField;

        /// <remarks/>
        public SignatureSignedInfoCanonicalizationMethod CanonicalizationMethod
        {
            get
            {
                return this.canonicalizationMethodField;
            }
            set
            {
                this.canonicalizationMethodField = value;
            }
        }

        /// <remarks/>
        public SignatureSignedInfoSignatureMethod SignatureMethod
        {
            get
            {
                return this.signatureMethodField;
            }
            set
            {
                this.signatureMethodField = value;
            }
        }

        /// <remarks/>
        public SignatureSignedInfoReference Reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoCanonicalizationMethod
    {

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoSignatureMethod
    {

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReference
    {

        private SignatureSignedInfoReferenceTransforms transformsField;

        private SignatureSignedInfoReferenceDigestMethod digestMethodField;

        private string digestValueField;

        private string uRIField;

        /// <remarks/>
        public SignatureSignedInfoReferenceTransforms Transforms
        {
            get
            {
                return this.transformsField;
            }
            set
            {
                this.transformsField = value;
            }
        }

        /// <remarks/>
        public SignatureSignedInfoReferenceDigestMethod DigestMethod
        {
            get
            {
                return this.digestMethodField;
            }
            set
            {
                this.digestMethodField = value;
            }
        }

        /// <remarks/>
        public string DigestValue
        {
            get
            {
                return this.digestValueField;
            }
            set
            {
                this.digestValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string URI
        {
            get
            {
                return this.uRIField;
            }
            set
            {
                this.uRIField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReferenceTransforms
    {

        private SignatureSignedInfoReferenceTransformsTransform transformField;

        /// <remarks/>
        public SignatureSignedInfoReferenceTransformsTransform Transform
        {
            get
            {
                return this.transformField;
            }
            set
            {
                this.transformField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReferenceTransformsTransform
    {

        private string xPathField;

        private string algorithmField;

        /// <remarks/>
        public string XPath
        {
            get
            {
                return this.xPathField;
            }
            set
            {
                this.xPathField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReferenceDigestMethod
    {

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get
            {
                return this.algorithmField;
            }
            set
            {
                this.algorithmField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureKeyInfo
    {

        private ulong keyNameField;

        private SignatureKeyInfoKeyValue keyValueField;

        /// <remarks/>
        public ulong KeyName
        {
            get
            {
                return this.keyNameField;
            }
            set
            {
                this.keyNameField = value;
            }
        }

        /// <remarks/>
        public SignatureKeyInfoKeyValue KeyValue
        {
            get
            {
                return this.keyValueField;
            }
            set
            {
                this.keyValueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureKeyInfoKeyValue
    {

        private SignatureKeyInfoKeyValueRSAKeyValue rSAKeyValueField;

        /// <remarks/>
        public SignatureKeyInfoKeyValueRSAKeyValue RSAKeyValue
        {
            get
            {
                return this.rSAKeyValueField;
            }
            set
            {
                this.rSAKeyValueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureKeyInfoKeyValueRSAKeyValue
    {

        private string modulusField;

        private string exponentField;

        /// <remarks/>
        public string Modulus
        {
            get
            {
                return this.modulusField;
            }
            set
            {
                this.modulusField = value;
            }
        }

        /// <remarks/>
        public string Exponent
        {
            get
            {
                return this.exponentField;
            }
            set
            {
                this.exponentField = value;
            }
        }
    }

}

