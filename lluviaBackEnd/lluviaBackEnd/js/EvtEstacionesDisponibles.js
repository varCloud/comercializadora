var key;
function onBeginSubmit() {

}

function onCompleteSubmit() {

}

function onSuccessResult(data) {
    console.log("onSuccessResult", JSON.stringify(data));

    //if (data.Estatus == 200) {
    //    if () {
    //        location.href = rootUrl("Dashboard/Index/");
    //    }
    //    else {
    //        location.href = rootUrl("Dashboard/Index/");
    //    }

    //} else {
    //    InitRecaptcha();
    //    MuestraToast("error", data.Mensaje);
    //}
}

function onFailureResult() {
    console.log("onFailureResult");
}

function SeleccionarEstacion(idEstacion) {
    $.ajax({
        url: rootUrl("/Login/SeleccionarEstacion"),
        data: { idEstacion : idEstacion },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            //console.log("SeleccionarEstacion");
            //console.log(data);
            //if (data.Estatus == 200)
            //{
                location.href = rootUrl("Dashboard/Index/");
            //}

        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

$(document).ready(function () {

    //$('#cancelar').click(function (e) {
    //    location.href = rootUrl("Login/Login/");
    //});

});