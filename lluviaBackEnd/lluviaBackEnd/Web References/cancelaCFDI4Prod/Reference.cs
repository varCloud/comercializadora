﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Microsoft.VSDesigner generó automáticamente este código fuente, versión=4.0.30319.42000.
// 
#pragma warning disable 1591

namespace lluviaBackEnd.cancelaCFDI4Prod {
    using System.Diagnostics;
    using System;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System.Web.Services;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="enviaAcuseCancelacionBinding", Namespace="http://edifact.com.mx/xsd")]
    public partial class enviaAcuseCancelacion : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CallenviaAcuseCancelacionOperationCompleted;
        
        private System.Threading.SendOrPostCallback aceptarRechazarCancelacionOperationCompleted;
        
        private System.Threading.SendOrPostCallback obtenerPeticionesPendientesOperationCompleted;
        
        private System.Threading.SendOrPostCallback obtenerRelacionadosOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public enviaAcuseCancelacion() {
            this.Url = global::lluviaBackEnd.Properties.Settings.Default.lluviaBackEnd_cancelaCFDI4Prod_enviaAcuseCancelacion;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
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
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("https://www.edifactmx-pac.com/serviceCFDI4/cancelaCFDI.php/enviaAcuseCancelacion", RequestNamespace="http://edifact.com.mx/xsd", ResponseNamespace="http://edifact.com.mx/xsd")]
        [return: System.Xml.Serialization.SoapElementAttribute("ns1:return")]
        public string CallenviaAcuseCancelacion(string xmlFile) {
            object[] results = this.Invoke("CallenviaAcuseCancelacion", new object[] {
                        xmlFile});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CallenviaAcuseCancelacionAsync(string xmlFile) {
            this.CallenviaAcuseCancelacionAsync(xmlFile, null);
        }
        
        /// <remarks/>
        public void CallenviaAcuseCancelacionAsync(string xmlFile, object userState) {
            if ((this.CallenviaAcuseCancelacionOperationCompleted == null)) {
                this.CallenviaAcuseCancelacionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCallenviaAcuseCancelacionOperationCompleted);
            }
            this.InvokeAsync("CallenviaAcuseCancelacion", new object[] {
                        xmlFile}, this.CallenviaAcuseCancelacionOperationCompleted, userState);
        }
        
        private void OnCallenviaAcuseCancelacionOperationCompleted(object arg) {
            if ((this.CallenviaAcuseCancelacionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CallenviaAcuseCancelacionCompleted(this, new CallenviaAcuseCancelacionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("https://www.edifactmx-pac.com/serviceCFDI4/cancelaCFDI.php/aceptarRechazarCancela" +
            "cion", RequestNamespace="http://edifact.com.mx/xsd", ResponseNamespace="http://edifact.com.mx/xsd")]
        [return: System.Xml.Serialization.SoapElementAttribute("ns1:return")]
        public string aceptarRechazarCancelacion(string xmlFile) {
            object[] results = this.Invoke("aceptarRechazarCancelacion", new object[] {
                        xmlFile});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void aceptarRechazarCancelacionAsync(string xmlFile) {
            this.aceptarRechazarCancelacionAsync(xmlFile, null);
        }
        
        /// <remarks/>
        public void aceptarRechazarCancelacionAsync(string xmlFile, object userState) {
            if ((this.aceptarRechazarCancelacionOperationCompleted == null)) {
                this.aceptarRechazarCancelacionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnaceptarRechazarCancelacionOperationCompleted);
            }
            this.InvokeAsync("aceptarRechazarCancelacion", new object[] {
                        xmlFile}, this.aceptarRechazarCancelacionOperationCompleted, userState);
        }
        
        private void OnaceptarRechazarCancelacionOperationCompleted(object arg) {
            if ((this.aceptarRechazarCancelacionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.aceptarRechazarCancelacionCompleted(this, new aceptarRechazarCancelacionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("https://www.edifactmx-pac.com/serviceCFDI4/cancelaCFDI.php/obtenerPeticionesPendi" +
            "entes", RequestNamespace="http://edifact.com.mx/xsd", ResponseNamespace="http://edifact.com.mx/xsd")]
        [return: System.Xml.Serialization.SoapElementAttribute("ns1:return")]
        public string obtenerPeticionesPendientes(string rfcReceptor) {
            object[] results = this.Invoke("obtenerPeticionesPendientes", new object[] {
                        rfcReceptor});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void obtenerPeticionesPendientesAsync(string rfcReceptor) {
            this.obtenerPeticionesPendientesAsync(rfcReceptor, null);
        }
        
        /// <remarks/>
        public void obtenerPeticionesPendientesAsync(string rfcReceptor, object userState) {
            if ((this.obtenerPeticionesPendientesOperationCompleted == null)) {
                this.obtenerPeticionesPendientesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnobtenerPeticionesPendientesOperationCompleted);
            }
            this.InvokeAsync("obtenerPeticionesPendientes", new object[] {
                        rfcReceptor}, this.obtenerPeticionesPendientesOperationCompleted, userState);
        }
        
        private void OnobtenerPeticionesPendientesOperationCompleted(object arg) {
            if ((this.obtenerPeticionesPendientesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.obtenerPeticionesPendientesCompleted(this, new obtenerPeticionesPendientesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("https://www.edifactmx-pac.com/serviceCFDI4/cancelaCFDI.php/obtenerRelacionados", RequestNamespace="http://edifact.com.mx/xsd", ResponseNamespace="http://edifact.com.mx/xsd")]
        [return: System.Xml.Serialization.SoapElementAttribute("ns1:return")]
        public string obtenerRelacionados(string xmlFile) {
            object[] results = this.Invoke("obtenerRelacionados", new object[] {
                        xmlFile});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void obtenerRelacionadosAsync(string xmlFile) {
            this.obtenerRelacionadosAsync(xmlFile, null);
        }
        
        /// <remarks/>
        public void obtenerRelacionadosAsync(string xmlFile, object userState) {
            if ((this.obtenerRelacionadosOperationCompleted == null)) {
                this.obtenerRelacionadosOperationCompleted = new System.Threading.SendOrPostCallback(this.OnobtenerRelacionadosOperationCompleted);
            }
            this.InvokeAsync("obtenerRelacionados", new object[] {
                        xmlFile}, this.obtenerRelacionadosOperationCompleted, userState);
        }
        
        private void OnobtenerRelacionadosOperationCompleted(object arg) {
            if ((this.obtenerRelacionadosCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.obtenerRelacionadosCompleted(this, new obtenerRelacionadosCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    public delegate void CallenviaAcuseCancelacionCompletedEventHandler(object sender, CallenviaAcuseCancelacionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CallenviaAcuseCancelacionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CallenviaAcuseCancelacionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    public delegate void aceptarRechazarCancelacionCompletedEventHandler(object sender, aceptarRechazarCancelacionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class aceptarRechazarCancelacionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal aceptarRechazarCancelacionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    public delegate void obtenerPeticionesPendientesCompletedEventHandler(object sender, obtenerPeticionesPendientesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class obtenerPeticionesPendientesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal obtenerPeticionesPendientesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    public delegate void obtenerRelacionadosCompletedEventHandler(object sender, obtenerRelacionadosCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class obtenerRelacionadosCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal obtenerRelacionadosCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591