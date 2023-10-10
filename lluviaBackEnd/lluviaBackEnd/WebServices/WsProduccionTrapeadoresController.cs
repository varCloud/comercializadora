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
    public class WsProduccionTrapeadoresController : ApiController
    {
        public Notificacion<string> agreagrProductoInventarioTrapeadoresFinalizados(RequestAgregarProductoInventarioLiquidos request)
        {
            try
            {
                return new InventarioDAO().agreagrProductoInventarioTrapeadoresFinalizados(request);
            }
            catch (Exception ex)
            {
                return WsUtils<string>.RegresaExcepcion(ex, null);
            }

        }
    }
}
