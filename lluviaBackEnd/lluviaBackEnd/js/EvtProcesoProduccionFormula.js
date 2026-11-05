var tblFormulas;
var catalogoProductos = {};      // idProducto -> { idProducto, descripcion, unidadMedida }
var catalogoMateriasPrimas = {}; // idProducto -> { idProducto, descripcion, unidadMedida }
var TITULO_REPORTE_FORMULAS = "Relación Producción";
var LIMITE_LOTE = 10000;

/* ---------- Listado ---------- */

function ObtenerFormulas() {
    $.ajax({
        url: rootUrl("/ProcesoProduccionFormula/_ObtenerFormulas"),
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function () {
            ShowLoader("Cargando fórmulas...");
        },
        success: function (data) {
            OcultarLoader();
            tblFormulas?.destroy();
            $('#ViewFormulas').html(data);
            initTblFormulas();
        },
        error: function (xhr) {
            OcultarLoader();
            MuestraToast("error", xhr.status === 403 ? "No tiene permiso para administrar las fórmulas de producción" : "Ocurrió un error al cargar las fórmulas");
        }
    });
}

function initTblFormulas() {
    var NombreTabla = "tblFormulas";
    tblFormulas = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblFormulas, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: TITULO_REPORTE_FORMULAS,
                filename: 'Relacion_Produccion',
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['6%', '38%', '16%', '20%', '20%'];

                    doc.content.splice(0, 1);
                    doc.pageMargins = [30, 85, 20, 30];
                    doc['header'] = SetHeaderPDF(TITULO_REPORTE_FORMULAS);
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                title: TITULO_REPORTE_FORMULAS,
                filename: 'Relacion_Produccion',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            }
        ]
    });

    tblFormulas.buttons(0, null).container().prependTo(
        tblFormulas.table().container()
    );

    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" id="btnAgregarFormula" data-toggle="tooltip" title="Agregar fórmula"><i class="fas fa-plus"></i></a>');
}

/* ---------- Formulario ---------- */

function initSelect2Formulario() {
    $('#idProducto, #selMateriaPrima').each(function () {
        if ($(this).hasClass('select2-hidden-accessible')) {
            $(this).select2('destroy');
        }
    });
    $('#idProducto, #selMateriaPrima').select2({
        width: "100%",
        dropdownParent: $('#mdlFormula'),
        language: {
            noResults: function () { return "No hay resultado"; },
            searching: function () { return "Buscando.."; }
        }
    });
}

function llenarSelect(selector, productos, mapa) {
    var $sel = $(selector);
    $sel.empty().append($('<option>').val('').text('-- SELECCIONA --'));
    $.each(mapa, function (k) { delete mapa[k]; });
    $.each(productos || [], function (i, p) {
        mapa[p.idProducto] = p;
        $sel.append($('<option>').val(p.idProducto).text(p.descripcion));
    });
}

function resetForm() {
    $('#frmFormula .field-validation-error').html("");
    $('#idProcesoProduccionFormula').val('0');
    $('#cantidadLote').val('');
    $('#txtUnidadProducto').val('');
    $('#txtUnidadMateriaPrima').val('');
    $('#txtCantidadMateriaPrima').val('');
    $('#tblMateriasPrimas tbody').empty();
    $('#idProducto').val('').trigger('change');
    $('#selMateriaPrima').val('').trigger('change');
}

function disabledForm(soloLectura) {
    $('#frmFormula input, #frmFormula select').prop('disabled', soloLectura);
    $('#rowAgregarMateriaPrima').toggle(!soloLectura);
    $('#btnGuardarFormula').toggle(!soloLectura);
    $('#tblMateriasPrimas .btn-quitar-mp').toggle(!soloLectura);
}

