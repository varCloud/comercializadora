﻿var arrayPreciosRangos = [];
var arrayProductos = [];
var puedeGuardar = true;

function ValidarAperturaCaja() {
    return true;
}

$('#btnCancelar').click(function (e) {

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/EntregarPedido"),
        data: null,
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
        }
    });

});

$('#btnCancelarEntregarPedidoEspecial').click(function (e) {
    onCancelar()
})

$('#btnGuardarPedidoEspecial').click(function (e) {

    document.getElementById("chkCliente").checked = true;
    document.getElementById("chkRuteo").checked = false;
    document.getElementById("chkTaxi").checked = false;
    document.getElementById("chkLiquidado").checked = false;
    document.getElementById("chkCredito").checked = false;
    document.getElementById("chkCreditoConAbono").checked = false;
    document.getElementById("chkFacturarPedido").checked = false;

    $('#idUsuarioRuteo').val("").trigger('change');
    $('#idUsuarioTaxi').val("").trigger('change');
    $('#formaPago').val("1").trigger('change');
    $('#usoCFDI').val("3").trigger('change');

    document.getElementById("idUsuarioRuteo").disabled = true;
    document.getElementById("idUsuarioTaxi").disabled = true;
    document.getElementById("numeroUnidadTaxi").value = "";
    document.getElementById("observacionesPedidoRuta").value = "";
    document.getElementById("numeroUnidadTaxi").disabled = true;
    document.getElementById("divObservacionesPedidoRuta").style.display = 'none';
   
    actualizarSubTotal();

    if (validarProductosAceptados()) {

        ValidarPedidoEnRuta();

        $('#ModalEntregarPedidoEspecial').modal({ backdrop: 'static', keyboard: false, show: true });
    }
    calculaTotales('true');
});

$("#formaPago").on("change", function (value) {
    //    this.value == 1 ? $('#dvEfectivo').css('display', '') : $('#dvEfectivo').css('display', 'none');
    var chk = "";

    if ($("#chkLiquidado").is(":checked"))
        chk = "chkLiquidado";

    if ($("#chkCredito").is(":checked"))
        chk = "chkCredito";

    if ($("#chkCreditoConAbono").is(":checked"))
        chk = "chkCreditoConAbono";
    
    RevisarInputEfectivo(chk);
    calculaTotales('true');

});

function RevisarInputEfectivo(chk) {

    var formaPago = parseInt($('#formaPago').val());

    if  (
         ( formaPago == parseInt(4) || formaPago == parseInt(18)) && 
         ( (chk == 'chkCredito' || chk == 'chkLiquidado') )
        ) {
            $('#dvEfectivo').css('display', 'none');
        }
    else {
        $('#dvEfectivo').css('display', '');
        }


}


