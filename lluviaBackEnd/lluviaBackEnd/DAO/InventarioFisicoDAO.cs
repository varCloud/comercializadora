using Dapper;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.DAO
{
    public class InventarioFisicoDAO
    {
        private IDbConnection db;

        public Notificacion<string> InsertaInventarioFisico(InventarioFisico inventarioFisico)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idInventarioFisico", inventarioFisico.idInventarioFisico);
                    parameters.Add("@nombre", inventarioFisico.Nombre);
                    parameters.Add("@idUsuario", inventarioFisico.Usuario.idUsuario);
                    parameters.Add("@activo", inventarioFisico.Activo);
                    notificacion = db.QuerySingle<Notificacion<string>>("SP_INSERTA_ACTUALIZA_INVENTARIO_FISICO", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public List<InventarioFisico> ObtenerInventarioFisico()
        {
            List<InventarioFisico> inventarioFisicos = new List<InventarioFisico>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_CONSULTA_INVENTARIO_FISICO", null, commandType: CommandType.StoredProcedure);
                    var rs1 = result.ReadFirst();
                    if (rs1.status == 200)
                    {                        
                        inventarioFisicos = result.Read<InventarioFisico, Sucursal, Usuario,InventarioFisico>(MapInventarioFisico, splitOn: "idSucursal,idUsuario").ToList();
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return inventarioFisicos;
        }

        public InventarioFisico MapInventarioFisico(InventarioFisico i,Sucursal s,Usuario u)
        {
            i.Sucursal = s;
            i.Usuario = u;
            return i;
        }   

    }
}