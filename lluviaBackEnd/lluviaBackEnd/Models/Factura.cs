using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Factura
    {
        public Int64 idFactura { get; set; }
        public string idVenta { get; set; }
        public string folio { get; set; }
        public EnumEstatusFactura estatusFactura { get; set; }
        public string mensajeError { get; set; }
        public string  UUID { get; set; }
        public DateTime fechaTimbrado { get; set; }
        public int idUsuario { get; set; }

    }
}