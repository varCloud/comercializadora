var table;
var iframe;

//busqueda
function onBeginSubmitVentas() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitVentas() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultVentas(data) {
    console.log("onSuccessResultVentas", JSON.stringify(data) );    
    table.destroy();
    $('#rowVentas').html(data);
    InitDataTable();
}
function onFailureResultVentas() {
    console.log("onFailureResult___");
}



//// guardar-modificar
//function onBeginSubmitGuardarProducto() {
//    console.log("onBeginSubmit___");
//}
//function onCompleteSubmitGuardarProducto() {
//    console.log("onCompleteSubmit___");
//}
//function onSuccessResultGuardarProducto(data) {
//    console.log("onSuccessResult", JSON.stringify(data));   
//    if (data.Estatus == 200) {
//        MuestraToast('success', data.Mensaje);
//        PintarTabla();
//    } else {
//        //MuestraToast("error", data.Mensaje);
//    }
//    $('#EditarProductoModal').modal('hide');
//}
//function onFailureResultGuardarProducto() {
//    console.log("onFailureResult___");
//}


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
            table.destroy();
            $('#rowVentas').html(data);
            InitDataTable();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

//function BuscarProductos(data) {
//    $.ajax({
//        url: "/Productos/BuscarProductos",
//        data: { idUsuario: data.idProducto },
//        method: 'post',
//        dataType: 'html',
//        async: false,
//        beforeSend: function (xhr) {
//        },
//        success: function (data) {
//            table.destroy();
//            $('#rowProductos').html(data);
//            InitDataTable();
//        },
//        error: function (xhr, status) {
//            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
//            console.log(xhr);
//            console.log(status);
//        }
//    });
//}



function InitDataTable() {

    table = $('#tablaRepVentas').DataTable({
        "language": {
            "lengthMenu": "Muestra _MENU_ registros por pagina",
            "zeroRecords": "No existen registros",
            "info": "Pagina _PAGE_ de _PAGES_",
            "infoEmpty": "No existe informacion para mostrar",
            "infoFiltered": "(filtered from _MAX_ total records)",
            "search": "Buscar:",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            },
        },
        "dom": 'Bfrtip',
        buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Ventas de Productos",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8; //2, 3, 4,etc
                    doc.styles.tableHeader.fontSize = 10; //2, 3, 4, etc
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '20%', '10%'];
                },
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
            },


        ],

        "bDestroy": true, // es necesario para poder ejecutar la funcion LimpiaTabla()
    });


}


//function ObtenerProducto(idProducto) {

//    var result = '';
//    $.ajax({
//        url: "/Productos/ObtenerProductos",
//        data: { idProducto: idProducto },
//        method: 'post',
//        dataType: 'json',
//        async: false,
//        beforeSend: function (xhr) {
//            console.log("Antes")
//        },
//        success: function (data) {

//            result = data;
//        },
//        error: function (xhr, status) {
//            console.log('hubo un problema pongase en contacto con el administrador del sistema');
//            console.log(xhr);
//            console.log(status);
//        }
//    });

//    return result;
//}

//function VerProducto(idProducto) {

//    $('#btnGuardarProducto').prop('disabled', true);

//    var data = ObtenerProducto(idProducto);

//    $('#idProducto').val(idProducto);
//    $('#activo').val(data.activo);
//    $('#descripcion').val(data.descripcion).prop('disabled', true);
//    $('#idUnidadMedida').val(data.idUnidadMedida).prop('disabled', true);
//    $('#idLineaProducto').val(data.idLineaProducto).prop('disabled', true);
//    $('#cantidadUnidadMedida').val(data.cantidadUnidadMedida).prop('disabled', true);
//    $('#articulo').val(data.articulo).prop('disabled', true);
//    //para abrir el modal
//    $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
//    $('#TituloModalProducto').html("Información del Producto");
//}

//function EditarProducto(idProducto) {

//    $('#btnGuardarProducto').prop('disabled', false);

//    var data = ObtenerProducto(idProducto);

//    $('#idProducto').val(idProducto);
//    $('#activo').val(data.activo);
//    $('#descripcion').val(data.descripcion).prop('disabled', false);
//    $('#idUnidadMedida').val(data.idUnidadMedida).prop('disabled', false);
//    $('#idLineaProducto').val(data.idLineaProducto).prop('disabled', false);
//    $('#cantidadUnidadMedida').val(data.cantidadUnidadMedida).prop('disabled', false);
//    $('#articulo').val(data.articulo).prop('disabled', false);
//    //para abrir el modal
//    $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
//    $('#TituloModalUsuario').html("Editar Producto");
//}


//function InitBtnAgregar() {
//    $('#btnAgregarProducto').click(function (e) {

//        $('#btnGuardarProducto').prop('disabled', false);

//        $('#idProducto').val(0);
//        $('#descripcion').val('').prop('disabled', false);
//        $('#idUnidadMedida').val('').prop('disabled', false);
//        $('#idLineaProducto').val('').prop('disabled', false);
//        $('#cantidadUnidadMedida').val('').prop('disabled', false);
//        $('#articulo').val('').prop('disabled', false);
//        //para abrir el modal
//        $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
//        $('#TituloModalProducto').html("Agregar Producto");
//    });
//}

//function EliminarProducto(idProducto) {

//    swal({
//        title: 'Mensaje',
//        text: 'Estas seguro que deseas eliminar a este Producto?',
//        icon: 'warning',
//        buttons: true,
//        dangerMode: true,
//    })
//        .then((willDelete) => {
//            if (willDelete) {
//                $.ajax({
//                    url: "/Productos/ActualizarEstatusProducto",
//                    data: { idProducto: idProducto, activo: false },
//                    method: 'post',
//                    dataType: 'json',
//                    async: false,
//                    beforeSend: function (xhr) {
//                        console.log("Antes ")
//                    },
//                    success: function (data) {
//                        MuestraToast('success', data.Mensaje);
//                        PintarTabla();
//                    },
//                    error: function (xhr, status) {
//                        console.log('Hubo un problema al intentar eliminar al usuario, contactese con el administrador del sistema');
//                        console.log(xhr);
//                        console.log(status);
//                    }
//                });

//            } else {
//                console.log("cancelar");
//            }
//        });
//}

function InitRangePicker() {

    $('.daterange-cus').daterangepicker({
        locale: { format: 'YYYY-MM-DD' },
        drops: 'down',
        opens: 'right'
    });

}

$(document).ready(function () {

    InitDataTable();
    InitRangePicker();
    $('#idLineaProductoBusqueda').val('');
    $('#idClienteBusqueda').val('');
    $('#idUsuarioBusqueda').val('');

});