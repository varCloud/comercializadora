using Dapper;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace lluviaBackEnd.DAO
{
    public class ProcesoProduccionFormulaDAO
    {
        private IDbConnection db;

        public Notificacion<List<ProcesoProduccionFormulaModel>> ObtenerFormulas()
        {
            Notificacion<List<ProcesoProduccionFormulaModel>> notificacion = new Notificacion<List<ProcesoProduccionFormulaModel>>();
            using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var result = db.QueryMultiple("SP_OBTENER_PROCESO_PRODUCCION_FORMULAS", null, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                notificacion.Estatus = LeerEstatus(r1);
                notificacion.Mensaje = LeerMensaje(r1);
                // Sin formulas el listado se muestra vacio (no nulo) para poder pintar la tabla y el boton de agregar
                notificacion.Modelo = notificacion.Estatus == 200
                    ? result.Read<ProcesoProduccionFormulaModel>().ToList()
                    : new List<ProcesoProduccionFormulaModel>();
            }
            return notificacion;
        }

        // Catalogos del formulario. Si idProcesoProduccionFormula > 0 (editar / ver) incluye el producto de esa
        // formula entre los productos a producir y devuelve sus materias primas.
        public Notificacion<ProcesoProduccionFormulaCatalogosModel> ObtenerCatalogos(long idProcesoProduccionFormula)
        {
            Notificacion<ProcesoProduccionFormulaCatalogosModel> notificacion = new Notificacion<ProcesoProduccionFormulaCatalogosModel>();
            using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idProcesoProduccionFormula", idProcesoProduccionFormula > 0 ? (object)idProcesoProduccionFormula : null);
                var result = db.QueryMultiple("SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_FORMULA", parameters, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                notificacion.Estatus = LeerEstatus(r1);
                notificacion.Mensaje = LeerMensaje(r1);
                if (notificacion.Estatus == 200)
                {
                    notificacion.Modelo = new ProcesoProduccionFormulaCatalogosModel();
                    notificacion.Modelo.productos = result.Read<ProcesoProduccionFormulaProductoModel>().ToList();
                    notificacion.Modelo.materiasPrimas = result.Read<ProcesoProduccionFormulaProductoModel>().ToList();
                }
            }

            if (notificacion.Estatus == 200 && idProcesoProduccionFormula > 0)
            {
                Notificacion<List<ProcesoProduccionFormulaDetalleModel>> detalle = ObtenerDetalle(idProcesoProduccionFormula);
                if (detalle.Estatus != 200)
                {
                    notificacion.Estatus = detalle.Estatus;
                    notificacion.Mensaje = detalle.Mensaje;
                    notificacion.Modelo = null;
                }
                else
                {
                    notificacion.Modelo.detalle = detalle.Modelo;
                }
            }
            return notificacion;
        }

        public Notificacion<List<ProcesoProduccionFormulaDetalleModel>> ObtenerDetalle(long idProcesoProduccionFormula)
        {
            Notificacion<List<ProcesoProduccionFormulaDetalleModel>> notificacion = new Notificacion<List<ProcesoProduccionFormulaDetalleModel>>();
            using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idProcesoProduccionFormula", idProcesoProduccionFormula);
                var result = db.QueryMultiple("SP_OBTENER_PROCESO_PRODUCCION_FORMULA_DETALLE", parameters, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                notificacion.Estatus = LeerEstatus(r1);
                notificacion.Mensaje = LeerMensaje(r1);
                if (notificacion.Estatus == 200)
                {
                    notificacion.Modelo = result.Read<ProcesoProduccionFormulaDetalleModel>().ToList();
                }
            }
            return notificacion;
        }

        // Alta (idProcesoProduccionFormula = 0) o edicion. Las validaciones de negocio definitivas viven en el SP;
        // aqui solo se valida lo necesario para armar el XML con valores correctos.
        public Notificacion<string> GuardarFormula(ProcesoProduccionFormulaGuardarModel request, int idUsuario)
        {
            Notificacion<string> notificacion = new Notificacion<string>();

            int idProducto;
            double cantidadLote;
            if (!int.TryParse(request.idProducto, NumberStyles.Integer, CultureInfo.InvariantCulture, out idProducto) || idProducto <= 0)
            {
                return Rechazo("Selecciona el producto a producir");
            }
            if (!TryParseCantidad(request.cantidadLote, out cantidadLote))
            {
                return Rechazo("La cantidad del lote debe ser mayor a 0");
            }

            List<ProcesoProduccionFormulaMateriaPrimaRequest> materias = request.materiasPrimas ?? new List<ProcesoProduccionFormulaMateriaPrimaRequest>();
            if (materias.Count == 0)
            {
                return Rechazo("Agrega al menos una materia prima");
            }
            if (materias.GroupBy(m => m.idProducto).Any(g => g.Count() > 1))
            {
                return Rechazo("Una materia prima no puede repetirse en la formula");
            }

            XElement raiz = new XElement("materiasPrimas");
            foreach (ProcesoProduccionFormulaMateriaPrimaRequest m in materias)
            {
                double cantidad;
                if (m.idProducto <= 0 || !TryParseCantidad(m.cantidad, out cantidad))
                {
                    return Rechazo("La cantidad de cada materia prima debe ser mayor a 0");
                }
                raiz.Add(new XElement("materiaPrima",
                    new XAttribute("idProducto", m.idProducto.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("cantidad", cantidad.ToString("R", CultureInfo.InvariantCulture))));
            }

            using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idProcesoProduccionFormula", request.idProcesoProduccionFormula);
                parameters.Add("@idProducto", idProducto);
                parameters.Add("@cantidadLote", cantidadLote);
                parameters.Add("@idUsuario", idUsuario);
                parameters.Add("@xmlMateriasPrimas", raiz.ToString(SaveOptions.DisableFormatting), DbType.Xml);
                var result = db.QueryMultiple("SP_AGREGA_ACTUALIZA_PROCESO_PRODUCCION_FORMULA", parameters, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                notificacion.Estatus = LeerEstatus(r1);
                notificacion.Mensaje = LeerMensaje(r1);
                notificacion.Modelo = "";
            }
            return notificacion;
        }

        public Notificacion<string> DesactivarFormula(long idProcesoProduccionFormula, int idUsuario)
        {
            Notificacion<string> notificacion = new Notificacion<string>();
            using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@idProcesoProduccionFormula", idProcesoProduccionFormula);
                parameters.Add("@idUsuario", idUsuario);
                var result = db.QueryMultiple("SP_DESACTIVAR_PROCESO_PRODUCCION_FORMULA", parameters, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                notificacion.Estatus = LeerEstatus(r1);
                notificacion.Mensaje = LeerMensaje(r1);
                notificacion.Modelo = "";
            }
            return notificacion;
        }

        // Historial de producciones registradas (pantalla "Produccion"). Fechas vacias => el SP aplica los ultimos 30 dias.
        public Notificacion<ProcesoProduccionConsultaResultadoModel> ObtenerHistorial(ProcesoProduccionConsultaFiltroModel filtro)
        {
            Notificacion<ProcesoProduccionConsultaResultadoModel> notificacion = new Notificacion<ProcesoProduccionConsultaResultadoModel>();
            filtro = filtro ?? new ProcesoProduccionConsultaFiltroModel();

            DateTime? fechaIni, fechaFin;
            if (!TryParseFecha(filtro.fechaIni, out fechaIni) || !TryParseFecha(filtro.fechaFin, out fechaFin))
            {
                notificacion.Estatus = -1;
                notificacion.Mensaje = "El rango de fechas no es valido";
                return notificacion;
            }
            if (fechaIni.HasValue && fechaFin.HasValue && fechaFin.Value < fechaIni.Value)
            {
                notificacion.Estatus = -1;
                notificacion.Mensaje = "La fecha final no puede ser menor a la fecha inicial";
                return notificacion;
            }

            using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@fechaIni", fechaIni.HasValue ? (object)fechaIni.Value : null, DbType.Date);
                parameters.Add("@fechaFin", fechaFin.HasValue ? (object)fechaFin.Value : null, DbType.Date);
                parameters.Add("@idProducto", filtro.idProducto.HasValue && filtro.idProducto.Value > 0 ? (object)filtro.idProducto.Value : null);
                parameters.Add("@idAlmacen", filtro.idAlmacen.HasValue && filtro.idAlmacen.Value > 0 ? (object)filtro.idAlmacen.Value : null);
                var result = db.QueryMultiple("SP_OBTENER_PROCESO_PRODUCCION_FORMULA_HISTORIAL", parameters, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                notificacion.Estatus = LeerEstatus(r1);
                notificacion.Mensaje = LeerMensaje(r1);
                if (notificacion.Estatus == 200)
                {
                    notificacion.Modelo = new ProcesoProduccionConsultaResultadoModel();
                    notificacion.Modelo.producciones = result.Read<ProcesoProduccionConsultaModel>().ToList();
                    notificacion.Modelo.detalle = result.Read<ProcesoProduccionConsultaDetalleModel>().ToList();
                }
            }
            return notificacion;
        }

        // Productos con formula para el filtro de producto de la pantalla "Produccion".
        public Notificacion<List<ProcesoProduccionConsultaProductoModel>> ObtenerProductosHistorial()
        {
            Notificacion<List<ProcesoProduccionConsultaProductoModel>> notificacion = new Notificacion<List<ProcesoProduccionConsultaProductoModel>>();
            using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var result = db.QueryMultiple("SP_OBTENER_PRODUCTOS_PROCESO_PRODUCCION_HISTORIAL", null, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                notificacion.Estatus = LeerEstatus(r1);
                notificacion.Mensaje = LeerMensaje(r1);
                notificacion.Modelo = notificacion.Estatus == 200
                    ? result.Read<ProcesoProduccionConsultaProductoModel>().ToList()
                    : new List<ProcesoProduccionConsultaProductoModel>();
            }
            return notificacion;
        }

        // Vacio => null (el SP usa su rango por defecto); con valor debe ser yyyy-MM-dd.
        private static bool TryParseFecha(string texto, out DateTime? fecha)
        {
            fecha = null;
            if (string.IsNullOrWhiteSpace(texto)) return true;
            DateTime d;
            if (!DateTime.TryParseExact(texto.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) return false;
            fecha = d.Date;
            return true;
        }

        private static Notificacion<string> Rechazo(string mensaje)
        {
            return new Notificacion<string>() { Estatus = -1, Mensaje = mensaje, Modelo = "" };
        }

        private static bool TryParseCantidad(string texto, out double valor)
        {
            valor = 0;
            if (string.IsNullOrWhiteSpace(texto)) return false;
            return double.TryParse(texto.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out valor)
                && valor > 0 && !double.IsInfinity(valor);
        }

        // El contrato fija la primera fila como "status, mensaje"; se acepta tambien "estatus" por si el SP usa el
        // nombre de otras pantallas del proyecto.
        private static int LeerEstatus(dynamic fila)
        {
            var d = (IDictionary<string, object>)fila;
            object v;
            if (d.TryGetValue("status", out v) || d.TryGetValue("estatus", out v))
            {
                return Convert.ToInt32(v);
            }
            return -1;
        }

        private static string LeerMensaje(dynamic fila)
        {
            var d = (IDictionary<string, object>)fila;
            object v;
            if (d.TryGetValue("mensaje", out v) && v != null)
            {
                return v.ToString();
            }
            return "";
        }
    }
}
