var table;
var iframe;
var tablaInventario;  


//busqueda
function onBeginSubmitProductos() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitProductos() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultProductos(data) {
    console.log("onSuccessResult", JSON.stringify(data) );    
    tablaInventario.destroy();
    $('#rowProductos').html(data);
    InitDataTableInventario();
}
function onFailureResultProductos() {
    console.log("onFailureResult___");
}

function PintarTabla() {
    $.ajax({
        url: "/Reportes/BuscarInventario",
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaInventario.destroy();
            $('#rowProductos').html(data);
            InitDataTableInventario();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

//function InitDataTable() {

//    table = $('#tablaRepProductos').DataTable({
//        "language": {
//            "lengthMenu": "Muestra _MENU_ registros por pagina",
//            "zeroRecords": "No existen registros",
//            "info": "Pagina _PAGE_ de _PAGES_",
//            "infoEmpty": "No existe informacion para mostrar",
//            "infoFiltered": "(filtered from _MAX_ total records)",
//            "search": "Buscar:",
//            "paginate": {
//                "first": "Primero",
//                "last": "Ultimo",
//                "next": "Siguiente",
//                "previous": "Anterior"
//            },
//        },
//        "dom": 'Bfrtip',
//        buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
//        buttons: [
//            {
//                extend: 'pdfHtml5',
//                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
//                className: '',
//                titleAttr: 'Exportar a PDF',
//                title: "Inventario de Productos",
//                customize: function (doc) {
//                    doc.defaultStyle.fontSize = 8; //2, 3, 4,etc
//                    doc.styles.tableHeader.fontSize = 10; //2, 3, 4, etc
//                    doc.defaultStyle.alignment = 'center';
//                    doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '20%', '10%'];
//                },
//                exportOptions: {
//                    columns: [0, 1, 2, 3]
//                },
//            },
//            {
//                extend: 'excel',
//                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
//                className: '',
//                titleAttr: 'Exportar a Excel',
//                exportOptions: {
//                    columns: [0, 1, 2, 3]
//                },
//            },


//        ],

//        "bDestroy": true, // es necesario para poder ejecutar la funcion LimpiaTabla()
//    });


//}


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
                    doc.content[1].table.widths = ['10%', '23%', '23%', '22%', '22%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Inventario");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
        ],

    });

    tablaInventario.buttons(0, null).container().prependTo(
        tablaInventario.table().container()
    );
}

function InitRangePicker() {

    $('#rangeInventario').daterangepicker({
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear'
        }     
    }, function (start, end, label) {
        console.log("A new date selection was made: " + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD'));
        $('#fechaIniInventario').val(start.format('YYYY-MM-DD'));
        $('#fechaFinInventario').val(end.format('YYYY-MM-DD'));

    });

    $('#rangeInventario').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
    });

    $('#rangeInventario').on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
    });

}

$(document).ready(function () {

    InitDataTableInventario();
    InitRangePicker();
    $('#idLineaProductoBusqueda').val('');

});