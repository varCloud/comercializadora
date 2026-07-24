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
    public class ProduccionAgranelController : Controller
    {
        // GET: ProduccionAgranel
        public ActionResult ProduccionAgranel()
        {
            ViewBag.estatusProduccionAgranel = new ProcesoProduccionAgranelDAO().ObtenerEstatusProduccionProcesoAgranel();
            ViewBag.lstUsuarios = new List<SelectListItem>();
            List<Usuario> _lstUsuarios = new List<Usuario>();
            _lstUsuarios = new UsuarioDAO().ObtenerUsuarios(new Usuario { idRol = 13 });
            _lstUsuarios.Insert(0, new Usuario { nombreCompleto = "TODOS", idUsuario = 0 });
            _lstUsuarios.ForEach(u => ViewBag.lstUsuarios.Add(new SelectListItem() { Value = u.idUsuario.ToString(), Text = u.nombreCompleto.ToString(), Selected = false }));
            return View(new FiltroCostoProduccionAgranel());
        }

        public ActionResult ObtenerListadoCostoProduccion(FiltroCostoProduccionAgranel f )
        {

            return PartialView("_ObtenerListadoCostoProduccion", new ProcesoProduccionAgranelDAO().ObtenerProcesoProduccion(f));
        }

        // GET: ProduccionAgranel/MPLIndividual
        [PermisoAttribute(Permiso = EnumRolesPermisos.Puede_visualizar_PRODUCCION_AGRANEL)]
        public ActionResult MPLIndividual()
        {
            ViewBag.Title = "MPL Individual";
            ViewBag.estatusProduccionAgranel = new ProcesoProduccionAgranelDAO().ObtenerEstatusProduccionProcesoAgranel();
            return View(new FiltroCostoProduccionAgranel());
        }

        public ActionResult ObtenerCostoProduccionPorProducto(FiltroCostoProduccionAgranel f)
        {
            return PartialView("_ObtenerCostoProduccionPorProducto", new ProcesoProduccionAgranelDAO().ObtenerCostoProduccionPorProducto(f));
        }
    }
}