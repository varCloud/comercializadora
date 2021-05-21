var table;
var iframe;
var tablaVentas;
var arrayPreciosRangos = [];
var arrayProductos = [];
var tblProductosPedidoEspecial;
var productosPedidoEspecial = '';
var idPedidoEspecial = parseInt(0);
var producto_value = null;
var esDecimal_ = parseInt(0);
var idVentaComplemento = parseInt(0);
var arrayProductosVentaComplemento = [];
var ultimoCambio = parseFloat(0);

//busqueda
function onBeginSubmitVentas() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitVentas() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultVentas(data) {
    console.log("onSuccessResultVentas", JSON.stringify(data));
    tablaVentas.destroy();
    $('#rowVentas').html(data);
    InitDataTableVentas();
}
function onFailureResultVentas() {
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

function _facturaVenta(idVenta) {
    console.log("facturaVenta_" + idVenta);
    $.ajax({
        url: rootUrl("/Factura/GenerarFactura"),
        data: { idVenta: idVenta },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Facturando Venta.");
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            OcultarLoader();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}


function facturaVenta(idVenta) {
    console.log("facturaVenta_" + idVenta);
    $.ajax({
        url: pathDominio + "api/WsFactura/GenerarFactura",
        data: { idVenta: idVenta, idUsuario: idUsuarioGlobal },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Facturando Venta.");
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            OcultarLoader();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

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
    document.getElementById("tablaRepVentas").deleteRow(index_);
    actualizaTicketVenta();
}

$('#limpiar').click(function (e) {
    limpiarTicket();
});

$('#cancelar').click(function (e) {
    document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(ultimoCambio).toFixed(2) + "</h4>";
    limpiaModalPrevio();
});

$("#ModalPrevioVenta").on("hidden.bs.modal", function () {
    document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(ultimoCambio).toFixed(2) + "</h4>";
});

function limpiarTicket() {

    var max_id = parseFloat(0);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        var maximo_actual = parseFloat(fila.children[0].innerHTML);
        if (maximo_actual > max_id) {
            max_id = maximo_actual;
        }
    });

    var i;
    for (i = max_id; i >= 1; i--) {
        document.getElementById("tablaRepVentas").deleteRow(i);
    }

    actualizaTicketVenta();
    limpiaModalPrevio();
    $('#cantidad').val('1');
    $("#listProductos").val("");
    producto_value = null;
    //$('#idProducto').val("0").trigger('change');
    $('#idVenta').val(0);
    $('#vaConDescuento').val(0);
    idPedidoEspecial = 0;
    idVentaComplemento = 0;
    arrayProductosVentaComplemento = [];
    $("#actionVenta").html("");

}

function limpiaModalPrevio() {
    var esAgregarProductos = $('#esAgregarProductos').val();

    var row_ = "<address>" +
        "    <strong></strong><br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "</address>";

    document.getElementById("nombreCliente").innerHTML = row_;

    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoDescuentoMenudeo").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoComisionBancaria").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    $('#efectivo').val('');
    $('#formaPago').val("1").trigger('change');
    $('#usoCFDI').val("3").trigger('change');

    if ((esAgregarProductos == "True") || (esAgregarProductos == "true")) {
        $('#idCliente').val($('#idClienteDevolucion').val()).trigger('change');
    }
    else {
        $('#idCliente').val("1").trigger('change');
    }

}



$('#previoVenta').click(function (e) {

    var esDevolucion = $('#esDevolucion').val();

    if ((esDevolucion == "true") || (esDevolucion == "True")) {
        // validamos que al menos exista devolucion de un item
        var tblVtas = document.getElementById('tablaRepVentas');
        var rCount = tblVtas.rows.length;
        var productosOriginales = parseInt(0);
        var productosDevueltos = parseInt(0);

        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {
                productosDevueltos += parseInt(tblVtas.rows[i].cells[7].children[0].value);
                productosOriginales += parseInt(tblVtas.rows[i].cells[4].children[0].value);
            }
        }

        if (productosDevueltos <= 0) {
            MuestraToast('warning', "Debe seleccionar al menos un producto para devolver.");
            return;
        }

        if (productosDevueltos >= productosOriginales) {
            MuestraToast('warning', "Para devolver todos los productos cancele la venta desde el menu de Editar Ventas");
            return;
        }


        $('#motivoDevolucion').val('');
        $('#ModalDevolucion').modal({ backdrop: 'static', keyboard: false, show: true });
    }
    else {
        abrirModalPrevioVenta();
    }
});

function abrirModalPrevioVenta() {

    limpiaModalPrevio();

    var total = parseFloat(0);
    var descuento = parseFloat(0);
    ultimoCambio = parseFloat(document.getElementById("ultimoCambio").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);


    $('#tablaRepVentas tbody tr').each(function (index, fila) {

        if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
            total += parseFloat(fila.children[5].innerHTML.replace('$', ''));
            descuento += parseFloat(fila.children[6].innerHTML.replace('$', ''));
        }

    });

    if (total > 0) {
        document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(total + descuento).toFixed(2) + "</h4>";
        document.getElementById("previoDescuentoMenudeo").innerHTML = "<h4>$" + parseFloat(descuento).toFixed(2) + "</h4>";
        document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(total + descuento - descuento).toFixed(2) + "</h4>";
        document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(total + descuento - descuento).toFixed(2) + "</h4>";
        $('#ModalPrevioVenta').modal({ backdrop: 'static', keyboard: false, show: true });
    }
    else {
        MuestraToast('warning', "Debe tener productos agregados para continuar con la venta.");
    }

}



