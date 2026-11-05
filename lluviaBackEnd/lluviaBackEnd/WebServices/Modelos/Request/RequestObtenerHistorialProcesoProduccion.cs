using System;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerHistorialProcesoProduccion
    {
        public int idUsuario { get; set; }
        public DateTime? fechaIni { get; set; }
        public DateTime? fechaFin { get; set; }
    }
}
