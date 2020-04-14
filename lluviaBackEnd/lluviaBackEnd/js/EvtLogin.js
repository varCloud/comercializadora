var key;
function onBeginSubmit() {

}
function onCompleteSubmit() {
 
}

function onSuccessResult(data) {
    console.log("onSuccessResult", JSON.stringify(data));   
    
    if (data.Estatus == 200) {

        if (data.Modelo.configurado == "0" ) {
            location.href = rootUrl("Login/EstacionesDisponibles/");
        }
        else {
            location.href = rootUrl("Dashboard/Index/");
        }
    } else {
        InitRecaptcha();
        MuestraToast("error", data.Mensaje);
    }
}

function onFailureResult() {
    console.log("onFailureResult");
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
    
});