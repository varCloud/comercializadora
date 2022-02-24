using lluviaBackEnd.DAO;
using lluviaBackEnd.Filters;
using lluviaBackEnd.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace lluviaBackEnd.Controllers
{
    [SessionTimeout]
    public class DashBoardController : Controller
    {
        public object DefaultAuthenticationTypes { get; private set; }

        // GET: DashBoard
        public ActionResult index()
        {
            try
            {
                DashboardDAO dao = new DashboardDAO();
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                int idEstacion = 0;
                if (usuario.idRol == 3)
                {
                    idEstacion = usuario.idEstacion;
                }              

                ViewBag.estaciones= dao.ObtenerVentasEstacion(null,null, idEstacion);
                ViewBag.topTenProductos = dao.ObtenerTopTen(EnumTipoReporteGrafico.Dia,EnumTipoGrafico.TopTenProductos, idEstacion);
                ViewBag.topTenClientes = dao.ObtenerTopTen(EnumTipoReporteGrafico.Dia, EnumTipoGrafico.TopTenClientes, idEstacion);
                ViewBag.topTenProveedores = dao.ObtenerTopTen(EnumTipoReporteGrafico.Mensuales, EnumTipoGrafico.TopTenProvedores, idEstacion);
                ViewBag.InformacionGlobal=dao.ObtenerInformacionGlobal(EnumTipoReporteGrafico.Dia, idEstacion);
                ViewBag.mermaMensual = dao.ObtenerMermaMensual();
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public ActionResult _Grafico(EnumTipoGrafico tipoGrafico, EnumTipoReporteGrafico tipoReporteGrafico)
        {
            try
            {
                DashboardDAO dao = new DashboardDAO();
                Notificacion<Grafico> grafico = new Notificacion<Grafico>();
                Sesion usuario = Session["UsuarioActual"] as Sesion;
                int idEstacion = 0;
                if (usuario.idRol == 3)
                {
                    idEstacion = usuario.idEstacion;
                }

                if (tipoGrafico == EnumTipoGrafico.VentasPorFecha)
                {
                    //Notificacion<List<Estacion>> estaciones = dao.ObtenerVentasEstacion();
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

                        List<Dictionary<string,object>> _series = new List<Dictionary<string, object>> ();
                        Dictionary<string, object> _serie = new Dictionary<string, object>();
                        

                        categorias.Modelo.ForEach(x => {
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
                        Debug.Write(JsonConvert.SerializeObject(_series));
                        Debug.Write(JsonConvert.SerializeObject(_categorias));

                        ViewBag.categoriasPE = _categorias;
                        ViewBag.seriesPE = _series;
                    }
                                        
        

                }

                if (tipoGrafico == EnumTipoGrafico.TopTenProductos || tipoGrafico == EnumTipoGrafico.TopTenClientes || tipoGrafico == EnumTipoGrafico.TopTenProvedores)
                {
                    Notificacion<List<Categoria>> categorias = dao.ObtenerTopTen(tipoReporteGrafico, tipoGrafico);
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
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult CerrarSesion()
        {
            try
            {
                FormsAuthentication.SignOut();
                Session.Abandon();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

    }
}
