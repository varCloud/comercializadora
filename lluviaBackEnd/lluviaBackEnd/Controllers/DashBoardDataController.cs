using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace lluviaBackEnd.Controllers
{
    /// <summary>
    /// Hosts the async dashboard sections. Session is read-only here so the parallel
    /// AJAX calls are not serialized by ASP.NET's exclusive session lock.
    /// Do NOT write to Session from any action in this controller.
    /// </summary>
    [SessionTimeout]
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class DashBoardDataController : Controller
    {
        private const string ViewsPath = "~/Views/DashBoard/";

        private int ResolveIdEstacion()
        {
            Sesion usuario = Session["UsuarioActual"] as Sesion;
            return (usuario != null && usuario.idRol == 3) ? usuario.idEstacion : 0;
        }

        public ActionResult _Indicadores()
        {
            DashboardDAO dao = new DashboardDAO();
            int idEstacion = ResolveIdEstacion();

            ViewBag.estaciones = dao.ObtenerVentasEstacion(null, null, idEstacion);
            ViewBag.InformacionGlobal = dao.ObtenerInformacionGlobal(EnumTipoReporteGrafico.Dia, idEstacion);
            ViewBag.mermaMensual = dao.ObtenerMermaMensual();
            ViewBag.costoProduccionAgranel = dao.ObtenerCostoProduccionMensualAgranel();
            return PartialView(ViewsPath + "_Indicadores.cshtml");
        }

        public ActionResult _VentasEstaciones()
        {
            DashboardDAO dao = new DashboardDAO();
            ViewBag.estaciones = dao.ObtenerVentasEstacion(null, null, ResolveIdEstacion());
            return PartialView(ViewsPath + "_VentasEstaciones.cshtml");
        }

        public ActionResult _Grafico(EnumTipoGrafico tipoGrafico, EnumTipoReporteGrafico tipoReporteGrafico)
        {
            // The IVA series is only consumed by the VentasPorFecha chart.
            if (tipoGrafico == EnumTipoGrafico.VentasPorFecha)
            {
                CrearDataGraficoIVA(tipoGrafico, tipoReporteGrafico);
            }
            DashboardDAO dao = new DashboardDAO();
            Notificacion<Grafico> grafico = new Notificacion<Grafico>();
            int idEstacion = ResolveIdEstacion();

            if (tipoGrafico == EnumTipoGrafico.VentasPorFecha)
            {
                Notificacion<List<Categoria>> categorias = dao.ObtenerVentasPorFecha(tipoReporteGrafico, idEstacion);

                grafico.Estatus = categorias.Estatus;
                grafico.Mensaje = categorias.Mensaje;

                List<Data> dataEstaciones = new List<Data>();
                List<seriesDrilldown> SeriesDrilldown = new List<seriesDrilldown>();

                if (categorias.Estatus == 200)
                {
                    foreach (Categoria categoria in categorias.Modelo)
                    {
                        Data data = new Data();
                        data.name = categoria.categoria;
                        data.y = categoria.total;
                        if (idEstacion == 0)
                        {
                            Notificacion<List<Estacion>> estaciones = dao.ObtenerVentasEstacion(categoria.fechaIni, categoria.fechaFin);
                            if (estaciones.Estatus == 200)
                            {
                                data.y = estaciones.Modelo.Sum(x => x.montoTotalDia);

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
                        }

                        dataEstaciones.Add(data);
                    }

                    grafico.Modelo = new Grafico();
                    if (dataEstaciones.Count > 0)
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

                    List<string> _categorias = new List<string>();
                    List<float> _ventas = new List<float>();
                    List<float> _ventasPE = new List<float>();

                    List<Dictionary<string, object>> _series = new List<Dictionary<string, object>>();
                    Dictionary<string, object> _serie = new Dictionary<string, object>();

                    categorias.Modelo.ForEach(x =>
                    {
                        _categorias.Add(x.categoria);
                        _ventas.Add(x.total);
                        _ventasPE.Add(x.totalPE);
                    });
                    _serie.Add("name", "Ventas");
                    _serie.Add("data", _ventas);
                    _series.Add(_serie);
                    _serie = new Dictionary<string, object>();
                    _serie.Add("name", "Ventas PE");
                    _serie.Add("data", _ventasPE);
                    _series.Add(_serie);

                    ViewBag.categoriasPE = _categorias;
                    ViewBag.seriesPE = _series;
                }
            }

            if (tipoGrafico == EnumTipoGrafico.TopTenProductos || tipoGrafico == EnumTipoGrafico.TopTenClientes || tipoGrafico == EnumTipoGrafico.TopTenProvedores)
            {
                Notificacion<List<Categoria>> categorias = dao.ObtenerTopTen(tipoReporteGrafico, tipoGrafico, idEstacion);
                grafico.Estatus = categorias.Estatus;
                grafico.Mensaje = categorias.Mensaje;
                if (categorias.Estatus == 200)
                {
                    grafico.Modelo = new Grafico();
                    grafico.Modelo.categorias = categorias.Modelo;
                }
            }

            if (tipoGrafico == EnumTipoGrafico.InformacionGlobal)
            {
                Notificacion<List<Categoria>> categorias = dao.ObtenerInformacionGlobal(tipoReporteGrafico, idEstacion);
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

            return PartialView(ViewsPath + "_Grafico.cshtml", grafico);
        }

        [NonAction]
        private void CrearDataGraficoIVA(EnumTipoGrafico tipoGrafico, EnumTipoReporteGrafico tipoReporteGrafico)
        {
            DashboardDAO dao = new DashboardDAO();
            int idEstacion = ResolveIdEstacion();
            Notificacion<List<Categoria>> categorias = dao.ObtenerIvaAcumuladoPorFecha(tipoReporteGrafico, idEstacion);

            if (categorias.Estatus == 200)
            {
                List<Data> _serieGrafico = new List<Data>();
                categorias.Modelo.ForEach(x =>
                {
                    _serieGrafico.Add(new Data() { name = x.categoria, drilldown = x.categoria, y = (x.total + x.totalPE) });
                });
                ViewBag.dataGraficoIVAAcumulado = _serieGrafico;
            }
        }
    }
}
