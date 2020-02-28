function onBeginSubmit() {

}

function onCompleteSubmit() {
 
}

function onSuccessResult(data) {
    console.log("onSuccessResult", JSON.stringify(data));   
    
    if (data.Estatus == 200) {
        location.href = 'Dashboard/Index/';
    } else {
        MuestraToast("error", data.Mensaje);
    }
}

function onFailureResult() {
    console.log("onFailureResult");
}

$(document).ready(function () {


});