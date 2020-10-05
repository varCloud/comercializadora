var table;
var iframe;
var tablaProductos;


function VerUbicacionesProducto(idProducto) {
    $.ajax({
        url: rootUrl("/Productos/_UbicacionesProducto"),
        data: { idProducto: idProducto },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $("#ubicacionProducto").html(data);
            $('#modalUbicacionProducto').modal({ backdrop: 'static', keyboard: false, show: true });

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar mostrar las ubicaciones del producto, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}

//busqueda
function onBeginSubmitProductos() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitProductos() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultProductos(data) {
    console.log("onSuccessResult", JSON.stringify(data));

    tablaProductos.destroy();
    $('#rowProductos').html(data);
    InitTableProductos();

}
function onFailureResultProductos() {
    console.log("onFailureResult___");
}

// guardar-modificar
function onBeginSubmitGuardarProducto() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitGuardarProducto() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultGuardarProducto(data) {
    console.log("onSuccessResult", JSON.stringify(data));
    if (data.Estatus == 200) {
        MuestraToast('success', data.Mensaje);
        PintarTabla();
    } else {
        //MuestraToast("error", data.Mensaje);
    }
    $('#EditarProductoModal').modal('hide');
}
function onFailureResultGuardarProducto() {
    console.log("onFailureResult___");
}

function PintarTabla() {
    $.ajax({
        url: rootUrl("/Productos/_ObtenerProductos"),
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaProductos.destroy();
            $('#rowProductos').html(data);
            InitTableProductos();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function BuscarProductos(data) {
    $.ajax({
        url: rootUrl("/Productos/BuscarProductos"),
        data: { idUsuario: data.idProducto },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            table.destroy();
            $('#rowProductos').html(data);
            InitDataTable();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitTableProductos() {
    var NombreTabla = "tablaProductos";
    tablaProductos = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaProductos, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Productos",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%','30%', '30%', '20%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Productos");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3,4]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3,4]
                },
            },
        ],
    });

    tablaProductos.buttons(0, null).container().prependTo(
        tablaProductos.table().container()
    );



    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarProducto" data-toggle="tooltip" title="Agregar Producto"><i class="fas fa-plus"></i></a>');
    InitBtnAgregar();
}


