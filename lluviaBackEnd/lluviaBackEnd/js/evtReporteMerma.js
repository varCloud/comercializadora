var tblMerma;
$(document).ready(function () {   
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

//busqueda
function onBeginSubmitMerma() {
    ShowLoader("Buscando...");
}
function onSuccessResultMerma(data) {
    if (tblMerma != null)
        tblMerma.destroy();

    $('#ViewMerma').html(data);
    if ($("#tblMerma").length > 0)
        InitDataTableMerma();
    OcultarLoader();
}
function onFailureResultMerma() {
    OcultarLoader();
}

function InitDataTableMerma() {
    var NombreTabla = "tblMerma";
    tblMerma = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblMerma, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Indicador Merma",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    // doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Indicador Merma");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7,8,9,10,11]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7,8,9,10,11]
                },
            },
        ],

    });

    tblMerma.buttons(0, null).container().prependTo(
        tblMerma.table().container()
    );
}
