var table;
var iframe;
var tablaInventario;  


//busqueda
function onBeginSubmitProductos() {
    ShowLoader("Buscando...");
}
function onSuccessResultProductos(data) {  
    tablaInventario.destroy();
    $('#rowProductos').html(data);
    InitDataTableInventario();
    OcultarLoader();
}
function onFailureResultProductos() {
    OcultarLoader();
}

function InitDataTableInventario() {
    var NombreTabla = "tablaRepProductos";
    tablaInventario = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaInventario, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Inventario",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    //doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '20%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Inventario");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4,5,6,7]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4,5,6,7]
                },
            },
        ],

    });

    tablaInventario.buttons(0, null).container().prependTo(
        tablaInventario.table().container()
    );
}

$(document).ready(function () {

    InitDataTableInventario();
    //InitRangePicker('rangeInventario', 'fechaIni', 'fechaFin');
    ////$('#idLineaProductoBusqueda').val('0');
    //$('#fechaIni').val($('#rangeInventario').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeInventario').data('daterangepicker').startDate.format('YYYY-MM-DD'));

});