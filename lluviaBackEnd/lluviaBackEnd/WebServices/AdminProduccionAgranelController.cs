using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using lluviaBackEnd.WebServices.Modelos.Request;
using lluviaBackEnd.WebServices.Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace lluviaBackEnd.WebServices
{
    public class AdminProduccionAgranelController : ApiController
    {

        public Notificacion<string> agregarProductosProduccionAgranel(RequestAgregarProductoProduccionAgranel request)
        {
            try
            {
                return new InventarioDAO().agregarProductosProduccionAgranel(request);
            }
            catch (Exception ex)
            {
                return WsUtils<string>.RegresaExcepcion(ex, null);
            }

        }

        public Notificacion<List<ResponseObtenerProductosProduccionAgranel>> obtenerProductosProduccionAgranel(RequestObtenerProductosProduccionAgranel request)
        {
            try
            {
                return new InventarioDAO().obtenerProductosProduccionAgranel(request);
            }
            catch (Exception ex)
            {
                return WsUtils<List<ResponseObtenerProductosProduccionAgranel>>.RegresaExcepcion(ex, null);
            }

        }

                public Notificacion<String> aprobarProductosProduccionAgranel(RequestAprobarProductosProduccionAgranel request)
        {
            try
            {
                return new InventarioDAO().aprobarProductosProduccionAgranel(request);
            }
            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }

        }

        
    }
}
