using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lluviaBackEnd.WebServices.Modelos.Request
{
    public class RequestObtenerLimitesInventario
    {
        public int idProducto{ get; set; }
        public int idAlmacen { get; set; }

        /// <summary>
        /// 1	Invietario dentro de sus limites 
        /// 2	Cantidad superior por el maximo permitido
        /// 3	Cantidad por debajo del minimo permitido
        /// </summary>
        public int idEstatusLimiteInv { get; set; }
    }
}