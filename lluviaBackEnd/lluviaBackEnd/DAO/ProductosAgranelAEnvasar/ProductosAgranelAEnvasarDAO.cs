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

        public Notificacion<string> AgregarCombinacionesMPL(ProductosAgranelAEnvasarModel request   )
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idRelacionEnvasadoAgranel", request.idRelacionEnvasadoAgranel);
                    parameters.Add("@idProductoEnvasado", request.idProductoEnvasado);
                    parameters.Add("@idProductoAgranel", request.idProductoAgranel);
                    parameters.Add("@idProducoEnvase", request.idProducoEnvase);
                    parameters.Add("@unidadMedidad", request.unidadMedidad);
                    parameters.Add("@valorUnidadMedida",Convert.ToSingle(request.valorUnidadMedidaConverter));
                    parameters.Add("@idUnidadMedida", request.idUnidadMedidad);
                    var result = db.QueryMultiple("SP_AGREGA_ACTUALIZA_COMBINACION_PRODUCTOS_ENSAVDOS_A_AGRANEL", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.estatus == 200)
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = "";
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

        public Notificacion<List<UnidadMedida>> ObtenerUnidadesMedidasMPL()
        {
            Notificacion<List<UnidadMedida>> notificacion = new Notificacion<List<UnidadMedida>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_OBTENER_UNIDADES_DE_MEDIDA_LIQUIDOS_AGRANEL", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.estatus == 200)
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<UnidadMedida>().ToList();
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

        public Notificacion<Dictionary<string, object>> ObtenerProductos()
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
                        List<SelectListItem> listAgranel = new SelectList(result.Read<Producto>().ToList(), "idProducto", "descripcion").ToList();
                        List<SelectListItem> listEnvasado = new SelectList(result.Read<Producto>().ToList(), "idProducto", "descripcion").ToList();
                        List<SelectListItem> listEnvases = new SelectList(result.Read<Producto>().ToList(), "idProducto", "descripcion").ToList();
                        List<SelectListItem> listUnidadesMedidas = new SelectList(ObtenerUnidadesMedidasMPL().Modelo, "idUnidadMedida", "descripcion").ToList();
                        listAgranel.Insert(0, AgregarItemDeSelecciona());
                        listEnvasado.Insert(0, AgregarItemDeSelecciona());
                        listEnvases.Insert(0, AgregarItemDeSelecciona());
                        listUnidadesMedidas.Insert(0, AgregarItemDeSelecciona());
                        notificacion.Modelo = new Dictionary<string, object>();
                        notificacion.Modelo.Add("dataAgranel", listAgranel);
                        notificacion.Modelo.Add("dataEnvasado", listEnvasado);
                        notificacion.Modelo.Add("dataEnvases", listEnvases);
                        notificacion.Modelo.Add("dataUnidadesMedida", listUnidadesMedidas);
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

        private SelectListItem AgregarItemDeSelecciona()
        {
            return  new SelectListItem() { Text = "-- SELECCIONA --", Value = "", Selected = true };
        }

        public Notificacion<string> DesactivarCombinacionProductosEnvasadosAgranel(int idRelacionEnvasadoAgranel)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idRelacionEnvasadoAgranel", idRelacionEnvasadoAgranel);

                    notificacion = db.QuerySingle<Notificacion<string>>("SP_DESACTIVAR_COMBINACION_PRODUCTOS_ENVASADOS_A_AGRANEL", parameters, commandType: CommandType.StoredProcedure);
                    return notificacion;
                    //var r1 = result.ReadFirst();
                    //if (r1.estatus == 200)
                    //{
                    //    notificacion.Estatus = r1.estatus;
                    //    notificacion.Mensaje = r1.mensaje;
                    //}
                    //else
                    //{
                    //    notificacion.Estatus = r1.estatus;
                    //    notificacion.Mensaje = r1.mensaje;
                    //}
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