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
                DashboardDAO dao = new DashboardDAO();
                ViewBag.estaciones= dao.ObtenerVentasEstacion(null,null);
                ViewBag.topTenProductos = dao.ObtenerTopTen(EnumTipoReporteGrafico.Dia,EnumTipoGrafico.TopTenProductos);
                ViewBag.topTenClientes = dao.ObtenerTopTen(EnumTipoReporteGrafico.Dia, EnumTipoGrafico.TopTenClientes);
                ViewBag.topTenProveedores = dao.ObtenerTopTen(EnumTipoReporteGrafico.Mensuales, EnumTipoGrafico.TopTenProvedores);
                ViewBag.InformacionGlobal=dao.ObtenerInformacionGlobal(EnumTipoReporteGrafico.Dia);
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
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

            if(tipoGrafico==EnumTipoGrafico.TopTenProductos || tipoGrafico == EnumTipoGrafico.TopTenClientes || tipoGrafico == EnumTipoGrafico.TopTenProvedores)
            {
                Notificacion<List<Categoria>> categorias= dao.ObtenerTopTen(tipoReporteGrafico, tipoGrafico);
                grafico.Estatus = categorias.Estatus;
                grafico.Mensaje = categorias.Mensaje;
                if(categorias.Estatus==200)
                {
                    grafico.Modelo = new Grafico();
                    grafico.Modelo.categorias = categorias.Modelo;
                }

            }

            if (tipoGrafico == EnumTipoGrafico.InformacionGlobal)
            {
                Notificacion<List<Categoria>> categorias = dao.ObtenerInformacionGlobal(tipoReporteGrafico);
                grafico.Estatus = categorias.Estatus;
                grafico.Mensaje = categorias.Mensaje;
                if (categorias.Estatus == 200)
                {
                    grafico.Modelo = new Grafico();
                    grafico.Modelo.categorias = categorias.Modelo;
                }

            }

            ViewBag.tipoGrafico = tipoGrafico;
            ViewBag.tipoReporteGrafico = tipoReporteGrafico;


            return PartialView(grafico);
        }

    }
}
