

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



$('#btnGuardarPedidoEspecial').click(function (e) {

    document.getElementById("chkCliente").checked = false;
    document.getElementById("chkRuteo").checked = false;
    document.getElementById("chkTaxi").checked = false;
    document.getElementById("chkLiquidado").checked = false;
    document.getElementById("chkCredito").checked = false;
    document.getElementById("chkCreditoConAbono").checked = false;
    $('#idUsuarioRuteo').val("0").trigger('change');
    $('#idUsuarioTaxi').val("0").trigger('change');

    if (validarProductosAceptados()) {
        $('#ModalEntregarPedidoEspecial').modal({ backdrop: 'static', keyboard: false, show: true });
    }

});



$('#btnEntregarPedidoEspecial').click(function (e) {

    var idUsuarioRuteo = $('#idUsuarioRuteo').val();
    var idUsuarioTaxi = $('#idUsuarioTaxi').val();
    var numeroUnidadTaxi = "0";
    var idUsuarioEntrega = parseInt(0);
    var idPedidoEspecial = parseInt(0);
    var idEstatusPedidoEspecial = parseInt(0);
    var idEstatusCuentaPorCobrar = parseInt(0);    
    var montoTotal = parseFloat(0.0);
    var montoTotalcantidadAbonada = parseFloat(0.0);
    var productos = [];

    // validaciones
    if  (
            !$("#chkCliente").is(":checked") &&
            !$("#chkRuteo").is(":checked") &&
            !$("#chkTaxi").is(":checked") 
        )
    {
        MuestraToast('warning', "Debe elegir a quien va a entregar el Pedido Especial");
        return;
    }

    if (
        !$("#chkLiquidado").is(":checked") &&
        !$("#chkCredito").is(":checked") &&
        !$("#chkCreditoConAbono").is(":checked")
    ) {
        MuestraToast('warning', "Debe elegir el tipo de pago");
        return;
    }

    // 4	Entregado y pagado
    // 6	Pagado
    if ($("#chkLiquidado").is(":checked")) {
        if  ( $("#chkCliente").is(":checked") ) {
            idEstatusPedidoEspecial = parseInt(6);
        }
        else {
            idEstatusPedidoEspecial = parseInt(4);
        }
    }
    // 5	Entregado a repartidor sin ser pagado
    // 7	Entregado a crédito
    else{
        if ( $("#chkCliente").is(":checked") ) {
            idEstatusPedidoEspecial = parseInt(7);
        }
        else{
            idEstatusPedidoEspecial = parseInt(5);
        }
    }


    // entregado a encargado de ruteo
    if ($("#chkRuteo").is(":checked")  )
    {
        if (isNaN(parseInt(idUsuarioRuteo))) {
            MuestraToast('warning', "Debe elegir el usuario Encargado de Ruteo que recibe el Pedido Especial");
            return;
        }
        numeroUnidadTaxi = "0";
        idUsuarioEntrega = idUsuarioRuteo;
    }


    // entregado a taxi
    if ($("#chkTaxi").is(":checked")) {

        if (isNaN(parseInt(idUsuarioTaxi))) {
            MuestraToast('warning', "Debe elegir el usuario de Taxi que recibe el Pedido Especial");
            return;
        }

        if ( ($('#numeroUnidadTaxi').val() == "") )
        {
            MuestraToast('warning', "Debe elegir el numero de la unidad de Taxi que recibe el Pedido Especial");
            return;
        }

        idUsuarioEntrega = idUsuarioTaxi;
        numeroUnidadTaxi = $('#numeroUnidadTaxi').val();
    }



    // si es liquidado en su totalidad 
    if ($("#chkLiquidado").is(":checked")) {

        if (($('#montoTotal').val() == "")) {
            MuestraToast('warning', "Debe escribir el monto total de liquidación");
            return;
        }
        montoTotal = $('#montoTotal').val();
    }


    // si es a credito pero deja abono
    if ($("#chkCreditoConAbono").is(":checked")) {

        if (($('#cantidadAbonada').val() == "")) {
            MuestraToast('warning', "Debe escribir la cantidad que abono al Pedido Especial");
            return;
        }
        montoTotalcantidadAbonada = $('#cantidadAbonada').val();
    }



    // si todo bien
    var tblProductos = document.getElementById('tblConfirmarProductos');
    var rCount = tblProductos.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var row_ = {
                idProducto: parseInt(tblProductos.rows[i].cells[1].innerHTML),
                cantidadSolicitada: parseFloat(tblProductos.rows[i].cells[5].innerHTML),
                cantidadAtendida: parseFloat(tblProductos.rows[i].cells[7].innerHTML),
                cantidadRechazada: parseFloat(tblProductos.rows[i].cells[8].innerHTML),
                cantidadAceptada: parseFloat(tblProductos.rows[i].cells[9].children[0].value),
                observaciones: tblProductos.rows[i].cells[10].children[0].value,
                idPedidoEspecialDetalle: parseInt(tblProductos.rows[i].cells[12].innerHTML),
            };                
            productos.push(row_);
            idPedidoEspecial = parseInt(tblProductos.rows[i].cells[11].innerHTML);
        }
    }

    dataToPost = JSON.stringify({ productos: productos, idPedidoEspecial: idPedidoEspecial, idEstatusPedidoEspecial : idEstatusPedidoEspecial, idUsuarioEntrega: idUsuarioEntrega, numeroUnidadTaxi: numeroUnidadTaxi, idEstatusCuentaPorCobrar: idEstatusCuentaPorCobrar, montoTotal: montoTotal, montoTotalcantidadAbonada: montoTotalcantidadAbonada });
    console.log(dataToPost);
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
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            

            if (data.Estatus == 200) {
            
                window.location.href = rootUrl("/PedidosEspecialesV2/EntregarPedido");

            }
            $('#ModalEntregarPedidoEspecial').modal('hide');

        },
        error: function (xhr, status) {
            OcultarLoader();
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            console.log('Hubo un problema al guardar la confirmacion de productos, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

});



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
    }

    if (chk == 'chkRuteo') {
        document.getElementById("chkCliente").checked = false;
        document.getElementById("chkTaxi").checked = false;
    }

    if (chk == 'chkTaxi') {
        document.getElementById("chkCliente").checked = false;
        document.getElementById("chkRuteo").checked = false;
    }

}



