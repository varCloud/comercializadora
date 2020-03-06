using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace lluviaBackEnd.WebServices
{
    public class WsProductosController : ApiController
    {

        public Notificacion<Producto> AgregarProducto(Producto producto)
        {
            try
            {
                return new ProductosDAO().GuardarProducto(producto);
            }
            catch (Exception ex)
            {
                return WsUtils<Producto>.RegresaExcepcion(ex, producto);
            }

        }
    }
}
