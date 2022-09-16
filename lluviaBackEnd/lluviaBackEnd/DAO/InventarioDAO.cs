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
using System.Xml.Serialization;
using System.Text;
using System.Xml;

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
                    parameters.Add("@idUsuario", request.idUsuario);
                    
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

        #region  APP ADMINISTRACION DE LUQUIDOS
        public Notificacion<String> agregarLiquidosAInventario(RequestAgregarProductoInventarioLiquidos request)
        {
            Notificacion<String> notificacion = null;
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@cantidad", request.cantidad);
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@idAlmacen", request.idAlmacen);
                    notificacion = this.db.QuerySingle<Notificacion<String>>("SP_APP_AGREGAR_PRODUCTO_INVENTARIO_LIQUIDOS_ENVASADO", param: parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;

        }
        #endregion

        #region  APP ADMINISTRACION DE PRODUCCION AGRANAEL CONVERSION DE MPL A PRODUCTROS AGRANEL
        public Notificacion<String> agregarProductosProduccionAgranel(RequestAgregarProductoProduccionAgranel request)
        {
            Notificacion<String> notificacion = null;
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@cantidad", request.cantidad);
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@idAlmacen", request.idAlmacen);
                    notificacion = this.db.QuerySingle<Notificacion<String>>("SP_APP_INVENTARIO_AGREGAR_PRODUCTO_PRODUCCION_AGRANEL", param: parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;

        }

        public Notificacion<List<ResponseObtenerProductosProduccionAgranel>> obtenerProductosProduccionAgranel(RequestObtenerProductosProduccionAgranel request)
        {
            Notificacion <List<ResponseObtenerProductosProduccionAgranel>> notificacion = new Notificacion<List<ResponseObtenerProductosProduccionAgranel>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idAlmacen", request.idAlmacen);
                    parameters.Add("@idUsuario", request.idUsuario == 0 ? null : (object)request.idUsuario);
                    parameters.Add("@idEstatusProduccionAgranel", request.idEstatusProduccionAgranel == 0 ? null :(object)request.idEstatusProduccionAgranel);
                    parameters.Add("@fechaIni", request.fechaIni == DateTime.MinValue ? (object)null : request.fechaIni);
                    parameters.Add("@fechaFin", request.fechaFin == DateTime.MinValue ? (object)null : request.fechaFin);
                    var rs = this.db.QueryMultiple("SP_APP_OBTENER_PRODUCTOS_PRODUCCION_AGRANEL", param: parameters, commandType: CommandType.StoredProcedure);
                    
                    var rs1 = rs.ReadFirst();
                    if (rs1.Estatus == 200)
                    {
                        notificacion.Estatus = rs1.Estatus;
                        notificacion.Mensaje = rs1.Mensaje;
                        notificacion.Modelo = rs.Read<ResponseObtenerProductosProduccionAgranel, Producto, ResponseObtenerProductosProduccionAgranel>((responseObtenerPedidosInternos, producto ) =>
                        {
 
                            responseObtenerPedidosInternos.producto = producto;
                            return responseObtenerPedidosInternos;

                        }, splitOn: "idProducto").ToList();
                    }
                    else
                    {
                        notificacion.Estatus = rs1.Estatus;
                        notificacion.Mensaje = rs1.Mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;

        }

        public Notificacion<String> aprobarProductosProduccionAgranel(RequestAprobarProductosProduccionAgranel request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();

            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@xmlProductos", SerializeProductos(request.productos));
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@idAlmacen", request.idAlmacen);
                    notificacion = db.QuerySingle<Notificacion<String>>("SP_APP_APROBAR_PRODUCTOS_PRODCUCCION_AGRANEL", parameters, commandType: CommandType.StoredProcedure);
                    //var r  = db.QueryMultiple("SP_APP_ACTUALIZA_ESTATUS_PRODUCTO_COMPRA", parameters, commandType: CommandType.StoredProcedure);
                    //var r1 = r.ReadFirst();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public string SerializeProductos(List<ProductosProduccionAgranel> precios)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<ProductosProduccionAgranel>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                xmlSerializer.Serialize(xmlWriter, precios);
            }
            Console.WriteLine(stringBuilder.ToString());
            return stringBuilder.ToString();

        }
        #endregion
    }
}