using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Producto
    {

        public int idProducto { get; set; }


        //[Display(Name = "nombre")]
        //[Required(ErrorMessage = "Este campo no puede estar vacio")]
        //public string  nombre { get; set; }

        //[Display(Name = "descripcion")]
        //[Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string  descripcion { get; set; }


        public int idUnidadMedida { get; set; }
        public string DescripcionUnidadMedida { get; set; }
        public int idLineaProducto { get; set; }
        public string DescripcionLinea { get; set; }
        public int cantidadUnidadMedida { get; set; }
        public string codigoBarras { get; set; }
        public Boolean activo { get; set; }
        public string articulo { get; set; }
        public string[] idLineaProductoConsulta { get; set; }
        //public string[] idUnidadMedidaConsulta { get; set; }
    }
}