using AccesoDatos;
using lluviaBackEnd.Models;
using lluviaBackEnd;
//using lluviaBackEndEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace lluviaBackEndDAO
{
    public class SesionDAO
    {

        private DBManager db;


        public Sesion ValidaUsuario(Sesion sesion)
        {
            //string passwordEncriptado = EncrypSHA1.EnciptaSHA1(usuario.noUsuario + "", usuario.password).ToString().Substring(0, 30).ToUpper();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(2);
                    db.AddParameters(0, "numusuario", sesion.nombre);
                    db.AddParameters(1, "contrasena", sesion.contrasena);
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_VALIDA_CONTRASENA");
                    if (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            if (db.DataReader.NextResult())
                            {
                                while (db.DataReader.Read())
                                {
                                    sesion.idUsuario = Convert.ToInt32(db.DataReader["idUsuario"]);
                                    sesion.idRol = Convert.ToInt32(db.DataReader["idRol"]);
                                    sesion.nombre = db.DataReader["nombre"].ToString();
                                    sesion.telefono = db.DataReader["telefono"].ToString();
                                    sesion.contrasena = db.DataReader["contrasena"].ToString();
                                    sesion.idAlmacen = Convert.ToInt32(db.DataReader["idAlmacen"]);
                                    sesion.idSucursal = Convert.ToInt32(db.DataReader["idSucursal"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sesion;
        }



    }
}
