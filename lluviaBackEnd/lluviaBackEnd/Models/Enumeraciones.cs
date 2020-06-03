using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public enum EnumTipoGrafico
    {
        VentasPorFecha = 1,
        TopTenProductos = 2,
        TopTenClientes = 3,
        TopTenProvedores = 4,
        InformacionGlobal = 5
    }

    public enum EnumTipoReporteGrafico
    {
        Semanal = 1,
        Mensuales = 2,
        Anuales = 3,
        Dia = 4
    }

    public enum EnumTipoRetiro
    {
        [Description("Retiros Exceso Efectivo")]
        RetirosExcesoEfectivo =1,

        [Description("Retiros Cierre Diario")]
        RetirosCierreDia=2
    }
}