var tblCompras
var arrayProductos = [];
$(document).ready(function () {
    if ($("#tblCompras").length > 0) {
        InitTableCompras();
    }
    ConsultaProductos();
    InitRangePicker('rangeCompras', 'fechaIni', 'fechaFin');
    //$('#fechaIni').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

    $('#rangeCompras').val('');
    $('.select-multiple').select2({
        width: "100%",
        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        },

    });

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarCompras").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarCompras .select-multiple").trigger("change");
    });

    $('#precio').focus();




});

function ConsultaProductos() {

    var result = '';
    $.ajax({
        url: rootUrl("/Productos/ObtenerProductosPorUsuario"),
        data: { activo: true },
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

    var i;
    for (i = 0; i < result.Modelo.length; i++) {
        result.Modelo[i].id = result.Modelo[i]['idProducto'];
        result.Modelo[i].text = result.Modelo[i]['descripcion'];
    }

    arrayProductos = result.Modelo;

}

function InitSelect2Productos() {
    $("#idProducto").html('').select2();
    $('#idProducto').select2({
        width: "100%",
        placeholder: "--SELECCIONA--",
        data: arrayProductos,

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
}


function InitTableCompras() {
    var NombreTabla = "tblCompras";
    tblCompras = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblCompras, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Compras",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    //doc.content[1].table.widths = ['10%', '30%', '20%', '10%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Compras");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                },
            },
        ],
    });

    tblCompras.buttons(0, null).container().prependTo(
        tblCompras.table().container()
    );


    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a onclick="NuevaCompra(0)" class="btn btn-icon btn-success" name="" id="btnNuevaCompra" data-toggle="tooltip" title="Nueva compra"><i class="fas fa-plus"></i></a>');


}

function onBeginSubmitObtenerCompras() {
    ShowLoader("Buscando...");
}
function onCompleteObtenerCompras() {
    //OcultarLoader();
}
function onSuccessResultObtenerCompras(data) {
    $("#DivtblCompras").html(data);
    if ($("#tblCompras").length > 0) {
        tblCompras.destroy();
        InitTableCompras();
    }

    OcultarLoader();
}
function onFailureResultObtenerCompras() {
    OcultarLoader();
}

function EliminarCompra(idCompra) {

    swal({
        title: '',
        text: 'Estas seguro que deseas eliminar a esta Compra?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Compras/EliminaCompra"),
                    data: { idCompra: idCompra },
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader("Eliminando Compra.");
                    },
                    success: function (data) {
                        OcultarLoader();
                        if (data.Estatus == 200) {
                            MuestraToast("success", data.Mensaje);
                            $("#btnBuscarCompras").click();
                        }
                        else
                            MuestraToast("error", data.Mensaje);

                    },
                    error: function (xhr, status) {
                        OcultarLoader();
                        console.log('Hubo un problema al intentar eliminar la compra, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });

            } else {
                console.log("cancelar");
            }
        });


}

