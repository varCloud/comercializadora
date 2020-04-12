using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public enum EnumTipoCliente
    {
        General = 1,
        A,
        B,
        VIP,
    }

    public enum EnumEstatusFactura
    {
        Facturada = 1,
        Cancelada,
        Error,
        
    }
}