$('#btnAceptarDevolucion').click(function (e) {


    if (($('#motivoDevolucion').val() == "")) {
        MuestraToast('warning', "Debe seleccionar el motivo de la devolución");
        return;
    }

    $('#ModalDevolucion').modal('hide');
    //abrirModalPrevioVenta();
    document.getElementById("btnGuardarVenta").click();

});



$('#btnAgregarProducto').click(function (e) {

    //if (AgregarProducto($('#idProducto').select2('data')[0], $('#cantidad').val())) {
    //    $('#cantidad').val('');

    //    Agregar envase
    //    if (parseInt($('#idProducto').select2('data')[0].idLineaProducto) === 20) {
    //        $('#ModalAgregarEnvase').modal({ backdrop: 'static', keyboard: false, show: true });
    //    }

    //    actualizaTicketVenta();
    //    initInputsTabla();
    //}

    if (AgregarProducto(producto_value, $('#cantidad').val())) {

        //Agregar envase
        if (parseInt(producto_value.idLineaProducto) === 20) {
            $('#ModalAgregarEnvase').modal({ backdrop: 'static', keyboard: false, show: true });
        }

        $('#cantidad').val('1');
        $("#listProductos").val('');
        producto_value = null;
        $("#listProductos").focus();

        actualizaTicketVenta();
        initInputsTabla();
    }
});

$('#btnAgregarEnvase').click(function (e) {
    if (AgregarProducto($('#idProductoEnvase').select2('data')[0], $('#cantidadEnvase').val())) {
        $('#cantidadEnvase').val('');
        $('#idProductoEnvase').val("0").trigger('change');
        $('#ModalAgregarEnvase').modal('hide');
        actualizaTicketVenta();
        initInputsTabla();
    }
});

function AgregarProducto(producto, cantidad) {

    var esAgregarProductos = $('#esAgregarProductos').val();

    cantidad = parseFloat(cantidad) || 0.0;

    if (producto === null || producto === undefined) {
        MuestraToast('warning', "Debe seleccionar el producto que desea agregar.");
        return false;
    }

    if (parseFloat(cantidad) === 0) {
        MuestraToast('warning', "Debe escribir la cantidad de productos que va a agregar.");
        return false;
    }

    if (producto.cantidad < parseFloat(cantidad)) {
        MuestraToast('warning', "No existe suficiente producto en inventario.");
        return false;
    }

    if (producto.precioIndividual <= 0 && producto.precioMenudeo <= 0) {
        preguntaAltaPrecios();
        return false;
    }

    if (producto.precioIndividual <= 0) {
        MuestraToast('warning', "Debe configurar el precio invidual del producto.");
        return false;
    }

    if (producto.precioMenudeo <= 0) {
        MuestraToast('warning', "Debe configurar el precio Mayoreo del producto.");
        return false;
    }

    var btnEliminaFila = "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
    var precio = parseFloat(0).toFixed(2);
    var descuento = parseFloat(0).toFixed(2);
    var existeProducto = false;
    var existeProductoAgregar = false;

    var tblVtas = document.getElementById('tablaRepVentas');
    var rCount = tblVtas.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            if ((esAgregarProductos == "true") || (esAgregarProductos == "True")) {
                if (
                    (producto.idProducto === parseInt(tblVtas.rows[i].cells[1].innerHTML)) &&
                    (!tblVtas.rows[i].cells[7].getAttribute("class").includes('esAgregarProductos'))
                ) {
                    var cantidad = parseFloat(tblVtas.rows[i].cells[4].children[0].value) + cantidad;

                    if (cantidad > producto.cantidad) {
                        MuestraToast('warning', "No existe suficiente producto en inventario.");
                        return false;
                    }
                    tblVtas.rows[i].cells[4].children[0].value = cantidad;
                    existeProductoAgregar = true;
                }
            }
            else {
                if (producto.idProducto === parseInt(tblVtas.rows[i].cells[1].innerHTML)) {
                    var cantidad = parseFloat(tblVtas.rows[i].cells[4].children[0].value) + cantidad;

                    if (cantidad > producto.cantidad) {
                        MuestraToast('warning', "No existe suficiente producto en inventario.");
                        return false;
                    }
                    tblVtas.rows[i].cells[4].children[0].value = cantidad;
                    existeProducto = true;
                }
            }
        }
    }


    if ((!existeProducto) && (!existeProductoAgregar)) {
        // si todo bien    
        var row_ = "<tr>" +
            "  <td>1</td>" +
            "  <td> " + producto.idProducto + "</td>" +
            "  <td> " + producto.descripcion + "</td>" +
            "  <td class=\"text-center\">$" + precio + "</td>" +
            "  <td class=\"text-center\"><input type='text' onkeypress=\"return numerico(event)\" style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + cantidad + "\"></td>" +
            "  <td class=\"text-center\">$" + precio + "</td>" +
            "  <td class=\"text-center\">$" + descuento + "</td>" +
            "  <td class=\"text-center\">" +
            btnEliminaFila +
            "  <td style=\"display: none;\">0</td>" +
            "  </td>" +
            "</tr >";

        $("#tablaRepVentas tbody").prepend(row_);
    }

    return true;
}


