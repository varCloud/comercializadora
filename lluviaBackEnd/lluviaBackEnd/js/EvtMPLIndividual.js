var tblMPLIndividual;
var tituloReporteMPLIndividual;

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

    InitRangePicker('rangeMPLIndividual', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeMPLIndividual').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeMPLIndividual').data('daterangepicker').endDate.format('YYYY-MM-DD'));
});

//busqueda
function onBeginSubmitMPLIndividual() {
    ShowLoader("Buscando...");
}

function onSuccessResultMPLIndividual(data) {
    if (tblMPLIndividual != null)
        tblMPLIndividual.destroy();

    tituloReporteMPLIndividual = "MPL Individual" + " " + $("#fechaIni").val() + " - " + $("#fechaFin").val();
    $('#ViewMPLIndividual').html(data);
    if ($("#tblMPLIndividual").length > 0)
        InitDataTableMPLIndividual();
    OcultarLoader();
}

function onFailureResultMPLIndividual() {
    OcultarLoader();
}

function InitDataTableMPLIndividual() {
    var NombreTabla = "tblMPLIndividual";
    tblMPLIndividual = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblMPLIndividual, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: tituloReporteMPLIndividual,
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF(tituloReporteMPLIndividual);
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                },
            },
            {
                extend: 'excel',
                messageTop: tituloReporteMPLIndividual,
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                },
            },
        ],

    });

    tblMPLIndividual.buttons(0, null).container().prependTo(
        tblMPLIndividual.table().container()
    );
}
