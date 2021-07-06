using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.Models
{
    public enum EnumTipoCliente
    {
        General = 1,
        A,
        B,
        VIP,
    }

    public enum EnumEstatusFactura
    {
        Facturada = 1,
        Cancelada,
        Error,

    }

    public enum EnumTipoGrafico
    {
        VentasPorFecha = 1,
        TopTenProductos = 2,
        TopTenClientes = 3,
        TopTenProvedores = 4,
        InformacionGlobal = 5
    }

    public enum EnumTipoReporteGrafico
    {
        Semanal = 1,
        Mensuales = 2,
        Anuales = 3,
        Dia = 4
    }

    public enum EnumTipoRetiro
    {
        [Description("Retiros Exceso Efectivo")]
        RetirosExcesoEfectivo = 1,

        [Description("Retiros Cierre Diario")]
        RetirosCierreDia = 2
    }

    public enum EnumTipoIngresoEfectivo
    {
        [Description("Apertura de cajas")]
        AperturaCajas = 1,

        [Description("Solicitud de efectivo")]
        SolicitudEfectivo = 2
    }

    public enum EnumTipoMargenBruto
    {
        Global=1,
        Linea=2,
        Producto=3,
        Venta_Producto=4
    }

    public enum EnumRolesPermisos
    {
        #region  DashBoard
        Puede_visualizar_DashBoard = 1,
        #endregion

        #region Usuarios
        Puede_visualizar_Usuarios,
        #endregion

        #region Estaciones
        Puede_visualizar_Estaciones,
        #endregion

        #region Proveedores
        Puede_visualizar_Proveedores,
        #endregion


        #region Compras
        Puede_visualizar_Compras,
        #endregion

        #region Ventas
        Puede_visualizar_Ventas,
        #endregion

        #region PedidosEspeciales
        Puede_visualizar_PedidosEspeciales=12,
        #endregion

        #region  Clientes
        Puede_visualizar_Clientes = 7,
        #endregion

        #region Productos
        Puede_visualizar_Productos=8,
        #endregion

        #region Bitacoras
        Puede_visualizar_Bitacoras,
        #endregion

        #region Reportes
        Puede_visualizar_Reportes,
        #endregion

        #region InventarioFisico
        Puede_visualizar_InventarioFisico

        #endregion

    }

    public enum EnumRoles
    {
        Administrador = 1,
        Encargado_de_almacen,
        Cajero,
        Montacarguista,
        Distribuidor,
        Picker,
        Almacenista,
        Usuario_de_Compras
    }

    public enum EnumEstadoCompras
    {
        Ninguno=0,
        Correcta=1,
        Incorrecta=2
    }

    public enum EnumTipoVenta
    {
        Normal = 1,
        Devolucion = 2,
        AgregarProductosVenta = 3,
        ProductosLiquidos=4,
        ConsultaDevolucion = 5
    }

    public enum EnumEstatusVenta
    {
        Activa = 1,
        Cancelada = 2
    }

    public enum TipoTicket
    { 
        Devolucion = 1,
        Complemento = 2
    }

}