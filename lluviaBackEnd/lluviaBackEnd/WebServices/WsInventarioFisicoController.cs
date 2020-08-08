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
    public class WsInventarioFisicoController : ApiController
    {
        [HttpPost]
        public Notificacion<String> RegistraProductoInventarioFisico(RegistraProductoInventarioFisico request)
        {
            try
            {
                return new InventarioFisicoDAO().RegistraProductoInventarioFisico(request);
            }
            catch (Exception ex)
            {

                return WsUtils<string>.RegresaExcepcion(ex, null);
            }
        }

        [HttpPost]
        public Notificacion<String> ValidaInventarioFisico(RequesValidaInventarioFisico request)
        {
            try
            {
                return new InventarioFisicoDAO().ValidaInventarioFisico(request);
            }
            catch (Exception ex)
            {

                return WsUtils<String>.RegresaExcepcion(ex, null);
            }
        }
    }
}
