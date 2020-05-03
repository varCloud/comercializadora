using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class ComprasController : Controller
    {
        // GET: Compras
        
        public ActionResult Compra(Compras compras)
        {
            try
            {
                Notificacion<List<Producto>> listProductos = new Notificacion<List<Producto>>();
                listProductos = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });               
                ViewBag.listProductos = listProductos.Modelo;
                return View(compras);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public ActionResult _DetalleCompra(Compras compras,Boolean enableEdit=true)
        {
            try
            {
                List<SelectListItem> listProveedores = new ProveedorDAO().ObtenerProveedores(0).Where(x => x.Value != "0").ToList();
                List<SelectListItem> listEstatus = new SelectList(new ComprasDAO().ObtenerStatusCompra().Modelo, "idStatus", "descripcion").ToList();
                ViewBag.listProveedores = listProveedores;
                ViewBag.listStatusCompra = listEstatus;
                ViewBag.enableEdit = enableEdit;

                if (compras.idCompra > 0)
                {
                    Notificacion<List<Compras>> notificacion = new ComprasDAO().ObtenerCompras(compras, true);
                    if (notificacion.Estatus == 200)
                    {
                        compras = notificacion.Modelo[0];
                        //compras.producto = new Producto();
                        foreach (Compras c in notificacion.Modelo)
                            compras.listProductos.Add(c.producto);
                        compras.producto = new Producto();
                    }


                }
                return PartialView(compras);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarCompra(Compras compra)
        {
            try
            {
                Sesion usuarioSesion = Session["UsuarioActual"] as Sesion;
                compra.usuario.idUsuario = usuarioSesion.idUsuario;
                Notificacion<Compras> result = new ComprasDAO().GuardarCompra(compra);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult Compras()
        {
            try
            {
                List<SelectListItem> listProveedores = new ProveedorDAO().ObtenerProveedores(0).ToList();
                List<SelectListItem> listEstatus = new SelectList(new ComprasDAO().ObtenerStatusCompra().Modelo, "idStatus", "descripcion").ToList();
                List<SelectListItem> listUsuarios;
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                if (usuario.idRol == 1)
                {
                    listUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                }
                else
                    listUsuarios = new UsuarioDAO().ObtenerUsuarios(usuario.idUsuario).Where(x => x.Value != "0").ToList();


                ViewBag.listProveedores = listProveedores;
                ViewBag.listEstatusCompras = listEstatus;
                ViewBag.listUsuarios = listUsuarios;

                Compras compras = new Compras();
                return View(compras);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ActionResult _ObtenerCompras(Compras compra)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;

                if (compra.usuario.idUsuario == 0 && usuario.idRol != 1)
                    compra.usuario.idUsuario = usuario.idUsuario;


                Notificacion<List<Compras>> compras = new ComprasDAO().ObtenerCompras(compra);
                return PartialView(compras);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult EliminaCompra(Compras compras)
        {
            try
            {
                return Json(new ComprasDAO().EliminaCompra(compras.idCompra), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}