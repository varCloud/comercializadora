using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using lluviaBackEnd.WebServices.Modelos.Request;
using lluviaBackEnd.WebServices.Modelos.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers.InventarioAlmacen
{
    [SessionTimeout]
    public class InventarioAlmacenController : Controller
    {
        private const string MensajeSinPermiso = "No tiene permiso para agregar inventario a un almacen";
        private const string MensajeInventarioFisico = "No es posible modificar el inventario mientras exista un inventario fisico activo";
        // Estatus del SP de ubicaciones: 1 = sin acomodar, 2 = acomodado, 3 = todo
        private const int EstatusProductoTodo = 3;

        // [PermisoAttribute] no bloquea el acceso por URL (su chequeo esta comentado), por eso cada accion
        // valida aqui el permiso Puede_visualizar_Inventario contra la sesion. Devuelve null si no lo tiene.
        private Sesion ObtenerSesionConPermiso()
        {
            Sesion sesion = Session["UsuarioActual"] as Sesion;
            if (sesion == null || sesion.permisosModulo == null)
            {
                return null;
            }
            bool tienePermiso = sesion.permisosModulo.Exists(p =>
                p.idModulo == (int)EnumRolesPermisos.Puede_visualizar_Inventario && p.tienePermiso);
            return tienePermiso ? sesion : null;
        }

        private JsonResult RespuestaSinPermiso()
        {
            return Json(new Notificacion<string>() { Estatus = -1, Mensaje = MensajeSinPermiso, Modelo = "" }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult RespuestaInventarioFisico()
        {
            return Json(new Notificacion<string>() { Estatus = -1, Mensaje = MensajeInventarioFisico, Modelo = "" }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult RespuestaInvalida(string mensaje)
        {
            return Json(new Notificacion<string>() { Estatus = -1, Mensaje = mensaje, Modelo = "" }, JsonRequestBehavior.AllowGet);
        }

        // Cantidad capturada como texto con punto decimal; debe ser un numero finito > 0 que no se anule al pasar a float
        private static bool TryParseCantidad(string texto, out float cantidad)
        {
            cantidad = 0;
            double valor;
            if (!double.TryParse((texto ?? "").Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor))
            {
                return false;
            }
            if (double.IsNaN(valor) || double.IsInfinity(valor) || valor <= 0 || valor > float.MaxValue)
            {
                return false;
            }
            cantidad = (float)valor;
            return cantidad > 0;
        }

        // GET: InventarioAlmacen
        public ActionResult AgregarInventarioAlmacen()
        {
            if (ObtenerSesionConPermiso() == null)
            {
                return RedirectToAction("SinPermisos", "Login");
            }
            if (Sesion.ExisteInventarioFisicoActivo())
            {
                return RedirectToAction("InventarioFisicoActivo", "Login");
            }
            return View(new AgregarInventarioAlmacenModel());
        }

        // Catalogos de la pantalla (almacenes, proveedores, pasillos/raqs/pisos)
        [HttpPost]
        public JsonResult ObtenerCatalogos()
        {
            if (ObtenerSesionConPermiso() == null)
            {
                return RespuestaSinPermiso();
            }
            try
            {
                InventarioAlmacenCatalogosModel catalogos = new InventarioAlmacenCatalogosModel();
                catalogos.almacenes = new UsuarioDAO().ObtenerAlmacenes();
                // ObtenerProveedores agrega la opcion "-- TODOS --" (valor 0) para los filtros; aqui no aplica
                catalogos.proveedores = new ProveedorDAO().ObtenerProveedores(0).Where(p => p.Value != "0").ToList();

                Notificacion<ResponseObtenerPisoPasilloRaq> ubicaciones = new InventarioDAO().ObtenerPasilloRaqPiso();
                if (ubicaciones == null || ubicaciones.Modelo == null)
                {
                    return RespuestaInvalida("No se pudo obtener el catalogo de pasillos, raqs y pisos");
                }
                catalogos.pasillos = ubicaciones.Modelo.pasillos ?? new List<Pasillo>();
                catalogos.raqs = ubicaciones.Modelo.raqs ?? new List<Raq>();
                catalogos.pisos = ubicaciones.Modelo.pisos ?? new List<Piso>();

                return Json(new Notificacion<InventarioAlmacenCatalogosModel>() { Estatus = 200, Mensaje = "OK", Modelo = catalogos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        // Productos activos para el almacen elegido
        [HttpPost]
        public JsonResult ObtenerProductosAlmacen(int idAlmacen)
        {
            if (ObtenerSesionConPermiso() == null)
            {
                return RespuestaSinPermiso();
            }
            try
            {
                if (idAlmacen <= 0)
                {
                    return RespuestaInvalida("Selecciona el almacen");
                }
                Notificacion<List<Producto>> productos = new ProductosDAO().ObtenerProductosPorAlmacen(idAlmacen);
                Notificacion<List<InventarioAlmacenProductoModel>> notificacion = new Notificacion<List<InventarioAlmacenProductoModel>>()
                {
                    Estatus = productos.Estatus,
                    Mensaje = productos.Mensaje,
                    Modelo = (productos.Modelo ?? new List<Producto>()).Select(p => new InventarioAlmacenProductoModel
                    {
                        idProducto = p.idProducto,
                        descripcion = p.descripcion,
                        codigoBarras = p.codigoBarras,
                        unidadMedida = p.DescripcionUnidadMedida
                    }).ToList()
                };
                JsonResult json = Json(notificacion, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AgregarInventario(AgregarInventarioAlmacenModel request)
        {
            Sesion sesion = ObtenerSesionConPermiso();
            if (sesion == null)
            {
                return RespuestaSinPermiso();
            }
            try
            {
                if (Sesion.ExisteInventarioFisicoActivo())
                {
                    return RespuestaInventarioFisico();
                }
                if (request == null)
                {
                    return RespuestaInvalida("Solicitud invalida");
                }
                int idAlmacen, idProducto, idProveedor;
                float cantidad;
                if (!int.TryParse(request.idAlmacen, NumberStyles.None, CultureInfo.InvariantCulture, out idAlmacen) || idAlmacen <= 0)
                {
                    return RespuestaInvalida("Selecciona el almacen");
                }
                if (!int.TryParse(request.idProducto, NumberStyles.None, CultureInfo.InvariantCulture, out idProducto) || idProducto <= 0)
                {
                    return RespuestaInvalida("Selecciona el producto");
                }
                if (!int.TryParse(request.idProveedor, NumberStyles.None, CultureInfo.InvariantCulture, out idProveedor) || idProveedor <= 0)
                {
                    return RespuestaInvalida("Selecciona el proveedor");
                }
                if (!TryParseCantidad(request.cantidad, out cantidad))
                {
                    return RespuestaInvalida("La cantidad debe ser un numero mayor a 0");
                }

                Notificacion<string> notificacion = new InventarioDAO().AgreagrProductoInventario(new RequestAgreagrProductoInventario()
                {
                    idAlmacen = idAlmacen,
                    idProducto = idProducto,
                    idProveedor = idProveedor,
                    cantidad = cantidad,
                    idUsuario = sesion.idUsuario
                });
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        // Ubicaciones del producto en el almacen (HTML parcial). Sin permiso responde 403 para que el ajax caiga en su handler de error.
        [HttpPost]
        public ActionResult _ObtenerUbicaciones(int idAlmacen, int idProducto)
        {
            Sesion sesion = ObtenerSesionConPermiso();
            if (sesion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, MensajeSinPermiso);
            }
            try
            {
                Notificacion<List<Ubicacion>> notificacion = new Notificacion<List<Ubicacion>>() { Estatus = -1, Mensaje = "Selecciona el almacen y el producto", Modelo = new List<Ubicacion>() };
                if (idAlmacen > 0 && idProducto > 0)
                {
                    notificacion = ObtenerUbicaciones(idAlmacen, idProducto, sesion.idUsuario);
                    CompletarDescripcionesUbicaciones(notificacion, idAlmacen, idProducto);
                }
                return PartialView("_ObtenerUbicaciones", notificacion);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ActualizarUbicacion(ActualizarUbicacionInventarioAlmacenModel request)
        {
            Sesion sesion = ObtenerSesionConPermiso();
            if (sesion == null)
            {
                return RespuestaSinPermiso();
            }
            try
            {
                if (Sesion.ExisteInventarioFisicoActivo())
                {
                    return RespuestaInventarioFisico();
                }
                if (request == null)
                {
                    return RespuestaInvalida("Solicitud invalida");
                }
                float cantidad;
                if (request.idAlmacen <= 0 || request.idProducto <= 0 || request.idUbicacion <= 0)
                {
                    return RespuestaInvalida("Solicitud invalida");
                }
                if (request.idPasillo <= 0 || request.idRaq <= 0 || request.idPiso <= 0)
                {
                    return RespuestaInvalida("Selecciona el pasillo, el raq y el piso");
                }
                if (!TryParseCantidad(request.cantidad, out cantidad))
                {
                    return RespuestaInvalida("La cantidad debe ser un numero mayor a 0");
                }

                // La ubicacion origen debe pertenecer al producto/almacen y no se puede mover mas de lo que tiene
                Notificacion<List<Ubicacion>> actuales = ObtenerUbicaciones(request.idAlmacen, request.idProducto, sesion.idUsuario);
                Ubicacion origen = (actuales.Modelo ?? new List<Ubicacion>()).FirstOrDefault(u => u.idUbicacion == request.idUbicacion);
                if (origen == null)
                {
                    return RespuestaInvalida("La ubicacion indicada no corresponde al producto en el almacen seleccionado");
                }
                if (cantidad > origen.cantidad)
                {
                    return RespuestaInvalida("La cantidad a acomodar no puede ser mayor a la cantidad disponible en la ubicacion");
                }

                Notificacion<string> notificacion = new InventarioDAO().ActualizarUbicacionInventario(new RequestActualizarUbicacionInventario()
                {
                    idAlmacen = request.idAlmacen,
                    idProducto = request.idProducto,
                    IdUbicacion = request.idUbicacion,
                    idPasillo = request.idPasillo,
                    idRack = request.idRaq,
                    idPiso = request.idPiso,
                    cantidad = cantidad,
                    idUsuario = sesion.idUsuario
                });
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(WsUtils<String>.RegresaExcepcion(ex, "Ocurrio un error: "), JsonRequestBehavior.AllowGet);
            }
        }

        // El SP puede regresar vacias las descripciones; se completan desde el catalogo (solo para mostrar).
        // Nunca deja una descripcion en blanco: si no se encuentra se usa "Producto #id" / "#id".
        private void CompletarDescripcionesUbicaciones(Notificacion<List<Ubicacion>> notificacion, int idAlmacen, int idProducto)
        {
            if (notificacion == null || notificacion.Modelo == null || notificacion.Modelo.Count == 0)
            {
                return;
            }

            string descripcionProducto = null;
            try
            {
                Notificacion<List<Producto>> productos = new ProductosDAO().ObtenerProductosPorAlmacen(idAlmacen);
                Producto p = (productos != null && productos.Modelo != null) ? productos.Modelo.FirstOrDefault(x => x.idProducto == idProducto) : null;
                descripcionProducto = p != null ? p.descripcion : null;
            }
            catch (Exception)
            {
                // Solo afecta la etiqueta mostrada
            }

            Dictionary<int, string> pasillos = new Dictionary<int, string>();
            Dictionary<int, string> raqs = new Dictionary<int, string>();
            Dictionary<int, string> pisos = new Dictionary<int, string>();
            try
            {
                Notificacion<ResponseObtenerPisoPasilloRaq> catalogo = new InventarioDAO().ObtenerPasilloRaqPiso();
                if (catalogo != null && catalogo.Modelo != null)
                {
                    foreach (Pasillo x in catalogo.Modelo.pasillos ?? new List<Pasillo>()) { pasillos[x.idPasillo] = x.descripcion; }
                    foreach (Raq x in catalogo.Modelo.raqs ?? new List<Raq>()) { raqs[x.idRaq] = x.descripcion; }
                    foreach (Piso x in catalogo.Modelo.pisos ?? new List<Piso>()) { pisos[x.idPiso] = x.descripcion; }
                }
            }
            catch (Exception)
            {
                // Se usan los ids como respaldo
            }

            foreach (Ubicacion u in notificacion.Modelo)
            {
                if (string.IsNullOrWhiteSpace(u.descripcionProducto))
                {
                    u.descripcionProducto = !string.IsNullOrWhiteSpace(descripcionProducto) ? descripcionProducto.Trim() : "Producto #" + idProducto;
                }
                if (string.IsNullOrWhiteSpace(u.descripcionAlmacen))
                {
                    u.descripcionAlmacen = "Almacén #" + idAlmacen;
                }
                u.descripcionPasillo = ResolverDescripcion(u.descripcionPasillo, pasillos, u.idPasillo);
                u.descripcionRaq = ResolverDescripcion(u.descripcionRaq, raqs, u.idRaq);
                u.descripcionPiso = ResolverDescripcion(u.descripcionPiso, pisos, u.idPiso);
            }
        }

        private static string ResolverDescripcion(string actual, Dictionary<int, string> catalogo, int id)
        {
            if (id <= 0)
            {
                return actual;
            }
            if (!string.IsNullOrWhiteSpace(actual))
            {
                return actual.Trim();
            }
            string desc;
            if (catalogo.TryGetValue(id, out desc) && !string.IsNullOrWhiteSpace(desc))
            {
                return desc.Trim();
            }
            return "#" + id;
        }

        private Notificacion<List<Ubicacion>> ObtenerUbicaciones(int idAlmacen, int idProducto, int idUsuario)
        {
            return new InventarioDAO().ObtenerUbicacionProductoInventario(new RequestObtenerUbicacionProductoInventario()
            {
                idAlmacen = idAlmacen,
                idProducto = idProducto,
                EstatusProducto = EstatusProductoTodo,
                idUsuario = idUsuario
            });
        }
    }
}
