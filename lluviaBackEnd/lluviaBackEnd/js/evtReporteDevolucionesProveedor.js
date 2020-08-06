var tblDevolucionesProveedor;
$(document).ready(function () {
    InitRangePicker('rangeDevolucionesProveedor', 'fechaIni', 'fechaFin');
    //$('#fechaIni').val($('#rangeDevolucionesProveedor').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeDevolucionesProveedor').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $("#rangeDevolucionesProveedor").val("");
    $("#frmBuscarDevolucionesProveedor").submit();

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
function onBeginSubmitDevolucionesProveedor() {
    ShowLoader("Buscando...");
}
function onSuccessResultDevolucionesProveedor(data) {
    if (tblDevolucionesProveedor != null)
        tblDevolucionesProveedor.destroy();

    $('#ViewDevolucionesProveedor').html(data);
    if ($("#tblDevolucionesProveedor").length > 0)
        InitDataTabletblDropSize();
    OcultarLoader();
}
function onFailureResultDevolucionesProveedor() {
    OcultarLoader();
}

function InitDataTabletblDropSize() {
    var NombreTabla = "tblDevolucionesProveedor";
    tblDevolucionesProveedor = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblDevolucionesProveedor, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Nivel de Servicio Proveedor",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    // doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Nivel de Servicio Proveedor");
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

    tblDevolucionesProveedor.buttons(0, null).container().prependTo(
        tblDevolucionesProveedor.table().container()
    );
}
