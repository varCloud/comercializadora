﻿var table;
var iframe;
var tablaDevoluciones;

//busqueda
function onBeginSubmitDevoluciones() {
    ShowLoader('Buscando...');
}

function onSuccessResultDevoluciones(data) {
    OcultarLoader();
    $('#rowDevoluciones').html(data);
    tablaDevoluciones.destroy();    
    InitDataTableDevoluciones();
}
function onFailureResultDevoluciones() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar las Devoluciones");
}


function InitDataTableDevoluciones() {
    var NombreTabla = "tablaRepDevoluciones";
    tablaDevoluciones = initDataTable(NombreTabla);

    if ($("#tablaRepDevoluciones").length > 0) {
        new $.fn.dataTable.Buttons(tablaDevoluciones, {
            buttons: [
                {
                    extend: 'pdfHtml5',
                    text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a PDF',
                    title: "Reporte Devoluciones y Complementos",
                    customize: function (doc) {
                        doc.defaultStyle.fontSize = 8;
                        doc.styles.tableHeader.fontSize = 10;
                        doc.defaultStyle.alignment = 'center';
                        //doc.content[1].table.widths = ['20%', '20%', '10%', '10%', '10%', '10%', '15%', '10%'];
                        doc.pageMargins = [30, 85, 20, 30];
                        doc.content.splice(0, 1);
                        doc['header'] = SetHeaderPDF("Reporte Devoluciones y Complementos");
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

        tablaDevoluciones.buttons(0, null).container().prependTo(
            tablaDevoluciones.table().container()
        );
    }
}


$(document).ready(function () {

    InitDataTableDevoluciones();
    InitSelect2();
    InitRangePicker('rangeDevoluciones', 'fechaIni', 'fechaFin');
    $('#rangeDevoluciones').val('');

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarDevoluciones").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarDevoluciones .select-multiple").trigger("change");

    });

});