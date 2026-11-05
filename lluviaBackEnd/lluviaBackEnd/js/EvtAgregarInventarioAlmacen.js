var catalogoProductos = {};   // idProducto -> { idProducto, descripcion, codigoBarras, unidadMedida }
var enviandoInventario = false;   // evita doble envio mientras la peticion esta en curso
var inventarioConfirmado = false; // el submit pasa solo despues de aceptar la confirmacion
var enviandoUbicacion = false;
var cantidadDisponibleUbicacion = 0;
var productoModal = '';
var almacenModal = '';

/* ---------- Utilidades ---------- */

function cantidadValida(texto) {
    var t = $.trim(texto);
    if (!/^\d+(\.\d+)?$/.test(t)) {
        return null;
    }
    var n = Number(t);
    return (isFinite(n) && n > 0) ? n : null;
}

function textoSeleccionado(selector) {
    return $.trim($(selector + ' option:selected').text());
}

function llenarSelectSimple(selector, items, getValue, getText) {
    var $sel = $(selector);
    $sel.empty().append($('<option>').val('').text('-- SELECCIONA --'));
    $.each(items || [], function (i, it) {
        $sel.append($('<option>').val(getValue(it)).text(getText(it)));
    });
}

function initSelect2(selector, dropdownParent) {
    var $sel = $(selector);
    if ($sel.hasClass('select2-hidden-accessible')) {
        $sel.select2('destroy');
    }
    var opciones = {
        width: "100%",
        language: {
            noResults: function () { return "No hay resultado"; },
            searching: function () { return "Buscando.."; }
        }
    };
    if (dropdownParent) {
        opciones.dropdownParent = dropdownParent;
    }
    $sel.select2(opciones);
}

/* ---------- Catalogos ---------- */

function CargarCatalogos() {
    $.ajax({
        url: rootUrl("/InventarioAlmacen/ObtenerCatalogos"),
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function () {
            ShowLoader("Cargando...");
        },
        success: function (data) {
            OcultarLoader();
            if (data.Estatus !== 200 || !data.Modelo) {
                MuestraToast("error", data.Mensaje || "No se pudo cargar la información");
                return;
            }
            var m = data.Modelo;
            llenarSelectSimple('#idAlmacen', m.almacenes, function (a) { return a.Value; }, function (a) { return a.Text; });
            llenarSelectSimple('#idProveedor', m.proveedores, function (p) { return p.Value; }, function (p) { return p.Text; });
            // 0 = sin ubicacion; no se ofrece como destino
            llenarSelectSimple('#selPasillo', $.grep(m.pasillos || [], function (x) { return x.idPasillo > 0; }), function (x) { return x.idPasillo; }, function (x) { return x.descripcion; });
            llenarSelectSimple('#selRaq', $.grep(m.raqs || [], function (x) { return x.idRaq > 0; }), function (x) { return x.idRaq; }, function (x) { return x.descripcion; });
            llenarSelectSimple('#selPiso', $.grep(m.pisos || [], function (x) { return x.idPiso > 0; }), function (x) { return x.idPiso; }, function (x) { return x.descripcion; });

            initSelect2('#idAlmacen');
            initSelect2('#idProveedor');
            initSelect2('#idProducto');
            initSelect2('#selPasillo', $('#mdlUbicacion'));
            initSelect2('#selRaq', $('#mdlUbicacion'));
            initSelect2('#selPiso', $('#mdlUbicacion'));
            $('#idProducto').prop('disabled', true);
        },
        error: function (xhr) {
            OcultarLoader();
            MuestraToast("error", xhr.status === 403 ? "No tiene permiso para agregar inventario a un almacén" : "Ocurrió un error al cargar la información");
        }
    });
}

function CargarProductos(idAlmacen) {
    $.each(catalogoProductos, function (k) { delete catalogoProductos[k]; });
    llenarSelectSimple('#idProducto', [], null, null);
    $('#txtUnidadProducto').val('');
    $('#idProducto').val('').trigger('change');
    LimpiarUbicaciones();

    if (!idAlmacen) {
        $('#idProducto').prop('disabled', true);
        return;
    }
    $.ajax({
        url: rootUrl("/InventarioAlmacen/ObtenerProductosAlmacen"),
        data: { idAlmacen: idAlmacen },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function () {
            ShowLoader("Cargando productos...");
        },
        success: function (data) {
            OcultarLoader();
            if (data.Estatus !== 200) {
                MuestraToast("error", data.Mensaje || "No se pudieron cargar los productos");
                return;
            }
            // El almacen pudo cambiar mientras respondia el servidor
            if ($('#idAlmacen').val() !== String(idAlmacen)) {
                return;
            }
            $.each(data.Modelo || [], function (i, p) { catalogoProductos[p.idProducto] = p; });
            llenarSelectSimple('#idProducto', data.Modelo,
                function (p) { return p.idProducto; },
                function (p) { return p.descripcion + (p.codigoBarras ? ' [' + p.codigoBarras + ']' : ''); });
            $('#idProducto').prop('disabled', false);
            initSelect2('#idProducto');
        },
        error: function (xhr) {
            OcultarLoader();
            MuestraToast("error", xhr.status === 403 ? "No tiene permiso para agregar inventario a un almacén" : "Ocurrió un error al cargar los productos");
        }
    });
}

