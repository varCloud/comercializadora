using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class AjusteInventarioFisico
    {
        public Int64 idAjusteInventarioFisico { get; set; }
        public int idInventarioFisico { get; set; }
        public float cantidadActual { get; set; }
        public float cantidadEnFisico { get; set; }
        public float cantidadAAjustar { get; set; }
        public Producto producto { get; set; }
        public Usuario usuario { get; set; }
        public DateTime fechaAlta { get; set; }
        public Boolean ajustado { get; set; }

        public Decimal totalVentas { get; set; }
        public Decimal totalCompras { get; set; }
        public float porcMerma { get; set; }

        public int errorHumano { get; set; }

        public AjusteInventarioFisico()
        {
            producto = new Producto();
            usuario = new Usuario();
        }
    }
}