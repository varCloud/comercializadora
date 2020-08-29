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
    public class WsLimitesInventarioController : ApiController
    {
        [HttpPost]
        public Notificacion<List<LimiteInvetario>> ObtenerLimitesInventario(RequestObtenerLimitesInventario request)
        {
            try
            {
                return new LimiteInventarioDAO().ObtenerLimitesInventario(request);
            }
            catch (Exception ex)
            {
                return WsUtils<List<LimiteInvetario>>.RegresaExcepcion(ex, null);
            }
        }
    }
}
