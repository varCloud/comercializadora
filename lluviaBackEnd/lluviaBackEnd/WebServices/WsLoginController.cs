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
                return new LoginDAO().ValidaUsuario(new Sesion() { usuario = login.Usuario, contrasena = login.Contrasena });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
