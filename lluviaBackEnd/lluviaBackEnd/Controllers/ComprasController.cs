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
                compra.idUsuario = usuarioSesion.idUsuario;               
                Notificacion<Compras> result = new ComprasDAO().GuardarCompra(compra);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}