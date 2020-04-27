using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Dapper;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.DAO
{
    public class ComprasDAO
    {
        private IDbConnection db;
        public Notificacion<Compras> GuardarCompra(Compras compra)
        {
            Notificacion<Compras> notificacion = new Notificacion<Compras>();
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(List<Producto>));
                var stringBuilder = new StringBuilder();
                using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                {
                    xmlSerializer.Serialize(xmlWriter, compra.listProductos);
                }


                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idCompra", compra.idCompra);
                    parameters.Add("@idProveedor", compra.proveedor.idProveedor);
                    parameters.Add("@idUsuario ", compra.idUsuario);
                    parameters.Add("@idStatusCompra", compra.statusCompra.idStatus);
                    parameters.Add("@productos", stringBuilder.ToString());

                    var result = db.QueryMultiple("SP_REGISTRA_COMPRA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new Compras() { idCompra = r1.idCompra };
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

        public Notificacion<List<Status>> ObtenerStatusCompra()
        {
            Notificacion<List<Status>> notificacion = new Notificacion<List<Status>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var result = db.QueryMultiple("SP_CONSULTA_ESTATUS_COMPRA", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Status>().ToList();
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


    }
}