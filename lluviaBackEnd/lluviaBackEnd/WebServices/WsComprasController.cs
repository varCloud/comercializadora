using AutoMapper;
using lluviaBackEnd.DAO;
using lluviaBackEnd.Models;
using lluviaBackEnd.WebServices.Modelos;
using lluviaBackEnd.WebServices.Modelos.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace lluviaBackEnd.WebServices
{
    public class WsComprasController : ApiController
    {

        [HttpPost]
        public Notificacion<List<Compras>> ObtenerCompras(RequestObtenerCompras request)
        {
            try
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<RequestObtenerCompras, Compras>());
                var mapper = config.CreateMapper();
                Compras compra = mapper.Map<Compras>(request);

                return new ComprasDAO().ObtenerComprasApp(compra, false);
            }
            catch (Exception ex)
            {
                return WsUtils<List<Compras>>.RegresaExcepcion(ex, null);
            }
        }

        [HttpPost]
        public Notificacion<List<CompraDetalle>> ObtenerDetalleCompra(RequestObtenerDetalleCompra request)
        {
            try
            {
                return new ComprasDAO().ObtenerDetalleCompra(request);
            }
            catch (Exception ex)
            {
                return WsUtils<List<CompraDetalle>>.RegresaExcepcion(ex, null);
            }
        }
        [HttpPost]
        public Notificacion<String> ActualizarEstatusProductoCompra(RequestActualizaEstatusProductoCompra request)
        {
            try
            {
                return new ComprasDAO().ActualizarEstatusProductoCompra(request);
            }
            catch (Exception ex)
            {
                return WsUtils<String>.RegresaExcepcion(ex, null);
            }
        }


    }
}
