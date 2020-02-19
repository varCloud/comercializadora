function onBeginSubmit() {
    console.log("onBeginSubmit");
}

function onCompleteSubmit() {
    console.log("onCompleteSubmit");
}

function onSuccessResult(data) {
    console.log("onSuccessResult");
    //location.href = 'Dashboard/Index/';
    //alert(data.usuarioValido);
   
    if (data.usuarioValido) {
        location.href = 'Dashboard/Index/';
    } else {


    }
}

function onFailureResult() {
    console.log("onFailureResult");
}