using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lluviaBackEnd.Models.Facturacion.FacturacionFactory
{
    public class FacturacionFactory
    {
        public FacturacionFactory(bool PedidoEspecial)
        {
            
        }

    }

    public class FacturaModel : IFacturacion
    {
        public long id { get; set ; }
        public string folio { get  ; set  ; }
        public EnumEstatusFactura estatusFactura { get  ; set  ; }
        public string mensajeError { get  ; set  ; }
        public string UUID { get  ; set  ; }
        public DateTime fechaTimbrado { get  ; set  ; }
        public int idUsuario { get  ; set  ; }
        public string nombreCliente { get  ; set  ; }
        public string nombreUsuarioFacturacion { get  ; set  ; }
        public string nombreUsuarioCancelacion { get  ; set  ; }
        public DateTime fechaCancelacion { get  ; set  ; }
        public DateTime fechaIni { get  ; set  ; }
        public DateTime fechaFin { get  ; set  ; }
        public string pathArchivoFactura { get  ; set  ; }
        public string codigoBarras { get  ; set  ; }
        public decimal montoTotal { get  ; set  ; }
        public string correoAdicional { get  ; set  ; }
        public string STORE_PROCEDURE_CANCELAR_FACTURA { get; set; }
        public string STORE_PROCEDURE_FACTURAS_OBTENER_PATH_ARCHIVO { get; set; }
        public FacturaModel()
        {
            this.STORE_PROCEDURE_CANCELAR_FACTURA = "SP_FACTURACION_INSERTA_FACTURA_CANCELADA";
            this.STORE_PROCEDURE_FACTURAS_OBTENER_PATH_ARCHIVO = "SP_FACTURAS_OBTENER_PATH_ARCHIVO";
        }

    }

    public class FacturaPedidoEspecialModel : IFacturacion
    {
        public long id { get; set; }
        public string folio { get; set; }
        public EnumEstatusFactura estatusFactura { get; set; }
        public string mensajeError { get; set; }
        public string UUID { get; set; }
        public DateTime fechaTimbrado { get; set; }
        public int idUsuario { get; set; }
        public string nombreCliente { get; set; }
        public string nombreUsuarioFacturacion { get; set; }
        public string nombreUsuarioCancelacion { get; set; }
        public DateTime fechaCancelacion { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public string pathArchivoFactura { get; set; }
        public string codigoBarras { get; set; }
        public decimal montoTotal { get; set; }
        public string correoAdicional { get; set; }
        public string STORE_PROCEDURE_CANCELAR_FACTURA { get; set; }
        public string STORE_PROCEDURE_FACTURAS_OBTENER_PATH_ARCHIVO { get; set; }
        public FacturaPedidoEspecialModel()
        {
            this.STORE_PROCEDURE_CANCELAR_FACTURA = "SP_FACTURACION_INSERTA_FACTURA_CANCELADA";
            this.STORE_PROCEDURE_FACTURAS_OBTENER_PATH_ARCHIVO = "SP_FACTURACION_INSERTA_FACTURA_CANCELADA_PEDIDOS_ESPECIALES";
        }

    }

    public interface IFacturacion
    {
         Int64 id { get; set; }
         string folio { get; set; }
         EnumEstatusFactura estatusFactura { get; set; }
         string mensajeError { get; set; }
         string UUID { get; set; }
         DateTime fechaTimbrado { get; set; }
         int idUsuario { get; set; }
         string nombreCliente { get; set; }
         string nombreUsuarioFacturacion { get; set; }
         string nombreUsuarioCancelacion { get; set; }
         DateTime fechaCancelacion { get; set; }
         DateTime fechaIni { get; set; }
         DateTime fechaFin { get; set; }
         string pathArchivoFactura { get; set; }
         string codigoBarras { get; set; }
         decimal montoTotal { get; set; }
         string correoAdicional { get; set; }

        string STORE_PROCEDURE_CANCELAR_FACTURA { get; set; }
        string STORE_PROCEDURE_FACTURAS_OBTENER_PATH_ARCHIVO { get; set; }


    }
}
