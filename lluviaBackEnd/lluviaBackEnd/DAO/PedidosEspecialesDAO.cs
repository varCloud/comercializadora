using System;
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

namespace lluviaBackEnd.DAO
{
    public class PedidosEspecialesDAO
    {
        private IDbConnection db = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Nuevas PedidosEspeciales
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //public Notificacion<List<Precio>> ObtenerProductoPorPrecio(Precio precio)
        //{
        //    Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idProducto", precio.idProducto);
        //            parameters.Add("@cantidad", precio.cantidad);
        //            parameters.Add("@vaConDescuento", precio.vaConDescuento);
        //            var result = db.QueryMultiple("SP_CONSULTA_PRECIO_X_VOLUMEN", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = result.Read<Precio>().ToList();
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = new List<Precio> { precio };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}


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
                        //notificacion.Modelo.Clear();// [0] = producto;
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
                        //notificacion.Modelo.Clear();// [0] = producto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        
        //public Notificacion<PedidosEspeciales> GuardarPedidoEspecial(List<PedidosEspeciales> venta, int idCliente, int formaPago, int usoCFDI, int idVenta, int idUsuario, int idEstacion, int aplicaIVA, int numClientesAtendidos, int tipoVenta, string motivoDevolucion)
        public Notificacion<PedidosEspeciales> GuardarPedidoEspecial(PedidosEspeciales pedido)
        {
            Notificacion<PedidosEspeciales> notificacion = new Notificacion<PedidosEspeciales>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@XML", Serialize(pedido.lstPedidosInternosDetalle));
                    parameters.Add("@idCliente", pedido.idCliente);
                    parameters.Add("@idPedidoEspecial", pedido.idPedidoEspecial);
                    parameters.Add("@idUsuario", pedido.idUsuario);
                    parameters.Add("@idAlmacenOrigen", pedido.almacenOrigen.idAlmacen);
                    parameters.Add("@descripcion", pedido.descripcion);

                    var result = db.QueryMultiple("SP_GUARDA_PEDIDO_ESPECIAL", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new PedidosEspeciales() { idPedidoEspecial = r1.idPedidoEspecial };
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

        public string Serialize(List<PedidosInternosDetalle> productos)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<PedidosInternosDetalle>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                xmlSerializer.Serialize(xmlWriter, productos);
            }

            return stringBuilder.ToString();

        }

