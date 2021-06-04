var table;
var iframe;
var tablaPedidosEspeciales;
var arrayPreciosRangos = [];
var arrayProductos = [];

//busqueda
function onBeginSubmitPedidosEspeciales() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitPedidosEspeciales() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultPedidosEspeciales(data) {
    console.log("onSuccessResultPedidosEspeciales", JSON.stringify(data));
    tablaPedidosEspeciales.destroy();
    $('#rowPedidosEspeciales').html(data);
    InitDataTablePedidosEspeciales();
}
function onFailureResultPedidosEspeciales() {
    console.log("onFailureResult___");
}


function preguntaAltaPrecios() {

    swal({
        title: 'Mensaje',
        text: 'Este producto no tiene un precio configurado, ¿Desea Configurarlo?',
        icon: 'info',
        buttons: ["No", "Sí"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                console.log(willDelete);
                location.href = rootUrl("/Productos/Productos");
            } else {
                console.log("cancelar");
            }
        });
}

//function _facturaVenta(idPedidoEspecial) {
//    console.log("facturaVenta_" + idPedidoEspecial);
//    $.ajax({
//        url: rootUrl("/Factura/GenerarFactura"),
//        data: { idPedidoEspecial: idPedidoEspecial },
//        method: 'post',
//        dataType: 'json',
//        async: true,
//        beforeSend: function (xhr) {
//            ShowLoader("Facturando Venta.");
//        },
//        success: function (data) {
//            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
//            OcultarLoader();
//        },
//        error: function (xhr, status) {
//            console.log('Disculpe, existió un problema');
//            console.log(xhr);
//            console.log(status);
//            OcultarLoader();
//        }
//    });
//}
//function facturaVenta(idPedidoEspecial) {
//    console.log("facturaVenta_" + idPedidoEspecial);
//    $.ajax({
//        url: pathDominio + "api/WsFactura/GenerarFactura",
//        data: { idPedidoEspecial: idPedidoEspecial, idUsuario: idUsuarioGlobal },
//        method: 'post',
//        dataType: 'json',
//        async: true,
//        beforeSend: function (xhr) {
//            ShowLoader("Facturando Venta.");
//        },
//        success: function (data) {
//            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
//            OcultarLoader();
//        },
//        error: function (xhr, status) {
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

function eliminaFila(index_) {
    document.getElementById("tablaRepPedidosEspeciales").deleteRow(index_);
    actualizaTicketPedidoEspecial();
}

$('#limpiar').click(function (e) {
    limpiarTicket();
});


$('#cancelar').click(function (e) {
    limpiaModalPrevio();
});

function limpiarTicket() {

    var max_id = parseFloat(0);

    $('#tablaRepPedidosEspeciales tbody tr').each(function (index, fila) {
        var maximo_actual = parseFloat(fila.children[0].innerHTML);
        if (maximo_actual > max_id) {
            max_id = maximo_actual;
        }
    });

    var i;
    for (i = max_id; i >= 1; i--) {
        document.getElementById("tablaRepPedidosEspeciales").deleteRow(i);
    }

    actualizaTicketPedidoEspecial();
    limpiaModalPrevio();
    $('#cantidad').val('');
    $('#idProducto').val("0").trigger('change');
    $('#idPedidoEspecial').val(0);
    $('#vaConDescuento').val(0);

}

function limpiaModalPrevio() {
    //var esAgregarProductos = $('#esAgregarProductos').val();

    //var row_ = "<address>" +
    //    "    <strong></strong><br>" +
    //    "    <br>" +
    //    "    <br>" +
    //    "    <br>" +
    //    "    <br>" +
    //    "    <br>" +
    //    "</address>";

    //document.getElementById("nombreCliente").innerHTML = row_;

    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoDescuentoMenudeo").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    //document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    //document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    //if ((idPedidoEspecial == 0)) {
        $('#descripcionPedidoInterno').val('');
    //}
    //$('#formaPago').val("1").trigger('change');
    //$('#usoCFDI').val("3").trigger('change');

    //if ((esAgregarProductos == "True") || (esAgregarProductos == "true")) {
    //if ((idPedidoEspecial > 0) ) {
    //    $('#idCliente').val($('#idClienteDevolucion').val()).trigger('change');
    //}
    //else {
    //    $('#idCliente').val("1").trigger('change');
    //}

}



$('#previoPedidoEspecial').click(function (e) {

    //var esDevolucion = $('#esDevolucion').val();

    //if ((esDevolucion == "true") || (esDevolucion == "True")) {
    //    // validamos que al menos exista devolucion de un item
    //    var tblVtas = document.getElementById('tablaRepPedidosEspeciales');
    //    var rCount = tblVtas.rows.length;
    //    var productosOriginales = parseInt(0);
    //    var productosDevueltos = parseInt(0);

    //    if (rCount >= 2) {
    //        for (var i = 1; i < rCount; i++) {
    //            productosDevueltos += parseInt(tblVtas.rows[i].cells[7].children[0].value);
    //            productosOriginales += parseInt(tblVtas.rows[i].cells[4].children[0].value);
    //        }
    //    }

    //    if (productosDevueltos <= 0) {
    //        MuestraToast('warning', "Debe seleccionar al menos un producto para devolver.");
    //        return;
    //    }

    //    if (productosDevueltos >= productosOriginales) {
    //        MuestraToast('warning', "Para devolver todos los productos cancele la venta desde el menu de Editar PedidosEspeciales");
    //        return;
    //    }


    //    $('#motivoDevolucion').val('');
    //    $('#ModalDevolucion').modal({ backdrop: 'static', keyboard: false, show: true });
    //}
    //else {
        abrirModalPrevioPedidoEspecial();
    //}
});

