using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class CierrePedidosEspeciales
    {
		public Int64 idCierrePedidoEspecial { get; set; }
		public DateTime fechaCierre { get; set; }
		public string nombreUsuario { get; set; }
		public float VentasContado { get; set; }
		public float VentasTC { get; set; }
		public float VentasTransferencias { get; set; }
		public float VentasOtrasFormasPago { get; set; }
		public float VentasCredito { get; set; }
		public float MontoDevoluciones { get; set; }
		public float MontoIngresosEfectivo { get; set; }
		public float MontoRetirosEfectivo { get; set; }
		public float MontoCierreEfectivo { get; set; }
		public float MontoCierreTC { get; set; }
		public float EfectivoEntregadoEnCierre { get; set; }
		public int noDevoluciones { get; set; }
		public int NoTicketsEfectivo { get; set; }
		public int NoTicketsCredito { get; set; }
		public int NoPedidosEnResguardo { get; set; }
	}
}