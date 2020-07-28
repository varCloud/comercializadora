using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class InventarioFisicoController : Controller
    {
        // GET: InventarioFisico
        public ActionResult InventarioFisico()
        {
            ViewBag.ListInventarioFisico = new List<InventarioFisico>();
            return View();
        }
    }
}