var arrayProductos = [];
var productosPorLinea = '';
var tablaProductos; 

function eliminaFila(index_) {
    document.getElementById("tablaCodigos").deleteRow(index_);
    actualizaTabla();
}

$('#limpiar').click(function (e) {
    limpiarTabla();
});


function limpiarTabla() {

    var max_id = parseFloat(0);

    $('#tablaCodigos tbody tr').each(function (index, fila) {
        var maximo_actual = parseFloat(fila.children[0].innerHTML);
        if (maximo_actual > max_id) {
            max_id = maximo_actual;
        }
    });

    var i;
    for (i = max_id; i >= 1; i--) {
        document.getElementById("tablaCodigos").deleteRow(i);
    }

    actualizaTabla();

}


$('#btnAgregarCodigo').click(function (e) {

    var btnEliminaFila = "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";

    var producto = $('#idProducto').select2('data')[0];

    // si todo bien    
    var row_ =
        "<tr>" +
        "  <td class=\"text-center\">1</td>" +
        "  <td class=\"text-center\"> " + producto.idProducto + "</td>" +
        "  <td class=\"text-center\"> " + producto.descripcion + "</td>" +
        "  <td class=\"text-center\"> " + producto.DescripcionLinea + "</td>" +
        "  <td class=\"text-center\"> " + producto.precioIndividual + "</td>" +
        "  <td class=\"text-center\"> " + producto.precioMenudeo + "</td>" +
        "  <td class=\"text-center\"> " + producto.codigoBarras + "</td>" +
        "  <td class=\"text-center\">" + btnEliminaFila + "  </td>" +
        "</tr >";

    $("table tbody").append(row_);

    actualizaTabla();

});


$('#btnAgregarLinea').click(function (e) {

    var idLineaProducto = $('#idLineaProducto').val();

    $.ajax({
        url: rootUrl("/Productos/ObtenerProductosPorLineaProducto"),
        data: { idProducto: 0, idUsuario: 0, idLineaProducto: idLineaProducto },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Agregando Códigos de Barras...");
        },
        success: function (data) {

            if (data.Estatus === 200) {
                productosPorLinea = data;

                if (data.Modelo.length <= 0) {
                    MuestraToast('warning', "Esta linea no tiene productos registrados.");
                }
                else {
                    var btnEliminaFila = "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
                    var i;
                    for (i = 0; i < productosPorLinea.Modelo.length; i++) {

                        // si todo bien    
                        var row_ =
                            "<tr>" +
                            "  <td class=\"text-center\">1</td>" +
                            "  <td class=\"text-center\"> " + productosPorLinea.Modelo[i].idProducto + "</td>" +
                            "  <td class=\"text-center\"> " + productosPorLinea.Modelo[i].descripcion + "</td>" +
                            "  <td class=\"text-center\"> " + productosPorLinea.Modelo[i].DescripcionLinea + "</td>" +
                            "  <td class=\"text-center\"> " + productosPorLinea.Modelo[i].precioIndividual + "</td>" +
                            "  <td class=\"text-center\"> " + productosPorLinea.Modelo[i].precioMenudeo + "</td>" +
                            "  <td class=\"text-center\"> " + productosPorLinea.Modelo[i].codigoBarras + "</td>" +
                            "  <td class=\"text-center\">" + btnEliminaFila + "  </td>" +
                            "</tr >";

                        $("table tbody").append(row_);

                        actualizaTabla();

                    }

                }

            }
            OcultarLoader();

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

});

function actualizaTabla() {

    // acttualizamos el id y la funcion de eliminar fila
    $('#tablaCodigos tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[7].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
    });

}


$('#btnGenerarCodigos').click(function (e) {

    var productos = [];
    var tblCodigos = document.getElementById('tablaCodigos');
    var rCount = tblCodigos.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var row_ = {
                idProducto: parseInt(tblCodigos.rows[i].cells[1].innerHTML),
                descripcion: tblCodigos.rows[i].cells[2].innerHTML,
                DescripcionLinea: tblCodigos.rows[i].cells[3].innerHTML,
                precioIndividual: parseFloat(tblCodigos.rows[i].cells[4].innerHTML),
                precioMenudeo: parseFloat(tblCodigos.rows[i].cells[5].innerHTML),
                codigoBarras: tblCodigos.rows[i].cells[6].innerHTML,
            };
            productos.push(row_);
        }
    }
    else {
        MuestraToast('warning', "Debe agregar al menos un código para generar el PDF.");
        return;
    }
    console.log("productos", productos);
    dataToPost = JSON.stringify({ productos: productos });
    
    $.ajax({
        url: rootUrl("/Productos/GenerarCodigosBarras"),
        data: dataToPost,
        method: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Generando Códigos de Barras...");
        },
        success: function (data) {
            console.log("data :::::::::" , data)
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            window.open("http://" + window.location.host + "/Codigos/"+data.Modelo.nombreArchivoPDF+"");
            setTimeout(() => { eliminaArchivo(data.Modelo.nombreArchivoPDF); }, 3000);
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al generar los codigos, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

});


function eliminaArchivo(rutaArchivo) {
    console.log(rutaArchivo);
    $.ajax({
        url: rootUrl("/Productos/EliminaArchivo"),
        data: { 'rutaArchivo': rutaArchivo },
        method: 'post',
        dataType: 'json',
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
        result.Modelo[i].text = result.Modelo[i]['descripcion'];
    }

    $("#idProducto").html('').select2();
    $('#idProducto').select2({
        width: "100%",
        placeholder: "--SELECCIONA--",
        data: result.Modelo,

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

$(document).ready(function () {

    InitSelect2(); // los demas select2
    InitSelect2Productos();

});