function ObtenerProducto(idProducto) {

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

function VerProducto(idProducto) {

    $('#btnGuardarProducto').prop('disabled', true);

    var data = ObtenerProducto(idProducto);

    $('#idProducto').val(idProducto);
    $('#activo').val(data.activo);
    $('#descripcion').val(data.descripcion).prop('disabled', true);
    $('#idUnidadMedida').val(data.idUnidadMedida).prop('disabled', true);
    $('#idLineaProducto').val(data.idLineaProducto).prop('disabled', true);
    $('#claveProdServ').val(data.claveProdServ).prop('disabled', true);
    $('#claveUnidad').val(data.claveUnidad).prop('disabled', true);
    $('#cantidadUnidadMedida').val(data.cantidadUnidadMedida).prop('disabled', true);
    $('#articulo').val(data.articulo).prop('disabled', true);
    $('.field-validation-error').html("");
    document.getElementById('barra').src = '';
    document.getElementById('qr').src = '';
    obtenerCodigos();
    //para abrir el modal
    $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalProducto').html("Información del Producto");
}

function VerPrecios(idProducto) {

    $('#precioIndividual').val('');
    $('#precioMenudeo').val('');
    $('#porcUtilidadMayoreo').val('');
    $('#precioMenudeo').val('');
    $('#ultimoCostoCompra').val('');
    $('#max_').val('');
    $('#min_').val('');
    $('#precio').val('');
    $('#porcUtilidad').val('');
    $('#idProductoRango').val(idProducto);
    $("#tablaRangosPrecios").find("tr:gt(0)").remove();
    //document.getElementById("btnGuardarPrecios").disabled = true;
    ObtenerIndividualMenudeo(idProducto);
    ObtenerPrecios(idProducto);
   

    //para abrir el modal
    $('#RangosPreciosProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalRangosPrecios').html("Precios por Producto");

}

function ObtenerPrecios(idProducto) {

    var result = '';
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
            pintarPrecios(data);
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
            $('#precioIndividual').val(data.precioIndividual);
            if (data.porcUtilidadIndividual == 0 && data.costo > 0 && data.precioIndividual > 0)
                data.porcUtilidadIndividual=CalcularPorcUtilidad(data.costo, data.precioIndividual);
            $('#porcUtilidadIndividual').val(data.porcUtilidadIndividual);            
            $('#precioMenudeo').val(data.precioMenudeo);
            if (data.porcUtilidadMayoreo == 0 && data.costo > 0 && data.precioMenudeo > 0)
                data.porcUtilidadMayoreo=CalcularPorcUtilidad(data.costo, data.precioMenudeo);
            $('#porcUtilidadMayoreo').val(data.porcUtilidadMayoreo);
            $('#ultimoCostoCompra').val(data.costo)
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

function pintarPrecios(data) {
    document.getElementById("sinPrecios").innerHTML = "";

    if (data.Modelo == null) {
        document.getElementById("sinPrecios").innerHTML = "<strong>No se tienen configurados precios para este producto. &nbsp;</strong>";
    }
    else {
        var i = 0;
        for (i = 0; i < data.Modelo.length; i++) {
            //console.log(data.Modelo[i].contador);

            if (data.Modelo[i].costo > 0 && data.Modelo[i].porcUtilidad == 0 && $("#ultimoCostoCompra").val() > 0)
                data.Modelo[i].porcUtilidad = CalcularPorcUtilidad($("#ultimoCostoCompra").val(), data.Modelo[i].costo);
 
            var row_ =
                "<tr>" +
                "    <td>0</td>" +
                "    <td class=\"text-center\">" + data.Modelo[i].min + "</td>" +
                "    <td class=\"text-center\">" + data.Modelo[i].max + "</td>" +
                "    <td class=\"text-center\">" + data.Modelo[i].costo + "</td>" +             
                "    <td class=\"text-center\">" + data.Modelo[i].porcUtilidad + "</td>" +
                "    <td class=\"text-center\">" +
                "       <a href=\"javascript:eliminaFilaPrecios(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
                "    </td>" +
                "</tr>";

            $("#tablaRangosPrecios tbody").append(row_);
        }
        actualizaTablaPrecios();
        $('#max_').val('');
        $('#min_').val('');
        $('#precio').val('');
        $('#porcUtilidad').val('');
    }
}

$('#tablaProductos tbody').on('click', 'td', function () {

    var col_ = tablaProductos.row(this).data();
    document.getElementById("descNombreProducto").innerHTML = "<strong>Descripción: &nbsp;</strong> " + col_[1];

});

function EditarProducto(idProducto) {

    $('#btnGuardarProducto').prop('disabled', false);

    var data = ObtenerProducto(idProducto);

    $('#idProducto').val(idProducto);
    $('#activo').val(data.activo);
    $('#descripcion').val(data.descripcion).prop('disabled', false);
    $('#idUnidadMedida').val(data.idUnidadMedida).prop('disabled', false);
    $('#idLineaProducto').val(data.idLineaProducto).prop('disabled', false);
    //console.log("idClaveProdServ", data.idClaveProdServ)
    $('#cbClaveProdServ').val(data.idClaveProdServ).prop('disabled', false);
    $('#claveUnidad').val(data.claveUnidad).prop('disabled', false);
    $('#cantidadUnidadMedida').val(data.cantidadUnidadMedida).prop('disabled', false);
    $('#articulo').val(data.articulo).prop('disabled', false);
    $('.field-validation-error').html("");
    document.getElementById('barra').src = '';
    document.getElementById('qr').src = '';
    obtenerCodigos();
    //para abrir el modal
    $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
    $('#TituloModalUsuario').html("Editar Producto");
}

function InitBtnAgregar() {
    $('#btnAgregarProducto').click(function (e) {

        $('#btnGuardarProducto').prop('disabled', false);

        $('#idProducto').val(0);
        $('#descripcion').val('').prop('disabled', false);
        $('#idUnidadMedida').val('').prop('disabled', false);
        $('#idLineaProducto').val('').prop('disabled', false);
        $('#cbClaveProdServ').val('').prop('disabled', false);
        $('#cantidadUnidadMedida').val('').prop('disabled', false);
        $('#articulo').val('').prop('disabled', false);
        $('.field-validation-error').html("");
        document.getElementById('barra').src = '';
        document.getElementById('qr').src = '';

        //para abrir el modal
        $('#EditarProductoModal').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalProducto').html("Agregar Producto");
    });
}

function EliminarProducto(idProducto) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a este Producto?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Productos/ActualizarEstatusProducto"),
                    data: { idProducto: idProducto, activo: false },
                    method: 'post',
                    dataType: 'json',
                    async: false,
                    beforeSend: function (xhr) {
                        console.log("Antes ")
                    },
                    success: function (data) {
                        MuestraToast('success', data.Mensaje);
                        PintarTabla();
                    },
                    error: function (xhr, status) {
                        console.log('Hubo un problema al intentar eliminar al usuario, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });

            } else {
                console.log("cancelar");
            }
        });
}

