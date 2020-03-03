using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    public class ClientesController : Controller
    {
        // GET: Clientes
        public ActionResult Clientes()
        {

            ViewBag.lstClientes = this.ObtenerClientes(new Cliente() { idCliente = 0 });
            ViewBag.lstTipoClientes = this.ObtenerTipoClientes();
            return View();
        }

        [HttpPost]
        public ActionResult _ObtenerClientes()
        {
            return PartialView("_ObtenerClientes", this.ObtenerClientes(new Cliente() { idCliente = 0 }));
        }

        [HttpPost]
        public List<Cliente> ObtenerClientes(Cliente c)
        {
            return new ClienteDAO().ObtenerClientes(c);
        }

        [HttpPost]
        public ActionResult GuardarCliente(Cliente c)
        {
            try
            {
               return Json(new ClienteDAO().GuardarCliente(c), JsonRequestBehavior.AllowGet); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        [HttpPost]
        public ActionResult EliminarCliente(Cliente c)
        {
            try
            {
                return Json(new ClienteDAO().EliminarCliente(c), JsonRequestBehavior.AllowGet); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private List<TipoCliente> ObtenerTipoClientes()
        {
            return new ClienteDAO().ObtenerTipoClientes();
        }
    }
}