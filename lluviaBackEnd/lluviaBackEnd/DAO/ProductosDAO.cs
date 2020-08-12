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
using lluviaBackEnd.WebServices.Modelos.Request;
using System.Xml;
using System.Xml.Serialization;

namespace lluviaBackEnd.DAO
{
    public class ProductosDAO
    {
        private IDbConnection db = null;
        private DBManager db1 = null;

        public Notificacion<List<Producto>> ObtenerProductos(Producto producto)
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();

            try
            {
                //if (  producto.fechaIni == (new DateTime(0001,01,01) ) )
                //{
                //    producto.fechaIni = new DateTime(1900, 01, 01);

                //}
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@descripcion", producto.descripcion);
                    parameters.Add("@idUnidadMedida", producto.idUnidadMedida);
                    parameters.Add("@idLineaProducto", producto.idLineaProducto);
                    parameters.Add("@activo", producto.activo);
                    parameters.Add("@articulo", producto.articulo);
                    parameters.Add("@claveProdServ", producto.idClaveProdServ);
                    //parameters.Add("@claveUnidad", producto.claveUnidad);

                    var result = db.QueryMultiple("SP_CONSULTA_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Producto>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo.Clear();// [0] = producto;
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public Notificacion<List<Producto>> ObtenerProductosPorUsuario(Producto producto)
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();

            try
            {
                //if (  producto.fechaIni == (new DateTime(0001,01,01) ) )
                //{
                //    producto.fechaIni = new DateTime(1900, 01, 01);

                //}
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@descripcion", producto.descripcion);
                    parameters.Add("@idUnidadMedida", producto.idUnidadMedida);
                    parameters.Add("@idLineaProducto", producto.idLineaProducto);
                    parameters.Add("@activo", producto.activo);
                    parameters.Add("@articulo", producto.articulo);
                    parameters.Add("@claveProdServ", producto.idClaveProdServ);
                    parameters.Add("@fechaIni", producto.fechaIni == DateTime.MinValue ? (object)null : producto.fechaIni);
                    parameters.Add("@fechaFin", producto.fechaFin == DateTime.MinValue ? (object)null : producto.fechaFin);
                    parameters.Add("@idUsuario", producto.idUsuario);

                    var result = db.QueryMultiple("SP_CONSULTA_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Producto>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo.Clear();// [0] = producto;
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public Notificacion<Producto> GuardarProducto(Producto producto)
        {
            Notificacion<Producto> notificacion = new Notificacion<Producto>();
            try
            {

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@descripcion", producto.descripcion);
                    parameters.Add("@idUnidadMedida", producto.idUnidadMedida);
                    parameters.Add("@idLineaProducto", producto.idLineaProducto);
                    parameters.Add("@cantidadUnidadMedida", producto.cantidadUnidadMedida);
                    parameters.Add("@codigoBarras", producto.codigoBarras);
                    parameters.Add("@activo", producto.activo);
                    parameters.Add("@articulo", producto.articulo);
                    parameters.Add("@claveProdServ", producto.idClaveProdServ);
                    //parameters.Add("@claveUnidad", producto.claveUnidad);

                    var result = db.QueryMultiple("SP_INSERTA_ACTUALIZA_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.Estatus == 200)
                    {
                        notificacion.Estatus = r1.Estatus;
                        notificacion.Mensaje = r1.Mensaje;
                        notificacion.Modelo = producto; //result.ReadSingle<Producto>();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = producto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }
        public Notificacion<Producto> ActualizarEstatusProducto(Producto producto)
        {
            Notificacion<Producto> notificacion = new Notificacion<Producto>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@activo", producto.activo);
                    var result = db.QueryMultiple("SP_ACTUALIZA_STATUS_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = producto;
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = producto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        /// <summary>
        /// es metodo se expone mediante web service
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public Notificacion<Producto> ObtenerProductoXCodigo(RequestObtenerProductoXCodigo request)
        {
            Notificacion<Producto> notificacion = new Notificacion<Producto>();

            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@codigo", request.codigo);
                    var result = db.QueryMultiple("SP_CONSULTA_PRODUCTOS_POR_CODIGO_BARRAS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.estatus == 200)
                    {
                        notificacion.Estatus = r1.estatus;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.ReadSingle<Producto>();
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


        public Notificacion<Precio> GuardarPrecios(List<Precio> precios, Producto producto)
        {
            Notificacion<Precio> notificacion = new Notificacion<Precio>();
            try
            {

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@XML", Serialize(precios));
                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@precioIndividual", producto.precioIndividual);
                    parameters.Add("@precioMenudeo", producto.precioMenudeo);

                    var result = db.QueryMultiple("SP_INSERTA_ACTUALIZA_RANGOS_PRECIOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo = precios; //result.ReadSingle<Producto>();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo = producto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<List<Precio>> ObtenerPrecios(Precio precio)
        {
            Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();

            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", precio.idProducto);
                    var result = db.QueryMultiple("SP_CONSULTA_TIPOS_DE_PRECIOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Precio>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo.Clear();// [0] = producto;
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }


        public List<Producto> ObtenerListaProductos(Producto producto)
        {
            List<Producto> lstClientes = null;
            try
            {
                //using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                //{
                //    lstClientes = this.db.Query<Producto>("SP_CONSULTA_PRODUCTOS", commandType: CommandType.StoredProcedure).ToList();
                //    lstClientes.Insert(0, new Producto() { idProducto = 0, descripcion = "--TODOS--" });
                //}

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@descripcion", producto.descripcion);
                    parameters.Add("@idUnidadMedida", producto.idUnidadMedida);
                    parameters.Add("@idLineaProducto", producto.idLineaProducto);
                    parameters.Add("@activo", producto.activo);
                    parameters.Add("@articulo", producto.articulo);
                    parameters.Add("@claveProdServ", producto.idClaveProdServ);

                    var result = db.QueryMultiple("SP_CONSULTA_PRODUCTOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        lstClientes = result.Read<Producto>().ToList();
                    }

                    lstClientes.Insert(0, new Producto() { idProducto = 0, descripcion = "--TODOS--" });

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstClientes;
        }

        public string Serialize(List<Precio> precios)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<Precio>));
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                xmlSerializer.Serialize(xmlWriter, precios);
            }

            return stringBuilder.ToString();

        }

        public Notificacion<List<Producto>> ObtenerUbicacionProducto(Producto producto)
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();

            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idProducto", producto.idProducto);
                    parameters.Add("@idSucursal", producto.idSucursal);
                    var result = db.QueryMultiple("SP_CONSULTA_PRODUCTOS_X_UBICACION", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Producto>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
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

        public List<SelectListItem> ObtenerEstatusProductoCompra()
        {
            List<SelectListItem> lstEstatus = new List<SelectListItem>();
            try
            {
                using (db1 = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db1.Open();
                    db1.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_ESTATUS_PRODUCTO_COMPRA]");
                    while (db1.DataReader.Read())
                    {
                        
                            lstEstatus.Add(
                                        new SelectListItem
                                        {
                                            Text = db1.DataReader["descripcion"].ToString(),
                                            Value = db1.DataReader["idEstatusProductoCompra"].ToString()
                                        });
                        

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (lstEstatus.Count > 0)
                lstEstatus.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "0" });

            return lstEstatus;
        }

        public Notificacion<List<Ubicacion>> ObtenerUbicacion(Ubicacion ubicacion)
        {
            Notificacion<List<Ubicacion>> notificacion = new Notificacion<List<Ubicacion>>();

            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idSucursal", ubicacion.idSucursal);
                    parameters.Add("@idAlmacen", ubicacion.idAlmacen);
                    parameters.Add("@idPasillo", ubicacion.idPasillo);
                    parameters.Add("@idRaq", ubicacion.idRaq);
                    parameters.Add("@idPiso", ubicacion.idPiso);
                    var result = db.QueryMultiple("SP_CONSULTA_UBICACION", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Ubicacion>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
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



        public List<SelectListItem> ObtenerPisos()
        {
            List<SelectListItem> lstPisos = new List<SelectListItem>();
            try
            {
                using (db1 = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db1.Open();
                    db1.CreateParameters(1);
                    db1.AddParameters(0, "@caso", 1);
                    db1.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_PASILLO_PISO_RAQ]");
                    db1.DataReader.NextResult();
                    while (db1.DataReader.Read())
                    {

                        lstPisos.Add(
                                    new SelectListItem
                                    {
                                        Text = db1.DataReader["descripcion"].ToString(),
                                        Value = db1.DataReader["id"].ToString()
                                    });


                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstPisos;
        }

        public List<SelectListItem> ObtenerPasillos()
        {
            List<SelectListItem> lstPasillos = new List<SelectListItem>();
            try
            {
                using (db1 = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db1.Open();
                    db1.CreateParameters(1);
                    db1.AddParameters(0, "@caso", 2);
                    db1.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_PASILLO_PISO_RAQ]");
                    db1.DataReader.NextResult();
                    while (db1.DataReader.Read())
                    {

                        lstPasillos.Add(
                                    new SelectListItem
                                    {
                                        Text = db1.DataReader["descripcion"].ToString(),
                                        Value = db1.DataReader["id"].ToString()
                                    });


                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstPasillos;
        }

        public List<SelectListItem> ObtenerRacks()
        {
            List<SelectListItem> lstRacks = new List<SelectListItem>();
            try
            {
                using (db1 = new DBManager(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    db1.Open();
                    db1.CreateParameters(1);
                    db1.AddParameters(0, "@caso", 3);
                    db1.ExecuteReader(System.Data.CommandType.StoredProcedure, "[SP_CONSULTA_PASILLO_PISO_RAQ]");
                    db1.DataReader.NextResult();
                    while (db1.DataReader.Read())
                    {

                        lstRacks.Add(
                                    new SelectListItem
                                    {
                                        Text = db1.DataReader["descripcion"].ToString(),
                                        Value = db1.DataReader["id"].ToString()
                                    });


                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lstRacks;
        }



    }
}