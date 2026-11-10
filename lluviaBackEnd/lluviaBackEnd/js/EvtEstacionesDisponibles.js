var key;
function onBeginSubmit() {

}

function onCompleteSubmit() {
 
}

function onSuccessResult(data) {

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
        }
    });
}

$(document).ready(function () {

    //$('#cancelar').click(function (e) {
    //    location.href = rootUrl("Login/Login/");
    //});

});