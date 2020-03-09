var tablaClientes;


function onBeginSubmitGuardarCliente() {
}

function onCompleteSubmitGuardarCliente() {
}

function onFailureResultGuardarCliente() {
}

function onSuccessResultGuardarCliente(data) {
    console.log(data);
    if (data.Estatus === 200) {
        MuestraToast("success", data.Mensaje)
        ObtenerClientes();
        $('#mdlAgregarCliente').modal('hide');

    } else {
        MuestraToast("error", data.Mensaje)
    }
    

}

function ObtenerClientes() {
    $.ajax({
        url: rootUrl("/Clientes/_ObtenerClientes"),
        data: { idCliente: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaClientes.destroy();
            $('#rowTblClientes').html(data);
            InitTableClientes();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function EliminarCliente(idCliente) {
    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar este cliente?',
        icon: 'warning',
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Clientes/EliminarCliente"),
                    data: { idCliente: idCliente },
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                    },
                    success: function (data) {
                        console.log(data);
                        MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
                        ObtenerClientes();
                    },
                    error: function (xhr, status) {
                        console.log('Disculpe, existió un problema');
                        console.log(xhr);
                        console.log(status);
                    }
                });

            } else {
                console.log("cancelar");
            }
        });
}

function VerCliente(idCliente, accion) {
    //accion = 1 solo quiere ver al cliente
    //accion = 2 Va a editar el usario

    var data = ObtenerCliente(idCliente)
    if (data.Estatus == 200) {
        if (accion == 1) {
            $("#frmClientes input").prop("disabled", true);
            $("#frmClientes select").prop("disabled", true);
            $('#btnGuardarCliente').css('display', 'none');
            $('#TituloModalCliente').html("Cliente");
        } else {
            console.log("accion", accion);
            $('#idCliente').val(data.Modelo.idCliente);
            $("#frmClientes input").prop("disabled", false);
            $("#frmClientes select").prop("disabled", false)
            $('#btnGuardarCliente').css('display', '');
            $('#TituloModalCliente').html("Actualizar Cliente");
        }
        $('#nombres').val(data.Modelo.nombres);
        $('#apellidoPaterno').val(data.Modelo.apellidoPaterno);
        $('#apellidoMaterno').val(data.Modelo.apellidoMaterno);
        $('#telefono').val(data.Modelo.telefono);
        $('#correo').val(data.Modelo.correo);
        $('#rfc').val(data.Modelo.correo);
        $('#calle').val(data.Modelo.calle);
        $('#colonia').val(data.Modelo.colonia);
        $('#municipio').val(data.Modelo.municipio);
        $('#cp').val(data.Modelo.cp);
        $('#estado').val(data.Modelo.estado);
        $('#cbTipoCliente').val(data.Modelo.tipoCliente.idTipoCliente);
        $('#mdlAgregarCliente').modal({ backdrop: 'static', keyboard: false, show: true })
        
    } else {
        MuestraToast('info', data.Mensaje)
    }
}

function ObtenerCliente(idCliente)
{
    result = { "Estatus" : -1 ,  "Mensaje":"Espere un momento y vuelva a intentarlo"};
    $.ajax({
        url: rootUrl("/Clientes/ObtenerCliente"),
        data: { idCliente: idCliente },
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

function InitTableClientes() {
    var NombreTabla = "tblClientes";
    tablaClientes = initDataTable(NombreTabla)

    new $.fn.dataTable.Buttons(tablaClientes, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Clientes",
                customize: function (doc) {

                    doc.defaultStyle.fontSize = 8; 
                    doc.styles.tableHeader.fontSize = 10; 
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['5%', '20%', '20%', '15%', '15%', '10%', '15%'];

                    doc.content.splice(0, 1);
                    doc.pageMargins = [30, 85, 20, 30];
                    doc['header'] = (function () {
                        return {
                            columns: [
                                {
                                    image: logoBase64,
                                    width: 64,
                                    margin: [0, 20, -20, 0]
                                },
                                /*{
                                    alignment: 'left',
                                    italics: true,
                                    text: 'dataTables',
                                    fontSize: 18,
                                    margin: [10,0]
                                },*/
                                {
                                    alignment: 'center',
                                    fontSize: 14,
                                    text: "Clientes",
                                    margin: [0, 40 ,80]
                                }
                            ],
                            margin: [10, 0]
                        }
                    });// fin del doc header*/
                    doc['footer'] = (function (page, pages) {
                        return {
                            columns: [
                                {
                                    alignment: 'right',
                                    text: ['pagina ', { text: page.toString() }, ' de ', { text: pages.toString() }]
                                }
                            ],
                            margin: [0, 0,30]
                        }
                    });	// fin del doc footer*/	
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

            }
        ]
    });

    tablaClientes.buttons(0, null).container().prependTo(
        tablaClientes.table().container()
    );

    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarCliente" data-toggle="tooltip" title="Agregar Cliente"><i class="fas fa-user-plus"></i></a>');
    InitBtnAgregar();
}

function InitBtnAgregar() {
    $('#btnAgregarCliente').click(function (e) {

       
        $("#frmClientes input").prop("disabled", false);
        $("#frmClientes select").prop("disabled", false);
        $('#btnResetGuardarUsuario').trigger('click');
        $('#btnGuardarCliente').css('display', '');
        $('#idCliente').val('0');
        //para abrir el modal
        $('#mdlAgregarCliente').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalCliente').html("Agregar Cliente");

    });
}

$(document).ready(function () {
    InitTableClientes();
    $('#btnResetGuardarUsuario').css('display','none');
});