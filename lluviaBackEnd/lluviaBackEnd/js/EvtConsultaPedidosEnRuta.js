var table;
var iframe;
var tablaConsultaPedidosEnRuta; 

//busqueda
function onBeginSubmitConsultaPedidosEnRuta() {
    ShowLoader("Buscando...");
}

function onCompleteSubmitConsultaPedidosEnRuta() {
}

function onFailureResultConsultaPedidosEnRuta() {
    OcultarLoader();
}

function onSuccessResultConsultaPedidosEnRuta(data) {
    $('#rowConsultaPedidosEnRuta').html(data);
    tablaConsultaPedidosEnRuta.destroy();
    InitDataTableConsultaPedidosEnRuta();
    OcultarLoader();
}



function ImprimeTicketPedidosEnRuta(idPedidoEspecial, idTipoTicketPedidoEspecial, idTicketPedidoEspecial, ticketFinal) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ImprimeTicket"),
        data: { idPedidoEspecial: idPedidoEspecial, idTipoTicketPedidoEspecial: idTipoTicketPedidoEspecial, idTicketPedidoEspecial: idTicketPedidoEspecial, ticketFinal: ticketFinal },
        method: 'post',
        dataType: 'html',
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



function InitDataTableConsultaPedidosEnRuta() {
    var NombreTabla = "tablaConsultaPedidosEnRuta";
    tablaConsultaPedidosEnRuta = initDataTable(NombreTabla);

    if ($("#tablaConsultaPedidosEnRuta").length > 0) {
        new $.fn.dataTable.Buttons(tablaConsultaPedidosEnRuta, {
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7]
                    },
                },
            ],

        });

        tablaConsultaPedidosEnRuta.buttons(0, null).container().prependTo(
            tablaConsultaPedidosEnRuta.table().container()
        );

    }
}



function VerTicket(idVenta) {
    $.ajax({
        url: rootUrl("/Ventas/VerTicket"),
        data: { idVenta: idVenta },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            OcultarLoader();
            console.log(data);
            window.open("http://" + window.location.host + data.Modelo, "_blank");
            console.log("http://" + window.location.host + data.Modelo);
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}


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


$(document).ready(function () {

    InitSelect2Multiple();
    InitDataTableConsultaPedidosEnRuta();
    InitRangePicker('rangeConsultaPedidosEnRuta', 'fechaIni', 'fechaFin');
    //$("#btnBuscarPedidosEnRuta").trigger('click');

    //$('#fechaIni').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

});