/* ---------- Agregar inventario ---------- */

function onBeginSubmitInventario() {
    enviandoInventario = true;
    $('#btnAgregarInventario').prop('disabled', true);
    ShowLoader("Agregando al inventario...");
}

function finalizarEnvioInventario() {
    enviandoInventario = false;
    inventarioConfirmado = false;
    $('#btnAgregarInventario').prop('disabled', false);
    OcultarLoader();
}

function onFailureResultInventario() {
    finalizarEnvioInventario();
    MuestraToast("error", "Ocurrió un error al agregar el producto al inventario");
}

function onSuccessResultInventario(data) {
    finalizarEnvioInventario();
    if (data.Estatus === 200) {
        MuestraToast("success", data.Mensaje);
        $('#cantidad').val('');
        ConsultarUbicaciones();
    } else {
        MuestraToast("error", data.Mensaje);
    }
}

/* ---------- Ubicaciones ---------- */

function LimpiarUbicaciones() {
    $('#ViewUbicaciones').html('<p class="text-muted mb-0">Seleccione el almacén y el producto, o agregue inventario, para ver sus ubicaciones.</p>');
}

function ConsultarUbicaciones() {
    var idAlmacen = $('#idAlmacen').val();
    var idProducto = $('#idProducto').val();
    if (!idAlmacen || !idProducto) {
        MuestraToast("error", "Selecciona el almacén y el producto");
        return;
    }
    $.ajax({
        url: rootUrl("/InventarioAlmacen/_ObtenerUbicaciones"),
        data: { idAlmacen: idAlmacen, idProducto: idProducto },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function () {
            ShowLoader("Consultando ubicaciones...");
        },
        success: function (data) {
            OcultarLoader();
            $('#ViewUbicaciones').html(data);
        },
        error: function (xhr) {
            OcultarLoader();
            MuestraToast("error", xhr.status === 403 ? "No tiene permiso para agregar inventario a un almacén" : "Ocurrió un error al consultar las ubicaciones");
        }
    });
}

function AbrirAsignarUbicacion(idUbicacion, cantidad, producto, almacen) {
    cantidadDisponibleUbicacion = Number(cantidad);
    var p = catalogoProductos[$('#idProducto').val()];
    productoModal = $.trim(producto) || (p ? p.descripcion : '') || textoSeleccionado('#idProducto');
    almacenModal = $.trim(almacen) || textoSeleccionado('#idAlmacen');
    $('#lblModalProducto').text(productoModal);
    $('#lblModalAlmacen').text(almacenModal);
    $('#lblModalDisponible').text(cantidad + (p && p.unidadMedida ? ' ' + p.unidadMedida : ''));
    $('#hidIdUbicacion').val(idUbicacion);
    $('#txtCantidadUbicacion').val(String(cantidad));
    $('#lblCantidadDisponible').text('(disponible: ' + cantidad + ')');
    $('#selPasillo, #selRaq, #selPiso').val('').trigger('change');
    $('#mdlUbicacion').modal({ backdrop: 'static', keyboard: false, show: true });
}

