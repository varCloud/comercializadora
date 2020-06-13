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

namespace lluviaBackEnd.DAO
{
    public class ReportesDAO
    {
        private IDbConnection db = null;


        public Notificacion<List<Producto>> ObtenerInventario(Producto producto)
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto==0 ? (object) null : producto.idProducto);
                    parameters.Add("@descripcion", string.IsNullOrEmpty(producto.descripcion) ? (object)null : producto.descripcion);                    
                    parameters.Add("@idLineaProducto", string.IsNullOrEmpty(producto.idLineaProducto) ? (object)null : Convert.ToInt32(producto.idLineaProducto));                    
                    parameters.Add("@articulo", string.IsNullOrEmpty(producto.articulo) ? (object)null : producto.articulo);
                    parameters.Add("@idAlmacen", producto.idAlmacen==0 ? (object)null : producto.idAlmacen);
                    parameters.Add("@fechaIni", producto.fechaIni==DateTime.MinValue ? (object)null : producto.fechaIni);
                    parameters.Add("@fechaFin", producto.fechaFin == DateTime.MinValue ? (object)null : producto.fechaFin);
                    var result = db.QueryMultiple("SP_CONSULTA_INVENTARIO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Producto>().ToList();
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

        public Notificacion<List<Ventas>> ObtenerVentas(Ventas ventas)
        {
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();

            try
            {
                if (ventas.fechaIni == (new DateTime(0001, 01, 01)))
                    ventas.fechaIni = new DateTime(1900, 01, 01);

                if (ventas.fechaFin == (new DateTime(0001, 01, 01)))
                    ventas.fechaFin = new DateTime(1900, 01, 01);

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", ventas.idProducto);
                    parameters.Add("@descProducto", ventas.descripcionProducto);
                    parameters.Add("@idLineaProducto", ventas.idLineaProducto);
                    parameters.Add("@idCliente", ventas.idCliente);
                    parameters.Add("@idUsuario", ventas.idUsuario);
                    parameters.Add("@fechaIni", ventas.fechaIni);
                    parameters.Add("@fechaFin", ventas.fechaFin);
                    parameters.Add("@tipoConsulta", ventas.tipoConsulta);

                    var result = db.QueryMultiple("SP_CONSULTA_VENTAS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Ventas>().ToList();
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


        //public Notificacion<List<Compras>> ObtenerCompras(Compras compras)
        //{
        //    Notificacion<List<Compras>> notificacion = new Notificacion<List<Compras>>();

        //    try
        //    {
        //        if (compras.fechaIni == (new DateTime(0001, 01, 01)))
        //            compras.fechaIni = new DateTime(1900, 01, 01);

        //        if (compras.fechaFin == (new DateTime(0001, 01, 01)))
        //            compras.fechaFin = new DateTime(1900, 01, 01);

        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();

        //            parameters.Add("@idProducto", compras.producto.idProducto);
        //            parameters.Add("@descProducto", compras.producto.descripcion);
        //            parameters.Add("@idProveedor", compras.proveedor.idProveedor);
        //            parameters.Add("@idLineaProducto", compras.producto.idLineaProducto);
        //            parameters.Add("@idUsuario", compras.usuario.idUsuario);
        //            parameters.Add("@fechaIni", compras.fechaIni);
        //            parameters.Add("@fechaFin", compras.fechaFin);

        //            var result = db.QueryMultiple("SP_CONSULTA_REPORTE_COMPRAS", parameters, commandType: CommandType.StoredProcedure);
        //            var r1 = result.ReadFirst();
        //            if (r1.status == 200)
        //            {
        //                notificacion.Estatus = r1.status;
        //                notificacion.Mensaje = r1.mensaje;
        //                notificacion.Modelo = result.Read<Compras>().ToList();
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

    }
}