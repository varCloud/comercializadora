using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.Controllers
{
    public class ComprasController : Controller
    {
        // GET: Compras
        public ActionResult Compra()
        {
            Notificacion<List<Producto>> listProductos = new Notificacion<List<Producto>>();
            listProductos = new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 });
            List<SelectListItem> listProveedores = new ProveedorDAO().ObtenerProveedores(0).Where(x => x.Value != "0").ToList();
            ViewBag.listProductos = listProductos.Modelo;
            ViewBag.listProveedores = listProveedores;
            ViewBag.listStatusCompra = new ComprasDAO().ObtenerStatusCompra().Modelo;

            Compras compras = new Compras();
            return View(compras);
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
            return View();
        }


        public ActionResult _ObtenerCompras(Compras compra)
        {

            Sesion usuario = Session["UsuarioActual"] as Sesion;

            if (compra.usuario.idUsuario==0 && usuario.idRol != 1)
                compra.usuario.idUsuario = usuario.idUsuario;


            Notificacion<List<Compras>> compras = new ComprasDAO().ObtenerCompras(compra);
            return PartialView(compras);
        }
    }
}