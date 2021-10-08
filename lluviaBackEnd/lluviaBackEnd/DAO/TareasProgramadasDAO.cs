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
    public class TareasProgramadasDAO
    {
        private IDbConnection db;

        public Notificacion<String> CierreDeInventarioDiario()
        {
            Notificacion<String> notificacion = null;
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    //parameters.Add("@idTipoMovInventario", request.idTipoMovInventario);
                    notificacion = this.db.QuerySingle<Notificacion<String>>("SP_TAREA_PROGRAMADA_INVENTARIO_PROM", param: null, commandType: CommandType.StoredProcedure);
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