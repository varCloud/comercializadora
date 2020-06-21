var tblMargenBruto;
$(document).ready(function () {

    InitRangePicker('rangeMargenBruto', 'fechaIni', 'fechaFin');
    //$('#fechaIni').val($('#rangeMargenBruto').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeMargenBruto').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $("#rangeMargenBruto").val("");
    //$("#idTipoMargenBruto").val(1);


    $("#frmBuscarMargenBruto").submit();


});




//busqueda
function onBeginSubmitMargenBruto() {
    ShowLoader("Buscando...");
}
function onSuccessResultMargenBruto(data) {
    if (tblMargenBruto != null)
        tblMargenBruto.destroy();

    $('#ViewMargenBruto').html(data);
    if ($("#tblMargenBruto").length > 0)
        InitDataTableMargenBruto();
    OcultarLoader();
}
function onFailureResultMargenBruto() {
    OcultarLoader();
}

function InitDataTableMargenBruto() {
    var NombreTabla = "tblMargenBruto";
    tblMargenBruto = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblMargenBruto, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Margen Bruto",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Margen Bruto");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
        ],

    });

    tblMargenBruto.buttons(0, null).container().prependTo(
        tblMargenBruto.table().container()
    );
}
