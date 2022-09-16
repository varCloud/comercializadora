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
                    parameters.Add("@idEstacion", idEstacion == 0 ? (object)null : idEstacion);
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

        public Notificacion<List<Estacion>> ObtenerVentasEstacion(DateTime ? fechaIni,DateTime ? fechaFin,int idEstacion=0)
        {
            Notificacion<List<Estacion>> grafico = new Notificacion<List<Estacion>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@fechaIni",fechaIni==DateTime.MinValue ? (object) null : fechaIni);
                    parameters.Add("@fechaFin",fechaFin==DateTime.MinValue ? (object) null : fechaFin);
                    parameters.Add("@idEstacion", idEstacion == 0 ? (object)null : idEstacion);
                    var rs = db.QueryMultiple("SP_DASHBOARD_CONSULTA_TOTAL_VENTAS_POR_ESTACION", parameters, commandType: CommandType.StoredProcedure);
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

        public Notificacion<List<Categoria>> ObtenerTopTen(EnumTipoReporteGrafico idTipoReporte, EnumTipoGrafico idTipoGrafico,int idEstacion = 0)
        {
            Notificacion<List<Categoria>> categoria = new Notificacion<List<Categoria>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idTipoReporte", idTipoReporte);
                    parameters.Add("@idTipoGrafico", idTipoGrafico);
                    parameters.Add("@idEstacion", idEstacion == 0 ? (object)null : idEstacion);
                    var rs = db.QueryMultiple("SP_DASHBOARD_CONSULTA_TOP_TEN", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
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

        public Notificacion<List<Categoria>> ObtenerInformacionGlobal(EnumTipoReporteGrafico idTipoReporte, int idEstacion=0)
        {
            Notificacion<List<Categoria>> categoria = new Notificacion<List<Categoria>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idTipoReporte", idTipoReporte);
                    parameters.Add("@idEstacion", idEstacion == 0 ? (object)null : idEstacion);
                    var rs = db.QueryMultiple("SP_DASHBOARD_CONSULTA_INFORMACION_GLOBAL", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
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

        public Notificacion<List<MermaMensual>> ObtenerMermaMensual()
        {
            Notificacion<List<MermaMensual>> categoria = new Notificacion<List<MermaMensual>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    var rs = db.QueryMultiple("SP_DASHBOARD_MERMA", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        categoria.Estatus = rs1.status;
                        categoria.Mensaje = rs1.mensaje;
                        categoria.Modelo = rs.Read<MermaMensual>().ToList();
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

        public Notificacion<List<Categoria>> ObtenerIvaAcumuladoPorFecha(EnumTipoReporteGrafico idTipoReporte, int idEstacion = 0)
        {
            Notificacion<List<Categoria>> categoria = new Notificacion<List<Categoria>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idTipoReporte", idTipoReporte);
                    parameters.Add("@idEstacion", idEstacion == 0 ? (object)null : idEstacion);
                    var rs = db.QueryMultiple("SP_DASHBOARD_OBTENER_IVA_ACUMULADO", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
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

        public Notificacion<List<CostoProduccionAgranelMensual>> ObtenerCostoProduccionMensualAgranel()
        {
            Notificacion<List<CostoProduccionAgranelMensual>> categoria = new Notificacion<List<CostoProduccionAgranelMensual>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    var rs = db.QueryMultiple("SP_DASHBOARD_COSTO_PRODUCCION", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        categoria.Estatus = rs1.status;
                        categoria.Mensaje = rs1.mensaje;
                        categoria.Modelo = rs.Read<CostoProduccionAgranelMensual>().ToList();
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

    }
}
