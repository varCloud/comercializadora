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
using lluviaBackEnd.Models.ProduccionProductos;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ZXing;

namespace lluviaBackEnd.DAO.ProduccionProductos
{
    public class ProduccionProductosDAO
    {
        private IDbConnection db;

        public Notificacion<string> AgregarCombinacionesProduccionProductos(ProduccionProductosModel request)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (this.db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@idRelacion", request.id );
                    param.Add("@idProductoMateria1", request.idProductoMateria1 );
                    param.Add("@idProductoProduccion", request.idProductoProduccion );
                    param.Add("@idProductoMateria2", request.idProductoMateria2);
                    param.Add("@unidadMedidad", request.unidadMedidad );
                    param.Add("@valorUnidadMedida", Convert.ToSingle(request.valorUnidadMedidaConverter));
                    param.Add("@idUnidadMedida", request.idUnidadMedidad );
                    var result = this.db.QueryMultiple("SP_AGREGA_ACTUALIZA_COMBINACION_PRODUCCION_PRODUCTOS",param, commandType: CommandType.StoredProcedure);
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
            catch (Exception exception1)
            {
                throw exception1;
            }
            return notificacion;
        }


        public Notificacion<List<ProduccionProductosModel>> ObtenerCombinaciones()
        {
            Notificacion<List<ProduccionProductosModel>> notificacion = new Notificacion<List<ProduccionProductosModel>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_OBTENER_COMBINACION_PRODUCTOS_PRODUCCION", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.estatus == 200)
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<ProduccionProductosModel>().ToList();
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

        public Notificacion<List<UnidadMedida>> ObtenerUnidadesMedidasTrapeadores()
        {
            Notificacion<List<UnidadMedida>> notificacion = new Notificacion<List<UnidadMedida>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_OBTENER_UNIDADES_DE_MEDIDA_TRAPEADORES", null, commandType: CommandType.StoredProcedure);
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

                    var result = db.QueryMultiple("SP_OBTENER_PRODUCTOS_FABRICAR_TRAPEADORES", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.estatus == 200)
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
                        List<SelectListItem> listMatra = new SelectList(result.Read<Producto>().ToList(), "idProducto", "descripcion").ToList();
                        List<SelectListItem> listTrapeadores = new SelectList(result.Read<Producto>().ToList(), "idProducto", "descripcion").ToList();
                        List<SelectListItem> listBastones = new SelectList(result.Read<Producto>().ToList(), "idProducto", "descripcion").ToList();
                        List<SelectListItem> listUnidadesMedidas = new SelectList(ObtenerUnidadesMedidasTrapeadores().Modelo, "idUnidadMedida", "descripcion").ToList();
                        listMatra.Insert(0, AgregarItemDeSelecciona());
                        listTrapeadores.Insert(0, AgregarItemDeSelecciona());
                        listBastones.Insert(0, AgregarItemDeSelecciona());
                        listUnidadesMedidas.Insert(0, AgregarItemDeSelecciona());
                        notificacion.Modelo = new Dictionary<string, object>();
                        notificacion.Modelo.Add("dataMatra", listMatra);
                        notificacion.Modelo.Add("dataTrapeadores", listTrapeadores);
                        notificacion.Modelo.Add("dataBastones", listBastones);
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
            return new SelectListItem() { Text = "-- SELECCIONA --", Value = "", Selected = true };
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
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificacion;
        }

        public Notificacion<string> DesactivarCombinacionProductosProduccionProductos(int idProductoProduccion)
        {
            Notificacion<string> notificacion2;
            Notificacion<string> notificacion = new Notificacion<string>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var param = new DynamicParameters();
                    param.Add("@idProductoProduccion", idProductoProduccion);
                    notificacion2 = db.QuerySingle<Notificacion<string>>("SP_DESACTIVAR_COMBINACION_PRODUCTOS_PRODUCCION", param , commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exception1)
            {
                throw exception1;
            }
            return notificacion2;
        }
    }
}