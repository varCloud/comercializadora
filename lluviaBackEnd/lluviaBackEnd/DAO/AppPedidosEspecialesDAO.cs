using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.DAO
{
    public class AppPedidosEspecialesDAO
    {
        public Notificacion<dynamic> ObtenerNotificacionesPedidosInternos(RequestObtenerNotificacionesPedidosEspeciales request)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idEstatusPedidoEspecialDetalle", request.idEstatusPedidoEspecialDetalle == 0 ? (object)null : request.idEstatusPedidoEspecialDetalle);
                parameters.Add("@idAlmacenOrigen", request.idAlmacenOrigen == 0 ? (object)null : request.idAlmacenOrigen);
                parameters.Add("@idAlmacenDestino", request.idAlmacenDestino == 0 ? (object)null : request.idAlmacenDestino);
                notificacion = ConstructorDapper.Consultar("SP_APP_PEDIDOS_ESPECIALES_OBTENER_NOTIFICACIONES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerPedidoseEspecialesXAlmacen(RequestObtenerPedidosEspecialesXAlmacen request)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idAlmacen", request.idAlmacen == 0 ? (object)null : request.idAlmacen);
                parameters.Add("@idEstatusPedidoEspecialDetalle", request.idEstatusPedidoEspecialDetalle == 0 ? (object)null : request.idEstatusPedidoEspecialDetalle);
                notificacion = ConstructorDapper.Consultar("SP_APP_PEDIDOS_ESPECIALES_OBTENER_PEDIDOS_ESPECIALES_X_ALMACEN", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerDetallePedidoseEspecial(RequestObtenerDetallePedidoEspecial request)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idPedidoEspecial", request.idPedidoEspecial == 0 ? (object)null : request.idPedidoEspecial);
                parameters.Add("@idAlmacen", request.idAlmacen == 0 ? (object)null : request.idAlmacen);
                notificacion = ConstructorDapper.Consultar("SP_APP_PEDIDOS_ESPECIALES_OBTENER_DETALLE_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

    }
}