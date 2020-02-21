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

namespace lluviaBackEnd.DAO
{
    public class UsuarioDAO
    {

        private DBManager db = null;


        public List<Usuario> ObtenerUsuarios(Usuario usuario)
        {

            List<Usuario> lstUsuarios = new List<Usuario>();
            try
            {
                using (db = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db.Open();
                    db.CreateParameters(1);
                    db.AddParameters( 0, "@idRol", usuario.idRol );
                    db.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_USUARIOS]");
                    while (db.DataReader.Read())
                    {
                        if (Convert.ToInt32(db.DataReader["status"].ToString()) == 200)
                        {
                            if (db.DataReader.NextResult())
                            {
                                while (db.DataReader.Read())
                                {
                                    Usuario u = new Usuario();
                                    //u.idUsuario = Convert.ToInt32(db.DataReader["idSocio"]);
                                    u.nombre = db.DataReader["nombre"].ToString();
                                    u.Sucursal = db.DataReader["Apellidos"].ToString();
                                    u.Almacen = db.DataReader["mail"].ToString();
                                    u.Rol = db.DataReader["telefono"].ToString();
                                    lstUsuarios.Add(u);
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
            return lstUsuarios;
        }







    }
}