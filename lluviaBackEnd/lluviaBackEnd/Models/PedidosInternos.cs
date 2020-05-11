using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class PedidosInternos
    {
        public int idPedidoInterno { get; set; }
        public DateTime fechaAlta { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }

        public Almacen almacenOrigen { get; set; }
        public Almacen almacenDestino { get; set; }
        public Usuario usuario { get; set; }
        public Status estatusPedido { get; set; }
        public Producto producto { get; set; }       
        
        public PedidosInternos()
        {
            almacenOrigen = new Almacen();
            almacenDestino = new Almacen();
            usuario = new Usuario();
            estatusPedido = new Status();
            producto = new Producto();
        }
    }
}