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

        public Notificacion<String> ValidaInventarioFisico(RequesValidaInventarioFisico request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", request.idUsuario);
                    var rs = db.QueryMultiple("SP_APP_VALIDA_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = rs.ReadFirst();
                    notificacion.Mensaje = r1.Mensaje;
                    notificacion.Estatus = r1.Estatus;
                    notificacion.Modelo = r1.idInventarioFisico+"";
                                             
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
                    notificacion = db.QuerySingle<Notificacion<string>>("SP_INSERTA_ACTUALIZA_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<string> ActualizaEstatusInventarioFisico(InventarioFisico inventarioFisico)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idInventarioFisico", inventarioFisico.idInventarioFisico);
                    parameters.Add("@idEstatusInventarioFisico", inventarioFisico.EstatusInventarioFisico.idStatus);
                    parameters.Add("@idUsuario", inventarioFisico.Usuario.idUsuario);
                    parameters.Add("@observaciones", inventarioFisico.Observaciones);
                    notificacion = db.QuerySingle<Notificacion<string>>("SP_ACTUALIZA_ESTATUS_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }
        
        public List<InventarioFisico> ObtenerInventarioFisico(int idSucursal,int idInventarioFisico,int idEstatus)
        {
            List<InventarioFisico> inventarioFisicos = new List<InventarioFisico>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@idSucursal", idSucursal==0 ? (object)null : idSucursal);
                    parameters.Add("@idInventarioFisico", idInventarioFisico == 0 ? (object)null : idInventarioFisico);
                    parameters.Add("@idEstatus", idEstatus == 0 ? (object)null : idEstatus);
                    var result = db.QueryMultiple("SP_CONSULTA_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = result.ReadFirst();
                    if (rs1.status == 200)
                    {                        
                        inventarioFisicos = result.Read<InventarioFisico, Sucursal, Usuario,Status,InventarioFisico>(MapInventarioFisico, splitOn: "idSucursal,idUsuario,idStatus").ToList();
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return inventarioFisicos;
        }

        public InventarioFisico MapInventarioFisico(InventarioFisico i,Sucursal s,Usuario u, Status status)
        {
            i.Sucursal = s;
            i.Usuario = u;
            i.EstatusInventarioFisico = status;
            return i;
        }

        public Notificacion<String> ValidaExisteInventarioFisicoActivo(int idUsuario)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", idUsuario == 0 ? (object)DBNull.Value : idUsuario);
                    var rs = db.QueryMultiple("SP_VALIDA_EXISTE_INVENTARIO_FISICO_ACTIVO", parameters, commandType: CommandType.StoredProcedure);
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

        public List<AjusteInventarioFisico> ObtenerAjusteInventario(AjusteInventarioFisico a)
        {
            List<AjusteInventarioFisico> ajusteInventarios = new List<AjusteInventarioFisico>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@idInventarioFisico", a.idInventarioFisico == 0 ? (object)null : a.idInventarioFisico);
                    parameters.Add("@idProducto", a.producto.idProducto == 0 ? (object)null : a.producto.idProducto);
                    parameters.Add("@idLineaProducto", string.IsNullOrEmpty(a.producto.idLineaProducto)? (object)null : a.producto.idLineaProducto);
                    parameters.Add("@idAlmacen", a.producto.idAlmacen == 0 ? (object)null : a.producto.idAlmacen);

                    var result = db.QueryMultiple("SP_CONSULTA_AJUSTE_INVENTARIO", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = result.ReadFirst();
                    if (rs1.status == 200)
                    {
                        ajusteInventarios = result.Read<AjusteInventarioFisico, Producto, Usuario, AjusteInventarioFisico>(MapAjusteInventarioFisico, splitOn: "idProducto,idUsuario").ToList();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ajusteInventarios;
        }

        public AjusteInventarioFisico MapAjusteInventarioFisico(AjusteInventarioFisico i, Producto p, Usuario u)
        {           
            i.producto = p;
            i.usuario = u;
          
            return i;
        }

        public Notificacion<List<Ubicacion>> ObtenerProductosAjutadoPorInventarioFisico(RequestObtenerProductosAjustados request)
        {
            Notificacion<List<Ubicacion>> notificacion = new Notificacion<List<Ubicacion>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idAlmacen", (request.idAlmacen == 0 ? (object)null : request.idAlmacen));
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@idInventarioFisico", request.idInventarioFisico);
                    var result = this.db.QueryMultiple("SP_APP_OBTENER_PRODUCTO_AJUSTADO_POR_INVENTARIO_FISICO", param: parameters, commandType: CommandType.StoredProcedure);
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

    }
}
