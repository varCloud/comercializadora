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
                ViewBag.estaciones=new DAO.DashboardDAO().ObtenerVentasEstacion(null,null);
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
                //Notificacion<List<Estacion>> estaciones = dao.ObtenerVentasEstacion();
                Notificacion<List<Categoria>> categorias = dao.ObtenerVentasPorFecha(tipoReporteGrafico);
                grafico.Estatus = categorias.Estatus;
                grafico.Mensaje = categorias.Mensaje;

                List<Data> dataEstaciones = new List<Data>();
                List<seriesDrilldown> SeriesDrilldown = new List<seriesDrilldown>();
                
                
                if (categorias.Estatus==200)
                {
                    foreach(Categoria categoria in categorias.Modelo)
                    {
                        Notificacion<List<Estacion>> estaciones = dao.ObtenerVentasEstacion(categoria.fechaIni,categoria.fechaFin);
                        Data data = new Data();
                        data.name = categoria.categoria;
                        if (estaciones.Estatus==200)
                        {                            
                            data.y = estaciones.Modelo.Sum(x=>x.montoTotalDia);                           

                            List<List<Object>> DataDrilldown = new List<List<Object>>();
                            foreach (Estacion e in estaciones.Modelo)
                            {
                                DataDrilldown.Add(new List<Object>() { e.nombre.ToString(), e.montoTotalDia });
                            }


                            SeriesDrilldown.Add(new seriesDrilldown()
                            {
                                id = categoria.id + "_" + categoria.categoria,
                                name = categoria.categoria,
                                data = DataDrilldown
                            });
                        }
                        data.drilldown = categoria.id + "_" + categoria.categoria;
                        dataEstaciones.Add(data);
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
