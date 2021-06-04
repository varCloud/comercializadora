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
                    parameters.Add("@fechaIni", producto.fechaIni==DateTime.MinValue ? (object)null : producto.fechaIni);
                    parameters.Add("@fechaFin", producto.fechaFin == DateTime.MinValue ? (object)null : producto.fechaFin);
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
                    parameters.Add("@fechaIni", ventas.fechaIni);
                    parameters.Add("@fechaFin", ventas.fechaFin);
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


        public DevolucionProveedor MapDevolucionProveedor(DevolucionProveedor d,Producto p,Usuario u,Proveedor pr)
        {
            d.producto = p;
            d.usuario = u;
            d.proveedor = pr;
            return d;
        }

        public Notificacion<List<AjusteInventarioFisico>> ObtenerMerma(AjusteInventarioFisico ajusteInventarioFisico)
        {
            Notificacion<List<AjusteInventarioFisico>> notificacion = new Notificacion<List<AjusteInventarioFisico>>();
            try
            {
                using (db = new SqlConnection(ConfigurationManager.AppSettings["conexionString"].ToString()))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@idInventarioFisico", ajusteInventarioFisico.idInventarioFisico == 0 ? (object)null : ajusteInventarioFisico.idInventarioFisico);
                    parameters.Add("@idLinea", string.IsNullOrEmpty(ajusteInventarioFisico.producto.idLineaProducto) ? (object)null : ajusteInventarioFisico.producto.idLineaProducto);
                    parameters.Add("@idAlmacen", ajusteInventarioFisico.producto.idAlmacen == 0 ? (object)null : ajusteInventarioFisico.producto.idAlmacen);

                    var result = db.QueryMultiple("SP_CONSULTA_MERMA", parameters, commandType: CommandType.StoredProcedure);
                    var rs1 = result.ReadFirst();
                    if (rs1.status == 200)
                    {
                        notificacion.Estatus = rs1.status;
                        notificacion.Mensaje = rs1.mensaje;
                        notificacion.Modelo=result.Read<AjusteInventarioFisico, Producto, AjusteInventarioFisico>(MapAjusteInventarioFisico, splitOn: "idProducto").ToList();
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


    }
}