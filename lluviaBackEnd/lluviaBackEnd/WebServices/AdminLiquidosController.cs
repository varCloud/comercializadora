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
    public class AdminLiquidosController : ApiController
    {
        // GET api/<controller>
        public Notificacion<List<Producto>> obtenerProductosXLineaProducto(RequestObtenerProductosXLineaProducto request)
        {
            try {
                return new ProductosDAO().obtenerProductosXLineaProducto(request);
            }
            catch (Exception ex)
            {
                return WsUtils<List<Producto>>.RegresaExcepcion(ex, null);
            }
        
        }

        public Notificacion<string> agregarLiquidosAInventario(RequestAgregarProductoInventarioLiquidos request)
        {
            try
            {
                return new InventarioDAO().agregarLiquidosAInventario(request);
            }
            catch (Exception ex)
            {
                return WsUtils<string>.RegresaExcepcion(ex, null);
            }

        }

        public Notificacion<dynamic> BuscarCargaMercanciaLiquidos(FiltroLiquidos filtroLiquidos)
        {
            try
            {
                return new ProductosDAO().BuscarCargaMercanciaLiquidos(filtroLiquidos);
            }
            catch (Exception ex)
            {

                return WsUtils<dynamic>.RegresaExcepcion(ex, null);
            }
        }
    }
}