function obtenerCodigos() {
    //console.log($('#articulo').val());
    if ($('#articulo').val() !== '') {
        $.ajax({
            url: rootUrl("/Productos/ObtenerCodigos"),
            data: { cadena: $('#articulo').val() },
            method: 'post',
            dataType: 'json',
            async: true,
            beforeSend: function (xhr) {
                console.log("Antes ")
            },
            success: function (data) {
                console.log(data);
                $("#barra").attr('src', 'data:image/png;base64,' + data.barra);
                $("#qr").attr('src', 'data:image/png;base64,' + data.qr);
            },
            error: function (xhr, status) {
                console.log('Hubo un problema al intentar eliminar al usuario, contactese con el administrador del sistema');
                console.log(xhr);
                console.log(status);
            }
        });
    }
}


$('#btnAgregarPrecio').click(function (e) {
    //console.log($('#min_').val());
    //console.log($('#max_').val());
    //console.log($('#precio').val());
    if ($('#min_').val() == "" || $('#max_').val() == "" || $('#precio').val() == "") {
        MuestraToast('warning', "Debe poner todos los datos para insertar el rango de precios.");
    }
    else {

        if (
            (parseFloat($('#min_').val()) >= parseFloat(0) && parseFloat($('#min_').val()) <= parseFloat(11)) ||
            (parseFloat($('#max_').val()) >= parseFloat(0) && parseFloat($('#max_').val()) <= parseFloat(11))

        ) {
            MuestraToast('warning', "Para precios de productos entre 1 y 11 artículos debe asignar el Precio Individual.");
        }
        else {

            if ((parseFloat($('#min_').val()) == parseFloat(12)) || (parseFloat($('#max_').val()) == parseFloat(12))) {
                MuestraToast('warning', "Para precios de productos de 12 artículos debe asignar el Precio Menudeo.");
            }
            else {

                if ((parseFloat($('#min_').val())) >= (parseFloat($('#max_').val()))) {
                    MuestraToast('warning', "El máximo debe ser mayor al mínimo del rango que quiere agregar.");
                }
                else {

                    var maximo = parseFloat(0);
                    var precioMinimo = parseFloat(0);

                    $('#tablaRangosPrecios tbody tr').each(function (index, fila) {
                        //console.log(fila.children[1].innerHTML + ", " + fila.children[2].innerHTML);
                        var maximo_actual = parseFloat(fila.children[2].innerHTML);
                        var PrecioActual = parseFloat(fila.children[3].innerHTML);
                        if (maximo_actual > maximo) {
                            maximo = maximo_actual;
                        }
                        if (precioMinimo == 0)
                            precioMinimo = PrecioActual

                        if (PrecioActual < precioMinimo) {
                            precioMinimo = PrecioActual
                        }

                    });


                    if (parseFloat(maximo) >= parseFloat($('#min_').val())) {
                        MuestraToast('warning', "El mínimo que intenta insertar debe ser mayor al maximo del rango anterior");
                    }
                    else if ((precioMinimo == parseFloat(0)) && (parseFloat($('#precio').val()) >= parseFloat($('#precioIndividual').val()) || parseFloat($('#precio').val()) >= parseFloat($('#precioMenudeo').val()))) {
                        MuestraToast('warning', "El precio debe ser menor que el precio de mayoreo y que el precio de menudeo.");
                    }
                    else if (precioMinimo>0 && parseFloat($('#precio').val()) >= precioMinimo) {
                        MuestraToast('warning', "El precio que intenta insertar debe ser menor al precio minimo del rango anterior.");
                    }
                    else {


                        // si todo bien 
                        var row_ =
                            "<tr>" +
                            "    <td>0</td>" +
                            "    <td class=\"text-center\">" + $('#min_').val() + "</td>" +
                            "    <td class=\"text-center\">" + $('#max_').val() + "</td>" +
                            "    <td class=\"text-center\">" + $('#precio').val() + "</td>" +
                            "    <td class=\"text-center\">" + $('#porcUtilidad').val() + "</td>" +
                            "    <td class=\"text-center\">" +
                            "       <a href=\"javascript:eliminaFilaPrecios(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
                            "    </td>" +
                            "</tr>";
                        $("#tablaRangosPrecios tbody").append(row_);
                        actualizaTablaPrecios();
                        $('#max_').val('');
                        $('#min_').val('');
                        $('#precio').val('');
                        $('#porcUtilidad').val('');

                        //validaBtnGuardarPrecios();
                    }
                }
            }
        }
    }
});


