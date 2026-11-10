using lluviaBackEnd.Filters;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace lluviaBackEnd.Controllers
{
    /// <summary>
    /// Dashboard shell. The data sections are loaded asynchronously by DashBoardDataController.
    /// </summary>
    [SessionTimeout]
    public class DashBoardController : Controller
    {
        // GET: DashBoard
        public ActionResult index()
        {
            return View();
        }

        public ActionResult CerrarSesion()
        {
            try
            {
                FormsAuthentication.SignOut();
                Session.Abandon();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
