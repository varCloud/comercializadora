using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;

namespace lluviaBackEnd.DAO
{
    public class InventarioFisicoDAO
    {
        private IDbConnection db;

        public Notificacion<String> RegistraProductoInventarioFisico(RegistraProductoInventarioFisico request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idInventarioFisico", request.idInventarioFisico);
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@idUbicacion", request.idUbicacion);
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@cantidadEnFisico", request.cantidadEnFisico);
                    notificacion = db.QuerySingle<Notificacion<String>>("SP_APP_REGISTRA_PRODUCTO_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<String> ValidaInventarioFisico()
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    var rs = db.QueryMultiple("SP_APP_VALIDA_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = rs.ReadFirst();
                    notificacion.Mensaje = r1.Mensaje;
                    notificacion.Estatus = r1.Estatus;
                    notificacion.Modelo = r1.idInventarioFisico;
                                             
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<string> InsertaInventarioFisico(InventarioFisico inventarioFisico)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idInventarioFisico", inventarioFisico.idInventarioFisico);
                    parameters.Add("@nombre", inventarioFisico.Nombre);
                    parameters.Add("@idUsuario", inventarioFisico.Usuario.idUsuario);
                    parameters.Add("@activo", inventarioFisico.Activo);
                    notificacion = db.QuerySingle<Notificacion<string>>("SP_INSERTA_ACTUALIZA_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public List<InventarioFisico> ObtenerInventarioFisico()
        {
            List<InventarioFisico> inventarioFisicos = new List<InventarioFisico>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_CONSULTA_INVENTARIO_FISICO", null, commandType: CommandType.StoredProcedure);
                    var rs1 = result.ReadFirst();
                    if (rs1.status == 200)
                    {                        
                        inventarioFisicos = result.Read<InventarioFisico, Sucursal, Usuario,InventarioFisico>(MapInventarioFisico, splitOn: "idSucursal,idUsuario").ToList();
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return inventarioFisicos;
        }

        public InventarioFisico MapInventarioFisico(InventarioFisico i,Sucursal s,Usuario u)
        {
            i.Sucursal = s;
            i.Usuario = u;
            return i;
        } 
    }
}
