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
    public class UsuarioDAO
    {

        private DBManager db = null;
        private IDbConnection db_ = null;


        public List<Usuario> ObtenerUsuarios(Usuario usr)
        {

            List<Usuario> lstUsuarios = new List<Usuario>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(3);
                    db.AddParameters(0, "@idUsuario", usr.idUsuario);
                    db.AddParameters(1, "@idAlmacen", usr.idAlmacen);
                    db.AddParameters(2, "@idRol", usr.idRol);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_USUARIOS]");

                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            Usuario u = new Usuario();
                            u.idUsuario = Convert.ToInt32(db.DataReader["idUsuario"]);
                            u.contador = Convert.ToInt32(db.DataReader["contador"]);
                            u.usuario = db.DataReader["usuario"].ToString();
                            u.contrasena = db.DataReader["contrasena"].ToString();
                            u.nombre = db.DataReader["nombre"].ToString();
                            u.apellidoPaterno = db.DataReader["apellidoPaterno"].ToString();
                            u.apellidoMaterno = db.DataReader["apellidoMaterno"].ToString();
                            u.Sucursal = db.DataReader["descripcionSucursal"].ToString();
                            u.Almacen = db.DataReader["descripcionAlmacen"].ToString();
                            u.Rol = db.DataReader["descripcionRol"].ToString();
                            u.idRol = Convert.ToInt32(db.DataReader["idRol"].ToString());
                            u.idAlmacen = Convert.ToInt32(db.DataReader["idAlmacen"].ToString());
                            u.idSucursal = Convert.ToInt32(db.DataReader["idSucursal"].ToString());
                            u.telefono = db.DataReader["telefono"].ToString();
                            u.activo = Convert.ToBoolean(db.DataReader["activo"]);
                            u.nombreCompleto = db.DataReader["nombreCompleto"].ToString();
                            
                            lstUsuarios.Add(u);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstUsuarios;
        }

        public List<SelectListItem> ObtenerUsuarios(int idUsuario)
        {
            List<SelectListItem> lstUsuarios = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@idUsuario", idUsuario);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_USUARIOS]");
                    while (db.DataReader.Read())
                    {
                        lstUsuarios.Add(
                            new SelectListItem
                            {
                                Text = db.DataReader["nombre"].ToString() + ' ' + db.DataReader["apellidoPaterno"].ToString() + ' ' + db.DataReader["apellidoMAterno"].ToString(),
                                Value = db.DataReader["idUsuario"].ToString()
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            lstUsuarios.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "0" });
            return lstUsuarios;
        }

        public List<SelectListItem> ObtenerClientes(int idCliente)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@idCliente", idCliente);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_CLIENTES]");
                    db.DataReader.NextResult();
                    while (db.DataReader.Read())
                    {
                        //string nombre = db.DataReader["nombres"].ToString() + ' ' + db.DataReader["apellidoPaterno"].ToString() + ' ' + db.DataReader["apellidoMaterno"].ToString();
                        lst.Add(
                            new SelectListItem
                            {
                                Text = db.DataReader["nombres"].ToString() + ' ' + db.DataReader["apellidoPaterno"].ToString() + ' ' + db.DataReader["apellidoMaterno"].ToString(),
                                Value = db.DataReader["idCliente"].ToString()
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            lst.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "0" });
            return lst;
        }

        public List<SelectListItem> ObtenerRoles(Rol rol)
        {
            List<SelectListItem> listRoles = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@idRol", rol.idRol);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_ROLES]");
                    while (db.DataReader.Read())
                    {
                        listRoles.Add(
                            new SelectListItem
                            {
                                Text = db.DataReader["descripcion"].ToString(),
                                Value = db.DataReader["idRol"].ToString()
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listRoles;
        }


        public List<SelectListItem> ObtenerAlmacenes(int idSucursal = 0, int idTipoAlmacen = 0)
        {
            List<SelectListItem> lstAlmacenes = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(2);
                    db.AddParameters(0, "@idSucursal", idSucursal == 0 ? (object)null : idSucursal);
                    db.AddParameters(1, "@idTipoAlmacen", idTipoAlmacen == 0 ? (object)null : idTipoAlmacen);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_ALMACENES]");
                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt16(db.DataReader["status"]) == 200)
                            lstAlmacenes.Add(
                                new SelectListItem
                                {
                                    Text = db.DataReader["descripcion"].ToString(),
                                    Value = db.DataReader["idAlmacen"].ToString()
                                });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstAlmacenes;
        }


        public Notificacion<List<Almacen>> ObtenerAlmacenes(Almacen almacen)
        {
            Notificacion<List<Almacen>> lstAlmacenes = new Notificacion<List<Almacen>>();
            lstAlmacenes.Modelo = new List<Almacen>();
            lstAlmacenes.Mensaje = "Ok";
            lstAlmacenes.Estatus = 200;
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(2);
                    db.AddParameters(0, "@idSucursal", almacen.idSucursal == 0 ? (object)null : almacen.idSucursal);
                    db.AddParameters(1, "@idTipoAlmacen", almacen.idTipoAlmacen == 0 ? (object)null : almacen.idTipoAlmacen);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_ALMACENES]");
                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt16(db.DataReader["status"]) == 200)
                            lstAlmacenes.Modelo.Add(new Almacen { idAlmacen = Convert.ToInt32(db.DataReader["idAlmacen"].ToString()), descripcion = db.DataReader["descripcion"].ToString() });
                    }
                }
            }
            catch (Exception ex)
            {
                lstAlmacenes.Mensaje = "Error - " + ex.Message;
                lstAlmacenes.Estatus = -1;
                throw ex;
            }
            return lstAlmacenes;
        }


        public List<SelectListItem> ObtenerSucursales()
        {
            List<SelectListItem> lstSucursales = new List<SelectListItem>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    //db.CreateParameters(0);
                    //db.AddParameters(0, "@idRol", rol.idRol);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_SUCURSALES]");
                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt16(db.DataReader["status"]) == 200)
                            lstSucursales.Add(
                            new SelectListItem
                            {
                                Text = db.DataReader["descripcion"].ToString(),
                                Value = db.DataReader["idSucursal"].ToString()
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstSucursales;
        }


        public Result GuardarUsuario(Usuario usr)
        {
            Result result = new Result();
            result.status = -1;
            result.Mensaje = "Existió un error al hacer la actualización.";
            System.Globalization.TextInfo ti = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(12);
                    db.AddParameters(0, "@idUsuario", usr.idUsuario);
                    db.AddParameters(1, "@idRol", usr.idRolGuardar[0]);
                    db.AddParameters(2, "@usuario", usr.usuario);
                    db.AddParameters(3, "@telefono", usr.telefono);
                    db.AddParameters(4, "@contrasena", usr.contrasena);
                    db.AddParameters(5, "@idAlmacen", usr.idAlmacenGuardar[0]);
                    db.AddParameters(6, "@idSucursal", usr.idSucursalGuardar[0]);
                    db.AddParameters(7, "@nombre", string.IsNullOrEmpty(usr.nombre) ? "" : ti.ToTitleCase(usr.nombre.ToLower()));
                    db.AddParameters(8, "@apellidoPaterno", string.IsNullOrEmpty(usr.apellidoPaterno) ? "" : ti.ToTitleCase(usr.apellidoPaterno.ToLower()));
                    db.AddParameters(9, "@apellidoMaterno", string.IsNullOrEmpty(usr.apellidoMaterno) ? "" : ti.ToTitleCase(usr.apellidoMaterno.ToLower()));
                    db.AddParameters(10, "@fecha_alta", DateTime.Now);
                    db.AddParameters(11, "@activo", usr.activo);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_INSERTA_ACTUALIZA_USUARIOS]");
                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            result.status = Convert.ToInt32(db.DataReader["status"].ToString());
                            result.Mensaje = db.DataReader["mensaje"].ToString();
                        }else if (Convert.ToInt32(db.DataReader["status"].ToString()) == -1)
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

        public Result ActualizarEstatusUsuario(int idUsuario, bool activo)
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
                    db.AddParameters(0, "@idUsuario", idUsuario);
                    db.AddParameters(1, "@activo", activo);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_ACTUALIZA_STATUS_USUARIO");
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


        //public Notificacion<List<Usuario>> ObtenerUsuariosPorAlmacen(int idAlmacen)
        //{
        //    Notificacion<List<Usuario>> notificacion = new Notificacion<List<Usuario>>();
        //    try
        //    {
        //        using (db_ = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@idEstacion", retiros.idEstacion == 0 ? (object)null : retiros.idEstacion);
        //            parameters.Add("@idTipoRetiro", retiros.tipoRetiro == 0 ? (object)null : retiros.tipoRetiro);
        //            parameters.Add("@fecha", retiros.fechaAlta == DateTime.MinValue ? (object)null : retiros.fechaAlta);
        //            parameters.Add("@idUsuario", retiros.idUsuario == 0 ? (object)null : retiros.idUsuario);
        //            var result = db_.QueryMultiple("SP_CONSULTA_RETIROS", parameters, commandType: CommandType.StoredProcedure);
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

    }
}