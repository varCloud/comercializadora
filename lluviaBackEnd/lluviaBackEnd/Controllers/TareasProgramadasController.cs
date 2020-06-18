using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    public class TareasProgramadasController : Controller
    {
        // GET: TareasProgramadas
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CierreDeInventarioDiario() {

            return Json("Ejecutando tarea programada", JsonRequestBehavior.AllowGet);
        }
    }
}