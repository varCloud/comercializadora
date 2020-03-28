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
                }
            }
            return c;
        }

        private Comprobante MapComprobanteAEmisor(Comprobante comprobante, ComprobanteEmisor comprobanteEmisor)
        {
            comprobante.Emisor = comprobanteEmisor;
            return comprobante;
        }
    }
}