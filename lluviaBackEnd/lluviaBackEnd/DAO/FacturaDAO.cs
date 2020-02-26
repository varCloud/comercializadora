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
    public class FacturaDAO
    {

        private DBManager db = null;


        //    public List<Factura> ObtenerFacturas(Factura usr)
        //    {

        //        List<Factura> lstFacturas = new List<Factura>();
        //        try
        //        {
        //            using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //            {
        //                db.Open();
        //                db.CreateParameters(1);
        //                db.AddParameters(0, "@idFactura", usr.idFactura); 
        //                db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_FacturaS]");

        //                while (db.DataReader.Read())
        //                {
        //                    if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
        //                    {
        //                        Factura u = new Factura();
        //                        u.idFactura = Convert.ToInt32(db.DataReader["idFactura"]);
        //                        u.Factura = db.DataReader["Factura"].ToString();
        //                        u.contrasena = db.DataReader["contrasena"].ToString();
        //                        u.nombre = db.DataReader["nombre"].ToString();
        //                        u.apellidoPaterno = db.DataReader["apellidoPaterno"].ToString();
        //                        u.apellidoMaterno = db.DataReader["apellidoMaterno"].ToString();
        //                        u.Sucursal = db.DataReader["descripcionSucursal"].ToString();
        //                        u.Almacen = db.DataReader["descripcionAlmacen"].ToString();
        //                        u.Rol = db.DataReader["descripcionRol"].ToString();
        //                        u.idRol = Convert.ToInt32(db.DataReader["idRol"].ToString());
        //                        u.idAlmacen = Convert.ToInt32(db.DataReader["idAlmacen"].ToString());
        //                        u.idSucursal = Convert.ToInt32(db.DataReader["idSucursal"].ToString());
        //                        u.telefono = db.DataReader["telefono"].ToString();
        //                        u.activo = Convert.ToBoolean(db.DataReader["activo"]);
        //                        lstFacturas.Add(u);
        //                    }
        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        return lstFacturas;
        //    }


        //    public List<SelectListItem> ObtenerRoles(Rol rol)
        //    {
        //        List<SelectListItem> listRoles = new List<SelectListItem>();
        //        try
        //        {
        //            using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //            {
        //                db.Open();
        //                db.CreateParameters(1);
        //                db.AddParameters(0, "@idRol", rol.idRol);
        //                db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_ROLES]");
        //                while (db.DataReader.Read())
        //                {
        //                    listRoles.Add(
        //                        new SelectListItem
        //                        {
        //                            Text = db.DataReader["descripcion"].ToString(),
        //                            Value = db.DataReader["idRol"].ToString()
        //                        });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        return listRoles;
        //    }


        //    public List<SelectListItem> ObtenerAlmacenes()
        //    {
        //        List<SelectListItem> lstAlmacenes = new List<SelectListItem>();
        //        try
        //        {
        //            using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //            {
        //                db.Open();
        //                //db.CreateParameters(0);
        //                //db.AddParameters(0, "@idRol", rol.idRol);
        //                db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_ALMACENES]");
        //                while (db.DataReader.Read())
        //                {
        //                    lstAlmacenes.Add(
        //                        new SelectListItem
        //                        {
        //                            Text = db.DataReader["descripcion"].ToString(),
        //                            Value = db.DataReader["idAlmacen"].ToString()
        //                        });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        return lstAlmacenes;
        //    }

        //    public List<SelectListItem> ObtenerSucursales()
        //    {
        //        List<SelectListItem> lstSucursales = new List<SelectListItem>();
        //        try
        //        {
        //            using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //            {
        //                db.Open();
        //                //db.CreateParameters(0);
        //                //db.AddParameters(0, "@idRol", rol.idRol);
        //                db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_SUCURSALES]");
        //                while (db.DataReader.Read())
        //                {
        //                    lstSucursales.Add(
        //                        new SelectListItem
        //                        {
        //                            Text = db.DataReader["descripcion"].ToString(),
        //                            Value = db.DataReader["idSucursal"].ToString()
        //                        });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        return lstSucursales;
        //    }


        //    public Result GuardarFactura(Factura usr)
        //    {
        //        Result result = new Result();
        //        result.status = -1;
        //        result.mensaje = "Existió un error al hacer la actualización.";
        //        try
        //        {
        //            using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //            {
        //                db.Open();
        //                db.CreateParameters(12);
        //                db.AddParameters(0, "@idFactura", usr.idFactura);
        //                db.AddParameters(1, "@idRol", usr.idRolGuardar[0]);
        //                db.AddParameters(2, "@Factura", usr.Factura);
        //                db.AddParameters(3, "@telefono", usr.telefono);
        //                db.AddParameters(4, "@contrasena", usr.contrasena);
        //                db.AddParameters(5, "@idAlmacen", usr.idAlmacenGuardar[0]);
        //                db.AddParameters(6, "@idSucursal", usr.idSucursalGuardar[0]);
        //                db.AddParameters(7, "@nombre", usr.nombre);
        //                db.AddParameters(8, "@apellidoPaterno", usr.apellidoPaterno);
        //                db.AddParameters(9, "@apellidoMaterno", usr.apellidoMaterno);
        //                db.AddParameters(10, "@fecha_alta", DateTime.Now);
        //                db.AddParameters(11, "@activo", usr.activo);
        //                db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_INSERTA_ACTUALIZA_FACTURAS]");
        //                while (db.DataReader.Read())
        //                {
        //                    if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
        //                    {
        //                        result.status = Convert.ToInt32(db.DataReader["status"].ToString());
        //                        result.mensaje = db.DataReader["mensaje"].ToString();
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        return result;
        //    }

        //    public Result ActualizarEstatusFactura(int idFactura, bool activo)
        //    {
        //        Result result = new Result();
        //        result.status = -1;
        //        result.mensaje = "Existio un error al hacer la actualización.";
        //        try
        //        {
        //            using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //            {
        //                db.Open();
        //                db.CreateParameters(2);
        //                db.AddParameters(0, "@idFactura", idFactura);
        //                db.AddParameters(1, "@activo", activo);
        //                db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_ACTUALIZA_STATUS_FACTURA");
        //                if (db.DataReader.Read())
        //                {
        //                    if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
        //                    {
        //                        result.status = Convert.ToInt32(db.DataReader["status"].ToString());
        //                        result.mensaje = db.DataReader["mensaje"].ToString();
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //        }
        //        return result;
        //    }

    }
}