function actualizaTicketVenta() {

    // acttualizamos el id y la funcion de eliminar fila
    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;

        if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
            fila.children[7].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        }

    });

    // contabilizamos todos los productos para consultar que precio le corresponde a cada uno
    var productos = [];
    var tblVtas = document.getElementById('tablaRepVentas');
    var rCount = tblVtas.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var row_ = {
                idProducto: parseInt(tblVtas.rows[i].cells[1].innerHTML),
                cantidad: parseInt(tblVtas.rows[i].cells[4].children[0].value),
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

        //Si es complemento de venta se suman a la cantidad los productos vendidos
        if (idVentaComplemento > 0) {

            for (var c = 0; c < arrayProductosVentaComplemento.length; c++) {
                if (productos.some(x => x.idProducto === arrayProductosVentaComplemento[c].idProducto)) {
                    productos.find(x => x.idProducto == arrayProductosVentaComplemento[c].idProducto).cantidad += arrayProductosVentaComplemento[c].cantidad;
                }
                else {
                    var row_ = {
                        idProducto: parseInt(arrayProductosVentaComplemento[c].idProducto),
                        cantidad: parseInt(arrayProductosVentaComplemento[c].cantidad),
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


    //  si se ejecuta precio de mayoreo cuando el ticket tiene 12 o + articulos
    for (var o = 0; o < productos.length; o++) {
        if (cantidadDeProductos >= 12) {
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
            (cantidadTotalPorProducto[q].cantidad > 12) && // ocupa ser mayor a 12 para que se evalue un rango
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


    //console.log(cantidadTotalPorProducto);
    //console.log(productos);
    //console.log(arrayProductos);

    // actualizamos el ticket
    for (var j = 0; j < productos.length; j++) {

        var tblVtas = document.getElementById('tablaRepVentas');
        var rCount = tblVtas.rows.length;

        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {

                var cantidad = parseFloat(tblVtas.rows[i].cells[4].children[0].value);

                if ((!tblVtas.rows[i].cells[7].getAttribute("class").includes('esDevolucion')) && (!tblVtas.rows[i].cells[7].getAttribute("class").includes('esAgregarProductos'))) {
                    //console.log(tblVtas.rows[i].cells[7].getAttribute("class"));

                    if ((parseInt(tblVtas.rows[i].cells[1].innerHTML)) === (parseInt(productos[j].idProducto))) {
                        tblVtas.rows[i].cells[3].innerHTML = "$" + parseFloat(productos[j].precioVenta).toFixed(2);   //precio
                        tblVtas.rows[i].cells[5].innerHTML = "$" + (parseFloat(productos[j].precioVenta) * cantidad).toFixed(2);   //total
                        tblVtas.rows[i].cells[6].innerHTML = "$" + (parseFloat(productos[j].precioIndividual - productos[j].precioVenta) * cantidad).toFixed(2);  //descuento
                    }
                }
            }
        }
    }

    actualizarSubTotal();

    //return;

    //dataToPost = JSON.stringify({ precios: productos });

    //$.ajax({
    //    url: rootUrl("/Ventas/ObtenerPreciosDeProductos"),
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

    //                var tblVtas = document.getElementById('tablaRepVentas');
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



function maxed(a, b) {
    return a > b;
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

    $('#tablaRepVentas input').on('change', function () {

        var thisInput = $(this);
        var mensaje = "Debe escribir la cantidad de productos.";
        var cell = $(this).closest('td');
        var row = cell.closest('tr');
        var rowIndex = row[0].rowIndex;
        var tblVtas = document.getElementById('tablaRepVentas');
        var idProducto = parseInt(tblVtas.rows[rowIndex].cells[1].innerHTML);
        var producto = arrayProductos.find(x => x.idProducto == idProducto);

        console.log(producto);
        console.log(parseInt(thisInput.val()));

        if ((parseInt(thisInput.val())) > (parseInt(producto.cantidad))) {
            MuestraToast('warning', "No existe suficiente producto en inventario.");
            document.execCommand('undo');
            return;
        }

        if (thisInput.hasClass("esDevolucion")) {
            mensaje = "Debe escribir la cantidad de productos que va a devolver.";
        }

        if ((thisInput.val() == "") || (thisInput.val() == "0")) {
            MuestraToast('warning', mensaje);
            document.execCommand('undo');
        }

        if (thisInput.hasClass("esDevolucion")) {

            if ((parseInt(thisInput.val())) > (parseInt(tblVtas.rows[rowIndex].cells[4].children[0].value))) {
                MuestraToast('warning', "No puede regresar mas de lo que compro.");
                document.execCommand('undo');
                return;
            }

            actualizarSubTotalDevoluciones();
        }
        else {
            actualizaTicketVenta();
        }

        $("#listProductos").focus();

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
    $('#tablaRepVentas tbody tr').each(function (index, fila) {

        if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
            subTotal += parseFloat(fila.children[5].innerHTML.replace('$', ''));
        }

    });
    return subTotal;
}

function actualizarSubTotal() {

    var subTotal = parseFloat(0);
    //var descuento = parseFloat(0);
    var esDevolucion = $('#esDevolucion').val();
    subTotal = cuentaSubTotal();
    //$('#tablaRepVentas tbody tr').each(function (index, fila) {
    //    subTotal += parseFloat(fila.children[5].innerHTML.replace('$', ''));
    //    descuento += parseFloat(fila.children[6].innerHTML.replace('$', ''));
    //});

    if ((esDevolucion == "true") || (esDevolucion == "True")) {
        subTotal = 0;
    }

    document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(subTotal).toFixed(2) + "</h4>";
}

function actualizarSubTotalDevoluciones() {

    var tblVtas = document.getElementById('tablaRepVentas');
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
        url: rootUrl("/Ventas/ObtenerProductoPorPrecio"),
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

function initTimer(){
    var timer = setTimeout(function () {
        $("#btnGuardarVenta").removeClass('btn-progress disabled');
        console.log('timer');
        clearInterval(timer);
    }, 100);
}


$('#btnGuardarVenta').click(function (e) {
    console.log('#btnGuardarVenta');
    $("#btnGuardarVenta").addClass('btn-progress disabled');
   
    var productos = [];
    var idCliente = $('#idCliente').val();
    var formaPago = $('#formaPago').val();
    var usoCFDI = $('#usoCFDI').val();
    var idVenta = $('#idVenta').val();
    var aplicaIVA = parseInt(0);
    var numClientesAtendidos = parseInt(0);
    var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
    var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);
    var esDevolucion = $('#esDevolucion').val();
    var esAgregarProductos = $('#esAgregarProductos').val();
    var esVentaNormal = "true";
    var motivoDevolucion = $('#motivoDevolucion').val();
    var tipoVenta = parseInt(1); // 1-Normal / 2-Devolucion / 3-Agregar Productos a la venta



    if (((esDevolucion == "true") || (esDevolucion == "True")) || ((esAgregarProductos == "true") || (esAgregarProductos == "True"))) {
        esVentaNormal = "false"
    }

    if ((esDevolucion == "false") || (esDevolucion == "False")) {
        // validaciones
        if (($('#efectivo').val() == "") && (parseInt(formaPago) == parseInt(1))) {
            MuestraToast('warning', "Debe escribir con cuanto efectivo le estan pagando.");
            //document.getElementById("btnGuardarVenta").disabled = false;
            $("#btnGuardarVenta").removeClass('btn-progress disabled');
            return;
        }

        if (parseFloat(efectivo_) < parseFloat(total_)) {
            MuestraToast('warning', "El efectivo no alcanza a cubrir el costo total de la venta: " + total_.toString());
            //document.getElementById("btnGuardarVenta").disabled = false;
            $("#btnGuardarVenta").removeClass('btn-progress disabled');
            return;
        }

        if ($("#chkFacturar").is(":checked")) {
            aplicaIVA = parseInt(1);
        }

        if (($("#idCliente").find("option:selected").text()).includes('RUTA')) {

            if ($('#numClientesAtendidos').val() == "") {
                MuestraToast('warning', "Debe escribir cuantos clientes son atendidos por la ruta.");
                //document.getElementById("btnGuardarVenta").disabled = false;
                $("#btnGuardarVenta").removeClass('btn-progress disabled');
                return;
            }
            else {
                numClientesAtendidos = parseInt($('#numClientesAtendidos').val());
            }
        }

        if ((parseInt(formaPago) !== parseInt(1))) // si no es efectivo
        {
            efectivo_ = total_;
        }

    }
    else {
        efectivo_ = parseFloat(0);
    }



    // si todo bien
    var tblVtas = document.getElementById('tablaRepVentas');
    var rCount = tblVtas.rows.length;

    if ((esVentaNormal == "true") || (esVentaNormal == "True")) {
        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {
                var row_ = {
                    idProducto: parseInt(tblVtas.rows[i].cells[1].innerHTML),
                    cantidad: parseFloat(tblVtas.rows[i].cells[4].children[0].value),
                };
                productos.push(row_);
            }
        }
    }

    if ((esDevolucion == "true") || (esDevolucion == "True")) {
        if (rCount >= 2) {
            tipoVenta = parseInt(2);
            for (var i = 1; i < rCount; i++) {
                var row_ = {
                    idProducto: parseInt(tblVtas.rows[i].cells[1].innerHTML),
                    cantidad: parseFloat(tblVtas.rows[i].cells[4].children[0].value),
                    productosDevueltos: parseInt(tblVtas.rows[i].cells[7].children[0].value),
                    idVentaDetalle: parseInt(tblVtas.rows[i].cells[8].innerHTML),
                };
                productos.push(row_);
            }
        }
    }

    if ((esAgregarProductos == "true") || (esAgregarProductos == "True")) {
        if (rCount >= 2) {
            tipoVenta = parseInt(3);
            for (var i = 1; i < rCount; i++) {
                var row_ = {
                    idProducto: parseInt(tblVtas.rows[i].cells[1].innerHTML),
                    cantidad: parseFloat(tblVtas.rows[i].cells[4].children[0].value),
                    idVentaDetalle: parseInt(tblVtas.rows[i].cells[8].innerHTML),
                };
                productos.push(row_);
            }
        }
    }

    dataToPost = JSON.stringify({ venta: productos, idCliente: idCliente, formaPago: formaPago, usoCFDI: usoCFDI, idVenta: idVenta, aplicaIVA: aplicaIVA, numClientesAtendidos: numClientesAtendidos, tipoVenta: tipoVenta, motivoDevolucion: motivoDevolucion, idPedidoEspecial: idPedidoEspecial, idVentaComplemento: idVentaComplemento, montoTotalVenta: total_, montoPagado: efectivo_ });

    $.ajax({
        url: rootUrl("/Ventas/GuardarVenta"),
        data: dataToPost,
        method: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Guardando Venta.");
            $("#btnGuardarVenta").addClass('btn-progress disabled');
        },
        success: function (data) {
            OcultarLoader();

            if ((esDevolucion == "true") || (esDevolucion == "True")) {
                //swal(data.Mensaje, '', data.Estatus == 200 ? 'success' : 'error');
            }
            else {
                MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            }

            if (data.Estatus == 200) {
                //console.log(esVentaNormal);

                if ((esVentaNormal == "true") || (esVentaNormal == "True")) {
                    ImprimeTicket(data.Modelo.idVenta);
                    if (data.Modelo.cantProductosLiq > 0)
                        ImprimeTicketDespachadores(data.Modelo.idVenta);

                    if ($("#chkFacturar").is(":checked")) {
                        facturaVenta(data.Modelo.idVenta);
                    }
                }

                if ((esDevolucion == "true") || (esDevolucion == "True")) {
                    ImprimeTicketDevolucion(data.Modelo.idVenta);
                    ImprimeTicket(data.Modelo.idVenta);

                    swal({
                        title: "Mensaje",
                        text: data.Mensaje,
                        type: data.Estatus == 200 ? 'success' : 'error'
                    }).then(function () {
                        window.location.href = rootUrl("/Ventas/ConsultaVentas")
                        //"http://" + window.location.host + "/Ventas/ConsultaVentas";
                    });

                }

                if ((esAgregarProductos == "true") || (esAgregarProductos == "True")) {
                    ImprimeTicket(data.Modelo.idVenta);
                    //window.location.href = "http://" + window.location.host + "/Ventas/Ventas";
                    window.location.href = rootUrl("/Ventas/Ventas");
                    
                }

                ultimoCambio = parseFloat(document.getElementById("ultimoCambio").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

                InitSelect2Productos();
                limpiarTicket();
                ConsultExcesoEfectivo();
            }
            $('#ModalPrevioVenta').modal('hide');
            //document.getElementById("btnGuardarVenta").disabled = false;
            //$("#btnGuardarVenta").removeClass('btn-progress disabled');
            initTimer();

        },
        error: function (xhr, status) {
            OcultarLoader();
            //document.getElementById("btnGuardarVenta").disabled = false;
            $("#btnGuardarVenta").removeClass('btn-progress disabled');
            console.log('Hubo un problema al guardar la venta, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
    //document.getElementById("btnGuardarVenta").disabled = false;
    //console.log('#btnGuardarVenta boton habilitado');

});


$('#chkFacturar').click(function () {

    var idCliente = $('#idCliente').val();
    var esDevolucion = $('#esDevolucion').val();

    if ((esDevolucion == "true") || (esDevolucion == "True")) {
        MuestraToast('warning', "No es posible facturar una Devolución.");
        document.getElementById("chkFacturar").checked = false;
        return
    }

    if (idCliente == 1) {
        MuestraToast('warning', "Debe seleccionar un cliente diferente a " + $("#idCliente").find("option:selected").text());
        document.getElementById("chkFacturar").checked = false;
        return
    }

    $('#efectivo').val('');
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    var subTotal = parseFloat(document.getElementById("previoSubTotal").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    var iva = parseFloat(0).toFixed(2);
    var final = parseFloat(subTotal).toFixed(2);
    if ($(this).is(':checked')) {
        document.getElementById("divUsoCFDI").style.display = 'block';
        iva = parseFloat(subTotal * 0.16).toFixed(2);
        final = (parseFloat(subTotal) + parseFloat(iva)).toFixed(2);
    } else {
        document.getElementById("divUsoCFDI").style.display = 'none';
    }
    document.getElementById("previoIVA").innerHTML = "<h4>$" + iva + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + final + "</h4>";
});

function ImprimeTicket(idVenta) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicket"),
        data: { idVenta: idVenta },
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
            setTimeout(() => { eliminaArchivo(data.Modelo.archivo); }, 3000);
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

function ImprimeTicketDespachadores(idVenta) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicketDespachadores"),
        data: { idVenta: idVenta },
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

function ImprimeTicketDevolucion(idVenta) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicketDevolucion"),
        data: { idVenta: idVenta },
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
        url: rootUrl("/Ventas/AbrirCajon"),
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


function eliminaArchivo(rutaArchivo) {
    $.ajax({
        url: rootUrl("/Productos/EliminaArchivo"),
        data: { 'rutaArchivo': rutaArchivo },
        method: 'post',
        dataType: 'json',
        //contentType: "text/xml",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            console.log(data);
            OcultarLoader();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
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





$("#efectivo").on("keyup", function (event) {

    if (event.keyCode === 13) {

        event.preventDefault();
        document.getElementById("btnGuardarVenta").click();

    }
    else {

        var cambio_ = parseFloat(0).toFixed(2);
        var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
        var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

        if (parseFloat(efectivo_) > parseFloat(total_)) {
            cambio_ = efectivo_ - total_;
            document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>";
            document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>";
        }
        else {
            document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
            document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
        }
    }

});


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
    var idCliente = parseFloat($('#idCliente').val());
    var data = ObtenerCliente(idCliente);
    var nombre = data.Modelo.nombres + "  " + data.Modelo.apellidoPaterno + "  " + data.Modelo.apellidoMaterno;
    var descuento = parseFloat(0.0);

    if (idCliente != 1) {
        descuento = parseFloat(data.Modelo.tipoCliente.descuento).toFixed(2);;
    }
    $("#txtDescuentoCliente").val(descuento);
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
    calculaTotales();
});


$("#formaPago").on("change", function (value) {
    this.value == 1 ? $('#dvEfectivo').css('display', '') : $('#dvEfectivo').css('display', 'none');
    calculaTotales();
});

function calculaTotales() {

    $('#efectivo').val('');
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    //document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";    
    document.getElementById("chkFacturar").checked = false;
    document.getElementById("divUsoCFDI").style.display = 'none';
    $('#usoCFDI').val("3").trigger('change');
    var formaPago = $('#formaPago').val();
    var porcentajeComisionBancaria = parseFloat(0);
    var descuento = parseFloat(0);

    // si la forma de pago es tarjeta de debito o credito se agrega comision bancaria
    if ((parseInt(formaPago) == parseInt(4)) || (parseInt(formaPago) == parseInt(18))) {
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

    var subTotal = parseFloat(total - descuentoMenudeo - cantidadDescontada).toFixed(2);
    var comisionBancaria = parseFloat((subTotal) * (porcentajeComisionBancaria / 100)).toFixed(2);
    subTotal = parseFloat(subTotal) + parseFloat(comisionBancaria);
    var iva = parseFloat(0).toFixed(2);

    // si lleva iva
    if ($("#chkFacturar").is(":checked")) {
        iva = parseFloat(subTotal * 0.16).toFixed(2);
    }

    var final = (parseFloat(subTotal) + parseFloat(iva)).toFixed(2);

    document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + cantidadDescontada + "</h4>";
    document.getElementById("previoComisionBancaria").innerHTML = "<h4>$" + comisionBancaria + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + subTotal + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + iva + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + final + "</h4>";



}

function AbrirModalConsultaExistencias() {
    $("#listProductosExistencia").val('');
    $("#idProductoExistencia").val('');
    $('#existencias').html(''); 
    //$("#listProductosExistencia").focus();
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
    //ConsultRetirosV2();
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
    //var totEfe = parseFloat($('#totalEfectivoCierre').html().replace('<p class=\"clearfix\"> <span class=\"float-left\">Total Efectivo:</span><span class=\"float-right text-muted\">$', '').replace('</span></p>', '').replace(' ', '')).toFixed(2);
    //var totRet = parseFloat($('#retirosDelDiaCierre').html().replace('<p class=\"clearfix\"> <span class=\"float-left\">Retiros del Día:</span><span class=\"float-right text-muted\">$', '').replace('</span></p>', '').replace(' ', '')).toFixed(2);
    //var monto = totEfe - totRet;

    //if ((monto) <= 0.0) {
    //    MuestraToast('warning', "No cuenta con saldo para hacer el cierre de esta Estación.");
    //    return
    //}

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
                    url: rootUrl("/Ventas/RealizaCierreEstacion"),
                    data: { monto: 0 },
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
                        location.href = rootUrl("Ventas/Ventas/");
                        //ConsultExcesoEfectivo();
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
        url: rootUrl("/Ventas/RetirarExcesoEfectivo"),
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
            ConsultExcesoEfectivo();
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
        url: rootUrl("/Ventas/_ObtenerRetiros"),
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
        url: rootUrl("/Ventas/_ObtenerRetirosV2"),
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
        url: rootUrl("/Ventas/ConsultaInfoCierre"),
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            OcultarLoader();
            $('#AperturaCaja').html("<p class=\"clearfix\"> <span class=\"float-left\">Apertura de Caja: </span><span class=\"float-right text-muted\">$" + data.Modelo.montoApertura + "</span></p>");
            $('#IngresosEfectivo').html("<p class=\"clearfix\"> <span class=\"float-left\">Ingresos de Efectivo (Solicitud): </span><span class=\"float-right text-muted\">$" + data.Modelo.montoIngresosEfectivo + "</span></p>");

            $('#ventasDelDia').html("<p class=\"clearfix\"> <span class=\"float-left\">Ventas del día: </span><span class=\"float-right text-muted\">" + data.Modelo.totalVentas + "</span></p>");
            $('#montoVentasDelDia').html("<p class=\"clearfix\"> <span class=\"float-left\">Monto de Ventas: </span><span class=\"float-right text-muted\">$" + data.Modelo.montoVentasDelDia + "</span></p>");
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
        url: rootUrl("/Ventas/_CierreDia"),
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (view) {
            $('#PartialViewCierreDia').html(view);
            OcultarLoader();
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

    var finalData = $.map(result.Modelo, function (item) {

        return {
            label: item.descripcionConExistencias,
            producto: item
        }

    });

    $("#listProductos").autocomplete({
        //source take a list of data
        source: finalData,
        minLength: 1,//min = 2 characters
        select: function (event, ui) {
            producto_value = null;
            //producto_value = ui.item.producto; // start an alert which contains the value of proposal
            if (ui.item.producto.cantidad > 0) {
                $("#listProductos").val(ui.item.label);
                producto_value = ui.item.producto;
            } else {
                MuestraToast('warning', "No existe suficiente producto en inventario");
                $("#listProductos").val("");
            }

            console.log(ui.item.producto.idUnidadMedida);
            var cantidad_ = document.getElementById("cantidad");

            // se valida si el campo cantidad puede aceptar decimales para cuando la unidad de medida es Kilogramo,Gramo,Litro,Mililitro
            if (
                parseInt(ui.item.producto.idUnidadMedida) === parseInt(1) ||    // Kilogramo
                parseInt(ui.item.producto.idUnidadMedida) === parseInt(2) ||    // Gramo
                parseInt(ui.item.producto.idUnidadMedida) === parseInt(3) ||    // Litro
                parseInt(ui.item.producto.idUnidadMedida) === parseInt(4)       // Mililitro
            ) {
                esDecimal_ = parseInt(1);
            }
            else {
                esDecimal_ = parseInt(0);
            }

            $('#cantidad').val('1');

            return false;
        }
    });

    $("#listProductos").keypress(function (evt) {
        producto_value = null;
        if (evt.which == 13) {
            if (($("#listProductos").val()) !== "") {
                var producto = arrayProductos.find(x => x.codigoBarras == $("#listProductos").val());
                if (producto != null) {
                    $("#listProductos").val(producto.descripcionConExistencias);
                    producto_value = producto;
                    $("#cantidad").val(1);
                    $("#btnAgregarProducto").click();
                }
                else {
                    MuestraToast("error", "El producto no existe");
                    $("#listProductos").val("");
                }
            }
        }
    });


    //existencias
    var DataExistencias = $.map(result.Modelo, function (item) {

        return {
            label: item.descripcion,
            producto: item
        }

    });

    $("#listProductosExistencia").autocomplete({
        //source take a list of data
        source: DataExistencias,
        minLength: 1,//min = 2 characters
        select: function (event, ui) {
            $("#listProductosExistencia").val(ui.item.label);
            return false;
        }
    });

    $("#listProductosExistencia").keypress(function (evt) { 
        $("#idProductoExistencia").val('');
        if (evt.which == 13) {
            if (($("#listProductosExistencia").val()) !== "") {
                let producto = arrayProductos.find(x => x.codigoBarras.toUpperCase() == $("#listProductosExistencia").val().toUpperCase() || x.descripcion.toUpperCase() == $("#listProductosExistencia").val().toUpperCase());
                if (producto != null) {
                    $("#listProductosExistencia").val(producto.descripcion);
                    $("#idProductoExistencia").val(producto.idProducto);                  
                    $("#btnBuscarExistencias").click();
                }
                else {
                    MuestraToast("error", "El producto no existe");
                    $("#listProductosExistencia").val("");
                }
            }
        }
    });


    //$("#idProducto").html('').select2();
    //$('#idProducto').select2({
    //    width: "100%",
    //    placeholder: "--SELECCIONA--",
    //    data: result.Modelo,

    //    language: {
    //        noResults: function () {
    //            return "No hay resultado";
    //        },
    //        searching: function () {
    //            return "Buscando..";
    //        }
    //    }
    //});

    //$('#idProducto').val("0").trigger('change');

    //Producto Tipo Envase
    $("#idProductoEnvase").html('').select2();
    $('#idProductoEnvase').select2({
        width: "100%",
        placeholder: "--SELECCIONA--",
        data: (result.Modelo).filter(x => x.idLineaProducto == 19),

        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });

    $('#idProductoEnvase').val("0").trigger('change');

}


function validaTipoMedida(txt, evt) {

    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;

    if (esDecimal_ === parseInt(1)) {

        if (charCode == 46) {
            if (txt.value.indexOf('.') === -1) {
                return true;
            } else {
                return false;
            }
        } else {
            if (charCode > 31 &&
                (charCode < 48 || charCode > 57))
                return false;
        }
        return true;
    }
    else {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
}




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
    //ValidaAperturaCajas();
    arrayPreciosRangos = ObtenerPrecios_(0);
    InitSelect2Productos();
    InitSelect2(); // los demas select2
    actualizaTicketVenta();
    initInputsTabla();

    document.getElementById("divUsoCFDI").style.display = 'none';
    $('#idSucursalExistencia').val('1').change().prop('disabled', false);

    var esAgregarProductos = $('#esAgregarProductos').val();
    if ((esAgregarProductos == "True") || (esAgregarProductos == "true")) {
        $('#idCliente').val($('#idClienteDevolucion').val()).trigger('change');
    }

    $("#listProductos").focus();
    $('#dvEfectivo').css('display', '')

});

//*********************** PEDIDO ESPECIAL  ************************************
$("#NoPedidoEspecial").keyup(function (evt) {
    evt.preventDefault();
    LimpiarFormPedidoEspecial();

});

function AbrirModalPedidoEspecial() {
    $("#NoPedidoEspecial").val("");
    LimpiarFormPedidoEspecial();
    $('#ModalPedidoEspecial').modal({ backdrop: 'static', keyboard: false, show: true });
}

function BuscarPedidoEspecial() {

    var NoPedidoEspecial = parseInt($("#NoPedidoEspecial").val()) || 0;
    if (parseInt(NoPedidoEspecial) === 0) {
        MuestraToast('warning', "Debe especificar el numero de ticket del pedido especial que desea buscar.");
        return false;
    }

    $.ajax({
        url: rootUrl("/Productos/ObtenerProductosPedidoEspecial"),
        data: { idPedidoEspecial: NoPedidoEspecial },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            if (data.Estatus === 200) {
                productosPedidoEspecial = data;
                var i;
                var html = "";

                for (i = 0; i < productosPedidoEspecial.Modelo.length; i++) {

                    html += '<tr>';
                    html += '<td>' + productosPedidoEspecial.Modelo[i].idProducto + '</td>';
                    html += '<td>' + productosPedidoEspecial.Modelo[i].descripcion + '</td>';
                    html += '<td>' + productosPedidoEspecial.Modelo[i].cantidadRecibida + '</td>';
                    html += '<td>' + productosPedidoEspecial.Modelo[i].cantidad + '</td>';

                    if (productosPedidoEspecial.Modelo[i].precioIndividual <= 0 && productosPedidoEspecial.Modelo[i].precioMenudeo <= 0) {
                        productosPedidoEspecial.Modelo[i].activo = false;
                        html += '<td><div class="badge badge-danger badge-shadow">No tiene configurado un precio</div></td>';
                    }
                    else if (productosPedidoEspecial.Modelo[i].precioIndividual <= 0) {
                        productosPedidoEspecial.Modelo[i].activo = false;
                        html += '<td><div class="badge badge-danger badge-shadow">Debe configurar el precio invidual del producto.</div></td>';
                    }
                    else if (productosPedidoEspecial.Modelo[i].precioMenudeo <= 0) {
                        productosPedidoEspecial.Modelo[i].activo = false;
                        html += '<td><div class="badge badge-danger badge-shadow">Debe configurar el precio Mayoreo del producto.</div></td>';
                    }
                    else if (parseInt(productosPedidoEspecial.Modelo[i].cantidad) === 0) {
                        productosPedidoEspecial.Modelo[i].activo = false;
                        html += '<td><div class="badge badge-danger badge-shadow">Sin existencias en el inventario</div></td>';
                    }
                    else if (productosPedidoEspecial.Modelo[i].cantidadRecibida > productosPedidoEspecial.Modelo[i].cantidad)
                        html += '<td><div class="badge badge-warning badge-shadow">Cantidad en existencia menor a la solicitada</div></td>';
                    else
                        html += '<td><div class="badge badge-success badge-shadow">Correcto</div></td>';

                    html += '</tr>';

                }

                $("#tblProductosPedidoEspecial tbody").html(html);
                tblProductosPedidoEspecial = initDataTable("tblProductosPedidoEspecial");
                $("#tblProductosPedidoEspecial").show();
                $("#btnAgregarPedidoEspecial").show();


            } else {
                MuestraToast('error', data.Mensaje);
                LimpiarFormPedidoEspecial();
            }
            OcultarLoader();
        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast('error', 'Ocurrio un error al consultar el pedido especial');
        }
    });
}

function AgregarPedidoEspecial() {

    var i, totalProductosAgregados = 0;
    for (i = 0; i < productosPedidoEspecial.Modelo.length; i++) {
        if (productosPedidoEspecial.Modelo[i].activo) {
            AgregarProducto(productosPedidoEspecial.Modelo[i], (productosPedidoEspecial.Modelo[i].cantidad < productosPedidoEspecial.Modelo[i].cantidadRecibida ? productosPedidoEspecial.Modelo[i].cantidad : productosPedidoEspecial.Modelo[i].cantidadRecibida))
            totalProductosAgregados = totalProductosAgregados + 1;
        }
    }

    if (totalProductosAgregados > 0) {

        idPedidoEspecial = $("#NoPedidoEspecial").val();
        actualizaTicketVenta();
        initInputsTabla();
        $('#ModalPedidoEspecial').modal('hide');
    }
    else {
        MuestraToast("error", "No existen productos válidos para agregar a la venta");
    }



}

function LimpiarFormPedidoEspecial() {

    if (tblProductosPedidoEspecial != null && tblProductosPedidoEspecial != undefined) {
        tblProductosPedidoEspecial.destroy();
        tblProductosPedidoEspecial = null;
    }
    $("#tblProductosPedidoEspecial tbody").html("");
    $("#tblProductosPedidoEspecial").hide();
    $("#btnAgregarPedidoEspecial").hide();
    productosPedidoEspecial = '';

}

//Ingreso Efectivo
function AbrirModalIngresoEfectivo() {
    $.ajax({
        url: rootUrl("/Ventas/_IngresoEfectivo"),
        data: { idTipoIngresoEfectivo: 2 },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $("#viewIngresoEfectivo").html(data);
            $('#ModalIngresoEfectivo').modal({ backdrop: 'static', keyboard: false, show: true });
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar mostrar la vista de ingreso de efectivo');
            console.log(xhr);
            console.log(status);
        }
    });
}

function onBeginSubmitIngresoEfectivo() {
    ShowLoader();
}
function onCompleteSubmitIngresoEfectivo() {
    OcultarLoader();
}
function onSuccessResultIngresoEfectivo(data) {
    OcultarLoader();
    if (data.status === 200) {
        MuestraToast('success', data.Mensaje);
        ImprimeTicketIngresoEfectivo(data.id);
        $('#ModalIngresoEfectivo').modal('hide');
        ConsultExcesoEfectivo();
    }
    else {
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

//Complemento de ticket de venta

$("#codigoBarrasTicketVenta").keypress(function (evt) {
    if (evt.which == 13) {
        BuscarVentaCodigoBarras();
    }
});

function AbrirModalComplementoVenta() {
    $("#codigoBarrasTicketVenta").val("");
    $('#ModalComplementoVenta').modal({ backdrop: 'static', keyboard: false, show: true });
}

function BuscarVentaCodigoBarras() {
    $.ajax({
        url: rootUrl("/Ventas/BuscaVentaCodigoBarras"),
        data: { codigoBarras: $("#codigoBarrasTicketVenta").val() },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            if (data.Estatus == 200) {
                $("#actionVenta").html("<a class='btn btn-primary' style='cursor:default'>Complemento del ticket " + $("#codigoBarrasTicketVenta").val() + "</a>");
                arrayProductosVentaComplemento = data.Modelo;
                idVentaComplemento = data.Modelo[0].idVenta;
                $('#ModalComplementoVenta').modal('hide');
                actualizaTicketVenta();
            }
            else {
                MuestraToast("error", data.Mensaje);
            }

        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast("error", "Hubo un problema pongase en contacto con el administrador del sistema");
        }
    });
}

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip()
});


