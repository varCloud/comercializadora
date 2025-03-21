﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccesoDatos;
using lluviaBackEnd.Models;
using lluviaBackEnd;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace lluviaBackEnd.DAO
{
    public class PedidosEspecialesV2DAO
    {
        private IDbConnection db = null;


        public Notificacion<PedidosEspecialesV2> GuardarPedidoEspecial(List<Producto> productos, int tipoRevision, int idCliente, int idUsuario, int idEstatusPedidoEspecial, int idEstacion, int idPedidoEspecial, int idPedidoEspecialMayoreo_)
        {
            Notificacion<PedidosEspecialesV2> notificacion = new Notificacion<PedidosEspecialesV2>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@XML", Serialize(productos));
                    parameters.Add("@tipoRevision", tipoRevision);
                    parameters.Add("@idCliente", idCliente);
                    parameters.Add("@idUsuario", idUsuario);
                    parameters.Add("@idEstatusPedidoEspecial", idEstatusPedidoEspecial);
                    parameters.Add("@idEstacion", idEstacion);
                    parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                    parameters.Add("@idPedidoEspecialMayoreo_", idPedidoEspecialMayoreo_);

                    var result = db.QueryMultiple("SP_GUARDA_PEDIDO_ESPECIAL_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new PedidosEspecialesV2() { idPedidoEspecial = r1.idPedidoEspecial };
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


        public Notificacion<PedidosEspecialesV2> CancelarPedidoEspecial(int idPedidoEspecial)
        {
            Notificacion<PedidosEspecialesV2> notificacion = new Notificacion<PedidosEspecialesV2>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                    var result = db.QueryMultiple("SP_CANCELAR_PEDIDO_ESPECIAL_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo = new PedidosEspecialesV2() { idPedidoEspecial = r1.idPedidoEspecial };
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


        public Notificacion<List<Producto>> ConsultaExistenciasAlmacen(Producto producto)
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@idAlmacen", producto.idAlmacen);

                    var result = db.QueryMultiple("SP_CONSULTA_EXISTENCIA_PRODUCTO_ALMACEN", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Producto>().ToList();
                        //notificacion.Modelo = result.ReadSingle<List<Producto>>();
                        //notificacion.Modelo = new Producto() { idPedidoEspecial = r1.idPedidoEspecial };
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


        public Notificacion<List<PedidosEspecialesV2>> ObtenerEntregarPedidos(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoEspecial", pedidosEspecialesV2.idPedidoEspecial == 0 ? (object)null : pedidosEspecialesV2.idPedidoEspecial);
                    var result = db.QueryMultiple("SP_CONSULTA_PEDIDOS_ESPECIALES_ID_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<PedidosEspecialesV2>().ToList();
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


        public Notificacion<List<Precio>> ObtenerPreciosDeProductos(List<Precio> precios)
        {
            Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@PRODUCTOS", SerializePrecios(precios));
                    var result = db.QueryMultiple("SP_CONSULTA_PRECIO_X_VOLUMEN", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Precio>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = precios;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<List<FormaPago>> ObtenerFormasPago()
        {
            Notificacion<List<FormaPago>> notificacion = new Notificacion<List<FormaPago>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    var result = db.QueryMultiple("SP_CONSULTA_FORMA_PAGO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<FormaPago>().ToList();
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


        public Notificacion<List<UsoCFDI>> ObtenerUsoCFDI()
        {
            Notificacion<List<UsoCFDI>> notificacion = new Notificacion<List<UsoCFDI>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    var result = db.QueryMultiple("SP_CONSULTA_USO_CFDI", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<UsoCFDI>().ToList();
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

        public string Serialize(List<Producto> productos)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Producto>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                xmlSerializer.Serialize(xmlWriter, productos);
            }
            Debug.WriteLine(stringBuilder.ToString());
            return stringBuilder.ToString();

        }

        public string SerializePrecios(List<Precio> precios)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Precio>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
            {
                xmlSerializer.Serialize(xmlWriter, precios);
            }

            return stringBuilder.ToString();

        }



        public List<Producto> ConsultaPedidoEspecialDetalle(int idPedidoEspecial)
        {
            List<Producto> lstProductosPedido = new List<Producto>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                    var result = db.QueryMultiple("SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        //notificacion.Estatus = r1.status;
                        //notificacion.Mensaje = r1.mensaje;
                        lstProductosPedido = result.Read<Producto>().ToList();
                    }
                    else
                    {
                        //notificacion.Estatus = r1.status;
                        //notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstProductosPedido;
        }


        //public Notificacion<List<Ticket>> ObtenerTicketsPedidoEspecialV2(Ticket ticket)
        //{
        //    Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idPedidoEspecial", ticket.idPedidoEspecial);
        //            var result = db.QueryMultiple("SP_CONSULTA_TICKET_PEDIDO_ESPECIALV2", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = result.Read<Ticket>().ToList();
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = new List<Ticket> { ticket };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}


        public Notificacion<PedidosEspecialesV2> GuardarIVAPedido(PedidosEspecialesV2 pedido)
        {
            Notificacion<PedidosEspecialesV2> notificacion = new Notificacion<PedidosEspecialesV2>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoEspecial", pedido.idPedidoEspecial);
                    parameters.Add("@idCliente", pedido.idCliente);
                    parameters.Add("@idFactFormaPago", pedido.idFactFormaPago);
                    parameters.Add("@idFactUsoCFDI", pedido.idFactUsoCFDI);
                    var result = db.QueryMultiple("SP_GUARDA_IVA_PEDIDO_ESPECIAL_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.ReadSingle<PedidosEspecialesV2>(); ;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = pedido;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<List<PedidosEspecialesV2>> ConsultaPedidosEspeciales(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoEspecial", pedidosEspecialesV2.idPedidoEspecial);
                    parameters.Add("@fechaIni", pedidosEspecialesV2.fechaIni == DateTime.MinValue ? (object)null : pedidosEspecialesV2.fechaIni);
                    parameters.Add("@fechaFin", pedidosEspecialesV2.fechaFin == DateTime.MinValue ? (object)null : pedidosEspecialesV2.fechaFin);
                    var result = db.QueryMultiple("SP_CONSULTA_PEDIDOS_ESPECIALES_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<PedidosEspecialesV2>().ToList();
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


        public Notificacion<List<PedidosEspecialesV2>> ConsultaPedidosEnRuta(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuarioRuteo", pedidosEspecialesV2.idUsuarioRuteo);
                    parameters.Add("@fechaIni", pedidosEspecialesV2.fechaIni == DateTime.MinValue ? (object)null : pedidosEspecialesV2.fechaIni);
                    parameters.Add("@fechaFin", pedidosEspecialesV2.fechaFin == DateTime.MinValue ? (object)null : pedidosEspecialesV2.fechaFin);
                    var result = db.QueryMultiple("SP_CONSULTA_PEDIDOS_EN_RUTA_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<PedidosEspecialesV2>().ToList();
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

        //public Notificacion<PedidosEspeciales> AceptarRechazarPedidoEspecial(PedidosEspeciales pedido)
        //{
        //    Notificacion<PedidosEspeciales> notificacion = new Notificacion<PedidosEspeciales>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@XML", Serialize(pedido.lstPedidosInternosDetalle));
        //            parameters.Add("@idPedidoEspecial", pedido.idPedidoEspecial);
        //            parameters.Add("@idUsuario", pedido.idUsuario);

        //            var result = db.QueryMultiple("SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = pedido; 
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}



        public Notificacion<PedidosEspeciales> CancelaPedidoEspecial(PedidosEspeciales pedidoEspecial)
        {
            Notificacion<PedidosEspeciales> notificacion = new Notificacion<PedidosEspeciales>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoEspecial", pedidoEspecial.idPedidoEspecial);
                    var result = db.QueryMultiple("SP_ELIMINA_VENTA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = pedidoEspecial;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = pedidoEspecial;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<PedidosEspeciales> ConsultaPedidoEspecial(PedidosEspeciales pedidoEspecial)
        {
            Notificacion<PedidosEspeciales> notificacion = new Notificacion<PedidosEspeciales>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta", pedidoEspecial.idPedidoEspecial);
                    var result = db.QueryMultiple("SP_CONSULTA_VENTA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.ReadSingle<PedidosEspeciales>(); ;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = pedidoEspecial;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<List<PedidosEspeciales>> ObtenerPedidosEspeciales(PedidosEspeciales pedidosEspeciales)
        {
            Notificacion<List<PedidosEspeciales>> p = new Notificacion<List<PedidosEspeciales>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@IdEstatusPedidoInterno", pedidosEspeciales.estatusPedido.idStatus == 0 ? (object)null : pedidosEspeciales.estatusPedido.idStatus);
                    parameters.Add("@idAlmancenOrigen", pedidosEspeciales.almacenOrigen.idAlmacen == 0 ? (object)null : pedidosEspeciales.almacenOrigen.idAlmacen);
                    parameters.Add("@idAlmacenDestino", pedidosEspeciales.almacenDestino.idAlmacen == 0 ? (object)null : pedidosEspeciales.almacenDestino.idAlmacen);
                    parameters.Add("@idUsuario", pedidosEspeciales.usuario.idUsuario == 0 ? (object)null : pedidosEspeciales.usuario.idUsuario);
                    parameters.Add("@fechaIni", pedidosEspeciales.fechaIni == DateTime.MinValue ? (object)null : pedidosEspeciales.fechaIni);
                    parameters.Add("@fechaFin", pedidosEspeciales.fechaFin == DateTime.MinValue ? (object)null : pedidosEspeciales.fechaFin);
                    parameters.Add("@idPedidoInterno", pedidosEspeciales.idPedidoEspecial == 0 ? (object)null : pedidosEspeciales.idPedidoEspecial);
                    parameters.Add("@idPedidoInterno", pedidosEspeciales.idPedidoEspecial == 0 ? (object)null : pedidosEspeciales.idPedidoEspecial);
                    parameters.Add("@descripcion", pedidosEspeciales.descripcion == "" ? (object)null : pedidosEspeciales.descripcion);
                    parameters.Add("@idTipoPedidoInterno", 2);
                    var rs = db.QueryMultiple("SP_CONSULTA_PEDIDOS_ESPECIALES", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        p.Estatus = rs1.status;
                        p.Mensaje = rs1.mensaje;
                        p.Modelo = rs.Read<PedidosEspeciales, Almacen, Almacen, Usuario, Status, PedidosEspeciales>(MapPedidosEspeciales, splitOn: "idAlmacenOrigen,idAlmacenDestino,idUsuario,idStatus").ToList();
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


        public PedidosEspeciales MapPedidosEspeciales(PedidosEspeciales p, Almacen almacenOrigen, Almacen almacenDestino, Usuario usuario, Status status)
        {
            p.almacenOrigen = almacenOrigen;
            p.almacenDestino = almacenDestino;
            p.usuario = usuario;
            p.estatusPedido = status;
            //p.producto = producto;
            return p;
        }



        public List<PedidosInternosDetalle> ObtenerProductosPedidoEspecial(int idPedidoEspecial)
        {
            List<PedidosInternosDetalle> p = new List<PedidosInternosDetalle>();

            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                    var rs = db.QueryMultiple("SP_CONSULTA_PEDIDOS_INTERNOS_DETALLE_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        p = rs.Read<PedidosInternosDetalle>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return p;
        }


        public Notificacion<List<PedidosEspeciales>> ObtenerDetallePedidosEspeciales(int idPedidoInterno)
        {
            Notificacion<List<PedidosEspeciales>> p = new Notificacion<List<PedidosEspeciales>>();
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
                        p.Modelo = rs.Read<PedidosEspeciales, Almacen, Almacen, Usuario, Status, PedidosEspeciales>(MapPedidosEspeciales, splitOn: "idAlmacenOrigen,idAlmacenDestino,idUsuario,idStatus").ToList();
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

        public Notificacion<dynamic> ObtenerCotizaciones()
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_COTIZACIONES_PEDIDOS_ESPECIALES", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerClientesPedidosEspeciales()
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_CLIENTES_PEDIDOS_ESPECIALES", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerUsuariosPedidosEspeciales()
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_USUARIOS_PEDIDOS_ESPECIALES", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerEstatusPedidosEspeciales()
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_ESTATUS_PEDIDOS_ESPECIALES", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }


        public Notificacion<dynamic> ObtenerPedidosEspeciales(Filtro filtro)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idCliente", filtro.idCliente == 0 ? (object)null : filtro.idCliente);
                parameters.Add("@idUsuario", filtro.idUsuario == 0 ? (object)null : filtro.idUsuario);
                parameters.Add("@idEstatusPedidoEspecial", filtro.idEstatusPedidoEspecial == 0 ? (object)null : filtro.idEstatusPedidoEspecial);
                parameters.Add("@fechaIni", filtro.fechaIni == DateTime.MinValue ? (object)null : filtro.fechaIni);
                parameters.Add("@fechaFin", filtro.fechaFin == DateTime.MinValue ? (object)null : filtro.fechaFin);
                parameters.Add("@codigoBarras", String.IsNullOrEmpty(filtro.codigoBarras) ? (object)null : filtro.codigoBarras);
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerPedidosEspecialesDetalle(Int64 idPedidoEspecial)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_V2", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerPedidosEspecialesDetalleBitacora(Int64 idPedidoEspecial)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_PEDIDOS_ESPECIALES_DETALLE_BITACORA_V2", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<PedidosEspecialesV2> GuardarConfirmacion(List<Producto> productos, int idPedidoEspecial, int idEstatusPedidoEspecial, int idUsuarioEntrega, string numeroUnidadTaxi, 
                                                                     int idEstatusCuentaPorCobrar, float montoPagado, bool aCredito, bool aCreditoConAbono, int aplicaIVA, int idFactFormaPago, 
                                                                     int idFactUsoCFDI, string observacionesPedidoRuta, int idUsuarioRuteo, Boolean esPedidoEnRuta, int idUsuarioLiquida)
        {
            Notificacion<PedidosEspecialesV2> notificacion = new Notificacion<PedidosEspecialesV2>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@listaProductos", Serialize(productos));
                    parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                    parameters.Add("@idEstatusPedidoEspecial", idEstatusPedidoEspecial);
                    parameters.Add("@idUsuarioEntrega", idUsuarioEntrega);
                    parameters.Add("@numeroUnidadTaxi", numeroUnidadTaxi);
                    parameters.Add("@idEstatusCuentaPorCobrar", idEstatusCuentaPorCobrar);
                    parameters.Add("@montoPagado", montoPagado);
                    parameters.Add("@aCredito", aCredito);
                    parameters.Add("@aCreditoConAbono", aCreditoConAbono);
                    parameters.Add("@aplicaIVA", aplicaIVA);
                    //parameters.Add("@idMetodoPago", idMetodoPago);
                    parameters.Add("@idFactFormaPago", idFactFormaPago);
                    parameters.Add("@idFactUsoCFDI", idFactUsoCFDI);
                    parameters.Add("@observacionesPedidoRuta", observacionesPedidoRuta);
                    parameters.Add("@idUsuarioRuteo", idUsuarioRuteo);
                    parameters.Add("@esPedidoEnRuta", esPedidoEnRuta);
                    parameters.Add("@idUsuarioLiquida", idUsuarioLiquida);


                    var result = db.QueryMultiple("SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
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

        public Notificacion<List<dynamic>> consultaTicketPedidoEspecial(Int64 idPedidoEspecial)
        {
            Notificacion<List<dynamic>> notificacion = new Notificacion<List<dynamic>>();
            notificacion.Modelo = new List<dynamic>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                    var result = db.QueryMultiple("SP_CONSULTA_TICKET_PEDIDOS_ESPECIALES_V2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        while (!result.IsConsumed)
                        {
                            var r2 = result.Read<dynamic>();
                            notificacion.Modelo.Add(r2);
                        }
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


        #region CuentasPorCobrar
        public Notificacion<dynamic> ObtenerClientesCuentasXCobrar()
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_CLIENTES_CUENTAS_X_COBRAR", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerCuentasXCobrar(Filtro filtro)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idCliente", filtro.idCliente == 0 ? (object)null : filtro.idCliente);
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerCuentasXCobrarDetalle(Int64 idCliente)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idCliente", idCliente);
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_DETALLE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<string> RealizarAbonoPedidoEspecial(AbonoCliente abono)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idCliente", abono.idCliente);
                parameters.Add("@idUsuario", abono.idUsuario);
                parameters.Add("@monto", abono.montoAbono);
                parameters.Add("@montoIVA", abono.montoIVA);
                parameters.Add("@montoComision", abono.montoComision);
                parameters.Add("@requiereFactura", abono.requiereFactura);
                parameters.Add("@idFactFormaPago", abono.idFactFormaPago);
                parameters.Add("@idFactUsoCFDI", abono.idFactUsoCFDI);
                parameters.Add("@idPedidoEspecial", abono.idPedidoEspecial==0 ? (object)null :abono.idPedidoEspecial);
                parameters.Add("@montoRecibido", abono.montoRecibido);
                notificacion = ConstructorDapper.Ejecutar("SP_REALIZA_ABONO_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerBalanceCuentasXCobrar(Int64 idCliente)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idCliente", idCliente);
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_BALCANCE_CUENTAS_X_COBRAR_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtieneAbonosCliente(Int64 idAbonoCliente, Int64 idCliente)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idAbonoCliente", idAbonoCliente==0?(object)null:idAbonoCliente);
                parameters.Add("@idCliente", idCliente==0?(object)null:idCliente);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_ABONOS_CLIENTE_PEDIDO_ESPECIAL", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }



        #endregion

        #region IngresoEfectivo

        public Notificacion<string> ValidaAperturaCajas(int idUsuario)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", idUsuario);              
                notificacion = ConstructorDapper.Ejecutar("SP_VALIDA_APERTURA_CAJAS_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<string> IngresoEfectivo(int idUsuario, float monto, int idTipoIngreso)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", idUsuario);
                parameters.Add("@monto", monto);
                parameters.Add("@idTipoIngreso", idTipoIngreso);
                notificacion = ConstructorDapper.Ejecutar("SP_INGRESO_EFECTIVO_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<dynamic> ObtenerIngresosEfectivo(IngresoEfectivo ingresoEfectivo)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idTipoIngreso", ingresoEfectivo.idTipoIngreso == 0 ? (object)null : ingresoEfectivo.idTipoIngreso);
                parameters.Add("@fecha", ingresoEfectivo.fechaAlta == DateTime.MinValue ? (object)null : ingresoEfectivo.fechaAlta);
                parameters.Add("@idIngresoPedidoEspecial", ingresoEfectivo.idIngreso == 0 ? (object)null : ingresoEfectivo.idIngreso);
                parameters.Add("@idUsuario", ingresoEfectivo.idUsuario == 0 ? (object)null : ingresoEfectivo.idUsuario);
                parameters.Add("@idAlmacen", ingresoEfectivo.idAlmacen == 0 ? (object)null : ingresoEfectivo.idAlmacen);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_INGRESOS_EFECTIVO_PEDIDOS_ESPECIALES", parameters);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        #endregion

        #region RetiroExcesoEfectivo
        public Notificacion<dynamic> ObtenerRetirosEfectivo(Retiros retiros)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idEstacion", retiros.idEstacion == 0 ? (object)null : retiros.idEstacion);
                parameters.Add("@fecha", retiros.fechaAlta == DateTime.MinValue ? (object)null : retiros.fechaAlta);
                parameters.Add("@idRetiro", retiros.idRetiro == 0 ? (object)null : retiros.idRetiro);
                parameters.Add("@idUsuario", retiros.idUsuario == 0 ? (object)null : retiros.idUsuario);
                parameters.Add("@idAlmacen", retiros.idAlmacen == 0 ? (object)null : retiros.idAlmacen);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_RETIROS_EFECTIVO_PEDIDOS_ESPECIALES", parameters);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<dynamic> ObtieneInfoCierre(Cierre cierre)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idEstacion", cierre.idEstacion);
                parameters.Add("@idUsuario", cierre.idUsuario);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_INFO_CIERRE_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<string> RetirarExcesoEfectivo(Retiros retiros)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idEstacion", retiros.idEstacion);
                parameters.Add("@idUsuario", retiros.idUsuario);
                parameters.Add("@monto", retiros.montoRetiro);
                parameters.Add("@caso", 1);
                notificacion = ConstructorDapper.Ejecutar("SP_RETIRA_EFECTIVO_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        #endregion

        #region Devoluciones

        public Notificacion<string> RealizaDevolucionPedidoEspecial(List<ProductosDevueltosPedidoEspecial> productos, Int64 idPedidoEspecial,float montoDevuelto,int idUsuario,string motivoDevolucion)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@motivoDevolucion", motivoDevolucion);
                parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                parameters.Add("@montoDevuelto", montoDevuelto);
                parameters.Add("@idUsuario", idUsuario);
                parameters.Add("@xmlProductos", SerializeProductosDevueltos(productos));
                notificacion = ConstructorDapper.Ejecutar("SP_REALIZA_DEVOLUCION_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public string SerializeProductosDevueltos(List<ProductosDevueltosPedidoEspecial> productos)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<ProductosDevueltosPedidoEspecial>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                xmlSerializer.Serialize(xmlWriter, productos);
            }

            return stringBuilder.ToString();

        }

        public Notificacion<dynamic> ObtieneTicketsPedidoEspecial(Int64 idPedidoEspecial)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_TICKETS_PEDIDO_ESPECIAL", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<dynamic> ObtieneDetalleTicketPedidoEspecial(Int64 idPedidoEspecial,int idTipoTicketPedidoEspecial,int idTicketPedidoEspecial, Boolean ticketFinal)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idPedidoEspecial", idPedidoEspecial);
                parameters.Add("@idTipoTicketPedidoEspecial", idTipoTicketPedidoEspecial);
                parameters.Add("@idTicketPedidoEspecial", idTicketPedidoEspecial==0? (object)null : idTicketPedidoEspecial);
                parameters.Add("@ticketFinal", ticketFinal);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_DETALLE_TICKET_PEDIDO_ESPECIAL", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        #endregion

        #region Cierre
        public Notificacion<dynamic> ObtieneCierreDia(int idUsuario,int idEstacion,DateTime fecha,Int64 idCierrePedidoEspecial)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", idUsuario);
                parameters.Add("@idEstacion", idEstacion);
                parameters.Add("@fecha", fecha==DateTime.MinValue? (object)null : fecha);
                parameters.Add("@idCierrePedidoEspecial", idCierrePedidoEspecial == 0 ? (object)null : idCierrePedidoEspecial);
                notificacion = ConstructorDapper.Consultar("SP_CONSULTA_CIERRE_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<dynamic> ObtieneConfiguracionPedidosEspeciales(EnumTipoConfig tipoConfig)
        {
            Notificacion<dynamic> notificacion = new Notificacion<dynamic>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idConfig", tipoConfig==0 ? (object) null : tipoConfig);               
                notificacion = ConstructorDapper.Consultar("SP_OBTENER_CONFIGURACION_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<string> ValidaCierreCajas(int idUsuario)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", idUsuario);
                notificacion = ConstructorDapper.Ejecutar("SP_VALIDA_CIERRE_CAJAS_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<string> RealizaCierreEstacion(int idUsuario,int idEstacion, float efectivoEntregadoEnCierre)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", idUsuario);
                parameters.Add("@idEstacion", idEstacion);
                parameters.Add("@efectivoEntregadoEnCierre", efectivoEntregadoEnCierre);             
                notificacion = ConstructorDapper.Ejecutar("SP_REALIZA_CIERRE_PEDIDOS_ESPECIALES", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }


        #endregion

    }
}