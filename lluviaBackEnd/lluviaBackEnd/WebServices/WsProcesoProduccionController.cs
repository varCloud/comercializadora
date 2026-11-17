using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using lluviaBackEnd.WebServices.Modelos.Request;
using lluviaBackEnd.WebServices.Modelos.Response;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace lluviaBackEnd.WebServices
{
    // Proceso Produccion por formula (app movil). Rutas: api/WsProcesoProduccion/<Action>
    public class WsProcesoProduccionController : ApiController
    {
        // Tope de lotes por produccion (regla de negocio); debe coincidir con @limiteLotes del SP y LIMITE_LOTES de la app movil.
        private const double LimiteLotes = 100;

        [HttpPost]
        public Notificacion<List<ProductoFormula>> BuscarProductosFormula(RequestBuscarProductosFormulaProcesoProduccion request)
        {
            try
            {
                if (request == null || request.idUsuario <= 0)
                    return Invalido("Usuario no valido.", new List<ProductoFormula>());
                return new ProcesoProduccionDAO().BuscarProductosFormula(request);
            }
            catch (Exception ex)
            {
                return WsUtils<List<ProductoFormula>>.RegresaExcepcion(ex, null);
            }
        }

        [HttpPost]
        public Notificacion<List<MateriaPrimaFaltante>> AgregarProcesoProduccion(RequestAgregarProcesoProduccion request)
        {
            try
            {
                if (request == null)
                    return Invalido("Solicitud no valida.", new List<MateriaPrimaFaltante>());
                if (request.idProducto <= 0)
                    return Invalido("Producto no valido.", new List<MateriaPrimaFaltante>());
                if (request.lotes <= 0 || double.IsNaN(request.lotes) || double.IsInfinity(request.lotes))
                    return Invalido("Los lotes deben ser mayores a cero.", new List<MateriaPrimaFaltante>());
                if (request.lotes > LimiteLotes)
                    return Invalido("El numero de lotes no puede ser mayor a 100.", new List<MateriaPrimaFaltante>());
                if (request.idUsuario <= 0)
                    return Invalido("Usuario no valido.", new List<MateriaPrimaFaltante>());
                if (request.idAlmacen <= 0)
                    return Invalido("Almacen no valido.", new List<MateriaPrimaFaltante>());
                return new ProcesoProduccionDAO().AgregarProcesoProduccion(request);
            }
            catch (Exception ex)
            {
                return WsUtils<List<MateriaPrimaFaltante>>.RegresaExcepcion(ex, null);
            }
        }

        [HttpPost]
        public Notificacion<List<ProcesoProduccionHistorial>> ObtenerHistorialProcesoProduccion(RequestObtenerHistorialProcesoProduccion request)
        {
            try
            {
                if (request == null || request.idUsuario <= 0)
                    return Invalido("Usuario no valido.", new List<ProcesoProduccionHistorial>());
                return new ProcesoProduccionDAO().ObtenerHistorialProcesoProduccion(request);
            }
            catch (Exception ex)
            {
                return WsUtils<List<ProcesoProduccionHistorial>>.RegresaExcepcion(ex, null);
            }
        }

        // Valida el NIP compartido de la formula (el NIP vive en BD como hash; no se registra ni se devuelve).
        // Solo devuelve el estatus; con NIP correcto el servidor deja una autorizacion corta para ObtenerFormulaProducto.
        [HttpPost]
        public Notificacion<string> ValidarNipFormula(RequestValidarNipFormula request)
        {
            try
            {
                if (request == null || request.idUsuario <= 0)
                    return Invalido("Usuario no valido.", "");
                if (string.IsNullOrEmpty(request.nip))
                    return Invalido("Ingrese el NIP.", "");
                return new ProcesoProduccionDAO().ValidarNipFormula(request);
            }
            catch (Exception)
            {
                // No se usa RegresaExcepcion para no filtrar detalles ni registrar la solicitud con el NIP.
                return Invalido("Ocurrio un error al validar el NIP.", "");
            }
        }

        // Formula (materias primas por lote) de un producto. Exige una validacion de NIP previa y reciente (un solo uso, se verifica en BD);
        // en cualquier error Modelo va vacio. No se usa RegresaExcepcion para no registrar datos de la formula.
        [HttpPost]
        public Notificacion<List<MateriaPrimaFormula>> ObtenerFormulaProducto(RequestObtenerFormulaProducto request)
        {
            try
            {
                if (request == null || request.idUsuario <= 0)
                    return Invalido("Usuario no valido.", new List<MateriaPrimaFormula>());
                if (request.idProducto <= 0)
                    return Invalido("Producto no valido.", new List<MateriaPrimaFormula>());
                return new ProcesoProduccionDAO().ObtenerFormulaProducto(request);
            }
            catch (Exception)
            {
                return Invalido("Ocurrio un error al obtener la formula.", new List<MateriaPrimaFormula>());
            }
        }

        private static Notificacion<T> Invalido<T>(string mensaje, T modelo) where T : class
        {
            return new Notificacion<T> { Estatus = -1, Mensaje = mensaje, Modelo = modelo };
        }
    }
}