function calculaTotales(conReseteoCampos) {

    if (conReseteoCampos === 'true') {
        $('#efectivo').val('');
        document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
        document.getElementById("chkFacturarPedido").checked = false;
        document.getElementById("divUsoCFDI").style.display = 'none';
        $('#usoCFDI').val("3").trigger('change');
    }

    var formaPago = $('#formaPago').val();
    var porcentajeComisionBancaria = parseFloat(0);
    var descuento = parseFloat(0);

    // si la forma de pago es tarjeta de debito o credito se agrega comision bancaria
    if (
        ((parseInt(formaPago) == parseInt(4)) ||  //Tarjeta de crédito
            (parseInt(formaPago) == parseInt(18))) &&  //Tarjeta de débito
        (!$("#chkFacturarPedido").is(":checked"))       // y si la venta no es facturada
    ) {
        porcentajeComisionBancaria = $('#comisionBancaria').val();
    }
    //console.log('id_cliente___' + $('#idCliente').val());
    if (parseFloat($('#idCliente').val()) != 1) {
        descuento = $("#txtDescuentoCliente").val();
    }

    var total = parseFloat(document.getElementById("previoTotal").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    var descuentoMenudeo = parseFloat(document.getElementById("previoDescuentoMenudeo").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    var cantidadDescontada = parseFloat(0).toFixed(2);

    if (descuento > 0.0) {
        cantidadDescontada = parseFloat((total - descuentoMenudeo) * (descuento / 100)).toFixed(2);
    }
    //console.log(porcentajeComisionBancaria);
    var subTotal = parseFloat(total - descuentoMenudeo - cantidadDescontada).toFixed(2);
    var comisionBancaria = (parseFloat((subTotal) * (porcentajeComisionBancaria / 100))).toFixed(2);
    subTotal = (parseFloat(subTotal) + parseFloat(comisionBancaria)).toFixed(2);
    var iva = parseFloat(0).toFixed(2);

    // si lleva iva
    if ($("#chkFacturarPedido").is(":checked")) {
        iva = parseFloat(subTotal * 0.16).toFixed(2);
    }

    var final = (parseFloat(subTotal) + parseFloat(iva)).toFixed(2);

    document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + cantidadDescontada + "</h4>";
    document.getElementById("previoComisionBancaria").innerHTML = "<h4>$" + comisionBancaria + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + subTotal + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + iva + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + final + "</h4>";

}



function RevisarDescuentoCliente() { 

    var idCliente = parseFloat($('#idCliente').val());
    var descuento = parseFloat(0.0);

    if (idCliente != 1) {
        descuento = parseFloat($('#descuentoCliente').val()).toFixed(2);;
    }
    $("#txtDescuentoCliente").val(descuento);
    calculaTotales('true');
}


function ValidarClienteParaFacturacion(checked_) {
    var idCliente = $('#idCliente').val();
    var status = true;
    cliente = listClientes.find(x => x.idCliente == idCliente)

    if (cliente) {
        if (!validarEmail(cliente.correo)) {
            MuestraToast('warning', "No es posible facturar a un cliente sin correo electrónico vàlido");
            document.getElementById("chkFacturarPedido").checked = checked_;
            status = false;
        }

        if (!validarRFC(cliente.rfc)) {
            MuestraToast('warning', "No es posible facturar a un cliente sin RFC vàlido");
            document.getElementById("chkFacturarPedido").checked = checked_;
            status = false;
        }
    } else {
        MuestraToast('warning', "No es posible facturar a este cliente por favor comuníquese con el administrador web");
        document.getElementById("chkFacturarPedido").checked = checked_;
        status = false;
    }
    return status;
}


$('#chkFacturarPedido').click(function () {

    //var idCliente = $('#idCliente').val();
    var esDevolucion = $('#esDevolucion').val();
    var formaPago = $('#formaPago').val();
    var iva = parseFloat(0).toFixed(2);
    var porcentajeIva = parseFloat(0.16).toFixed(2);

    if ($('#chkFacturarPedido').is(':checked')) {

        if (!ValidarClienteParaFacturacion(false)) {
            return;
        }
        document.getElementById("divChkLiquidado").style.display = 'block';
        document.getElementById("divChkCredito").style.display = 'none';
        document.getElementById("divChkCreditoConAbono").style.display = 'none';

    }
    else {
        document.getElementById("divChkLiquidado").style.display = 'block';
        document.getElementById("divChkCredito").style.display = 'block';
        document.getElementById("divChkCreditoConAbono").style.display = 'block';
    }


    $('#efectivo').val('');

    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    //    document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    var subTotal = parseFloat(document.getElementById("previoSubTotal").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    var final = parseFloat(subTotal).toFixed(2);

    if ($(this).is(':checked')) {
        document.getElementById("divUsoCFDI").style.display = 'block';
        iva = parseFloat(subTotal * porcentajeIva).toFixed(2);
        final = (parseFloat(subTotal) + parseFloat(iva)).toFixed(2);
    } else {
        document.getElementById("divUsoCFDI").style.display = 'none';
    }
    document.getElementById("previoIVA").innerHTML = "<h4>$" + iva + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + final + "</h4>";

    calculaTotales('false');

});

$('#btnEntregarPedidoEspecial').click(function (e) {
    console.log('guardando....')
    if (!puedeGuardar) {
        console.log('No puede guardar varias veces esta venta')
        return false
    }
    puedeGuardar = false;

    var idUsuarioRuteo = $('#idUsuarioRuteo').val() == '' ? 0 : $('#idUsuarioRuteo').val()
    var idUsuarioTaxi = $('#idUsuarioTaxi').val();
    var numeroUnidadTaxi = "0";
    var idUsuarioEntrega = parseInt(0);
    var idPedidoEspecial = parseInt(0);
    var idEstatusPedidoEspecial = parseInt(0);
    var idEstatusCuentaPorCobrar = parseInt(0);
    var montoPagado = parseFloat(0);
    var observacionesPedidoRuta = "";
    var esPedidoEnRuta = $('#esPedidoEnRuta').val();
    var productos = [];
    var aCredito = parseInt(0);
    var aCreditoConAbono = parseInt(0);
    var formaPago = $('#formaPago').val();
    var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
    var total_ = parseFloat($("#previoFinal").html().replace('<h4>$', '').replace('</h4>', ''));
    var aplicaIVA = parseInt(0);
    var idFactUsoCFDI = parseInt(0);
    var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

    $("#btnEntregarPedidoEspecial").addClass('btn-progress disabled');

    if ($("#chkLiquidado").is(":checked") || $("#chkCreditoConAbono").is(":checked")) {
        
        if (parseInt(formaPago) != parseInt(4) && parseInt(formaPago) != parseInt(18)) {

            if ($('#efectivo').val() == "") {
                MuestraToast('warning', "Debe escribir con cuanto efectivo le estan pagando.");
                $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
                puedeGuardar = true
                return;
            }

            if ( (parseFloat(efectivo_) < parseFloat(total_)) && ($("#chkLiquidado").is(":checked")) ) {
                MuestraToast('warning', "El efectivo no alcanza a cubrir el costo total del pedido: " + total_.toString());
                $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
                puedeGuardar = true
                return;
            }
        
        }

        if ($('#efectivo').val() == "") {
            montoPagado = parseFloat(0.0);
        }
        else {
            montoPagado = $('#efectivo').val();
        }
        
    }


    if ($("#chkFacturarPedido").is(":checked")) {
        aplicaIVA = parseInt(1);
        idFactUsoCFDI = $('#usoCFDI').val();
    }

    if ((parseInt(formaPago) !== parseInt(1))) // si no es efectivo
    {
        efectivo_ = total_;
    }

    if (
        !$("#chkCliente").is(":checked") &&
        !$("#chkRuteo").is(":checked") &&
        !$("#chkTaxi").is(":checked")
    ) {
        MuestraToast('warning', "Debe elegir a quien va a entregar el Pedido Especial");
        $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
        puedeGuardar = true
        return;
    }

    if (
        !$("#chkLiquidado").is(":checked") &&
        !$("#chkCredito").is(":checked") &&
        !$("#chkCreditoConAbono").is(":checked")
    ) {
        MuestraToast('warning', "Debe elegir el tipo de pago");
        $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
        puedeGuardar = true
        return;
    }

    // 4	Entregado y pagado
    // 6	Pagado
    if ($("#chkLiquidado").is(":checked")) {
        if ($("#chkCliente").is(":checked")) {
            idEstatusPedidoEspecial = parseInt(6);
        }
        else {
            idEstatusPedidoEspecial = parseInt(4);
        }
    }
    // 5	Entregado a repartidor sin ser pagado
    // 7	Entregado a crédito
    else {
        if ($("#chkCliente").is(":checked")) {
            idEstatusPedidoEspecial = parseInt(7);
        }
        else {
            idEstatusPedidoEspecial = parseInt(5);
        }
    }

    // entregado a encargado de ruteo
    if ($("#chkRuteo").is(":checked")) {
        if (
            (isNaN(parseInt(idUsuarioRuteo))) ||
            ((parseInt(idUsuarioRuteo)) == 0)
            ) {
            MuestraToast('warning', "Debe elegir el usuario Encargado de Ruteo que recibe el Pedido Especial");
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            puedeGuardar = true
            return;
        }
        numeroUnidadTaxi = "0";
        idEstatusPedidoEspecial = parseInt(9);  // 9	Pedido en Ruta
        observacionesPedidoRuta = $('#observacionesPedidoRuta').val();
        idUsuarioRuteo = $('#idUsuarioRuteo').val();
    }


    // entregado a taxi
    if ($("#chkTaxi").is(":checked")) {

        if (isNaN(parseInt(idUsuarioTaxi))) {
            MuestraToast('warning', "Debe elegir el usuario de Taxi que recibe el Pedido Especial");
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            puedeGuardar = true
            return;
        }

        if (($('#numeroUnidadTaxi').val() == "")) {
            MuestraToast('warning', "Debe elegir el numero de la unidad de Taxi que recibe el Pedido Especial");
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            puedeGuardar = true
            return;
        }

        idUsuarioEntrega = idUsuarioTaxi;
        numeroUnidadTaxi = $('#numeroUnidadTaxi').val();
    }



    // si es liquidado en su totalidad 
    if ($("#chkLiquidado").is(":checked")) {

        if  (
                ($('#efectivo').val() == "") &&
                (parseInt(formaPago) != parseInt(4) && parseInt(formaPago) != parseInt(18))
            ) {
            MuestraToast('warning', "Debe escribir el monto total de liquidación");
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            puedeGuardar = true
            return;
        }
        //montoTotal = $('#efectivo').val();
    }


    // si es a credito pero deja abono
    if ($("#chkCreditoConAbono").is(":checked")) {

        if (($('#efectivo').val() == "")) {
            MuestraToast('warning', "Debe escribir la cantidad que abono al Pedido Especial");
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            puedeGuardar = true
            return;
        }
        //montoPagado = $('#efectivo').val();
    }


    // si es a credito -> afectar estructuras de cuentas por cobrar
    if ( ($("#chkCredito").is(":checked")) ) {
        aCredito = parseInt(1);
        //idMetodoPago = parseInt(2);
    }


    if ( ($("#chkCreditoConAbono").is(":checked")) ) {
        aCreditoConAbono = parseInt(1);
        //idMetodoPago = parseInt(2);
    }


    //validaciones
    if ( (parseFloat(efectivo_) > parseFloat(total_) ) && ( formaPago != 1 ) ) {
        MuestraToast('warning', "No puede abonar mas del total del pedido especial .");
        $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
        puedeGuardar = true
        return;
    }
    //console.log(esPedidoEnRuta);
    if (esPedidoEnRuta == null) {
        esPedidoEnRuta = 'false';
    }
    //console.log(esPedidoEnRuta);

    // si todo bien
    var tblProductos = document.getElementById('tblConfirmarProductos');
    var rCount = tblProductos.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var row_ = {
                idProducto: parseInt(tblProductos.rows[i].cells[1].innerHTML),
                cantidadSolicitada: parseFloat(tblProductos.rows[i].cells[6].innerHTML),
                cantidadAtendida: parseFloat(tblProductos.rows[i].cells[8].innerHTML),
                cantidadRechazada: parseFloat(tblProductos.rows[i].cells[9].innerHTML),
                cantidadAceptada: parseFloat(tblProductos.rows[i].cells[10].children[0].value),
                observaciones: tblProductos.rows[i].cells[11].children[0].value,
                idPedidoEspecialDetalle: parseInt(tblProductos.rows[i].cells[13].innerHTML),
            };
            productos.push(row_);
            idPedidoEspecial = parseInt(tblProductos.rows[i].cells[12].innerHTML);
        }
    }

    dataToPost = JSON.stringify({
        productos: productos, idPedidoEspecial: idPedidoEspecial, idEstatusPedidoEspecial: idEstatusPedidoEspecial, idUsuarioEntrega: idUsuarioEntrega,
        numeroUnidadTaxi: numeroUnidadTaxi, idEstatusCuentaPorCobrar: idEstatusCuentaPorCobrar, montoPagado: montoPagado, aCredito: aCredito,
        aCreditoConAbono: aCreditoConAbono, aplicaIVA: aplicaIVA, idFactFormaPago: formaPago, idFactUsoCFDI: idFactUsoCFDI,
        observacionesPedidoRuta: observacionesPedidoRuta, idUsuarioRuteo: idUsuarioRuteo, esPedidoEnRuta: esPedidoEnRuta
    });

    //console.log(dataToPost);
    //$("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
    //return;

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/GuardarConfirmacion"),
        data: dataToPost,
        method: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Guardando Confirmación de Productos.");
            $("#btnEntregarPedidoEspecial").addClass('btn-progress disabled');
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            if (data.Estatus == 200) {

                if (
                    (idEstatusPedidoEspecial == 9) &&
                    (esPedidoEnRuta == 'false' || esPedidoEnRuta == 'False')
                ) {
                    ImprimeTicketPedidoEspecial(idPedidoEspecial, 3, 0);
                }
                else {
                    ImprimeTicketPedidoEspecial(idPedidoEspecial,1,0);
                }


                if ($("#chkFacturarPedido").is(":checked")) {
                    const urlRedirect = "/PedidosEspecialesV2/EntregarPedido"
                    UtilsFacturaPedidoEspecial(idPedidoEspecial, urlRedirect, null);
                    $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
                }
                else
                    window.location.href = rootUrl(redirectPathBack(esPedidoEnRuta));

            }
            OcultarLoader();
            $('#ModalEntregarPedidoEspecial').modal('hide');
            puedeGuardar = true

        },
        error: function (xhr, status) {
            OcultarLoader();
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            console.log('Hubo un problema al guardar la confirmacion de productos, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
            puedeGuardar = true
        }
    });

});