function abrirModalPrevioPedidoEspecial() {

    limpiaModalPrevio();

    var total = parseFloat(0);
    var descuento = parseFloat(0);

    $('#tablaRepPedidosEspeciales tbody tr').each(function (index, fila) {

        //if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
            total += parseFloat(fila.children[5].innerHTML.replace('$', ''));
            descuento += parseFloat(fila.children[6].innerHTML.replace('$', ''));
        //}

    });

    if (total > 0) {
        document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(total + descuento).toFixed(2) + "</h4>";
        document.getElementById("previoDescuentoMenudeo").innerHTML = "<h4>$" + parseFloat(descuento).toFixed(2) + "</h4>";
        document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(total + descuento - descuento).toFixed(2) + "</h4>";
        document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(total + descuento - descuento).toFixed(2) + "</h4>";
        $('#ModalPrevioPedidoEspecial').modal({ backdrop: 'static', keyboard: false, show: true });
    }
    else {
        MuestraToast('warning', "Debe tener productos agregados para continuar con el alta del pedido especial.");
    }

}



//$('#btnAceptarDevolucion').click(function (e) {


//    if (($('#motivoDevolucion').val() == "")) {
//        MuestraToast('warning', "Debe seleccionar el motivo de la devolución");
//        return;
//    }

//    $('#ModalDevolucion').modal('hide');
//    //abrirModalPrevioPedidoEspecial();
//    document.getElementById("btnGuardarPedidoEspecial").click();

//});



$('#btnAgregarProducto').click(function (e) {

    if (($('#idProducto').val() == "") || ($('#idProducto').val() == null)) {
        MuestraToast('warning', "Debe seleccionar el producto que desea agregar.");
        return;
    }

    if ($('#cantidad').val() == "") {
        MuestraToast('warning', "Debe escribir la cantidad de productos que va a agregar.");
        return;
    }

    if ($('#idProducto').select2('data')[0].precioIndividual <= 0 && $('#idProducto').select2('data')[0].precioMenudeo <= 0) {
        preguntaAltaPrecios();
        return
    }

    //if ($('#idProducto').select2('data')[0].cantidad < parseInt($('#cantidad').val())) {
    //    MuestraToast('warning', "no existe suficiente producto en inventario");
    //    return;
    //}

    if ($('#idProducto').select2('data')[0].precioIndividual <= 0) {
        MuestraToast('warning', "Debe configurar el precio invidual del producto.");
        return;
    }

    if ($('#idProducto').select2('data')[0].precioMenudeo <= 0) {
        MuestraToast('warning', "Debe configurar el precio Mayoreo del producto.");
        return;
    }

    var cantidad = $('#cantidad').val();
    //var esAgregarProductos = $('#esAgregarProductos').val();
    var btnEliminaFila = "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
    var precio = parseFloat(0).toFixed(2);
    var descuento = parseFloat(0).toFixed(2);

    //if (esAgregarProductos == 'true') {
    //    btnEliminaFila = "";
    //}

    var existeProducto = false;

    var tblPedidos = document.getElementById('tablaRepPedidosEspeciales');
    var rCount = tblPedidos.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {           
            if ($('#idProducto').select2('data')[0].idProducto === parseInt(tblPedidos.rows[i].cells[1].innerHTML)) {
                var cantidad = parseInt(tblPedidos.rows[i].cells[4].children[0].value) + parseInt(cantidad);
                tblPedidos.rows[i].cells[4].children[0].value = cantidad;
                existeProducto = true;
            }
        }
    }

    if (!existeProducto) {

        // si todo bien    
        var row_ = "<tr>" +
            "  <td>1</td>" +
            "  <td> " + $('#idProducto').val() + "</td>" +
            "  <td> " + $("#idProducto").find("option:selected").text().substr(0, $("#idProducto").find("option:selected").text().indexOf('- (')) + "</td>" +
            "  <td class=\"text-center\">$" + precio + "</td>" +
            "  <td class=\"text-center\"><input type='text' onkeypress=\"return numerico(event)\" style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + cantidad + "\"></td>" +
            "  <td class=\"text-center\">$" + precio + "</td>" +
            "  <td class=\"text-center\">$" + descuento + "</td>" +
            "  <td class=\"text-center\">" +
            btnEliminaFila +
            "  <td style=\"display: none;\">0</td>" +
            "  </td>" +
            "</tr >";

        $("#tablaRepPedidosEspeciales tbody").append(row_);
        $('#cantidad').val('');

    }




    actualizaTicketPedidoEspecial();
    initInputsTabla();

});


