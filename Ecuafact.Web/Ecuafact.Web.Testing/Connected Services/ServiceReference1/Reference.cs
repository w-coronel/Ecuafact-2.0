﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ecuafact.Web.Testing.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://ec.gob.sri.ws.autorizacion", ConfigurationName="ServiceReference1.AutorizacionComprobantes")]
    public interface AutorizacionComprobantes {
        
        // CODEGEN: El parámetro 'RespuestaAutorizacionComprobante' requiere información adicional de esquema que no se puede capturar con el modo de parámetros. El atributo específico es 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(mensaje[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(autorizacion[]))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="RespuestaAutorizacionComprobante")]
        Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteResponse autorizacionComprobante(Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobante request);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteResponse> autorizacionComprobanteAsync(Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobante request);
        
        // CODEGEN: El parámetro 'RespuestaAutorizacionLote' requiere información adicional de esquema que no se puede capturar con el modo de parámetros. El atributo específico es 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(mensaje[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(autorizacion[]))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="RespuestaAutorizacionLote")]
        Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLoteResponse autorizacionComprobanteLote(Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLote request);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        System.Threading.Tasks.Task<Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLoteResponse> autorizacionComprobanteLoteAsync(Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLote request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ec.gob.sri.ws.autorizacion")]
    public partial class respuestaComprobante : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string claveAccesoConsultadaField;
        
        private string numeroComprobantesField;
        
        private autorizacion[] autorizacionesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string claveAccesoConsultada {
            get {
                return this.claveAccesoConsultadaField;
            }
            set {
                this.claveAccesoConsultadaField = value;
                this.RaisePropertyChanged("claveAccesoConsultada");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string numeroComprobantes {
            get {
                return this.numeroComprobantesField;
            }
            set {
                this.numeroComprobantesField = value;
                this.RaisePropertyChanged("numeroComprobantes");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public autorizacion[] autorizaciones {
            get {
                return this.autorizacionesField;
            }
            set {
                this.autorizacionesField = value;
                this.RaisePropertyChanged("autorizaciones");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ec.gob.sri.ws.autorizacion")]
    public partial class autorizacion : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string estadoField;
        
        private string numeroAutorizacionField;
        
        private System.DateTime fechaAutorizacionField;
        
        private bool fechaAutorizacionFieldSpecified;
        
        private string ambienteField;
        
        private string comprobanteField;
        
        private mensaje[] mensajesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string estado {
            get {
                return this.estadoField;
            }
            set {
                this.estadoField = value;
                this.RaisePropertyChanged("estado");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string numeroAutorizacion {
            get {
                return this.numeroAutorizacionField;
            }
            set {
                this.numeroAutorizacionField = value;
                this.RaisePropertyChanged("numeroAutorizacion");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public System.DateTime fechaAutorizacion {
            get {
                return this.fechaAutorizacionField;
            }
            set {
                this.fechaAutorizacionField = value;
                this.RaisePropertyChanged("fechaAutorizacion");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fechaAutorizacionSpecified {
            get {
                return this.fechaAutorizacionFieldSpecified;
            }
            set {
                this.fechaAutorizacionFieldSpecified = value;
                this.RaisePropertyChanged("fechaAutorizacionSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string ambiente {
            get {
                return this.ambienteField;
            }
            set {
                this.ambienteField = value;
                this.RaisePropertyChanged("ambiente");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string comprobante {
            get {
                return this.comprobanteField;
            }
            set {
                this.comprobanteField = value;
                this.RaisePropertyChanged("comprobante");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public mensaje[] mensajes {
            get {
                return this.mensajesField;
            }
            set {
                this.mensajesField = value;
                this.RaisePropertyChanged("mensajes");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ec.gob.sri.ws.autorizacion")]
    public partial class mensaje : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string identificadorField;
        
        private string mensaje1Field;
        
        private string informacionAdicionalField;
        
        private string tipoField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string identificador {
            get {
                return this.identificadorField;
            }
            set {
                this.identificadorField = value;
                this.RaisePropertyChanged("identificador");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("mensaje", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string mensaje1 {
            get {
                return this.mensaje1Field;
            }
            set {
                this.mensaje1Field = value;
                this.RaisePropertyChanged("mensaje1");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string informacionAdicional {
            get {
                return this.informacionAdicionalField;
            }
            set {
                this.informacionAdicionalField = value;
                this.RaisePropertyChanged("informacionAdicional");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string tipo {
            get {
                return this.tipoField;
            }
            set {
                this.tipoField = value;
                this.RaisePropertyChanged("tipo");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ec.gob.sri.ws.autorizacion")]
    public partial class respuestaLote : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string claveAccesoLoteConsultadaField;
        
        private string numeroComprobantesLoteField;
        
        private autorizacion[] autorizacionesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string claveAccesoLoteConsultada {
            get {
                return this.claveAccesoLoteConsultadaField;
            }
            set {
                this.claveAccesoLoteConsultadaField = value;
                this.RaisePropertyChanged("claveAccesoLoteConsultada");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string numeroComprobantesLote {
            get {
                return this.numeroComprobantesLoteField;
            }
            set {
                this.numeroComprobantesLoteField = value;
                this.RaisePropertyChanged("numeroComprobantesLote");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public autorizacion[] autorizaciones {
            get {
                return this.autorizacionesField;
            }
            set {
                this.autorizacionesField = value;
                this.RaisePropertyChanged("autorizaciones");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="autorizacionComprobante", WrapperNamespace="http://ec.gob.sri.ws.autorizacion", IsWrapped=true)]
    public partial class autorizacionComprobante {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ec.gob.sri.ws.autorizacion", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string claveAccesoComprobante;
        
        public autorizacionComprobante() {
        }
        
        public autorizacionComprobante(string claveAccesoComprobante) {
            this.claveAccesoComprobante = claveAccesoComprobante;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="autorizacionComprobanteResponse", WrapperNamespace="http://ec.gob.sri.ws.autorizacion", IsWrapped=true)]
    public partial class autorizacionComprobanteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ec.gob.sri.ws.autorizacion", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Ecuafact.Web.Testing.ServiceReference1.respuestaComprobante RespuestaAutorizacionComprobante;
        
        public autorizacionComprobanteResponse() {
        }
        
        public autorizacionComprobanteResponse(Ecuafact.Web.Testing.ServiceReference1.respuestaComprobante RespuestaAutorizacionComprobante) {
            this.RespuestaAutorizacionComprobante = RespuestaAutorizacionComprobante;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="autorizacionComprobanteLote", WrapperNamespace="http://ec.gob.sri.ws.autorizacion", IsWrapped=true)]
    public partial class autorizacionComprobanteLote {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ec.gob.sri.ws.autorizacion", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string claveAccesoLote;
        
        public autorizacionComprobanteLote() {
        }
        
        public autorizacionComprobanteLote(string claveAccesoLote) {
            this.claveAccesoLote = claveAccesoLote;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="autorizacionComprobanteLoteResponse", WrapperNamespace="http://ec.gob.sri.ws.autorizacion", IsWrapped=true)]
    public partial class autorizacionComprobanteLoteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ec.gob.sri.ws.autorizacion", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Ecuafact.Web.Testing.ServiceReference1.respuestaLote RespuestaAutorizacionLote;
        
        public autorizacionComprobanteLoteResponse() {
        }
        
        public autorizacionComprobanteLoteResponse(Ecuafact.Web.Testing.ServiceReference1.respuestaLote RespuestaAutorizacionLote) {
            this.RespuestaAutorizacionLote = RespuestaAutorizacionLote;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface AutorizacionComprobantesChannel : Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AutorizacionComprobantesClient : System.ServiceModel.ClientBase<Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes>, Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes {
        
        public AutorizacionComprobantesClient() {
        }
        
        public AutorizacionComprobantesClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AutorizacionComprobantesClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AutorizacionComprobantesClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AutorizacionComprobantesClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteResponse Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes.autorizacionComprobante(Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobante request) {
            return base.Channel.autorizacionComprobante(request);
        }
        
        public Ecuafact.Web.Testing.ServiceReference1.respuestaComprobante autorizacionComprobante(string claveAccesoComprobante) {
            Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobante inValue = new Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobante();
            inValue.claveAccesoComprobante = claveAccesoComprobante;
            Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteResponse retVal = ((Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes)(this)).autorizacionComprobante(inValue);
            return retVal.RespuestaAutorizacionComprobante;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteResponse> Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes.autorizacionComprobanteAsync(Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobante request) {
            return base.Channel.autorizacionComprobanteAsync(request);
        }
        
        public System.Threading.Tasks.Task<Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteResponse> autorizacionComprobanteAsync(string claveAccesoComprobante) {
            Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobante inValue = new Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobante();
            inValue.claveAccesoComprobante = claveAccesoComprobante;
            return ((Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes)(this)).autorizacionComprobanteAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLoteResponse Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes.autorizacionComprobanteLote(Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLote request) {
            return base.Channel.autorizacionComprobanteLote(request);
        }
        
        public Ecuafact.Web.Testing.ServiceReference1.respuestaLote autorizacionComprobanteLote(string claveAccesoLote) {
            Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLote inValue = new Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLote();
            inValue.claveAccesoLote = claveAccesoLote;
            Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLoteResponse retVal = ((Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes)(this)).autorizacionComprobanteLote(inValue);
            return retVal.RespuestaAutorizacionLote;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLoteResponse> Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes.autorizacionComprobanteLoteAsync(Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLote request) {
            return base.Channel.autorizacionComprobanteLoteAsync(request);
        }
        
        public System.Threading.Tasks.Task<Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLoteResponse> autorizacionComprobanteLoteAsync(string claveAccesoLote) {
            Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLote inValue = new Ecuafact.Web.Testing.ServiceReference1.autorizacionComprobanteLote();
            inValue.claveAccesoLote = claveAccesoLote;
            return ((Ecuafact.Web.Testing.ServiceReference1.AutorizacionComprobantes)(this)).autorizacionComprobanteLoteAsync(inValue);
        }
    }
}
