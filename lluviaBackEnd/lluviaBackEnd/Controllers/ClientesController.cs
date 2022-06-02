using lluviaBackEnd.DAO;
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
    public class ClientesController : Controller
    {
        // GET: Clientes
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_Clientes)]
        public ActionResult Clientes()
        {
            try
            {
                ViewBag.lstRegimens = this.ObtenerRegimens();
                ViewBag.lstClientes = this.ObtenerClientes(new Cliente() { idCliente = 0 });
                List<TipoCliente> lst = this.ObtenerTipoClientes(new TipoCliente() { idTipoCliente = 0 });
                ViewBag.lstTipoClientes = lst;
                List<SelectListItem> listTipoClientes = new List<SelectListItem>();
                lst.ForEach(x => listTipoClientes.Add(new SelectListItem() { Text = x.descripcion, Value = (x.idTipoCliente.ToString().Equals("0") ? "" : x.idTipoCliente.ToString()) }));
                ViewBag.lstTipoClientes = listTipoClientes;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult _ObtenerClientes()
        {
            try
            {
                return PartialView("_ObtenerClientes", this.ObtenerClientes(new Cliente() { idCliente = 0 }));

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public List<Cliente> ObtenerClientes(Cliente c)
        {
            try
            {
                return new ClienteDAO().ObtenerClientes(c);
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }

        [HttpPost]
        public ActionResult ObtenerCliente(Cliente c)
        {
            try
            {
                List<Cliente> lstClientes = this.ObtenerClientes(c);
                Notificacion<Cliente> n = new Notificacion<Cliente>();
                if (lstClientes == null)
                {
                    n.Estatus = -1;
                    n.Mensaje = "Espere un momento y vuelva a intentar";

                }
                else
                {
                    n.Estatus = 200;
                    n.Modelo = lstClientes.FirstOrDefault();
                }

                return Json(n, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarCliente(Cliente c)
        {
            try
            {
                return Json(new ClienteDAO().GuardarCliente(c), JsonRequestBehavior.AllowGet);
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
        private List<TipoCliente> ObtenerTipoClientes(TipoCliente tipoCliente)
        {
            try
            {
                return new ClienteDAO().ObtenerTipoClientes(tipoCliente);
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        private List<SelectListItem> ObtenerRegimens()
        {
            try
            {
                return new ClienteDAO().ObteneRegimenFiscal();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// DESCUENTOS
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Descuentos()
        {
            try
            {
                Notificacion<List<TipoCliente>> notificacion = new Notificacion<List<TipoCliente>>();
                notificacion = new ClienteDAO().ObtenerTiposClientes(new TipoCliente() { idTipoCliente = 0 });
                ViewBag.LstTiposClientes = notificacion.Modelo;
                return View();
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        [HttpPost]
        public ActionResult _ObtenerTiposClientes(int idTipoCliente)
        {
            try
            {
                Notificacion<List<TipoCliente>> notificacion = new Notificacion<List<TipoCliente>>();
                notificacion = new ClienteDAO().ObtenerTiposClientes(new TipoCliente { idTipoCliente = idTipoCliente });
                ViewBag.lstTiposClientes = notificacion.Modelo;
                return PartialView("_ObtenerTiposClientes");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ObtenerTipoCliente(int idTipoCliente)
        {
            try
            {
                Notificacion<List<TipoCliente>> notificacion = new Notificacion<List<TipoCliente>>();
                notificacion = new ClienteDAO().ObtenerTiposClientes(new TipoCliente() { idTipoCliente = idTipoCliente });
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GuardarTipoCliente(TipoCliente tipoCliente)
        {
            try
            {
                return Json(new ClienteDAO().GuardarTipoCliente(tipoCliente), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult EliminarTipoCliente(int idTipoCliente)
        {
            try
            {
                Notificacion<TipoCliente> notificacion = new Notificacion<TipoCliente>();
                notificacion = new ClienteDAO().EliminarTipoCliente(new TipoCliente() { idTipoCliente = idTipoCliente });
                return Json(notificacion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}