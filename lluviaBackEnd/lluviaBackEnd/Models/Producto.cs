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


        [Display(Name = "descripcion")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string  descripcion { get; set; }

        [Display(Name = "idUnidadMedida")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idUnidadMedida { get; set; }

        [Display(Name = "idLineaProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idLineaProducto { get; set; }

        [Display(Name = "idLineaProducto")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string idClaveProdServ { get; set; }

        public string DescripcionUnidadMedida { get; set; }



        public string DescripcionLinea { get; set; }

        [Display(Name = "cantidadUnidadMedida")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public int cantidadUnidadMedida { get; set; }

        public string codigoBarras { get; set; }

        public Boolean activo { get; set; }

        [Display(Name = "articulo")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string articulo { get; set; }

        public string[] idLineaProductoConsulta { get; set; }

        public int cantidad { get; set; }

        public DateTime fechaAlta { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public int contador { get; set; }

        public string claveUnidad { get; set; }
       
        public float precioIndividual { get; set; }
        public float precioMenudeo { get; set; }

        public float precio { get; set; }

        public int idPasillo { get; set; }
        public string Pasillo { get; set; }
        public int idRaq { get; set; }
        public string Raq { get; set; }
        public int idPiso { get; set; }
        public string Piso { get; set; }
        public int idAlmacen { get; set; }
        public string Almacen { get; set; }

        public int idSucursal { get; set; }
        public string Sucursal { get; set; }

        public string descripcionConExistencias { get; set; }

        public int idUsuario { get; set; }

        public float costo { get; set; }
        public float total { get; set; }

        public string observaciones { get; set; }
        public string usuarioRecibio { get; set; }
        public EstatusProducto estatusProducto { get; set; }
        public int cantidadRecibida { get; set; }
        public int cantidadDevuelta { get; set; }

        public float porcUtilidadIndividual { get; set; }
        public float porcUtilidadMayoreo { get; set; }

        public Producto()
        {
            estatusProducto = new EstatusProducto();
        }
    }
}