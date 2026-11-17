using Dapper;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos.Request;
using lluviaBackEnd.WebServices.Modelos.Response;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace lluviaBackEnd.DAO
{
    // Servicios web de la app movil para Proceso Produccion por formula.
    public class ProcesoProduccionDAO
    {
        public Notificacion<List<ProductoFormula>> BuscarProductosFormula(RequestBuscarProductosFormulaProcesoProduccion request)
        {
            var notificacion = new Notificacion<List<ProductoFormula>>();
            using (var db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", request.idUsuario);
                parameters.Add("@descripcion", string.IsNullOrWhiteSpace(request.descripcion) ? null : request.descripcion.Trim());
                parameters.Add("@codigoBarras", string.IsNullOrWhiteSpace(request.codigoBarras) ? null : request.codigoBarras.Trim());
                using (var rs = db.QueryMultiple("SP_APP_BUSCAR_PRODUCTOS_PROCESO_PRODUCCION_FORMULA", parameters, commandType: CommandType.StoredProcedure))
                {
                    LeerEstatus(rs.ReadFirst(), notificacion);
                    if (notificacion.Estatus == 200)
                    {
                        // La formula (materias primas) no viaja en la busqueda: solo la entrega ObtenerFormulaProducto (tras validar el NIP).
                        notificacion.Modelo = rs.Read<ProductoFormula>().ToList();
                    }
                    else
                    {
                        notificacion.Modelo = new List<ProductoFormula>();
                    }
                }
            }
            return notificacion;
        }

        public Notificacion<List<MateriaPrimaFaltante>> AgregarProcesoProduccion(RequestAgregarProcesoProduccion request)
        {
            var notificacion = new Notificacion<List<MateriaPrimaFaltante>>();
            using (var db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idProducto", request.idProducto);
                parameters.Add("@lotes", request.lotes);
                parameters.Add("@idUsuario", request.idUsuario);
                parameters.Add("@idAlmacen", request.idAlmacen);
                using (var rs = db.QueryMultiple("SP_APP_AGREGAR_PROCESO_PRODUCCION", parameters, commandType: CommandType.StoredProcedure))
                {
                    LeerEstatus(rs.ReadFirst(), notificacion);
                    notificacion.Modelo = new List<MateriaPrimaFaltante>();
                    if (notificacion.Estatus == 200)
                    {
                        // Set 2: idProcesoProduccion generado.
                        if (!rs.IsConsumed)
                        {
                            var fila = rs.ReadFirstOrDefault() as IDictionary<string, object>;
                            if (fila != null && fila.Count > 0)
                            {
                                notificacion.id = Convert.ToInt64(fila.Values.First());
                            }
                        }
                    }
                    else if (!rs.IsConsumed)
                    {
                        // Set 2: materias primas faltantes.
                        notificacion.Modelo = rs.Read<MateriaPrimaFaltante>().ToList();
                    }
                }
            }
            return notificacion;
        }

        public Notificacion<List<ProcesoProduccionHistorial>> ObtenerHistorialProcesoProduccion(RequestObtenerHistorialProcesoProduccion request)
        {
            var notificacion = new Notificacion<List<ProcesoProduccionHistorial>>();
            using (var db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", request.idUsuario);
                parameters.Add("@fechaIni", request.fechaIni.HasValue && request.fechaIni.Value != DateTime.MinValue ? (object)request.fechaIni.Value : null);
                parameters.Add("@fechaFin", request.fechaFin.HasValue && request.fechaFin.Value != DateTime.MinValue ? (object)request.fechaFin.Value : null);
                using (var rs = db.QueryMultiple("SP_APP_OBTENER_PROCESO_PRODUCCION_HISTORIAL", parameters, commandType: CommandType.StoredProcedure))
                {
                    LeerEstatus(rs.ReadFirst(), notificacion);
                    notificacion.Modelo = notificacion.Estatus == 200
                        ? rs.Read<ProcesoProduccionHistorial>().ToList()
                        : new List<ProcesoProduccionHistorial>();
                }
            }
            return notificacion;
        }

        // NIP compartido para ver la formula en la app movil. El SP valida rol, hash y bloqueo por intentos; aqui nunca se registra el NIP.
        // Solo devuelve el estatus; un NIP correcto deja una autorizacion corta en BD que consume ObtenerFormulaProducto.
        public Notificacion<string> ValidarNipFormula(RequestValidarNipFormula request)
        {
            var notificacion = new Notificacion<string>();
            using (var db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", request.idUsuario);
                parameters.Add("@nip", request.nip, DbType.AnsiString, size: 10);
                using (var rs = db.QueryMultiple("SP_APP_VALIDAR_NIP_FORMULA", parameters, commandType: CommandType.StoredProcedure))
                {
                    LeerEstatus(rs.ReadFirst(), notificacion);
                    notificacion.Modelo = "";
                }
            }
            return notificacion;
        }

        // Formula (materias primas por lote) de un producto. El SP exige y consume (un solo uso) la autorizacion que deja un NIP correcto;
        // sin ella, o en cualquier error, Modelo va vacio.
        public Notificacion<List<MateriaPrimaFormula>> ObtenerFormulaProducto(RequestObtenerFormulaProducto request)
        {
            var notificacion = new Notificacion<List<MateriaPrimaFormula>>();
            using (var db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idUsuario", request.idUsuario);
                parameters.Add("@idProducto", request.idProducto);
                using (var rs = db.QueryMultiple("SP_APP_OBTENER_FORMULA_PRODUCTO", parameters, commandType: CommandType.StoredProcedure))
                {
                    LeerEstatus(rs.ReadFirst(), notificacion);
                    notificacion.Modelo = notificacion.Estatus == 200 && !rs.IsConsumed
                        ? rs.Read<MateriaPrimaFormula>().ToList()
                        : new List<MateriaPrimaFormula>();
                }
            }
            return notificacion;
        }

        // Fila de estatus (status/estatus, mensaje) sin depender de mayusculas/minusculas del SP.
        private static void LeerEstatus<T>(dynamic fila, Notificacion<T> notificacion) where T : class
        {
            var d = (IDictionary<string, object>)fila;
            object estatus = null, mensaje = null;
            foreach (var kv in d)
            {
                var k = kv.Key.ToLowerInvariant();
                if (k == "status" || k == "estatus") estatus = kv.Value;
                else if (k == "mensaje") mensaje = kv.Value;
            }
            if (estatus == null && d.Count > 0) estatus = d.Values.First();
            if (mensaje == null && d.Count > 1) mensaje = d.Values.Skip(1).First();
            notificacion.Estatus = estatus == null ? -1 : Convert.ToInt32(estatus);
            notificacion.Mensaje = mensaje == null ? null : mensaje.ToString();
        }
    }
}
