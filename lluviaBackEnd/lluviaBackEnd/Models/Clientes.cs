using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Cliente
    {
        public Cliente()
        {
            tipoCliente = new TipoCliente();
        }
        public Int64 idCliente { get; set; }

        
        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [RequiredIfFalse("esPersonaMoral", ErrorMessage = "Esta campo es requerido")]
        public String nombres { get; set; }

        [Display(Name = "Apellido Paterno")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [RequiredIfFalse("esPersonaMoral", ErrorMessage = "Esta campo es requerido")]
        public String apellidoPaterno { get; set; }

        public string  apellidoMaterno { get; set; }

        [Display(Name = "Telefóno")]
        //[Required(AllowEmptyStrings =true, ErrorMessage = "Este campo no puede estar vacio")]
        [MaxLength(10,ErrorMessage ="El telefóno debe ser maximo  de 10 caracteres")]
        [MinLength(10, ErrorMessage = "El telefóno debe ser minimo de  10 caracteres")]
        [StringLength(10, ErrorMessage = "El telefóno debe ser  de  10 caracteres")]
        public string telefono { get; set; }

        public string correo { get; set; }

        public string rfc { get; set; }

        public string calle { get; set; }

        public string numeroExterior { get; set; }
        public string numeroInterior { get; set; }
        public string localidad { get; set; }

        public string colonia { get; set; }

        public string municipio { get; set; }

        public string cp { get; set; }

        public string estado { get; set; }

        public DateTime FechaAlta { get; set; }

        public bool activo { get; set; }

        public TipoCliente tipoCliente { get; set; }
        public int contador { get; set; }
        public decimal descuento { get; set; }

        public string nombreCompleto_ { get; set; }

        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Range(1,90,ErrorMessage = "Este campo no puede estar vacio")]
        public int idTipoCliente { get; set; }

        /*******PERSONA MORAL*********/
        public bool esPersonaMoral { get; set; }

        [RequiredIfTrue("esPersonaMoral", ErrorMessage = "Esta campo es requerido")]
        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        public string razonSocial { get; set; }

        [Display(Name = "RFC")]
        [RequiredIfTrue("esPersonaMoral", ErrorMessage = "Esta campo es requerido")]
        public string rfcPM { get; set; }

        public string nombreContacto { get; set; }
        public string telefonoContacto { get; set; }
        public string correoContacto { get; set; }

        /*******INFORMACION PARA PEDIDOS ESPECIALES*********/
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string nombreContactoPE { get; set; }
        public string telefonoContactoPE { get; set; }
        public string correoContactoPE { get; set; }

        public int diasCredito { get; set; }
        public float montoMaximoCredito { get; set; }

        public bool usarDatosCliente { get; set; }


    }
}