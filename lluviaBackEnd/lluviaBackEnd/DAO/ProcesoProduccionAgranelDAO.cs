using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.DAO
{
    public class ProcesoProduccionAgranelDAO
    {
        private IDbConnection db;
        public List<SelectListItem> ObtenerEstatusProduccionProcesoAgranel()
        {
            List<SelectListItem> lstMeses = new List<SelectListItem>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    var result = db.QueryMultiple("SP_CONSULTA_ESTATUS PROCESO_PRODUCCION", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        lstMeses = result.Read<SelectListItem>().ToList();
                    }
                    lstMeses.Insert(0, new SelectListItem() { Text = "TODOS", Value = "0", Selected=true });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstMeses;
        }
    }
}