using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class PedidosEspecialesV2
    {
        public int id { get; set; }
        public int idPedidoEspecial { get; set; }
        public int idCliente { get; set; }
        public int idUsuario { get; set; }
        public int idUsuarioRuteo { get; set; }
        public int idUsuarioTaxi { get; set; }
        public int formaPago { get; set; }
        public int usoCFDI { get; set; }
        public int idSucursal { get; set; }
        public int idAlmacen { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public DateTime fechaAlta { get; set; }
        public string nombreCliente { get; set; }
        public float montoTotal { get; set; }
        public float cantidad { get; set; }
        public string nombreUsuario { get; set; }
        public bool puedeEntregar { get; set; }
        public bool ticketVistaPrevia { get; set; }
        public int idPedidoEspecialMayoreo { get; set; }
        public int idEstatusPedidoEspecial { get; set; }
        public int idTicketMayoreo { get; set; }
        public int idFactMetodoPago { get; set; }
        public int idFactFormaPago { get; set; }
        public int idFactUsoCFDI { get; set; }
        public int idEstatusFactura { get; set; }
        public float montoIVA { get; set; }
        public float precioVenta { get; set; }
        public float ultimoCostoCompra { get; set; }
        public float ganancia { get; set; }
        public string rutaFactura { get; set; }
        public string sucursal { get; set; }
        public string tienda { get; set; }
        public string codigoBarrasTicket { get; set; }
        public string linea { get; set; }
        public string producto { get; set; }
        public string descripcionFactFormaPago { get; set; }
        public int idLineaProducto { get; set; }
        public List<lluviaBackEnd.Models.Producto> lstProductos { get; set; }

    }
}