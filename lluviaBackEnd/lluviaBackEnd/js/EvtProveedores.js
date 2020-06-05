var tablaProveedores;
var iframe;


function onBeginSubmitGuardarProveedor() {
    console.log("onBeginSubmitGuardarProveedor");
}
function onCompleteSubmitGuardarProveedor() {
    console.log("onCompleteSubmitGuardarProveedor");
}
function onSuccessResultGuardarProveedor(data) {
    console.log("onSuccessResultGuardarProveedor");
    
    if (data.Estatus == 200) {

        MuestraToast("success", data.Mensaje);
        PintarTabla();

    } else {
        MuestraToast("success", data.Mensaje);
    }

    $('#EditarProveedorModal').modal('hide');

}
function onFailureResultGuardarProveedor() {
    console.log("onFailureResultGuardarProveedor");
}

function PintarTabla() {
    $.ajax({
        url: rootUrl("/Proveedores/_ObtenerProveedores"),
        data: { idProveedor: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaProveedores.destroy();
            $('#rowTblProveedores').html(data);
            InitTableProveedores();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitTableProveedores() {
    var NombreTabla = "tablaProveedores";
    tablaProveedores = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaProveedores, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Proveedores",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '25%', '20%', '20%', '25%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Proveedores");
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

    tablaProveedores.buttons(0, null).container().prependTo(
        tablaProveedores.table().container()
    );

  
    $('#' + NombreTabla+'_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarProveedor" data-toggle="tooltip" title="Agregar Proveedor"><i class="fas fa-plus"></i></a>');
    InitBtnAgregar();
}



function ObtenerProveedor(idProveedor) {

    var result = '';
    $.ajax({
        url: rootUrl("/Proveedores/ObtenerProveedor"),
        data: { idProveedor: idProveedor },
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

function VerProveedor(idProveedor) {

    $('#btnGuardarProveedor').prop('disabled', true);

    var data = ObtenerProveedor(idProveedor);

    $('#idProveedor').val(idProveedor);
    $('#activo').val(data.activo);

    $('#nombre').val(data.nombre).prop('disabled', true);
    $('#descripcion').val(data.descripcion).prop('disabled', true);
    $('#telefono').val(data.telefono).prop('disabled', true);
    $('#direccion').val(data.direccion).change().prop('disabled', true);
    $('.field-validation-error').html("");

    //para abrir el modal
    $('#EditarProveedorModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalProveedor').html("Información del Proveedor");

}

function EditarProveedor(idProveedor) {

    $('#btnGuardarProveedor').prop('disabled', false);

    var data = ObtenerProveedor(idProveedor);
    
    $('#idProveedor').val(idProveedor);
    $('#activo').val(data.activo);

    $('#nombre').val(data.nombre).prop('disabled', false);
    $('#descripcion').val(data.descripcion).prop('disabled', false);
    $('#telefono').val(data.telefono).prop('disabled', false);
    $('#direccion').val(data.direccion).change().prop('disabled', false);
    $('.field-validation-error').html("");

    //para abrir el modal
    $('#EditarProveedorModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalProveedor').html("Editar Proveedor");

}


function InitBtnAgregar() {
    $('#btnAgregarProveedor').click(function (e) {

        $('#btnGuardarProveedor').prop('disabled', false);

        $('#idProveedor').val(0);
        $('#activo').val(0);

        $('#nombre').val('').prop('disabled', false);
        $('#descripcion').val('').prop('disabled', false);
        $('#telefono').val('').prop('disabled', false);
        $('#direccion').val('').change().prop('disabled', false);
        $('.field-validation-error').html("");

        //para abrir el modal
        $('#EditarProveedorModal').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalProveedor').html("Agregar Proveedor");

    });
}

function EliminarProveedor(idProveedor) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a este Proveedor?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Proveedores/ActualizarEstatusProveedor"),
                    data: { idProveedor: idProveedor, activo: false },
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
                        console.log('Hubo un problema al intentar eliminar al Proveedor, contactese con el administrador del sistema');
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
    InitTableProveedores();
});