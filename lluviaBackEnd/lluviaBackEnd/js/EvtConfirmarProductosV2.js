var arrayPreciosRangos = [];
var arrayProductos = [];


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
    document.getElementById("chkFacturarPedido").checked = false;

    $('#idUsuarioRuteo').val("").trigger('change');
    $('#idUsuarioTaxi').val("").trigger('change');
    $('#idCliente').val("0").trigger('change');
    $('#formaPago').val("1").trigger('change');
    $('#usoCFDI').val("1").trigger('change');

    document.getElementById("idCliente").disabled = true;
    document.getElementById("idUsuarioRuteo").disabled = true;
    document.getElementById("idUsuarioTaxi").disabled = true;
    document.getElementById("numeroUnidadTaxi").value = "";
    document.getElementById("numeroUnidadTaxi").disabled = true;

    document.getElementById("montoTotal").value = "";
    document.getElementById("montoTotal").disabled = true;
    document.getElementById("cantidadAbonada").value = "";
    document.getElementById("cantidadAbonada").disabled = true;

    actualizarSubTotal();

    if (validarProductosAceptados()) {
        $('#ModalEntregarPedidoEspecial').modal({ backdrop: 'static', keyboard: false, show: true });
    }

});

$("#formaPago").on("change", function (value) {
    this.value == 1 ? $('#dvEfectivo').css('display', '') : $('#dvEfectivo').css('display', 'none');
    calculaTotales('true');
});

//$("#idUsuarioRuteo").on("change", function (value) {
//    console.log(this.value);
//});

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



$("#idCliente").on("change", function () {

    var idCliente = parseFloat($('#idCliente').val());
    var data = ObtenerCliente(idCliente);
    var descuento = parseFloat(0.0);

    if (idCliente != 1) {
        descuento = parseFloat(data.Modelo.tipoCliente.descuento).toFixed(2);;
    }

    $("#txtDescuentoCliente").val(descuento);
    calculaTotales('true');

});


