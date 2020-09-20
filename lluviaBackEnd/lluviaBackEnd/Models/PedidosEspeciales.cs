using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class PedidosEspeciales
    {

        public int idPedidoEspecial { get; set; }
        public int idCliente { get; set; }
        public int idUsuario { get; set; }
        public DateTime fechaAlta { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public List<PedidosInternosDetalle> lstPedidosInternosDetalle { get; set; }
        public Almacen almacenOrigen { get; set; }
        public Almacen almacenDestino { get; set; }
        public Usuario usuario { get; set; }
        public Status estatusPedido { get; set; }
        //public Producto producto { get; set; }
        public int idProducto { get; set; }
        public int idSucursal { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public PedidosEspeciales()
        {
            almacenOrigen = new Almacen();
            almacenDestino = new Almacen();
            usuario = new Usuario();
            estatusPedido = new Status();
            lstPedidosInternosDetalle = new List<PedidosInternosDetalle>();
        }
    }
}