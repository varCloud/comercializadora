var tablaTipoClientes;

function onBeginSubmitGuardarTipoCliente() {
}
function onCompleteSubmitGuardarTipoCliente() {
}
function onFailureResultGuardarTipoCliente() {
}
function onSuccessResultGuardarTipoCliente(data) {
    console.log(data);
    if (data.Estatus === 200) {
        MuestraToast("success", data.Mensaje)
        ObtenerTiposClientes();
        $('#mdlAgregarTipoCliente').modal('hide');
    } else {
        MuestraToast("error", data.Mensaje)
    }
}

function ObtenerTiposClientes() {
    $.ajax({
        url: rootUrl("/Clientes/_ObtenerTiposClientes"),
        data: { idTipoCliente: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaTipoClientes.destroy();
            $('#rowTblTiposClientes').html(data);
            InitTableTipoClientes();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            console.log(data);
        }
    });
}

function EliminarTipoCliente(idTipoCliente) {
    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar este tipo de cliente?',
        icon: 'warning',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Clientes/EliminarTipoCliente"),
                    data: { idTipoCliente: idTipoCliente },
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                    },
                    success: function (data) {
                        console.log(data);
                        MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
                        ObtenerTiposClientes();
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

function EditarTipoCliente(idTipoCliente) {
    $('.field-validation-error').html("");

    var data = ObtenerTipoCliente(idTipoCliente)
    if (data.Estatus == 200) {
        $('#idTipoCliente').val(idTipoCliente);
        $('#descripcion').val(data.Modelo[0].descripcion);
        $('#descuento').val(data.Modelo[0].descuento);
        $('#activo').val(data.Modelo[0].activo);
        $('#mdlAgregarTipoCliente').modal({ backdrop: 'static', keyboard: false, show: true })
    } else {
        MuestraToast('info', data.Mensaje)
    }
}

function ObtenerTipoCliente(idTipoCliente)
{
    result = { "Estatus" : -1 ,  "Mensaje":"Espere un momento y vuelva a intentarlo"};
    $.ajax({
        url: rootUrl("/Clientes/ObtenerTipoCliente"),
        data: { idTipoCliente: idTipoCliente },
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

function InitTableTipoClientes() {
    var NombreTabla = "tblTipoClientes";
    tablaTipoClientes = initDataTable(NombreTabla)

    new $.fn.dataTable.Buttons(tablaTipoClientes, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Descuentos",
                customize: function (doc) {

                    doc.defaultStyle.fontSize = 8; 
                    doc.styles.tableHeader.fontSize = 10; 
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['5%', '40%', '40%'];

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
                                    text: "Descuentos",
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
                    columns: [0, 1, 2]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2]
                },

            }
        ]
    });

    tablaTipoClientes.buttons(0, null).container().prependTo(
        tablaTipoClientes.table().container()
    );

    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarTipoCliente" data-toggle="tooltip" title="Agregar Tipo de Cliente"><i class="fas fa-plus"></i></a>');
    InitBtnAgregar();
}

function InitBtnAgregar() {
    $('#btnAgregarTipoCliente').click(function (e) {
        $('.field-validation-error').html("");
        $("#frmTipoClientes input").prop("disabled", false);
        $('#btnResetGuardarTipoCliente').trigger('click');
        $('#btnGuardarTipoCliente').css('display', '');
        $('#idTipoCliente').val('0');
        //para abrir el modal
        $('#mdlAgregarTipoCliente').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalTipoCliente').html("Agregar Tipo de Cliente");
    });
}

$(document).ready(function () {
    InitTableTipoClientes();
    $('#btnResetGuardarTipoCliente').css('display','none');
});