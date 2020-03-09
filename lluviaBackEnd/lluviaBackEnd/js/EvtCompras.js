var table;
var iframe;
var tablaCompras;  


//busqueda
function onBeginSubmitCompras() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitCompras() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultCompras(data) {
    console.log("onSuccessResult", JSON.stringify(data) );    
    tablaCompras.destroy();
    $('#rowCompras').html(data);
    InitDataTableCompras();
}
function onFailureResultCompras() {
    console.log("onFailureResult___");
}

function PintarTabla() {
    $.ajax({
        url: "/Reportes/BuscarCompras",
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaCompras.destroy();
            $('#rowCompras').html(data);
            InitDataTableCompras();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
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
                    doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '20%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Compras");
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

    tablaCompras.buttons(0, null).container().prependTo(
        tablaCompras.table().container()
    );
}


//function InitRangePicker() {

//    $('#rangeCompras').daterangepicker({
//        autoUpdateInput: false,
//        locale: {
//            cancelLabel: 'Clear'
//        }
//    }, function (start, end, label) {
//        $('#fechaIniCompras').val(start.format('YYYY-MM-DD'));
//        $('#fechaFinCompras').val(end.format('YYYY-MM-DD'));
//    });

//    $('#rangeCompras').on('apply.daterangepicker', function (ev, picker) {
//        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
//    });

//    $('#rangeCompras').on('cancel.daterangepicker', function (ev, picker) {
//        $(this).val('');
//    });
    
//}

$(document).ready(function () {

    InitDataTableCompras();
    InitRangePicker('rangeCompras', 'fechaIni', 'fechaFin');
    $('#idLineaProductoBusqueda').val('0');
    $('#lstProveedoresBusqueda').val('0');
    $('#lstUsuariosBusqueda').val('0');
    $('#fechaIni').val($('#rangeCompras').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeCompras').data('daterangepicker').startDate.format('YYYY-MM-DD'));


});