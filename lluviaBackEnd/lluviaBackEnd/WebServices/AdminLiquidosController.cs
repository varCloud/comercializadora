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
        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}