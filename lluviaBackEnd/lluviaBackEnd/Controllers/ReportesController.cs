using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using Newtonsoft.Json;

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
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineasAlmacen(0).ToList();
                ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                ViewBag.listMeses = new ReportesDAO().ObtenerMeses(0);
                ViewBag.listAnios = new ReportesDAO().ObtenerAnios();
                /*InventarioFisico inventarioFisico = new InventarioFisico();
                inventarioFisico.Sucursal.idSucursal = usuarioSesion.idSucursal;
                inventarioFisico.EstatusInventarioFisico.idStatus = 3;*/
                //ViewBag.listInventarioFisico = new SelectList(new InventarioFisicoDAO().ObtenerInventarioFisico(inventarioFisico), "idInventarioFisico", "Nombre").ToList();
                return View(new Merma());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ObtenerMerma(Merma merma)
        {
            try
            {

                return PartialView("_Merma", new ReportesDAO().ObtenerMerma(merma));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ObtenerMesesAnio(int anio)
        {
            try
            {               
                return Json(new ReportesDAO().ObtenerMeses(anio), JsonRequestBehavior.AllowGet);
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


        public ActionResult VentasPedidosEspeciales()
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
                notificacion = new ReportesDAO().ObtenerVentasPedidosEspeciales(new Models.PedidosEspecialesV2() { idPedidoEspecial = 0 });
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


        public ActionResult BuscarVentasPedidosEspeciales(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            try
            {

                Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
                notificacion = new ReportesDAO().ObtenerVentasPedidosEspeciales(pedidosEspecialesV2);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstVentas = notificacion.Modelo;
                    return PartialView("_VentasPedidosEspeciales");
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



        public ActionResult DevolucionesPedidosEspeciales()
        {
            try
            {
                ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                List<SelectListItem> lstAlmacenes = new List<SelectListItem>();
                lstAlmacenes = new UsuarioDAO().ObtenerAlmacenes();
                lstAlmacenes.Insert(0, new SelectListItem { Text = "-- TODOS --", Value = "0" });
                ViewBag.lstAlmacenes = lstAlmacenes;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult BuscarDevolucionesPedidosEspeciales(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            try
            {
                Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();
                notificacion = new ReportesDAO().ObtenerDevolucionesPedidosEspeciales(pedidosEspecialesV2);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstVentas = notificacion.Modelo;
                    return PartialView("_DevolucionesPedidosEspeciales");
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


        public ActionResult CierresPedidosEspeciales()
        {
            try
            {
                //OBTENER TODOS LOS CAJEROS
                ViewBag.lstUsuarios = new UsuarioDAO().ObtenerUsuarios(0, idRol: 11);              
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult BuscarCierresPE(Filtro filtro)
        {
            try
            {
                List<CierrePedidosEspeciales> cierres = new ReportesDAO().ConsultaCierresPedidosEspeciales(filtro).Modelo;
                return Json(cierres, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //INDICADOR COSTO DE PRODUCCION AGRANEL

        public ActionResult CostoProduccionAgranel()
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineasAlmacen(0).ToList();
                ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                ViewBag.listMeses = new ReportesDAO().ObtenerMeses(0);
                ViewBag.listAnios = new ReportesDAO().ObtenerAnios();
                return View(new CostoProduccionAgranel());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ObtenerCostoProduccion(Merma merma)
        {
            try
            {

                return PartialView("_ObtenerCostoProduccion", new ReportesDAO().ObtenerReporteCostoProduccionAgranel(merma));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReporteGeneral(int id)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ReportesDAO().ObtenerReporteGeneral(id);

                if (notificacion.Modelo != null)
                {
                    generaCSVInventario(notificacion.Modelo, id);
                    notificacion.Estatus = 200;
                    notificacion.Mensaje = "Reporte generado correctamente.";
                }
                else
                {
                    ViewBag.titulo = "Mensaje: ";
                    ViewBag.mensaje = notificacion.Mensaje;
                    return PartialView("_SinResultados");
                }
                return Json(JsonConvert.SerializeObject(notificacion), JsonRequestBehavior.AllowGet);
                //return new EmptyResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public ActionResult generaCSVInventario(List<Producto> listaProductos, int tipo)
        {
            string nombreArchivo = tipo == 1 ? "ReporteInventarioGeneral_" : "ReporteInventarioUbicacion_";
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            nombreArchivo += dt.ToString("ddMMyyyy");

            string data = ""; 
            string header = "";

            header += "IdProducto" + ",";
            header += "Descripcion" + ",";
            header += "Ultimo Costo de Compra" + ",";
            header += "Precio Individual" + ",";
            header += "Precio Menudeo" + ",";
            header += "Cantidad" + ",";

            if (tipo == 2)
            {
                header += "IdPasillo" + ",";
                header += "IdRaq" + ",";
                header += "IdPiso" + ",";
            }

            foreach (Producto producto in listaProductos)
            {
                data += "\n";
                data += producto.idProducto + ",";

                if (producto.descripcion.Contains(","))
                {
                    producto.descripcion = "\"" + producto.descripcion.Replace("\"", "\"\"") + "\"";
                }

                data += producto.descripcion + ",";
                data += producto.ultimoCostoCompra + ",";
                data += producto.precioIndividual + ",";
                data += producto.precioMenudeo + ",";
                data += producto.cantidad + ",";
                
                if (tipo == 2) 
                { 
                    data += producto.idPasillo + ",";
                    data += producto.idRaq + ",";
                    data += producto.idPiso + ",";
                }
            }

            data = header + data;
            Response.Clear();
            Response.ContentType = "application/CSV";
            Response.AddHeader("content-disposition", "attachment; filename=\"" + nombreArchivo + ".csv\"");
            Response.Write(data);
            Response.End();
            return new EmptyResult();
        }


    }
}
