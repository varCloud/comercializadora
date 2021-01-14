using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class IngresoEfectivo
    {
        public int idIngreso { get; set; }
        public float monto { get; set; }
        public DateTime fechaAlta { get; set; }
        public int idUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public int idAlmacen { get; set; }
        public string Almacen { get; set; }
        public int idSucursal { get; set; }
        public string Sucursal { get; set; }
        public int idTipoIngreso { get; set; }


    }
}