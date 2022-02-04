using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public class Grafico
    {
        public List<Data> data { get; set; }
        public List<seriesDrilldown> seriesDrilldowns { get; set; }

        public List<Categoria> categorias { get; set; }


    }

    public class Categoria
    {
        public int id { get; set; }
        public string categoria { get; set; }
        public float total { get; set; }
        public float totalPE { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }


    }

    public class Data
    {
        public int Id { get; set; }
        public string name { get; set; }
        public float y { get; set; }
        public string color { get; set; }
        public string drilldown { get; set; }
        public string leyenda { get; set; }
    }

    public class seriesDrilldown
    {
        public String id { get; set; }
        public String name { get; set; }
        public List<List<Object>> data { get; set; }
    }
}