function ObtenerCliente(idCliente) {
    var result = "";
    $.ajax({
        url: rootUrl("/Clientes/ObtenerCliente"),
        data: { idCliente: idCliente },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            //console.log("Antes_")
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
    return result;
}




$('#chkFacturarPedido').click(function () {

    var idCliente = $('#idCliente').val();
    var esDevolucion = $('#esDevolucion').val();
    var formaPago = $('#formaPago').val();
    var iva = parseFloat(0).toFixed(2);
    var porcentajeIva = parseFloat(0.16).toFixed(2);

    if (idCliente == 1) {
        MuestraToast('warning', "Debe seleccionar un cliente diferente a " + $("#idCliente").find("option:selected").text());
        document.getElementById("chkFacturarPedido").checked = false;
        return
    }

    if ($('#chkFacturarPedido').is(':checked')) {
        cliente = listClientes.find(x => x.idCliente == idCliente)
        console.log(cliente);
        if (cliente) {
            if (!validarEmail(cliente.correo)) {
                MuestraToast('warning', "No es posible facturar a un cliente sin correo electrónico vàlido");
                document.getElementById("chkFacturarPedido").checked = false;
                return false;
            }

            if (!validarRFC(cliente.rfc)) {
                MuestraToast('warning', "No es posible facturar a un cliente sin RFC vàlido");
                document.getElementById("chkFacturarPedido").checked = false;
                return false;
            }
        } else {
            MuestraToast('warning', "No es posible facturar a  este cliente por favor comuníquese con el administrador web");
            document.getElementById("chkFacturarPedido").checked = false;
            return false;
        }
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

    var idUsuarioRuteo = $('#idUsuarioRuteo').val();
    var idUsuarioTaxi = $('#idUsuarioTaxi').val();
    var idFactUsoCFDI = parseInt(0);
    var numeroUnidadTaxi = "0";
    var idUsuarioEntrega = parseInt(0);
    var idPedidoEspecial = parseInt(0);
    var idEstatusPedidoEspecial = parseInt(0);
    var idEstatusCuentaPorCobrar = parseInt(0);    
    var montoTotal = parseFloat(0.0);
    var montoTotalcantidadAbonada = parseFloat(0.0);
    var productos = [];
    var aCredito = parseInt(0);
    var formaPago = $('#formaPago').val();
    var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
    var total_ = parseFloat($("#previoFinal").html().replace('<h4>$', '').replace('</h4>', ''));
    var aplicaIVA = parseInt(0);
    var cantidadAbonada_ = parseFloat($('#cantidadAbonada').val()).toFixed(2);
    var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);


    $("#btnEntregarPedidoEspecial").addClass('btn-progress disabled');

    if (($('#efectivo').val() == "") && (parseInt(formaPago) == parseInt(1))) {
        MuestraToast('warning', "Debe escribir con cuanto efectivo le estan pagando.");
        $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
        return;
    }

    if (parseFloat(efectivo_) < parseFloat(total_)) {
        MuestraToast('warning', "El efectivo no alcanza a cubrir el costo total de la venta: " + total_.toString());
        $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
        return;
    }

    if ($("#chkFacturarPedido").is(":checked")) {
        aplicaIVA = parseInt(1);
        idFactUsoCFDI = $('#usoCFDI').val();    
    }

    if ((parseInt(formaPago) !== parseInt(1))) // si no es efectivo
    {
        efectivo_ = total_;
    }

    if  (
            !$("#chkCliente").is(":checked") &&
            !$("#chkRuteo").is(":checked") &&
            !$("#chkTaxi").is(":checked") 
        )
    {
        MuestraToast('warning', "Debe elegir a quien va a entregar el Pedido Especial");
        $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
        return;
    }

    if (
        !$("#chkLiquidado").is(":checked") &&
        !$("#chkCredito").is(":checked") &&
        !$("#chkCreditoConAbono").is(":checked")
    ) {
        MuestraToast('warning', "Debe elegir el tipo de pago");
        $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
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
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            return;
        }

        if ( ($('#numeroUnidadTaxi').val() == "") )
        {
            MuestraToast('warning', "Debe elegir el numero de la unidad de Taxi que recibe el Pedido Especial");
            $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            return;
        }

        idUsuarioEntrega = idUsuarioTaxi;
        numeroUnidadTaxi = $('#numeroUnidadTaxi').val();
    }



    // si es liquidado en su totalidad 
    if ($("#chkLiquidado").is(":checked")) {

        if (($('#montoTotal').val() == "")) {
            MuestraToast('warning', "Debe escribir el monto total de liquidación");
            ("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            return;
        }
        montoTotal = $('#montoTotal').val();
    }


    // si es a credito pero deja abono
    if ($("#chkCreditoConAbono").is(":checked")) {

        if (($('#cantidadAbonada').val() == "")) {
            MuestraToast('warning', "Debe escribir la cantidad que abono al Pedido Especial");
        $   ("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            return;
        }
        montoTotalcantidadAbonada = $('#cantidadAbonada').val();
    }


    // si es a credito -> afectar estructuras de cuentas por cobrar
    if  (
            ($("#chkCredito").is(":checked")) ||
            ($("#chkCreditoConAbono").is(":checked")) 
        )
    {
        aCredito = parseInt(1);
    }


    //validaciones
    if (parseFloat(cantidadAbonada_) > parseFloat(total_)) {
        MuestraToast('warning', "No puede abonar mas del total del pedido especial .");
        $("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
        return;
    }
    

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
        numeroUnidadTaxi: numeroUnidadTaxi, idEstatusCuentaPorCobrar: idEstatusCuentaPorCobrar, montoTotal: montoTotal, montoTotalcantidadAbonada: montoTotalcantidadAbonada,
        aCredito: aCredito, idTipoPago: formaPago, aplicaIVA: aplicaIVA, idFactUsoCFDI: idFactUsoCFDI
    });
    
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

        $('#idUsuarioRuteo').val("").trigger('change');
        document.getElementById("idUsuarioRuteo").disabled = true;

        $('#idUsuarioTaxi').val("").trigger('change');
        document.getElementById("idUsuarioTaxi").disabled = true;

        document.getElementById("idCliente").disabled = false;
        document.getElementById("numeroUnidadTaxi").value = "";
        document.getElementById("numeroUnidadTaxi").disabled = true;

    }

    if (chk == 'chkRuteo') {

        document.getElementById("chkCliente").checked = false;
        document.getElementById("chkTaxi").checked = false;

        $('#idUsuarioTaxi').val("").trigger('change');
        document.getElementById("idUsuarioTaxi").disabled = true;

        $('#idCliente').val("0").trigger('change');
        document.getElementById("idCliente").disabled = true;

        document.getElementById("idUsuarioRuteo").disabled = false;
        document.getElementById("numeroUnidadTaxi").value = "";
        document.getElementById("numeroUnidadTaxi").disabled = true;

    }

    if (chk == 'chkTaxi') {

        document.getElementById("chkCliente").checked = false;
        document.getElementById("chkRuteo").checked = false;

        $('#idUsuarioRuteo').val("").trigger('change');
        document.getElementById("idUsuarioRuteo").disabled = true;

        $('#idCliente').val("0").trigger('change');
        document.getElementById("idCliente").disabled = true;

        document.getElementById("idUsuarioTaxi").disabled = false;
        document.getElementById("numeroUnidadTaxi").value = "";
        document.getElementById("numeroUnidadTaxi").disabled = false;

    }

}



function chkChangeTipoPago(chk) {

    if (chk == 'chkLiquidado') {

        document.getElementById("chkCredito").checked = false;
        document.getElementById("chkCreditoConAbono").checked = false;

        document.getElementById("montoTotal").disabled = false;
        document.getElementById("cantidadAbonada").value = "";
        document.getElementById("cantidadAbonada").disabled = true;
    }

    if (chk == 'chkCredito') {

        document.getElementById("chkLiquidado").checked = false;
        document.getElementById("chkCreditoConAbono").checked = false;

        document.getElementById("montoTotal").value = "";
        document.getElementById("montoTotal").disabled = true;
        document.getElementById("cantidadAbonada").value = "";
        document.getElementById("cantidadAbonada").disabled = true;
    }

    if (chk == 'chkCreditoConAbono') {

        document.getElementById("chkLiquidado").checked = false;
        document.getElementById("chkCredito").checked = false;

        document.getElementById("montoTotal").value = "";
        document.getElementById("montoTotal").disabled = true;
        document.getElementById("cantidadAbonada").disabled = false;
    }

}



function validarProductosAceptados() {

    var faltantes = parseInt(0);
    var cantidad = parseFloat(0);
    var tblProductos = document.getElementById('tblConfirmarProductos');
    var rCount = tblProductos.rows.length;
    
    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            if  (
                    ((parseFloat(tblProductos.rows[i].cells[6].innerHTML)) !== (parseFloat(tblProductos.rows[i].cells[10].children[0].value))) &&
                    (String(tblProductos.rows[i].cells[11].children[0].value) == "" )
                ) {
                if (faltantes == 0) {
                    MuestraToast('warning', "Tiene que capturar las observaciones si no esta aceptando todos los productos."); 
                }
                faltantes += 1;
            }
            cantidad += parseFloat(tblProductos.rows[i].cells[10].children[0].value);
        }
    }

    if (cantidad <= 0.0) {
        MuestraToast('warning', "Para cancelar todos los productos de todo el pedido debe hacerlo desde el menu de Entregar Pedido.");
        return false;
    }
    else {
        return !(faltantes > 0);
    }
    
}


