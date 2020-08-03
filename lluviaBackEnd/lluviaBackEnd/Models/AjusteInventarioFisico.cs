using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class AjusteInventarioFisico
    {
        public Int64 idAjusteInventarioFisico { get; set; }
        public int cantidadActual { get; set; }
        public int cantidadEnFisico { get; set; }
        public int cantidadAAjustar { get; set; }
        public Producto producto { get; set; }
        public Usuario usuario { get; set; }
        public DateTime fechaAlta { get; set; }
        public AjusteInventarioFisico()
        {
            producto = new Producto();
            usuario = new Usuario();
        }
    }
}