var tablaConsultaVentas; 
$(document).ready(function () {

    InitSelect2Multiple();
    InitDataTableConsultaVentas();
    InitRangePicker('rangeConsultaVentas', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

});

function InitSelect2Multiple() {
    $('.select-multiple').select2({
        width: "100%",
        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });
}

function InitDataTableConsultaVentas() {
    var NombreTabla = "tblVentasCanceladas";
    tablaConsultaVentas = initDataTable(NombreTabla);

    if ($("#tblVentasCanceladas").length > 0) {
        new $.fn.dataTable.Buttons(tablaConsultaVentas, {
            buttons: [
                {
                    extend: 'pdfHtml5',
                    text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a PDF',
                    title: "Ventas Canceladas",
                    customize: function (doc) {
                        doc.defaultStyle.fontSize = 8;
                        doc.styles.tableHeader.fontSize = 10;
                        doc.defaultStyle.alignment = 'center';
                        doc.content[1].table.widths = ['5%', '20%', '15%', '10%', '10%', '15%', '10%', '15%'];
                        doc.pageMargins = [30, 85, 20, 30];
                        doc.content.splice(0, 1);
                        doc['header'] = SetHeaderPDF("Ventas Canceladas");
                        doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                    },
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5,6,7]
                    },
                },
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5,6,7]
                    },
                },
            ],

        });

        tablaConsultaVentas.buttons(0, null).container().prependTo(
            tablaConsultaVentas.table().container()
        );

    }
}


function onBeginSubmitConsultaVentasCanceladas() {
    ShowLoader("Buscando...");
}

function onCompleteSubmitConsultaVentasCanceladas() {
    OcultarLoader();
}

function onSuccessResultConsultaVentasCanceladas(data) {
    $("#viewVentasCanceladas").html(data);
    if ($("#tblVentasCanceladas").length > 0) {
        tablaConsultaVentas.destroy();
        InitDataTableConsultaVentas();
    }

    OcultarLoader();
}

function onFailureResultConsultaVentasCanceladas() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar las ventas canceladas");
}

function ImprimeTicketVentaCancelada(idVenta) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicketVentaCancelada"),
        data: { idVenta: idVenta },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            console.log(data);
            OcultarLoader();
            MuestraToast('success', "Se envio el ticket a la impresora.");
        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast('error', "Ocurrio un error al enviar el ticket a la impresora.");
            console.log(xhr);
            console.log(status);
            console.log(data);
        }
    });
}