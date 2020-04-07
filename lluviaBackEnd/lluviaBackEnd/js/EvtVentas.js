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




var vtas = [];


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

$('#cancelar').click(function (e) {

    var max_id = parseFloat(0);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        var maximo_actual = parseFloat(fila.children[0].innerHTML);
        if (maximo_actual > max_id) {
            max_id = maximo_actual;
        }
    });

    var i;
    for (i = max_id; i >= 1 ; i--) {
        document.getElementById("tablaRepVentas").deleteRow(i);
    }

    actualizaTicket();
    $('#cantidad').val('');

});


$('#previoVenta').click(function (e) {


    $('#ModalPrevioVenta').modal({ backdrop: 'static', keyboard: false, show: true });


});

function actualizaTicket() {

    var total = parseFloat(0);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[6].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index+1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        total += parseFloat(fila.children[5].innerHTML.replace('$', ''));
    });

    //actualizar los totales
    document.getElementById("divSubTotal").innerHTML = "$" + parseFloat(total).toFixed(2);
    document.getElementById("divIva").innerHTML = "$" + parseFloat(total * 0.16).toFixed(2);
    document.getElementById("divTotal").innerHTML = "$" + parseFloat(total * 1.16).toFixed(2);

    //replicamos en el precio de la venta
    document.getElementById("previoTotal").innerHTML = "<h3>$" + parseFloat(total).toFixed(2) + "</h3>";
    document.getElementById("previoDescuento").innerHTML = "<h3>$" + parseFloat(0.0).toFixed(2) + "</h3>";
    document.getElementById("previoSubTotal").innerHTML = "<h3>$" + parseFloat(total).toFixed(2) + "</h3>";
    document.getElementById("previoIVA").innerHTML = "<h3>$" + parseFloat(total * 0.16).toFixed(2) + "</h3>";
    document.getElementById("previoFinal").innerHTML = "<h3>$" + parseFloat(total * 1.16).toFixed(2) + "</h3>";

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
            console.log($("#idProducto").find("option:selected").text());
            var row_ =  "<tr>" +
                        "  <td>1</td>" +
                        "  <td> " + $('#idProducto').val() + "</td>" +
                        "  <td> " + $("#idProducto").find("option:selected").text() + "</td>" +
                        "  <td class=\"text-center\">$" + precio + "</td>" +
                        "  <td class=\"text-center\">" + cantidad + "</td>" +
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
            formaPago: $('#formaPago').val()
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
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            MuestraToast('success', data.Mensaje);
            $('#ModalPrevioVenta').modal('hide');
        },
        error: function (xhr, status) {
            console.log('Hubo un problema al guardar la venta, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

});




$(document).ready(function () {

    actualizaTicket();
    InitSelect2Productos();

    $("#idCliente").on("change", function () {
        console.log($('#idCliente').val());
        
        var idCliente = parseFloat($('#idCliente').val());
        var data = ObtenerCliente(idCliente);
        var nombre = data.Modelo.nombres + "  " + data.Modelo.apellidoPaterno + "  " + data.Modelo.apellidoMaterno;
        var descuento = parseFloat(0.0);

        if (idCliente != 0) {
            descuento = parseFloat(data.Modelo.tipoCliente.descuento).toFixed(2);;
        }
        
        var total = parseFloat(document.getElementById("previoTotal").innerHTML.replace("<h3>$", "").replace("</h3>", "")).toFixed(2);
        var cantidadDescontada = parseFloat(0).toFixed(2);

        if (descuento > 0.0) {
            cantidadDescontada = parseFloat(total * (descuento / 100)).toFixed(2);
        }

        var subTotal = parseFloat(total - cantidadDescontada).toFixed(2);
        var iva = parseFloat(subTotal * 0.16).toFixed(2);
        var final = parseFloat(subTotal * 1.16).toFixed(2);

        document.getElementById("previoDescuento").innerHTML = "<h3>-$" + cantidadDescontada + "</h3>";
        document.getElementById("previoSubTotal").innerHTML = "<h3>$" + subTotal + "</h3>";
        document.getElementById("previoIVA").innerHTML = "<h3>$" + iva + "</h3>";
        document.getElementById("previoFinal").innerHTML = "<h3>$" + final + "</h3>";

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