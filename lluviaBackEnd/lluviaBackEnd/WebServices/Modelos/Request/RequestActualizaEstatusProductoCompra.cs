using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestActualizaEstatusProductoCompra
    {
        public int idAlmacen { get; set; }
        public int idUsuario { get; set; }
        public int idCompra { get; set; }
        public List<CompraDetalle> Productos { get; set; }
    }

}