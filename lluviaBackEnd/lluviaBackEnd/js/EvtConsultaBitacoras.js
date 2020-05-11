var tablaBitacoras;
$(document).ready(function () {
    InitTableBitacoras();
    InitRangePicker('rangeBitacoras', 'fechaIni', 'fechaFin');

    $('#rangeBitacoras').val('');
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
        $("#frmBuscarBitacoras").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarBitacoras .select-multiple").trigger("change");

    });

});

function InitTableBitacoras() {
    var NombreTabla = "tblBitacoras";
    tablaBitacoras = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaBitacoras, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Bitàcoras",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['5%', '15%', '15%', '15%', '20%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Bitàcoras");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                },
            },
        ],
    });

    tablaBitacoras.buttons(0, null).container().prependTo(
        tablaBitacoras.table().container()
    );
    }

function onBeginSubmitObtenerBitacoras() {
    ShowLoader("Buscando...");
}
function onCompleteObtenerBitacoras() {
    //OcultarLoader();
}
function onSuccessResultObtenerBitacoras(data) {
    $("#DivtblBitacoras").html(data);
    if ($("#tblBitacoras ").length > 0) {
        tablaBitacoras.destroy();
        InitTableBitacoras();
    }

    OcultarLoader();
}
function onFailureResultObtenerBitacoras() {
    OcultarLoader();
}