function redirectPathBack(esPedidoEnRuta) {
    var path = '/PedidosEspecialesV2'
    if (esPedidoEnRuta == 'false' || esPedidoEnRuta == 'False')
        path = `${path}/EntregarPedido`
    else
        path = `${path}/PedidosEnRuta`
 
    return path
}

function onCancelar() {
    var path = '/PedidosEspecialesV2'
    esPedidoEnRuta = $('#esPedidoEnRuta').val();
    if (esPedidoEnRuta == null) {
        path = `${path}/EntregarPedido`
    } else {
        path = `${path}/PedidosEnRuta`
    }
    window.location.href = rootUrl(redirectPathBack(esPedidoEnRuta));
}

function ImprimeTicketPedidoEspecial(idPedidoEspecial, idTipoTicketPedidoEspecial, idTicketPedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ImprimeTicket"),
        data: { idPedidoEspecial: idPedidoEspecial, idTipoTicketPedidoEspecial: idTipoTicketPedidoEspecial, idTicketPedidoEspecial: idTicketPedidoEspecial },
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

//function facturaPedidoEspecial(idPedidoEspecial) {
//    $.ajax({
//        url: pathDominio + "api/WsFactura/GenerarFactura",
//        data: { idPedidoEspecial: idPedidoEspecial, idVenta: 0, idUsuario: idUsuarioGlobal },
//        method: 'post',
//        dataType: 'json',
//        async: true,
//        beforeSend: function (xhr) {
//            ShowLoader("Facturando Venta.");
//        },
//        success: function (data) {
//            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
//            OcultarLoader();
//            window.location.href = rootUrl("/PedidosEspecialesV2/EntregarPedido");
//        },
//        error: function (xhr, status) {
//            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
//            console.log('Disculpe, existió un problema');
//            console.log(xhr);
//            console.log(status);
//            OcultarLoader();
//        }
//    });
//}

function InitSelect2() {
    $('.select-multiple').select2({
        width: "100%",
        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });
}


$('#chkCliente').click(function () {
    chkChangeEntregar('chkCliente');
});

$('#chkRuteo').click(function () {
    chkChangeEntregar('chkRuteo');
});

$('#chkTaxi').click(function () {
    chkChangeEntregar('chkTaxi');
});

$('#chkLiquidado').click(function () {
    chkChangeTipoPago('chkLiquidado');
});

$('#chkCredito').click(function () {
    chkChangeTipoPago('chkCredito');
});

$('#chkCreditoConAbono').click(function () {
    chkChangeTipoPago('chkCreditoConAbono');
});

function chkChangeEntregar(chk) {

    if (chk == 'chkCliente') {

        document.getElementById("chkRuteo").checked = false;
        document.getElementById("chkTaxi").checked = false;

        document.getElementById("divFormaPago").style.display = 'block';

        document.getElementById("dvEfectivo").style.display = 'block';
        document.getElementById("efectivo").value = "";

        $('#idUsuarioRuteo').val("").trigger('change');
        document.getElementById("idUsuarioRuteo").disabled = true;

        $('#idUsuarioTaxi').val("").trigger('change');
        document.getElementById("idUsuarioTaxi").disabled = true;

        //document.getElementById("idCliente").disabled = false;
        document.getElementById("numeroUnidadTaxi").value = "";
        document.getElementById("numeroUnidadTaxi").disabled = true;

        document.getElementById("chkFacturarPedido").checked = false;
        checksTipoPagoTodos('block');
        checksTipoPagoChecked(false);
        document.getElementById("chkFacturarPedido").disabled = false;
        document.getElementById("divUsoCFDI").style.display = 'none';

        document.getElementById("divObservacionesPedidoRuta").style.display = 'none';
        document.getElementById("observacionesPedidoRuta").value = "";

    }

    if (chk == 'chkRuteo') {

        document.getElementById("chkCliente").checked = false;
        document.getElementById("chkTaxi").checked = false;

        document.getElementById("divFormaPago").style.display = 'none';

        document.getElementById("dvEfectivo").style.display = 'none';
        document.getElementById("efectivo").value = "";

        $('#idUsuarioTaxi').val("").trigger('change');
        document.getElementById("idUsuarioTaxi").disabled = true;

        document.getElementById("idUsuarioRuteo").disabled = false;
        document.getElementById("numeroUnidadTaxi").value = "";
        document.getElementById("numeroUnidadTaxi").disabled = true;

        document.getElementById("chkFacturarPedido").checked = false;
        checksTipoPagoTodos('block');
        checksTipoPagoChecked(false);
        document.getElementById("chkFacturarPedido").disabled = true;
        document.getElementById("divUsoCFDI").style.display = 'none';

        document.getElementById("divObservacionesPedidoRuta").style.display = 'block';
        document.getElementById("observacionesPedidoRuta").value = "";

        //cuando es pedido en ruta solo puede ser el pedido a credito
        document.getElementById("divChkLiquidado").style.display = 'none';
        document.getElementById("divChkCreditoConAbono").style.display = 'none';
        document.getElementById("chkCredito").checked = true;

    }

    if (chk == 'chkTaxi') {

        document.getElementById("chkCliente").checked = false;
        document.getElementById("chkRuteo").checked = false;

        document.getElementById("divFormaPago").style.display = 'block';

        document.getElementById("dvEfectivo").style.display = 'block';
        document.getElementById("efectivo").value = "";

        $('#idUsuarioRuteo').val("").trigger('change');
        document.getElementById("idUsuarioRuteo").disabled = true;

        //$('#idCliente').val("0").trigger('change');
        //document.getElementById("idCliente").disabled = true;

        document.getElementById("idUsuarioTaxi").disabled = false;
        document.getElementById("numeroUnidadTaxi").value = "";
        document.getElementById("numeroUnidadTaxi").disabled = false;

        document.getElementById("chkFacturarPedido").checked = false;
        checksTipoPagoTodos('block');
        checksTipoPagoChecked(false);
        document.getElementById("chkFacturarPedido").disabled = false;
        document.getElementById("divUsoCFDI").style.display = 'none';

        document.getElementById("divObservacionesPedidoRuta").style.display = 'none';
        document.getElementById("observacionesPedidoRuta").value = "";

    }

}


function checksTipoPagoTodos(display)
 {
    document.getElementById("divChkLiquidado").style.display = display;
    document.getElementById("divChkCredito").style.display = display;
    document.getElementById("divChkCreditoConAbono").style.display = display;
}

function checksTipoPagoChecked(status) {
    document.getElementById("chkLiquidado").checked = status;
    document.getElementById("chkCredito").checked = status;
    document.getElementById("chkCreditoConAbono").checked = status;
}

function chkChangeTipoPago(chk) {
    //var formaPago = parseInt($('#formaPago').val());
    if (chk == 'chkLiquidado') {

        document.getElementById("formaPago").disabled = false;
        document.getElementById("divFormaPago").style.display = 'block';
        $('#dvEfectivo').css('display', '');
        $('#efectivo').val('');
        RevisarInputEfectivo(chk);

        document.getElementById("chkCredito").checked = false;
        document.getElementById("chkCreditoConAbono").checked = false;

    }

    if (chk == 'chkCredito') {

        document.getElementById("formaPago").disabled = true;
        document.getElementById("divFormaPago").style.display = 'none';
        $('#efectivo').val('');
        $('#dvEfectivo').css('display', 'none');

        document.getElementById("chkLiquidado").checked = false;
        document.getElementById("chkCreditoConAbono").checked = false;

    }

    if (chk == 'chkCreditoConAbono') {

        document.getElementById("formaPago").disabled = false;
        document.getElementById("divFormaPago").style.display = 'block';
        $('#dvEfectivo').css('display', '');
        $('#efectivo').val('');
        RevisarInputEfectivo(chk);

        document.getElementById("chkLiquidado").checked = false;
        document.getElementById("chkCredito").checked = false;

    }

}

function validarProductosAceptados() {

    var faltantes = parseInt(0);
    var faltantes_ = parseInt(0);
    var cantidad = parseFloat(0);
    var tblProductos = document.getElementById('tblConfirmarProductos');
    var rCount = tblProductos.rows.length;


    if (rCount >= 2) {

        for (var i = 1; i < rCount; i++) {
            
            if (parseFloat(tblProductos.rows[i].cells[10].children[0].value) > parseFloat(tblProductos.rows[i].cells[8].innerHTML))
            {
                if (faltantes_ == 0) {
                    MuestraToast('warning', "No puedes aceptar una cantidad de productos mayor a la cantidad atendida");
                }
                faltantes_ += 1;
                return false;
            }

            if  (
                  (parseFloat(tblProductos.rows[i].cells[10].children[0].value) == 0 || ( (parseFloat(tblProductos.rows[i].cells[10].children[0].value)) != (parseFloat(tblProductos.rows[i].cells[8].innerHTML)) ) ) &&
                  (String(tblProductos.rows[i].cells[11].children[0].value) == "")
                ) {
                    if (faltantes == 0) {
                        MuestraToast('warning', "Tiene que capturar las observaciones si no esta aceptando todos los productos.");
                    }
                    faltantes += 1;
                    return false;
            }

            cantidad += parseFloat(tblProductos.rows[i].cells[10].children[0].value);
        }
    }

    //console.log(faltantes);
    //console.log(faltantes_);
    if (cantidad <= 0.0) {
        MuestraToast('warning', "Para cancelar todos los productos de todo el pedido debe hacerlo desde el menu de Entregar Pedido.");
        return false;
    }
    else {

        if (faltantes == 0 && faltantes_ == 0) {
            return true;
        }
        else {
            return false;
        }

        
    }
}


function actualizaTicketPedidoEspecial() {

    // contabilizamos todos los productos para consultar que precio le corresponde a cada uno
    var productos = [];
    var tblVtas = document.getElementById('tblConfirmarProductos');
    var rCount = tblVtas.rows.length;
    var idPedidoEspecialMayoreo_ = parseInt(0);

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var row_ = {
                idProducto: parseInt(tblVtas.rows[i].cells[1].innerHTML),
                cantidad: parseFloat(tblVtas.rows[i].cells[10].children[0].value),  // cantidad aceptada
                min: 1,
                max: 11,
                maxCantidad: 0,
                precioIndividual: 0,
                precioVenta: 0,
                descuento: 0,
                totalPorIdProductos: 0
            };
            productos.push(row_);
            idPedidoEspecialMayoreo_ = parseInt(tblVtas.rows[i].cells[15].innerHTML);
        }

    }
    
    var cantidadTotalPorProducto = [];
    var cantidadDeProductos = parseFloat(0);

    // actualizamos el contador del max_cantidad para el caso de infinito
    for (var m = 0; m < productos.length; m++) {
        var max_precio = parseFloat(0);

        /////////////////////////////////////////////// cantidadTotalPorProducto
        if (typeof cantidadTotalPorProducto !== 'undefined' && cantidadTotalPorProducto.length > 0) {

            if (cantidadTotalPorProducto.some(e => e.idProducto === productos[m].idProducto)) {
                cantidadTotalPorProducto.find(x => x.idProducto === productos[m].idProducto).cantidad += productos[m].cantidad;
            }
            else {
                var row_ = {
                    idProducto: parseInt(productos[m].idProducto),
                    cantidad: parseFloat(productos[m].cantidad),
                    precioRango: parseFloat(0)
                };
                cantidadTotalPorProducto.push(row_);
            }
        }
        else {
            var row_ = {
                idProducto: parseInt(productos[m].idProducto),
                cantidad: parseFloat(productos[m].cantidad),
                precioRango: parseFloat(0)
            };
            cantidadTotalPorProducto.push(row_);
        }
        ////////////////////////////////////////////////

        cantidadDeProductos += parseFloat(productos[m].cantidad);

        for (var n = 0; n < arrayPreciosRangos.length; n++) {
            var max_actual = parseFloat(arrayPreciosRangos[n]['max']);
            if (arrayPreciosRangos[n]['idProducto'] == productos[m].idProducto) {
                if (max_actual > max_precio) {
                    max_precio = max_actual;
                }
            }
        }
        productos[m].max = max_precio
    }


    //  si se ejecuta precio de mayoreo cuando el ticket tiene 6 o + articulos
    for (var o = 0; o < productos.length; o++) {
        if ( (cantidadDeProductos >= 6) || (parseInt(idPedidoEspecialMayoreo_) > 0) ) {
            productos[o].precioVenta = arrayProductos.find(x => x.idProducto === productos[o].idProducto).precioMenudeo;
        }
        else {
            productos[o].precioVenta = arrayProductos.find(x => x.idProducto === productos[o].idProducto).precioIndividual;
        }
        productos[o].precioIndividual = arrayProductos.find(x => x.idProducto === productos[o].idProducto).precioIndividual;
    }


    // actualizamos los que caigan en un rango
    for (var q = 0; q < cantidadTotalPorProducto.length; q++) {
        for (var r = 0; r < arrayPreciosRangos.length; r++) {

            if (cantidadTotalPorProducto[q].idProducto === arrayPreciosRangos[r].idProducto) {

                if (
                    (cantidadTotalPorProducto[q].cantidad >= arrayPreciosRangos[r].min) &&
                    (cantidadTotalPorProducto[q].cantidad <= arrayPreciosRangos[r].max)
                ) {
                    cantidadTotalPorProducto[q].precioRango = arrayPreciosRangos[r].costo;
                }

            }

        }

        // si hay algun percio (caso infinito)
        var algunPrecio = parseFloat(0);
        if (arrayPreciosRangos.some(x => x.idProducto === cantidadTotalPorProducto[q].idProducto)) {
            algunPrecio = arrayPreciosRangos.find(x => x.idProducto === cantidadTotalPorProducto[q].idProducto).max; // cantidad maxima de precio maximo
        }

        if (
            (algunPrecio > 0) &&  // si hay un precio maximo en rago de precios
            (cantidadTotalPorProducto[q].precioRango === 0) && // si no cayo en ningun rango
            (cantidadTotalPorProducto[q].cantidad > 6) && // ocupa ser mayor a 6 para que se evalue un rango
            (cantidadTotalPorProducto[q].cantidad > algunPrecio)  // si la cantidad de productos es mayor del maximo en la tabla de rangos 
        ) {
            var max__ = productos.find(x => x.idProducto === cantidadTotalPorProducto[q].idProducto).max;
            var costo = arrayPreciosRangos.find(x => x.max === max__ && x.idProducto === cantidadTotalPorProducto[q].idProducto).costo;
            cantidadTotalPorProducto[q].precioRango = costo;
        }
    }

    // se asigna el precio de venta en caso q cayo en un rango
    for (var p = 0; p < cantidadTotalPorProducto.length; p++) {
        if (cantidadTotalPorProducto[p].precioRango > 0) {
            for (var s = 0; s < productos.length; s++) {
                if (cantidadTotalPorProducto[p].idProducto === productos[s].idProducto) {
                    productos[s].precioVenta = cantidadTotalPorProducto[p].precioRango;
                }
            }
        }
    }

    
    // actualizamos el ticket
    for (var j = 0; j < productos.length; j++) {

        var tblVtas = document.getElementById('tblConfirmarProductos');
        var rCount = tblVtas.rows.length;

        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {

                var cantidad = parseFloat(tblVtas.rows[i].cells[10].children[0].value);

                if ((parseInt(tblVtas.rows[i].cells[1].innerHTML)) === (parseInt(productos[j].idProducto))) {
                    tblVtas.rows[i].cells[4].innerHTML = "$" + ((parseFloat(productos[j].precioIndividual).toFixed(2)) * ((cantidad).toFixed(2))).toFixed(2);   // monto
                    //tblVtas.rows[i].cells[5].innerHTML = "$" + (parseFloat(productos[j].precioVenta) * cantidad).toFixed(2);   //total
                    tblVtas.rows[i].cells[5].innerHTML = "$" + (parseFloat(productos[j].precioIndividual - productos[j].precioVenta) * cantidad).toFixed(2);  //descuento
                }
            }
        }
    }

    actualizarSubTotal();

}


