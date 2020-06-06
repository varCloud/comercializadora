var table;
var iframe;
var tablaCompras;  


//busqueda
function onBeginSubmitCompras() {
    ShowLoader("Buscando...");
}
function onCompleteSubmitCompras() {
    OcultarLoader();
}
function onSuccessResultCompras(data) {
    OcultarLoader();
    tablaCompras.destroy();
    $('#rowCompras').html(data);
    InitDataTableCompras();
}
function onFailureResultCompras() {
    OcultarLoader();
    MuestraToast("error","Ocurrio un error al consultar las compras");
}


function InitDataTableCompras() {
    var NombreTabla = "tablaRepCompras";
    tablaCompras = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaCompras, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Compras",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['5%','20%', '20%', '20%', '10%', '10%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Compras");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5,6]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5,6]
                },
            },
        ],

    });

    tablaCompras.buttons(0, null).container().prependTo(
        tablaCompras.table().container()
    );
}


$(document).ready(function () {

    InitDataTableCompras();
    InitRangePicker('rangeCompras', 'fechaIni', 'fechaFin');
    //$('#idLineaProductoBusqueda').val('0');
    //$('#lstProveedoresBusqueda').val('0');
    //$('#lstUsuariosBusqueda').val('0');
    $('#fechaIni').val($('#rangeCompras').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeCompras').data('daterangepicker').startDate.format('YYYY-MM-DD'));


});