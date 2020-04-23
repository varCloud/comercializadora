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
        text: 'Este producto no tiene un rango de precios configurado, desea configurarlo?',
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

function preguntaQuiereFactura(idVenta) {
    console.log(idVenta);
    swal({
        title: 'Mensaje',
        text: '¿Desea facturar esta venta?',
        icon: 'info',
        buttons: ["No", "Sí"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {

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

            } else {
                console.log("cancelar");
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
    actualizaTicket();
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

    actualizaTicket();
    limpiaModalPrevio();
    $('#cantidad').val('');
    $('#idProducto').val(0);
    $('#idVenta').val(0);

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
    document.getElementById("previoDescuento").innerHTML = "<h4>$" + parseFloat(0.0).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    $('#idCliente').val('0');
    $('#formaPago').val('0');
    //console.log(document.getElementById('idCliente').selectedIndex);

    //document.getElementById('idCliente').selectedIndex = "0";
    //document.getElementById('formaPago').selectedIndex = 0;

}



$('#previoVenta').click(function (e) {

    limpiaModalPrevio();
    document.getElementById("previoTotal").innerHTML = document.getElementById("divSubTotal").innerHTML;
    document.getElementById("previoDescuento").innerHTML = "<h4>$" + parseFloat(0.0).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = document.getElementById("divSubTotal").innerHTML;
    document.getElementById("previoIVA").innerHTML = document.getElementById("divIva").innerHTML;
    document.getElementById("previoFinal").innerHTML = document.getElementById("divTotal").innerHTML;

    $('#ModalPrevioVenta').modal({ backdrop: 'static', keyboard: false, show: true });
});

function actualizaTicket() {

    var total = parseFloat(0);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[6].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index+1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        total += parseFloat(fila.children[5].innerHTML.replace('$', ''));
        console.log(total + " " + index);
    });

    //actualizar los totales
    document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
    document.getElementById("divIva").innerHTML = "<h4>$" + parseFloat(total * 0.16).toFixed(2) + "</h4>";
    document.getElementById("divTotal").innerHTML = "<h4>$" + parseFloat(total * 1.16).toFixed(2) + "</h4>";

    //replicamos en el precio de la venta
    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
    document.getElementById("previoDescuento").innerHTML = "<h4>$" + parseFloat(0.0).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(total * 0.16).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(total * 1.16).toFixed(2) + "</h4>";

}

$('#btnAgregarProducto').click(function (e) {

    if ($('#cantidad').val() == "") {
        MuestraToast('warning', "Debe escribir la cantidad de productos que va a agregar.");
    }
    else {

        //var precio = parseFloat($('#precio').val());
        var idProducto = $('#idProducto').val();
        var cantidad = $('#cantidad').val();
        var data = ObtenerProductoPorPrecio(idProducto, cantidad);
        var precio = parseFloat(data.Modelo[0].costo);

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
                        "  <td class=\"text-center\" onclick=\"listenerDobleClick(this," + $('#idProducto').val()+ ");\" onblur=\"this.contentEditable=false;\">" + cantidad + "</td>" +
                        "  <td class=\"text-center\">$" + cantidad * precio + "</td>" +
                        "  <td class=\"text-center\">" +
                        "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
                        "  </td>" +
                        "</tr >";

            $("table tbody").append(row_);
            $('#cantidad').val('');

            actualizaTicket();
        }

    }
    
});


function ObtenerProductoPorPrecio(idProducto,cantidad) {

    var result = '';
    $.ajax({
        url: rootUrl("/Ventas/ObtenerProductoPorPrecio"),
        data: { idProducto: idProducto, cantidad: cantidad, costo : 0 },
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
            cantidad: fila.children[4].innerHTML,
            idUsuario: 4,//fila.children[3].innerHTML,
            formaPago: $('#formaPago').val(),
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
            preguntaQuiereFactura(data.Modelo.idVenta);
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al guardar la venta, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

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



function listenerDobleClick(element, idProducto) {
    element.contentEditable = true;

    $(element).keydown(function (evt) {
        el = evt.target;

        if (evt.keyCode == 27) {
            document.execCommand('undo');
            element.contentEditable = false;
        }
        //        console.log("nodeName_" + el.nodeName + " el_" + el.innerHTML);

        if (evt.keyCode == 13 || (evt.keyCode > 31 && (evt.keyCode < 48 || evt.keyCode > 57))) {
            event.preventDefault();

            var cantidad = parseFloat(el.innerHTML);
            var data = ObtenerProductoPorPrecio(idProducto, cantidad);
            var precio = parseFloat(data.Modelo[0].costo);
            console.log(precio);

            document.execCommand('undo');

            element.contentEditable = false;
            console.log("quedo: " + el.innerHTML);
        }
        else {       // si es un numero         
        }

    });

    setTimeout(function () {
        if (document.activeElement !== element) {
            element.contentEditable = false;
            ///alert("time");
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


$("#efectivo").on("change", function () {

    var cambio_     = parseFloat(0).toFixed(2);
    var efectivo_   = parseFloat($('#efectivo').val()).toFixed(2);
    var total_      = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);


    if (parseFloat(efectivo_) > parseFloat(total_)) {
        cambio_ = efectivo_ - total_;
        //console.log("cambio_:" + cambio_);
        document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>";
    }
    else {
        document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    }

});



$(document).ready(function () {

    actualizaTicket();
    InitSelect2Productos();

    $("#idCliente").on("change", function () {
        //console.log($('#idCliente').val());
        
        var idCliente = parseFloat($('#idCliente').val());
        var data = ObtenerCliente(idCliente);
        var nombre = data.Modelo.nombres + "  " + data.Modelo.apellidoPaterno + "  " + data.Modelo.apellidoMaterno;
        var descuento = parseFloat(0.0);

        if (idCliente != 0) {
            descuento = parseFloat(data.Modelo.tipoCliente.descuento).toFixed(2);;
        }
        
        var total = parseFloat(document.getElementById("previoTotal").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
        var cantidadDescontada = parseFloat(0).toFixed(2);

        if (descuento > 0.0) {
            cantidadDescontada = parseFloat(total * (descuento / 100)).toFixed(2);
        }

        var subTotal = parseFloat(total - cantidadDescontada).toFixed(2);
        var iva = parseFloat(subTotal * 0.16).toFixed(2);
        var final = parseFloat(subTotal * 1.16).toFixed(2);

        document.getElementById("previoDescuento").innerHTML = "<h4>-$" + cantidadDescontada + "</h4>";
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

        if ( (data.idCliente != 0) && (idCliente != 0) ) {
            row_ =  "<address>" +
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


});