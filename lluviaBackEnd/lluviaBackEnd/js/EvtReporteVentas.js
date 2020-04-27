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
    console.log("onSuccessResultVentas", JSON.stringify(data));
    tablaVentas.destroy();
    $('#rowVentas').html(data);
    InitDataTableVentas();
}
function onFailureResultVentas() {
    console.log("onFailureResult___");
}

function PintarTabla() {
    $.ajax({
        url: "/Reportes/BuscarVentas",
        data: { idUsuario: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaVentas.destroy();
            $('#rowVentas').html(data);
            InitDataTableVentas();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitDataTableVentas() {
    var NombreTabla = "tablaRepVentas";
    tablaVentas = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaVentas, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Ventas",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '20%', '20%', '10%', '20%', '20%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Ventas");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
        ],

    });

    tablaVentas.buttons(0, null).container().prependTo(
        tablaVentas.table().container()
    );
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
    //InitSelect2Productos();
    InitRangePicker('rangeVentas', 'fechaIni', 'fechaFin');
    $('#idLineaProductoBusqueda').val('0');
    $('#idClienteBusqueda').val('0');
    $('#idUsuarioBusqueda').val('0');
    $('#fechaIni').val($('#rangeVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

});