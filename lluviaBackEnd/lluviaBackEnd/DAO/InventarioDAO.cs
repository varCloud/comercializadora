using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Web;
using lluviaBackEnd.WebServices.Modelos.Request;

namespace lluviaBackEnd.DAO
{
    public class InventarioDAO
    {
        private IDbConnection db;


        public Notificacion<String> AgreagrProductoInventario(RequestAgreagrProductoInventario request)
        {
            Notificacion<String> notificacion = null;
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@idProveedor", request.idProveedor);
                    parameters.Add("@cantidad", request.cantidad);
                    parameters.Add("@idUsuario", request.idUsuario);
                    notificacion = this.db.QuerySingle<Notificacion<String>>("SP_AGREGAR_PRODUCTO_INVENTARIO",param:parameters, commandType: CommandType.StoredProcedure);
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