using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;
using System.Web.Mvc;

namespace lluviaBackEnd.DAO
{
    public class EstacionesDAO
    {
        private IDbConnection db;

        public Notificacion<Estacion> GuardarEstacion(Estacion estacion)
        {
            Notificacion<Estacion> notificacion = new Notificacion<Estacion>();
            try
            {

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idEstacion", estacion.idEstacion);
                    parameters.Add("@idAlmacen", estacion.idAlmacen);
                    parameters.Add("@macAdress", estacion.macAdress);
                    parameters.Add("@nombre", estacion.nombre);
                    parameters.Add("@numero", estacion.numero);
                    parameters.Add("@configurado", estacion.configurado);
                    parameters.Add("@idUsuario", estacion.idUsuario);
                    //parameters.Add("@fechaAlta", estacion.fechaAlta);
                    //parameters.Add("@idStatus", estacion.idStatus);

                    var result = db.QueryMultiple("SP_INSERTA_ACTUALIZA_ESTACIONES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = estacion; 
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = estacion;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public Notificacion<Estacion> EliminarEstacion(Estacion estacion)
        {
            Notificacion<Estacion> notificacion = new Notificacion<Estacion>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idEstacion", estacion.idEstacion);
                    parameters.Add("@idStatus", estacion.idStatus);

                    var result = db.QueryMultiple("SP_ACTUALIZA_STATUS_ESTACIONES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = estacion;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = estacion;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public Notificacion<List<Estacion>> ObtenerEstaciones(Estacion estacion)
        {
            Notificacion<List<Estacion>> notificacion = new Notificacion<List<Estacion>>();

            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idEstacion", estacion.idEstacion);
                    var result = db.QueryMultiple("SP_CONSULTA_ESTACIONES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Estacion>().ToList();
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
        

    }



}