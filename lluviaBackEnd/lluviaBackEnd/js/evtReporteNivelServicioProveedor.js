var tblNivelServicioProveedor;
$(document).ready(function () {
    InitRangePicker('rangeNivelServicioProveedor', 'fechaIni', 'fechaFin');
    //$('#fechaIni').val($('#rangeNivelServicioProveedor').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeNivelServicioProveedor').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $("#rangeNivelServicioProveedor").val("");
    $("#frmBuscarNivelServicioProveedor").submit();
});

//busqueda
function onBeginSubmitNivelServicioProveedor() {
    ShowLoader("Buscando...");
}
function onSuccessResultNivelServicioProveedor(data) {
    if (tblNivelServicioProveedor != null)
        tblNivelServicioProveedor.destroy();

    $('#ViewNivelServicioProveedor').html(data);
    if ($("#tblNivelServicioProveedor").length > 0)
        InitDataTabletblDropSize();
    OcultarLoader();
}
function onFailureResultNivelServicioProveedor() {
    OcultarLoader();
}

function InitDataTabletblDropSize() {
    var NombreTabla = "tblNivelServicioProveedor";
    tblNivelServicioProveedor = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblNivelServicioProveedor, {
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

    tblNivelServicioProveedor.buttons(0, null).container().prependTo(
        tblNivelServicioProveedor.table().container()
    );
}
