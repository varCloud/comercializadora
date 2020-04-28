using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Usuario
    {
        public Boolean usuarioValido { get; set; }
        public Boolean activo { get; set; }
        public int idUsuario { get; set; }

        [Display(Name = "usuario")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string usuario { get; set; }

        [Display(Name = "contrasena")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string contrasena { get; set; }

        [Display(Name = "idRol")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int idRol { get; set; }


        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string nombre { get; set; }

        [Display(Name = "apellidoPaterno")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string apellidoPaterno { get; set; }

        [Display(Name = "apellidoMaterno")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string apellidoMaterno { get; set; }

        [Display(Name = "telefono")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string telefono { get; set; }


        public int idAlmacen { get; set; }


        public int idSucursal { get; set; }

        [Display(Name = "Sucursal")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Sucursal { get; set; }

        [Display(Name = "Almacen")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string Almacen { get; set; }


        public string Rol { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string[] idRolGuardar { get; set; }

        [Display(Name = "idAlmacen")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string[] idAlmacenGuardar { get; set; }

        [Display(Name = "idSucursal")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string[] idSucursalGuardar { get; set; }

        public int contador { get; set; }

        public string nombreCompleto { get; set; }


    }
}