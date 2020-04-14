var key;
function onBeginSubmit() {

}

function onCompleteSubmit() {
 
}

function onSuccessResult(data) {
    console.log("onSuccessResult", JSON.stringify(data));   
    
    if (data.Estatus == 200) {
        if (){
            location.href = rootUrl("Dashboard/Index/");
        }
        else{
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



$(document).ready(function () {

    //$('#cancelar').click(function (e) {
    //    location.href = rootUrl("Login/Login/");
    //});

});