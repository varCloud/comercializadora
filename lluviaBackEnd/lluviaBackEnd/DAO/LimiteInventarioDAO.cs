using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.DAO
{
    public class LimiteInventarioDAO
    {
        private IDbConnection db;
        public Notificacion<List<LimiteInvetario>> ObtenerLimitesInventario(RequestObtenerLimitesInventario request)
        {
            Notificacion<List<LimiteInvetario>> notificacion = new Notificacion<List<LimiteInvetario>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", request.idProducto == 0 ? (object)null : request.idProducto);
                    parameters.Add("@idAlmacen", request.idAlmacen == 0 ? (object)null : request.idAlmacen);
                    parameters.Add("@idEstatusLimiteInv", request.idEstatusLimiteInv == 0 ? (object)null : request.idEstatusLimiteInv);
                    var result = db.QueryMultiple("SP_OBTENER_LIMITES_INVENTARIO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<LimiteInvetario , Status, LimiteInvetario>((limiteInvetario , status)=> {
                            limiteInvetario.estatusInventario = status;
                           return limiteInvetario;


                        }, splitOn:"idEstatusLimiteInventario").ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
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