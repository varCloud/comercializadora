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
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace lluviaBackEnd.DAO
{
    public class LoginDAO
    {

        private IDbConnection db = null;


        public Notificacion<Sesion> ValidaUsuario(Sesion sesion)
        {
            Notificacion<Sesion> n = null;
            try
            {
              
               n = new Notificacion<Sesion>();
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@usuario", sesion.usuario);
                    parameters.Add("@contrasena",sesion.contrasena);
                    var result = db.QueryMultiple("SP_VALIDA_CONTRASENA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        n.Estatus = 200;
                        n.Mensaje = "OK";
                        n.Modelo = result.ReadSingle<Sesion>();
                    }
                    else {
                        n.Estatus = -1;
                        n.Mensaje = "Datos incorrectos";
                        n.Modelo = sesion;
                    }

                    //if (db.DataReader.Read())
                    //{
                    //    if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                    //    {
                    //        sesion.idUsuario = Convert.ToInt32(db.DataReader["idUsuario"]);
                    //        sesion.idRol = Convert.ToInt32(db.DataReader["idRol"]);
                    //        sesion.nombre = db.DataReader["nombre"].ToString();
                    //        sesion.telefono = db.DataReader["telefono"].ToString();
                    //        sesion.contrasena = db.DataReader["contrasena"].ToString();
                    //        sesion.idAlmacen = Convert.ToInt32(db.DataReader["idAlmacen"]);
                    //        sesion.idSucursal = Convert.ToInt32(db.DataReader["idSucursal"]);
                    //        sesion.usuarioValido = true;
                    //        n.Estatus = 200;
                    //        n.Mensaje = "OK";
                    //        n.Modelo = sesion;

                    //    }
                    //    else
                    //    {
                    //        n.Estatus = -1;
                    //        n.Mensaje = "Datos incorrectos";
                    //        n.Modelo = sesion;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return n;
        }





    }
}