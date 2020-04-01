using lluviaBackEnd.Models.Facturacion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;

namespace lluviaBackEnd.DAO
{
    public class FacturacionDAO
    {
        private IDbConnection _db;
        public void GenerarFactura()
        {
            Comprobante c = new Comprobante();
        }

        public Comprobante ObtenerComprobante()
        {
            Comprobante c = new Comprobante();
            using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
            {
                var result = this._db.QueryMultiple("SP_FACTURACION_OBTENER_CONFIGURACION_COMPROBANTE",  commandType: CommandType.StoredProcedure);
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

        public List<ComprobanteConcepto> ObtenerConceptos(int idVenta)
        {
            List<ComprobanteConcepto> listConceptos = null;
            try
            {
                using (_db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idVenta", idVenta);
                    var result = this._db.QueryMultiple("SP_FACTURACION_OBTENER_DETALLE_VENTA",param:parameters ,commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.Estatus == 200)
                    {
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
                                        TasaOCuota = 0.16M,
                                        Impuesto = "002",
                                        Importe = Math.Round((data.Importe * 0.16M),2,MidpointRounding.AwayFromZero)
                                    }
                                }
                            }

                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listConceptos;
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
                impuestos.Traslados.Traslado.TasaOCuota = 0.16M;
                c.Impuestos = impuestos;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void ObtenerTotal(ref Comprobante c) {
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
    }
}