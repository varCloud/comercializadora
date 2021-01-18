using lluviaBackEnd.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Sesion
    {
        public Boolean usuarioValido { get; set; }
        [Display(Name ="Usuario")]
        [Required(ErrorMessage ="Este campo no puede estar vacio")]
        public string usuario { get; set; }

        //public int NumeroUsuario { get; set; }
        [Display(Name = "Constraseña")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string contrasena { get; set; }

        public string Token { get; set; }

        public int idUsuario { get; set; }
        public int idRol { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string telefono { get; set; }
        //public string contrasena { get; set; }
        public int idAlmacen { get; set; }
        public int idSucursal { get; set; }
        public string Sucursal { get; set; }
        public string Almacen { get; set; }
        public string Rol { get; set; }
        public string configurado { get; set; }
        public int idEstacion { get; set; }
        public string macAdress { get; set; }
        public float comisionBancaria { get; set; }

        public int devolucionesPermitidas { get; set; }
        public int agregarProductosPermitidos { get; set; }
        public List<Permiso> permisosModulo { get; set; }

        public string pathDominio { get; set; }
        
        public static bool TienePermiso(EnumRolesPermisos valor)
        {
            HttpContext context = HttpContext.Current;
            Sesion sesion = (Sesion)context.Session["UsuarioActual"];

            return sesion.permisosModulo.Where(x => (EnumRolesPermisos)x.idPermiso == valor).Any();
        }

        public static bool ExisteInventarioFisicoActivo()
        {
            HttpContext context = HttpContext.Current;
            Sesion sesion = (Sesion)context.Session["UsuarioActual"];

            return new InventarioFisicoDAO().ValidaExisteInventarioFisicoActivo(sesion.idUsuario).Estatus==200 ? true : false;
        }

    }
}