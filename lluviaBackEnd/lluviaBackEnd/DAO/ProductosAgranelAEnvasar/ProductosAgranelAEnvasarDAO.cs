using Dapper;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.DAO.ProductosAgranelAEnvasar
{
    public class ProductosAgranelAEnvasarDAO
    {
        private IDbConnection db;
        public Notificacion<List<ProductosAgranelAEnvasarModel>> ObtenerCombinaciones()
        {
            Notificacion<List<ProductosAgranelAEnvasarModel>> notificacion = new Notificacion<List<ProductosAgranelAEnvasarModel>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_OBTENER_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.estatus == 200)
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<ProductosAgranelAEnvasarModel>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<Dictionary<string,object>> ObtenerProductos()
        {
            Notificacion<Dictionary<string, object>> notificacion = new Notificacion<Dictionary<string, object>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_OBTENER_PRODUCTOS_ENVASES_LIQUIDOS_AGRANEL", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.estatus == 200)
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
                        List<SelectListItem> listAgranel = new SelectList(result.Read<Producto>().ToList(), "id", "descripcion").ToList();
                        List<SelectListItem> listEnvasado = new SelectList(result.Read<Producto>().ToList(), "id", "descripcion").ToList();
                        List<SelectListItem> listLiquidos = new SelectList(result.Read<Producto>().ToList(), "id", "descripcion").ToList();
                        notificacion.Modelo = new Dictionary<string, object>();
                        notificacion.Modelo.Add("dataAgranel", listAgranel);
                        notificacion.Modelo.Add("dataEnvases", listEnvasado);
                        notificacion.Modelo.Add("dataLiquidos", listLiquidos);
                    }
                    else
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
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