function actualizarSubTotal() {

    var total = parseFloat(0);
    var subTotal = parseFloat(0);
    var descuento = parseFloat(0);
    var tblProductos = document.getElementById('tblConfirmarProductos');
    var rCount = tblProductos.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            total += parseFloat(tblProductos.rows[i].cells[4].innerHTML.replace('$', '')); //(parseFloat(tblProductos.rows[i].cells[4].innerHTML.replace('$', ''))) * (parseFloat(tblProductos.rows[i].cells[10].children[0].value));
            descuento += parseFloat(tblProductos.rows[i].cells[5].innerHTML.replace('$', ''));
        }
    }

    subTotal = total - descuento;

    $(".divSubTotal").html("$" + parseFloat(subTotal).toFixed(2));

    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
    document.getElementById("previoDescuentoMenudeo").innerHTML = "<h4>$" + parseFloat(descuento).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(subTotal).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(subTotal).toFixed(2) + "</h4>";

}



$("#efectivo").on("keyup", function (event) {

    if (event.keyCode === 13) {
        event.preventDefault();
        $("#btnEntregarPedidoEspecial").addClass('btn-progress disabled');
        document.getElementById("btnEntregarPedidoEspecial").click();
    }
    else {

        var cambio_ = parseFloat(0).toFixed(2);
        var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
        var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

        if (parseFloat(efectivo_) > parseFloat(total_)) {
            cambio_ = efectivo_ - total_;
            document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>";
            //document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>";
        }
        else {
            document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
            //document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
        }
    }

});


