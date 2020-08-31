using AccesoDatos;
using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

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

        public Notificacion<List<LimiteInvetario>> ObtenerLimitesInventario(LimiteInvetario limite)
        {
            Notificacion<List<LimiteInvetario>> notificacion = new Notificacion<List<LimiteInvetario>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", limite.idProducto == 0 ? (object)null : limite.idProducto);
                    parameters.Add("@idAlmacen", limite.idAlmacen == 0 ? (object)null : limite.idAlmacen);
                    parameters.Add("@idEstatusLimiteInv", limite.estatusInventario.idStatus == 0 ? (object)null : limite.estatusInventario.idStatus);
                    parameters.Add("@idLineaProducto", limite.idLineaProducto == 0 ? (object)null : limite.idLineaProducto);
                    var result = db.QueryMultiple("SP_OBTENER_LIMITES_INVENTARIO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<LimiteInvetario, Status, LimiteInvetario>((limiteInvetario, status) => {
                            limiteInvetario.estatusInventario = status;
                            return limiteInvetario;


                        }, splitOn: "idEstatusLimiteInventario").ToList();
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

        public List<Status> ObtenerEstatusLimitesInventario()
        {
            List<Status> lstStatus = new List<Status>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    lstStatus = db.Query<Status>("SP_OBTENER_ESTATUS_LIMITES_INVENTARIO", null, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstStatus;
        }

        public Notificacion<string> InsertaActualizaLimiteInventario(LimiteInvetario limiteInvetario,int idUsuario)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", limiteInvetario.idProducto);
                    parameters.Add("@idAlmacen", limiteInvetario.idAlmacen);
                    parameters.Add("@idUsuario", idUsuario);
                    parameters.Add("@minimo", limiteInvetario.minimo);
                    parameters.Add("@maximo", limiteInvetario.maximo);
                    notificacion = db.QuerySingle<Notificacion<string>>("SP_INSERTA_ACTUALIZA_LIMITE_INVENTARIO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<string> InsertaActualizaLimiteInventarioMasivo(List<LimiteInvetario> limiteInvetarios, int idUsuario)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(List<LimiteInvetario>));
                var stringBuilder = new StringBuilder();
                using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                {
                    xmlSerializer.Serialize(xmlWriter, limiteInvetarios);
                }
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@xmlLimitesInventario", stringBuilder.ToString());    
                    parameters.Add("@idUsuario", idUsuario);               
                    notificacion = db.QuerySingle<Notificacion<string>>("SP_INSERTA_ACTUALIZA_LIMITES_INVENTARIO_MASIVO", parameters, commandType: CommandType.StoredProcedure);
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