function GuardarUbicacion() {
    if (enviandoUbicacion) {
        return;
    }
    var cantidad = cantidadValida($('#txtCantidadUbicacion').val());
    var idPasillo = $('#selPasillo').val();
    var idRaq = $('#selRaq').val();
    var idPiso = $('#selPiso').val();

    if (cantidad === null) {
        MuestraToast("error", "La cantidad a acomodar debe ser mayor a 0");
        return;
    }
    if (cantidad > cantidadDisponibleUbicacion) {
        MuestraToast("error", "La cantidad a acomodar no puede ser mayor a la disponible (" + cantidadDisponibleUbicacion + ")");
        return;
    }
    if (!idPasillo || !idRaq || !idPiso) {
        MuestraToast("error", "Selecciona el pasillo, el raq y el piso");
        return;
    }

    swal({
        title: 'Mensaje',
        text: '¿Estás seguro que deseas acomodar ' + cantidad + ' de "' + productoModal + '" del almacén "' + almacenModal +
            '" en Pasillo ' + textoSeleccionado('#selPasillo') + ' · Raq ' + textoSeleccionado('#selRaq') +
            ' · Piso ' + textoSeleccionado('#selPiso') + '?',
        icon: 'info',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then(function (acepto) {
            if (!acepto || enviandoUbicacion) {
                return;
            }
            enviandoUbicacion = true;
            $('#btnGuardarUbicacion').prop('disabled', true);
            $.ajax({
                url: rootUrl("/InventarioAlmacen/ActualizarUbicacion"),
                data: {
                    idAlmacen: $('#idAlmacen').val(),
                    idProducto: $('#idProducto').val(),
                    idUbicacion: $('#hidIdUbicacion').val(),
                    idPasillo: idPasillo,
                    idRaq: idRaq,
                    idPiso: idPiso,
                    cantidad: String(cantidad)
                },
                method: 'post',
                dataType: 'json',
                async: true,
                beforeSend: function () {
                    ShowLoader("Asignando ubicación...");
                },
                success: function (data) {
                    OcultarLoader();
                    MuestraToast(data.Estatus === 200 ? "success" : "error", data.Mensaje);
                    if (data.Estatus === 200) {
                        $('#mdlUbicacion').modal('hide');
                        ConsultarUbicaciones();
                    }
                },
                error: function () {
                    OcultarLoader();
                    MuestraToast("error", "Ocurrió un error al asignar la ubicación");
                },
                complete: function () {
                    enviandoUbicacion = false;
                    $('#btnGuardarUbicacion').prop('disabled', false);
                }
            });
        });
}

/* ---------- Eventos ---------- */

function initEvents() {
    $('#idAlmacen').on('change', function () {
        CargarProductos($(this).val());
    });

    // Unidad automática (solo lectura) según el producto elegido
    $('#idProducto').on('change', function () {
        var p = catalogoProductos[$(this).val()];
        $('#txtUnidadProducto').val(p ? p.unidadMedida : '');
    });

    $('#btnConsultarUbicaciones').on('click', ConsultarUbicaciones);

    $('#ViewUbicaciones').on('click', '.btn-asignar-ubicacion', function (e) {
        e.preventDefault();
        AbrirAsignarUbicacion($(this).data('id-ubicacion'), String($(this).attr('data-cantidad')), $(this).attr('data-producto'), $(this).attr('data-almacen'));
    });

    $('#btnGuardarUbicacion').on('click', GuardarUbicacion);

    // Enter no debe enviar el formulario por accidente (mueve inventario real)
    $('#frmInventario').on('keydown', 'input', function (e) {
        if (e.which === 13) {
            e.preventDefault();
        }
    });

    // Se ejecuta antes del manejador de jquery.unobtrusive-ajax (delegado en document)
    $('#frmInventario').on('submit', function (e) {
        if (enviandoInventario) {
            e.preventDefault();
            e.stopPropagation();
            return;
        }
        if (inventarioConfirmado) {
            // Ya confirmado: deja pasar el submit al manejador de unobtrusive-ajax
            inventarioConfirmado = false;
            return;
        }

        e.preventDefault();
        e.stopPropagation();

        var idAlmacen = $('#idAlmacen').val();
        var idProducto = $('#idProducto').val();
        var idProveedor = $('#idProveedor').val();
        var cantidad = cantidadValida($('#cantidad').val());

        if (!idAlmacen) { MuestraToast("error", "Selecciona el almacén"); return; }
        if (!idProducto) { MuestraToast("error", "Selecciona el producto"); return; }
        if (!idProveedor) { MuestraToast("error", "Selecciona el proveedor"); return; }
        if (cantidad === null) { MuestraToast("error", "La cantidad debe ser un número mayor a 0"); return; }

        var p = catalogoProductos[idProducto];
        var unidad = p && p.unidadMedida ? ' ' + p.unidadMedida : '';
        swal({
            title: 'Mensaje',
            text: '¿Estás seguro que deseas agregar ' + cantidad + unidad + ' de "' + (p ? p.descripcion : textoSeleccionado('#idProducto')) +
                '" al inventario del almacén "' + textoSeleccionado('#idAlmacen') + '"? Esta acción modifica el inventario.',
            icon: 'info',
            buttons: ["Cancelar", "Aceptar"],
            dangerMode: true,
        })
            .then(function (acepto) {
                if (!acepto || enviandoInventario) {
                    return;
                }
                inventarioConfirmado = true;
                $('#frmInventario').trigger('submit');
            });
    });
}

$(document).ready(function () {
    initEvents();
    CargarCatalogos();
});
