using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using lluviaBackEnd.WebServices.Modelos.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace lluviaBackEnd.WebServices
{
    public class WsProveedorController : ApiController
    {
        [HttpPost]
        public Models.Notificacion<List<Proveedor>> ObtenerProveedores(RequestObtenerProveedores request)
        {
            try
            {
                List<Proveedor> list = new ProveedorDAO().ObtenerProveedores(new Models.Proveedor() { idProveedor = request.idProveedor });
                return new Notificacion<List<Proveedor>>()
                {
                    Estatus = list.Count > 0 ? 200 : -1,
                    Mensaje = list.Count > 0 ? "OK" : "Sin resultados",
                    Modelo = list
                };
            }
            catch (Exception ex)
            {
                return WsUtils<List<Proveedor>>.RegresaExcepcion(ex, null);
            }
        }

        [HttpPost]
        public Models.Notificacion<Proveedor> AgregarProveedor(Proveedor p)
        {
            try
            {
                Notificacion<Proveedor> r = new ProveedorDAO().GuardarProveedor(p);
                return new Notificacion<Proveedor>()
                {
                    Estatus = r.Estatus,
                    Mensaje = r.Mensaje,
                    Modelo = p
                };
            }
            catch (Exception ex)
            {
                return WsUtils<Proveedor>.RegresaExcepcion(ex, null);
            }
        }
    }
}
