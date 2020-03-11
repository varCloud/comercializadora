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
    public class VentasDAO
    {
        private IDbConnection db = null;

        public Notificacion<List<Ventas>> ObtenerVentas(Ventas ventas)
        {
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();

            try
            {

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", ventas.idVenta);

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
        public Notificacion<Ventas> GuardarVentas(List<Ventas> venta)
        {
            Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    //parameters.Add("@idProducto", venta.idVenta);

                    //var result = db.QueryMultiple("SP_INSERTA_ACTUALIZA_VENTAS", parameters, commandType: CommandType.StoredProcedure);
                    //var r1 = result.ReadFirst();
                    //if (r1.status == 200)
                    //{
                    //    notificacion.Estatus = r1.status;
                    //    notificacion.Mensaje = r1.mensaje;
                    //    notificacion.Modelo = venta; 
                    //}
                    //else
                    //{
                    //    notificacion.Estatus = r1.status;
                    //    notificacion.Mensaje = r1.mensaje;
                    //    notificacion.Modelo = venta;
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<Ventas> ActualizarEstatusVentas(Ventas ventas)
        {
            Notificacion<Ventas> notificacion = new Notificacion<Ventas>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", ventas.idProducto);
                    
                    var result = db.QueryMultiple("SP_ACTUALIZA_STATUS_VENTAS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = ventas; 
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = ventas;
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