using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Cierre
    {
        public int idEstacion { get; set; }
        public int idUsuario { get; set; }
        public int totalVentas { get; set; }
        public float efectivoDisponible { get; set; }
        public float retirosHechosDia { get; set; }
        public float montoCierre { get; set; }
        public float montoVentasDelDia { get; set; }
        public int idAlmacen { get; set; }
    }
}