// Abre el formulario. accion: 'agregar' | 'editar' | 'ver'. fila (opcional): { idProducto, cantidadLote }
function AbrirFormulario(accion, id, fila) {
    $.ajax({
        url: rootUrl("/ProcesoProduccionFormula/ObtenerDatosFormulario"),
        data: { idProcesoProduccionFormula: id || 0 },
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
            resetForm();
            disabledForm(false);
            llenarSelect('#idProducto', data.Modelo.productos, catalogoProductos);
            llenarSelect('#selMateriaPrima', data.Modelo.materiasPrimas, catalogoMateriasPrimas);
            initSelect2Formulario();

            if (accion !== 'agregar') {
                $('#idProcesoProduccionFormula').val(id);
                $('#idProducto').val(fila.idProducto).trigger('change');
                $('#cantidadLote').val(fila.cantidadLote);
                $.each(data.Modelo.detalle || [], function (i, d) {
                    agregarFilaMateriaPrima(d.idProducto, d.descripcionProducto, d.unidadMedida, d.cantidad);
                });
            }
            disabledForm(accion === 'ver');
            $('#TituloModalFormula').html(accion === 'agregar' ? "Agregar fórmula" : (accion === 'ver' ? "Fórmula de producción" : "Actualizar fórmula"));
            $('#mdlFormula').modal({ backdrop: 'static', keyboard: false, show: true });
        },
        error: function () {
            OcultarLoader();
            MuestraToast("error", "Ocurrió un error al cargar la información");
        }
    });
}

function reindexarMateriasPrimas() {
    $('#tblMateriasPrimas tbody tr').each(function (i) {
        $(this).find('.hid-id').attr('name', 'materiasPrimas[' + i + '].idProducto');
        $(this).find('.hid-cantidad').attr('name', 'materiasPrimas[' + i + '].cantidad');
    });
}

function agregarFilaMateriaPrima(idProducto, descripcion, unidad, cantidad) {
    var $tr = $('<tr>').attr('data-id-producto', idProducto);
    $tr.append($('<td>').text(descripcion)
        .append($('<input type="hidden" class="hid-id">').val(idProducto))
        .append($('<input type="hidden" class="hid-cantidad">').val(cantidad)));
    $tr.append($('<td>').text(unidad));
    $tr.append($('<td>').text(cantidad));
    $tr.append($('<td class="text-center">').append(
        $('<button type="button" class="btn btn-icon btn-danger btn-sm btn-quitar-mp" title="Quitar"><i class="fas fa-times"></i></button>')));
    $('#tblMateriasPrimas tbody').append($tr);
    reindexarMateriasPrimas();
}

function onAgregarMateriaPrima() {
    var idProducto = $('#selMateriaPrima').val();
    var texto = $.trim($('#txtCantidadMateriaPrima').val());
    var cantidad = Number(texto);

    if (!idProducto) {
        MuestraToast("error", "Selecciona la materia prima");
        return;
    }
    if (!texto || !isFinite(cantidad) || cantidad <= 0) {
        MuestraToast("error", "La cantidad por lote debe ser mayor a 0");
        return;
    }
    if ($('#tblMateriasPrimas tbody tr[data-id-producto="' + idProducto + '"]').length > 0) {
        MuestraToast("error", "La materia prima ya está en la fórmula");
        return;
    }
    var mp = catalogoMateriasPrimas[idProducto];
    agregarFilaMateriaPrima(mp.idProducto, mp.descripcion, mp.unidadMedida, String(cantidad));
    $('#selMateriaPrima').val('').trigger('change');
    $('#txtCantidadMateriaPrima').val('');
}

/* ---------- Guardar / eliminar ---------- */

function onBeginSubmitFormula() {
    ShowLoader("Guardando...");
}

function onFailureResultFormula() {
    OcultarLoader();
    MuestraToast("error", "Ocurrió un error al guardar la fórmula");
}

function onSuccessResultFormula(data) {
    OcultarLoader();
    if (data.Estatus === 200) {
        MuestraToast("success", data.Mensaje);
        $('#mdlFormula').modal('hide');
        ObtenerFormulas();
    } else {
        MuestraToast("error", data.Mensaje);
    }
}

