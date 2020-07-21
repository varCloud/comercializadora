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
using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;

namespace lluviaBackEnd.DAO
{
    public class ComprasDAO
    {
        private IDbConnection db;
        public Notificacion<Compras> GuardarCompra(Compras compra)
        {
            Notificacion<Compras> notificacion = new Notificacion<Compras>();
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(List<Producto>));
                var stringBuilder = new StringBuilder();
                using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                {
                    xmlSerializer.Serialize(xmlWriter, compra.listProductos);
                }


                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idCompra", compra.idCompra);
                    parameters.Add("@idProveedor", compra.proveedor.idProveedor);
                    parameters.Add("@idUsuario ", compra.usuario.idUsuario);
                    parameters.Add("@idStatusCompra", compra.statusCompra.idStatus);
                    parameters.Add("@productos", stringBuilder.ToString());
                    parameters.Add("@observaciones", compra.observaciones);

                    var result = db.QueryMultiple("SP_REGISTRA_COMPRA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new Compras() { idCompra = r1.idCompra };
                        //notificacion.Modelo = precios; //result.ReadSingle<Producto>();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo = producto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<List<Status>> ObtenerStatusCompra()
        {
            Notificacion<List<Status>> notificacion = new Notificacion<List<Status>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_CONSULTA_ESTATUS_COMPRA", null, commandType: CommandType.StoredProcedure);
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

        public Notificacion<List<Compras>> ObtenerCompras(Compras compra, bool detalleCompra = false)
        {
            Notificacion<List<Compras>> compras = new Notificacion<List<Compras>>();
            int idLineaProducto = string.IsNullOrEmpty(compra.producto.idLineaProducto) ? 0 : Convert.ToInt32(compra.producto.idLineaProducto);
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idCompra", compra.idCompra == 0 ? (object)null : compra.idCompra);
                    parameters.Add("@idProveedor", compra.proveedor.idProveedor == 0 ? (object)null : compra.proveedor.idProveedor);
                    parameters.Add("@idStatusCompra", compra.statusCompra.idStatus == 0 ? (object)null : compra.statusCompra.idStatus);
                    parameters.Add("@idUsuario", compra.usuario.idUsuario == 0 ? (object)null : compra.usuario.idUsuario);
                    parameters.Add("@idAlmacen", compra.usuario.idAlmacen == 0 ? (object)null : compra.usuario.idAlmacen);
                    parameters.Add("@fechaInicio", compra.fechaIni == DateTime.MinValue ? (object)null : compra.fechaIni);
                    parameters.Add("@fechaFin", compra.fechaFin == DateTime.MinValue ? (object)null : compra.fechaFin);
                    parameters.Add("@idProducto", compra.producto.idProducto == 0 ? (object)null : compra.producto.idProducto);
                    parameters.Add("@descripcionProducto", string.IsNullOrEmpty(compra.producto.descripcion) ? (object)null : compra.producto.descripcion);
                    parameters.Add("@idLineaProducto", idLineaProducto == 0 ? (object)null : idLineaProducto);
                    parameters.Add("@detalleCompra", detalleCompra);

                    var rs = db.QueryMultiple("SP_CONSULTA_COMPRAS", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        compras.Estatus = rs1.status;
                        compras.Mensaje = rs1.mensaje;
                        compras.Modelo = rs.Read<Compras, Proveedor, Status, Usuario, Producto,EstatusProducto, Compras>(MapCompras, splitOn: "idProveedor,idStatus,idUsuario,idProducto,idEstatusProducto").ToList();

                    }
                    else
                    {
                        compras.Estatus = rs1.status;
                        compras.Mensaje = rs1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return compras;
        }

        private Compras MapCompras(Compras compras, Proveedor proveedor, Status status, Usuario usuario, Producto producto,EstatusProducto estatusProducto)
        {
            compras.proveedor = proveedor;
            compras.statusCompra = status;
            compras.usuario = usuario;
            compras.producto = producto;
            compras.producto.estatusProducto = estatusProducto;

            return compras;
        }

        public Notificacion<Compras> EliminaCompra(int idCompra)
        {
            Notificacion<Compras> notificacion = new Notificacion<Compras>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idCompra", idCompra);

                    var result = db.QueryMultiple("SP_ELIMINA_COMPRA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    notificacion.Estatus = r1.status;
                    notificacion.Mensaje = r1.mensaje;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<List<CompraDetalle>> ObtenerDetalleCompra(RequestObtenerDetalleCompra request)
        {
            Notificacion<List<CompraDetalle>> notificacion = new Notificacion<List<CompraDetalle>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idCompra", request.idCompra);
                    var result = db.QueryMultiple("SP_OBTENER_DETALLE_COMPRA", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<CompraDetalle, Producto,Status, Usuario,  CompraDetalle>((c, producto, status, usuario) =>
                        {
                            c.producto = producto;
                            c.status = status;
                            c.usuario = usuario;
                            return c;
                        }, splitOn: "idProducto,idEstatusProductoCompra,idUsuario").ToList();
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

    }
}