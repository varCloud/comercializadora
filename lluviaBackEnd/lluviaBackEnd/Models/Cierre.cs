using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Cierre
    {
        public int idCierre { get; set; }
        public int idEstacion { get; set; }
        public int idUsuario { get; set; }
        public int totalVentas { get; set; }       
        public float retirosHechosDia { get; set; }
        public float retirosExcesoEfectivo { get; set; }        
        public float montoVentasDelDia { get; set; }
        public float montoVentasContado { get; set; }
        public float montoVentasTarjeta { get; set; }
        public float montoVentasTransferencias { get; set; }
        public float montoVentasOtros { get; set; }
        public float montoVentasCanceladas { get; set; }
        public float montoApertura { get; set; }
        public float montoIngresosEfectivo { get; set; }
        public int idAlmacen { get; set; }
        public float efectivoDisponible { get; set; }
        public float montoCierre { get; set; }
        public float ProductosDevueltos { get; set; }
        public float MontoTotalDevoluciones { get; set; }
        public DateTime fechaCierre { get; set; }
        public string descAlmacen { get; set; }
        public string nombreUsuario { get; set; }   
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public float EfectivoEntregadoEnCierre { get; set; }
    }
}