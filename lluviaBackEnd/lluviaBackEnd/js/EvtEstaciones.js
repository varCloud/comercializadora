var tablaEstaciones;


function onBeginSubmitGuardarEstacion() {
}

function onCompleteSubmitGuardarEstacion() {
}

function onFailureResultGuardarEstacion() {
}

function onSuccessResultGuardarEstacion(data) {
    console.log(data);
    if (data.Estatus === 200) {
        MuestraToast("success", data.Mensaje)
        ObtenerEstaciones();
        $('#mdlAgregarEstacion').modal('hide');
    } else {
        MuestraToast("error", data.Mensaje)
    }
}

function ObtenerEstaciones() {
    $.ajax({
        url: rootUrl("/Estaciones/_ObtenerEstaciones"),
        data: { idEstacion: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaEstaciones.destroy();
            $('#rowTblEstaciones').html(data);
            InitTableEstaciones();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function EliminarEstacion(idEstacion) {
    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar esta Estación?',
        icon: 'warning',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Estaciones/EliminarEstacion"),
                    data: { idEstacion: idEstacion, idStatus :2  },
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                    },
                    success: function (data) {
                        console.log(data);
                        MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
                        ObtenerEstaciones();
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



function EditarEstacion(idEstacion) {
    $('.field-validation-error').html("");
    $('#idEstacion').val('0');

    var data = ObtenerEstacion(idEstacion);
    console.log(data);
    if (data.Estatus == 200) {

        $("#frmEstacion input").prop("disabled", false);
        $("#frmEstacion select").prop("disabled", false)
        $('#btnGuardarEstacion').css('display', '');
        $('#TituloModalEstacion').html("Actualizar Estación");
        
        $('#idEstacion').val(idEstacion);
        $('#nombre').val(data.Modelo[0].nombre);
        $('#numero').val(data.Modelo[0].numero);
        $('#idAlmacen').val(data.Modelo[0].idAlmacen);
        $('#mdlAgregarEstacion').modal({ backdrop: 'static', keyboard: false, show: true })

    } else {
        MuestraToast('info', data.Mensaje)
    }
}

function ObtenerEstacion(idEstacion)
{
    result = { "Estatus" : -1 ,  "Mensaje":"Espere un momento y vuelva a intentarlo"};
    $.ajax({
        url: rootUrl("/Estaciones/ObtenerEstacion"),
        data: { idEstacion: idEstacion },
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

function InitTableEstaciones() {
    var NombreTabla = "tblEstaciones";
    tablaEstaciones = initDataTable(NombreTabla)

    new $.fn.dataTable.Buttons(tablaEstaciones, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Estaciones",
                customize: function (doc) {

                    doc.defaultStyle.fontSize = 8; 
                    doc.styles.tableHeader.fontSize = 10; 
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '25%', '25%', '15%', '15%', '10%'];

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
                                {
                                    alignment: 'center',
                                    fontSize: 14,
                                    text: "Estaciones",
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

    tablaEstaciones.buttons(0, null).container().prependTo(
        tablaEstaciones.table().container()
    );

    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarEstacion" data-toggle="tooltip" title="Agregar Estación"><i class="fas fa-plus"></i></a>');
    InitBtnAgregar();
}

function InitBtnAgregar() {
    $('#btnAgregarEstacion').click(function (e) {
        $('.field-validation-error').html("");

       
        $("#frmEstacion input").prop("disabled", false);
        $("#frmEstacion select").prop("disabled", false);
        $('#idEstacion').val('0');
        $('#btnResetGuardarEstacion').trigger('click');
        $('#btnGuardarEstacion').css('display', '');
        //$('#idCliente').val('0');
        //para abrir el modal
        $('#mdlAgregarEstacion').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalEstacion').html("Agregar Estación");

    });
}

$(document).ready(function () {
    InitTableEstaciones();
    $('#btnResetGuardarEstacion').css('display','none');
});