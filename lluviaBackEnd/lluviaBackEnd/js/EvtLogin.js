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

    $('#Toast').click(function (e) {
        //MuestraToast("error", "error");
        iziToast.warning({
            title: 'Hello, world!',
            message: 'This awesome plugin is made by iziToast',
            position: 'topRight',
            timeout: 95000,
            //icon: 'fa fa-chrome'

        });
    });

});