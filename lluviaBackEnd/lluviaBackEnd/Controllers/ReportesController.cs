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
            //Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            //notificacion = new ReportesDAO().ObtenerInventario(new Models.Producto() { idProducto = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            //ViewBag.lstUnidadMedida = new LineaProductoDAO().ObtenerUnidadesMedidas();
            ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0);
            //ViewBag.lstProductos = notificacion.Modelo;
            return View();
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
            Notificacion<List<Compras>> notificacion = new Notificacion<List<Compras>>();
            notificacion = new ComprasDAO().ObtenerCompras(new Models.Compras() { idCompra = 0 },true);
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstProveedores = new ProveedorDAO().ObtenerProveedores(0); // todos los proovedores en formato select list
            ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0); // todos los usuarios en formato select list
            ViewBag.lstEstatusProducto = new ProductosDAO().ObtenerEstatusProductoCompra(); // todos los estatus en formato select list
            ViewBag.lstCompras = notificacion.Estatus==200 ?  notificacion.Modelo : new List<Compras>();
            return View();
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
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();
            notificacion = new ReportesDAO().ObtenerVentas(new Models.Ventas() { idVenta = 0 });
            ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos();
            ViewBag.lstClientes = new UsuarioDAO().ObtenerClientes(0);
            ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
            ViewBag.lstVentas = notificacion.Modelo;
            return View();
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
            List<SelectListItem> listProveedores = new ProveedorDAO().ObtenerProveedores(0).Where(x => x.Value != "0").ToList();
            ViewBag.listProveedores = listProveedores;
            return View();
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


    }


}
