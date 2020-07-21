$(document).ready(function () {
    //actualizaTicket();
    $('.select-multipleCompra').select2({
     
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
        }else if (Number($('#precio').val()) ==0) {
            MuestraToast('warning', "El costo del producto que desea agregar debe de ser mayor que 0.");
        }
        else if (Number($('#cantidad').val()) == 0) {
            MuestraToast('warning', "La cantidad del producto que desea agregar debe de ser mayor que 0.");
        }
        else {

            var idProducto = $('#idProducto').val();
            var cantidad = $('#cantidad').val();
            var precio = $('#precio').val();
            var existeProducto = false;

            $("#tblComprasDetalle tbody tr").each(function (index) {
                if ($(this).attr("id") == idProducto) {
                    MuestraToast("error","El producto ya existe en su compra");
                    existeProducto = true;
                    return;
                }
            });

            if (existeProducto == false) {
                var row_ = "<tr id=" + idProducto + ">" +
                    //"  <td>1</td>" +
                    "  <td> " + $('#idProducto').val() + "</td>" +
                    "  <td> " + $("#idProducto").find("option:selected").text() + "</td>" +
                    "  <td><div class='badge badge-light badge-shadow'>Pendiente</div></td>" +
                    "  <td></td>" +
                    "  <td>0</td>" +
                    "  <td class=\"text-center\"><input type='text' onfocusout=\"actualizaTicket()\" onkeypress=\"return esNumero(event)\" style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + cantidad + "\" ></td>" +
                    "  <td class=\"text-center\"><input type='text' onfocusout=\"actualizaTicket()\" onkeypress=\"return esPrecio(event)\" style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + precio + "\" ></td>" +
                    "  <td class=\"text-center\">$" + cantidad * precio + "</td>" +
                    "  <td class=\"text-center\">" +
                    "      <a href=\"javascript:eliminaFila(" + idProducto + ",0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
                    "  </td>" +
                    "</tr >";

                $("#tblComprasDetalle tbody").append(row_);
            }
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
        var error = 0;
        $('#tblComprasDetalle tbody tr').each(function (index, fila) {
           
            if (Number($(fila.children[5].children[0]).val()) == 0) 
            {
                MuestraToast('warning', "La cantidad solicitada del producto " + fila.children[1].innerHTML + " debe ser mayor a 0.");
                error = error + 1;
                return false;
            } 
            
            if (Number($(fila.children[6].children[0]).val()) == 0) {
                MuestraToast('warning', "El costo del producto " + fila.children[1].innerHTML + " debe ser mayor a 0.");
                error = error + 1;
                return false;
            } 
            
            var row_ = {
                idProducto: fila.children[0].innerHTML,
                cantidad: Number($(fila.children[5].children[0]).val()),
                precio: Number($(fila.children[6].children[0]).val())               
            };
            productos.push(row_);
        });

        if (error > 0)
            return;

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
                    compra.observaciones = $("#ObservacionesCompra").val();

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
                                //if ($("#idCompra").val() > 0)
                                    window.location = rootUrl("/Compras/Compras")
                                //else
                                //    $("#limpiar").click();
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
        var Cantidad = parseFloat(0);
        var Precio = parseFloat(0);

        //fila.children[0].innerHTML = index + 1;
        //fila.children[6].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        Precio = Number($(fila.children[6].children[0]).val().replace(',', '.'));
        Cantidad = Number($(fila.children[5].children[0]).val());
        fila.children[7].innerHTML = "$" + (Precio * Cantidad)
        total += parseFloat(fila.children[7].innerHTML.replace('$', ''));
    });


    document.getElementById("divTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
}

function eliminaFila(idProducto, idEstatusProducto) {
    if (idEstatusProducto != 0)
    {
        MuestraToast("error", "El producto no se puede eliminar de la compra ya que fue recibido");
    }        
    else {
        $('#tblComprasDetalle tbody tr#' + idProducto).remove();
        actualizaTicket();
    }

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


