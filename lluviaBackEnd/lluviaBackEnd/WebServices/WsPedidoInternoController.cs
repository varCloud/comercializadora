﻿using lluviaBackEnd.DAO;
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
    public class WsPedidoInternoController : ApiController
    {

        [HttpPost]
        public  Notificacion<List<Status>> ObtenerStatusPedidosInternos()
        {
            try
            {
                return new BitacoraDAO().ObtenerStatusPedidosInternos();
            }

            catch (Exception ex)
            {
                return WsUtils<List<Status>>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<String> GenerarPedidoInterno(RequestGenerarPedidoInterno request)
        {
            try
            {
                return new BitacoraDAO().GenerarPedidoInterno(request);
            }

            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<String> ActualizarEstatusPedidoInterno(RequestActualizarEstatusPedidoInterno request)
        {
            try
            {
                return new BitacoraDAO().ActualizarEstatusPedidoInterno(request);
            }

            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<List<ResponseObtenerPedidosInternos>> ObtenerPedidosInternosApp(RequestObtenerPedidosInternos request)
        {
            try
            {
                return new BitacoraDAO().ObtenerPedidosInternosApp(request);
            }

            catch (Exception ex)
            {
                return WsUtils<List<ResponseObtenerPedidosInternos>>.RegresaExcepcion(ex, null);
            }

        }

        
    }
}