function actualizaTicketPedidoEspecial() {

    // contabilizamos todos los productos para consultar que precio le corresponde a cada uno
    var productos = [];
    var tblVtas = document.getElementById('tblConfirmarProductos');
    var rCount = tblVtas.rows.length;

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
        if (cantidadDeProductos >= 6) {
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

                if ((parseInt(tblVtas.rows[i].cells[1].innerHTML)) === (parseInt(productos[j].idProducto)))
                {
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


$("#cantidadAbonada").on("keyup", function (event) {

    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("btnEntregarPedidoEspecial").click();
    }

});

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
        url: rootUrl("/Productos/ObtenerProductosPorUsuario"),
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


$(document).ready(function () {
    
    InitSelect2();
    initInputsTabla();
    arrayPreciosRangos = ObtenerPrecios_(0);
    InitarrayProductos();
    actualizaTicketPedidoEspecial();

    $('#idCliente').val("0").trigger('change');
    $('#formaPago').val("1").trigger('change');
    $('#usoCFDI').val("1").trigger('change');

    document.getElementById("idCliente").disabled = true;
    document.getElementById("idUsuarioRuteo").disabled = true;
    document.getElementById("idUsuarioTaxi").disabled = true;
    document.getElementById("numeroUnidadTaxi").disabled = true;
    document.getElementById("montoTotal").disabled = true;
    document.getElementById("cantidadAbonada").disabled = true;

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


});



