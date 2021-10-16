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
    public class WsPedidoInternoController : ApiController
    {

        [HttpPost]
        public Notificacion<List<Status>> ObtenerStatusPedidosInternos()
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
        public Notificacion<String> AceptarPedidoInterno(RequestActualizarEstatusPedidoInterno request)
        {
            try
            {
                return new BitacoraDAO().AceptarPedidoInterno(request);
            }

            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<String> RechazarPedidoInterno(RequestActualizarEstatusPedidoInterno request)
        {
            try
            {
                return new BitacoraDAO().RechazarPedidoInterno(request);
            }

            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }

        }


        [HttpPost]
        public Notificacion<List<ResponseObtenerPedidosInternos>> ObtenerPedidosInternosUsuarioApp(RequestObtenerPedidosInternosUsuario request)
        {
            try
            {
                return new BitacoraDAO().ObtenerPedidosInternosUsuarioApp(request);
            }

            catch (Exception ex)
            {
                return WsUtils<List<ResponseObtenerPedidosInternos>>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<List<ResponseObtenerPedidosInternos>> ObtenerPedidosInternosAlamcenApp(RequestObtenerPedidosInternosAlamcen request)
        {
            try
            {
                return new BitacoraDAO().ObtenerPedidosInternosAlmacenApp(request);
            }

            catch (Exception ex)
            {
                return WsUtils<List<ResponseObtenerPedidosInternos>>.RegresaExcepcion(ex, null);
            }

        }

        //[HttpPost]
        //public Notificacion<List<ResponseObtenerPedidosInternos>> ObtenerPedidosInternosApp(RequestObtenerPedidosInternos request)
        //{
        //    try
        //    {
        //        return new BitacoraDAO().ObtenerPedidosInternosApp(request);
        //    }

        //    catch (Exception ex)
        //    {
        //        return WsUtils<List<ResponseObtenerPedidosInternos>>.RegresaExcepcion(ex, null);
        //    }

        //}


        [HttpPost]
        public Notificacion<List<DetallePedidoInterno>> ObtenerDetallePedidoInterno(RequestObtenerDetallePedidoInterno request)
        {
            try
            {
                return new BitacoraDAO().ObtenerDetallePedidoInterno(request);
            }

            catch (Exception ex)
            {
                return WsUtils<List<DetallePedidoInterno>>.RegresaExcepcion(ex, null);
            }

        }

        [HttpPost]
        public Notificacion<List<ResponseObtenerPedidosInternosEspeciales>> ObtenerPedidosInternoEspecialesUsuariosApp(RequestObtenerPedidosInternosUsuario request)
        {
            try
            {
                return new BitacoraDAO().ObtenerPedidosInternoEspecialesUsuariosApp(request);
            }

            catch (Exception ex)
            {
                return WsUtils<List<ResponseObtenerPedidosInternosEspeciales>>.RegresaExcepcion(ex, null);
            }

        }

        //[HttpPost]
        //public Notificacion<List<ResponseObtenerPedidosInternosEspeciales>> ObtenerPedidosInternoEspecialesAlmacenApp(RequestObtenerPedidosInternosAlamcen request)
        //{
        //    try
        //    {
        //        return new BitacoraDAO().ObtenerPedidosInternoEspecialesAlmacenApp(request);
        //    }

        //    catch (Exception ex)
        //    {
        //        return WsUtils<List<ResponseObtenerPedidosInternosEspeciales>>.RegresaExcepcion(ex, null);
        //    }

        //}

        //[HttpPost]
        //public Notificacion<String> AprobarPedidosInternosEspeciales(RequestAprobarPedidoEspecial request)
        //{
        //    try
        //    {
        //        return new BitacoraDAO().AprobarPedidosInternosEspeciales(request);
        //    }

        //    catch (Exception ex)
        //    {
        //        return WsUtils<String>.RegresaExcepcion(ex, null);
        //    }
        //}

        //[HttpPost]
        //public Notificacion<String> RechazarPedidosInternosEspeciales(RequestRechazarPedidoInternoEspecial request)
        //{
        //    try
        //    {
        //        return new BitacoraDAO().RechazarPedidosInternosEspeciales(request);
        //    }

        //    catch (Exception ex)
        //    {
        //        return WsUtils<String>.RegresaExcepcion(ex, null);
        //    }
        //}

        [HttpPost]
        public Notificacion<List<ResponseObtenerPedidosInternos>> ObtenerNotificacionesPedidosInternos(RequestObtenerNotificacionesPedidosInternos request)
        {
            try
            {
                return new BitacoraDAO().ObtenerNotificacionesPedidosInternos(request);
            }

            catch (Exception ex)
            {
                return WsUtils<List<ResponseObtenerPedidosInternos>>.RegresaExcepcion(ex, null);
            }

        }


    }
}
