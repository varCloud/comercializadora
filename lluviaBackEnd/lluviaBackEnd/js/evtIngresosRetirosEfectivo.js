var cantidadEfectivo = parseFloat(0);
$(document).ready(function () {
    
    if (ValidarAperturaCaja()) {
        if (!ValidaCajaAbierta()) {
            AbrirModalIngresoEfectivo(1);
        }
    }


    $("#montoIngresoEfectivo").on("keyup", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            document.getElementById("GuardarIngresoEfectivo").click();
        }
    });

    $("#GuardarIngresoEfectivo").click(function (evt) {
        evt.preventDefault();

        if ($('#montoIngresoEfectivo').val() == "") {
            MuestraToast('warning', "Debe escribir la cantidad de ingreso de efectivo.");
            return;
        }

        var monto = parseFloat($('#montoIngresoEfectivo').val());

        if ($("#idTipoIngresoEfectivo").val() == 2) {
            if (monto <= 0) {
                MuestraToast('warning', "El ingreso de efectivo debe de ser mayor que 0");
                return;
            }
        }
       
        $.ajax({
            url: rootUrl("/PedidosEspecialesV2/IngresoEfectivo"),
            data: JSON.stringify({ montoIngresoEfectivo: monto, idTipoIngresoEfectivo: $("#idTipoIngresoEfectivo").val() }),
            method: 'post',
            dataType: 'json',
            async: true,
            contentType: "application/json; charset=utf-8",
            beforeSend: function (xhr) {
                ShowLoader("Registrando ingreso de efectivo...")
            },
            success: function (data) {
                OcultarLoader();
                var result = JSON.parse(data);
                console.log(result);
                if (result.Estatus === 200) {
                    MuestraToast('success', result.Mensaje);
                    ImprimeTicketIngresoEfectivo(parseInt(result.id));
                    $('#ModalIngresoEfectivo').modal('hide');
                    //ConsultExcesoEfectivo();
                }
                else
                    MuestraToast("error", result.Mensaje);
            },
            error: function (xhr, status) {
                console.log('Disculpe, existió un problema');
                console.log(xhr);
                console.log(status);
                OcultarLoader();
            }
        });
    })

    $('#btnRetirarExcesoEfectivo').click(function (e) {

        //var cantidadEfectivo = parseFloat($('#cantidadEfectivo').html().replace('<p class=\"clearfix\"> <span class=\"float-left\">Cantidad en Efectivo:</span><span class=\"float-right text-muted\">$', '').replace('</span></p>', '').replace(' ', '')).toFixed(2);
        var montoARetirar_ = parseFloat($('#montoARetirar').val()).toFixed(2);

        if ($('#montoARetirar').val() == "") {
            MuestraToast('warning', "Debe seleccionar un monto para retirar.");
            return
        }

        if ((cantidadEfectivo - montoARetirar_) < 0.0) {
            MuestraToast('warning', "Solo tiene : $" + (cantidadEfectivo).toString() + " para retirar.");
            return
        }
        // si todo bien
        retirarExcesoEfectivo(montoARetirar_);

    });


});

//Ingresos de efectivo
function ValidaCajaAbierta() {
    var cajaAbierta = false;
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ValidaCajaAbierta"),
        data: {  },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            cajaAbierta = data;

        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });

    return cajaAbierta;
}

function AbrirModalIngresoEfectivo(idTipoIngreso) {
    if (idTipoIngreso == 1) {
        $("#headerModalIngresoEfectivo").html('<h5 class="modal-title">Apertura de caja</h5>');
    }
    else {
        $("#headerModalIngresoEfectivo").html('<h5 class="modal-title">Ingreso de Efectivo</h5><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');

    }
    $('#idTipoIngresoEfectivo').val(idTipoIngreso);
    $('#montoIngresoEfectivo').val(0);
    $('#ModalIngresoEfectivo').modal({ backdrop: 'static', keyboard: false, show: true });
}

function ImprimeTicketIngresoEfectivo(idIngresoEfectivo) {    
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ImprimirTicketIngresoEfectivo"),
        data: JSON.stringify({ idIngresoEfectivo: parseInt(idIngresoEfectivo) }),
        method: 'post',
        dataType: 'html',
        async: true,
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            ShowLoader("Imprimiendo ticket...");
        },
        success: function (data) {
            console.log(data);
            OcultarLoader();
            MuestraToast('success', "Se envio el ticket a la impresora.");
        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast('error', "Ocurrio un error al enviar el ticket a la impresora.");
            console.log(xhr);
            console.log(status);
           
        }
    });
}

//Fin Ingresos de efectivo

//Retiros de efectivo
//retiro de exceso de efectivo
function AbrirModalRetiroExcesoEfectivo() {
    ConsultRetiros();
    ConsultaInfoCierre();
    $('#montoARetirar').val('');
    $('#ModalRetiroExcesoEfectivo').modal({ backdrop: 'static', keyboard: false, show: true });
}