function NuevaCompra(idCompra) {

    $.ajax({
        url: rootUrl("/Compras/_Compra"),
        data: { idCompra: idCompra },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();

            if (idCompra > 0)
                $("#titleModalCompra").html("Editar Compra");
            else
                $("#titleModalCompra").html("Nueva Compra");
            $("#NuevaCompra").html(data);
            InicializaElementosCompra();
            $('#modalNuevaCompra').modal({ backdrop: 'static', keyboard: false, show: true });


        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar mostrar el detalle de la compra, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}


////////////////////////// COMPONENTES NUEVA O EDITAR COMPRA ////////////////////////////////

function InicializaElementosCompra() {
    InitSelect2Productos();
    actualizaTicket();
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
    $("#idProducto").change(function (evt) {
        evt.preventDefault();
        var producto = $('#idProducto').select2('data')[0];
        if (producto != null && producto != undefined) {
            $("#unidadCompra").val(producto.unidadCompra.descripcionUnidadCompra);
            $("#cantidadUnidadCompra").val(producto.unidadCompra.cantidadUnidadCompra);
            $("#unidadVenta").val(producto.DescripcionUnidadMedida);
            $("#precio").val(producto.costo);
            $("#cantPorUnidadCompra").val(0);
            $("#cantidad").val(0);
            $('#precio').focus();
            console.log("focus");
        }
    });

    $("#cantPorUnidadCompra").blur(function (evt) {
        var cantidadUnidadCompra = parseInt($("#cantidadUnidadCompra").val());
        var cantidadPorUnidadCompra = parseFloat($("#cantPorUnidadCompra").val());
        $("#cantidad").val(Math.round(cantidadUnidadCompra * cantidadPorUnidadCompra));
    });

    $("#cantidad").blur(function (evt) {
        var cantidadUnidadCompra = parseInt($("#cantidadUnidadCompra").val());
        var cantidadComprada = parseInt($("#cantidad").val());
        var cantidadPorUnidadCompra = (cantidadUnidadCompra > 0 ? roundToTwo(cantidadComprada / cantidadUnidadCompra) : cantidadComprada);
        $("#cantPorUnidadCompra").val(cantidadPorUnidadCompra);
    });

    $('#limpiar').click(function (e) {
        $('#tblComprasDetalle tbody').html("");
        $('#idProveedor').val("").trigger('change');
        $('#idStatusCompra').val("").trigger('change');
        actualizaTicket();
    });

    $('#btnAgregarProducto').click(function (e) {

        if ($('#idProducto').val() <= 0) {
            MuestraToast('warning', "Debe seleccionar un producto.");
        } else if (Number($('#precio').val()) == 0) {
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
                    MuestraToast("error", "El producto ya existe en su compra");
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
                    "  <td>0</td>" +
                    "  <td class=\"text-center\"><input type='text' onfocusout=\"actualizaTicket()\" onkeypress=\"return esNumero(event)\" style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + cantidad + "\" ></td>" +
                    "  <td class=\"text-center\"><input type='text' onfocusout=\"actualizaTicket()\" onkeypress=\"return esDecimal(this, event);\" style=\"text-align: center; border: none; border-color: transparent;  background: transparent; \" value=\"" + precio + "\" ></td>" +
                    "  <td class=\"text-center\">$" + roundToTwo(cantidad * precio) + "</td>" +
                    "  <td class=\"text-center\">" +
                    "      <a href=\"javascript:eliminaFila(" + idProducto + ",0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
                    "  </td>" +
                    "</tr >";

                $("#tblComprasDetalle tbody").append(row_);
            }

            $('#idProducto').val('').trigger('change');
            $('#cantidad').val('0');
            $('#precio').val('0');
            $("#unidadCompra").val('');
            $("#cantidadUnidadCompra").val('');
            $("#unidadVenta").val('');
            $("#cantPorUnidadCompra").val('');
            actualizaTicket();
            $('#cantidad').focus();


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

            if (Number($(fila.children[6].children[0]).val()) == 0) {
                MuestraToast('warning', "La cantidad solicitada del producto " + fila.children[1].innerHTML + " debe ser mayor a 0.");
                error = error + 1;
                return false;
            }

            if (Number($(fila.children[7].children[0]).val()) == 0) {
                MuestraToast('warning', "El costo del producto " + fila.children[1].innerHTML + " debe ser mayor a 0.");
                error = error + 1;
                return false;
            }

            var row_ = {
                idProducto: fila.children[0].innerHTML,
                cantidad: Number($(fila.children[6].children[0]).val()),
                precio: Number($(fila.children[7].children[0]).val())
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
                                $('#modalNuevaCompra').modal('hide');
                                $("#frmBuscarCompras").submit();
                                //if ($("#idCompra").val() > 0)
                                //window.location = rootUrl("/Compras/Compras")
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

}


function actualizaTicket() {

    var total = parseFloat(0);

    $('#tblComprasDetalle tbody tr').each(function (index, fila) {
        var Cantidad = parseFloat(0);
        var Precio = parseFloat(0);

        //fila.children[0].innerHTML = index + 1;
        //fila.children[6].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        Precio = Number($(fila.children[7].children[0]).val().replace(',', '.'));
        //if ($("#idStatusCompra").val()==)
        //Cantidad = Number($(fila.children[4].children[0]).val());
        if ($("#idStatusCompra").val() == 3 || $("#idStatusCompra").val() == 2)
            Cantidad = Number(fila.children[4].innerHTML);
        else
            Cantidad = Number($(fila.children[6].children[0]).val());
       
        fila.children[8].innerHTML = "$" + roundToTwo(Precio * Cantidad)
        total += parseFloat(fila.children[8].innerHTML.replace('$', ''));
    });


    document.getElementById("divTotal").innerHTML = "<h4>$" + roundToTwo(total) + "</h4>";
}

function eliminaFila(idProducto, idEstatusProducto) {
    if (idEstatusProducto != 0) {
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

