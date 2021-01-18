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
    public class VentasDAO
    {
        private IDbConnection db = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Nuevas Ventas
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


        public Notificacion<Ventas> GuardarVenta(List<Ventas> venta, int idCliente, int formaPago, int usoCFDI, int idVenta, int idUsuario, int idEstacion, int aplicaIVA, int numClientesAtendidos, int tipoVenta, string motivoDevolucion, int idPedidoEspecial)
        {
            Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@XML", Serialize(venta));
                    parameters.Add("@idCliente", idCliente);
                    parameters.Add("@idFactFormaPago", formaPago);
                    parameters.Add("@idFactUsoCFDI", usoCFDI);
                    parameters.Add("@idVenta", idVenta);
                    parameters.Add("@idUsuario", idUsuario);
                    parameters.Add("@idEstacion", idEstacion);
                    parameters.Add("@aplicaIVA", aplicaIVA);
                    parameters.Add("@numClientesAtendidos", numClientesAtendidos);
                    parameters.Add("@tipoVenta", tipoVenta);
                    parameters.Add("@motivoDevolucion", motivoDevolucion);
                    parameters.Add("@idPedidoEspecial", idPedidoEspecial);

                    var result = db.QueryMultiple("SP_REALIZA_VENTA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new Ventas() { idVenta = r1.idVenta, cantProductosLiq = r1.cantProductosLiq };
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

        public string Serialize(List<Ventas> venta)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Ventas>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                xmlSerializer.Serialize(xmlWriter, venta);
            }

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


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //  Editar Ventas
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Notificacion<List<Ticket>> ObtenerTickets(Ticket ticket)
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


        public Notificacion<Ventas> CancelaVenta(Ventas venta)
        {
            Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta", venta.idVenta);
                    parameters.Add("@idUsuario", venta.idUsuario);
                    var result = db.QueryMultiple("SP_ELIMINA_VENTA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = venta;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = venta;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }




        public Notificacion<Ventas> ConsultaVenta(Ventas venta)
        {
            Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta", venta.idVenta);
                    var result = db.QueryMultiple("SP_CONSULTA_VENTA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.ReadSingle<Ventas>(); ;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = venta;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<Ventas> GuardarIVA(Ventas venta)
        {
            Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta", venta.idVenta);
                    parameters.Add("@montoIVA", venta.montoIVA);
                    parameters.Add("@idCliente", venta.idCliente);
                    parameters.Add("@idFactFormaPago", venta.idFactFormaPago);
                    parameters.Add("@idFactUsoCFDI", venta.idFactUsoCFDI);
                    var result = db.QueryMultiple("SP_GUARDA_IVA_VENTA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.ReadSingle<Ventas>(); ;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = venta;
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
        //  Herramientas
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Notificacion<Cierre> ConsultaInfoCierre(Cierre cierre)
        {
            Notificacion<Cierre> notificacion = new Notificacion<Cierre>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idEstacion", cierre.idEstacion);
                    parameters.Add("@idUsuario", cierre.idUsuario);
                    var result = db.QueryMultiple("SP_CONSULTA_INFO_CIERRE", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.ReadSingle<Cierre>(); ;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = cierre;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<List<Retiros>> ConsultaRetirosEfectivo(Retiros retiros)
        {
            Notificacion<List<Retiros>> notificacion = new Notificacion<List<Retiros>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idEstacion", retiros.idEstacion == 0 ? (object)null : retiros.idEstacion);
                    parameters.Add("@fecha", retiros.fechaAlta == DateTime.MinValue ? (object)null : retiros.fechaAlta);
                    parameters.Add("@idRetiro", retiros.idRetiro == 0 ? (object)null : retiros.idRetiro);
                    parameters.Add("@idUsuario", retiros.idUsuario == 0 ? (object)null : retiros.idUsuario);
                    parameters.Add("@idAlmacen", retiros.idAlmacen == 0 ? (object)null : retiros.idAlmacen);

                    var result = db.QueryMultiple("SP_CONSULTA_RETIROS_EFECTIVO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Retiros, Status, Retiros>(MapRetiros, splitOn: "idStatus").ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new List<Retiros> { retiros };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<List<Retiros>> ConsultaRetiros(Retiros retiros)
        {
            Notificacion<List<Retiros>> notificacion = new Notificacion<List<Retiros>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idEstacion", retiros.idEstacion == 0 ? (object)null : retiros.idEstacion);
                    parameters.Add("@idTipoRetiro", retiros.tipoRetiro == 0 ? (object)null : retiros.tipoRetiro);
                    parameters.Add("@fecha", retiros.fechaAlta == DateTime.MinValue ? (object)null : retiros.fechaAlta);
                    parameters.Add("@idUsuario", retiros.idUsuario == 0 ? (object)null : retiros.idUsuario);
                    parameters.Add("@idAlmacen", retiros.idAlmacen == 0 ? (object)null : retiros.idAlmacen);
                    var result = db.QueryMultiple("SP_CONSULTA_RETIROS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Retiros, Status, Retiros>(MapRetiros, splitOn: "idStatus").ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new List<Retiros> { retiros };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<string> ActualizaEstatusRetiro(Retiros retiros)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idRetiro", retiros.idRetiro);
                    parameters.Add("@idStatus", retiros.estatusRetiro.idStatus);
                    parameters.Add("@monto", retiros.montoAutorizado);
                    parameters.Add("@idUsuario", retiros.idUsuario);
                    parameters.Add("@idTipoRetiro", retiros.tipoRetiro);
                    notificacion = db.QuerySingle<Notificacion<string>>("SP_ACTUALIZA_STATUS_RETIROS", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }



        public Retiros MapRetiros(Retiros r, Status s)
        {
            r.estatusRetiro = s;
            return r;
        }


        public Notificacion<Retiros> RetirarExcesoEfectivo(Retiros retiros)
        {
            Notificacion<Retiros> notificacion = new Notificacion<Retiros>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idEstacion", retiros.idEstacion);
                    parameters.Add("@idUsuario", retiros.idUsuario);
                    parameters.Add("@monto", retiros.montoRetiro);
                    parameters.Add("@caso", 1);
                    var result = db.QueryMultiple("SP_RETIRA_EFECTIVO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = retiros;// result.ReadSingle<Retiros>(); ;
                        notificacion.Modelo = new Retiros() { idRetiro = r1.idRetiro };
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = retiros;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<Retiros> RealizaCierreEstacion(Retiros retiros)
        {
            Notificacion<Retiros> notificacion = new Notificacion<Retiros>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idEstacion", retiros.idEstacion);
                    parameters.Add("@idUsuario", retiros.idUsuario);
                    parameters.Add("@monto", retiros.montoRetiro);
                    parameters.Add("@caso", 2);
                    var result = db.QueryMultiple("SP_RETIRA_EFECTIVO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = retiros;// result.ReadSingle<Retiros>(); ;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = retiros;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Result ValidaAperturaCajas(int idUsuario)
        {
            Result result = new Result();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", idUsuario);
                    result = db.Query<Result>("SP_VALIDA_APERTURA_CAJAS", parameters, commandType: CommandType.StoredProcedure).First();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Result IngresoEfectivo(int idUsuario,float monto,int idTipoIngreso)
        {
            Result result = new Result();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", idUsuario);
                    parameters.Add("@monto", monto);
                    parameters.Add("@idTipoIngreso", idTipoIngreso);
                    result = db.Query<Result>("SP_INGRESO_ECTIVO", parameters, commandType: CommandType.StoredProcedure).First();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Notificacion<List<IngresoEfectivo>> ConsultaIngresosEfectivo(IngresoEfectivo ingresoEfectivo)
        {
            Notificacion<List<IngresoEfectivo>> notificacion = new Notificacion<List<IngresoEfectivo>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idTipoIngreso", ingresoEfectivo.idTipoIngreso == 0 ? (object)null : ingresoEfectivo.idTipoIngreso);
                    parameters.Add("@fecha", ingresoEfectivo.fechaAlta == DateTime.MinValue ? (object)null : ingresoEfectivo.fechaAlta);
                    parameters.Add("@idIngreso", ingresoEfectivo.idIngreso == 0 ? (object)null : ingresoEfectivo.idIngreso);
                    parameters.Add("@idUsuario", ingresoEfectivo.idUsuario == 0 ? (object)null : ingresoEfectivo.idUsuario);
                    parameters.Add("@idAlmacen", ingresoEfectivo.idAlmacen == 0 ? (object)null : ingresoEfectivo.idAlmacen);

                    var result = db.QueryMultiple("SP_CONSULTA_INGRESOS_EFECTIVO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<IngresoEfectivo>().ToList();
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

        public List<ExcesoEfectivo> excesoEfectivo(int idUsuario)
        {
            List<ExcesoEfectivo> excesoEfectivos = new List<ExcesoEfectivo>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", idUsuario == 0 ? (object)null : idUsuario);
                    excesoEfectivos = db.Query<ExcesoEfectivo>("SP_CONSULTA_EXCESO_EFECTIVO", parameters, commandType: CommandType.StoredProcedure).ToList();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return excesoEfectivos;
        }

    }
}