
function ImprimeTicketRetiro(idRetiro, tipoRetiro) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicketRetiro"),
        data: { idRetiro: idRetiro, idCliente: idRetiro, tipoRetiro: tipoRetiro },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {          
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