$('#btnGuardarPrecios').click(function (e) {

    var error = 0;
    if ($('#precioIndividual').val() == "" || $('#precioMenudeo').val() == "" || $('#precioIndividual').val() == "0" || $('#precioMenudeo').val() == "0") {
        MuestraToast('warning', "Por favor asigne un valor para los precios de Mayoreo y Menudeo.");
    }
    else if (parseFloat($('#precioIndividual').val()) < parseFloat($('#precioMenudeo').val())) {
        MuestraToast('warning', "El precio de Menudeo debe ser mayor que el precio de Mayoreo.");
    }
    else {

        var idProducto = parseInt(0);
        idProducto = $('#idProductoRango').val();

        var rangos = [];

        $('#tablaRangosPrecios tbody tr').each(function (index, fila) {

            var row_ = {
                contador: fila.children[0].innerHTML,
                min: fila.children[1].innerHTML,
                max: fila.children[2].innerHTML,
                costo: fila.children[3].innerHTML,
                porcUtilidad: fila.children[4].innerHTML
                //idProducto: $('#idProductoRango').val()
            };

            if (parseFloat(row_.costo) >= parseFloat($('#precioIndividual').val()) || row_.costo >= parseFloat($('#precioMenudeo').val())) {
                error = error + 1;
                MuestraToast('warning', "El precio de " + row_.min + " a " + row_.max + " productos debe ser menor que el precio de mayoreo y que el precio de menudeo.");
                return
            }
            rangos.push(row_);
        });

        if (error == 0) {

            var producto = {
                idProducto: idProducto,
                precioIndividual: $('#precioIndividual').val(),
                precioMenudeo: $('#precioMenudeo').val(),
                costo: $('#ultimoCostoCompra').val(),
                porcUtilidadIndividual: $('#porcUtilidadIndividual').val(),
                porcUtilidadMayoreo: $('#porcUtilidadMayoreo').val()

            };

            dataToPost = JSON.stringify({ precios: rangos, producto: producto });

            $.ajax({
                url: rootUrl("/Productos/GuardarPrecios"),
                data: dataToPost,
                method: 'POST',
                dataType: 'JSON',
                contentType: "application/json; charset=utf-8", // specify the content type
                async: false,
                beforeSend: function (xhr) {
                },
                success: function (data) {
                    //MuestraToast('success', data.Mensaje);
                    MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
                    $('#RangosPreciosProductoModal').modal('hide');
                },
                error: function (xhr, status) {
                    console.log('Hubo un problema al insertar el rango, contactese con el administrador del sistema');
                    console.log(xhr);
                    console.log(status);
                }
            });
        }
    }


});

function eliminaFilaPrecios(index_) {

    var max_id = parseFloat(0);

    $('#tablaRangosPrecios tbody tr').each(function (index, fila) {
        var maximo_actual = parseFloat(fila.children[0].innerHTML);
        if (maximo_actual > max_id) {
            max_id = maximo_actual;
        }
    });

    var i;
    for (i = max_id; i >= index_; i--) {
        document.getElementById("tablaRangosPrecios").deleteRow(i);
    }

    actualizaTablaPrecios();
    //validaBtnGuardarPrecios();

}

