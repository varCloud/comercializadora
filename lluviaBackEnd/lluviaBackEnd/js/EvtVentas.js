﻿var table;
var iframe;
var tablaVentas;  

//busqueda
function onBeginSubmitVentas() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitVentas() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultVentas(data) {
    console.log("onSuccessResultVentas", JSON.stringify(data) );    
    tablaVentas.destroy();
    $('#rowVentas').html(data);
    InitDataTableVentas();
}
function onFailureResultVentas() {
    console.log("onFailureResult___");
}


function PintarTabla() {
    $.ajax({
        url: "/Reportes/BuscarVentas",
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaVentas.destroy();
            $('#rowVentas').html(data);
            InitDataTableVentas();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}


function InitDataTableVentas() {
    var NombreTabla = "tablaRepVentas";
    tablaVentas = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaVentas, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Ventas",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '20%', '20%', '10%', '20%', '20%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Ventas");
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

    tablaVentas.buttons(0, null).container().prependTo(
        tablaVentas.table().container()
    );
}

function InitRangePicker() {

    $('#rangeVentas').daterangepicker({
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear'
        }
    }, function (start, end, label) {
        $('#fechaIniVentas').val(start.format('YYYY-MM-DD'));
        $('#fechaFinVentas').val(end.format('YYYY-MM-DD'));
    });

    $('#rangeVentas').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
    });

    $('#rangeVentas').on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
    });

}

$(document).ready(function () {

    InitDataTableVentas();
    InitRangePicker();
    $('#idLineaProductoBusqueda').val('');
    $('#idClienteBusqueda').val('');
    $('#idUsuarioBusqueda').val('');

});