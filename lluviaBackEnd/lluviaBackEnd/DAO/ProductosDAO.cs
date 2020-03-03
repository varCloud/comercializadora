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
    public class ProductosDAO
    {
        private IDbConnection db = null;

        public Notificacion<List<Producto>> ObtenerProductos(Producto producto)
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();

            try
            {
                //if (  producto.fechaIni == (new DateTime(0001,01,01) ) )
                //{
                //    producto.fechaIni = new DateTime(1900, 01, 01);

                //}
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@descripcion", producto.descripcion);
                    parameters.Add("@idUnidadMedida", producto.idUnidadMedida);
                    parameters.Add("@idLineaProducto", producto.idLineaProducto);
                    parameters.Add("@activo", producto.activo);
                    parameters.Add("@articulo", producto.articulo);

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


        public Notificacion<Producto> GuardarProducto(Producto producto)
        {

            Notificacion<Producto> notificacion = new Notificacion<Producto>();

            try
            {

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@descripcion", producto.descripcion);
                    parameters.Add("@idUnidadMedida", producto.idUnidadMedida);
                    parameters.Add("@idLineaProducto", producto.idLineaProducto);
                    parameters.Add("@cantidadUnidadMedida", producto.cantidadUnidadMedida);
                    parameters.Add("@codigoBarras", producto.codigoBarras);
                    parameters.Add("@activo", producto.activo);
                    parameters.Add("@articulo", producto.articulo);

                    var result = db.QueryMultiple("SP_INSERTA_ACTUALIZA_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = producto; //result.ReadSingle<Producto>();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = producto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<Producto> ActualizarEstatusProducto(Producto producto)
        {
            Notificacion<Producto> notificacion = new Notificacion<Producto>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@activo", producto.activo);
                    var result = db.QueryMultiple("SP_ACTUALIZA_STATUS_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = producto; 
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = producto;
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