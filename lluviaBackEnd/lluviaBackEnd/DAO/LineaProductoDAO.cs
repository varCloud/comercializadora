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

namespace lluviaBackEnd.DAO
{
    public class LineaProductoDAO
    {

        private DBManager db = null;

        public List<LineaProducto> ObtenerLineaProductos(LineaProducto lineaProducto)
        {

            List<LineaProducto> lstLineaProductos = new List<LineaProducto>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@idLineaProducto", lineaProducto.idLineaProducto);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_LINEAS_PRODUCTO]");

                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            LineaProducto p = new LineaProducto();
                            p.idLineaProducto = Convert.ToInt32(db.DataReader["idLineaProducto"]);
                            p.contador = Convert.ToInt32(db.DataReader["contador"]);
                            p.descripcion = db.DataReader["descripcion"].ToString();
                            p.activo = Convert.ToBoolean(db.DataReader["activo"]);
                            lstLineaProductos.Add(p);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstLineaProductos;
        }


        public Result GuardarLineaProducto(LineaProducto LineaProducto)
        {
            Result result = new Result();
            result.status = -1;
            result.Mensaje = "Existió un error al hacer la actualización.";
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(3);
                    db.AddParameters(0, "@idLineaProducto", LineaProducto.idLineaProducto);
                    db.AddParameters(1, "@descripcion", LineaProducto.descripcion);
                    db.AddParameters(2, "@activo", LineaProducto.activo);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_INSERTA_ACTUALIZA_LINEAS_PRODUCTO]");
                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            result.status = Convert.ToInt32(db.DataReader["status"].ToString());
                            result.Mensaje = db.DataReader["mensaje"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Result ActualizarEstatusLineaProducto(int idLineaProducto, bool activo)
        {
            Result result = new Result();
            result.status = -1;
            result.Mensaje = "Existio un error al hacer la actualización.";
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(2);
                    db.AddParameters(0, "@idLineaProducto", idLineaProducto);
                    db.AddParameters(1, "@activo", activo);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_ACTUALIZA_STATUS_LINEAS_PRODUCTO");
                    if (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            result.status = Convert.ToInt32(db.DataReader["status"].ToString());
                            result.Mensaje = db.DataReader["mensaje"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return result;
        }


        public List<SelectListItem> ObtenerLineaProductos()
        {
            List<SelectListItem> lstLineasDeProductos = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@idLineaProducto", 0);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_LINEAS_PRODUCTO]");
                    while (db.DataReader.Read())
                    {
                        lstLineasDeProductos.Add(
                            new SelectListItem
                            {
                                Text = db.DataReader["descripcion"].ToString(),
                                Value = db.DataReader["idLineaProducto"].ToString()
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            lstLineasDeProductos.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "" });
            return lstLineasDeProductos;
        }

        public List<SelectListItem> ObtenerUnidadesMedidas()
        {
            List<SelectListItem> lstUnidadMedida = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_CONSULTA_UNIDADES_MEDIDA");
                    if (db.DataReader.Read())
                    {
                        if (db.DataReader["Estatus"].ToString().Equals("200"))
                        {
                            db.DataReader.NextResult();
                            while (db.DataReader.Read())
                            {
                                lstUnidadMedida.Add(
                                    new SelectListItem
                                    {
                                        Text = db.DataReader["descripcion"].ToString(),
                                        Value = db.DataReader["idUnidadMedida"].ToString()
                                    });
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            lstUnidadMedida.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "" });
            return lstUnidadMedida;
        }


        public List<SelectListItem> ObtenerClavesProductos()
        {
            List<SelectListItem> lstClavesProductos = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_CLAVES_PRODUCTOS]");
                    while (db.DataReader.Read())
                    {
                        lstClavesProductos.Add(
                            new SelectListItem
                            {
                                Text = db.DataReader["descripcion"].ToString(),
                                Value = db.DataReader["claveProdServ"].ToString()
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            lstClavesProductos.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "" });
            return lstClavesProductos;
        }


        public List<SelectListItem> ObtenerClavesUnidad()
        {
            List<SelectListItem> lstClavesUnidad = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_FACTURACION_CONSULTA_CLAVES_UNIDAD");
                    while (db.DataReader.Read())
                    {
                        lstClavesUnidad.Add(
                            new SelectListItem
                            {
                                Text = db.DataReader["nombre"].ToString(),
                                Value = db.DataReader["claveUnidad"].ToString()
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            lstClavesUnidad.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "0" });
            return lstClavesUnidad;
        }

        public List<SelectListItem> ObtenerUnidadesCompra()
        {
            List<SelectListItem> lstUnidadCompra = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_CONSULTA_UNIDADES_COMPRA");
                    while (db.DataReader.Read())
                    {
                        lstUnidadCompra.Add(
                            new SelectListItem
                            {
                                Text = db.DataReader["descripcion"].ToString(),
                                Value = db.DataReader["idUnidadCompra"].ToString()
                            });
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstUnidadCompra;
        }



    }
}