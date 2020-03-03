var table;
var iframe;


function onBeginSubmitGuardarLineaProducto() {
    console.log("onBeginSubmitGuardarLineaProducto");
}
function onCompleteSubmitGuardarLineaProducto() {
    console.log("onCompleteSubmitGuardarLineaProducto");
}
function onSuccessResultGuardarLineaProducto(data) {
    console.log("onSuccessResultGuardarLineaProducto");

    if (data.status == 200) {

        mensajeOK(data.mensaje);
        PintarTabla();

    } else {
        mensajeERROR(data.mensaje);
    }

    $('#EditarLineaProductoModal').modal('hide');

}
function onFailureResultGuardarLineaProducto() {
    console.log("onFailureResultGuardarLineaProducto");
}

function PintarTabla() {
    $.ajax({
        url: "/LineaProducto/_ObtenerLineaProducto",
        data: { idLineaProducto: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            table.destroy();
            $('#rowTblLineaProducto').html(data);
            InitDataTable();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}


function InitDataTable() {

    table = $('#tablaLineaProductos').DataTable({
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
                title: "LineaProductoes",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8; //2, 3, 4,etc
                    doc.styles.tableHeader.fontSize = 10; //2, 3, 4, etc
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '20%', '10%'];
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

        "bDestroy": true, // es necesario para poder ejecutar la funcion LimpiaTabla()
    });

    $('#tablaLineaProductos_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarLineaProducto" data-toggle="tooltip" title="Agregar LineaProducto"><i class="fas fa-user-plus"></i></a>');
    InitBtnAgregar();
}

function ObtenerLineaProducto(idLineaProducto) {

    var result = '';
    $.ajax({
        url: "/LineaProducto/ObtenerLineaProducto",
        data: { idLineaProducto: idLineaProducto },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes")
        },
        success: function (data) {

            result = data;
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

    return result;
}

function mensajeOK(mensaje) {
    swal(mensaje, 'Presione OK para continuar.', 'success');
}

function mensajeERROR(mensaje) {
    swal(mensaje, 'Presione OK para continuar.', 'error');
}

function VerLineaProducto(idLineaProducto) {

    $('#btnGuardarLineaProducto').prop('disabled', true);

    var data = ObtenerLineaProducto(idLineaProducto);

    $('#idLineaProducto').val(idLineaProducto);
    $('#activo').val(data.activo);
    $('#descripcion').val(data.descripcion).prop('disabled', true);

    //para abrir el modal
    $('#EditarLineaProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalLineaProducto').html("Información del Linea de Producto");

}

function EditarLineaProducto(idLineaProducto) {

    $('#btnGuardarLineaProducto').prop('disabled', false);

    var data = ObtenerLineaProducto(idLineaProducto);
    
    $('#idLineaProducto').val(idLineaProducto);
    $('#activo').val(data.activo);
    $('#descripcion').val(data.descripcion).prop('disabled', false);

    //para abrir el modal
    $('#EditarLineaProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalLineaProducto').html("Editar Linea de Producto");

}


function InitBtnAgregar() {
    $('#btnAgregarLineaProducto').click(function (e) {

        $('#btnGuardarLineaProducto').prop('disabled', false);

        $('#idLineaProducto').val(0);
        $('#activo').val(1);
        $('#descripcion').val('').prop('disabled', false);

        //para abrir el modal
        $('#EditarLineaProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalLineaProducto').html("Agregar Linea de Producto");

    });
}

function EliminarLineaProducto(idLineaProducto) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a esta Linea de Producto?',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: "/LineaProducto/ActualizarEstatusLineaProducto",
                    data: { idLineaProducto: idLineaProducto, activo: false },
                    method: 'post',
                    dataType: 'json',
                    async: false,
                    beforeSend: function (xhr) {
                        console.log("Antes ")
                    },
                    success: function (data) {
                        mensajeOK(data.mensaje);
                        PintarTabla();
                    },
                    error: function (xhr, status) {
                        console.log('Hubo un problema al intentar eliminar la Linea de Producto, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });

            } else {
                console.log("cancelar");
            }
        });
}



$(document).ready(function () {

    InitDataTable();

});