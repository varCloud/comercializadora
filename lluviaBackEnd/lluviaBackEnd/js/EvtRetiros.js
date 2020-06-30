var tblRetiros;
$(document).ready(function () {
    var idAlmacenUsuario = $('#idAlmacenUsuario').val();
    var idUsuarioLogueado = $('#idUsuarioLogueado').val();
    
    //console.log(idAlmacenUsuario);
    if (idAlmacenUsuario > 0) {
        $('#idAlmacen').val(idAlmacenUsuario).trigger('change');
        $('#idUsuario').val(idUsuarioLogueado).trigger('change');
        $('#idAlmacen').prop('disabled', true);
        $('#idUsuario').prop('disabled', true);
    }
    else {
        InitSelect2Usuarios(0);
    }
    //alert($('#idUsuario').val());

    $("#frmRetiros").submit();
    //InitDataTableRetiros();
});


function InitDataTableRetiros() {
    var NombreTabla = "tblRetiros";
    tblRetiros = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblRetiros, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Retiros",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['20%', '10%', '20%', '20%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Retiros de Efectivo");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5,6]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5,6]
                },
            },
        ],

    });

    tblRetiros.buttons(0, null).container().prependTo(
        tblRetiros.table().container()
    );
}

//busqueda
function onBeginSubmitRetirosAutorizacion() {
    ShowLoader("Buscando retiros...");
}

function onSuccessResultRetirosAutorizacion(data) {
    OcultarLoader();
    if (tblRetiros!=null)
        tblRetiros.destroy();
    $('#DivRetiros').html(data);
    InitDataTableRetiros();
}
function onFailureResultRetirosAutorizacion() {
    OcultarLoader();
    MuestraToast("error","Ocurrio un error al buscar los retiros")
}

function ActualizarEstatusRetiro(idRetiro, tipoRetiro,idStatus) {

    if (idStatus == 2) {     
 
        if ($("#Monto_" + idRetiro).val() == "" || parseFloat($("#Monto_" + idRetiro).val()) <= 0) {
            MuestraToast("error", "Debe de capturar el monto por el cual va a autorizar el retiro");
            return;
        }  
        

    }

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas ' + (idStatus == 2 ? 'autorizar' : 'cancelar') + ' el retiro ' + (tipoRetiro == 1 ? 'por exceso de efectivo' : 'al cierre del dìa') + '?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {

                var Status = {
                    idStatus: idStatus,
                    descripcion: "",
                  
                };

                var Retiro = {
                    idRetiro: idRetiro,
                    tipoRetiro: tipoRetiro,
                    estatusRetiro: Status,
                    montoAutorizado: parseFloat($("#Monto_" + idRetiro).val())
                };

                dataToPost = JSON.stringify({ retiros: Retiro });
                console.log(dataToPost);

                $.ajax({
                    url: rootUrl("/Ventas/ActualizaEstatusRetiro"),
                    data: { idRetiro: idRetiro, tipoRetiro: tipoRetiro, estatusRetiro: Status, montoAutorizado: (idStatus == 2 ? parseFloat($("#Monto_" + idRetiro).val())  : 0)},
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader((idStatus == 2 ? 'Autorizando...' : 'Cancelando...') );
                    },
                    success: function (data) {
                        OcultarLoader();
                        if (data.Estatus == 200) {
                            MuestraToast('success', data.Mensaje);
                            $("#frmRetiros").submit();
                        }
                        else {
                            MuestraToast('error', data.Mensaje);
                        }
                        
                    },
                    error: function (xhr, status) {
                        OcultarLoader();
                        MuestraToast('error', "Ocurrio un error al cambiar de estatus el retiro ");
                    }
                });

            } 
        });

}



$("#idAlmacen").on("change", function () {

    var idAlmacen = parseInt($('#idAlmacen').val());
    //alert(idAlmacen);

    if (idAlmacen > 0) {
        InitSelect2Usuarios(idAlmacen);
    }
    else {
        InitSelect2Usuarios(idAlmacen);
    }
    
}); 

function InitSelect2Usuarios(idAlmacen) {

    var result = '';
    $('#idUsuario').prop('disabled', false);

    $.ajax({
        url: rootUrl("/Usuarios/ObtenerUsuariosPorAlmacenyRol"),
        data: { idUsuario: 0, idRol: 3, idAlmacen : idAlmacen },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
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

    var i;
    for (i = 0; i < result.length; i++) {
        result[i].id = result[i]['idUsuario'];
        result[i].text = result[i]['nombre'] + " " + result[i]['apellidoPaterno'] + " " + result[i]['apellidoMaterno'];
    }
    //console.log(result);

    $("#idUsuario").html('').select2();
    $('#idUsuario').select2({
        width: "100%",
        placeholder: "-- TODOS --",
        data: result,

        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });

    $('#idUsuario').val("0").trigger('change');

    if (result.length <= 0) {
        $('#idUsuario').prop('disabled', true);
    }

}