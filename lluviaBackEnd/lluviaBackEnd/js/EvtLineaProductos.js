
var iframe;
var tablaLineaProductos;

function onBeginSubmitGuardarLineaProducto() {
    console.log("onBeginSubmitGuardarLineaProducto");
}

function onCompleteSubmitGuardarLineaProducto() {
    console.log("onCompleteSubmitGuardarLineaProducto");
}

function onSuccessResultGuardarLineaProducto(data) {
    console.log("onSuccessResultGuardarLineaProducto");

    if (data.status == 200) {

        MuestraToast("success", data.Mensaje);
        PintarTabla();

    } else {
        MuestraToast("error", data.Mensaje);
    }

    $('#EditarLineaProductoModal').modal('hide');

}

function onFailureResultGuardarLineaProducto() {
    console.log("onFailureResultGuardarLineaProducto");
}

function PintarTabla() {
    $.ajax({
        url: rootUrl("/LineaProducto/_ObtenerLineaProducto"),
        data: { idLineaProducto: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaLineaProductos.destroy();
            $('#rowTblLineaProducto').html(data);
            InitTableLineaProductos();

        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitTableLineaProductos() {
    var NombreTabla = "tablaLineaProductos";
    tablaLineaProductos = initDataTable(NombreTabla)
    new $.fn.dataTable.Buttons(tablaLineaProductos, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Linea de Productos",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8; 
                    doc.styles.tableHeader.fontSize = 10; 
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['50%', '50%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Linea de Productos");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1]
                },
            },
        ],
    });

    tablaLineaProductos.buttons(0, null).container().prependTo(
        tablaLineaProductos.table().container()
    );

    $('#' + NombreTabla+'_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarLineaProducto" data-toggle="tooltip" title="Agregar LineaProducto"><i class="fas fa-plus"></i></a>');
    InitBtnAgregar();
}

function ObtenerLineaProducto(idLineaProducto) {

    var result = '';
    $.ajax({
        url: rootUrl("/LineaProducto/ObtenerLineaProducto"),
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

function VerLineaProducto(idLineaProducto) {

    $('#btnGuardarLineaProducto').prop('disabled', true);

    var data = ObtenerLineaProducto(idLineaProducto);

    $('#idLineaProducto').val(idLineaProducto);
    $('#activo').val(data.activo);
    $('#descripcion').val(data.descripcion).prop('disabled', true);
    $('.field-validation-error').html("");

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
    $('.field-validation-error').html("");

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
        $('.field-validation-error').html("");

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
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/LineaProducto/ActualizarEstatusLineaProducto"),
                    data: { idLineaProducto: idLineaProducto, activo: false },
                    method: 'post',
                    dataType: 'json',
                    async: false,
                    beforeSend: function (xhr) {
                        console.log("Antes ")
                    },
                    success: function (data) {
                        MuestraToast("success", data.Mensaje);
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
    InitTableLineaProductos();
});