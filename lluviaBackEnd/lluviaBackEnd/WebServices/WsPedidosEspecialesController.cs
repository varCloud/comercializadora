﻿using lluviaBackEnd.DAO;
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
    public class WsPedidosEspecialesController : ApiController
    {

        [HttpPost]
        public Notificacion<dynamic> ObtenerNotificacionesPedidosEspeciales(RequestObtenerNotificacionesPedidosEspeciales request)
        {
            try
            {
                return new AppPedidosEspecialesDAO().ObtenerNotificacionesPedidosEspeciales(request);
            }

            catch (Exception ex)
            {
                return WsUtils<dynamic>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<dynamic> ObtenerPedidoEspecialesXAlmacen(RequestObtenerPedidosEspecialesXAlmacen request)
        {
            try
          {
                return new AppPedidosEspecialesDAO().ObtenerPedidoseEspecialesXAlmacen(request);
            }

            catch (Exception ex)
            {
                return WsUtils<dynamic>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<dynamic> ObtenerDetallePedidosEspeciales(RequestObtenerDetallePedidoEspecial request)
        {
            try
            {
                return new AppPedidosEspecialesDAO().ObtenerDetallePedidoseEspecial(request);
            }

            catch (Exception ex)
            {
                return WsUtils<dynamic>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<String> AprobarPedidosEspeciales(RequestAprobarPedidoEspecial request)
        {
            try
            {
                return new AppPedidosEspecialesDAO().AprobarPedidosEspeciales(request);
            }

            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }
        }

        [HttpPost]
        public Notificacion<String> RechazarPedidosEspeciales(RequestRechazarPedidoInternoEspecial request)
        {
            try
            {
                return new AppPedidosEspecialesDAO().RechazarPedidosEspeciales(request);
            }

            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }
        }
    }
}