function ImprimirCodigos(articulo, descProducto) {
    $.ajax({
        url: rootUrl("/Productos/ImprimirCodigos"),
        data: { articulo: articulo, descProducto: descProducto },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            OcultarLoader();
            //console.log(data);
            window.open("http://" + window.location.host + "/Codigos/" + data.Modelo, "_blank");
            //console.log("http://" + window.location.host + "/Codigos/" + data.Modelo);
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

function actualizaTablaPrecios() {
    $('#tablaRangosPrecios tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[5].innerHTML = "      <a href=\"javascript:eliminaFilaPrecios(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
    });
}

function CalcularPorcUtilidad(Costo,Precio)
{   
    var PorcUtilidad = 0;
    if (Costo > 0 && Precio>0)
        PorcUtilidad = roundToTwo(((Precio * 100) / Costo) - 100);
    return PorcUtilidad;
}

function CalcularPecioPorUtilidad(Costo,Utilidad) {
    var Precio = 0;   
    if (Costo > 0 && Utilidad > 0)
        Precio = roundToTwo(Costo * (1 + (Utilidad/100)));
    return Precio;
}

$(document).ready(function () {

    InitTableProductos();
    InitRangePicker();
    //$('#idLineaProductoBusqueda').val('0');

    document.getElementById('articulo').onchange = function () {
        obtenerCodigos();
    };

    $('#articulo').keyup(function () {
        obtenerCodigos();
    });

    $('#descripcion').keyup(function () {
        if (($('#idLineaProducto').val()) !== null) {
            $('#articulo').val($("#idLineaProducto option:selected").text().replace('Linea ', '').substring(0, 2).concat('-').concat($("#descripcion").val().substring(0, 3)).toUpperCase());
            obtenerCodigos();
        }
    });

    $('#idLineaProducto').change(function () {
        if (($('#descripcion').val()) !== '') {
            $('#articulo').val($("#idLineaProducto option:selected").text().replace('Linea ', '').substring(0, 2).concat('-').concat($("#descripcion").val().substring(0, 3)).toUpperCase());
            obtenerCodigos();
        }
    });

    ////Ajustamos precio y porcentaje de utilidad
    $("#precioIndividual").keyup(function () {
        document.getElementById("btnGuardarPrecios").disabled = false;
        $("#porcUtilidadIndividual").val(CalcularPorcUtilidad($("#ultimoCostoCompra").val(), $("#precioIndividual").val()));
    });

    $("#precioMenudeo").keyup(function () {
        document.getElementById("btnGuardarPrecios").disabled = false;
        $("#porcUtilidadMayoreo").val(CalcularPorcUtilidad($("#ultimoCostoCompra").val(), $("#precioMenudeo").val()));
    });

    $("#porcUtilidadIndividual").keyup(function () {
        document.getElementById("btnGuardarPrecios").disabled = false;
        $("#precioIndividual").val(CalcularPecioPorUtilidad($("#ultimoCostoCompra").val(), $("#porcUtilidadIndividual").val()));
    });

    $("#porcUtilidadMayoreo").keyup(function () {
        document.getElementById("btnGuardarPrecios").disabled = false;
        $("#precioMenudeo").val(CalcularPecioPorUtilidad($("#ultimoCostoCompra").val(), $("#porcUtilidadMayoreo").val()));
    });

    $("#precio").keyup(function () {       
        $("#porcUtilidad").val(CalcularPorcUtilidad($("#ultimoCostoCompra").val(), $("#precio").val()));
    });

    $("#porcUtilidad").keyup(function () {       
        $("#precio").val(CalcularPecioPorUtilidad($("#ultimoCostoCompra").val(), $("#porcUtilidad").val()));
    });

    $("#ultimoCostoCompra").keyup(function () {

        $("#porcUtilidadIndividual").val(CalcularPorcUtilidad($("#ultimoCostoCompra").val(), $("#precioIndividual").val()));
        $("#porcUtilidadMayoreo").val(CalcularPorcUtilidad($("#ultimoCostoCompra").val(), $("#precioMenudeo").val()));

        $('#tablaRangosPrecios tbody tr').each(function (index, fila) {
            var Precio=fila.children[3].innerHTML;
            fila.children[4].innerHTML = CalcularPorcUtilidad($("#ultimoCostoCompra").val(), Precio)
        });

    });
});