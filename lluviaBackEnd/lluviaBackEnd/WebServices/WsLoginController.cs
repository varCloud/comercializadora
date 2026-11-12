using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace lluviaBackEnd.WebServices
{
    public class WsLoginController : ApiController
    {
        public Models.Notificacion<Sesion> ValidaUsario(RequestLogin login)
        {
            try
            {
                Models.Notificacion<Sesion> n = new LoginDAO().ValidaUsuario(new Sesion() { usuario = login.Usuario, contrasena = login.Contrasena });
                // Contraseña para ver la fórmula en la HH: solo Administrador y Encargado de producción líquidos.
                if (n.Estatus == 200 && n.Modelo != null && (n.Modelo.idRol == (int)EnumRoles.Administrador || n.Modelo.idRol == (int)EnumRoles.Encargado_producción_liquidos))
                    n.Modelo.contrasenaFormula = System.Configuration.ConfigurationManager.AppSettings["contrasenaFormula"];
                return n;
            }
            catch (Exception ex)
            {
                return WsUtils<Sesion>.RegresaExcepcion(ex, null);
            }

        }
    }
}
