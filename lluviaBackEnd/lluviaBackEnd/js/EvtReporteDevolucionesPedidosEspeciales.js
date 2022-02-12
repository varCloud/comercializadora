var table;
var iframe;
var tablaDevolucionesPedidosEspeciales;

//busqueda
function onBeginSubmitDevolucionesPedidosEspeciales() {
    ShowLoader('Buscando...');
}

function onSuccessResultDevolucionesPedidosEspeciales(data) {
    OcultarLoader();
    $('#rowDevolucionesPedidosEspeciales').html(data);
    tablaDevolucionesPedidosEspeciales.destroy();    
    InitDataTableDevoluciones();
}
function onFailureResultDevolucionesPedidosEspeciales() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar las Devoluciones de Pedidos Especiales");
}


function InitDataTableDevoluciones() {
    var NombreTabla = "tablaRepDevolucionesPedidosEspeciales";
    tablaDevolucionesPedidosEspeciales = initDataTable(NombreTabla);

    if ($("#tablaRepDevoluciones").length > 0) {
        new $.fn.dataTable.Buttons(tablaDevolucionesPedidosEspeciales, {
            buttons: [
                {
                    extend: 'pdfHtml5',
                    text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a PDF',
                    title: "Reporte Devoluciones de Pedidos Especiales",
                    customize: function (doc) {
                        doc.defaultStyle.fontSize = 8;
                        doc.styles.tableHeader.fontSize = 10;
                        doc.defaultStyle.alignment = 'center';
                        //doc.content[1].table.widths = ['20%', '20%', '10%', '10%', '10%', '10%', '15%', '10%'];
                        doc.pageMargins = [30, 85, 20, 30];
                        doc.content.splice(0, 1);
                        doc['header'] = SetHeaderPDF("Reporte Devoluciones de Pedidos Especiales");
                        doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                    },
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7,8,9,10]
                    },
                },
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7,8,9,10]
                    },
                },
            ],

        });

        tablaDevoltablaDevolucionesPedidosEspecialesuciones.buttons(0, null).container().prependTo(
            tablaDevolucionesPedidosEspeciales.table().container()
        );
    }
}


$(document).ready(function () {

    InitDataTableDevoluciones();
    InitSelect2();
    InitRangePicker('rangeDevoluciones', 'fechaIni', 'fechaFin');
    $('#rangeDevoluciones').val('');

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarDevolucionesPedidosEspeciales").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarDevolucionesPedidosEspeciales .select-multiple").trigger("change");
    });

});