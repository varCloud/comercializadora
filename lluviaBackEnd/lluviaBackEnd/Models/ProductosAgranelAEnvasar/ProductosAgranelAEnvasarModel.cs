using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class ProductosAgranelAEnvasarModel
    {
        public int idRelacionEnvasadoAgranel { get; set; }
        public int idProductoEnvasado { get; set; }
        public int idProductoAgranel { get; set; }
        public int idProducoEnvase { get; set; }
        public int activo { get; set; }
        public DateTime fechaAlta { get; set; }
        public string unidadMedidad { get; set; }
        public float valorUnidadMedida { get; set; }
        public string envaseDescripcion { get; set; }
        public string agranelDescripcion { get; set; }
        public string envasadoDescripcion { get; set; }
    }
}