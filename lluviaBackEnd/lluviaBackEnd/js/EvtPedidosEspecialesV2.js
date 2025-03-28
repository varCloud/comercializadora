﻿var table;
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
var PuedeRealizarVenta = true;

function ValidarAperturaCaja() {
    return true;
}

$("#idAlmacenExistencia").on("change", function () {
    ActualizarProductosAlmacen();        
});

function ActualizarProductosAlmacen() {

    var idAlmacenExistencia = parseInt($('#idAlmacenExistencia').val());

    if ((idAlmacenExistencia == null) || (idAlmacenExistencia == 0) || (Number.isNaN(idAlmacenExistencia))) {
        $("#listProductos").val('');
        MuestraToast('info', "Debe seleccionar un almacen para consultar sus existencias");
        //$("#listProductos").html('').select2();
    }
    else {
        InitSelect2Productos(idAlmacenExistencia);
    }

}

function InitSelect2Productos(idAlmacen) {
    MuestraToast('info', "Actualizando productos del almacen");
    $("#listProductos").val('');
    
    var result = '';
    $.ajax({
        url: rootUrl("/Productos/ObtenerProductosPorAlmacen"),
        data: { idAlmacen: idAlmacen },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Consultando productos de almacen.");
        },
        success: function (data) {
            result = data;
            OcultarLoader();

            //
            //console.log(idAlmacen);
            //console.log(result);
            //arrayProductos = [];
            //arrayProductos = result.Modelo;

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

        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });

}

function InitarrayProductos() {
    
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


$('#btnAprobarPrecioMayoreo').click(function (e) {    
    ModalAutorizarPrecioMayoreo();
});

function ModalAutorizarPrecioMayoreo() {
    $("#usuarioAutoriza").val("");
    $("#contrasenaAutoriza").val("");
    $("#idPedidoEspecialMayoreo").val("");
    $("#usuarioAutorizaPrecioMayoreo").val("");
    $("#contrasenaAutorizaPrecioMayoreo").val("");

    $("#_articulos").html("");
    $("#_total").html("");
    $("#_cliente").html("");
    $('#ModalAutorizarPrecioMayoreo').modal({ backdrop: 'static', keyboard: false, show: true });
}

$('#btnAgregarProducto').click(function (e) {

    if (AgregarProducto(producto_value, $('#cantidad').val())) {

        //Agregar envase
        //if (parseInt(producto_value.idLineaProducto) === 20) {
        //    $('#ModalAgregarEnvase').modal({ backdrop: 'static', keyboard: false, show: true });
        //}

        $('#cantidad').val('1');
        $("#listProductos").val('');
        producto_value = null;
        $("#listProductos").focus();

        actualizaTicketVenta();
        initInputsTabla();
    }
});

function AgregarProducto(producto, cantidad, cotizacion) {
    //console.log(producto);
    //var esAgregarProductos = $('#esAgregarProductos').val();
    var cantidadCotizada = parseFloat(0);
    var leyenda = "";
    if (cotizacion == true) {
        cantidadCotizada = producto.cantidad;
        producto.cantidad = producto.cantidadActualInvAlmacen;

        if (cantidadCotizada > producto.cantidad)
            leyenda = "<div class='badge badge-danger badge-shadow'>Cantidad cotizada " + cantidadCotizada + "</div>";
        else
            leyenda = "<div class='badge badge-success badge-shadow'>Cantidad de inventario actual " + producto.cantidad + "</div>";
    }

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
            //si el producto ya esta en la tabla
            console.log(producto.idAlmacen );
            console.log(tblVtas.rows[i].cells[9].innerHTML);
                if  (
                        ( producto.idProducto === parseFloat(tblVtas.rows[i].cells[1].innerHTML) ) &&
                        ( producto.idAlmacen  === parseFloat(tblVtas.rows[i].cells[9].innerHTML)  ) 
                    ) 
                {
                    var cantidad = parseFloat(tblVtas.rows[i].cells[5].children[0].value) + cantidad;

                    if (cantidad > producto.cantidad) {
                        MuestraToast('warning', "No existe suficiente producto en inventario.");
                        return false;
                    }
                    tblVtas.rows[i].cells[5].children[0].value = cantidad;
                    existeProducto = true;
                }
        }
    }


    if ((!existeProducto) && (!existeProductoAgregar)) {
        // si todo bien    
        var row_ = "<tr>" +
            "  <td>1</td>" +
            "  <td> " + producto.idProducto + "</td>" +
            "  <td> " + producto.descripcion + "</td>" +
            "  <td> " + producto.Almacen + "</td>" +
            "  <td class=\"text-center\">$" + precio + "</td>";

        if (
            (producto.idLineaProducto == 12) ||  // si son liquidos
            (producto.idLineaProducto == 20) ||
            (producto.idLineaProducto == 22) ||
            (producto.idLineaProducto == 25)
        ) {
            row_ += "  <td class=\"text-center\"><input type='text' onkeypress=\"return esDecimal(this, event);\" style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + cantidad + "\">" + leyenda + "</td>";
        }
        else {
            row_ += "  <td class=\"text-center\"><input type='text' onkeypress=\"return esNumero(event)\" style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + cantidad + "\">" + leyenda + "</td>";
        }

        row_ +=
            "  <td class=\"text-center\">$" + precio + "</td>" +
            "  <td class=\"text-center\">$" + descuento + "</td>" +
            "  <td class=\"text-center\">" + btnEliminaFila +
            "  <td style=\"display: none;\">" + producto.idAlmacen + "</td>" +
            //"  <td style=\"display: ;\" >"+producto.ultimoCostoCompra+"</td>" +
            "  </td>" +
            "</tr >";

        $("#tablaRepVentas tbody").prepend(row_);
    }

    return true;
}

