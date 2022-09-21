using Dapper;
using lluviaBackEnd.Models;
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
                    var result = db.QueryMultiple("SP_CONSULTA_ESTATUS_PROCESO_PRODUCCION", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        lstMeses = result.Read<SelectListItem>().ToList();
                    }
                    lstMeses.Insert(0, new SelectListItem() { Text = "TODOS", Value = "0", Selected = true });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstMeses;
        }
        public Notificacion<List<CostoProduccionAgranel>> ObtenerProcesoProduccion(FiltroCostoProduccionAgranel f)
        {
            Notificacion<List<CostoProduccionAgranel>> lstProcesoProduccion = new Notificacion<List<CostoProduccionAgranel>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", f.idUsuario == 0 ? (object)null : f.idUsuario);
                    parameters.Add("@idEstatusProcesoProduccionAgranel", f.idEstatusProcesoProduccionAgranel == 0 ? (object)null : f.idEstatusProcesoProduccionAgranel);
                    parameters.Add("@fechaIni", f.fechaIni.ToString("yyyyMMdd"));
                    parameters.Add("@fechaFin", f.fechaFin.ToString("yyyyMMdd"));
                    var result = db.QueryMultiple("SP_CONSULTA_PROCESO_PRODUCCION", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        lstProcesoProduccion.Estatus = 200;
                        lstProcesoProduccion.Mensaje = "Productos encontrados";
                        lstProcesoProduccion.Modelo = result.Read<CostoProduccionAgranel>().ToList();
                    }
                    else
                    {
                        lstProcesoProduccion.Mensaje = "No se encotraron resultados ...";
                        lstProcesoProduccion.Estatus = -1;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstProcesoProduccion;
        }

    }
}