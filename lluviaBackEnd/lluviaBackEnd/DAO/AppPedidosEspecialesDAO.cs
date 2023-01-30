using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace lluviaBackEnd.DAO
{
    public class AppPedidosEspecialesDAO
    {
        public Notificacion<dynamic> ObtenerNotificacionesPedidosEspeciales(RequestObtenerNotificacionesPedidosEspeciales request)
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
                parameters.Add("@fechaInicio", request.fechaInicio == DateTime.MinValue ? (object)null : request.fechaInicio);
                parameters.Add("@fechaFin", request.fechaFin == DateTime.MinValue ? (object)null : request.fechaFin);
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

        public Notificacion<String> AprobarPedidosEspeciales(RequestAprobarPedidoEspecial request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();

            try
            {                
                    var parameters = new DynamicParameters();
                    parameters.Add("@xmlProductos", SerializeProductos(request.Productos));
                    parameters.Add("@idPedidoEspecial", request.idPedidoEspecial);
                    parameters.Add("@idAlmacenOrigen", request.idAlmacenOrigen);         
                    parameters.Add("@idAlmacenDestino", request.idAlmacenDestino);
                    parameters.Add("@idUsuario", request.idUsuario);
                    notificacion = ConstructorDapper.Ejecutar("SP_APP_APROBAR_PEDIDOS_ESPECIALES", parameters);     
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public string SerializeProductos(List<ProductosPedidoEspecial> precios)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<ProductosPedidoEspecial>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                xmlSerializer.Serialize(xmlWriter, precios);
            }
            Debug.WriteLine(stringBuilder.ToString());
            return stringBuilder.ToString();

        }

        public Notificacion<String> RechazarPedidosEspeciales(RequestRechazarPedidoInternoEspecial request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idPedidoEspecial", request.idPedidoEspecial);
                parameters.Add("@idUsuario", request.idUsuario);
                parameters.Add("@idAlmacen", request.idAlmacen);
                parameters.Add("@observaciones", request.observaciones);
                notificacion = ConstructorDapper.Ejecutar("SP_APP_RECHAZAR_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


    }
}