function EliminarFormula(id) {
    swal({
        title: 'Mensaje',
        text: '¿Estás seguro que deseas eliminar la fórmula de producción? Las producciones ya registradas no se modifican.',
        icon: 'info',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (!willDelete) {
                return;
            }
            $.ajax({
                url: rootUrl("/ProcesoProduccionFormula/DesactivarFormula"),
                data: { idProcesoProduccionFormula: id },
                method: 'post',
                dataType: 'json',
                async: true,
                beforeSend: function () {
                    ShowLoader("Eliminando...");
                },
                success: function (data) {
                    OcultarLoader();
                    MuestraToast(data.Estatus === 200 ? "success" : "error", data.Mensaje);
                    if (data.Estatus === 200) {
                        ObtenerFormulas();
                    }
                },
                error: function () {
                    OcultarLoader();
                    MuestraToast("error", "Ocurrió un error al eliminar la fórmula");
                }
            });
        });
}

/* ---------- Eventos ---------- */

function initEvents() {
    // Unidad automática (solo lectura) según el producto elegido
    $('#idProducto').on('change', function () {
        var p = catalogoProductos[$(this).val()];
        $('#txtUnidadProducto').val(p ? p.unidadMedida : '');
    });
    $('#selMateriaPrima').on('change', function () {
        var p = catalogoMateriasPrimas[$(this).val()];
        $('#txtUnidadMateriaPrima').val(p ? p.unidadMedida : '');
    });

    $('#btnAgregarMateriaPrima').on('click', onAgregarMateriaPrima);

    $('#tblMateriasPrimas').on('click', '.btn-quitar-mp', function () {
        $(this).closest('tr').remove();
        reindexarMateriasPrimas();
    });

    // Enter no debe guardar la fórmula por accidente; en la cantidad de MPL agrega la fila
    $('#frmFormula').on('keydown', 'input', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            if (this.id === 'txtCantidadMateriaPrima') {
                onAgregarMateriaPrima();
            }
        }
    });

    // Se ejecuta antes del manejador de jquery.unobtrusive-ajax (delegado en document)
    $('#frmFormula').on('submit', function (e) {
        var lote = Number($.trim($('#cantidadLote').val()));
        if (!isFinite(lote) || lote <= 0) {
            e.preventDefault();
            e.stopPropagation();
            MuestraToast("error", "La cantidad del lote debe ser mayor a 0");
            return;
        }
        if (lote > LIMITE_LOTE) {
            e.preventDefault();
            e.stopPropagation();
            MuestraToast("error", "La cantidad del lote no puede ser mayor a 10,000.");
            return;
        }
        if ($('#tblMateriasPrimas tbody tr').length === 0) {
            e.preventDefault();
            e.stopPropagation();
            MuestraToast("error", "Agrega al menos una materia prima");
        }
    });

    // Acciones del listado (el HTML se reemplaza en cada consulta)
    $('#ViewFormulas').on('click', '#btnAgregarFormula', function (e) {
        e.preventDefault();
        AbrirFormulario('agregar', 0, null);
    });
    $('#ViewFormulas').on('click', '.btn-ver-formula, .btn-editar-formula', function (e) {
        e.preventDefault();
        var $tr = $(this).closest('tr');
        var fila = { idProducto: $tr.data('id-producto'), cantidadLote: String($tr.attr('data-cantidad-lote')) };
        AbrirFormulario($(this).hasClass('btn-ver-formula') ? 'ver' : 'editar', $(this).data('id'), fila);
    });
    $('#ViewFormulas').on('click', '.btn-eliminar-formula', function (e) {
        e.preventDefault();
        EliminarFormula($(this).data('id'));
    });
}

$(document).ready(function () {
    initEvents();
    ObtenerFormulas();
});