function chkChangeTipoPago(chk) {

    if (chk == 'chkLiquidado') {
        document.getElementById("chkCredito").checked = false;
        document.getElementById("chkCreditoConAbono").checked = false;
    }

    if (chk == 'chkCredito') {
        document.getElementById("chkLiquidado").checked = false;
        document.getElementById("chkCreditoConAbono").checked = false;
    }

    if (chk == 'chkCreditoConAbono') {
        document.getElementById("chkLiquidado").checked = false;
        document.getElementById("chkCredito").checked = false;
    }

}



function validarProductosAceptados() {

    var faltantes = parseInt(0);
    var tblProductos = document.getElementById('tblConfirmarProductos');
    var rCount = tblProductos.rows.length;
    
    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            if  (
                    ((parseFloat(tblProductos.rows[i].cells[5].innerHTML)) !== (parseFloat(tblProductos.rows[i].cells[9].children[0].value))) &&
                    (String(tblProductos.rows[i].cells[10].children[0].value) == "" )
                ) {
                if (faltantes == 0) {
                    MuestraToast('warning', "Tiene que capturar las observaciones si no esta aceptando todos los productos."); 
                }
                faltantes += 1;
            }
        }
    }
    
    return !(faltantes > 0);
}


function actualizarSubTotal() {

    var subTotal = parseFloat(0);

    $('#tblConfirmarProductos tbody tr').each(function (index, fila) {
        subTotal += parseFloat(fila.children[4].innerHTML.replace('$', ''));
    });

    $(".divSubTotal").html("$" + parseFloat(subTotal).toFixed(2));
}



function initInputsTabla() {

    $('#tblConfirmarProductos input.productos_').on('change', function () {

        var thisInput = $(this);
        var mensaje = "Debe escribir la cantidad de productos.";
        var cell = $(this).closest('td');
        var row = cell.closest('tr');
        var rowIndex = row[0].rowIndex;
        var tblProductos = document.getElementById('tblConfirmarProductos');
        var idProducto = parseInt(tblProductos.rows[rowIndex].cells[1].innerHTML);
        var productosSolicitados = parseInt(tblProductos.rows[rowIndex].cells[5].innerHTML);
        
        if ((thisInput.val() == "") || (thisInput.val() == "0")) {
            MuestraToast('warning', mensaje);
            document.execCommand('undo');
        }

        if ((parseFloat(thisInput.val())) > (parseFloat(productosSolicitados))) {
            MuestraToast('warning', "No puede aceptar mas productos de los solicitados.");
            document.execCommand('undo');
            return;
        }

    });
}




function EliminarProductos(id) {

    var tblConfirmarProductos = document.getElementById('tblConfirmarProductos');
    tblConfirmarProductos.rows[id].cells[9].children[0].value = 0;

}

$(document).ready(function () {
    
    InitSelect2();
    initInputsTabla();
    actualizarSubTotal();
});



