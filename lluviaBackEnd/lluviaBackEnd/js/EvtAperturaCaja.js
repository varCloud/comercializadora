//Ingreso Efectivo
function onBeginSubmitIngresoEfectivo() {
    ShowLoader();
}
function onCompleteSubmitIngresoEfectivo() {
    OcultarLoader();
}
function onSuccessResultIngresoEfectivo(data) {
    
    if (data.status === 200) {
        MuestraToast('success', data.Mensaje);
        ImprimeTicketIngresoEfectivo(data.id);
        OcultarLoader();   
        location.href = rootUrl("Ventas/Ventas/");
    }
    else {
        OcultarLoader();   
        MuestraToast('error', data.Mensaje);
    }

}
function onFailureResultIngresoEfectivo() {
    OcultarLoader();
}

function ImprimeTicketIngresoEfectivo(idIngresoEfectivo) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicketIngresosEfectivo"),
        data: { idIngresoEfectivo: idIngresoEfectivo },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            console.log(data);
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
