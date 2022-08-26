using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace lluviaBackEnd.Models.Facturacion
{


    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    //[XmlRoot(Namespace = "http://www.sat.gob.mx/cfd/4", IsNullable = false, ElementName = "Comprobante")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    [XmlRoot(Namespace = "http://www.sat.gob.mx/cfd/3", IsNullable = false, ElementName = "Comprobante")]
    public partial class Comprobante
    {
        public Comprobante()
        {
            //xsiSchemaLocation = "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd";
            xsiSchemaLocation = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd";
        }
       
        private string xsiSchemaLocation;

        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string schemaLocation
        {
            get
            {
                return this.xsiSchemaLocation;
            }
            set
            {
                this.xsiSchemaLocation = value;
            }
        }

        private ComprobanteCfdiRelacionados cfdiRelacionadosField;

        private ComprobanteEmisor emisorField;

        private ComprobanteReceptor receptorField;

        private ComprobanteConcepto[] conceptosField;

        private ComprobanteImpuestos impuestosField;

        private ComprobanteAddenda addendaField = null;

        private ComprobanteComplemento complementoField;

        private decimal versionField;

        private string serieField;

        private string folioField;

        private string fechaField;

        private string selloField;

        private string noCertificadoField;

        private string certificadoField;

        private decimal subTotalField;

        private string monedaField;

        private decimal totalField;

        private string tipoDeComprobanteField;

        private string formaPagoField;

        private string metodoPagoField;

        private string condicionesDePagoField;

        private decimal descuentoField = 0;

        private ushort lugarExpedicionField;

        //private string exportacion; //CFDI 4.0
              
        /// <remarks/>
        public ComprobanteCfdiRelacionados CfdiRelacionados
        {
            get
            {
                return this.cfdiRelacionadosField;
            }
            set
            {
                this.cfdiRelacionadosField = value;
            }
        }

        /// <remarks/>
        public ComprobanteEmisor Emisor
        {
            get
            {
                return this.emisorField;
            }
            set
            {
                this.emisorField = value;
            }
        }

        /// <remarks/>
        public ComprobanteReceptor Receptor
        {
            get
            {
                return this.receptorField;
            }
            set
            {
                this.receptorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Concepto", IsNullable = false)]
        public ComprobanteConcepto[] Conceptos
        {
            get
            {
                return this.conceptosField;
            }
            set
            {
                this.conceptosField = value;
            }
        }

        /// <remarks/>
        public ComprobanteImpuestos Impuestos
        {
            get
            {
                return this.impuestosField;
            }
            set
            {
                this.impuestosField = value;
            }
        }

        /// <remarks/>
        public ComprobanteComplemento Complemento
        {
            get
            {
                return this.complementoField;
            }
            set
            {
                this.complementoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public string Exportacion
        //{
        //    get
        //    {
        //        return this.exportacion;
        //    }
        //    set
        //    {
        //        this.exportacion = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Serie
        {
            get
            {
                return this.serieField;
            }
            set
            {
                this.serieField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Folio
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Fecha
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
        public string Sello
        {
            get
            {
                return this.selloField;
            }
            set
            {
                this.selloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "integer")]
        public string NoCertificado
        {
            get
            {
                return this.noCertificadoField;
            }
            set
            {
                this.noCertificadoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Certificado
        {
            get
            {
                return this.certificadoField;
            }
            set
            {
                this.certificadoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal SubTotal
        {
            get
            {
                return this.subTotalField;
            }
            set
            {
                this.subTotalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Moneda
        {
            get
            {
                return this.monedaField;
            }
            set
            {
                this.monedaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoDeComprobante
        {
            get
            {
                return this.tipoDeComprobanteField;
            }
            set
            {
                this.tipoDeComprobanteField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string FormaPago
        {
            get
            {
                return this.formaPagoField;
            }
            set
            {
                this.formaPagoField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string MetodoPago
        {
            get
            {
                return this.metodoPagoField;
            }
            set
            {
                this.metodoPagoField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string CondicionesDePago
        {
            get
            {
                return this.condicionesDePagoField;
            }
            set
            {
                this.condicionesDePagoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public decimal Descuento
        {
            get
            {
                return this.descuentoField;
            }
            set
            {
                this.descuentoField = value;
            }
        }

        public bool ShouldSerializeDescuento() { return descuentoField == 0 ? false : true; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort LugarExpedicion
        {
            get
            {
                return this.lugarExpedicionField;
            }
            set
            {
                this.lugarExpedicionField = value;
            }
        }

        public ComprobanteAddenda Addenda
        {
            get
            {
                return this.addendaField;
            }
            set
            {
                this.addendaField = value;
            }
        }

        public bool ShouldAddenda() { return addendaField == null ? false : true; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteCfdiRelacionados
    {

        private ComprobanteCfdiRelacionadosCfdiRelacionado cfdiRelacionadoField;

        private byte tipoRelacionField;

        /// <remarks/>
        public ComprobanteCfdiRelacionadosCfdiRelacionado CfdiRelacionado
        {
            get
            {
                return this.cfdiRelacionadoField;
            }
            set
            {
                this.cfdiRelacionadoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte TipoRelacion
        {
            get
            {
                return this.tipoRelacionField;
            }
            set
            {
                this.tipoRelacionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteCfdiRelacionadosCfdiRelacionado
    {

        private string uUIDField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteEmisor
    {

        private string rfcField;

        private string nombreField;

        private ushort regimenFiscalField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Rfc
        {
            get
            {
                return this.rfcField;
            }
            set
            {
                this.rfcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Nombre
        {
            get
            {
                return this.nombreField;
            }
            set
            {
                this.nombreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort RegimenFiscal
        {
            get
            {
                return this.regimenFiscalField;
            }
            set
            {
                this.regimenFiscalField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute( AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteReceptor
    {

        private string rfcField;

        private string nombreField;

        private string usoCFDIField;

        //private string domicilioFiscalReceptor; //CFDI4.0

        //private string regimenFiscalReceptor;//CFDI4.0
        /// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public string RegimenFiscalReceptor
        //{
        //    get
        //    {
        //        return this.regimenFiscalReceptor;
        //    }
        //    set
        //    {
        //        this.regimenFiscalReceptor = value;
        //    }
        //}
        ///// <remarks/>
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public string DomicilioFiscalReceptor
        //{
        //    get
        //    {
        //        return this.domicilioFiscalReceptor;
        //    }
        //    set
        //    {
        //        this.domicilioFiscalReceptor = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Rfc
        {
            get
            {
                return this.rfcField;
            }
            set
            {
                this.rfcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Nombre
        {
            get
            {
                return this.nombreField;
            }
            set
            {
                this.nombreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UsoCFDI
        {
            get
            {
                return this.usoCFDIField;
            }
            set
            {
                this.usoCFDIField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteConcepto
    {

        private ComprobanteConceptoImpuestos impuestosField;

        private ComprobanteConceptoParte parteField;

        private ComprobanteConceptoInformacionAduanera informacionAduaneraField;

        private ComprobanteConceptoCuentaPredial cuentaPredialField;

        private string claveProdServField;

        private string claveUnidadField;

        private string noIdentificacionField;

        private decimal cantidadField;

        private string unidadField;

        private string descripcionField;

        private decimal valorUnitarioField;

        private decimal importeField;

        //private string objetoImp; // FACT 4.0
        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public string ObjetoImp
        //{
        //    get
        //    {
        //        return this.objetoImp;
        //    }
        //    set
        //    {
        //        this.objetoImp = value;
        //    }
        //}

        /// <remarks/>
        public ComprobanteConceptoImpuestos Impuestos
        {
            get
            {
                return this.impuestosField;
            }
            set
            {
                this.impuestosField = value;
            }
        }

        /// <remarks/>
        public ComprobanteConceptoParte Parte
        {
            get
            {
                return this.parteField;
            }
            set
            {
                this.parteField = value;
            }
        }

        /// <remarks/>
        public ComprobanteConceptoInformacionAduanera InformacionAduanera
        {
            get
            {
                return this.informacionAduaneraField;
            }
            set
            {
                this.informacionAduaneraField = value;
            }
        }

        /// <remarks/>
        public ComprobanteConceptoCuentaPredial CuentaPredial
        {
            get
            {
                return this.cuentaPredialField;
            }
            set
            {
                this.cuentaPredialField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ClaveProdServ
        {
            get
            {
                return this.claveProdServField;
            }
            set
            {
                this.claveProdServField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ClaveUnidad
        {
            get
            {
                return this.claveUnidadField;
            }
            set
            {
                this.claveUnidadField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NoIdentificacion
        {
            get
            {
                return this.noIdentificacionField;
            }
            set
            {
                this.noIdentificacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Cantidad
        {
            get
            {
                return this.cantidadField;
            }
            set
            {
                this.cantidadField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Unidad
        {
            get
            {
                return this.unidadField;
            }
            set
            {
                this.unidadField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Descripcion
        {
            get
            {
                return this.descripcionField;
            }
            set
            {
                this.descripcionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ValorUnitario
        {
            get
            {
                return this.valorUnitarioField;
            }
            set
            {
                this.valorUnitarioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteConceptoImpuestos
    {

        private ComprobanteConceptoImpuestosTraslados trasladosField;

        private ComprobanteConceptoImpuestosRetenciones retencionesField;

        /// <remarks/>
        public ComprobanteConceptoImpuestosTraslados Traslados
        {
            get
            {
                return this.trasladosField;
            }
            set
            {
                this.trasladosField = value;
            }
        }

        /// <remarks/>
        public ComprobanteConceptoImpuestosRetenciones Retenciones
        {
            get
            {
                return this.retencionesField;
            }
            set
            {
                this.retencionesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteConceptoImpuestosTraslados
    {

        private ComprobanteConceptoImpuestosTrasladosTraslado trasladoField;

        /// <remarks/>
        public ComprobanteConceptoImpuestosTrasladosTraslado Traslado
        {
            get
            {
                return this.trasladoField;
            }
            set
            {
                this.trasladoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteConceptoImpuestosTrasladosTraslado
    {

        private decimal baseField;

        private string impuestoField;

        private string tipoFactorField;

        private decimal tasaOCuotaField;

        private decimal importeField;

        
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Base
        {
            get
            {
                return this.baseField;
            }
            set
            {
                this.baseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Impuesto
        {
            get
            {
                return this.impuestoField;
            }
            set
            {
                this.impuestoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoFactor
        {
            get
            {
                return this.tipoFactorField;
            }
            set
            {
                this.tipoFactorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TasaOCuota
        {
            get
            {
                return this.tasaOCuotaField;
            }
            set
            {
                this.tasaOCuotaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteConceptoImpuestosRetenciones
    {

        private ComprobanteConceptoImpuestosRetencionesRetencion retencionField;

        /// <remarks/>
        public ComprobanteConceptoImpuestosRetencionesRetencion Retencion
        {
            get
            {
                return this.retencionField;
            }
            set
            {
                this.retencionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteConceptoImpuestosRetencionesRetencion
    {

        private uint baseField;

        private byte impuestoField;

        private string tipoFactorField;

        private decimal tasaOCuotaField;

        private uint importeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint Base
        {
            get
            {
                return this.baseField;
            }
            set
            {
                this.baseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Impuesto
        {
            get
            {
                return this.impuestoField;
            }
            set
            {
                this.impuestoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoFactor
        {
            get
            {
                return this.tipoFactorField;
            }
            set
            {
                this.tipoFactorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TasaOCuota
        {
            get
            {
                return this.tasaOCuotaField;
            }
            set
            {
                this.tasaOCuotaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteConceptoParte
    {

        private ComprobanteConceptoParteInformacionAduanera informacionAduaneraField;

        private uint claveProdServField;

        private ushort noIdentificacionField;

        private decimal cantidadField;

        private string descripcionField;

        private string unidadField;

        private decimal valorUnitarioField;

        private decimal importeField;

        /// <remarks/>
        public ComprobanteConceptoParteInformacionAduanera InformacionAduanera
        {
            get
            {
                return this.informacionAduaneraField;
            }
            set
            {
                this.informacionAduaneraField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint ClaveProdServ
        {
            get
            {
                return this.claveProdServField;
            }
            set
            {
                this.claveProdServField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort NoIdentificacion
        {
            get
            {
                return this.noIdentificacionField;
            }
            set
            {
                this.noIdentificacionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Cantidad
        {
            get
            {
                return this.cantidadField;
            }
            set
            {
                this.cantidadField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Descripcion
        {
            get
            {
                return this.descripcionField;
            }
            set
            {
                this.descripcionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Unidad
        {
            get
            {
                return this.unidadField;
            }
            set
            {
                this.unidadField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal ValorUnitario
        {
            get
            {
                return this.valorUnitarioField;
            }
            set
            {
                this.valorUnitarioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteConceptoParteInformacionAduanera
    {

        private string numeroPedimentoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumeroPedimento
        {
            get
            {
                return this.numeroPedimentoField;
            }
            set
            {
                this.numeroPedimentoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteConceptoInformacionAduanera
    {

        private string numeroPedimentoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumeroPedimento
        {
            get
            {
                return this.numeroPedimentoField;
            }
            set
            {
                this.numeroPedimentoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteConceptoCuentaPredial
    {

        private ushort numeroField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort Numero
        {
            get
            {
                return this.numeroField;
            }
            set
            {
                this.numeroField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteImpuestos
    {

        private ComprobanteImpuestosRetencion[] retencionesField;

        private ComprobanteImpuestosTraslados trasladosField;

        private decimal totalImpuestosRetenidosField = 0;

        private decimal totalImpuestosTrasladadosField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Retencion", IsNullable = false)]
        public ComprobanteImpuestosRetencion[] Retenciones
        {
            get
            {
                return this.retencionesField;
            }
            set
            {
                this.retencionesField = value;
            }
        }

        /// <remarks/>
        public ComprobanteImpuestosTraslados Traslados
        {
            get
            {
                return this.trasladosField;
            }
            set
            {
                this.trasladosField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalImpuestosRetenidos
        {
            get
            {
                return this.totalImpuestosRetenidosField;
            }
            set
            {
                this.totalImpuestosRetenidosField = value;
            }
        }

        public bool ShouldSerializeTotalImpuestosRetenidos() { return totalImpuestosRetenidosField == 0 ? false : true; }
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TotalImpuestosTrasladados
        {
            get
            {
                return this.totalImpuestosTrasladadosField;
            }
            set
            {
                this.totalImpuestosTrasladadosField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteImpuestosRetencion
    {

        private byte impuestoField;

        private uint importeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Impuesto
        {
            get
            {
                return this.impuestoField;
            }
            set
            {
                this.impuestoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteImpuestosTraslados
    {

        private ComprobanteImpuestosTrasladosTraslado trasladoField;

        /// <remarks/>
        public ComprobanteImpuestosTrasladosTraslado Traslado
        {
            get
            {
                return this.trasladoField;
            }
            set
            {
                this.trasladoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteImpuestosTrasladosTraslado
    {

        private string impuestoField;

        private string tipoFactorField;

        private decimal tasaOCuotaField;

        private decimal importeField;

        //private decimal _base;

        //[System.Xml.Serialization.XmlAttributeAttribute()]
        //public decimal Base
        //{
        //    get
        //    {
        //        return this._base;
        //    }
        //    set
        //    {
        //        this._base = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Impuesto
        {
            get
            {
                return this.impuestoField;
            }
            set
            {
                this.impuestoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TipoFactor
        {
            get
            {
                return this.tipoFactorField;
            }
            set
            {
                this.tipoFactorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal TasaOCuota
        {
            get
            {
                return this.tasaOCuotaField;
            }
            set
            {
                this.tasaOCuotaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Importe
        {
            get
            {
                return this.importeField;
            }
            set
            {
                this.importeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
    public partial class ComprobanteComplemento
    {

        private TimbreFiscalDigital timbreFiscalDigitalField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital")]
        public TimbreFiscalDigital TimbreFiscalDigital
        {
            get
            {
                return this.timbreFiscalDigitalField;
            }
            set
            {
                this.timbreFiscalDigitalField = value;
            }
        }


    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital", IsNullable = false)]
    public partial class TimbreFiscalDigital
    {

        private decimal versionField;

        private string uUIDField;

        private System.DateTime fechaTimbradoField;

        private string rfcProvCertifField;

        private string selloCFDField;

        private string noCertificadoSATField;

        private string selloSATField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime FechaTimbrado
        {
            get
            {
                return this.fechaTimbradoField;
            }
            set
            {
                this.fechaTimbradoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RfcProvCertif
        {
            get
            {
                return this.rfcProvCertifField;
            }
            set
            {
                this.rfcProvCertifField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SelloCFD
        {
            get
            {
                return this.selloCFDField;
            }
            set
            {
                this.selloCFDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "integer")]
        public string NoCertificadoSAT
        {
            get
            {
                return this.noCertificadoSATField;
            }
            set
            {
                this.noCertificadoSATField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SelloSAT
        {
            get
            {
                return this.selloSATField;
            }
            set
            {
                this.selloSATField = value;
            }
        }
    }

}

