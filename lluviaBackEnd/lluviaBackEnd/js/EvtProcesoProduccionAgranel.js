var table;
var iframe;
var tblCostoProduccion;

function onBeginSubmitCostoProduccion() {
    ShowLoader("Buscando...");
}

function onSuccessResultCostoProduccion(data) {
    if (tblCostoProduccion != null)
        tblCostoProduccion.destroy();

    tituloReporte = "Listado Productos Produccion" + " " + $("#idMes option:selected").text() + " " + $("#idAnio option:selected").text();
    $('#ViewCostoProduccion').html(data);
    if ($("#tblCostoProduccion").length > 0)
        InitDataTableCostoProduccion();
    OcultarLoader();
}

function onFailureResultCostoProduccion() {
    OcultarLoader();
}

function InitDataTableCostoProduccion() {
    var NombreTabla = "tblCostoProduccion";
    tblCostoProduccion = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblCostoProduccion, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: tituloReporte,
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    // doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF(tituloReporte);
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7,8]
                },
            },
            {
                extend: 'excel',
                messageTop: tituloReporte,
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7,8]
                },
            },
        ],

    });

    tblCostoProduccion.buttons(0, null).container().prependTo(
        tblCostoProduccion.table().container()
    );
}



$(document).ready(function () {
    InitSelect2();
    InitRangePicker('rangeProduccionAgranel', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeProduccionAgranel').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeProduccionAgranel').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarCargaMercanciaLiquidos").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarCargaMercanciaLiquidos .select-multiple").trigger("change");
    });
    $("#btnBuscar").trigger('click');  
});


