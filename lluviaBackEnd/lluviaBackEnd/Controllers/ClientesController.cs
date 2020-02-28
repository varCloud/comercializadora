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
            return View();
        }

        [HttpPost]
        public ActionResult _ObtenerClientes()
        {
            return View();
        }

        public List<Cliente> ObtenerClientes(Cliente c)
        {
            return new ClienteDAO().ObtenerClientes(c);
        }
    }
}