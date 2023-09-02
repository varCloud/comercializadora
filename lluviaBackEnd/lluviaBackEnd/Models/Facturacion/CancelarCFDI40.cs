using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace lluviaBackEnd.Models.Facturacion
{
    [XmlRoot(Namespace = "http://cancelacfd.sat.gob.mx", IsNullable = false, ElementName = "Cancelacion")]
    public class CancelarCFDI40
    {
        private CancelacionFolios foliosField;

        private string fechaField;

        private string rfcEmisorField;

        /// <remarks/>
        public CancelacionFolios Folios
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cancelacfd.sat.gob.mx")]
    public partial class CancelacionFolios
    {

        private CancelacionFoliosFolio folioField;

        /// <remarks/>
        public CancelacionFoliosFolio Folio
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://cancelacfd.sat.gob.mx")]
    public partial class CancelacionFoliosFolio
    {

        private string uUIDField;

        private string motivoField;

        private string folioSustitucionField;

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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FolioSustitucion
        {
            get
            {
                return this.folioSustitucionField;
            }
            set
            {
                this.folioSustitucionField = value;
            }
        }

    }
}