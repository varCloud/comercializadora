using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class ReportesController : Controller
    {
        //REPORTE INVENTARIO
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_Reportes)]
        public ActionResult Inventario()
        {
            try
            {
                //Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                //notificacion = new ReportesDAO().ObtenerInventario(new Models.Producto() { idProducto = 0 });
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos(UsuarioActual.idUsuario);
                //ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
                ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                //ViewBag.lstProductos = notificacion.Modelo;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

      

        public ActionResult BuscarInventario(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ReportesDAO().ObtenerInventario(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_Inventario");
                }
                else
                {
                    ViewBag.titulo = "Mensaje: ";
                    ViewBag.mensaje = notificacion.Mensaje;
                    return PartialView("_SinResultados");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //REPORTE COMPRAS

        public ActionResult Compras()
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Compras>> notificacion = new Notificacion<List<Compras>>();
                notificacion = new ComprasDAO().ObtenerCompras(new Models.Compras() { idCompra = 0 }, true);
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos(UsuarioActual.idUsuario);
                ViewBag.lstProveedores = new ProveedorDAO().ObtenerProveedores(0); // todos los proovedores en formato select list
                ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0); // todos los usuarios en formato select list
                ViewBag.lstEstatusProducto = new ProductosDAO().ObtenerEstatusProductoCompra(); // todos los estatus en formato select list
                ViewBag.lstCompras = notificacion.Estatus == 200 ? notificacion.Modelo : new List<Compras>();
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

       

        public ActionResult BuscarCompras(Compras compras)
        {
            try
            {

                Notificacion<List<Compras>> notificacion = new Notificacion<List<Compras>>();
                notificacion = new ComprasDAO().ObtenerCompras(compras,true);

                if (notificacion.Modelo != null)
                {
                    //ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
                    //ViewBag.lstProveedores = new ProveedorDAO().ObtenerProveedores(0); // todos los proovedores en formato select list
                    //ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0); // todos los usuarios en formato select list
                    ViewBag.lstCompras = notificacion.Modelo;
                    return PartialView("_Compras");
                }
                else
                {
                    ViewBag.titulo = "Mensaje: ";
                    ViewBag.mensaje = notificacion.Mensaje;
                    return PartialView("_SinResultados");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //REPORTE VENTAS

        public ActionResult Ventas()
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                notificacion = new ReportesDAO().ObtenerVentas(new Models.Ventas() { idVenta = 0 });
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos(UsuarioActual.idUsuario);
                ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);
                ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                ViewBag.lstVentas = notificacion.Modelo;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

      

        public ActionResult BuscarVentas(Ventas ventas)
        {
            try
            {

                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                notificacion = new ReportesDAO().ObtenerVentas(ventas);

                if (notificacion.Modelo != null)
                {
                    // ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
                    // ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);
                    // ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                    ViewBag.lstVentas = notificacion.Modelo;
                   // ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_Ventas");
                }
                else
                {
                    ViewBag.titulo = "Mensaje: ";
                    ViewBag.mensaje = notificacion.Mensaje;
                    return PartialView("_SinResultados");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //REPORTE MARGEN BRUTO

        public ActionResult MargenBruto()
        {           
            return View(new MargenBruto() { tipoMargenBruto = EnumTipoMargenBruto.Global });
        }

        public ActionResult BuscarMargenBruto(MargenBruto margenBruto)
        {
            try
            {
                Notificacion<List<MargenBruto>> notificacion = new ReportesDAO().ObtenerMargenBruto(margenBruto);
                return PartialView("_MargenBruto", notificacion);             

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //REPORTE DIAS PROMEDIO INVENTARIO
        public ActionResult DiasPromedioInventario()
        {
            return View(new DiasPromedioInventario() { tipoMargenBruto = EnumTipoMargenBruto.Global });
        }

        public ActionResult ObtenerDiasPromedioInventario(DiasPromedioInventario diasPromedioInventario)
        {
            try
            {
                Notificacion<List<DiasPromedioInventario>> notificacion = new ReportesDAO().ObtenerDiasPromedioInventario(diasPromedioInventario);
                return PartialView("_DiasPromedioInventario", notificacion);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //REPORTE DROPSIZE
        public ActionResult DropSize()
        {
            return View(new DropSize() { tipoMargenBruto = EnumTipoMargenBruto.Global });
        }

        public ActionResult ObtenerDropSize(DropSize dropSize)
        {
            try
            {
                Notificacion<List<DropSize>> notificacion = new ReportesDAO().ObtenerDropSize(dropSize);
                return PartialView("_DropSize", notificacion);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //INDICADOR NIVEL DE SERVICIO PROVEEDOR
        public ActionResult NivelServicioProveedor()
        {
            return View();
        }

        public ActionResult ObtenerNivelServicioProveedor(Proveedor proveedor)
        {
            try
            {
                List<Proveedor> proveedores = new ProveedorDAO().ObtenerProveedores(proveedor);
                return PartialView("_NivelServicioProveedor", proveedores);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //REPORTE DEVOLUCIONES PROVEEDOR

        public ActionResult DevolucionesProveedor()
        {
            try
            {
                List<SelectListItem> listProveedores = new ProveedorDAO().ObtenerProveedores(0).Where(x => x.Value != "0").ToList();
                ViewBag.listProveedores = listProveedores;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ObtenerDevolucionesProveedor(Proveedor proveedor)
        {
            try
            {
              
                return PartialView("_DevolucionesProveedor", new ReportesDAO().ObtenerDevolucionesProveedor(proveedor));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //INDICADOR MERMA

        public ActionResult Merma()
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos(usuarioSesion.idUsuario).Where(x => x.Value != "").ToList();
                ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                InventarioFisico inventarioFisico = new InventarioFisico();
                inventarioFisico.Sucursal.idSucursal = usuarioSesion.idSucursal;
                inventarioFisico.EstatusInventarioFisico.idStatus = 3;
                ViewBag.listInventarioFisico = new SelectList(new InventarioFisicoDAO().ObtenerInventarioFisico(inventarioFisico), "idInventarioFisico", "Nombre").ToList();

                return View(new AjusteInventarioFisico());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ObtenerMerma(AjusteInventarioFisico ajusteInventario)
        {
            try
            {

                return PartialView("_Merma", new ReportesDAO().ObtenerMerma(ajusteInventario));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        // DEOLUCIONES DE VENTAS

        public ActionResult Devoluciones()
        {
            try
            {
                //Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                //notificacion = new ReportesDAO().ObtenerDevoluciones(new Models.Ventas() { idVenta = 0 });
                ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                List<SelectListItem> lstAlmacenes = new List<SelectListItem>();
                lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes();
                lstAlmacenes.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "0" });
                ViewBag.lstAlmacenes = lstAlmacenes;
                //ViewBag.lstVentas = notificacion.Modelo;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult BuscarDevoluciones(Ventas ventas,int tipoTicket=0)
        {
            try
            {
                Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
                notificacion = new ReportesDAO().ObtenerDevolucionesyComplementos(ventas, tipoTicket);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstVentas = notificacion.Modelo;
                    return PartialView("_Devoluciones");
                }
                else
                {
                    ViewBag.titulo = "Mensaje: ";
                    ViewBag.mensaje = notificacion.Mensaje;
                    return PartialView("_SinResultados");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DEOLUCIONES DE VENTAS

        public ActionResult Cierres()
        {
            try
            {
                //OBTENER TODOS LOS CAJEROS
                ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0 , idRol:3);
                List<SelectListItem> lstAlmacenes = new List<SelectListItem>();
                lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes();
                lstAlmacenes.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "0" });                
                ViewBag.lstAlmacenes = lstAlmacenes;
                List<Cierre> cierres = new ReportesDAO().ConsultaCierresCaja(new Cierre()).Modelo; 
                ViewBag.lstCierres = cierres;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult BuscarCierres(Cierre cierre)
        {
            try
            {
                List<Cierre> cierres = new ReportesDAO().ConsultaCierresCaja(cierre).Modelo;
                return Json(cierres, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }


}
