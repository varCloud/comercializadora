using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using lluviaBackEnd.WebServices.Modelos.Request;
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
        [HttpPost]
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

        [HttpPost]
        public Notificacion<List<Producto>> ObtenerProductos(RequestObtenerProductos request)
        {
            try
            {
                Producto p = new Producto();
                p.articulo = request.articulo;
                p.idLineaProducto = request.idLineaProducto.ToString();
                p.descripcion= request.descripcion;
                p.idProducto = request.idProducto;
                return new ProductosDAO().ObtenerProductos(p);
            }
            catch (Exception ex)
            {
                return WsUtils<List<Producto>>.RegresaExcepcion(ex, null);
            }
        }

        [HttpPost]
        public Notificacion<Producto> ObtenerProductoXCodigo(RequestObtenerProductoXCodigo request)
        {
            try
            {
                return new ProductosDAO().ObtenerProductoXCodigo(request);
            }
            catch (Exception ex)
            {
                return WsUtils<Producto>.RegresaExcepcion(ex, null);
            }
        }
    }
}
