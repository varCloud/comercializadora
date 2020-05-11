using lluviaBackEnd.Models.Facturacion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using lluviaBackEnd.Models;

namespace lluviaBackEnd.DAO
{
    public class FacturaDAO
    {
        private IDbConnection _db;
      

        public Comprobante ObtenerConfiguracionComprobante()
        {
            Comprobante c = new Comprobante();
            using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var result = this._db.QueryMultiple("SP_FACTURACION_OBTENER_CONFIGURACION_COMPROBANTE", commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                if (r1.Estatus == 200)
                {
                    c = result.Read<Comprobante, ComprobanteEmisor, Comprobante>(MapComprobanteAEmisor, splitOn: "RegimenFiscal").ToList().FirstOrDefault();
                    c.Fecha = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            }
            return c;
        }

        private Comprobante MapComprobanteAEmisor(Comprobante comprobante, ComprobanteEmisor comprobanteEmisor)
        {
            comprobante.Emisor = comprobanteEmisor;
            return comprobante;
        }

        public Dictionary<string, object> ObtenerComprobante(string idVenta, Comprobante c)
        {
            Dictionary<string, object> items = null;
            List<ComprobanteConcepto> listConceptos = null;
            List<ConceptosAddenda> listConceptosAdenda = null;
            
            try
            {
                // para generar qr del sat = https://groups.google.com/forum/#!topic/vfp-factura-electronica-mexico/wLMK1MAhZWQ
                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                var parameters = new DynamicParameters();
                parameters.Add("@idVenta", idVenta);
                var result = this._db.QueryMultiple("SP_FACTURACION_OBTENER_DETALLE_VENTA", param: parameters, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                if (r1.Estatus == 200)
                {
                    items = new Dictionary<string, object>();

                    var receptor = result.ReadFirst();
                    //OBTENER LA FORMA DE PAGO 
                    c.FormaPago = receptor.FormaPago;
                    //OBTENEMOS LAS DESCRIPCIONES DE LA INFORMACION DEL PAGO 
                    items.Add("descripcionUsoCFDI", receptor.descripcionUsoCFDI);
                    items.Add("descripcionFormaPago", receptor.descripcionUsoCFDI);

                    ////OBTENEMOS LOS DATOS DEL CLIENTE PARA TIMBRAR
                    c.Receptor = new ComprobanteReceptor();
                    c.Receptor.Nombre = receptor.Nombre;
                    c.Receptor.Rfc = receptor.Rfc;
                    c.Receptor.UsoCFDI = receptor.UsoCFDI;      

                    //OBTENEMOS LOS CONCEPTOS PARA TIMBRAR 
                    listConceptos = result.Read<ComprobanteConcepto>().ToList();
                    if (listConceptos != null)
                    {
                        listConceptos.ForEach(data =>  data.Impuestos = new ComprobanteConceptoImpuestos()
                        {
                            
                            Traslados = new ComprobanteConceptoImpuestosTraslados()
                            {
                                Traslado = new ComprobanteConceptoImpuestosTrasladosTraslado()
                                {
                                    Base = data.Importe,
                                    TasaOCuota = 0.160000M,
                                    Impuesto = "002",
                                    TipoFactor="Tasa",
                                    Importe = Math.Round((data.Importe * 0.16M), 2, MidpointRounding.AwayFromZero)
                                }
                            }
                        });
                        c.Conceptos = listConceptos.ToArray();
                    }

                    //OBTENEMOS LOS CONCEPTOS PARA LA ADDENDA
                    listConceptosAdenda = result.Read<ConceptosAddenda>().ToList();
                    if (listConceptos != null) {
                        listConceptosAdenda.ForEach(item => item.IVA = Math.Round((item.Importe * 0.16M), 2, MidpointRounding.AwayFromZero));
                    }
                    items.Add("comprobante", c);
                    items.Add("conceptosAddenda", listConceptosAdenda);
                }
               
                
               
            }
             
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _db.Close();
                _db.Dispose();
                _db = null;
            }
            return items;
        }
        public void ObtenerImpuestosGenerales(ref Comprobante c)
        {
            try
            {
                decimal TotalImpuestosTrasladados = 0;
                TotalImpuestosTrasladados = c.Conceptos.ToList().Sum(data => data.Impuestos.Traslados.Traslado.Importe);
                ComprobanteImpuestos impuestos = new ComprobanteImpuestos();
                impuestos.TotalImpuestosTrasladados = TotalImpuestosTrasladados;
                impuestos.Traslados = new ComprobanteImpuestosTraslados();
                impuestos.Traslados.Traslado = new ComprobanteImpuestosTrasladosTraslado();
                impuestos.Traslados.Traslado.Importe = TotalImpuestosTrasladados;
                impuestos.Traslados.Traslado.Impuesto = "002";
                impuestos.Traslados.Traslado.TipoFactor = "Tasa";
                impuestos.Traslados.Traslado.TasaOCuota = 0.160000M;
                c.Impuestos = impuestos;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void ObtenerTotal(ref Comprobante c)
        {
            try
            {
                c.SubTotal = c.Conceptos.Sum(data => data.Importe);
                c.Total = c.SubTotal + c.Impuestos.TotalImpuestosTrasladados;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Notificacion<String> GuardarFactura( Factura f) {

            Notificacion<String> n  = new Notificacion<String>();
            try
            {
                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                var parameters = new DynamicParameters();
                parameters.Add("@idVenta", f.folio);
                
                parameters.Add("@idUsuario", f.idUsuario);
                parameters.Add("@fechaTimbrado",(f.fechaTimbrado == DateTime.MinValue ? DateTime.Now : (f.fechaTimbrado)));
                parameters.Add("@UUID", string.IsNullOrEmpty(f.UUID) ? (object)null : f.UUID);
                parameters.Add("@idEstatusFactura",f.estatusFactura);
                parameters.Add("@msjError", f.mensajeError);
                n = _db.QuerySingle<Notificacion<String>>("SP_FACTURACION_INSERTA_FACTURA", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return n;
        }

        public Notificacion<String> CancelarFactura(Factura f)
        {

                Notificacion<String> n = new Notificacion<String>();
                try
                {

                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta", f.idVenta);
                    parameters.Add("@idUsuario", f.idUsuario );
                    parameters.Add("@idEstatusFactura", f.estatusFactura);
                    parameters.Add("@msjError", f.mensajeError);
                    n = _db.QuerySingle<Notificacion<String>>("SP_FACTURACION_INSERTA_FACTURA_CANCELADA", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            return n; 
        }

        public Notificacion<List<Factura>> ObtenerFacturas(Factura factura)
        {
            Notificacion<List<Factura>> facturas = new Notificacion<List<Factura>>();
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idStatusFactura", factura.estatusFactura == 0 ? (object)null : factura.estatusFactura);
                    parameters.Add("@idUsuario", factura.idUsuario == 0 ? (object)null : factura.idUsuario);
                    parameters.Add("@fechaIni", factura.fechaIni == DateTime.MinValue ? (object)null : factura.fechaIni);
                    parameters.Add("@fechaFin", factura.fechaFin == DateTime.MinValue ? (object)null : factura.fechaFin);
                    var rs = _db.QueryMultiple("SP_CONSULTA_FACTURAS", parameters, commandType: CommandType.StoredProcedure);                   
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        facturas.Estatus = rs1.status;
                        facturas.Mensaje = rs1.mensaje;
                        facturas.Modelo = rs.Read<Factura>().ToList();

                    }
                    else
                    {
                        facturas.Estatus = rs1.status;
                        facturas.Mensaje = rs1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return facturas;
        }


    }
}