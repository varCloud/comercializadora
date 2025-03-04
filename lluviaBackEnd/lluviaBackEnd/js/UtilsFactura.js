function ActualizarEstatusCancelacionFactura(idVenta, esPedidoEspecial = false, withAlert = false, fn = null) {
    $.ajax({
        url: pathDominio + "api/WsFactura/ActualizaEstatusCancelacionFactura",
        data: { id: idVenta, esPedidoEspecial: esPedidoEspecial },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader('Consultando estatus actual de la factura:' + idVenta)
        },
        success: function (data) {
            OcultarLoader();
            if (fn) {
                setTimeout(() => {
                    fn();
                }, 1000)
            }
            if (withAlert) {
                swal({
                    title: 'Estatus de la factura: #' + idVenta,
                    text: `${data.Modelo.Estado} | ${data.Modelo.EstatusCancelacion} | ${data.Modelo.ValidacionEFOS} `,
                    icon: 'success',
                })

            }
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('LLUVIA: Hubo un problema al intentar eliminar al usuario, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function UtilsFacturaPedidoEspecial(idPedidoEspecial, urlToRedirect = undefined, fn = null) {
    console.log(`UtilsFacturaPedidoEspecial`);
    $.ajax({
        url: pathDominio + "api/WsFactura/GenerarFactura",
        data: { idPedidoEspecial: idPedidoEspecial, idVenta: 0, idUsuario: idUsuarioGlobal },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Facturando Venta.");
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            OcultarLoader();
            if (urlToRedirect) {
                window.location.href = rootUrl(urlToRedirect);
            }
            if (fn) {
                fn();
            }
        },
        error: function (xhr, status) {
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}