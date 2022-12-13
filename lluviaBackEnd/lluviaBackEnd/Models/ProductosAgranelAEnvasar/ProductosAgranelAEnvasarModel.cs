using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class ProductosAgranelAEnvasarModel
    {
        public int idRelacionEnvasadoAgranel { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idProductoEnvasado { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idProductoAgranel { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idProducoEnvase { get; set; }

        public int activo { get; set; }
        public DateTime fechaAlta { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idUnidadMedidad { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string valorUnidadMedida { get; set; }

        public string valorUnidadMedidaConverter { get; set; }

        public string envaseDescripcion { get; set; }
        public string agranelDescripcion { get; set; }
        public string envasadoDescripcion { get; set; }
        public string unidadMedidad { get; set; }
    }
}