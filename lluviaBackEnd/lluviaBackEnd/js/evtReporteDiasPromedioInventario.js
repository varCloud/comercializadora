var tblDiasPromedioInventario;
$(document).ready(function () {
    InitSelect2();
    InitRangePicker('rangeDiasPromedioInventario', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeDiasPromedioInventario').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeDiasPromedioInventario').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$("#rangeDiasPromedioInventario").val("");
   // $("#frmBuscarDiasPromedioInventario").submit();
});




//busqueda
function onBeginSubmitDiasPromedioInventario() {
    ShowLoader("Buscando...");
}
function onSuccessResultDiasPromedioInventario(data) {
    if (tblDiasPromedioInventario != null)
        tblDiasPromedioInventario.destroy();

    $('#ViewDiasPromedioInventario').html(data);
    if ($("#tblDiasPromedioInventario").length > 0)
        InitDataTableDiasPromedioInventario();
    OcultarLoader();
}
function onFailureResultDiasPromedioInventario() {
    OcultarLoader();
}

function InitDataTableDiasPromedioInventario() {
    var NombreTabla = "tblDiasPromedioInventario";
    tblDiasPromedioInventario = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblDiasPromedioInventario, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Dias Inventario Promedio",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                   // doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Dias Inventario Promedio");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5,6,7,8]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5,6,7,8]
                },
            },
        ],

    });

    tblDiasPromedioInventario.buttons(0, null).container().prependTo(
        tblDiasPromedioInventario.table().container()
    );
}
