var table;
var iframe;
var tablaVentasPedidosEspeciales;

//busqueda
function onBeginSubmitVentasPedidosEspeciales() {
    ShowLoader('Buscando...');
}

function onSuccessResultVentasPedidosEspeciales(data) {
    $('#rowVentasPedidosEspeciales').html(data);

    if ($.fn.DataTable.isDataTable('#tablaRepVentasPedidosEspeciales')) {
        tablaVentasPedidosEspeciales.destroy();
    }

    InitDataTableVentasPedidosEspeciales();
    OcultarLoader();
}

function onFailureResultVentasPedidosEspeciales() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar las ventas de pedidos especiales");
}


function InitDataTableVentasPedidosEspeciales() {
    var NombreTabla = "tablaRepVentasPedidosEspeciales";
    tablaVentasPedidosEspeciales = initDataTable(NombreTabla);

    if ($("#tablaRepVentasPedidosEspeciales").length > 0) {
        new $.fn.dataTable.Buttons(tablaVentasPedidosEspeciales, {
            buttons: [
            
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14]
                    },
                },
            ],

        });

        tablaVentasPedidosEspeciales.buttons(0, null).container().prependTo(
            tablaVentasPedidosEspeciales.table().container()
        );
    }
}



var vtas = [];


function SaveData() {

    $('#tablaRepVentasPedidosEspeciales tbody tr').each(function (index, fila) {

        var row_ = {
            idVenta: fila.children[0].innerHTML,
            nombreCliente: fila.children[1].innerHTML
        };
        vtas.push(row_);

    });

    dataToPost = JSON.stringify({ ventas: vtas });

    $.ajax({
        type: "POST",
        url: "/Ventas/GuardarVentas",
        contentType: "application/json; charset=utf-8", // specify the content type
        dataType: 'JSON', // make sure you use the correct case for dataType
        data: dataToPost,
        traditional: true
    });

}


function eliminaFila(index_) {
    document.getElementById("tablaRepVentasPedidosEspeciales").deleteRow(index_);
    actualizaTicket();
}

//$('#cancelar').click(function (e) {
//});

//function actualizaTicket() {

//    var total = parseFloat(0);

//    $('#tablaRepVentasPedidosEspeciales tbody tr').each(function (index, fila) {
//        fila.children[0].innerHTML = index + 1;
//        fila.children[5].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
//        total += parseFloat(fila.children[4].innerHTML.replace('$', ''));
//    });

//    //actualizar los totales
//    document.getElementById("divSubTotal").innerHTML = "$" + parseFloat(total).toFixed(2);
//    document.getElementById("divIva").innerHTML = "$" + parseFloat(total * 0.16).toFixed(2);
//    document.getElementById("divTotal").innerHTML = "$" + parseFloat(total * 1.16).toFixed(2);
//}

//$('#btnAgregarProducto').click(function (e) {

//    var row_ = "<tr>" +
//        "  <td>1</td>" +
//        "  <td>prueba Blitz TDR-3000</td>" +
//        "  <td class=\"text-center\">$" + $('#precio').val() + "</td>" +
//        "  <td class=\"text-center\">" + $('#cantidad').val() + "</td>" +
//        "  <td class=\"text-center\">$" + $('#cantidad').val() * $('#precio').val() + "</td>" +
//        "  <td class=\"text-center\">" +
//        "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
//        "  </td>" +
//        "</tr >";

//    $("table tbody").append(row_);

//    actualizaTicket();

//});




$(document).ready(function () {
    //InitDataTableVentasPedidosEspeciales();
    InitSelect2();
    InitRangePicker('rangeVentasPedidosEspeciales', 'fechaIni', 'fechaFin');
    //$('#rangeVentasPedidosEspeciales').val('');

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarVentasPedidosEspeciales").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarVentasPedidosEspeciales .select-multiple").trigger("change");
    });
});