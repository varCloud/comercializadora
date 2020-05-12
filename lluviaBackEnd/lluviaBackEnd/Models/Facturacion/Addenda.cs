using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models.Facturacion
{

    [System.Xml.Serialization.XmlTypeAttribute( AnonymousType = true, Namespace = "")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class ComprobanteAddenda
    {
        public List<ConceptosAddenda> conceptosAddenda { get; set; }

        public string descripcionUsoCFDI { get; set; }

        public string descripcionFormaPago { get; set; }
    }

    public class ConceptosAddenda
    {

        public string ClaveProdserv { get; set; }
        public string DescripcionClaveProdServ { get; set; }

        public string ClaveUnidad { get; set; }
        public string DescripcionClaveUnidad { get; set; }

        public int Cantidad { get; set; }

        public string Unidad { get; set; }

        public string NoIdentificacion { get; set; }

        public string Descripcion { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal Importe { get; set; }

        public decimal IVA { get; set; }
    }
}