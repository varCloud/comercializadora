using System;
using System.Collections.Generic;
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
        public String nombres { get; set; }
        public String apellidoPaterno { get; set; }

        public string  apellidoMaterno { get; set; }

        public string telefono { get; set; }

        public string correo { get; set; }

        public string rfc { get; set; }

        public string calle { get; set; }

        public string numeroExterior { get; set; }

        public string colonia { get; set; }

        public string municipio { get; set; }

        public string cp { get; set; }

        public string estado { get; set; }

        public DateTime FechaAlta { get; set; }

        public bool activo { get; set; }

        public TipoCliente tipoCliente { get; set; }
    }
}