var tablaUsuarios;  
var iframe;


function onBeginSubmitGuardarUsuario() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitGuardarUsuario() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultGuardarUsuario(data) {
    console.log("onSuccessResult___");

    if (data.status == 200) {

        MuestraToast("success", data.Mensaje)
        PintarTabla();

    } else {
        MuestraToast("error", data.Mensaje)
    }

    $('#EditarUsuarioModal').modal('hide');

}
function onFailureResultGuardarUsuario() {
    console.log("onFailureResult___");
}

function PintarTabla() {
    $.ajax({
        url: rootUrl("/Usuarios/_ObtenerUsuarios"),
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaUsuarios.destroy();
            $('#rowTblUsuario').html(data);
            InitTableUsuarios();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitTableUsuarios() {
    var NombreTabla = "tablaUsuarios";
    tablaUsuarios = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaUsuarios, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Usuarios",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '30%', '20%', '20%', '20%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Usuarios");
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

    tablaUsuarios.buttons(0, null).container().prependTo(
        tablaUsuarios.table().container()
    );


    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarUsuario" data-toggle="tooltip" title="Agregar Usuario"><i class="fas fa-user-plus"></i></a>');
    InitBtnAgregar();
}

function ObtenerUsuario(idUsuario) {

    var result = '';
    $.ajax({
        url: rootUrl("/Usuarios/ObtenerUsuario"),
        data: { idUsuario: idUsuario },
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


function VerUsuario(idUsuario) {

    $('#btnGuardarUsuario').prop('disabled', true);

    var data = ObtenerUsuario(idUsuario);

    $('#idUsuario').val(idUsuario);
    $('#idRol').val(data.idRol);
    $('#idAlmacen').val(data.idAlmacen);
    $('#idSucursal').val(data.idSucursal);
    $('#activo').val(data.activo);

    $('#usuario').val(data.usuario).prop('disabled', true);
    $('#contrasena').val(data.contrasena).prop('disabled', true);
    $('#nombre').val(data.nombre).prop('disabled', true);
    $('#apellidoPaterno').val(data.apellidoPaterno).prop('disabled', true);
    $('#apellidoMaterno').val(data.apellidoMaterno).prop('disabled', true);
    $('#telefono').val(data.telefono).prop('disabled', true);
    $('#idRolGuardar').val(data.idRol).change().prop('disabled', true);
    $('#idAlmacenGuardar').val(data.idAlmacen).change().prop('disabled', true);
    $('#idSucursalGuardar').val(data.idSucursal).change().prop('disabled', true);
    $('.field-validation-error').html("");

    //$('#activo').trigger('click');
    //para abrir el modal
    $('#EditarUsuarioModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalUsuario').html("Información del Usuario");

}

function EditarUsuario(idUsuario) {

    $('#btnGuardarUsuario').prop('disabled', false);

    var data = ObtenerUsuario(idUsuario);
    
    $('#idUsuario').val(idUsuario);
    $('#idRol').val(data.idRol);
    $('#idAlmacen').val(data.idAlmacen);
    $('#idSucursal').val(data.idSucursal);
    $('#activo').val(data.activo);

    $('#usuario').val(data.usuario).prop('disabled', false);
    $('#contrasena').val(data.contrasena).prop('disabled', false);
    $('#nombre').val(data.nombre).prop('disabled', false);
    $('#apellidoPaterno').val(data.apellidoPaterno).prop('disabled', false);
    $('#apellidoMaterno').val(data.apellidoMaterno).prop('disabled', false);
    $('#telefono').val(data.telefono).prop('disabled', false);
    $('#idRolGuardar').val(data.idRol).change().prop('disabled', false);
    $('#idAlmacenGuardar').val(data.idAlmacen).change().prop('disabled', false);
    $('#idSucursalGuardar').val(data.idSucursal).change().prop('disabled', false);
    $('.field-validation-error').html("");

    //$('#activo').trigger('click');
    //para abrir el modal
    $('#EditarUsuarioModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalUsuario').html("Editar Usuario");

}



function InitBtnAgregar() {
    $('#btnAgregarUsuario').click(function (e) {

        $('#btnGuardarUsuario').prop('disabled', false);

        $('#idUsuario').val(0);
        $('#idRol').val(0);
        $('#idAlmacen').val(0);
        $('#idSucursal').val(0);
        $('#activo').val(0);

        $('#usuario').val('').prop('disabled', false);
        $('#contrasena').val('').prop('disabled', false);
        $('#nombre').val('').prop('disabled', false);
        $('#apellidoPaterno').val('').prop('disabled', false);
        $('#apellidoMaterno').val('').prop('disabled', false);
        $('#telefono').val('').prop('disabled', false);
        $('#idRolGuardar').val('').change().prop('disabled', false);
        $('#idAlmacenGuardar').val('').change().prop('disabled', false);
        $('#idSucursalGuardar').val('').change().prop('disabled', false);
        $('.field-validation-error').html("");

        //$('#activo').trigger('click');
        //para abrir el modal
        $('#EditarUsuarioModal').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalUsuario').html("Agregar Usuario");

    });
}



function EliminarUsuario(idUsuario) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a este usuario?',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Usuarios/ActualizarEstatusUsuario"),
                    data: { idUsuario: idUsuario, activo: false },
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
                        console.log('Hubo un problema al intentar eliminar al usuario, contactese con el administrador del sistema');
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
    InitTableUsuarios();    
});