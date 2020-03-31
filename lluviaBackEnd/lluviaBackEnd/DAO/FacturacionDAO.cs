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
                    ObtenerConceptos(ref c , 1);
                    ObtenerImpuestosGenerales(ref c);
                }
            }
            return c;
        }

        private Comprobante MapComprobanteAEmisor(Comprobante comprobante, ComprobanteEmisor comprobanteEmisor)
        {
            comprobante.Emisor = comprobanteEmisor;
            return comprobante;
        }

        public void  ObtenerConceptos(ref Comprobante c, int idVenta)
        {

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
                        c.Conceptos = result.Read<ComprobanteConcepto>().ToArray();
                        if (c.Conceptos != null)
                        {
                            c.Conceptos.ToList().ForEach(data => data.Impuestos = new ComprobanteConceptoImpuestos()
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
            
        }

        public void ObtenerImpuestosGenerales(ref Comprobante c)
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

        }

    }
}