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
    public class WsInventarioController : ApiController
    {
        [HttpPost]
        public Models.Notificacion<String> AgreagrProductoInventario(RequestAgreagrProductoInventario request)
        {
            try
            {
                return new InventarioDAO().AgreagrProductoInventario(request);
            }

            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Models.Notificacion<List<Ubicacion>> ObtenerUbicacionProductoInventario(RequestObtenerUbicacionProductoInventario request)
        {
            try
            {
                return new InventarioDAO().ObtenerUbicacionProductoInventario(request);
            }

            catch (Exception ex)
            {
                return WsUtils< List<Ubicacion>>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Models.Notificacion<ResponseObtenerPisoPasilloRaq> ObtenerPasilloRaqPiso()
        {
            try
            {
                return new InventarioDAO().ObtenerPasilloRaqPiso();
            }

            catch (Exception ex)
            {
                return WsUtils<ResponseObtenerPisoPasilloRaq>.RegresaExcepcion(ex, null);
            }

        }


    }
}