function actualizaTicketPedidoEspecial() {

    // acttualizamos el id y la funcion de eliminar fila
    $('#tablaRepPedidosEspeciales tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;

        //if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
            fila.children[7].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        //}

    });

    // contabilizamos todos los productos para consultar que precio le corresponde a cada uno
    var productos = [];
    var tblVtas = document.getElementById('tablaRepPedidosEspeciales');
    var rCount = tblVtas.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var row_ = {
                idProducto: parseInt(tblVtas.rows[i].cells[1].innerHTML),
                cantidad: parseInt(tblVtas.rows[i].cells[4].children[0].value),
                min: 1,
                max: 5,
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
    var cantidadDeProductos = parseInt(0);
    //console.log(arrayPreciosRangos);

    // actualizamos el contador del max_cantidad para el caso de infinito
    for (var m = 0; m < productos.length; m++) {
        var max_precio = parseInt(0);

        /////////////////////////////////////////////// cantidadTotalPorProducto
        if (typeof cantidadTotalPorProducto !== 'undefined' && cantidadTotalPorProducto.length > 0) {
            
            if (cantidadTotalPorProducto.some(e => e.idProducto === productos[m].idProducto)) {
                cantidadTotalPorProducto.find(x => x.idProducto === productos[m].idProducto).cantidad += productos[m].cantidad;
            }
            else {
                var row_ = {
                    idProducto: parseInt(productos[m].idProducto),
                    cantidad: parseInt(productos[m].cantidad),
                    precioRango: parseFloat(0)
                };
                cantidadTotalPorProducto.push(row_);
            }
        }
        else {
            var row_ = {
                idProducto: parseInt(productos[m].idProducto),
                cantidad: parseInt(productos[m].cantidad),
                precioRango: parseFloat(0)
            };
            cantidadTotalPorProducto.push(row_);
        }
        ////////////////////////////////////////////////

        cantidadDeProductos += parseInt(productos[m].cantidad);

        for (var n = 0; n < arrayPreciosRangos.length; n++) {
            var max_actual = parseInt(arrayPreciosRangos[n]['max']);
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
            algunPrecio = arrayPreciosRangos.find(x => x.idProducto === cantidadTotalPorProducto[q].idProducto).max;
        }

        if ((algunPrecio > 0) && (cantidadTotalPorProducto[q].precioRango === 0) && (cantidadTotalPorProducto[q].cantidad > 6 )) {
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


    //console.log(cantidadTotalPorProducto);
    //console.log(productos);
    //console.log(arrayProductos);

    // actualizamos el ticket
    for (var j = 0; j < productos.length; j++) {

        var tblVtas = document.getElementById('tablaRepPedidosEspeciales');
        var rCount = tblVtas.rows.length;

        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {

                var cantidad = parseFloat(tblVtas.rows[i].cells[4].children[0].value);

                //if ((!tblVtas.rows[i].cells[7].getAttribute("class").includes('esDevolucion')) && (!tblVtas.rows[i].cells[7].getAttribute("class").includes('esAgregarProductos'))) {
                    //console.log(tblVtas.rows[i].cells[7].getAttribute("class"));

                    if ((parseInt(tblVtas.rows[i].cells[1].innerHTML)) === (parseInt(productos[j].idProducto))) {
                        tblVtas.rows[i].cells[3].innerHTML = "$" + parseFloat(productos[j].precioVenta).toFixed(2);   //precio
                        tblVtas.rows[i].cells[5].innerHTML = "$" + (parseFloat(productos[j].precioVenta) * cantidad).toFixed(2);   //total
                        tblVtas.rows[i].cells[6].innerHTML = "$" + (parseFloat(productos[j].precioIndividual - productos[j].precioVenta) * cantidad).toFixed(2);  //descuento
                    }
                //}
            }
        }
    }

    actualizarSubTotal();

    //return;

    //dataToPost = JSON.stringify({ precios: productos });

    //$.ajax({
    //    url: rootUrl("/PedidosEspeciales/ObtenerPreciosDeProductos"),
    //    data: dataToPost,
    //    method: 'POST',
    //    dataType: 'JSON',
    //    contentType: "application/json; charset=utf-8",
    //    async: false,
    //    beforeSend: function (xhr) {
    //        ShowLoader("Cargando...");
    //    },
    //    success: function (data) {
    //        OcultarLoader();

    //        if (data.Estatus == 200) {

    //            var j = 0;
    //            for (j = 0; j < data.Modelo.length; j++) {

    //                var tblVtas = document.getElementById('tablaRepPedidosEspeciales');
    //                var rCount = tblVtas.rows.length;

    //                if (rCount >= 2) {
    //                    for (var i = 1; i < rCount; i++) {

    //                        var cantidad = parseFloat(tblVtas.rows[i].cells[4].children[0].value);

    //                        if ((!tblVtas.rows[i].cells[7].getAttribute("class").includes('esDevolucion')) && (!tblVtas.rows[i].cells[7].getAttribute("class").includes('esAgregarProductos'))) {
    //                            //console.log(tblVtas.rows[i].cells[7].getAttribute("class"));

    //                            if ((parseInt(tblVtas.rows[i].cells[1].innerHTML)) == (parseInt(data.Modelo[j].idProducto))) {
    //                                tblVtas.rows[i].cells[3].innerHTML = "$" + parseFloat(data.Modelo[j].costo).toFixed(2);   //precio
    //                                tblVtas.rows[i].cells[5].innerHTML = "$" + (parseFloat(data.Modelo[j].costo) * cantidad).toFixed(2);   //total
    //                                tblVtas.rows[i].cells[6].innerHTML = "$" + (parseFloat(data.Modelo[j].descuento) * cantidad).toFixed(2);  //descuento
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    },
    //    error: function (xhr, status) {
    //        OcultarLoader();
    //        console.log('Hubo un problema al consultar los precios de los productos, contactese con el administrador del sistema');
    //        console.log(xhr);
    //        console.log(status);
    //    }
    //});

    //actualizarSubTotal();
}



