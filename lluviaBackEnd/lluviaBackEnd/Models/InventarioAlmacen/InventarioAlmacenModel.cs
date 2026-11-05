using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace lluviaBackEnd.Models
{
    // Opcion de producto para el select de la pantalla "Agregar inventario almacen"
    public class InventarioAlmacenProductoModel
    {
        public int idProducto { get; set; }
        public string descripcion { get; set; }
        public string codigoBarras { get; set; }
        public string unidadMedida { get; set; }
    }

    // Catalogos de la pantalla: almacenes, proveedores y catalogo de pasillos/raqs/pisos para asignar la ubicacion
    public class InventarioAlmacenCatalogosModel
    {
        public List<SelectListItem> almacenes { get; set; }
        public List<SelectListItem> proveedores { get; set; }
        public List<Pasillo> pasillos { get; set; }
        public List<Raq> raqs { get; set; }
        public List<Piso> pisos { get; set; }
    }

    // Modelo del formulario (Ajax.BeginForm). La cantidad llega como texto y se interpreta con cultura invariante (punto decimal).
    public class AgregarInventarioAlmacenModel
    {
        [Required(ErrorMessage = "Selecciona el almacen")]
        public string idAlmacen { get; set; }

        [Required(ErrorMessage = "Selecciona el producto")]
        public string idProducto { get; set; }

        [Required(ErrorMessage = "Selecciona el proveedor")]
        public string idProveedor { get; set; }

        [Required(ErrorMessage = "Captura la cantidad")]
        public string cantidad { get; set; }
    }

    // Asignacion de pasillo/raq/piso a la cantidad "sin acomodar" de un producto
    public class ActualizarUbicacionInventarioAlmacenModel
    {
        public int idAlmacen { get; set; }
        public int idProducto { get; set; }
        public int idUbicacion { get; set; }
        public int idPasillo { get; set; }
        public int idRaq { get; set; }
        public int idPiso { get; set; }
        // Texto: se interpreta con cultura invariante (punto decimal)
        public string cantidad { get; set; }
    }
}
