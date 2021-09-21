using Dapper;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.DAO
{
    public static class ConstructorDapper
    {

        public static Notificacion<dynamic> Consultar(string nombreSP, DynamicParameters parametros)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                using (var db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple(nombreSP, parametros, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    notificacion.Estatus = r1.status;
                    notificacion.Mensaje = r1.mensaje;
                    if (r1.status == 200)
                        notificacion.Modelo = result.Read();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public static Notificacion<string> Ejecutar(string nombreSP, DynamicParameters parametros)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (var db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    notificacion = db.QuerySingle<Notificacion<String>>(nombreSP, parametros, commandType: CommandType.StoredProcedure);

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