function actualizaTicketVenta() {

    var idPedidoEspecialMayoreo_ = parseInt($('#idPedidoEspecialMayoreo_').val());
    //console.log("id_"+idPedidoEspecialMayoreo_);

    // acttualizamos el id y la funcion de eliminar fila
    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;

        if ((!fila.children[8].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[8].getAttribute("class").includes('esDevolucion'))) {
            fila.children[8].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
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
                cantidad: parseFloat(tblVtas.rows[i].cells[5].children[0].value),
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
                        cantidad: parseFloat(arrayProductosVentaComplemento[c].cantidad),
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
    var cantidadDeProductos = parseFloat(0);
    //console.log(arrayPreciosRangos);

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
        if ( (cantidadDeProductos >= 6) || ( parseInt(idPedidoEspecialMayoreo_) > 0 ) ) {
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

        var tblVtas = document.getElementById('tablaRepVentas');
        var rCount = tblVtas.rows.length;

        if (rCount >= 2) {
            for (var i = 1; i < rCount; i++) {

                var cantidad = parseFloat(tblVtas.rows[i].cells[5].children[0].value);

                if ((!tblVtas.rows[i].cells[8].getAttribute("class").includes('esDevolucion')) && (!tblVtas.rows[i].cells[8].getAttribute("class").includes('esAgregarProductos'))) {
                    //console.log(tblVtas.rows[i].cells[7].getAttribute("class"));

                    if ((parseInt(tblVtas.rows[i].cells[1].innerHTML)) === (parseInt(productos[j].idProducto))) {
                        tblVtas.rows[i].cells[4].innerHTML = "$" + parseFloat(productos[j].precioVenta).toFixed(2);   //precio
                        tblVtas.rows[i].cells[6].innerHTML = "$" + (parseFloat(productos[j].precioVenta) * cantidad).toFixed(2);   //total
                        tblVtas.rows[i].cells[7].innerHTML = "$" + (parseFloat(productos[j].precioIndividual - productos[j].precioVenta) * cantidad).toFixed(2);  //descuento
                    }
                }
            }
        }
    }

    actualizarSubTotal();

}

$('#btnGuardarPedidoEspecial').click(function (e) {
    
    console.log("#idPedidoEspecial", $('#idPedidoEspecial').val());
    abrirModalGuardarPedidoEspecial(1);
   
});

function abrirModalGuardarPedidoEspecial(tipo) {

    //limpiaModalPrevio();
    document.getElementById("chkImprimePrevio").checked = false;
    $('#divCotizar').css('display', 'none');
    $('#buttonCerrar').css('display', 'block');
    $('#divTipoRevision').css('display', 'block');
    $('#TituloModalPedidoEspecial').html('Generar Pedido Especial');

    if ($('#idClienteCotizacion').val() > 0)
        $('#idClienteCotizacion').val("1").trigger('change');
    else
        $('#idCliente').val("1").trigger('change');

    var total = parseFloat(0);
    //var descuento = parseFloat(0);
    //ultimoCambio = parseFloat(document.getElementById("ultimoCambio").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        //if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
            total += parseFloat(fila.children[6].innerHTML.replace('$', ''));
            //descuento += parseFloat(fila.children[7].innerHTML.replace('$', ''));
        //}
    });
    //console.log(total);
    if (total > 0) {

    
        if (tipo === 2) { //cotizacion
            if ($('#idPedidoEspecial').val() > 0) {
                $('#TituloModalPedidoEspecial').html('Guardar cambios cotizaciòn');
            }
            else {
                $('#TituloModalPedidoEspecial').html('Generar Cotizaciòn');
            }
            $('#divCotizar').css('display', 'block');
            $('#buttonCerrar').css('display', 'none');
            $('#divTipoRevision').css('display', 'none');
            
        }
        $('#ModalGuardarPedidoEspecial').modal({ backdrop: 'static', keyboard: false, show: true });
    }
    else {
        MuestraToast('warning', "Debe tener productos agregados para continuar con el alta del pedido especial.");
    }

}

