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
using lluviaBackEndEntidades;

namespace lluviaBackEnd.DAO
{
    public class LoginDAO
    {

        private DBManager db = null;


        public Notificacion<Sesion> ValidaUsuario(Sesion sesion)
        {
            Notificacion<Sesion> n = null;
            try
            {
                throw new Exception("error");
               n = new Notificacion<Sesion>();
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(2);
                    db.AddParameters(0, "usuario", sesion.usuario);
                    db.AddParameters(1, "contrasena", sesion.contrasena);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_VALIDA_CONTRASENA");
                    if (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            sesion.idUsuario = Convert.ToInt32(db.DataReader["idUsuario"]);
                            sesion.idRol = Convert.ToInt32(db.DataReader["idRol"]);
                            sesion.nombre = db.DataReader["nombre"].ToString();
                            sesion.telefono = db.DataReader["telefono"].ToString();
                            sesion.contrasena = db.DataReader["contrasena"].ToString();
                            sesion.idAlmacen = Convert.ToInt32(db.DataReader["idAlmacen"]);
                            sesion.idSucursal = Convert.ToInt32(db.DataReader["idSucursal"]);
                            sesion.usuarioValido = true;
                            n.Estatus = 200;
                            n.Mensaje = "OK";
                            n.Modelo = sesion;

                        }
                        else
                        {
                            n.Estatus = -1;
                            n.Mensaje = "Datos incorrectos";
                            n.Modelo = sesion;
                        }
                    }
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