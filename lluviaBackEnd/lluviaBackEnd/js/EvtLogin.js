var key;
function onBeginSubmit() {
    console.log('onBeginSubmit');
    $("#btnIniciarSesion").addClass('btn-progress disabled');
}
function onCompleteSubmit() {
  
}

function onSuccessResult(data) {
    console.log("onSuccessResult", JSON.stringify(data));   
    if (data.notificacion.Estatus == 200) {

        if (data.notificacion.Modelo.configurado == "0" ) {
            location.href = rootUrl("Login/EstacionesDisponibles/");
        }
        else {
            $("#btnIniciarSesion").removeClass('btn-progress disabled');
            location.href = rootUrl(data.controladorAccion);
        }

    } else {
        InitRecaptcha();
        MuestraToast("error", data.notificacion.Mensaje);
        $("#btnIniciarSesion").removeClass('btn-progress disabled');
    }
}

function onFailureResult() {
    console.log("onFailureResult");
    $("#btnIniciarSesion").removeClass('btn-progress disabled');
}
function InitRecaptcha() {
    var key = '6LfandgUAAAAAC71dmEsGltVDobKPEYFvC_ocuP_';
    grecaptcha.ready(function () {
        grecaptcha.execute(key, { action: 'homepage' }).then(function (token) {
            $("#Token").val(token);
            console.log("token: ", token);
        });
        element = document.getElementsByClassName('grecaptcha-badge');

    });
}
$(document).ready(function () {
    InitRecaptcha();
    $("#usuario").focus();    
    //consultarTicketPedidoEspecial();

});