//revision por ticket
$('#btnRevisionPorTicket').click(function (e) {

    swal({
        title: 'Mensaje',
        text: '¿Esta seguro que desea hacer la revisión por ticket?',
        icon: 'info',
        buttons: ["No", "Sí"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                GuardarPedidoEspecial(1,1); // por ticket
            } else {
                console.log("cancelar");
            }
        });    

});


//revision por handHeld
$('#btnRevisionPorHandHeld').click(function (e) {

    swal({
        title: 'Mensaje',
        text: '¿Esta seguro que desea hacer la revisión por Hand Held?',
        icon: 'info',
        buttons: ["No", "Sí"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                GuardarPedidoEspecial(2,1); // por hand held
            } else {
                console.log("cancelar");
            }
        });    

});

// cotizaciones
$('#btnCotizar').click(function (e) {

    swal({
        title: 'Mensaje',
        text: $("#idPedidoEspecial").val() > 0 ? '¿Esta seguro que desea guardar los cambios de esta cotización?' :'¿Esta seguro que desea hacer guardar este pedido especial como una cotización?',
        icon: 'info',
        buttons: ["No", "Sí"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                //console.log(willDelete);
                GuardarPedidoEspecial(3, 2); // cotizacion
            } else {
                console.log("cancelar");
            }
        });

});