//$("#cantidadAbonada").on("keyup", function (event) {

//    if (event.keyCode === 13) {
//        event.preventDefault();
//        document.getElementById("btnEntregarPedidoEspecial").click();
//    }

//});

function initInputsTabla() {

    $('#tblConfirmarProductos input.productos_').on('change', function () {

        var thisInput = $(this);
        var mensaje = "Debe escribir la cantidad de productos.";
        var cell = $(this).closest('td');
        var row = cell.closest('tr');
        var rowIndex = row[0].rowIndex;
        var tblProductos = document.getElementById('tblConfirmarProductos');
        var idProducto = parseInt(tblProductos.rows[rowIndex].cells[1].innerHTML);
        var productosSolicitados = parseInt(tblProductos.rows[rowIndex].cells[6].innerHTML);


        if ((parseFloat(thisInput.val())) > (parseFloat(productosSolicitados))) {
            return;
        }

        actualizaTicketPedidoEspecial();

    });
}




function EliminarProductos(id) {

    var tblConfirmarProductos = document.getElementById('tblConfirmarProductos');
    tblConfirmarProductos.rows[id].cells[10].children[0].value = 0;
    actualizaTicketPedidoEspecial();
}



function ObtenerPrecios_(idProducto) {
    var result = [];
    $.ajax({
        url: rootUrl("/Productos/ObtenerPrecios"),
        data: { idProducto: idProducto },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes")
        },
        success: function (data) {
            result = data.Modelo;
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
    return result;
}



function InitarrayProductos() {
    MuestraToast('info', "Actualizando productos del almacen");
    $("#listProductos").val('');
    var result = '';
    $.ajax({
        url: rootUrl("/Productos/ObtenerTodosLosProductos"),
        data: { idProducto: 0, idUsuario: 0, activo: true },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            result = data;
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

    arrayProductos = [];
    arrayProductos = result.Modelo;

}

function ValidarPedidoEnRuta() {

    var esPedidoEnRuta = $('#esPedidoEnRuta').val();

    if (esPedidoEnRuta == true || esPedidoEnRuta == 'True') {
        
        var idUsuarioRuteoConsulta = $('#idUsuarioRuteoConsulta').val();

        document.getElementById("chkRuteo").checked = true;
        document.getElementById("chkCliente").checked = false;
        document.getElementById("chkTaxi").checked = false;

        document.getElementById("chkRuteo").disabled = true;
        document.getElementById("chkCliente").disabled = true;
        document.getElementById("chkTaxi").disabled = true;

        document.getElementById("numeroUnidadTaxi").value = "";
        document.getElementById("numeroUnidadTaxi").disabled = true;

        document.getElementById("idUsuarioRuteo").disabled = false;
        $('#idUsuarioRuteo').val(idUsuarioRuteoConsulta).trigger('change');
        document.getElementById("idUsuarioRuteo").disabled = true;

        document.getElementById("chkLiquidado").checked = true;
        document.getElementById("chkCredito").checked = false;
        document.getElementById("chkCreditoConAbono").checked = false;

        document.getElementById("chkLiquidado").disabled = true;
        document.getElementById("chkCredito").disabled = true;
        document.getElementById("chkCreditoConAbono").disabled = true;



    }

}


$(document).ready(function () {

    InitSelect2();
    initInputsTabla();
    arrayPreciosRangos = ObtenerPrecios_(0);
    InitarrayProductos();
    actualizaTicketPedidoEspecial();
    RevisarDescuentoCliente();
    ValidarClienteParaFacturacion(true);

    //$('#idCliente').val("0").trigger('change');
    $('#formaPago').val("1").trigger('change');
    

    //document.getElementById("idCliente").disabled = true;
    //document.getElementById("idUsuarioRuteo").disabled = true;
    document.getElementById("idUsuarioTaxi").disabled = true;
    document.getElementById("numeroUnidadTaxi").disabled = true;
    //document.getElementById("montoTotal").disabled = true;
    //document.getElementById("cantidadAbonada").disabled = true;

    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoDescuentoMenudeo").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoComisionBancaria").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    $('#efectivo').val('');
    document.getElementById("divUsoCFDI").style.display = 'none';
    $('#usoCFDI').val("3").trigger('change');
    //console.log("clientes", listClientes)

    
    //if ($("#cajaAbierta").val() == "False") {
    //    AbrirModalIngresoEfectivo(1);
    //}
    $('#usoCFDI').val("3").trigger('change');

});


//ingresos de efectivo
//Ingreso Efectivo