        public string SerializePrecios(List<Precio> precios)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Precio>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 , OmitXmlDeclaration= true }))
            {
                xmlSerializer.Serialize(xmlWriter, precios);
            }

            return stringBuilder.ToString();

        }



        //public Notificacion<Result> AceptarPedido(PedidosEspeciales pedido)
        //{
        //    Notificacion<Result> notificacion = new Notificacion<Result>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idPedidoEspecial", pedido.idPedidoEspecial);
        //            parameters.Add("@idUsuario", pedido.idUsuario);

        //            var result = db.QueryMultiple("SP_ACEPTA_PEDIDO_ESPECIAL", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                //notificacion.Modelo = new PedidosEspeciales() { idPedidoEspecial = r1.idPedidoEspecial };
        //                //notificacion.Modelo = precios; //result.ReadSingle<Producto>();
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                //notificacion.Modelo = producto;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}


        public Notificacion<Result> AceptarRechazarPedidoEspecial(PedidosEspeciales pedido)
        {
            Notificacion<Result> notificacion = new Notificacion<Result>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@XML", Serialize(pedido.lstPedidosInternosDetalle));
                    parameters.Add("@idPedidoEspecial", pedido.idPedidoEspecial);
                    parameters.Add("@idUsuario", pedido.idUsuario);

                    var result = db.QueryMultiple("SP_ACEPTAR_RECHAZAR_PEDIDO_ESPECIAL", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo = new PedidosEspeciales() { idPedidoEspecial = r1.idPedidoEspecial };
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



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Editar PedidosEspeciales
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Notificacion<List<Ticket>> ObtenerTicketsPedidosEspeciales(Ticket ticket)
        {
            Notificacion<List<Ticket>> notificacion = new Notificacion<List<Ticket>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta", ticket.idVenta);
                    parameters.Add("@tipoVenta", ticket.tipoVenta);
                    var result = db.QueryMultiple("SP_CONSULTA_TICKET", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Ticket>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new List<Ticket> { ticket };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


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


        //public Notificacion<PedidosEspeciales> GuardarIVA(PedidosEspeciales venta)
        //{
        //    Notificacion<PedidosEspeciales> notificacion = new Notificacion<PedidosEspeciales>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idVenta", venta.idVenta);
        //            parameters.Add("@montoIVA", venta.montoIVA);
        //            parameters.Add("@idCliente", venta.idCliente);
        //            parameters.Add("@idFactFormaPago", venta.idFactFormaPago);
        //            parameters.Add("@idFactUsoCFDI", venta.idFactUsoCFDI);
        //            var result = db.QueryMultiple("SP_GUARDA_IVA_VENTA", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = result.ReadSingle<PedidosEspeciales>(); ;
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = venta;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}





        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Herramientas
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //public Notificacion<Cierre> ConsultaInfoCierre(Cierre cierre)
        //{
        //    Notificacion<Cierre> notificacion = new Notificacion<Cierre>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idEstacion", cierre.idEstacion);
        //            parameters.Add("@idUsuario", cierre.idUsuario);
        //            var result = db.QueryMultiple("SP_CONSULTA_INFO_CIERRE", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = result.ReadSingle<Cierre>(); ;
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = cierre;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}


        //public Notificacion<List<Retiros>> ConsultaRetirosEfectivo(Retiros retiros)
        //{
        //    Notificacion<List<Retiros>> notificacion = new Notificacion<List<Retiros>>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idEstacion", retiros.idEstacion==0 ? (object) null : retiros.idEstacion);
        //            parameters.Add("@fecha", retiros.fechaAlta == DateTime.MinValue ? (object)null : retiros.fechaAlta);
        //            parameters.Add("@idRetiro", retiros.idRetiro==0 ? (object) null : retiros.idRetiro);
        //            parameters.Add("@idUsuario", retiros.idUsuario == 0 ? (object)null : retiros.idUsuario);
        //            parameters.Add("@idAlmacen", retiros.idAlmacen == 0 ? (object)null : retiros.idAlmacen);

        //            var result = db.QueryMultiple("SP_CONSULTA_RETIROS_EFECTIVO", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = result.Read<Retiros, Status, Retiros>(MapRetiros,splitOn: "idStatus").ToList();
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = new List<Retiros> { retiros };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}


        //public Notificacion<List<Retiros>> ConsultaRetiros(Retiros retiros)
        //{
        //    Notificacion<List<Retiros>> notificacion = new Notificacion<List<Retiros>>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idEstacion", retiros.idEstacion == 0 ? (object)null : retiros.idEstacion);
        //            parameters.Add("@idTipoRetiro", retiros.tipoRetiro == 0 ? (object)null : retiros.tipoRetiro);
        //            parameters.Add("@fecha", retiros.fechaAlta == DateTime.MinValue ? (object)null : retiros.fechaAlta);
        //            parameters.Add("@idUsuario", retiros.idUsuario == 0 ? (object)null : retiros.idUsuario);
        //            parameters.Add("@idAlmacen", retiros.idAlmacen == 0 ? (object)null : retiros.idAlmacen);
        //            var result = db.QueryMultiple("SP_CONSULTA_RETIROS", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;   
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = result.Read<Retiros, Status, Retiros>(MapRetiros, splitOn: "idStatus").ToList();
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = new List<Retiros> { retiros };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}

        //public Notificacion<string> ActualizaEstatusRetiro(Retiros retiros)
        //{
        //    Notificacion<string> notificacion = new Notificacion<string>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idRetiro", retiros.idRetiro);
        //            parameters.Add("@idStatus", retiros.estatusRetiro.idStatus);
        //            parameters.Add("@monto", retiros.montoAutorizado);
        //            parameters.Add("@idUsuario", retiros.idUsuario);
        //            parameters.Add("@idTipoRetiro", retiros.tipoRetiro);
        //            notificacion = db.QuerySingle<Notificacion<string>>("SP_ACTUALIZA_STATUS_RETIROS", parameters, commandType: CommandType.StoredProcedure);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}



        //public Retiros MapRetiros(Retiros r,Status s)
        //{
        //    r.estatusRetiro = s;
        //    return r;
        //}


        //public Notificacion<Retiros> RetirarExcesoEfectivo(Retiros retiros)
        //{
        //    Notificacion<Retiros> notificacion = new Notificacion<Retiros>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idEstacion", retiros.idEstacion);
        //            parameters.Add("@idUsuario", retiros.idUsuario);
        //            parameters.Add("@monto", retiros.montoRetiro);
        //            parameters.Add("@caso", 1);
        //            var result = db.QueryMultiple("SP_RETIRA_EFECTIVO", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = retiros;// result.ReadSingle<Retiros>(); ;
        //                notificacion.Modelo = new Retiros() { idRetiro = r1.idRetiro };
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = retiros;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}


        //public Notificacion<Retiros> RealizaCierreEstacion(Retiros retiros)
        //{
        //    Notificacion<Retiros> notificacion = new Notificacion<Retiros>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idEstacion", retiros.idEstacion);
        //            parameters.Add("@idUsuario", retiros.idUsuario);
        //            parameters.Add("@monto", retiros.montoRetiro);
        //            parameters.Add("@caso", 2);
        //            var result = db.QueryMultiple("SP_RETIRA_EFECTIVO", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = retiros;// result.ReadSingle<Retiros>(); ;
        //            }
        //            else
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = retiros;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}



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
                        //p.Estatus = rs1.status;
                        //p.Mensaje = rs1.mensaje;
                        p = rs.Read<PedidosInternosDetalle>().ToList();
                    }
                    //else
                    //{
                    //    p.Estatus = rs1.status;
                    //    p.Mensaje = rs1.mensaje;
                    //}
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


    }
}