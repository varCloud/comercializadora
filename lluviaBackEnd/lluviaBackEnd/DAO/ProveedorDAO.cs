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
    public class ProveedorDAO
    {

        private DBManager db = null;


        public List<Proveedor> ObtenerProveedores(Proveedor proveedor)
        {

            List<Proveedor> lstProveedores = new List<Proveedor>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@idProveedor", proveedor.idProveedor);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_PROVEEDORES]");

                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            Proveedor p = new Proveedor();
                            p.idProveedor = Convert.ToInt32(db.DataReader["idProveedor"]);
                            p.contador = Convert.ToInt32(db.DataReader["contador"]);
                            p.nombre = db.DataReader["nombre"].ToString();
                            p.descripcion = db.DataReader["descripcion"].ToString();
                            p.direccion = db.DataReader["direccion"].ToString();
                            p.telefono = db.DataReader["telefono"].ToString();
                            p.activo = Convert.ToBoolean(db.DataReader["activo"]);
                            p.totalPedidosTotales = Convert.ToInt32(db.DataReader["totalPedidosTotales"]);
                            p.totalPedidosIncompletos = Convert.ToInt32(db.DataReader["totalPedidosIncompletos"]);
                            p.PorcAtendido = Convert.ToSingle(db.DataReader["PorcAtendido"]);
                            lstProveedores.Add(p);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstProveedores;
        }


        public Notificacion<Proveedor> GuardarProveedor(Proveedor proveedor)
        {
            Notificacion<Proveedor> result = new Notificacion<Proveedor>();
            result.Estatus = -1;
            result.Mensaje = "Existió un error al hacer la actualización.";
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(6);
                    db.AddParameters(0, "@idProveedor", proveedor.idProveedor);
                    db.AddParameters(1, "@nombre", proveedor.nombre);
                    db.AddParameters(2, "@descripcion", proveedor.descripcion);
                    db.AddParameters(3, "@telefono", proveedor.telefono);
                    db.AddParameters(4, "@direccion", proveedor.direccion);
                    db.AddParameters(5, "@activo", proveedor.activo);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_INSERTA_ACTUALIZA_PROVEEDORES]");
                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            result.Estatus = Convert.ToInt32(db.DataReader["status"].ToString());
                            result.Mensaje = db.DataReader["mensaje"].ToString();
                            result.Modelo = new Proveedor
                            {
                                idProveedor = Convert.ToInt32(db.DataReader["idProveedor"].ToString()),
                                nombre = db.DataReader["nombreProveedor"].ToString()
                            };

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

        public Result ActualizarEstatusProveedor(int idProveedor, bool activo)
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
                    db.AddParameters(0, "@idProveedor", idProveedor);
                    db.AddParameters(1, "@activo", activo);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_ACTUALIZA_STATUS_PROVEEDOR");
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


        public List<SelectListItem> ObtenerProveedores(int idProveedor)
        {
            List<SelectListItem> lstProveedores = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@idProveedor", idProveedor);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_PROVEEDORES]");
                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"]) == 200)
                        {
                            lstProveedores.Add(
                                        new SelectListItem
                                        {
                                            Text = db.DataReader["nombre"].ToString(),
                                            Value = db.DataReader["idProveedor"].ToString()
                                        });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (lstProveedores.Count > 0)
                lstProveedores.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "0" });

            return lstProveedores;
        }


    }
}