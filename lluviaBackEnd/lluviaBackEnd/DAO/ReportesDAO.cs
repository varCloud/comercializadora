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
                if (producto.fechaIni == (new DateTime(0001, 01, 01)))
                    producto.fechaIni = new DateTime(1900, 01, 01);

                if (producto.fechaFin == (new DateTime(0001, 01, 01)))
                    producto.fechaFin = new DateTime(1900, 01, 01);

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@descripcion", producto.descripcion);
                    parameters.Add("@idUnidadMedida", producto.idUnidadMedida);
                    parameters.Add("@idLineaProducto", producto.idLineaProducto);
                    parameters.Add("@activo", producto.activo);
                    parameters.Add("@articulo", producto.articulo);
                    parameters.Add("@fechaIni", producto.fechaIni);
                    parameters.Add("@fechaFin", producto.fechaFin);

                    var result = db.QueryMultiple("SP_CONSULTA_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
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


        public Notificacion<List<Compras>> ObtenerCompras(Compras compras)
        {
            Notificacion<List<Compras>> notificacion = new Notificacion<List<Compras>>();

            try
            {
                if (compras.fechaIni == (new DateTime(0001, 01, 01)))
                    compras.fechaIni = new DateTime(1900, 01, 01);

                if (compras.fechaFin == (new DateTime(0001, 01, 01)))
                    compras.fechaFin = new DateTime(1900, 01, 01);

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", compras.idProducto);
                    parameters.Add("@descProducto", compras.descripcionProducto);
                    parameters.Add("@idProveedor", compras.idProveedor);
                    parameters.Add("@idLineaProducto", compras.idLineaProducto);
                    parameters.Add("@idUsuario", compras.idUsuario);
                    parameters.Add("@fechaIni", compras.fechaIni);
                    parameters.Add("@fechaFin", compras.fechaFin);

                    var result = db.QueryMultiple("SP_CONSULTA_COMPRAS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Compras>().ToList();
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