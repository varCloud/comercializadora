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
            InitDataTable();
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

function VerCliente(idCliente) {

    var data = ObtenerCliente(idCliente)
    if (data.Estatus == 200) {
        $('#nombres').val(data.Modelo.nombres).prop('disabled', true);
        $('#apellidoPaterno').val(data.Modelo.apellidoPaterno).prop('disabled', true);;
        $('#apellidoMaterno').val(data.Modelo.apellidoMaterno).prop('disabled', true);;
        $('#telefono').val(data.Modelo.telefono).prop('disabled', true);;
        $('#correo').val(data.Modelo.correo).prop('disabled', true);;
        $('#rfc').val(data.Modelo.correo).prop('disabled', true);;
        $('#calle').val(data.Modelo.calle).prop('disabled', true);;
        $('#colonia').val(data.Modelo.colonia).prop('disabled', true);;
        $('#municipio').val(data.Modelo.municipio).prop('disabled', true);;
        $('#cp').val(data.Modelo.cp).prop('disabled', true);;
        $('#estado').val(data.Modelo.estado).prop('disabled', true);;
        $('#cbTipoCliente').val(data.Modelo.tipoCliente.idTipoCliente).prop('disabled', true);;
        $('#btnGuardarUsuario').prop('disabled', false).css('display', 'none');
        $('#mdlAgregarCliente').modal({ backdrop: 'static', keyboard: false, show: true }).prop('disabled', true);
        $('#TituloModalCliente').html("Cliente");
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
                title: "Proveedores",
                customize: function (doc) {

                    doc.defaultStyle.fontSize = 8; 
                    doc.styles.tableHeader.fontSize = 10; 
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '20%', '10%'];

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
                                    // This is the right column
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

        $('#btnGuardarProveedor').prop('disabled', false);
        $("#frmClientes input").prop("disabled", false);
        $("#frmClientes select").prop("disabled", false);
        $('#btnResetGuardarUsuario').trigger('click');
        //para abrir el modal
        $('#mdlAgregarCliente').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalCliente').html("Agregar Cliente");

    });
}

$(document).ready(function () {
    InitTableClientes();
});