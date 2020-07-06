using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestAgreagrProductoInventario
    {
        public int idProveedor { get; set; }
        public int idProducto { get; set; }
        public int idUsuario { get; set; }

        public int cantidad { get; set; }
        public int idAlmacen { get; set; }
        /// <summary>
        /// especifica el medio por el cual se esta afectando el inventario
        ///    1	Venta
        ///    2	Resutir
        ///    3	Cancelacion
        ///    4	Carga de Inventario
        ///    5	Actualizacion de Inventario(asignacion de la ubicacion fisica del producto)
        ///    6	Actualizacion de Inventario(sobrante del producto sin ubicacion asignada)
        ///    7	Actualizacion de Inventario(salida de mercancia por pedido interno)
        ///    8	Actualizacion de Inventario(carga de mercancia por pedido interno)
        /// </summary>
        public int idTipoMovInventario { get; set; }
    }
}