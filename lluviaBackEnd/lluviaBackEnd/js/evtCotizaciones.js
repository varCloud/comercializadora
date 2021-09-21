var table;
var iframe;
var tblCotizaciones;


function InitDataTableCotizaciones() {
    var NombreTabla = "tblCotizaciones";
    tblCotizaciones = initDataTable(NombreTabla);

    if ($("#tblCotizaciones").length > 0) {
        new $.fn.dataTable.Buttons(tblCotizaciones, {
            buttons: [
                {
                    extend: 'pdfHtml5',
                    text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a PDF',
                    title: "Cotizaciones",
                    customize: function (doc) {
                        doc.defaultStyle.fontSize = 8;
                        doc.styles.tableHeader.fontSize = 10;
                        doc.defaultStyle.alignment = 'center';
                        doc.content[1].table.widths = ['10%', '30%', '30%', '10%', '20%'];
                        doc.pageMargins = [30, 85, 20, 30];
                        doc.content.splice(0, 1);
                        doc['header'] = SetHeaderPDF("Cotizaciones");
                        doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                    },
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4]
                    },
                },
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4]
                    },
                },
            ],

        });

        tblCotizaciones.buttons(0, null).container().prependTo(
            tblCotizaciones.table().container()
        );
    }
}


$(document).ready(function () {

    InitDataTableCotizaciones();


});