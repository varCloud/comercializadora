var table;
var iframe;
var tablaVentas; 

//busqueda
function onBeginSubmitVentas() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitVentas() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultVentas(data) {
    console.log("onSuccessResultVentas", JSON.stringify(data) );    
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
        icon: 'warning',
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

function facturaVenta(idVenta) {
    console.log(idVenta);
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

function InitSelect2Productos() {
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
    limpiaModalPrevio();
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
    $('#cantidad').val('');
    $('#idProducto').val("0").trigger('change');
    $('#idVenta').val(0);
    $('#vaConDescuento').val(0);

}

function limpiaModalPrevio() {

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
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    $('#efectivo').val('');
    $('#idCliente').val("0").trigger('change');
    $('#formaPago').val("1").trigger('change'); 

}



$('#previoVenta').click(function (e) {

    limpiaModalPrevio();

    var total = parseFloat(0);
    var descuento = parseFloat(0);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        total += parseFloat(fila.children[5].innerHTML.replace('$', ''));
        descuento += parseFloat(fila.children[6].innerHTML.replace('$', ''));
    });
    //console.log("total" + " " + total);
    //console.log("descuento" + " " + descuento);

    if (total > 0) {
        document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(total+descuento).toFixed(2) + "</h4>";
        document.getElementById("previoDescuentoMenudeo").innerHTML = "<h4>$" + parseFloat(descuento).toFixed(2) + "</h4>";
        document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(total + descuento - descuento).toFixed(2) + "</h4>";
        document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(total + descuento - descuento).toFixed(2) + "</h4>";
        $('#ModalPrevioVenta').modal({ backdrop: 'static', keyboard: false, show: true });
    }
    else {
        MuestraToast('warning', "Debe tener productos agregados para continuar con la venta.");
    }

    //document.getElementById("previoTotal").innerHTML = document.getElementById("divSubTotal").innerHTML;
    //document.getElementById("previoDescuento").innerHTML = "<h4>$" + parseFloat(0.0).toFixed(2) + "</h4>";
    //document.getElementById("previoSubTotal").innerHTML = document.getElementById("divSubTotal").innerHTML;
    //document.getElementById("previoIVA").innerHTML = document.getElementById("divIva").innerHTML;
    //document.getElementById("previoFinal").innerHTML = document.getElementById("divTotal").innerHTML;

});




$('#btnAgregarProducto').click(function (e) {

    if ($('#cantidad').val() == "") {
        MuestraToast('warning', "Debe escribir la cantidad de productos que va a agregar.");
    }
    else {

        //var precio = parseFloat($('#precio').val());
        var idProducto = $('#idProducto').val();
        var cantidad = $('#cantidad').val();
        var data = ObtenerProductoPorPrecio(idProducto, cantidad, $("#vaConDescuento").val());
        var precio = parseFloat(data.Modelo[0].costo);
        var descuento = parseFloat(data.Modelo[0].descuento);
        //console.log("desc_" + $("#vaConDescuento").val());
        if (precio == 0) {
            preguntaAltaPrecios();
        }
        else {
           // console.log($("#idProducto").find("option:selected").text());
            var row_ =  "<tr>" +
                        "  <td>1</td>" +
                        "  <td> " + $('#idProducto').val() + "</td>" +
                        "  <td> " + $("#idProducto").find("option:selected").text() + "</td>" +
                        "  <td class=\"text-center\">$" + precio + "</td>" +
                        //"  <td class=\"text-center\" onclick=\"listenerDobleClick(this);\"  onblur=\"this.contentEditable=false;\">" + cantidad + "</td>" +
                        "  <td class=\"text-center\" onclick=\"listenerDobleClick(this);\"  onblur=\"this.contentEditable=false;\"><input type='text' style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + cantidad +"\"></td>" +
                        "  <td class=\"text-center\">$" + cantidad * precio + "</td>" +
                        "  <td class=\"text-center\">$" + descuento + "</td>" +
                        "  <td class=\"text-center\">" +
                        "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
                        "  </td>" +
                        "</tr >";

            $("table tbody").append(row_);
            $('#cantidad').val('');

            actualizaTicketVenta();

            
            $('#tablaRepVentas input').on('change', function () {
                //alert("cambio");
                console.log("cambio");
                //console.log("change_name_val__" + $(this).name());
                //var theInput = $(this);
                ////theInput.setAttribute('value', '99'); 
                //document.querySelector('input[value="2"]').value = "99";
                //console.log("theInput_"+theInput.val());
                //console.log("change_n_val__" +  this.value);
                //console.log("change_new_val__" + $(this).val());

                //actualizaTicketVenta();
            });
        }

    }
    
});


