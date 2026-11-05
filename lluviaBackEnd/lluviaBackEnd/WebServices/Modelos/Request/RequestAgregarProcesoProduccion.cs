namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestAgregarProcesoProduccion
    {
        public int idProducto { get; set; }
        public double lotes { get; set; }
        public int idUsuario { get; set; }
        public int idAlmacen { get; set; }
    }
}
