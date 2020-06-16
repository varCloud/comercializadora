using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Retiros
    {

        public int idRetiro { get; set; }
        public int idCierre { get; set; }
        public float montoRetiro { get; set; }
        public int idUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public int idEstacion { get; set; }
        public string nombreEstacion { get; set; }
        public DateTime fechaAlta { get; set; }
        public float montoAutorizado { get; set; }
        public int idUsuarioAut { get; set; }
        public string nombreUsuarioAut { get; set; }
        public EnumTipoRetiro tipoRetiro { get; set; }
        public Status estatusRetiro { get; set; }
        public string descripcionSucursal { get; set; }
        public string descripcionAlmacen { get; set; }
        public Retiros()
        {
            estatusRetiro = new Status();
        }


        //public int MyProperty { get; set; }

    }
}