function GuardarPedidoEspecial(tipoRevision, idEstatusPedidoEspecial ) { // 1-Ticket   /  2-Hand Held

    var idPedidoEspecialMayoreo_ = parseInt($('#idPedidoEspecialMayoreo_').val());
    $("#btnGuardarVenta").addClass('btn-progress disabled');

    var productos = [];
    var idCliente = $('#idCliente').val();
    var idPedidoEspecial = $('#idPedidoEspecial').val();
    var copias = parseInt(1);

    if (tipoRevision == 2) //revision por handHeld
    {
        copias = parseInt(2);
    }

    // si todo bien
    var tblVtas = document.getElementById('tablaRepVentas');
    var rCount = tblVtas.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var row_ = {
                idProducto: parseInt(tblVtas.rows[i].cells[1].innerHTML),
                cantidad: parseFloat(tblVtas.rows[i].cells[5].children[0].value),
                idAlmacen: parseInt(tblVtas.rows[i].cells[9].innerHTML),
            };
            productos.push(row_);
        }
    }

   
    dataToPost = JSON.stringify({ productos: productos, tipoRevision: tipoRevision, idCliente: idCliente, idEstatusPedidoEspecial: idEstatusPedidoEspecial, idPedidoEspecial: idPedidoEspecial, idPedidoEspecialMayoreo_: idPedidoEspecialMayoreo_});

    $.ajax({
        
        url: rootUrl("/PedidosEspecialesV2/GuardarPedidoEspecial"),
        data: dataToPost,
        method: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Guardando Pedido Especial.");
            $("#btnRevisionPorTicket").addClass('btn-progress disabled');
            $("#btnRevisionPorHandHeld").addClass('btn-progress disabled');
        },
        success: function (data) {
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            productos = [];
            if (data.Estatus == 200) {
                if ($("#chkImprimePrevio").is(":checked")) {
                    ImprimeTicketPedidoEspecial(data.Modelo.idPedidoEspecial, 1, 0, false)
                }
                imprimirTicketAlmacenes(data.Modelo.idPedidoEspecial, copias);
                ActualizarProductosAlmacen();
                limpiarTicket();
                $("#listProductos").focus();
            }
            clearQueryParams();
            $('#idPedidoEspecial').val(0);
            $('#ModalGuardarPedidoEspecial').modal('hide');            
            $("#btnRevisionPorTicket").removeClass('btn-progress disabled');
            $("#btnRevisionPorHandHeld").removeClass('btn-progress disabled');

        },
        error: function (xhr, status) {
            OcultarLoader();            
            $("#btnRevisionPorTicket").removeClass('btn-progress disabled');
            $("#btnRevisionPorHandHeld").removeClass('btn-progress disabled');
            console.log('Hubo un problema al guardar el Pedido Especial, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });



}


function ConsultaExistenciasAlmacen( idProducto, idAlmacen ) { 

    var dataToPost = JSON.stringify({ idProducto: idProducto, idAlmacen: idAlmacen });
    var result = [];

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ConsultaExistenciasAlmacen"),
        data: dataToPost,
        method: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        async: false,
        beforeSend: function (xhr) {
            ShowLoader("Consultando Existencias...");
        },
        success: function (data) {
            OcultarLoader();
            
            if (data.Estatus == 200) {
                result = data.Modelo;
            }
            else {
                MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            }

        },
        error: function (xhr, status) {
            OcultarLoader();            
            console.log('Hubo un problema al consultar la existencia del producto, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
    
    return result;

}


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

//$('#cancelar').click(function (e) {
//    document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(ultimoCambio).toFixed(2) + "</h4>";
//    limpiaModalPrevio();
//});

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
    $("#btnRevisionPorTicket").removeClass('btn-progress disabled');
    $("#btnRevisionPorHandHeld").removeClass('btn-progress disabled');

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



//$('#previoVenta').click(function (e) {

//    var esDevolucion = $('#esDevolucion').val();

//    if ((esDevolucion == "true") || (esDevolucion == "True")) {
//        // validamos que al menos exista devolucion de un item
//        var tblVtas = document.getElementById('tablaRepVentas');
//        var rCount = tblVtas.rows.length;
//        var productosOriginales = parseFloat(0);
//        var productosDevueltos = parseFloat(0);

//        if (rCount >= 2) {
//            for (var i = 1; i < rCount; i++) {
//                productosDevueltos += parseFloat(tblVtas.rows[i].cells[7].children[0].value);
//                productosOriginales += parseFloat(tblVtas.rows[i].cells[4].children[0].value);
//            }
//        }

//        if (productosDevueltos <= 0) {
//            MuestraToast('warning', "Debe seleccionar al menos un producto para devolver.");
//            return;
//        }

//        if (productosDevueltos >= productosOriginales) {
//            MuestraToast('warning', "Para devolver todos los productos cancele la venta desde el menu de Editar Ventas");
//            return;
//        }


//        $('#motivoDevolucion').val('');
//        $('#ModalDevolucion').modal({ backdrop: 'static', keyboard: false, show: true });
//    }
//    else {
//        abrirModalPrevioVenta();
//    }
//});

//function abrirModalPrevioVenta() {

//    limpiaModalPrevio();

//    var total = parseFloat(0);
//    var descuento = parseFloat(0);
//    ultimoCambio = parseFloat(document.getElementById("ultimoCambio").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);


//    $('#tablaRepVentas tbody tr').each(function (index, fila) {

//        if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
//            total += parseFloat(fila.children[5].innerHTML.replace('$', ''));
//            descuento += parseFloat(fila.children[6].innerHTML.replace('$', ''));
//        }

//    });

//    if (total > 0) {
//        document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(total + descuento).toFixed(2) + "</h4>";
//        document.getElementById("previoDescuentoMenudeo").innerHTML = "<h4>$" + parseFloat(descuento).toFixed(2) + "</h4>";
//        document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(total + descuento - descuento).toFixed(2) + "</h4>";
//        document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(total + descuento - descuento).toFixed(2) + "</h4>";
//        $('#ModalPrevioVenta').modal({ backdrop: 'static', keyboard: false, show: true });
//    }
//    else {
//        MuestraToast('warning', "Debe tener productos agregados para continuar con la venta.");
//    }

//}



//$('#btnAceptarDevolucion').click(function (e) {


//    if (($('#motivoDevolucion').val() == "")) {
//        MuestraToast('warning', "Debe seleccionar el motivo de la devolución");
//        return;
//    }

//    $('#ModalDevolucion').modal('hide');
//    //abrirModalPrevioVenta();
//    document.getElementById("btnGuardarVenta").click();

//});



//$('#btnAgregarProducto').click(function (e) {

//    //if (AgregarProducto($('#idProducto').select2('data')[0], $('#cantidad').val())) {
//    //    $('#cantidad').val('');

//    //    Agregar envase
//    //    if (parseInt($('#idProducto').select2('data')[0].idLineaProducto) === 20) {
//    //        $('#ModalAgregarEnvase').modal({ backdrop: 'static', keyboard: false, show: true });
//    //    }

//    //    actualizaTicketVenta();
//    //    initInputsTabla();
//    //}

//    if (AgregarProducto(producto_value, $('#cantidad').val())) {

//        //Agregar envase
//        //if (parseInt(producto_value.idLineaProducto) === 20) {
//        //    $('#ModalAgregarEnvase').modal({ backdrop: 'static', keyboard: false, show: true });
//        //}

//        $('#cantidad').val('1');
//        $("#listProductos").val('');
//        producto_value = null;
//        $("#listProductos").focus();

//        actualizaTicketVenta();
//        initInputsTabla();
//    }
//});

$('#btnAgregarEnvase').click(function (e) {
    if (AgregarProducto($('#idProductoEnvase').select2('data')[0], $('#cantidadEnvase').val())) {
        $('#cantidadEnvase').val('');
        $('#idProductoEnvase').val("0").trigger('change');
        $('#ModalAgregarEnvase').modal('hide');
        actualizaTicketVenta();
        initInputsTabla();
    }
});

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
        var idAlmacen = parseInt(tblVtas.rows[rowIndex].cells[9].innerHTML);
        var producto = arrayProductos.find(x => x.idProducto == idProducto);
        var productoAlmacen = [];
        productoAlmacen = ConsultaExistenciasAlmacen(idProducto, idAlmacen);
        var cantidad = productoAlmacen.find(x => x.idProducto === idProducto).cantidad;

        if ((parseFloat(thisInput.val()) > (cantidad))) {
            MuestraToast('warning', "No existe suficiente producto en inventario.");
            document.execCommand('undo');
            return;
        }

        if ((thisInput.val() == "") || (thisInput.val() == "0")) {
            MuestraToast('warning', mensaje);
            document.execCommand('undo');
        }

        actualizaTicketVenta();
        $("#listProductos").focus();

    });
}

function cuentaSubTotal() {
    //var result = parseFloat(0);
    var subTotal = parseFloat(0);
    $('#tablaRepVentas tbody tr').each(function (index, fila) {

        if ((!fila.children[7].getAttribute("class").includes('esAgregarProductos')) && (!fila.children[7].getAttribute("class").includes('esDevolucion'))) {
            subTotal += parseFloat(fila.children[6].innerHTML.replace('$', ''));
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

    //document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(subTotal).toFixed(2) + "</h4>";
    $(".divSubTotal").html("$" + parseFloat(subTotal).toFixed(2));
}

//function actualizarSubTotalDevoluciones() {

//    var tblVtas = document.getElementById('tablaRepVentas');
//    var rCount = tblVtas.rows.length;
//    var cantidadDevelta = parseFloat(0);
//    var comisionBancariaDevuelta = parseFloat(0);

//    if (rCount >= 2) {
//        for (var i = 1; i < rCount; i++) {
//            cantidadDevelta += parseFloat(tblVtas.rows[i].cells[7].children[0].value) * parseFloat(tblVtas.rows[i].cells[3].innerHTML.replace('$', '')); //parseFloat(fila.children[5].innerHTML.replace('$', ''));
//            //                                              cantidad devuelta                                   monto comision                              cantidad restante
//            comisionBancariaDevuelta += (parseFloat(tblVtas.rows[i].cells[7].children[0].value)) * ((parseFloat(tblVtas.rows[i].cells[9].innerHTML)) / (parseFloat(tblVtas.rows[i].cells[4].children[0].value)))
//            console.log(comisionBancariaDevuelta);
//        }
//    }

//    //document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(cantidadDevelta).toFixed(2) + "</h4>";
//    $(".divSubTotal").html("$" + parseFloat(cantidadDevelta + comisionBancariaDevuelta).toFixed(2));
//    document.getElementById("divTotalDevolver").innerHTML = "<h4>$" + parseFloat(cantidadDevelta + comisionBancariaDevuelta).toFixed(2) + "</h4>";
//}


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

function initTimer() {
    var timer = setTimeout(function () {
        $("#btnGuardarVenta").removeClass('btn-progress disabled');
        PuedeRealizarVenta = true;
        console.log('timer');
        clearInterval(timer);
    }, 100);
}

function ImprimeTicketPedidoEspecial(idPedidoEspecial, idTipoTicketPedidoEspecial, idTicketPedidoEspecial, ticketFinal) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ImprimeTicket"),
        data: { idPedidoEspecial: idPedidoEspecial, idTipoTicketPedidoEspecial: idTipoTicketPedidoEspecial, idTicketPedidoEspecial: idTicketPedidoEspecial, ticketFinal: ticketFinal },
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
    calculaTotales('true');
});


$("#formaPago").on("change", function (value) {
    this.value == 1 ? $('#dvEfectivo').css('display', '') : $('#dvEfectivo').css('display', 'none');
    calculaTotales('true');
});

function calculaTotales(conReseteoCampos) {

    if (conReseteoCampos === 'true') {
        $('#efectivo').val('');
        document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
        //document.getElementById("ultimoCambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";    
        document.getElementById("chkFacturar").checked = false;
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
        (!$("#chkFacturar").is(":checked"))       // y si la venta no es facturada
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


function RequiereAutorizacion() {

    var requiereAutorizacion = true;

    $.ajax({
        url: rootUrl("/Ventas/ObtenerConfiguracionVentas"),
        data: { tipoConfigVentas: 2 },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            //console.log(data);
            if (parseInt(data.Modelo.valor) !== parseInt(1)) {
                requiereAutorizacion = false;
            }

            OcultarLoader();

        },
        error: function (xhr, status) {
            console.log('Hubo un problema al intentar hacer el cierre de esta estación, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });

    return requiereAutorizacion;
}


$("#usuarioAutoriza").on("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("btnAutorizarCierre").click();
    }
});

$("#contrasenaAutoriza").on("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("btnAutorizarCierre").click();
    }
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




$(document).ready(function () {
    InitarrayProductos();
    //ValidaAperturaCajas();
    arrayPreciosRangos = ObtenerPrecios_(0);
    //InitSelect2Productos();
    $('#idAlmacenExistencia').val("4").trigger('change');
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

    if ($("#idPedidoEspecial").val() > 0) {

        AgregarProductosPedidoEspecial();

    }

});

function AgregarProductosPedidoEspecial() {

        $.ajax({
            url: rootUrl("/PedidosEspecialesV2/ObtenerProductosPedidoEspecial"),
            data: { idPedidoEspecial: $("#idPedidoEspecial").val() },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (productosPedidoEspecial) {
            
            var i, totalProductosAgregados = 0;
            for (i = 0; i < productosPedidoEspecial.length; i++) {
                var cantidad = productosPedidoEspecial[i].cantidad > productosPedidoEspecial[i].cantidadActualInvAlmacen ? productosPedidoEspecial[i].cantidadActualInvAlmacen : productosPedidoEspecial[i].cantidad;
                AgregarProducto(productosPedidoEspecial[i], cantidad, true);
                    totalProductosAgregados = totalProductosAgregados + 1;
                
            }

            if (totalProductosAgregados > 0) {               
                actualizaTicketVenta();
                initInputsTabla();              
            }
            else {
                MuestraToast("error", "No existen productos válidos para agregar al pedido especial");
            }

            OcultarLoader();
        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast('error', 'Ocurrio un error al consultar el pedido especial');
        }
    });





}

////*********************** PEDIDO ESPECIAL  ************************************
//Complemento de ticket de venta

$("#codigoBarrasTicketVenta").keypress(function (evt) {
    if (evt.which == 13) {
        BuscarVentaCodigoBarras();
    }
});


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

$("#idPedidoEspecialMayoreo").on("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("btnConsultaTicketMayoreo").click();
    }
});

$("#contrasenaAutorizaPrecioMayoreo").on("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("btnAutorizarTicketMayoreo").click();
    }
});


$('#btnAutorizarTicketMayoreo').click(function (e) {

    var usuario = $('#usuarioAutorizaPrecioMayoreo').val();
    var contrasena = $('#contrasenaAutorizaPrecioMayoreo').val();
    var idPedidoEspecialMayoreo = $('#idPedidoEspecialMayoreo').val();

    var _articulos = document.getElementById("_articulos").innerHTML;
    var _total = document.getElementById("_total").innerHTML;
    var _cliente = document.getElementById("_cliente").innerHTML;


    if ((usuario === '') || (contrasena === '')) {
        MuestraToast("error", "Debe ingresar Usuario y Contraseña para autorizar el precio de mayoreo .");
        return;
    }

    if  (
            (idPedidoEspecialMayoreo === '') ||
            (_articulos === '') ||
            (_total === '') ||
            (_cliente === '') 
        ) {
        MuestraToast("error", "Debe consultar un pedido especial válido para autorizar el precio de mayoreo .");
        return;
    }



    $.ajax({
        url: rootUrl("/Ventas/ValidarContrasena"),
        data: { usuario: usuario, contrasena: contrasena },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Validando");
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);

            if (data.Estatus === 200) {

                swal({
                    title: 'Mensaje',
                    text: '¿Esta seguro que desea autorizar el ticket a precio de mayoreo?',
                    icon: 'info',
                    buttons: ["No", "Sí"],
                    dangerMode: true,
                })
                    .then((willDelete) => {
                        if (willDelete) {

                            $('#ModalAutorizarPrecioMayoreo').modal('hide');
                            $('#idPedidoEspecialMayoreo_').val($('#idPedidoEspecialMayoreo').val()); // $('#idPedidoEspecialMayoreo').val();
                            document.getElementById("divIdPedidoMayoreo").innerHTML = "Autorizado con Pedido #: <span> " + $('#idPedidoEspecialMayoreo').val() + " </span>";
                            $('#divIdPedidoMayoreo').css('display', '');
                            actualizaTicketVenta();
                        } else {
                            console.log("cancelar");
                            //$('#divIdPedidoMayoreo').css('display', 'none');
                        }
                    });    

            }

            OcultarLoader();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });

});

