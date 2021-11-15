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
        public List<lluviaBackEnd.Models.Producto> lstProductos { get; set; }

    }
}