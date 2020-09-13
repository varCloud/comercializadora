using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;
using lluviaBackEnd.WebServices.Modelos.Response;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.DAO
{
    public class BitacoraDAO
    {
        private IDbConnection db;

        public Notificacion<List<Status>> ObtenerStatusPedidosInternos()
        {
            Notificacion<List<Status>> notificacion = new Notificacion<List<Status>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_CONSULTA_ESTATUS_PEDIDOS_INTERNOS", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Status>().ToList();
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

        public Notificacion<List<PedidosInternos>> ObtenerPedidosInternos(PedidosInternos pedidosInternos)
        {
            Notificacion<List<PedidosInternos>> p = new Notificacion<List<PedidosInternos>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@IdEstatusPedidoInterno", pedidosInternos.estatusPedido.idStatus == 0 ? (object)null : pedidosInternos.estatusPedido.idStatus);
                    parameters.Add("@idAlmancenOrigen", pedidosInternos.almacenOrigen.idAlmacen == 0 ? (object)null : pedidosInternos.almacenOrigen.idAlmacen);
                    parameters.Add("@idAlmacenDestino", pedidosInternos.almacenDestino.idAlmacen == 0 ? (object)null : pedidosInternos.almacenDestino.idAlmacen);
                    parameters.Add("@idUsuario", pedidosInternos.usuario.idUsuario == 0 ? (object)null : pedidosInternos.usuario.idUsuario);
                    parameters.Add("@idProducto", pedidosInternos.producto.idProducto == 0 ? (object)null : pedidosInternos.producto.idProducto);
                    parameters.Add("@fechaIni", pedidosInternos.fechaIni == DateTime.MinValue ? (object)null : pedidosInternos.fechaIni);
                    parameters.Add("@fechaFin", pedidosInternos.fechaFin == DateTime.MinValue ? (object)null : pedidosInternos.fechaFin);
                    parameters.Add("@idPedidoInterno", pedidosInternos.idPedidoInterno == 0 ? (object)null : pedidosInternos.idPedidoInterno);
                    var rs = db.QueryMultiple("SP_CONSULTA_PEDIDOS_INTERNOS", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        p.Estatus = rs1.status;
                        p.Mensaje = rs1.mensaje;
                        p.Modelo = rs.Read<PedidosInternos, Almacen, Almacen, Usuario, Status, Producto, PedidosInternos>(MapPedidosInternos, splitOn: "idAlmacenOrigen,idAlmacenDestino,idUsuario,idStatus,idProducto").ToList();
                    }
                    else
                    {
                        p.Estatus = rs1.status;
                        p.Mensaje = rs1.mensaje;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return p;
        }

        public Notificacion<List<PedidosInternos>> ObtenerDetallePedidosInternos(int idPedidoInterno)
        {
            Notificacion<List<PedidosInternos>> p = new Notificacion<List<PedidosInternos>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoInterno", idPedidoInterno == 0 ? (object)null : idPedidoInterno);
                    var rs = db.QueryMultiple("SP_CONSULTA_DETALLE_PEDIDOS_INTERNOS", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        p.Estatus = rs1.status;
                        p.Mensaje = rs1.mensaje;
                        p.Modelo = rs.Read<PedidosInternos, Almacen, Almacen, Usuario, Status, Producto, PedidosInternos>(MapPedidosInternos, splitOn: "idAlmacenOrigen,idAlmacenDestino,idUsuario,idStatus,idProducto").ToList();
                    }
                    else
                    {
                        p.Estatus = rs1.status;
                        p.Mensaje = rs1.mensaje;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return p;
        }
        public PedidosInternos MapPedidosInternos(PedidosInternos p, Almacen almacenOrigen, Almacen almacenDestino, Usuario usuario, Status status, Producto producto)
        {
            p.almacenOrigen = almacenOrigen;
            p.almacenDestino = almacenDestino;
            p.usuario = usuario;
            p.estatusPedido = status;
            p.producto = producto;
            return p;
        }

        #region SP APP MOVIL HAND HELD
        public Notificacion<String> GenerarPedidoInterno(RequestGenerarPedidoInterno request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", request.idProducto);
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@idAlamacenOrigen", request.idAlmacenOrigen);
                    parameters.Add("@idAlamacenDestino", request.idAlmacenDestino);
                    parameters.Add("@cantidad", request.cantidad);
                    notificacion = db.QuerySingle<Notificacion<String>>("SP_APP_GENERAR_PEDIDO_INTERNO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<String> ActualizarEstatusPedidoInterno(RequestActualizarEstatusPedidoInterno request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoInterno", request.idPedidoInterno);
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@idEstatusPedidoInterno", request.idEstatusPedidoInterno);
                    parameters.Add("@idAlmacenOrigen", request.idAlmacenOrigen);
                    parameters.Add("@idAlmacenDestino", request.idAlmacenDestino);
                    parameters.Add("@idUbcacion", request.idUbicacion);
                    parameters.Add("@observacion", request.observacion);
                    parameters.Add("@cantidadAtendida", request.cantidad);
                    notificacion = db.QuerySingle<Notificacion<String>>("SP_APP_ACTUALIZA_ESTATUS_PEDIDO_INTERNO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<String> AceptarPedidoInterno(RequestActualizarEstatusPedidoInterno request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoInterno", request.idPedidoInterno);
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@idAlmacenOrigen", request.idAlmacenOrigen);
                    parameters.Add("@idAlmacenDestino", request.idAlmacenDestino);
                    parameters.Add("@observacion", request.observacion);
                    parameters.Add("@cantidadAceptada", request.cantidad);
                    notificacion = db.QuerySingle<Notificacion<String>>("SP_APP_ACEPTA_PEDIDO_INTERNO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<String> RechazarPedidoInterno(RequestActualizarEstatusPedidoInterno request)
        {
            Notificacion<String> notificacion = new Notificacion<String>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoInterno", request.idPedidoInterno);
                    parameters.Add("@idUsuario", request.idUsuario);
                    parameters.Add("@idAlmacenOrigen", request.idAlmacenOrigen);
                    parameters.Add("@idAlmacenDestino", request.idAlmacenDestino);
                    parameters.Add("@observacion", request.observacion);
                    notificacion = db.QuerySingle<Notificacion<String>>("SP_APP_RECHAZA_PEDIDO_INTERNO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }


        //public Notificacion<List<ResponseObtenerPedidosInternos>> ObtenerPedidosInternosApp(RequestObtenerPedidosInternos request)
        //{
        //    Notificacion<List<ResponseObtenerPedidosInternos>> lst = new Notificacion<List<ResponseObtenerPedidosInternos>>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idEstatusPedido", request.idEstatusPedido == 0 ? (object)null : request.idEstatusPedido);
        //            parameters.Add("@idAlmacenOrigen", request.idAlmacenOrigen == 0 ? (object)null : request.idAlmacenOrigen);
        //            parameters.Add("@idAlmacenDestino", request.idAlmacenDestino == 0 ? (object)null : request.idAlmacenDestino);
        //            parameters.Add("@idUsuario", request.idUsuario == 0 ? (object)null : request.idUsuario);
        //            parameters.Add("@fechaInicio", request.fechaInicio == DateTime.MinValue ? (object)null : request.fechaInicio);
        //            parameters.Add("@fechaFin", request.fechaFin == DateTime.MinValue ? (object)null : request.fechaFin);
        //            parameters.Add("@idPedidoInterno", request.idPedidoInterno == 0 ? (object)null : request.idPedidoInterno);
        //            var rs = db.QueryMultiple("SP_APP_OBTENER_PEDIDOS_INTERNOS", parameters, commandType: CommandType.StoredProcedure);
        //            var rs1 = rs.ReadFirst();
        //            if (rs1.Estatus == 200)
        //            {
        //                lst.Estatus = rs1.Estatus;
        //                lst.Mensaje = rs1.Mensaje;
        //                lst.Modelo = rs.Read<ResponseObtenerPedidosInternos, Producto, Almacen, Almacen, ResponseObtenerPedidosInternos>((responseObtenerPedidosInternos, producto, almacenO, almacenD) =>
        //                {
        //                    responseObtenerPedidosInternos.almacenOrigen = almacenO;
        //                    responseObtenerPedidosInternos.almacenDestino = almacenD;
        //                    responseObtenerPedidosInternos.producto = producto;
        //                    return responseObtenerPedidosInternos;

        //                }, splitOn: "idProducto , idAlmacenOrigen,idAlmacenDestino").ToList();
        //            }
        //            else
        //            {
        //                lst.Estatus = rs1.Estatus;
        //                lst.Mensaje = rs1.Mensaje;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return lst;
        //}

        public Notificacion<List<ResponseObtenerPedidosInternos>> ObtenerPedidosInternosUsuarioApp(RequestObtenerPedidosInternosUsuario request)
        {
            Notificacion<List<ResponseObtenerPedidosInternos>> lst = new Notificacion<List<ResponseObtenerPedidosInternos>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", request.idUsuario == 0 ? (object)null : request.idUsuario);
                    parameters.Add("@idEstatusPedido", request.idEstatusPedido == 0 ? (object)null : request.idEstatusPedido);
                    parameters.Add("@idAlmacenDestino", request.idAlmacenDestino == 0 ? (object)null : request.idAlmacenDestino);
                    parameters.Add("@fechaInicio", request.fechaInicio == DateTime.MinValue ? (object)null : request.fechaInicio);
                    parameters.Add("@fechaFin", request.fechaFin == DateTime.MinValue ? (object)null : request.fechaFin);
                    parameters.Add("@idPedidoInterno", request.idPedidoInterno == 0 ? (object)null : request.idPedidoInterno);
                    parameters.Add("@idTipoPedidoInterno", request.idTipoPedidoInterno == 0 ? 1 : request.idTipoPedidoInterno);
                    var rs = db.QueryMultiple("SP_APP_OBTENER_PEDIDOS_INTERNOS_X_USUARIO", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.Estatus == 200)
                    {
                        lst.Estatus = rs1.Estatus;
                        lst.Mensaje = rs1.Mensaje;
                        lst.Modelo = rs.Read<ResponseObtenerPedidosInternos, Producto, Almacen, Almacen, ResponseObtenerPedidosInternos>((responseObtenerPedidosInternos, producto, almacenO, almacenD) =>
                        {
                            responseObtenerPedidosInternos.almacenOrigen = almacenO;
                            responseObtenerPedidosInternos.almacenDestino = almacenD;
                            responseObtenerPedidosInternos.producto = producto;
                            return responseObtenerPedidosInternos;

                        }, splitOn: "idProducto , idAlmacenOrigen,idAlmacenDestino").ToList();
                    }
                    else
                    {
                        lst.Estatus = rs1.Estatus;
                        lst.Mensaje = rs1.Mensaje;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;
        }

        public Notificacion<List<ResponseObtenerPedidosInternos>> ObtenerPedidosInternosAlmacenApp(RequestObtenerPedidosInternosAlamcen request)
        {
            Notificacion<List<ResponseObtenerPedidosInternos>> lst = new Notificacion<List<ResponseObtenerPedidosInternos>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idEstatusPedido", request.idEstatusPedido == 0 ? (object)null : request.idEstatusPedido);
                    parameters.Add("@idAlmacenDestino", request.idAlmacenDestino == 0 ? (object)null : request.idAlmacenDestino);
                    parameters.Add("@fechaInicio", request.fechaInicio == DateTime.MinValue ? (object)null : request.fechaInicio);
                    parameters.Add("@fechaFin", request.fechaFin == DateTime.MinValue ? (object)null : request.fechaFin);
                    parameters.Add("@idPedidoInterno", request.idPedidoInterno == 0 ? (object)null : request.idPedidoInterno);
                    parameters.Add("@idTipoPedidoInterno", request.idTipoPedidoInterno == 0 ? 1 : request.idTipoPedidoInterno);
                    
                    var rs = db.QueryMultiple("SP_APP_OBTENER_PEDIDOS_INTERNOS_X_ALMACEN", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.Estatus == 200)
                    {
                        lst.Estatus = rs1.Estatus;
                        lst.Mensaje = rs1.Mensaje;
                        lst.Modelo = rs.Read<ResponseObtenerPedidosInternos, Producto, Almacen, Almacen, ResponseObtenerPedidosInternos>((responseObtenerPedidosInternos, producto, almacenO, almacenD) =>
                        {
                            responseObtenerPedidosInternos.almacenOrigen = almacenO;
                            responseObtenerPedidosInternos.almacenDestino = almacenD;
                            responseObtenerPedidosInternos.producto = producto;
                            return responseObtenerPedidosInternos;

                        }, splitOn: "idProducto , idAlmacenOrigen,idAlmacenDestino").ToList();
                    }
                    else
                    {
                        lst.Estatus = rs1.Estatus;
                        lst.Mensaje = rs1.Mensaje;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;
        }

        public Notificacion<List<DetallePedidoInterno>> ObtenerDetallePedidoInterno(RequestObtenerDetallePedidoInterno request)
        {
            Notificacion<List<DetallePedidoInterno>> lst = new Notificacion<List<DetallePedidoInterno>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoInterno", request.idPedidoInterno == 0 ? (object)null : request.idPedidoInterno);;

                    var rs = db.QueryMultiple("SP_APP_OBTENER_DETALLE_PEDIDOS_INTERNOS", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.Estatus == 200)
                    {
                        lst.Estatus = rs1.Estatus;
                        lst.Mensaje = rs1.Mensaje;
                        lst.Modelo = rs.Read<DetallePedidoInterno>().ToList();
                    }
                    else
                    {
                        lst.Estatus = rs1.Estatus;
                        lst.Mensaje = rs1.Mensaje;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lst;
        }


        #endregion
    }
}