function actualizaTicketVenta() {
    //alert();
    console.log("actualizaTicketVenta");
    var totalPiezas = parseFloat(0);
    var totalAhorro = parseFloat(0);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[7].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        totalPiezas += parseFloat(fila.children[4].innerHTML.replace('<input type="text" style="text-align: center; border: none; border-color: transparent;  background: transparent; " value="', '').replace('">', ''));
        totalAhorro += parseFloat(fila.children[6].innerHTML.replace('$', ''));
    });
    //console.log("totalPiezas" + " " + totalPiezas);
    //console.log("totalAhorro" + " " + totalAhorro);

    //actualizar los totales
    //document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";

    //totalPiezas = obtenerNumArticulos();

    if (totalPiezas >= 12) {
        //console.log("ya son 12 o+");
        // si hay 12 o mas piezas y todavia no se agrega el descuento al tikcet
        if (totalAhorro == 0) {
            agregarDescuentos();
            //$("#vaConDescuento").val("1");
        }
    }
    else {
        
        if (totalAhorro > 0) {
            //$("#vaConDescuento").val("0");
            quitarDescuentos();
        }
    }

    actualizarSubTotal();
}


function agregarDescuentos() {

    $("#vaConDescuento").val("1");

    $('#tablaRepVentas tbody tr').each(function (index, fila) {

        var idProducto = parseFloat(fila.children[1].innerHTML);
        var cantidad = parseFloat(fila.children[4].innerHTML.replace('<input type="text" style="text-align: center; border: none; border-color: transparent;  background: transparent; " value="', '').replace('">', '')); //parseFloat(fila.children[4].innerHTML);
        
        var data = ObtenerProductoPorPrecio(idProducto, cantidad, $("#vaConDescuento").val());
        var precio = parseFloat(data.Modelo[0].costo);

        if (precio == 0) {
            console.log("error_precio_" + precio);
        }
        else {
            fila.children[3].innerHTML = "$" + data.Modelo[0].costo;   //precio
            fila.children[5].innerHTML = "$" + parseFloat(data.Modelo[0].costo) * cantidad;   //total
            fila.children[6].innerHTML = "$" + parseFloat(data.Modelo[0].descuento);  //descuento

        }
        
    });

}


function quitarDescuentos() {

    $("#vaConDescuento").val("0");

    $('#tablaRepVentas tbody tr').each(function (index, fila) {

        var idProducto = parseFloat(fila.children[1].innerHTML);
        var cantidad = parseFloat(fila.children[4].innerHTML.replace('<input type="text" style="text-align: center; border: none; border-color: transparent;  background: transparent; " value="', '').replace('">', '')); //parseFloat(fila.children[4].innerHTML);

        var data = ObtenerProductoPorPrecio(idProducto, cantidad, $("#vaConDescuento").val());
        var precio = parseFloat(data.Modelo[0].costo);

        if (precio == 0) {
            console.log("error_precio_" + precio);
        }
        else {
            fila.children[3].innerHTML = "$" + data.Modelo[0].costo;   //precio
            fila.children[5].innerHTML = "$" + parseFloat(data.Modelo[0].costo) * cantidad;   //total
            fila.children[6].innerHTML = "$" + parseFloat(data.Modelo[0].descuento);  //descuento
        }
    });

}