$('#btnConsultaTicketMayoreo').click(function (e) {

    var pedido;
    var idPedidoEspecial = parseInt(0);

    $("#_articulos").html("");
    $("#_total").html("");
    $("#_cliente").html("");

    // validaciones
    if  (
         ($('#idPedidoEspecialMayoreo').val() == "") ||
         ($('#idPedidoEspecialMayoreo').val() == "0") 
        ) {
        MuestraToast('warning', "Debe escribir el # de ticket de mayoreo.");
        return;
    }

    idPedidoEspecial = parseInt($('#idPedidoEspecialMayoreo').val());    
    pedido = ConsultaDatosTicketPedidoEspecialV2(idPedidoEspecial);
    //console.log(pedido);


    if (pedido.Estatus == "200") {

        if (
            (parseInt(pedido.Modelo[0].idEstatusPedidoEspecial) == 4) ||
            (parseInt(pedido.Modelo[0].idEstatusPedidoEspecial) == 5) ||
            (parseInt(pedido.Modelo[0].idEstatusPedidoEspecial) == 6) ||
            (parseInt(pedido.Modelo[0].idEstatusPedidoEspecial) == 7)

        ) {

            if (parseInt(pedido.Modelo[0].cantidad) >= 6) {
                $("#_articulos").html(pedido.Modelo[0].cantidad);
                $("#_total").html("$" + pedido.Modelo[0].montoTotal);
                $("#_cliente").html(pedido.Modelo[0].nombreCliente);
            }
            else {
                MuestraToast('warning', "El ticket no contiene al menos 6 productos.");
                $("#_articulos").html("");
                $("#_total").html("");
                $("#_cliente").html("");
            }
        }
        else {
            MuestraToast('warning', "El ticket tiene que estar en estatus de entregado.");
            $("#_articulos").html("");
            $("#_total").html("");
            $("#_cliente").html("");
        }


    }
    else {
        MuestraToast('warning', pedido.Mensaje);
    }

});

function ConsultaDatosTicketPedidoEspecialV2(idPedidoEspecial) {

    var result = [];
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ConsultaDatosTicketPedidoEspecialV2"),
        data: { idPedidoEspecial: idPedidoEspecial },
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


$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip();
    $('#divIdPedidoMayoreo').css('display', 'none');

    //$("#btnTicket").click(function (evt) {
    //    consultarTicketPedidoEspecial();
    //});
    //$('#ModalPrevioVenta').on('shown.bs.modal', function () {
    //    PuedeRealizarVenta = true;
    //    console.log("puede realizar venta", PuedeRealizarVenta)
    //});


});


function imprimirTicketAlmacenes(idPedidoEspecial, copias) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/imprimirTicketAlmacenes"),
        data: { idPedidoEspecial: idPedidoEspecial, copias: copias },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            MuestraToast("info", data.Mensaje);
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
        }
    });
}

//cotizaciones
$('#btnGeneraCotizacion').click(function (e) {
    abrirModalGuardarPedidoEspecial(2);
});

