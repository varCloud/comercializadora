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
                    var rs = db.QueryMultiple("SP_CONSULTA_PEDIDOS_INTERNOS", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        p.Estatus = rs1.status;
                        p.Mensaje = rs1.mensaje;
                        p.Modelo = rs.Read<PedidosInternos, Almacen, Almacen, Usuario, Status, Producto,PedidosInternos>(MapPedidosInternos, splitOn: "idAlmacenOrigen,idAlmacenDestino,idUsuario,idStatus,idProducto").ToList();
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

        public PedidosInternos MapPedidosInternos(PedidosInternos p,Almacen almacenOrigen,Almacen almacenDestino,Usuario usuario,Status status,Producto producto)
        {
            p.almacenOrigen = almacenOrigen;
            p.almacenDestino = almacenDestino;
            p.usuario = usuario;
            p.estatusPedido = status;
            p.producto = producto;
            return p;
        }

    }
}