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
    public class WsLineaProductoController : ApiController
    {

       [HttpPost]
        public Models.Notificacion<List<LineaProducto>> ObtenerlineaProducto(RequestObtenerLineaProducto request)
        {
            try
            {
                List<LineaProducto> list = new LineaProductoDAO().ObtenerLineaProductos( new LineaProducto() { idLineaProducto = request.idLineaProducto },0);
                return new Notificacion<List<LineaProducto>>()
                {
                    Estatus = list.Count > 0 ?  200 : -1,
                    Mensaje = list.Count > 0 ? "OK" : "Sin resultados",
                    Modelo = list
                };
            }
            
            catch (Exception ex)
            {
                return WsUtils<List<LineaProducto>>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public void GuardarLineaProductos() {

        }
    }
}
