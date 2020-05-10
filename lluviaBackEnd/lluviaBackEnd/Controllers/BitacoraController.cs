using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    public class BitacoraController : Controller
    {
        // GET: Bitacora
        public ActionResult Bitacoras()
        {
            try
            {               
                List<SelectListItem> listEstatus = new SelectList(new BitacoraDAO().ObtenerStatusPedidosInternos().Modelo, "idStatus", "descripcion").ToList();
                List<SelectListItem> listAlmacenes= new UsuarioDAO().ObtenerAlmacenes();
                List<SelectListItem> listProductos = new SelectList(new ProductosDAO().ObtenerProductos(new Models.Producto() { idProducto = 0 }).Modelo, "idProducto", "descripcion").ToList();
                List<SelectListItem> listUsuarios;
                Sesion usuario = Session["UsuarioActual"] as Sesion;

                if (usuario.idRol == 1)
                {
                    listUsuarios = new UsuarioDAO().ObtenerUsuarios(0);
                }
                else
                    listUsuarios = new UsuarioDAO().ObtenerUsuarios(usuario.idUsuario).Where(x => x.Value != "0").ToList();


                ViewBag.listEstatusPedidosInternos = listEstatus;
                ViewBag.listAlmacenes = listAlmacenes;
                ViewBag.listUsuarios = listUsuarios;
                ViewBag.listProductos = listProductos;

   
                return View(new PedidosInternos());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult _ObtenerBitacoras(PedidosInternos pedidosInternos)
        {
            try
            {
                Sesion usuario = Session["UsuarioActual"] as Sesion;

                if (pedidosInternos.usuario.idUsuario == 0 && usuario.idRol != 1)
                    pedidosInternos.usuario.idUsuario = usuario.idUsuario;


                Notificacion<List<PedidosInternos>> p = new BitacoraDAO().ObtenerPedidosInternos(pedidosInternos);
                return PartialView(p);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}