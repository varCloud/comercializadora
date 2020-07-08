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
using lluviaBackEnd.WebServices.Modelos.Response;

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
                    parameters.Add("@idAlmacen", request.idAlmacen);
                    //parameters.Add("@idTipoMovInventario", request.idTipoMovInventario);
                    notificacion = this.db.QuerySingle<Notificacion<String>>("SP_APP_AGREGAR_PRODUCTO_INVENTARIO", param: parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<List<Ubicacion>> ObtenerUbicacionProductoInventario(RequestObtenerUbicacionProductoInventario request)
        {
            Notificacion<List<Ubicacion>> notificacion = new Notificacion<List<Ubicacion>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idAlmacen", (request.idAlmacen == 0 ? (object)null : request.idAlmacen));
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@EstatusProducto", request.EstatusProducto);
                    var result = this.db.QueryMultiple("SP_APP_OBTENER_UBICACION_PRODUCTO_INVENTARIO", param: parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.Estatus == 200)
                    {
                        notificacion.Modelo = result.Read<Ubicacion>().ToList();
                        notificacion.Estatus = 200;
                        notificacion.Mensaje = "OK";
                    }
                    else
                    {
                        notificacion.Estatus = -1;
                        notificacion.Mensaje = "Espere un momento y vuelva a ejecutarlo";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }
        
        public Notificacion<ResponseObtenerPisoPasilloRaq> ObtenerPasilloRaqPiso()
        {
            Notificacion<ResponseObtenerPisoPasilloRaq> notificacion = new Notificacion<ResponseObtenerPisoPasilloRaq>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    var result = this.db.QueryMultiple("SP_APP_OBTENER_PASILLO_RAQ_PISO", commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.Estatus == 200)
                    {
                        notificacion.Modelo = new ResponseObtenerPisoPasilloRaq();
                        notificacion.Modelo.pasillos = result.Read<Pasillo>().ToList();
                        notificacion.Modelo.raqs = result.Read<Raq>().ToList();
                        notificacion.Modelo.pisos = result.Read<Piso>().ToList();
                    }
                    


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<String> ActualizarUbicacionInventario(RequestActualizarUbicacionInventario request)
        {
            Notificacion<String> notificacion = null;
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUbicacion", request.IdUbicacion);
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@cantidad", request.cantidad);
                    parameters.Add("@idPasillo", request.idPasillo);
                    parameters.Add("@idRaq", request.idRack);
                    parameters.Add("@idPiso", request.idPiso);
                    parameters.Add("@idAlamacen", request.idAlmacen);
                    parameters.Add("@idUsuario", request.idUsuario);
                    notificacion = this.db.QuerySingle<Notificacion<String>>("SP_APP_ACTUALIZAR_UBICACION_INVENTARIO", param: parameters, commandType: CommandType.StoredProcedure , commandTimeout:90);
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