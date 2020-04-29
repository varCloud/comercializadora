var tblCompras
$(document).ready(function () {
    InitTableCompras();
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
});

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
                    doc.content[1].table.widths = ['10%', '30%', '20%', '10%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Compras");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            },
        ],
    });

    tblCompras.buttons(0, null).container().prependTo(
        tblCompras.table().container()
    );


    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnNuevaCompra" data-toggle="tooltip" title="Nueva compra"><i class="fas fa-plus"></i></a>');

    $('#btnNuevaCompra').click(function (e) {

    });
}

function onBeginSubmitObtenerCompras() {
    console.log("onBeginSubmitObtenerCompras");
}
function onCompleteObtenerCompras() {
    console.log("onCompleteObtenerCompras");
}
function onSuccessResultObtenerCompras(data) {
    console.log("onSuccessResultObtenerCompras", JSON.stringify(data));
    tblCompras.destroy();
    $("#DivtblCompras").html(data);
    InitTableCompras();
}
function onFailureResultObtenerCompras() {
    console.log("onFailureResultObtenerCompras");
}

