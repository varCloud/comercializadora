using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;

namespace lluviaBackEnd.DAO
{
    public class InventarioFisicoDAO
    {
        private IDbConnection db;

        public Notificacion<String> RegistraProductoInventarioFisico(RegistraProductoInventarioFisico request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idInventarioFisico", request.idInventarioFisico);
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@idUbicacion", request.idUbicacion);
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@cantidadEnFisico", request.cantidadEnFisico);
                    notificacion = db.QuerySingle<Notificacion<String>>("SP_APP_REGISTRA_PRODUCTO_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<String> ValidaInventarioFisico()
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    var rs = db.QueryMultiple("SP_APP_VALIDA_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = rs.ReadFirst();
                    notificacion.Mensaje = r1.Mensaje;
                    notificacion.Estatus = r1.Estatus;
                    notificacion.Modelo = r1.idInventarioFisico;
                                             
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }
    }
}