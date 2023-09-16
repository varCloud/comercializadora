﻿using System;

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using lluviaBackEnd.Utilerias;
using Newtonsoft.Json;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class ProductosController : Controller
    {
        // GET: Productos
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_Productos)]
        public ActionResult Productos()
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                LineaProductoDAO dao = new LineaProductoDAO();
                notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0, idUsuario = UsuarioActual.idUsuario });
                ViewBag.lstLineasDeProductos = dao.ObtenerLineaProductos(UsuarioActual.idUsuario);
                ViewBag.lstClaveProdServ = dao.ObtenerClavesProductos();
                ViewBag.lstClavesUnidad = dao.ObtenerClavesUnidad();
                ViewBag.lstUnidadMedida = dao.ObtenerUnidadesMedidas();
                ViewBag.lstUnidadCompra = dao.ObtenerUnidadesCompra();

                ViewBag.lstProductos = notificacion.Modelo;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ObtenerCodigos(string cadena)
        {
            try
            {
                Dictionary<string, object> codigos = new Dictionary<string, object>();
                codigos.Add("barra", Convert.ToBase64String(Utilerias.Utils.GenerarCodigoBarras(cadena)));
                codigos.Add("qr", Convert.ToBase64String(Utilerias.Utils.GenerarQR(cadena)));
                return Json(codigos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ObtenerProductos(Producto producto)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                producto.idUsuario = UsuarioActual.idUsuario;
                notificacion = new ProductosDAO().ObtenerProductos(producto);
                Producto p = new Producto();
                p = notificacion.Modelo[0];
                return Json(p, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ObtenerProductosPorUsuario(Producto producto)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                producto.idUsuario = UsuarioActual.idUsuario;
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductosPorUsuario(producto);
                var json = Json(notificacion, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = 500000000;
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ObtenerProductosPorAlmacen(int idAlmacen)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                //producto.idUsuario = UsuarioActual.idUsuario;
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductosPorAlmacen(idAlmacen);
                var json = Json(notificacion, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = 500000000;
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ObtenerProductosPedidoEspecial(int idPedidoEspecial)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Producto producto = new Producto();
                producto.activo = true;
                producto.idUsuario = UsuarioActual.idUsuario;
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductosPorUsuario(producto, idPedidoEspecial);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ObtenerTodosLosProductos(Producto producto)
        {
            try
            {
                producto.idUsuario = 0;
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductosPorUsuario(producto);
                var json = Json(notificacion, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = 500000000;
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ObtenerProductosPorLineaProducto(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(producto);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _ObtenerProductos(Producto producto)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                producto.idUsuario = UsuarioActual.idUsuario;
                notificacion = new ProductosDAO().ObtenerProductos(producto);
                ViewBag.lstProductos = notificacion.Modelo;
                return PartialView("_ObtenerProductos");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BuscarProductos(Producto producto)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                producto.idUsuario = UsuarioActual.idUsuario;
                notificacion = new ProductosDAO().ObtenerProductos(producto);

                if (notificacion.Modelo != null)
                {
                    ViewBag.lstProductos = notificacion.Modelo;
                    return PartialView("_ObtenerProductos");
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

        [HttpPost]
        public ActionResult GuardarProductos(Producto producto)
        {
            try
            {

                Notificacion<Producto> result = new Notificacion<Producto>();
                result = new ProductosDAO().GuardarProducto(producto);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ActualizarEstatusProducto(Producto producto)
        {
            try
            {
                Notificacion<Producto> notificacion = new Notificacion<Producto>();
                notificacion = new ProductosDAO().ActualizarEstatusProducto(producto);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public ActionResult GuardarPrecios(List<Precio> precios, Producto producto)
        {
            try
            {
                Notificacion<Precio> result = new Notificacion<Precio>();
                result = new ProductosDAO().GuardarPrecios(precios, producto);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult ObtenerPrecios(Precio precio)
        {
            try
            {
                Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
                notificacion = new ProductosDAO().ObtenerPrecios(precio);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _UbicacionesProducto(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerUbicacionProducto(producto);               
                return PartialView(notificacion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult AjustaInventarioProductoUbicacion(Ubicacion ubicacion)
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<string> notificacion = new Notificacion<string>();
                notificacion = new InventarioFisicoDAO().AjustaInventarioProductoUbicacion(ubicacion, UsuarioActual.idUsuario);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult _UbicacionesProductoPrecio(Producto producto)
        {
            try
            {
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerUbicacionProducto(producto);
                return PartialView(notificacion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult _PreciosProducto(Producto producto)
        {
            try
            {
                Notificacion<List<Precio>> notificacion = new Notificacion<List<Precio>>();
                notificacion = new ProductosDAO().ObtenerPrecios(new Precio() { idProducto = producto.idProducto });
                //return Json(notificacion, JsonRequestBehavior.AllowGet);

                //Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                //notificacion = new ProductosDAO().ObtenerUbicacionProducto(producto);
                return PartialView(notificacion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult ImprimirCodigos(string articulo, string descProducto)
        {
            Notificacion<String> notificacion = new Notificacion<string>();
            try
            {
                string pathPdfCodigos = Utils.ObtnerFolderCodigos() + @"/";
                ViewBag.pdfBase64 = Convert.ToBase64String(Utils.GenerarImprimibleCodigos(pathPdfCodigos, articulo, descProducto));
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Códigos generados correctamente.";
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public ActionResult Ubicaciones(Producto producto)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                Ubicacion ubicacion = new Ubicacion();
                ubicacion.idAlmacen = usuario.idAlmacen;
                ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
                if (usuario.idRol == 1)
                    ViewBag.Almacenes = new UsuarioDAO().ObtenerAlmacenes(1, 0);
                else
                    ViewBag.Almacenes = new UsuarioDAO().ObtenerAlmacenes(1, 0).Where(x => x.Value == usuario.idAlmacen.ToString()).ToList();

                ViewBag.lstPisos = new ProductosDAO().ObtenerPisos();
                ViewBag.lstPasillos = new ProductosDAO().ObtenerPasillos();
                ViewBag.lstRacks = new ProductosDAO().ObtenerRacks();

                return View();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult ObtenerUbicacion(Ubicacion ubicacion)
        {
            Notificacion<List<Ubicacion>> notificacion = new Notificacion<List<Ubicacion>>();

            try
            {
                notificacion = new ProductosDAO().ObtenerUbicacion(ubicacion);
                return Json(notificacion, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                notificacion.Mensaje = ex.Message;
                throw ex;
            }
        }


        public ActionResult ObtenerAlmacenes(Almacen almacen)
        {
            Notificacion<List<Almacen>> notificacion = new Notificacion<List<Almacen>>();
            try
            {
                notificacion = new UsuarioDAO().ObtenerAlmacenes(almacen);
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                notificacion.Mensaje = ex.Message;
                throw ex;
            }
        }



        public ActionResult GenerarUbicaciones(List<Ubicacion> ubicaciones)
        {
            Notificacion<Dictionary<string, object>> notificacion = new Notificacion<Dictionary<string, object>>();
            try
            {
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Ubicaciones generadas correctamente.";
                string pathPdfCodigos = Utils.ObtnerFolderCodigos() + @"/";
                notificacion.Modelo = Utilerias.Utils.GenerarUbicaciones(ubicaciones, pathPdfCodigos);

                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GenerarCodigosBarras(List<Producto> productos)
        {
            Notificacion<Dictionary<string, object>> notificacion = new Notificacion<Dictionary<string, object>>();
            try
            {
                notificacion.Estatus = 200;
                notificacion.Mensaje = "Códigos generadso correctamente.";
                string pathPdfCodigos = Utils.ObtnerFolderCodigos() + @"/";
                notificacion.Modelo = Utilerias.Utils.GenerarCodigosBarras(productos, pathPdfCodigos);

                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public void EliminaArchivo(string rutaArchivo)
        {
            try
            {
                Utilerias.Utils.DeleteFile(Utilerias.Utils.ObtnerFolderCodigos() + rutaArchivo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Límites Inventario
        public ActionResult LimitesInventario()
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                LimiteInvetario limiteInventario = new LimiteInvetario();
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos(usuarioSesion.idUsuario).Where(x => x.Value != "").ToList();

                if (usuarioSesion.idRol == 1)
                    ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0);
                else
                {
                    limiteInventario.idAlmacen = usuarioSesion.idAlmacen;
                    ViewBag.listAlmacen = new UsuarioDAO().ObtenerAlmacenes(0, 0).Where(x => x.Value == usuarioSesion.idAlmacen.ToString()).ToList();
                }

                //List<SelectListItem> selectLists = new SelectList(new LimiteInventarioDAO().ObtenerEstatusLimitesInventario(), "idStatus", "descripcion").ToList();
                ViewBag.listEstatusLimitesInventario = new SelectList(new LimiteInventarioDAO().ObtenerEstatusLimitesInventario(), "idStatus", "descripcion").ToList(); ;
                return View(limiteInventario);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ObtenerLimitesInventario(LimiteInvetario limiteInvetario)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                limiteInvetario.idAlmacen = usuarioSesion.idRol != 1 && limiteInvetario.idAlmacen == 0 ? usuarioSesion.idAlmacen : limiteInvetario.idAlmacen;
                return PartialView("_LimitesInventario", new LimiteInventarioDAO().ObtenerLimitesInventario(limiteInvetario));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ActualizaLimiteInventario(LimiteInvetario limiteInvetario)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                Notificacion<string> result = new LimiteInventarioDAO().InsertaActualizaLimiteInventario(limiteInvetario, usuarioSesion.idUsuario);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        [HttpPost]
        public ActionResult ImportarLimitesInventario(List<LimiteInvetario> limiteInvetarios)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                Notificacion<string> result = new LimiteInventarioDAO().InsertaActualizaLimiteInventarioMasivo(limiteInvetarios, usuarioSesion.idUsuario);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ImportarExcel()
        {
            Notificacion<string> result = new Notificacion<string>();
            string path = "", fileName = "", extension = "";
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                LimiteInventarioDAO limiteInventarioDAO = new LimiteInventarioDAO();
                DataTable dt = new DataTable();
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    fileName = Path.GetFileName(file.FileName);
                    extension = System.IO.Path.GetExtension(fileName).ToLower();
                    string connString = "";

                    string[] validFileTypes = { ".xls", ".xlsx" };
                    path = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads"), fileName);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Uploads"));
                    }
                    if (validFileTypes.Contains(extension))
                    {
                        if (System.IO.File.Exists(path))
                        { System.IO.File.Delete(path); }
                        file.SaveAs(path);
                        //connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //dt = ConvertXSLXtoDataTable(connString, "Hoja1");
                        Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(path);
                        Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.ActiveSheet;
                        Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange;
                        List<LimiteInvetario> listProducts = new List<LimiteInvetario>();
                        try
                        {
                            for (int row = 2; row <= range.Rows.Count; row++)
                            {
                                LimiteInvetario i = new LimiteInvetario();
                                i.codigoBarras = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 1]).Text;
                                i.descripcionAlmacen = ((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 2]).Text;
                                i.minimo = Convert.ToInt32(((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 3]).Text);
                                i.maximo = Convert.ToInt32(((Microsoft.Office.Interop.Excel.Range)range.Cells[row, 4]).Text);
                                listProducts.Add(i);
                            }
                            result = limiteInventarioDAO.InsertaActualizaLimiteInventarioMasivo(listProducts, usuarioSesion.idUsuario);
                        }
                        catch (Exception ex)
                        {
                            result.Mensaje = "Ocurrio un error al importar el archivo: " + ex.Message;
                        }
                        workbook.Save();
                        application.Workbooks.Close();
                        application.Quit();

                        if (System.IO.File.Exists(path))
                        { System.IO.File.Delete(path); }


                    }
                    else
                    {
                        result.Mensaje = "Por favor cargue los archivos con el formato correcto .xls, .xlsx";
                    }
                }
                else
                    result.Mensaje = "No se ha seleccionado ningun archivo";

            }
            catch (Exception ex)
            {
                result.Mensaje = "Ocurrio un error al importar el excel" + ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CodigosDeBarras(Producto producto)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                Notificacion<List<Producto>> p = new Notificacion<List<Producto>>();
                p = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
                ViewBag.lstProductos = p.Modelo;
                ViewBag.lstLineasDeProductos = new LineaProductoDAO().ObtenerLineaProductos(usuario.idUsuario);

                //ViewBag.lstSucursales = new UsuarioDAO().ObtenerSucursales();
                //ViewBag.Almacenes = new UsuarioDAO().ObtenerAlmacenes(1, 0);
                //ViewBag.lstPisos = new ProductosDAO().ObtenerPisos();
                //ViewBag.lstPasillos = new ProductosDAO().ObtenerPasillos();
                //ViewBag.lstRacks = new ProductosDAO().ObtenerRacks();

                return View();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public ActionResult ConsultarLiquidos()
        {
            try
            {
                List<SelectListItem> listRoles = new List<SelectListItem>();
                listRoles = new UsuarioDAO().ObtenerRoles(new Models.Rol() { idRol = 1 });
                listRoles = listRoles.Where(c => (c.Value == "12") || (c.Value == "13") ).ToList();
                listRoles.Insert(0, new SelectListItem { Text = "TODOS", Value = "0" });
                List<SelectListItem> lstUsuarios = new List<SelectListItem>();
                lstUsuarios.Insert(0, new SelectListItem { Text = "TODOS", Value = "0" });
                ViewBag.lstRoles = listRoles;
                ViewBag.lstUsuarios = lstUsuarios;                
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult ConsultarUsuariosLiquidos(int idRol)
        {
            try
            {
                List<Usuario> lstUsuarios = new List<Usuario>();
                lstUsuarios = new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = idRol });

                if (idRol == 0)
                {
                    lstUsuarios = lstUsuarios.Where(c => (c.idRol == 12) || (c.idRol == 13)).ToList();
                }
                                
                lstUsuarios.Insert(0, new Usuario { nombreCompleto = "TODOS", idUsuario = 0 });
                Notificacion<List<Usuario>> notificacion = new Notificacion<List<Usuario>>();
                notificacion.Estatus = 200;
                notificacion.Modelo = lstUsuarios;
                var json = Json(notificacion, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = 500000000;
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult BuscarCargaMercanciaLiquidos(FiltroLiquidos filtroLiquidos)
        {
            try
            {
                Notificacion<dynamic> liquidos = new ProductosDAO().BuscarCargaMercanciaLiquidos(filtroLiquidos);
                return Json(JsonConvert.SerializeObject(liquidos), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public FileStreamResult reportCsv()
        {
            try
            {
                Sesion UsuarioActual = (Sesion)Session["UsuarioActual"];
                Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
                notificacion = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0, idUsuario = UsuarioActual.idUsuario });
                var result = lluviaCsvHelper.ExportToCSV(notificacion.Modelo);
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "export.csv" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
       
        }



    }


}