function ConsultRetiros() {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ObtenerRetirosEfectivo"),
        data: { idRetiro: 0 },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            var html = "";
            var result = JSON.parse(data);
            if (result.Estatus !== 200) {
                html = '<div class="empty-state">' +
                    '<div class="empty-state-icon" >' +
                    '   <i class="fas fa-info"></i>' +
                    '</div>' +
                    '<h2> No se encontraron resultados</h2> ' +
                    '</div>';
            }
            else {

                html = '<div class="table-responsive">' +
                    '<table class="table table-striped" id = "tblRetirosEfectivo">' +
                    '    <thead>' +
                    '     <tr>' +
                    '         <th>Id</th>' +
                    '         <th>Monto</th>' +
                    '         <th>Usuario</th>' +
                    '         <th>Estación</th>' +
                    '         <th>Fecha</th>' +
                    '         <th>Reimprimir</th>' +
                    '     </tr>' +
                    ' </thead>' +
                    ' <tbody>';


                $.each(result.Modelo, function (index, dato) {
                    //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));           
                    html += '<tr>' +
                        '             <td>' + dato.idRetiro + '</td>' +
                        '             <td>' + formatoMoneda(dato.montoRetiro) + '</td>' +
                        '             <td>' + dato.nombreUsuario + '</td>' +
                        '             <td>' + dato.nombreEstacion + '</td>' +
                        '             <td>' + dato.fechaAlta + '</td>' +
                        '             <td class="text-center"><a href="javascript:ImprimeTicketRetiro(' + dato.idRetiro+')" data-toggle="tooltip" title="" data-original-title="Reimprimir Ticket"><i class="fas fa-print"></i></a> </td>' +
                        '</tr>';
                });
                html += ' </tbody>' +
                    '</table>' +
                    '</div>';
            }
            $('#divRetirosDia').html(html);
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

function ConsultaInfoCierre() {
    cantidadEfectivo = 0;
    $('#AperturaCaja').html("<p class=\"clearfix\"> <span class=\"float-left\">Apertura de Caja: </span><span class=\"float-right text-muted\">$0</span></p>");
    $('#IngresosEfectivo').html("<p class=\"clearfix\"> <span class=\"float-left\">Ingresos de Efectivo (Solicitud): </span><span class=\"float-right text-muted\">$0</span></p>");
    $('#pedidosEspecialesDelDia').html("<p class=\"clearfix\"> <span class=\"float-left\">Ventas del día: </span><span class=\"float-right text-muted\">$0</span></p>");
    $('#montoPedidosEspecialesDelDia').html("<p class=\"clearfix\"> <span class=\"float-left\">Monto de Ventas: </span><span class=\"float-right text-muted\">$0</span></p>");
    $('#cantidadEfectivo').html("<p class=\"clearfix\"> <span class=\"float-left\">Cantidad en Efectivo:</span><span class=\"float-right text-muted\">$0</span></p>");
    $('#cantidadRetirada').html("<p class=\"clearfix\"> <span class=\"float-left\">Cantidad Retirada del día:</span><span class=\"float-right text-muted\">$0</span></p>");

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ObtieneInfoCierre"),
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            OcultarLoader();
            var result = JSON.parse(data);
            console.log(result);
            if (result.Estatus === 200) {
                cantidadEfectivo = parseFloat(result.Modelo[0].efectivoDisponible);
                $('#AperturaCaja').html("<p class=\"clearfix\"> <span class=\"float-left\">Apertura de Caja: </span><span class=\"float-right text-muted\">" + formatoMoneda(result.Modelo[0].montoApertura) + "</span></p>");
                $('#IngresosEfectivo').html("<p class=\"clearfix\"> <span class=\"float-left\">Ingresos de Efectivo (Solicitud): </span><span class=\"float-right text-muted\">" + formatoMoneda(result.Modelo[0].montoIngresosEfectivo) + "</span></p>");
                $('#pedidosEspecialesDelDia').html("<p class=\"clearfix\"> <span class=\"float-left\">Ventas del día: </span><span class=\"float-right text-muted\">" + formatoMoneda(result.Modelo[0].totalPedidosEspeciales) + "</span></p>");
                $('#montoPedidosEspecialesDelDia').html("<p class=\"clearfix\"> <span class=\"float-left\">Monto de Ventas: </span><span class=\"float-right text-muted\">" + formatoMoneda(result.Modelo[0].montoPedidosEspecialesDelDia) + "</span></p>");
                $('#cantidadEfectivo').html("<p class=\"clearfix\"> <span class=\"float-left\">Cantidad en Efectivo:</span><span class=\"float-right text-muted\">" + formatoMoneda(result.Modelo[0].efectivoDisponible) + "</span></p>");
                $('#cantidadRetirada').html("<p class=\"clearfix\"> <span class=\"float-left\">Cantidad Retirada del día:</span><span class=\"float-right text-muted\">" + formatoMoneda(result.Modelo[0].retirosExcesoEfectivo) + "</span></p>");
            }
            else
                MuestraToast("error", result.Mensaje);
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}



function retirarExcesoEfectivo(montoRetiro) {

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/RetirarExcesoEfectivo"),
        data: JSON.stringify({ montoRetiro: parseFloat(montoRetiro) }),
        method: 'post',
        dataType: 'json',
        async: true,
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            OcultarLoader();
            var result = JSON.parse(data);
            MuestraToast(result.Estatus == 200 ? 'success' : 'error', result.Mensaje);
            $('#ModalRetiroExcesoEfectivo').modal('hide');
            ImprimeTicketRetiro(result.id);
            //ConsultExcesoEfectivo();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

function ImprimeTicketRetiro(idRetiro) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ImprimeTicketRetiro"),
        data: { idRetiro: idRetiro},
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            console.log(data);
            OcultarLoader();
            MuestraToast('success', "Se envio el ticket a la impresora.");
        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast('error', "Ocurrio un error al enviar el ticket a la impresora.");
            console.log(xhr);
            console.log(status);
            console.log(data);
        }
    });
}


//Fin de retiros de efectivo

//Abrir cajon
function AbrirCajonDinero() {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/AbrirCajon"),
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Abriendo Cajón...");
        },
        success: function (data) {
            console.log(data);
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast('error', "Ocurrio un error al intentar abrir el cajón del dinero.");
            console.log(xhr);
            console.log(status);
            console.log(data);
        }
    });
}
