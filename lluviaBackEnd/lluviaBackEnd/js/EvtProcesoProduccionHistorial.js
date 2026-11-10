var tblProduccionHistorial;
var tituloReporteProduccionHistorial;

$(document).ready(function () {
    $('.select-multiple').select2({
        width: "100%",
        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        },
    });

    InitRangePicker('rangeProduccionHistorial', 'fechaIni', 'fechaFin');

    // Por defecto los últimos 30 días (igual que el rango por defecto del SP)
    var picker = $('#rangeProduccionHistorial').data('daterangepicker');
    picker.setStartDate(moment().subtract(29, 'days'));
    picker.setEndDate(moment());
    $('#rangeProduccionHistorial').val(picker.startDate.format('YYYY-MM-DD') + ' - ' + picker.endDate.format('YYYY-MM-DD'));
    $('#fechaIni').val(picker.startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val(picker.endDate.format('YYYY-MM-DD'));

    // Detalle (materias primas consumidas) de una producción: el HTML ya viene en el parcial
    $('#ViewProduccionHistorial').on('click', '.btn-ver-detalle-produccion', function (e) {
        e.preventDefault();
        var $detalle = $('#detalleProduccion_' + $(this).data('id'));
        if ($detalle.length === 0) {
            return;
        }
        $('#TituloModalDetalleProduccion').text($detalle.attr('data-titulo'));
        $('#cuerpoDetalleProduccion').html($detalle.html());
        $('#mdlDetalleProduccion').modal('show');
    });

    $('#frmBuscarProduccionHistorial').on('submit', function (e) {
        var ini = $('#fechaIni').val();
        var fin = $('#fechaFin').val();
        if (ini && fin && fin < ini) {
            e.preventDefault();
            e.stopPropagation();
            MuestraToast("error", "La fecha final no puede ser menor a la fecha inicial");
        }
    });

    // Consulta inicial con el rango por defecto
    $('#frmBuscarProduccionHistorial').submit();
});

//busqueda
function onBeginSubmitProduccionHistorial() {
    ShowLoader("Buscando...");
}

function onSuccessResultProduccionHistorial(data) {
    OcultarLoader();
    // Una excepción del servidor llega como JSON (Notificacion con Estatus distinto de 200)
    if (typeof data === 'object' && data !== null) {
        MuestraToast("error", data.Mensaje || "Ocurrió un error al consultar las producciones");
        return;
    }

    if (tblProduccionHistorial != null) {
        tblProduccionHistorial.destroy();
        tblProduccionHistorial = null;
    }

    tituloReporteProduccionHistorial = "Producción " + $("#fechaIni").val() + " - " + $("#fechaFin").val();
    $('#ViewProduccionHistorial').html(data);
    if ($("#tblProduccionHistorial").length > 0)
        InitDataTableProduccionHistorial();
}

function onFailureResultProduccionHistorial(xhr) {
    OcultarLoader();
    MuestraToast("error", xhr && xhr.status === 403
        ? "No tiene permiso para consultar la producción"
        : "Ocurrió un error al consultar las producciones");
}

function InitDataTableProduccionHistorial() {
    var NombreTabla = "tblProduccionHistorial";
    tblProduccionHistorial = initDataTable(NombreTabla);

    // La columna Detalle (9) es solo un botón: no se exporta
    new $.fn.dataTable.Buttons(tblProduccionHistorial, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: tituloReporteProduccionHistorial,
                filename: 'Produccion',
                orientation: 'landscape',
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF(tituloReporteProduccionHistorial);
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
                },
            },
            {
                extend: 'excel',
                messageTop: tituloReporteProduccionHistorial,
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                filename: 'Produccion',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
                },
            },
        ],
    });

    tblProduccionHistorial.buttons(0, null).container().prependTo(
        tblProduccionHistorial.table().container()
    );
}
