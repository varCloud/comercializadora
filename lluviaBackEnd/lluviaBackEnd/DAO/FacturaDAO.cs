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
using System.ServiceModel;
using lluviaBackEnd.Utilerias;

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

        public Dictionary<string, object> ObtenerComprobante(Factura factura, Comprobante c)
        {
            Dictionary<string, object> items = null;
            List<ComprobanteConcepto> listConceptos = null;
            List<ConceptosAddenda> listConceptosAdenda = null;

            try
            {
                // para generar qr del sat = https://groups.google.com/forum/#!topic/vfp-factura-electronica-mexico/wLMK1MAhZWQ
                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                var parameters = new DynamicParameters();

               
                parameters.Add(factura.folio.Contains("PE") ? "@idPedidoEspecial":"@idVenta",(factura.folio.Contains("PE") ? factura.idPedidoEspecial.ToString(): factura.idVenta ));
                string sp = factura.folio.Contains("PE") ? "SP_FACTURACION_OBTENER_DETALLE_PEDIDO_ESPECIAL" : "SP_FACTURACION_OBTENER_DETALLE_VENTA" ;

                var result = this._db.QueryMultiple(sp, param: parameters, commandType: CommandType.StoredProcedure);
                var r1 = result.ReadFirst();
                if (r1.Estatus == 200)
                {
                    items = new Dictionary<string, object>();
                    items.Add("estatus", r1.Estatus);
                    var receptor = result.ReadFirst();
                    //OBTENER LA FORMA DE PAGO 
                    c.FormaPago = receptor.FormaPago;
                    //OBTENEMOS LAS DESCRIPCIONES DE LA INFORMACION DEL PAGO 
                    items.Add("descripcionUsoCFDI", receptor.descripcionUsoCFDI);
                    items.Add("descripcionFormaPago", receptor.descripcionFormaPago);
                    items.Add("correoCliente", receptor.correo);
                    items.Add("domicilioCliente", receptor.domicilio);
                    items.Add("RegimenFiscalReceptor", receptor.RegimenFiscalReceptor);
                    items.Add("descripcionRegimenFiscalReceptor", receptor.descripcionRegimenFiscalReceptor);
                    items.Add("nombreReceptor", receptor.Nombre +' '+receptor.sociedadMercantil);



                    ////OBTENEMOS LOS DATOS DEL CLIENTE PARA TIMBRAR
                    c.Receptor = new ComprobanteReceptor();
                    c.Receptor.Nombre =this.clearNombreReceptor(receptor.Nombre);
                    c.Receptor.Rfc = receptor.Rfc;
                    c.Receptor.UsoCFDI = receptor.UsoCFDI;
                    c.Receptor.DomicilioFiscalReceptor = receptor.DomicilioFiscalReceptor; //FAC 4.0
                    c.Receptor.RegimenFiscalReceptor = receptor.RegimenFiscalReceptor; //FAC 4.0
                    //FAC 4.0
                    if (c.Receptor.Rfc == "XAXX010101000")
                    {
                        c.Receptor.DomicilioFiscalReceptor = c.LugarExpedicion.ToString();
                        c.Receptor.RegimenFiscalReceptor = "616";
                        c.Receptor.UsoCFDI = "S01";
                    }
                    this.validarDataReceptor(c.Receptor);
                    //OBTENEMOS LOS CONCEPTOS PARA TIMBRAR 
                    listConceptos = result.Read<ComprobanteConcepto>().ToList();
                    if (listConceptos != null)
                    {
                        listConceptos.ForEach(data => data.Impuestos = new ComprobanteConceptoImpuestos()
                        {

                            Traslados = new ComprobanteConceptoImpuestosTraslados()
                            {
                                Traslado = new ComprobanteConceptoImpuestosTrasladosTraslado()
                                {
                                    Base = data.Importe,
                                    TasaOCuota = 0.160000M,
                                    Impuesto = "002",
                                    TipoFactor = "Tasa",
                                    Importe = Math.Round((data.Importe * 0.16M), 2, MidpointRounding.AwayFromZero)
                                }
                            }
                        });
                        listConceptos.ForEach(data => data.ObjetoImp = "02"); // FACT 4.0
                        c.Conceptos = listConceptos.ToArray();
                    }

                    //OBTENEMOS LOS CONCEPTOS PARA LA ADDENDA
                    listConceptosAdenda = result.Read<ConceptosAddenda>().ToList();
                    if (listConceptos != null)
                    {
                        listConceptosAdenda.ForEach(item => item.IVA = Math.Round((item.Importe * 0.16M), 2, MidpointRounding.AwayFromZero));
                    }
                    items.Add("comprobante", c);
                    items.Add("conceptosAddenda", listConceptosAdenda);
                }
                else {

                    items = new Dictionary<string, object>();
                    items.Add("estatus", r1.Estatus);
                    items.Add("mensaje", r1.mensaje);
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

        private string clearNombreReceptor(string nombreReceptor)
        {
            try
            {
                List<string> razonFiscal = ConfigurationManager.AppSettings["razonFiscal"].Split(',').ToList<string>();
                razonFiscal.ForEach(f => nombreReceptor = nombreReceptor.Replace(f,""));
                //if(nombreReceptor.Length > 0 && nombreReceptor.EndsWith("."))
                //{
                //    nombreReceptor = nombreReceptor.Substring(0, nombreReceptor.Length - 2);   
                //}
                return nombreReceptor.Trim();
            }
            catch (Exception ex)
            {
                throw new Exception("Por favor asigne elimine la razon social del cliente. Ejemplo S.A DE C.V o sus variantes");
                return nombreReceptor;
            }
        }

        private void  validarDataReceptor(ComprobanteReceptor c)
        {
            if (string.IsNullOrEmpty(c.RegimenFiscalReceptor))
                throw new Exception("Por favor asigne el régimen fiscal del cliente para poder emitir la factura.");

            if (string.IsNullOrEmpty(c.DomicilioFiscalReceptor))
                throw new Exception("Por favor actualice el Código postal(Domicilio Fiscal) del cliente para poder emitir la factura");

        }
        public void ObtenerImpuestosGenerales(ref Comprobante c)
        {
            try
            {
                decimal TotalImpuestosTrasladados = 0;
                decimal TotalBaseTrasladados = 0;
                TotalImpuestosTrasladados = c.Conceptos.ToList().Sum(data => data.Impuestos.Traslados.Traslado.Importe);
                TotalBaseTrasladados = c.Conceptos.ToList().Sum(data => data.Impuestos.Traslados.Traslado.Base);
                ComprobanteImpuestos impuestos = new ComprobanteImpuestos();
                impuestos.TotalImpuestosTrasladados = TotalImpuestosTrasladados;
                impuestos.Traslados = new ComprobanteImpuestosTraslados();
                impuestos.Traslados.Traslado = new ComprobanteImpuestosTrasladosTraslado();
                impuestos.Traslados.Traslado.Base = TotalBaseTrasladados;
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

        public Notificacion<String> GuardarFactura(Factura f)
        {

            Notificacion<String> n = new Notificacion<String>();
            try
            {
                _db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString());
                var parameters = new DynamicParameters();
                string sp = f.folio.Contains("PE") ? "SP_FACTURACION_PEDIDOS_ESPECIALES_INSERTA_FACTURA" : "SP_FACTURACION_INSERTA_FACTURA";
                parameters.Add(f.folio.Contains("PE") ? "@idPedidoEspecial" : "@idVenta", (f.folio.Contains("PE") ? f.idPedidoEspecial.ToString() : f.idVenta));
                parameters.Add("@idUsuario", f.idUsuario);
                parameters.Add("@fechaTimbrado", (f.fechaTimbrado == DateTime.MinValue ? DateTime.Now : (f.fechaTimbrado)));
                parameters.Add("@UUID", string.IsNullOrEmpty(f.UUID) ? (object)null : f.UUID);
                parameters.Add("@idEstatusFactura", f.estatusFactura);
                parameters.Add("@msjError", f.mensajeError);
                parameters.Add("@pathArchivo", f.pathArchivoFactura);
                n = _db.QuerySingle<Notificacion<String>>(sp, parameters, commandType: CommandType.StoredProcedure);
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
                string sp = f.idPedidoEspecial == 0 ? "SP_FACTURACION_INSERTA_FACTURA_CANCELADA" : "SP_FACTURACION_INSERTA_FACTURA_CANCELADA_PEDIDOS_ESPECIALES";
                parameters.Add(f.idPedidoEspecial == 0 ? "@idVenta" : "@idPedidoEspecial", f.idPedidoEspecial == 0 ? f.idVenta : f.idPedidoEspecial.ToString());
                parameters.Add("@idUsuario", f.idUsuario);
                parameters.Add("@idEstatusFactura", f.estatusFactura);
                parameters.Add("@msjError", f.mensajeError);
                n = _db.QuerySingle<Notificacion<String>>(sp, parameters, commandType: CommandType.StoredProcedure);
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
                        facturas.Modelo.ForEach(p => p.pathArchivoFactura = ConfigurationManager.AppSettings["urlDominio"].ToString() + p.pathArchivoFactura);

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


        public Notificacion<List<Factura>> ObtenerFacturasPedidosEspeciales(Factura factura)
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
                    var rs = _db.QueryMultiple("SP_FACTURACION_OBTENER_FACTURAS_PEDIDOS_ESPECIALES", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.status == 200)
                    {
                        facturas.Estatus = rs1.status;
                        facturas.Mensaje = rs1.mensaje;
                        facturas.Modelo = rs.Read<Factura>().ToList();
                        facturas.Modelo.ForEach(p => p.pathArchivoFactura = ConfigurationManager.AppSettings["urlDominio"].ToString() + p.pathArchivoFactura);

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

        public CancelarCFDI40 ObtenerCancelacionFactura(Factura factura)
        {
            CancelarCFDI40 c = null;
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    string sp = factura.idPedidoEspecial == 0 ? "SP_OBTENER_CANCELACION_FACTURA" : "SP_FACTURACION_OBTENER_CANCELACION_FACTURA";
                    parameters.Add(factura.idPedidoEspecial == 0 ? "@idVenta": "@idPedidoEspecial", factura.idPedidoEspecial == 0 ?factura.idVenta : factura.idPedidoEspecial.ToString());

                    var rs = _db.QueryMultiple(sp, parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.Estatus == 200)
                    {

                        var dataCancelacion = rs.ReadFirst();
                        if (dataCancelacion != null)
                        {
                            c = new CancelarCFDI40();
                            c.Fecha = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                            c.RfcEmisor = dataCancelacion.Rfc;
                            c.Folios = new CancelacionFolios();
                            c.Folios.Folio = new CancelacionFoliosFolio() { UUID = dataCancelacion.UUID, Motivo = "03" };
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Utils.EscribirLog("ObtenerCancelacionFactura" + ex.Message + " " + ex.StackTrace);
                throw ex;
            }
            return c;
        }

        public Notificacion<dynamic> ObtenerDetalleFactura(Factura f)
        {
            Notificacion<dynamic> facturas = new Notificacion<dynamic>();
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    string sp = "SP_FACTURACION_OBTENER_DATOS_FACTURA";
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta",f.idVenta);
                    var rs = _db.QueryMultiple(sp, parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.Estatus == 200)
                    {
                        facturas.Estatus = rs1.Estatus;
                        facturas.Mensaje = rs1.Mensaje;
                        facturas.Modelo = rs.ReadSingle();                   

                    }
                    else
                    {
                        facturas.Estatus = rs1.Estatus;
                        facturas.Mensaje = rs1.Mensaje;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return facturas;
        }

        public Notificacion<dynamic> ObtenerDetalleFacturaPE(Factura f)
        {
            Notificacion<dynamic> facturas = new Notificacion<dynamic>();
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idPedidoEspecial", f.idPedidoEspecial);
                    var rs = _db.QueryMultiple("SP_FACTURACION_OBTENER_DATOS_FACTURA_PEDIDO_ESPECIAL", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.Estatus == 200)
                    {
                        facturas.Estatus = rs1.Estatus;
                        facturas.Mensaje = rs1.Mensaje;
                        facturas.Modelo = rs.ReadSingle();

                    }
                    else
                    {
                        facturas.Estatus = rs1.Estatus;
                        facturas.Mensaje = rs1.Mensaje;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return facturas;
        }

        public Notificacion<dynamic> ObtenerPathXMLFactura(Int64 id , Boolean esPedidoEspecial = false)
        {
            Notificacion<dynamic> facturas = new Notificacion<dynamic>();
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    string sp = "SP_FACTURAS_OBTENER_PATH_ARCHIVO";
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta", esPedidoEspecial ? (Object)null : id);
                    parameters.Add("@idPedidoEspecial", esPedidoEspecial ? id : (Object)null);
                    var rs = _db.QueryMultiple(sp, parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = rs.ReadFirst();
                    if (rs1.Estatus == 200)
                    {
                        facturas.Estatus = rs1.Estatus;
                        facturas.Mensaje = rs1.Mensaje;
                        facturas.Modelo = rs.ReadSingle();

                    }
                    else
                    {
                        facturas.Estatus = rs1.Estatus;
                        facturas.Mensaje = rs1.Mensaje;
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