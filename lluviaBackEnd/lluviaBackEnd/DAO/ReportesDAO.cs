using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccesoDatos;
using lluviaBackEnd.Models;
using lluviaBackEnd;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace lluviaBackEnd.DAO
{
    public class ReportesDAO
    {
        private IDbConnection db = null;


        public Notificacion<List<Producto>> ObtenerInventario(Producto producto)
        {
            Notificacion<List<Producto>> notificacion = new Notificacion<List<Producto>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", producto.idProducto==0 ? (object) null : producto.idProducto);
                    parameters.Add("@descripcion", string.IsNullOrEmpty(producto.descripcion) ? (object)null : producto.descripcion);                    
                    parameters.Add("@idLineaProducto", string.IsNullOrEmpty(producto.idLineaProducto) ? (object)null : Convert.ToInt32(producto.idLineaProducto));                    
                    parameters.Add("@articulo", string.IsNullOrEmpty(producto.articulo) ? (object)null : producto.articulo);
                    parameters.Add("@idAlmacen", producto.idAlmacen==0 ? (object)null : producto.idAlmacen);
                    parameters.Add("@fecha", producto.fechaAlta == DateTime.MinValue ? (object)System.DateTime.Now.ToString("yyyyMMdd") : producto.fechaAlta);
                    //parameters.Add("@fechaIni", producto.fechaIni==DateTime.MinValue ? (object)null : producto.fechaIni);
                    //parameters.Add("@fechaFin", producto.fechaFin == DateTime.MinValue ? (object)null : producto.fechaFin);
                    var result = db.QueryMultiple("SP_CONSULTA_INVENTARIO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Producto>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        //notificacion.Modelo.Clear();// [0] = producto;
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public Notificacion<List<Ventas>> ObtenerVentas(Ventas ventas)
        {
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();

            try
            {
                if (ventas.fechaIni == (new DateTime(0001, 01, 01)))
                    ventas.fechaIni = new DateTime(1900, 01, 01);

                if (ventas.fechaFin == (new DateTime(0001, 01, 01)))
                    ventas.fechaFin = new DateTime(1900, 01, 01);

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProducto", ventas.idProducto);
                    parameters.Add("@descProducto", ventas.descripcionProducto);
                    parameters.Add("@idLineaProducto", ventas.idLineaProducto);
                    parameters.Add("@idCliente", ventas.idCliente);
                    parameters.Add("@idUsuario", ventas.idUsuario);
                    parameters.Add("@fechaIni", ventas.fechaIni.ToString("yyyyMMdd"));
                    parameters.Add("@fechaFin", ventas.fechaFin.ToString("yyyyMMdd"));
                    parameters.Add("@tipoConsulta", ventas.tipoConsulta);
                    parameters.Add("@idStatusVenta", ventas.estatusVenta == 0 ? 1: ventas.estatusVenta);
                    parameters.Add("@idFactFormaPago", ventas.idFactFormaPago == 0 ? (object)null : ventas.idFactFormaPago);
                    parameters.Add("@idAlmacen", ventas.idAlmacen == 0 ? (object)null : ventas.idAlmacen);
                    var result = db.QueryMultiple("SP_CONSULTA_VENTAS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Ventas>().ToList();
                        notificacion.Modelo.ForEach(p => p.rutaFactura = ConfigurationManager.AppSettings["urlDominio"].ToString() + "/" + p.rutaFactura);

                    
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<List<MargenBruto>> ObtenerMargenBruto(MargenBruto margenBruto)
        {
            Notificacion<List<MargenBruto>> notificacion = new Notificacion<List<MargenBruto>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idTipoMargenBruto", margenBruto.tipoMargenBruto);
                   parameters.Add("@fechaIni", margenBruto.fechaIni == DateTime.MinValue ? (object)null : margenBruto.fechaIni);
                    parameters.Add("@fechaFin", margenBruto.fechaFin == DateTime.MinValue ? (object)null : margenBruto.fechaFin);
                    var result = db.QueryMultiple("SP_INDICADOR_MARGEN_BRUTO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<MargenBruto>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public Notificacion<List<DiasPromedioInventario>> ObtenerDiasPromedioInventario(DiasPromedioInventario diasPromedioInventario)
        {
            Notificacion<List<DiasPromedioInventario>> notificacion = new Notificacion<List<DiasPromedioInventario>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idTipoInventarioPromedio", diasPromedioInventario.tipoMargenBruto);
                    parameters.Add("@fechaIni", diasPromedioInventario.fechaIni == DateTime.MinValue ? (object)null : diasPromedioInventario.fechaIni);
                    parameters.Add("@fechaFin", diasPromedioInventario.fechaFin == DateTime.MinValue ? (object)null : diasPromedioInventario.fechaFin);
                    var result = db.QueryMultiple("SP_INDICADOR_DIAS_PROMEDIO_INVENTARIO", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<DiasPromedioInventario>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public Notificacion<List<DropSize>> ObtenerDropSize(DropSize dropSize)
        {
            Notificacion<List<DropSize>> notificacion = new Notificacion<List<DropSize>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idTipoDropSize", dropSize.tipoMargenBruto);
                    parameters.Add("@fechaIni", dropSize.fechaIni == DateTime.MinValue ? (object)null : dropSize.fechaIni);
                    parameters.Add("@fechaFin", dropSize.fechaFin == DateTime.MinValue ? (object)null : dropSize.fechaFin);
                    var result = db.QueryMultiple("SP_INDICADOR_DROPSIZE", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<DropSize>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;

        }

        public List<DevolucionProveedor> ObtenerDevolucionesProveedor(Proveedor proveedor)
        {
            List<DevolucionProveedor> devoluciones = new List<DevolucionProveedor>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idProveedor", proveedor.idProveedor==0 ? (object) null : proveedor.idProveedor);
                    parameters.Add("@fechaIni", proveedor.fechaInicio == DateTime.MinValue ? (object)null : proveedor.fechaInicio);
                    parameters.Add("@fechaFin", proveedor.fechaFin == DateTime.MinValue ? (object)null : proveedor.fechaFin);
                    var result = db.QueryMultiple("SP_CONSULTA_DEVOLUCIONES_PROVEEDOR", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        devoluciones = result.Read<DevolucionProveedor,Producto,Usuario,Proveedor,DevolucionProveedor>(MapDevolucionProveedor,splitOn:"idProducto,idUsuario,idProveedor").ToList();
                    }
                   


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return devoluciones;

        }



        public Notificacion<List<Ventas>> ObtenerDevoluciones(Ventas ventas)
        {
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();

            try
            {
                if (ventas.fechaIni == (new DateTime(0001, 01, 01)))
                    ventas.fechaIni = new DateTime(1900, 01, 01);

                if (ventas.fechaFin == (new DateTime(0001, 01, 01)))
                    ventas.fechaFin = new DateTime(1900, 01, 01);

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idVenta", ventas.idVenta);
                    parameters.Add("@idAlmacen", ventas.idAlmacen);
                    parameters.Add("@idUsuario", ventas.idUsuario);
                    parameters.Add("@fechaIni", ventas.fechaIni);
                    parameters.Add("@fechaFin", ventas.fechaFin);                

                    var result = db.QueryMultiple("SP_CONSULTA_DEVOLUCIONES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Ventas>().ToList();
                        //notificacion.Modelo.ForEach(p => p.rutaFactura = ConfigurationManager.AppSettings["urlDominio"].ToString() + "/" + p.rutaFactura);


                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<List<Ventas>> ObtenerDevolucionesyComplementos(Ventas ventas,int tipoTicket)
        {
            Notificacion<List<Ventas>> notificacion = new Notificacion<List<Ventas>>();

            try
            {               
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idVenta", ventas.idVenta==0 ? (object) null : ventas.idVenta);
                    parameters.Add("@idAlmacen", ventas.idAlmacen == 0 ? (object)null : ventas.idAlmacen);
                    parameters.Add("@idUsuario", ventas.idUsuario == 0 ? (object)null : ventas.idUsuario);
                    parameters.Add("@fechaIni", ventas.fechaIni == DateTime.MinValue ? (object)null : ventas.fechaIni);
                    parameters.Add("@fechaFin", ventas.fechaFin == DateTime.MinValue ? (object)null : ventas.fechaFin);
                    parameters.Add("@idTipoConsulta", tipoTicket == 0 ? (object)null : tipoTicket);
                    

                    var result = db.QueryMultiple("SP_CONSULTA_DEVOLUCIONES_Y_COMPLEMENTOS", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Ventas>().ToList();
                        //notificacion.Modelo.ForEach(p => p.rutaFactura = ConfigurationManager.AppSettings["urlDominio"].ToString() + "/" + p.rutaFactura);


                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }



        public DevolucionProveedor MapDevolucionProveedor(DevolucionProveedor d,Producto p,Usuario u,Proveedor pr)
        {
            d.producto = p;
            d.usuario = u;
            d.proveedor = pr;
            return d;
        }

        //public Notificacion<List<AjusteInventarioFisico>> ObtenerMerma(AjusteInventarioFisico ajusteInventarioFisico)
        //{
        //    Notificacion<List<AjusteInventarioFisico>> notificacion = new Notificacion<List<AjusteInventarioFisico>>();
        //    try
        //    {
        //        using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
        //        {
        //            DynamicParameters parameters = new DynamicParameters();
        //            parameters.Add("@idInventarioFisico", ajusteInventarioFisico.idInventarioFisico == 0 ? (object)null : ajusteInventarioFisico.idInventarioFisico);
        //            parameters.Add("@idLinea", string.IsNullOrEmpty(ajusteInventarioFisico.producto.idLineaProducto) ? (object)null : ajusteInventarioFisico.producto.idLineaProducto);
        //            parameters.Add("@idAlmacen", ajusteInventarioFisico.producto.idAlmacen == 0 ? (object)null : ajusteInventarioFisico.producto.idAlmacen);

        //            var result = db.QueryMultiple("SP_CONSULTA_MERMA", parameters, commandType: CommandType.StoredProcedure);
        //            var rs1 = result.ReadFirst();
        //            if (rs1.status == 200)
        //            {
        //                notificacion.Estatus = rs1.status;
        //                notificacion.Mensaje = rs1.mensaje;
        //                notificacion.Modelo=result.Read<AjusteInventarioFisico, Producto, AjusteInventarioFisico>(MapAjusteInventarioFisico, splitOn: "idProducto").ToList();
        //            }
        //            else
        //            {
        //                notificacion.Estatus = rs1.status;
        //                notificacion.Mensaje = rs1.mensaje;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return notificacion;
        //}

        public Notificacion<List<Merma>> ObtenerMerma(Merma filtro)
        {
            Notificacion<List<Merma>> notificacion = new Notificacion<List<Merma>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@mesCalculo", filtro.MesCalculo == 0 ? (object)null : filtro.MesCalculo);
                    parameters.Add("@anioCalculo", filtro.AnioCalculo == 0 ? (object)null : filtro.AnioCalculo);
                    parameters.Add("@idLinea", filtro.idLineaProducto == 0 ?  (object)null : filtro.idLineaProducto);
                    parameters.Add("@idAlmacen", filtro.idAlmacen == 0 ?  (object)null : filtro.idAlmacen);

                    var result = db.QueryMultiple("SP_CONSULTA_MERMA", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = result.ReadFirst();
                    if (rs1.status == 200)
                    {
                        notificacion.Estatus = rs1.status;
                        notificacion.Mensaje = rs1.mensaje;
                        notificacion.Modelo = result.Read<Merma>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = rs1.status;
                        notificacion.Mensaje = rs1.mensaje;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public AjusteInventarioFisico MapAjusteInventarioFisico(AjusteInventarioFisico i, Producto p)
        {
            i.producto = p;
            return i;
        }

        public Notificacion<List<Cierre>> ConsultaCierresCaja(Cierre cierre)
        {
            Notificacion<List<Cierre>> notificacion = new Notificacion<List<Cierre>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", cierre.idUsuario==0 ? (object)null : cierre.idUsuario);
                    parameters.Add("@idAlmacen", cierre.idAlmacen == 0 ? (object)null : cierre.idAlmacen);
                    parameters.Add("@fechaIni", cierre.fechaIni==DateTime.MinValue? (object)null : cierre.fechaIni);
                    parameters.Add("@fechaFin", cierre.fechaFin == DateTime.MinValue ? (object)null : cierre.fechaFin);
                    var result = db.QueryMultiple("SP_CONSULTA_CIERRES_DIA", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<Cierre>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new List<Cierre>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public Notificacion<List<PedidosEspecialesV2>> ObtenerVentasPedidosEspeciales(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();

            try
            {
                DateTime hoy = new DateTime();
                hoy = DateTime.Now;

                if (pedidosEspecialesV2.fechaIni == (new DateTime(0001, 01, 01)))
                {
                    pedidosEspecialesV2.fechaIni = new DateTime(hoy.Year, hoy.Month, hoy.Day);
                }

                if (pedidosEspecialesV2.fechaFin == (new DateTime(0001, 01, 01)))
                {
                    pedidosEspecialesV2.fechaFin = new DateTime(hoy.Year, hoy.Month, hoy.Day);
                }

                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idCliente", pedidosEspecialesV2.idCliente);
                    parameters.Add("@idUsuario", pedidosEspecialesV2.idUsuario);
                    parameters.Add("@fechaIni", pedidosEspecialesV2.fechaIni.ToString("yyyyMMdd"));
                    parameters.Add("@fechaFin", pedidosEspecialesV2.fechaFin.ToString("yyyyMMdd"));

                    var result = db.QueryMultiple("SP_CONSULTA_VENTAS_PEDIDOS_ESPECIALESV2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<PedidosEspecialesV2>().ToList();
                        notificacion.Modelo.ForEach(p => p.rutaFactura = ConfigurationManager.AppSettings["urlDominio"].ToString() + "/" + p.rutaFactura);
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }



        public Notificacion<List<PedidosEspecialesV2>> ObtenerDevolucionesPedidosEspeciales(PedidosEspecialesV2 pedidosEspecialesV2)
        {
            Notificacion<List<PedidosEspecialesV2>> notificacion = new Notificacion<List<PedidosEspecialesV2>>();

            try
            {
                DateTime hoy = new DateTime();
                hoy = DateTime.Now;

                if (pedidosEspecialesV2.fechaIni == (new DateTime(0001, 01, 01)))
                {
                    pedidosEspecialesV2.fechaIni = new DateTime(hoy.Year, hoy.Month, hoy.Day);
                }

                if (pedidosEspecialesV2.fechaFin == (new DateTime(0001, 01, 01)))
                {
                    pedidosEspecialesV2.fechaFin = new DateTime(hoy.Year, hoy.Month, hoy.Day);
                }


                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@idPedidoEspecial", pedidosEspecialesV2.idPedidoEspecial == 0 ? (object)null : pedidosEspecialesV2.idPedidoEspecial);
                    parameters.Add("@idAlmacen", pedidosEspecialesV2.idAlmacen == 0 ? (object)null : pedidosEspecialesV2.idAlmacen);
                    parameters.Add("@idUsuario", pedidosEspecialesV2.idUsuario == 0 ? (object)null : pedidosEspecialesV2.idUsuario);
                    parameters.Add("@fechaIni", pedidosEspecialesV2.fechaIni);
                    parameters.Add("@fechaFin", pedidosEspecialesV2.fechaFin);

                    var result = db.QueryMultiple("SP_CONSULTA_DEVOLUCIONES_PEDIDOS_ESPECIALESV2", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<PedidosEspecialesV2>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }


        public List<SelectListItem> ObtenerMeses(int anio)
        {
            List<SelectListItem> lstMeses = new List<SelectListItem>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@anio", anio==0 ? (Object) null : anio);
                    var result = db.QueryMultiple("SP_CONSULTA_MESES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        lstMeses = result.Read<SelectListItem>().ToList();
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstMeses;
        }

        public List<SelectListItem> ObtenerAnios()
        {
            List<SelectListItem> lstAnios = new List<SelectListItem>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {

                    var result = db.QueryMultiple("SP_CONSULTA_ANIOS", null, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();

                    if (r1.status == 200)
                    {
                        lstAnios = result.Read<SelectListItem>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstAnios;
        }

        public Notificacion<List<CierrePedidosEspeciales>> ConsultaCierresPedidosEspeciales(Filtro cierre)
        {
            Notificacion<List<CierrePedidosEspeciales>> notificacion = new Notificacion<List<CierrePedidosEspeciales>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@idUsuario", cierre.idUsuario == 0 ? (object)null : cierre.idUsuario);                    
                    parameters.Add("@fechaIni", cierre.fechaIni == DateTime.MinValue ? (object)null : cierre.fechaIni);
                    parameters.Add("@fechaFin", cierre.fechaFin == DateTime.MinValue ? (object)null : cierre.fechaFin);
                    Console.WriteLine(cierre.idUsuario+" " + cierre.fechaIni+" "+ cierre.fechaFin);
                    var result = db.QueryMultiple("SP_REPORTE_CIERRES_PEDIDOS_ESPECIALES", parameters, commandType: CommandType.StoredProcedure);
                    var r1 = result.ReadFirst();
                    if (r1.status == 200)
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = result.Read<CierrePedidosEspeciales>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = r1.status;
                        notificacion.Mensaje = r1.mensaje;
                        notificacion.Modelo = new List<CierrePedidosEspeciales>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }

        public Notificacion<List<CostoProduccionAgranel>> ObtenerReporteCostoProduccionAgranel(Merma filtro)
        {
            Notificacion<List<CostoProduccionAgranel>> notificacion = new Notificacion<List<CostoProduccionAgranel>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@mesCalculo", filtro.MesCalculo == 0 ? (object)null : filtro.MesCalculo);
                    parameters.Add("@anioCalculo", filtro.AnioCalculo == 0 ? (object)null : filtro.AnioCalculo);
                    parameters.Add("@idLinea", filtro.idLineaProducto == 0 ? (object)null : filtro.idLineaProducto);
                    parameters.Add("@idAlmacen", filtro.idAlmacen == 0 ? (object)null : filtro.idAlmacen);

                    var result = db.QueryMultiple("SP_CONSULTA_COSTO_PRODUCCION", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = result.ReadFirst();
                    if (rs1.status == 200)
                    {
                        notificacion.Estatus = rs1.status;
                        notificacion.Mensaje = rs1.mensaje;
                        notificacion.Modelo = result.Read<CostoProduccionAgranel>().ToList();
                    }
                    else
                    {
                        notificacion.Estatus = rs1.status;
                        notificacion.Mensaje = rs1.mensaje;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return notificacion;
        }



    }
}