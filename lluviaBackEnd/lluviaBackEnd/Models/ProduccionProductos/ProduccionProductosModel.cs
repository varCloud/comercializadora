using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models.ProduccionProductos
{
    public class ProduccionProductosModel
    {
        public int id{ get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idProductoProduccion { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idProductoMateria1 { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idProductoMateria2 { get; set; }

        public int activo { get; set; }
        public DateTime fechaAlta { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idUnidadMedidad { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string valorUnidadMedida { get; set; }

        public string valorUnidadMedidaConverter { get; set; }

        public string productoProducido { get; set; }
        public string productoMateria1 { get; set; }
        public string productoMateria2 { get; set; }
        public string unidadMedidad { get; set; }
    }
}