//function maxed(a, b) {
//    return a > b;
//}



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
            //pintarPrecios(data.Modelo);
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


function initInputsTabla() {

    $('#tablaRepPedidosEspeciales input').on('change', function () {

        var thisInput = $(this);
        var mensaje = "Debe escribir la cantidad de productos.";

        if (thisInput.hasClass("esDevolucion")) {
            mensaje = "Debe escribir la cantidad de productos que va a devolver.";
        }

        if ((thisInput.val() == "") || (thisInput.val() == "0")) {
            MuestraToast('warning', mensaje);
            document.execCommand('undo');
        }

        if (thisInput.hasClass("esDevolucion")) {

            var cell = $(this).closest('td');
            var row = cell.closest('tr');
            var rowIndex = row[0].rowIndex;
            var tblVtas = document.getElementById('tablaRepPedidosEspeciales');

            if ((parseInt(thisInput.val())) > (parseInt(tblVtas.rows[rowIndex].cells[4].children[0].value))) {
                MuestraToast('warning', "No puede regresar mas de lo que compro.");
                document.execCommand('undo');
                return;
            }

            actualizarSubTotalDevoluciones();
        }
        else {
            actualizaTicketPedidoEspecial();
        }
    });
}


//function actualizaPreciosTabla(tabla) {

//    var tblVtas = document.getElementById(tabla);
//    var rCount = tblVtas.rows.length;

//    if (rCount >= 2) {
//        for (var i = 1; i < rCount; i++) {

//            var idProducto = parseFloat(tblVtas.rows[i].cells[1].innerHTML);
//            var cantidad = parseFloat(tblVtas.rows[i].cells[4].children[0].value);
//            var data = ObtenerProductoPorPrecio(idProducto, cantidad, $("#vaConDescuento").val());
//            var precio = parseFloat(data.Modelo[0].costo);

//            if (precio == 0) {
//                console.log("error_precio_" + precio);
//            }
//            else {
//                tblVtas.rows[i].cells[3].innerHTML = "$" + data.Modelo[0].costo;   //precio
//                tblVtas.rows[i].cells[5].innerHTML = "$" + parseFloat(data.Modelo[0].costo) * cantidad;   //total
//                tblVtas.rows[i].cells[6].innerHTML = "$" + parseFloat(data.Modelo[0].descuento);  //descuento
//            }
//        }
//    }
//}


function cuentaSubTotal() {
    //var result = parseFloat(0);
    var subTotal = parseFloat(0);
    $('#tablaRepPedidosEspeciales tbody tr').each(function (index, fila) {

        //if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
            subTotal += parseFloat(fila.children[5].innerHTML.replace('$', ''));
        //}

    });
    return subTotal;
}

function actualizarSubTotal() {

    var subTotal = parseFloat(0);
    //var descuento = parseFloat(0);
    var esDevolucion = $('#esDevolucion').val();
    subTotal = cuentaSubTotal();
    //$('#tablaRepPedidosEspeciales tbody tr').each(function (index, fila) {
    //    subTotal += parseFloat(fila.children[5].innerHTML.replace('$', ''));
    //    descuento += parseFloat(fila.children[6].innerHTML.replace('$', ''));
    //});

    if ((esDevolucion == "true") || (esDevolucion == "True")) {
        subTotal = 0;
    }

    document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(subTotal).toFixed(2) + "</h4>";
}

function actualizarSubTotalDevoluciones() {

    var tblVtas = document.getElementById('tablaRepPedidosEspeciales');
    var rCount = tblVtas.rows.length;
    var cantidadDevelta = parseInt(0);

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            cantidadDevelta += parseFloat(tblVtas.rows[i].cells[7].children[0].value) * parseFloat(tblVtas.rows[i].cells[3].innerHTML.replace('$', '')); //parseFloat(fila.children[5].innerHTML.replace('$', ''));
        }
    }

    document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(cantidadDevelta).toFixed(2) + "</h4>";
    document.getElementById("divTotalDevolver").innerHTML = "<h4>$" + parseFloat(cantidadDevelta).toFixed(2) + "</h4>";
}