function actualizarSubTotal() {

    var subTotal = parseFloat(0);
    var descuento = parseFloat(0);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        subTotal += parseFloat(fila.children[5].innerHTML.replace('$', ''));
        descuento += parseFloat(fila.children[6].innerHTML.replace('$', ''));
    });

    document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(subTotal).toFixed(2) + "</h4>";

}


function ObtenerProductoPorPrecio(idProducto, cantidad, vaConDescuento) {
    
    var result = '';
    $.ajax({
        url: rootUrl("/Ventas/ObtenerProductoPorPrecio"),
        data: { idProducto: idProducto, cantidad: cantidad, costo: 0, vaConDescuento: vaConDescuento},
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
    var result = "";//{ "Estatus": -1, "Mensaje": "Espere un momento y vuelva a intentarlo" };
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


$('#btnGuardarVenta').click(function (e) {

    var productos = [];

    $('#tablaRepVentas tbody tr').each(function (index, fila) {

        var row_ = {
            idCliente: $('#idCliente').val(),
            idProducto: fila.children[1].innerHTML,
            //cantidad: fila.children[4].innerHTML,
            cantidad: parseInt(fila.children[4].innerHTML.replace('<input type="text" style="text-align: center; border: none; border-color: transparent;  background: transparent; " value="', '').replace('">', '')),
            idUsuario: 4,//fila.children[3].innerHTML,
            formaPago: $('#formaPago').val(),
            usoCFDI: $('#usoCFDI').val(),
            idVenta: $('#idVenta').val(),
        };
        productos.push(row_);

    });

    dataToPost = JSON.stringify({ venta: productos });

    $.ajax({
        url: rootUrl("/Ventas/GuardarVenta"),
        data: dataToPost,
        method: 'POST',
        dataType: 'JSON',
        contentType: "application/json; charset=utf-8", 
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Guardando Venta.");
        },
        success: function (data) {
            OcultarLoader();
            MuestraToast('success', data.Mensaje);
            $('#ModalPrevioVenta').modal('hide');
            limpiarTicket();
            ImprimeTicket(data.Modelo.idVenta);
            if ($("#chkFacturar").is(":checked")) {
                console.log(" is checked!- facturar\n");
                facturaVenta(data.Modelo.idVenta);
            }
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al guardar la venta, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

});


$('#chkFacturar').click(function () {
    $('#efectivo').val('');
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
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
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            console.log(data);
            MuestraToast('success', "Se envio el ticket a la impresora.");
        },
        error: function (xhr, status) {
            MuestraToast('error', "Ocurrio un error al enviar el ticket a la impresora.");
            console.log(xhr);
            console.log(status);
            console.log(data);
        }
    });
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
             evt.keyCode == 13 ||
            (evt.keyCode > 31 && (evt.keyCode < 48 || evt.keyCode > 57))
           )
        {
            event.preventDefault();
            element.contentEditable = false;
        }
        else {       // si es un numero         
        }
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





$("#efectivo").on("keyup", function () {
    var cambio_     = parseFloat(0).toFixed(2);
    var efectivo_   = parseFloat($('#efectivo').val()).toFixed(2);
    var total_      = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

    if (parseFloat(efectivo_) > parseFloat(total_)) {
        cambio_ = efectivo_ - total_;
        document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>";
    }
    else {
        document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    }
});


$("#cantidad").on("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("btnAgregarProducto").click();
    }
});



$("#idCliente").on("change", function () {

    $('#efectivo').val('');
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    var idCliente = parseFloat($('#idCliente').val());
    var data = ObtenerCliente(idCliente);
    var nombre = data.Modelo.nombres + "  " + data.Modelo.apellidoPaterno + "  " + data.Modelo.apellidoMaterno;
    var descuento = parseFloat(0.0);

    if (idCliente != 0) {
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

    // si lleva iva
    if ($("#chkFacturar").is(":checked")) {
        console.log(" is checked!- facturar\n");
        iva = parseFloat(subTotal * 0.16).toFixed(2);
    }

    var final = (parseFloat(subTotal) + parseFloat(iva)).toFixed(2);

    document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + cantidadDescontada + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + subTotal + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + iva + "</h4>";
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

    if ((data.idCliente != 0) && (idCliente != 0)) {
        row_ = "<address>" +
            "    <strong>Datos del Cliente:</strong><br>" +
            "    Nombre: " + nombre.toUpperCase() + "<br>" +
            "    Telefono: " + data.Modelo.telefono + "<br>" +
            "    E-mail: " + data.Modelo.correo + "<br>" +
            "    RFC: " + data.Modelo.rfc + "<br>" +
            "    Tipo de Cliente: " + data.Modelo.tipoCliente.descripcion + "<br>" +
            "</address>";
    }


    document.getElementById("nombreCliente").innerHTML = row_;

}); 




function AbrirModalConsultaExistencias() {

    //$('#btnGuardarProducto').prop('disabled', true);

    //var data = ObtenerProducto(idProducto);

    //$('#idProducto').val(idProducto);
    //$('#activo').val(data.activo);
    //$('#descripcion').val(data.descripcion).prop('disabled', true);
    //$('#idUnidadMedida').val(data.idUnidadMedida).prop('disabled', true);
    //$('#idLineaProducto').val(data.idLineaProducto).prop('disabled', true);
    //$('#claveProdServ').val(data.claveProdServ).prop('disabled', true);
    //$('#claveUnidad').val(data.claveUnidad).prop('disabled', true);
    //$('#cantidadUnidadMedida').val(data.cantidadUnidadMedida).prop('disabled', true);
    //$('#articulo').val(data.articulo).prop('disabled', true);
    //$('.field-validation-error').html("");
    //document.getElementById('barra').src = '';
    //document.getElementById('qr').src = '';
    //obtenerCodigos();

    //para abrir el modal
    $('#ModalExistencias').modal({ backdrop: 'static', keyboard: false, show: true });

}

function AbrirModalCierreCajaExcedentes() {

    $('#ModalCierreExceso').modal({ backdrop: 'static', keyboard: false, show: true });

}

function AbrirModalCierreDia() {

    $('#ModalCierreExceso').modal({ backdrop: 'static', keyboard: false, show: true });

}


$('#btnRetirarExcesoEfectivo').click(function (e) {

    alert();

});



$('#btnBuscarExistencias').click(function (e) {

    var idProducto = $('#idProductoExistencia').val();
    var idSucursal = $('#idSucursalExistencia').val();

    $.ajax({
        url: rootUrl("/Productos/_UbicacionesProducto"),
        data: { idProducto: idProducto, idSucursal: idSucursal },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $("#existencias").html(data);
            //$('#modalUbicacionProducto').modal({ backdrop: 'static', keyboard: false, show: true });

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar mostrar las ubicaciones del producto, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });


});


//function VerUbicacionesProducto(idProducto) {
//    $.ajax({
//        url: rootUrl("/Productos/_UbicacionesProducto"),
//        data: { idProducto: idProducto },
//        method: 'post',
//        dataType: 'html',
//        async: true,
//        beforeSend: function (xhr) {
//            ShowLoader();
//        },
//        success: function (data) {
//            OcultarLoader();
//            $("#existencias").html(data);
//            //$('#modalUbicacionProducto').modal({ backdrop: 'static', keyboard: false, show: true });

//        },
//        error: function (xhr, status) {
//            OcultarLoader();
//            console.log('Hubo un problema al intentar mostrar las ubicaciones del producto, contactese con el administrador del sistema');
//            console.log(xhr);
//            console.log(status);
//        }
//    });
//}


$(document).ready(function () {

    actualizaTicketVenta();
    InitSelect2Productos();
    document.getElementById("divUsoCFDI").style.display = 'none';

});