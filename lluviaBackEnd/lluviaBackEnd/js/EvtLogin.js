var key;
function onBeginSubmit() {
    $("#btnIniciarSesion").addClass('btn-progress disabled');
}
function onCompleteSubmit() {
  
}

function onSuccessResult(data) {
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
    $("#btnIniciarSesion").removeClass('btn-progress disabled');
}
function InitRecaptcha() {
    var key = '6LfandgUAAAAAC71dmEsGltVDobKPEYFvC_ocuP_';
    grecaptcha.ready(function () {
        grecaptcha.execute(key, { action: 'homepage' }).then(function (token) {
            $("#Token").val(token);
        });
        element = document.getElementsByClassName('grecaptcha-badge');

    });
}
$(document).ready(function () {
    InitRecaptcha();
    $("#usuario").focus();

    // Ojito: alterna entre ocultar y mostrar la contraseña escrita.
    $("#btnVerContrasena").on("click", function () {
        var $campo = $("#contrasena");
        var mostrar = $campo.attr("type") === "password";
        $campo.attr("type", mostrar ? "text" : "password");
        $(this).attr("title", mostrar ? "Ocultar contraseña" : "Mostrar contraseña");
        $(this).find("i").toggleClass("fa-eye", !mostrar).toggleClass("fa-eye-slash", mostrar);
    });
    //consultarTicketPedidoEspecial();

});
