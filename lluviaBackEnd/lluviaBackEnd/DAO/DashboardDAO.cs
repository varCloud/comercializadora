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
    public class DashboardDAO
    {
        private IDbConnection db;

        public Notificacion<List<Categoria>> ObtenerVentasPorFecha(EnumTipoReporteGrafico idTipoReporte,int idEstacion=0)
        {
            Notificacion<List<Categoria>> categoria = new Notificacion<List<Categoria>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters= new DynamicParameters();
                    parameters.Add("@idTipoReporte", idTipoReporte);
                    parameters.Add("@@idEstacion", idEstacion == 0 ? (object)null : idEstacion);
                    var rs = db.QueryMultiple("SP_DASHBOARD_CONSULTA_TOTAL_VENTAS_POR_FECHA", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if(rs1.status==200)
                    {
                        categoria.Estatus = rs1.status;
                        categoria.Mensaje = rs1.mensaje;
                        categoria.Modelo = rs.Read<Categoria>().ToList();
                    }
                    else
                    {
                        categoria.Estatus = rs1.status;
                        categoria.Mensaje = rs1.mensaje;
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return categoria;
        }

        public Notificacion<List<Estacion>> ObtenerVentasEstacion()
        {
            Notificacion<List<Estacion>> grafico = new Notificacion<List<Estacion>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                     var rs = db.QueryMultiple("SP_DASHBOARD_CONSULTA_TOTAL_VENTAS_POR_ESTACION", null, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        grafico.Estatus = rs1.status;
                        grafico.Mensaje = rs1.mensaje;
                        grafico.Modelo = rs.Read<Estacion>().ToList();
                    }
                    else
                    {
                        grafico.Estatus = rs1.status;
                        grafico.Mensaje = rs1.mensaje;
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return grafico;
        }


    }
}