function ObtenerProductoPorPrecio(idProducto, cantidad, vaConDescuento) {

    var result = '';
    $.ajax({
        url: rootUrl("/PedidosEspeciales/ObtenerProductoPorPrecio"),
        data: { idProducto: idProducto, cantidad: cantidad, costo: 0, vaConDescuento: vaConDescuento },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes")
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

function ObtenerCliente(idCliente) {
    var result = "";
    $.ajax({
        url: rootUrl("/Clientes/ObtenerCliente"),
        data: { idCliente: idCliente },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes_")
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


$('#btnGuardarPedidoEspecial').click(function (e) {

    var productos = [];
    //var idCliente = $('#idCliente').val();
    var idPedidoEspecial = $('#idPedidoEspecial').val();
    var esPedidoNormal = "true";
    var descripcion = $('#descripcionPedidoInterno').val();


    //if (descripcion == "") {
    //    MuestraToast('warning', "Debe escribir una descripcion para el Pedido Interno");
    //    return;
    //}


    console.log(esPedidoNormal);
    // si todo bien
    var tblVtas = document.getElementById('tablaRepPedidosEspeciales');
    var rCount = tblVtas.rows.length;

    if ((esPedidoNormal == "true") || (esPedidoNormal == "True")) {
        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {
                var row_ = {
                    idProducto: parseInt(tblVtas.rows[i].cells[1].innerHTML),
                    cantidad: parseInt(tblVtas.rows[i].cells[4].children[0].value),
                };
                productos.push(row_);
            }
        }
    }
    else {

        console.log("else");

    }
   
    const pedido = {};
    //pedido.idCliente = idCliente;
    pedido.descripcion = descripcion;
    pedido.idPedidoEspecial = idPedidoEspecial;
    pedido.lstPedidosInternosDetalle = productos;

    $.ajax({
        url: rootUrl("/PedidosEspeciales/GuardarPedidoEspecial"),
        data: JSON.stringify(pedido),
        method: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Guardando Pedido.");
        },
        success: function (data) {
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);

            if (data.Estatus == 200) {

                //if ((esPedidoNormal == "true") || (esPedidoNormal == "True")) {
                ImprimeTicketPedido(data.Modelo.idPedidoEspecial);
                //}

                InitSelect2Productos();
                limpiarTicket();
            }
            $('#ModalPrevioPedidoEspecial').modal('hide');

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al guardar el pedido especial, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

});



function ImprimeTicketPedido(idPedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspeciales/ImprimeTicketPedido"),
        data: { idPedidoEspecial: idPedidoEspecial },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            //console.log(data);
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

function ImprimeTicketDevolucion(idPedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspeciales/ImprimeTicketDevolucion"),
        data: { idPedidoEspecial: idPedidoEspecial },
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

function AbrirCajonDinero() {
    $.ajax({
        url: rootUrl("/PedidosEspeciales/AbrirCajon"),
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Abriendo Cajón...");
        },
        success: function (data) {
            console.log(data);
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast('error', "Ocurrio un error al intentar abrir el cajón del dinero.");
            console.log(xhr);
            console.log(status);
            console.log(data);
        }
    });
}


function numerico(evt) {
    evt = (evt) ? evt : window.event;

    var charCode = (evt.which) ? evt.which : evt.keyCode;

    if (charCode === 27) {
        document.execCommand('undo');
    }

    if (charCode === 13) {
        $(':focus').blur();
    }

    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function listenerDobleClick(element) {
    element.contentEditable = true;

    $(element).keydown(function (evt) {
        el = evt.target;

        if (evt.keyCode == 27) {
            document.execCommand('undo');
            element.contentEditable = false;
        }

        if (
            evt.keyCode != 8 || //backspace
            evt.keyCode != 9 || //tab
            evt.keyCode != 13 || //enter
            evt.keyCode != 37 || // left arrow 
            evt.keyCode != 39 || // right arrow 
            evt.keyCode != 46 || //delete
            evt.keyCode != 48 || //0
            evt.keyCode != 49 || //1
            evt.keyCode != 50 || //2
            evt.keyCode != 51 || //3
            evt.keyCode != 52 || //4
            evt.keyCode != 53 || //5
            evt.keyCode != 54 || //6
            evt.keyCode != 55 || //7
            evt.keyCode != 56 || //8
            evt.keyCode != 57    //9

        ) {
            event.preventDefault();
            element.contentEditable = false;

        }

        //if (
        //     evt.keyCode == 13 ||
        //    (evt.keyCode > 31 && (evt.keyCode < 48 || evt.keyCode > 57))
        //   )
        //{
        //    event.preventDefault();
        //    element.contentEditable = false;
        //}
        //else {       // si es un numero         
        //}
    });

    setTimeout(function () {
        if (document.activeElement !== element) {
            element.contentEditable = false;
        }
    }, 300);
}

//document.addEventListener('keydown', function (event) {
//    console.log("keydwn");
//    var esc = event.which == 27,
//        nl = event.which == 13,
//        el = event.target,
//        input = el.nodeName != 'INPUT' && el.nodeName != 'TEXTAREA',
//        data = {};

//    if (input) {
//        if (esc) {
//            // restore state
//            document.execCommand('undo');
//            el.blur();
//        } else if (nl) {
//            // save
//            data[el.getAttribute('data-name')] = el.innerHTML;

//            // we could send an ajax request to update the field
//            /*
//            $.ajax({
//              url: window.location.toString(),
//              data: data,
//              type: 'post'
//            });
//            */
//            log(JSON.stringify(data));

//            el.blur();
//            event.preventDefault();
//        }
//    }
//}, true);

//function log(s) {
//    document.getElementById('debug').innerHTML = 'value changed to: ' + s;
//}





//$("#efectivo").on("keyup", function (event) {

//    if (event.keyCode === 13) {

//        event.preventDefault();
//        document.getElementById("btnGuardarPedidoEspecial").click();

//    }
//    else {

//        var cambio_ = parseFloat(0).toFixed(2);
//        var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
//        var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

//        if (parseFloat(efectivo_) > parseFloat(total_)) {
//            cambio_ = efectivo_ - total_;
//            document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>";
//        }
//        else {
//            document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
//        }
//    }

//});


$("#cantidad").on("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("btnAgregarProducto").click();
    }
});

$("#montoARetirar").on("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("btnRetirarExcesoEfectivo").click();
    }
});


$("#idCliente").on("change", function () {

    $('#efectivo').val('');
    //document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    //document.getElementById("chkFacturar").checked = false;
    //document.getElementById("divUsoCFDI").style.display = 'none';
    $('#usoCFDI').val("3").trigger('change');
    $('#formaPago').val("1").trigger('change');

    var idCliente = parseFloat($('#idCliente').val());
    var data = ObtenerCliente(idCliente);
    var nombre = data.Modelo.nombres + "  " + data.Modelo.apellidoPaterno + "  " + data.Modelo.apellidoMaterno;
    var descuento = parseFloat(0.0);

    if (idCliente != 1) {
        descuento = parseFloat(data.Modelo.tipoCliente.descuento).toFixed(2);;
    }

    var total = parseFloat(document.getElementById("previoTotal").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    var descuentoMenudeo = parseFloat(document.getElementById("previoDescuentoMenudeo").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    var cantidadDescontada = parseFloat(0).toFixed(2);

    if (descuento > 0.0) {
        cantidadDescontada = parseFloat((total - descuentoMenudeo) * (descuento / 100)).toFixed(2);
    }

    var subTotal = parseFloat(total - descuentoMenudeo - cantidadDescontada).toFixed(2);
    var iva = parseFloat(0).toFixed(2);

    //// si lleva iva
    //if ($("#chkFacturar").is(":checked")) {
    //    iva = parseFloat(subTotal * 0.16).toFixed(2);
    //}

    var final = (parseFloat(subTotal) + parseFloat(iva)).toFixed(2);

    document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + cantidadDescontada + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + subTotal + "</h4>";
    //document.getElementById("previoIVA").innerHTML = "<h4>$" + iva + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + final + "</h4>";

    // para los datos del cliente
    var row_ = "<address>" +
        "    <strong></strong><br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "</address>";

    if ((data.idCliente != 1) && (idCliente != 1)) {
        row_ = "<address>" +
            "    <strong>Datos del Cliente:</strong><br>" +
            "    Nombre: " + nombre.toUpperCase() + "<br>" +
            "    Telefono: " + data.Modelo.telefono + "<br>" +
            "    E-mail: " + data.Modelo.correo + "<br>" +
            "    RFC: " + data.Modelo.rfc + "<br>" +
            "    Tipo de Cliente: " + data.Modelo.tipoCliente.descripcion + "<br>" +
            "</address>";
    }

    // para los tipo de clietne ruta
    if (data.Modelo.nombres.includes('RUTA')) {
        row_ = "<div id =\"divClientesAtendidos\">" +
            "     <div class=\"section-title\"><strong>  </strong></div>" +
            "     <div class=\"input-group mb-3\">" +
            "         <div class=\"input-group-prepend\">" +
            "             <span class=\"input-group-text\">Número de Clientes Atendidos por Ruta:</span>" +
            "         </div>" +
            "         <input id=\"numClientesAtendidos\" type=\"text\" class=\"form-control\" onkeypress=\"return esNumero(event)\">" +
            "     </div>" +
            "</div><br><br><br><br>";
    }

    document.getElementById("nombreCliente").innerHTML = row_;
});


function AbrirModalConsultaExistencias() {
    $('#existencias').html('');
    //para abrir el modal
    $('#ModalExistencias').modal({ backdrop: 'static', keyboard: false, show: true });
}

function AbrirModalCierreCajaExcedentes() {
    ConsultRetiros();
    ConsultaInfoCierre();
    $('#montoARetirar').val('');
    $('#ModalCierreExceso').modal({ backdrop: 'static', keyboard: false, show: true });
}

function AbrirModalCierreDia() {
    ConsultRetirosV2();
    ConsultaInfoCierreDia();
    $('#ModalCierre').modal({ backdrop: 'static', keyboard: false, show: true });
}


$('#btnRetirarExcesoEfectivo').click(function (e) {

    var cantidadEfectivo = parseFloat($('#cantidadEfectivo').html().replace('<p class=\"clearfix\"> <span class=\"float-left\">Cantidad en Efectivo:</span><span class=\"float-right text-muted\">$', '').replace('</span></p>', '').replace(' ', '')).toFixed(2);
    var cantidadRetirada = parseFloat($('#cantidadRetirada').html().replace('<p class=\"clearfix\"> <span class=\"float-left\">Cantidad Retirada del día:</span><span class=\"float-right text-muted\">$', '').replace('</span></p>', '').replace(' ', '')).toFixed(2);
    var montoARetirar_ = parseFloat($('#montoARetirar').val()).toFixed(2);

    if ($('#montoARetirar').val() == "") {
        MuestraToast('warning', "Debe seleccionar un monto para retirar.");
        return
    }

    if (((cantidadEfectivo - cantidadRetirada) - montoARetirar_) < 0.0) {
        var cantidadPorRetirar = parseFloat(0).toFixed(2);
        if ((parseFloat(cantidadEfectivo).toFixed(2) - parseFloat(cantidadRetirada).toFixed(2)) < 0)
            cantidadPorRetirar = parseFloat(0);
        else
            cantidadPorRetirar = parseFloat(cantidadEfectivo).toFixed(2) - parseFloat(cantidadRetirada).toFixed(2);

        MuestraToast('warning', "Solo tiene : $" + (cantidadPorRetirar).toString() + " para retirar.");
        return
    }
    // si todo bien
    retirarExcesoEfectivo(montoARetirar_);

});


$('#btnCierreDia').click(function (e) {
    //var monto = parseFloat($('#totalCierre').html().replace('<p class=\"clearfix\"> <span class=\"float-left\">Cantidad para Cierre:</span><span class=\"float-right text-muted\">$', '').replace('</span></p>', '').replace(' ', '')).toFixed(2);
    var totEfe = parseFloat($('#totalEfectivoCierre').html().replace('<p class=\"clearfix\"> <span class=\"float-left\">Total Efectivo:</span><span class=\"float-right text-muted\">$', '').replace('</span></p>', '').replace(' ', '')).toFixed(2);
    var totRet = parseFloat($('#retirosDelDiaCierre').html().replace('<p class=\"clearfix\"> <span class=\"float-left\">Retiros del Día:</span><span class=\"float-right text-muted\">$', '').replace('</span></p>', '').replace(' ', '')).toFixed(2);
    var monto = totEfe - totRet;

    if ((monto) <= 0.0) {
        MuestraToast('warning', "No cuenta con saldo para hacer el cierre de esta Estación.");
        return
    }

    swal({
        title: 'Mensaje',
        text: '¿Estas seguro que desea hacer el cierre para esta Estación?',
        icon: 'warning',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {

                $.ajax({
                    url: rootUrl("/PedidosEspeciales/RealizaCierreEstacion"),
                    data: { monto: monto },
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader();
                    },
                    success: function (data) {
                        MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
                        $('#ModalCierre').modal('hide');
                        OcultarLoader();
                        ImprimeTicketRetiro(data.Modelo.idRetiro, 2);
                    },
                    error: function (xhr, status) {
                        console.log('Hubo un problema al intentar hacer el cierre de esta estación, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                        OcultarLoader();
                    }
                });

            } else {
                console.log("cancelar");
            }
        });
});

function retirarExcesoEfectivo(montoRetiro) {

    $.ajax({
        url: rootUrl("/PedidosEspeciales/RetirarExcesoEfectivo"),
        data: { montoRetiro: parseFloat(montoRetiro) },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            $('#ModalCierreExceso').modal('hide');
            ImprimeTicketRetiro(data.Modelo.idRetiro, 1);
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

function ConsultRetiros() {
    $.ajax({
        url: rootUrl("/PedidosEspeciales/_ObtenerRetiros"),
        data: { idRetiro: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $('#tblRetiros_').html(data);
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}


function ConsultRetirosV2() {
    $.ajax({
        url: rootUrl("/PedidosEspeciales/_ObtenerRetirosV2"),
        data: { idRetiro: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $('#retirosDelDiaV2').html(data);
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

function ConsultaInfoCierre() {
    $.ajax({
        url: rootUrl("/PedidosEspeciales/ConsultaInfoCierre"),
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            OcultarLoader();
            $('#PedidosEspecialesDelDia').html("<p class=\"clearfix\"> <span class=\"float-left\">PedidosEspeciales del día: </span><span class=\"float-right text-muted\">" + data.Modelo.totalPedidosEspeciales + "</span></p>");
            $('#montoPedidosEspecialesDelDia').html("<p class=\"clearfix\"> <span class=\"float-left\">Monto de PedidosEspeciales: </span><span class=\"float-right text-muted\">$" + data.Modelo.montoPedidosEspecialesDelDia + "</span></p>");
            $('#cantidadEfectivo').html("<p class=\"clearfix\"> <span class=\"float-left\">Cantidad en Efectivo:</span><span class=\"float-right text-muted\">$" + data.Modelo.efectivoDisponible + "</span></p>");
            $('#cantidadRetirada').html("<p class=\"clearfix\"> <span class=\"float-left\">Cantidad Retirada del día:</span><span class=\"float-right text-muted\">$" + data.Modelo.retirosHechosDia + "</span></p>");
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}


function ConsultaInfoCierreDia() {
    $.ajax({
        url: rootUrl("/PedidosEspeciales/ConsultaInfoCierre"),
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            OcultarLoader();
            $('#vtasDelDiaCierre').html("<p class=\"clearfix\"> <span class=\"float-left\">PedidosEspeciales del día:</span><span class=\"float-right text-muted\">" + data.Modelo.totalPedidosEspeciales + "</span></p>");
            $('#totalEfectivoCierre').html("<p class=\"clearfix\"> <span class=\"float-left\">Total Efectivo:</span><span class=\"float-right text-muted\">$" + data.Modelo.efectivoDisponible + "</span></p>");
            $('#retirosDelDiaCierre').html("<p class=\"clearfix\"> <span class=\"float-left\">Retiros del Día:</span><span class=\"float-right text-muted\">$" + data.Modelo.retirosHechosDia + "</span></p>");
            $('#totalCierre').html("<p class=\"clearfix\"> <span class=\"float-left\">Cantidad para Cierre:</span><span class=\"float-right text-muted\">$" + data.Modelo.efectivoDisponible + "</span></p>");
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

$('#btnBuscarExistencias').click(function (e) {
    var idProducto = $('#idProductoExistencia').val();
    var idSucursal = $('#idSucursalExistencia').val();
    $.ajax({
        url: rootUrl("/Productos/_UbicacionesProductoPrecio"),
        data: { idProducto: idProducto, idSucursal: idSucursal },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            $("#existencias").html(data);
            ObtenerPrecios(idProducto);
            ObtenerIndividualMenudeo(idProducto);
            OcultarLoader();
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar mostrar las existencias del producto, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
});


function ObtenerPrecios(idProducto) {
    var result = '';
    $.ajax({
        url: rootUrl("/Productos/_PreciosProducto"),
        data: { idProducto: idProducto },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes")
        },
        success: function (data) {
            $("#Precios_").html(data);
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
    return result;
}


function ObtenerIndividualMenudeo(idProducto) {
    var result = '';
    $.ajax({
        url: rootUrl("/Productos/ObtenerProductos"),
        data: { idProducto: idProducto },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes")
        },
        success: function (data) {
            $('#precioIndividual_').html("$" + data.precioIndividual);
            $('#precioMenudeo_').html("$" + data.precioMenudeo);
            console.log(data);
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

//function revisarExistenciasCombo() {
//    $('select[id*="idProducto"] option').each(function (index, value) {
//        if ($(this).text().includes(' - (S/E)')) {
//            $("#idProducto>option[value='" + $(this).val() + "']").prop('disabled', true);
//        }
//    });
//}


function InitSelect2Productos() {

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

    var i;
    for (i = 0; i < result.Modelo.length; i++) {
        result.Modelo[i].id = result.Modelo[i]['idProducto'];
        result.Modelo[i].text = result.Modelo[i]['descripcionConExistencias'];
        if (result.Modelo[i].cantidad == 0)
            result.Modelo[i].disabled = true;
        else
            result.Modelo[i].disabled = false;
    }

    $("#idProducto").html('').select2();
    $('#idProducto').select2({
        width: "100%",
        placeholder: "--SELECCIONA--",
        data: result.Modelo,

        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });

    $('#idProducto').val("0").trigger('change');
    //console.log(arrayProductos);
}



function ImprimeTicketRetiro(idRetiro, tipoRetiro) {
    $.ajax({
        url: rootUrl("/PedidosEspeciales/ImprimeTicketRetiro"),
        data: { idRetiro: idRetiro, idCliente: idRetiro, tipoRetiro: tipoRetiro },
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





$(document).ready(function () {

    arrayPreciosRangos = ObtenerPrecios_(0);
    InitSelect2Productos();
    InitSelect2(); // los demas select2
    actualizaTicketPedidoEspecial();
    initInputsTabla();

    //document.getElementById("divUsoCFDI").style.display = 'none';
    $('#idSucursalExistencia').val('1').change().prop('disabled', false);

    //var esAgregarProductos = $('#esAgregarProductos').val();
    //if ((esAgregarProductos == "True") || (esAgregarProductos == "true")) {
    //    $('#idCliente').val($('#idClienteDevolucion').val()).trigger('change');
    //}

});