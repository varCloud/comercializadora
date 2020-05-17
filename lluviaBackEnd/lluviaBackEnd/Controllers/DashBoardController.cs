using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class DashBoardController : Controller
    {

        // GET: DashBoard
        public ActionResult index()
        {
            try
            {
                ViewBag.estaciones=new DAO.DashboardDAO().ObtenerVentasEstacion();
                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        public ActionResult _Grafico(EnumTipoGrafico tipoGrafico, EnumTipoReporteGrafico tipoReporteGrafico)
        {
            DashboardDAO dao = new DashboardDAO();
            Notificacion<Grafico> grafico = new Notificacion<Grafico>();

            if (tipoGrafico==EnumTipoGrafico.VentasPorFecha)
            {
                Notificacion<List<Estacion>> estaciones = dao.ObtenerVentasEstacion();
                grafico.Estatus = estaciones.Estatus;
                grafico.Mensaje = estaciones.Mensaje;

                List<Data> dataEstaciones = new List<Data>();
                List<seriesDrilldown> SeriesDrilldown = new List<seriesDrilldown>();
                
                
                if (estaciones.Estatus==200)
                {
                    foreach(Estacion estacion in estaciones.Modelo)
                    {
                        Notificacion<List<Categoria>> categorias = dao.ObtenerVentasPorFecha(tipoReporteGrafico,estacion.idEstacion);
                        if(categorias.Estatus==200)
                        {
                            Data data = new Data();
                            data.name = estacion.nombre;
                            data.y = categorias.Modelo.Sum(x=>x.total);
                            data.drilldown = estacion.idEstacion + "_" + estacion.nombre;
                            dataEstaciones.Add(data);

                            List<List<Object>> DataDrilldown = new List<List<Object>>();
                            foreach (Categoria c in categorias.Modelo)
                            {
                                DataDrilldown.Add(new List<Object>() { c.categoria.ToString(), c.total });
                            }


                            SeriesDrilldown.Add(new seriesDrilldown()
                            {
                                id = estacion.idEstacion + "_" + estacion.nombre,
                                name = estacion.nombre,
                                data = DataDrilldown
                            });
                        }
                    }

                    grafico.Modelo = new Grafico();
                    if(dataEstaciones.Count>0)
                    {
                        grafico.Estatus = 200;
                        grafico.Modelo.data = dataEstaciones;
                        grafico.Modelo.seriesDrilldowns = SeriesDrilldown;
                    }
                    else
                    {
                        grafico.Estatus = -1;
                        grafico.Mensaje = "No existe información para mostrar";
                    }
                   
                }

                               
            }

            ViewBag.tipoGrafico = tipoGrafico;
            ViewBag.tipoReporteGrafico = tipoReporteGrafico;


            return PartialView(grafico);
        }

    }
}
