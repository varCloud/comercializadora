$(document).ready(function () {
    actualizaTicket();
    $('.select-multiple').select2({

        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        },

    });

    //$('#tblComprasDetalle').editableTableWidget();


    $('#limpiar').click(function (e) {
        $('#tblComprasDetalle tbody').html("");
        $('#idProveedor').val("").trigger('change');
        $('#idStatusCompra').val("").trigger('change');
        actualizaTicket();
    });

    $('#btnAgregarProducto').click(function (e) {

        if ($('#idProducto').val()<=0) {
            MuestraToast('warning', "Debe seleccionar un producto.");
        }else if ($('#precio').val() == "") {
            MuestraToast('warning', "Debe escribir el precio de productos que va a agregar.");
        }
        else if ($('#cantidad').val() == "") {
            MuestraToast('warning', "Debe escribir la cantidad de productos que va a agregar.");
        }
        else {

            var idProducto = $('#idProducto').val();
            var cantidad = $('#cantidad').val();
            var precio = $('#precio').val();

            // console.log($("#idProducto").find("option:selected").text());
            var row_ = "<tr>" +
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

            $("#tblComprasDetalle tbody").append(row_);
            $('#cantidad').val('');
            $('#precio').val('');
            $('#idProducto').val('').trigger('change');
            actualizaTicket();


        }

    });


    $('#btnGuardarCompra').click(function (e) {

        if ($('#idProveedor').val() == "") {
            MuestraToast('warning', "Debe seleccionar un proveedor.");
            return;
        }
        if ($('#idStatusCompra').val() == "") {
            MuestraToast('warning', "Debe seleccionar el estatus de la compra.");
            return;
        }


        var productos = [];
        $('#tblComprasDetalle tbody tr').each(function (index, fila) {
            var row_ = {
                idProducto: fila.children[1].innerHTML,
                cantidad: fila.children[4].innerHTML,
                precio: fila.children[3].innerHTML.replace('$', '')
            };
            productos.push(row_);
        });

        if (productos.length === 0) {
            MuestraToast('warning', "Debe agregar productos a la compra.");
            return;
        }

        swal({
            title: '',
            text: $("#idCompra").val() > 0 ? 'Estas seguro que deseas actualizar esta Compra?' : 'Estas seguro que deseas guardar esta Compra?',
            icon: '',
            buttons: ["Cancelar", "Aceptar"],
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    var Proveedor = new Object();
                    Proveedor.idProveedor = $("#idProveedor").val();

                    var StatusCompra = new Object();
                    StatusCompra.idStatus = $("#idStatusCompra").val();

                    var compra = new Object();
                    compra.idCompra = $("#idCompra").val();
                    compra.proveedor = Proveedor;
                    compra.listProductos = productos;
                    compra.statusCompra = StatusCompra;

                    dataToPost = JSON.stringify({ compra: compra });

                    $.ajax({
                        url: rootUrl("/Compras/GuardarCompra"),
                        data: dataToPost,
                        method: 'POST',
                        dataType: 'JSON',
                        contentType: "application/json; charset=utf-8",
                        async: true,
                        beforeSend: function (xhr) {
                            ShowLoader("Guardando Compra.");
                        },
                        success: function (data) {
                            OcultarLoader();

                            if (data.Estatus == 200) {
                                MuestraToast("success", data.Mensaje);
                                if ($("#idCompra").val() > 0)
                                    window.location = rootUrl("/Compras/Compras")
                                else
                                    $("#limpiar").click();
                            }
                            else
                                MuestraToast("error", data.Mensaje);


                        },
                        error: function (xhr, status) {
                            OcultarLoader();
                            console.log('Hubo un problema al guardar la compra, contactese con el administrador del sistema');
                            console.log(xhr);
                            console.log(status);
                        }
                    });
                } else {
                    console.log("cancelar");
                }
            });

    });

    $("#btnNuevoProveedor").click(function (e) {
        $('#frmProveedor #idProveedor').val(0);
        $('#frmProveedor #activo').val(0);
        $('#frmProveedor #nombre').val('');
        $('#frmProveedor #descripcion').val('');
        $('#frmProveedor #telefono').val('');
        $('#frmProveedor #direccion').val('');
        $('#EditarProveedorModal').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalProveedor').html("Nuevo Proveedor");
    });


});


function actualizaTicket() {

    var total = parseFloat(0);

    $('#tblComprasDetalle tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[6].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        total += parseFloat(fila.children[5].innerHTML.replace('$', ''));
    });

    //actualizar los totales
    //document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
    //document.getElementById("divIva").innerHTML = "<h4>$" + parseFloat(total * 0.16).toFixed(2) + "</h4>";
    //document.getElementById("divTotal").innerHTML = "<h4>$" + parseFloat(total * 1.16).toFixed(2) + "</h4>";
    document.getElementById("divTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
}

function eliminaFila(index_) {
    document.getElementById("tblComprasDetalle").deleteRow(index_);
    actualizaTicket();
}

//Proveedor
function onBeginSubmitGuardarProveedor() {
    ShowLoader("Guardando proveedor");
}
function onCompleteSubmitGuardarProveedor() {
    console.log("onCompleteSubmitGuardarProveedor");
}
function onSuccessResultGuardarProveedor(data) {
    console.log("onSuccessResultGuardarProveedor");
    if (data.Estatus == 200) {
        MuestraToast("success", data.Mensaje);
        var option = new Option(data.Modelo.nombre, data.Modelo.idProveedor, true, true);
        $('#idProveedor').append(option).trigger('change');
    } else {
        MuestraToast("error", data.Mensaje);
    }

    $('#EditarProveedorModal').modal('hide');
    OcultarLoader();

}
function onFailureResultGuardarProveedor() {
    OcultarLoader();
}


