var table;
var iframe;
var tablaProductos;  

//busqueda
function onBeginSubmitProductos() {
    console.log("onBeginSubmit___");
}
function onCompleteSubmitProductos() {
    console.log("onCompleteSubmit___");
}
function onSuccessResultProductos(data) {
    console.log("onSuccessResult", JSON.stringify(data) );

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
                    doc.content[1].table.widths = ['25%', '25%',  '25%', '25%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Productos");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
            },
        ],
    });

    tablaProductos.buttons(0, null).container().prependTo(
        tablaProductos.table().container()
    );

  

    $('#' + NombreTabla+'_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarProducto" data-toggle="tooltip" title="Agregar Producto"><i class="fas fa-plus"></i></a>');
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

    $('#max_').val('');
    $('#min_').val('');
    $('#precio').val('');
    $('#idProductoRango').val(idProducto);
    $("#tablaRangosPrecios").find("tr:gt(0)").remove();
    document.getElementById("btnGuardarPrecios").disabled = true;
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

function pintarPrecios(data) {
    document.getElementById("sinPrecios").innerHTML = "";

    if (data.Modelo == null) {
        document.getElementById("sinPrecios").innerHTML = "<strong>No se tienen configurados precios para este producto. &nbsp;</strong>";
    }
    else {
        var i = 0;
        for (i = 0; i < data.Modelo.length; i++) {
            console.log(data.Modelo[i].contador);

            var row_ =
                "<tr>" +
                "    <td>0</td>" +
                "    <td class=\"text-center\">" + data.Modelo[i].min + "</td>" +
                "    <td class=\"text-center\">" + data.Modelo[i].max + "</td>" +
                "    <td class=\"text-center\">" + data.Modelo[i].costo + "</td>" +
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
    console.log("idClaveProdServ", data.idClaveProdServ )
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
        icon: 'warning',
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
    console.log($('#articulo').val());
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

    if ( $('#min_').val() == "" || $('#max_').val() == "" || $('#precio').val() == ""  ) {
        MuestraToast('warning', "Debe poner todos los datos para insertar el rango de precios.");
    }
    else {

        if ((parseFloat($('#min_').val())) >= (parseFloat($('#max_').val()))) {
            MuestraToast('warning', "El máximo debe ser mayor al mínimo");
        }
        else {

            var maximo = parseFloat(0);

            $('#tablaRangosPrecios tbody tr').each(function (index, fila) {
                console.log(fila.children[1].innerHTML + ", " + fila.children[2].innerHTML);
                var maximo_actual = parseFloat(fila.children[2].innerHTML);
                if (maximo_actual > maximo) {
                    maximo = maximo_actual;
                }
            });

            if (parseFloat(maximo) >= parseFloat($('#min_').val())) {
                MuestraToast('warning', "El mínimo que intenta insertar debe ser mayor al maximo del rango anterior");
            }
            else {


                // si todo bien 
                var row_ =
                    "<tr>" +
                    "    <td>0</td>" +
                    "    <td class=\"text-center\">" + $('#min_').val() + "</td>" +
                    "    <td class=\"text-center\">" + $('#max_').val() + "</td>" +
                    "    <td class=\"text-center\">" + $('#precio').val() + "</td>" +
                    "    <td class=\"text-center\">" +
                    "       <a href=\"javascript:eliminaFilaPrecios(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
                    "    </td>" +
                    "</tr>";
                $("#tablaRangosPrecios tbody").append(row_);
                actualizaTablaPrecios();
                $('#max_').val('');
                $('#min_').val('');
                $('#precio').val('');

                validaBtnGuardarPrecios();
            }
        }
    }
});


$('#btnGuardarPrecios').click(function (e) {

    var rangos = [];

    $('#tablaRangosPrecios tbody tr').each(function (index, fila) {

        var row_ =  {
                        contador: fila.children[0].innerHTML,
                        min: fila.children[1].innerHTML,
                        max: fila.children[2].innerHTML,
                        costo: fila.children[3].innerHTML,
                        idProducto: $('#idProductoRango').val()
                    };
        rangos.push(row_);

    });

    dataToPost = JSON.stringify({ precios: rangos });

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
            MuestraToast('success', data.Mensaje);
            $('#RangosPreciosProductoModal').modal('hide');
        },
        error: function (xhr, status) {
            console.log('Hubo un problema al insertar el rango, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

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
    validaBtnGuardarPrecios();

}

function validaBtnGuardarPrecios() {

    var totalRows = $('#tablaRangosPrecios tr').length - 1;
    if (totalRows <= 0) {
        document.getElementById("btnGuardarPrecios").disabled = true;
    }
    else {
        document.getElementById("btnGuardarPrecios").disabled = false;
    }
    
}

function actualizaTablaPrecios() {
    $('#tablaRangosPrecios tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[4].innerHTML = "      <a href=\"javascript:eliminaFilaPrecios(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
    });
}


$(document).ready(function () {

    InitTableProductos();
    InitRangePicker();
    $('#idLineaProductoBusqueda').val('0');

    document.getElementById('articulo').onchange = function () {
        obtenerCodigos();
    };

    $('#articulo').keyup(function () {
        obtenerCodigos();
    });

    $('#descripcion').keyup(function () {
        if ( ($('#idLineaProducto').val()) !== null )
        {
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


});