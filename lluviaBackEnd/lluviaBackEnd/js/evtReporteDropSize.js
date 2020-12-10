var tblDropSize;
$(document).ready(function () {
    InitSelect2();
    InitRangePicker('rangeDropSize', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeDropSize').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeDropSize').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$("#rangeDiasPromedioInventario").val("");
    // $("#frmBuscarDiasPromedioInventario").submit();
});




//busqueda
function onBeginSubmitDropSize() {
    ShowLoader("Buscando...");
}
function onSuccessResultDropSize(data) {
    if (tblDropSize != null)
        tblDropSize.destroy();

    $('#ViewDropSize').html(data);
    if ($("#tblDropSize").length > 0)
        InitDataTabletblDropSize();
    OcultarLoader();
}
function onFailureResultDropSize() {
    OcultarLoader();
}

function InitDataTabletblDropSize() {
    var NombreTabla = "tblDropSize";
    tblDropSize = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblDropSize, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "DropSize",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    // doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("DropSize");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            },
        ],

    });

    tblDropSize.buttons(0, null).container().prependTo(
        tblDropSize.table().container()
    );
}
