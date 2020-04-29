using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models.Facturacion
{

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public class ComprobanteAddenda
    {
        public string NombreCliente { get; set; }

        public int MyProperty { get; set; }
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