var tblFacturas
$(document).ready(function () {
    InitTableFacturas();
    InitRangePicker('rangeFacturas', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

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


    //$("#fechaIni").daterangepicker({
    //    locale: { format: "YYYY-MM-DD", cancelLabel: 'Clear' },
    //    singleDatePicker: true,
    //    autoUpdateInput: false
    //}, (from_date, to_date) => {
    //    $("#fechaIni").val(from_date.format('YYYY-MM-DD'));
    //});


    //$("#fechaFin").daterangepicker({
    //    locale: { format: "YYYY-MM-DD", cancelLabel: 'Clear' },
    //    singleDatePicker: true,
    //    autoUpdateInput: false
    //}, (from_date, to_date) => {
    //    $("#fechaFin").val(from_date.format('YYYY-MM-DD'));
    //});

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarFacturas").trigger("reset");
        $("#frmBuscarFacturas .select-multiple").trigger("change");

    });

});

function InitTableFacturas() {
    var NombreTabla = "tblFacturas";
    tblFacturas = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblFacturas, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Facturas",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '30%', '30%', '10%','20%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Facturas");
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

    tblFacturas.buttons(0, null).container().prependTo(
        tblFacturas.table().container()
    );

}

function onBeginSubmitObtenerFacturas() {
    ShowLoader("Buscando...");
}
function onCompleteObtenerFacturas() {
    //OcultarLoader();
}
function onSuccessResultObtenerFacturas(data) {
    console.log("onSuccessResultObtenerFacturas", JSON.stringify(data));
    tblFacturas.destroy();
    $("#DivtblFacturas").html(data);
    if (data.estatus == 200)
        InitTableFacturas();
    OcultarLoader();
}
function onFailureResultObtenerFacturas() {
    OcultarLoader();
}
