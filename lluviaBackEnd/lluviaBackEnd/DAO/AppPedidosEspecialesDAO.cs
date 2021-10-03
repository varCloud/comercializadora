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

    }
}