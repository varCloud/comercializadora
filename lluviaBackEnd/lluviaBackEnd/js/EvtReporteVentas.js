var table;
var iframe;
var tablaVentas;

//busqueda
function onBeginSubmitVentas() {
    ShowLoader('Buscando...');
}

function onSuccessResultVentas(data) {
    $('#rowVentas').html(data);
    tablaVentas.destroy();
    InitDataTableVentas();

    OcultarLoader();
}
function onFailureResultVentas() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar las ventas");
}


function InitDataTableVentas() {
    var NombreTabla = "tablaRepVentas";
    tablaVentas = initDataTable(NombreTabla);

    if ($("#tablaRepVentas").length > 0) {
        new $.fn.dataTable.Buttons(tablaVentas, {
            buttons: [
            
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14,15,16]
                    },
                },
            ],

        });

        tablaVentas.buttons(0, null).container().prependTo(
            tablaVentas.table().container()
        );
    }
}



var vtas = [];


function SaveData() {

    $('#tablaRepVentas tbody tr').each(function (index, fila) {

        //codigo = fila.children[0].innerHTML;
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

//function InitSelect2Productos() {

//    $('.select-multiple').select2({
//        width: "100%",
//        language: {

//            noResults: function () {

//                return "No hay resultado";
//            },
//            searching: function () {

//                return "Buscando..";
//            }
//        }

//    });

//}

function eliminaFila(index_) {
    document.getElementById("tablaRepVentas").deleteRow(index_);
    actualizaTicket();
}

$('#cancelar').click(function (e) {


});

function actualizaTicket() {

    var total = parseFloat(0);

    $('#tablaRepVentas tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        fila.children[5].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        total += parseFloat(fila.children[4].innerHTML.replace('$', ''));
    });

    //actualizar los totales
    document.getElementById("divSubTotal").innerHTML = "$" + parseFloat(total).toFixed(2);
    document.getElementById("divIva").innerHTML = "$" + parseFloat(total * 0.16).toFixed(2);
    document.getElementById("divTotal").innerHTML = "$" + parseFloat(total * 1.16).toFixed(2);
}

$('#btnAgregarProducto').click(function (e) {

    var row_ = "<tr>" +
        "  <td>1</td>" +
        "  <td>prueba Blitz TDR-3000</td>" +
        "  <td class=\"text-center\">$" + $('#precio').val() + "</td>" +
        "  <td class=\"text-center\">" + $('#cantidad').val() + "</td>" +
        "  <td class=\"text-center\">$" + $('#cantidad').val() * $('#precio').val() + "</td>" +
        "  <td class=\"text-center\">" +
        "      <a href=\"javascript:eliminaFila(0)\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>" +
        "  </td>" +
        "</tr >";

    $("table tbody").append(row_);

    actualizaTicket();

});




$(document).ready(function () {
    InitDataTableVentas();
    InitSelect2();
    //InitSelect2Productos();
    InitRangePicker('rangeVentas', 'fechaIni', 'fechaFin');
    //$('#rangeVentas').val('');
    //$('#idLineaProductoBusqueda').val('0');
    //$('#idClienteBusqueda').val('0');
    //$('#idUsuarioBusqueda').val('0');
    // $('#fechaIni').val($('#rangeVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarVentas").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarVentas .select-multiple").trigger("change");

    });

});