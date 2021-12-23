$(document).ready(function () {
    $('#btnCierreCajasPedidosEspeciales').click(function (e) {
        e.preventDefault();

        if ($('#efectivoEntregadoEnCierre').val() === '') {
            MuestraToast('warning', "Debe escribir el monto entregado en el cierre.");
            return;
        }

        swal({
            title: 'Mensaje',
            text: '¿Estas seguro que desea hacer el cierre de pedidos especiales para esta Estación?',
            icon: 'warning',
            buttons: ["Cancelar", "Aceptar"],
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {

                    if (RequiereAutorizacion()) {
                        ModalAutorizarCierre();
                    }
                    else {
                        HacerCierre();
                    }

                } else {
                    console.log("cancelar");
                }
            });

    });

    $('#btnAutorizarCierre').click(function (e) {

        var usuario = $('#usuarioAutoriza').val();
        var contrasena = $('#contrasenaAutoriza').val();

        if ((usuario === '') || (contrasena === '')) {
            MuestraToast("error", "Debe ingresar Usuario y Contraseña para autorizar el cierre .");
            return;
        }

        $.ajax({
            url: rootUrl("/Ventas/ValidarContrasena"),
            data: { usuario: usuario, contrasena: contrasena },
            method: 'post',
            dataType: 'json',
            async: true,
            beforeSend: function (xhr) {
                ShowLoader("Validando");
            },
            success: function (data) {
                MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);

                if (data.Estatus === 200) {
                    $('#ModalAutorizarCierre').modal('hide');
                    HacerCierre();
                }

                OcultarLoader();
            },
            error: function (xhr, status) {
                console.log('Disculpe, existió un problema');
                console.log(xhr);
                console.log(status);
                OcultarLoader();
            }
        });

    });

    $("#usuarioAutoriza").on("keyup", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            document.getElementById("btnAutorizarCierre").click();
        }
    });

    $("#contrasenaAutoriza").on("keyup", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            document.getElementById("btnAutorizarCierre").click();
        }
    });

    
});


function ImprimeTicketCierreCajas() {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ImprimeTicketCierreCajas"),
        data: {  },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
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


function RequiereAutorizacion() {

    var requiereAutorizacion = true;

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ObtenerConfiguracionPedidosEspeciales"),
        data: { tipoConfig: 2 },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            //console.log(data);
            var result = JSON.parse(data);
            if (result.Estatus === 200) {
                if (parseInt(result.Modelo[0].valor) !== parseInt(1)) {
                    requiereAutorizacion = false;
                }
            }   
           
            OcultarLoader();
        },
        error: function (xhr, status) {
            console.log('Hubo un problema al intentar hacer el cierre de esta estación, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });

    return requiereAutorizacion;
}

function HacerCierre() {

    var efectivoEntregadoEnCierre = parseFloat($('#efectivoEntregadoEnCierre').val()).toFixed(2);

    if (efectivoEntregadoEnCierre <= 0) {
        MuestraToast('warning', "Debe escribir el monto entregado en el cierre.");
        return;
    }

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/RealizaCierreEstacion"),
        data: JSON.stringify({ efectivoEntregadoEnCierre: parseFloat($('#efectivoEntregadoEnCierre').val())}),
        method: 'post',
        dataType: 'json',
        async: true,
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            var result = JSON.parse(data); 
            OcultarLoader();
            MuestraToast(result.Estatus == 200 ? 'success' : 'error', result.Mensaje);
            if (result.Estatus) {
                //ImprimeTicketRetiro(data.Modelo.idRetiro, 2);
                location.href = rootUrl("PedidosEspecialesV2/CierreCajas/");
            }
        
            
            //ConsultExcesoEfectivo();
        },
        error: function (xhr, status) {
            console.log('Hubo un problema al intentar hacer el cierre de esta estación, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}


function ModalAutorizarCierre() {
    $("#usuarioAutoriza").val("");
    $("#contrasenaAutoriza").val("");
    $('#ModalAutorizarCierre').modal({ backdrop: 'static', keyboard: false, show: true });
}

