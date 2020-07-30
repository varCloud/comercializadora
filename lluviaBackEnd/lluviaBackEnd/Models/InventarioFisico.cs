using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class InventarioFisico
    {
        public int idInventarioFisico { get; set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public Boolean Activo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public Sucursal Sucursal { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime FechaAlta { get; set; }


        public InventarioFisico()
        {
            Sucursal = new Sucursal();
            Usuario = new Usuario();
        }
    }
}