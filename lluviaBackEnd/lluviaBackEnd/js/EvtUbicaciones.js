
function eliminaFila(index_) {
    document.getElementById("tablaUbicaciones").deleteRow(index_);
    actualizaTabla();
}

$('#limpiar').click(function (e) {
    limpiatTabla();
});


function limpiatTabla() {

    var max_id = parseFloat(0);

    $('#tablaUbicaciones tbody tr').each(function (index, fila) {
        var maximo_actual = parseFloat(fila.children[0].innerHTML);
        if (maximo_actual > max_id) {
            max_id = maximo_actual;
        }
    });

    var i;
    for (i = max_id; i >= 1; i--) {
        document.getElementById("tablaUbicaciones").deleteRow(i);
    }

    $('#idAlmacen').val("0").trigger('change');
    actualizaTabla();

}


$('#btnAgregarUbicacion').click(function (e) {

    if ( $('#idSucursal').val() == null) {
        MuestraToast('warning', "Debe seleccionar la sucursal que desea agregar.");
        return;
    }

    if ($('#idAlmacen').val() == null) {
        MuestraToast('warning', "Debe seleccionar el almacen que desea agregar.");
        return;
    }

    if ($('#idPiso').val() == null) {
        MuestraToast('warning', "Debe seleccionar el piso que desea agregar.");
        return;
    }

    if ($('#idPasillo').val() == null) {
        MuestraToast('warning', "Debe seleccionar el pasillo que desea agregar.");
        return;
    }

    if ($('#idRaq').val() == null) {
        MuestraToast('warning', "Debe seleccionar el raq que desea agregar.");
        return;
    }


    var btnEliminaFila = "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";

    // si todo bien    
    var row_ =
        "<tr>" +
        "  <td class=\"text-center\">1</td>" +
        "  <td class=\"text-center\"> " + $("#idSucursal").find("option:selected").text() + "</td>" +
        "  <td class=\"text-center\"> " + $("#idAlmacen").find("option:selected").text() + "</td>" +
        "  <td class=\"text-center\"> " + $('#idPiso').val() + "</td>" +
        "  <td class=\"text-center\"> " + $('#idPasillo').val() + "</td>" +
        "  <td class=\"text-center\"> " + $('#idRaq').val() + "</td>" +
        "  <td class=\"text-center\">" + btnEliminaFila + "  </td>" +
        "  <td style=\"display: none;\">" + $('#idAlmacen').val() + "</td>" +
        "</tr >";

    $("table tbody").append(row_);

    actualizaTabla();

});



function actualizaTabla() {

    // acttualizamos el id y la funcion de eliminar fila
    $('#tablaUbicaciones tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[6].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
    });

}


$('#btnGenerarUbicaciones').click(function (e) {

    var ubicaciones = [];
    var tblVtas = document.getElementById('tablaUbicaciones');
    var rCount = tblVtas.rows.length;

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            var row_ = {
                idAlmacen: parseInt(tblVtas.rows[i].cells[7].innerHTML),
                idPiso: parseInt(tblVtas.rows[i].cells[3].innerHTML),
                idPasillo: parseInt(tblVtas.rows[i].cells[4].innerHTML),
                idRaq: parseInt(tblVtas.rows[i].cells[5].innerHTML),
                descripcionAlmacen: tblVtas.rows[i].cells[2].innerHTML
            };
            ubicaciones.push(row_);
        }
    }
    else {
        MuestraToast('warning', "Debe agregar al menos una ubicación para generar el PDF.");
        return;
    }

    dataToPost = JSON.stringify({ ubicaciones : ubicaciones });
    
    $.ajax({
        url: rootUrl("/Productos/GenerarUbicaciones"),
        data: dataToPost,
        method: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Generando Ubicaciones...");
        },
        success: function (data) {
            OcultarLoader();
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            window.open("http://" + window.location.host + "/Codigos/Ubicaciones.pdf");
            setTimeout(() => { eliminaArchivo("Ubicaciones.pdf"); }, 3000);
        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al guardar la venta, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

});


function eliminaArchivo(rutaArchivo) {
    $.ajax({
        url: rootUrl("/Productos/EliminaArchivo"),
        data: { 'rutaArchivo': "Ubicaciones.pdf" },
        method: 'post',
        //dataType: 'json',
        contentType: "text/xml",
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




$("#idSucursal").on("change", function () {
    var idSucursal = $('#idSucursal').val();
    cargaSelect2Almacenes(idSucursal);
}); 


function cargaSelect2Almacenes(idSucursal) {

    var result = '';
    $.ajax({
        url: rootUrl("/Productos/ObtenerAlmacenes"),
        data: { idSucursal: idSucursal },
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
        result.Modelo[i].id = result.Modelo[i]['idAlmacen'];
        result.Modelo[i].text = result.Modelo[i]['descripcion'];
    }

    $("#idAlmacen").html('').select2();
    $('#idAlmacen').select2({
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

    $('#idAlmacen').val("0").trigger('change');

}



$("#idAlmacen").on("change", function () {

    $("#idPiso").html('').select2();
    $("#idPasillo").html('').select2();
    $("#idRaq").html('').select2();
    cargaCombosUbicaciones("idAlmacen");
}); 

$("#idPiso").on("change", function () {
    $("#idPasillo").val("0").trigger('change');
    $("#idRaq").val("0").trigger('change');
    cargaCombosUbicaciones("idPiso");
}); 
$("#idPasillo").on("change", function () {
    cargaCombosUbicaciones("idPasillo");
}); 


function cargaCombosUbicaciones(combo) {

    var idSucursal = $('#idSucursal').val();
    var idAlmacen = $('#idAlmacen').val();

    var idPiso = $('#idPiso').val();
    var idPasillo = $('#idPasillo').val();
    var idRaq = $('#idRaq').val();

    //console.log(idSucursal + " " + idAlmacen + " " + idPiso + " " + idPasillo + " " + idRaq );

    var result = '';
    $.ajax({
        url: rootUrl("/Productos/ObtenerUbicacion"),
        data: { idSucursal: idSucursal, idAlmacen: idAlmacen, idPasillo: idPasillo, idRaq: idRaq, idPiso: idPiso },
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

    if (combo == 'idAlmacen') {

        if (idAlmacen == null) {
            llenaCombo(result.Modelo.length = 0, "idPiso");
        }
        else {
            llenaCombo(result.Modelo, "idPiso");
        }

        llenaCombo(result.Modelo.length=0, "idPasillo");
        llenaCombo(result.Modelo.length=0, "idRaq");
    }


    if (combo == 'idPiso') {
        llenaCombo(result.Modelo, "idPasillo");
    }


    if (combo == 'idPasillo') {
        llenaCombo(result.Modelo, "idRaq");
    }

}



function llenaCombo(dataCombo, combo) {

    var k;
    for (k = 0; k < dataCombo.length; k++) {
        dataCombo[k].id = dataCombo[k][combo];
        dataCombo[k].text = dataCombo[k][combo];
    }

    $('#' + combo).html('').select2();
    $('#' + combo).select2({
        width: "100%",
        placeholder: "",
        data: dataCombo,
        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando...";
            }
        }
    });
    $('#' + combo).val("0").trigger('change');

}



$(document).ready(function () {

    $('#idSucursal').val(1);
    $("#idSucursal").change();

});










