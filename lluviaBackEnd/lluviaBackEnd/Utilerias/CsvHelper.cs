using CsvHelper;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace lluviaBackEnd.Utilerias
{
    public static class lluviaCsvHelper
    {
        public static byte[] ExportToCSV(List<Producto> productos)
        {
            StringBuilder sb = new StringBuilder();
            string data = "";
            var records = new List<object>();
            productos.ForEach(item =>
            {
                records.Add(new {
                 idProducto = item.idProducto,
                 descripcion =item.descripcion,
                 DescripcionLinea =item.DescripcionLinea,
                 precioIndividual =item.precioIndividual,
                 precioMenudeo =item.precioMenudeo,
                 porcUtilidadIndividual =item.porcUtilidadIndividual,
                 porcUtilidadMayoreo =item.porcUtilidadMayoreo,
                 descripcionUnidadCompra =item.unidadCompra.descripcionUnidadCompra,
                 cantidadUnidadCompra =item.unidadCompra.cantidadUnidadCompra,
                 costo=item.costo.ToString("C2"),
                });
            });

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
    
        }
    }
}