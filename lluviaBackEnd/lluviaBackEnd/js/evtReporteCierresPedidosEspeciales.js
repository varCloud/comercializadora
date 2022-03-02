var table;
var iframe;
var tblCierresPE;

//busqueda
function onBeginSubmitCierresPE() {
    ShowLoader('Buscando...');
}

function onSuccessResultCierresPE(data) {
    OcultarLoader();

    var html = "";
    if (data.length == 0) {
        html = '<div class="empty-state">' +
            '<div class="empty-state-icon" >' +
            '   <i class="fas fa-info"></i>' +
            '</div>' +
            '<h2> No se encontraron resultados</h2> ' +
            '</div>';
    }
    else {



        html = '<div class="table-responsive">' +
            '<table class="table table-striped" id = "tablaRepCierresPE">' +
            '    <thead>' +
                    '<tr>' +
                        '<th>Fecha</th>' +
                        '<th>Cajero</th>' +
                        //'<th>Total contado</th>' +
                        //'<th>Total TC & TD</th>' +
                        //'<th>Total Transferencias</th>' +
                        //'<th>Total otras formas de pago</th>' +
                        //'<th>Total crèdito</th>' +
                        '<th>Total devoluciones</th>' +
                        '<th>Ingresos de efectivo</th>' +
                        '<th>Retiros de efectivo</th>' +
                        '<th>Saldo final efectivo</th>' +
                        '<th>Saldo final TC y transferencias</th>' +
                        '<th>Efectivo entregado</th>' +
                        //'<th>No. devoluciones</th>' +
                        //'<th>No. tickets efectivo</th>' +
                        //'<th>No. tickets credito</th>' +
                        //'<th>No. pedidos en resguardo</th>' +
                        '<th>Imprimir</th>' +
                    '</tr>' +
            ' </thead>' +
            ' <tbody>';
        for (i = 0; i < data.length; i++) {
            var fecha = new Date(parseInt(data[i].fechaCierre.substr(6)));

            html += '<tr>' +
                '             <td>' + completarCeros(fecha.getDate()) + "/" + completarCeros(parseInt(fecha.getMonth()) + 1) + "/" + fecha.getFullYear() + '</td>' +
                '             <td>' + data[i].nombreUsuario + '</td>' +
                //'             <td>' + formatoMoneda(data[i].VentasContado) + '</td>' +
                //'             <td>' + formatoMoneda(data[i].VentasTC) + '</td>' +
                //'             <td>' + formatoMoneda(data[i].VentasTransferencias) + '</td>' +
                //'             <td>' + formatoMoneda(data[i].VentasOtrasFormasPago) + '</td>' +
                //'             <td>' + formatoMoneda(data[i].VentasCredito) + '</td>' +
                '             <td>' + formatoMoneda(data[i].MontoDevoluciones) + '</td>' +
                '             <td>' + formatoMoneda(data[i].MontoIngresosEfectivo) + '</td>' +
                '             <td>' + formatoMoneda(data[i].MontoRetirosEfectivo) + '</td>' +
                '             <td>' + formatoMoneda(data[i].MontoCierreEfectivo) + '</td>' +
                '             <td>' + formatoMoneda(data[i].MontoCierreTC) + '</td>' +
                '             <td>' + formatoMoneda(data[i].EfectivoEntregadoEnCierre) + '</td>' +
                //'             <td>' + data[i].noDevoluciones + '</td>' +
                //'             <td>' + data[i].NoTicketsEfectivo + '</td>' +
                //'             <td>' + data[i].NoTicketsCredito + '</td>' +
                //'             <td>' + data[i].NoPedidosEnResguardo + '</td>' +
                '             <td>' +
                '               <div class="buttons">' +
                '                   <a href="javascript:ImprimeTicketCierrePE(' + data[i].idCierrePedidoEspecial + ');" class="btn btn-icon btn-primary" data-toggle="tooltip" title="Imprimir ticket"><i class="fas fa-print"></i></a>' +
                '               </div>' +
                '             </td>' +
                '</tr>';
        }
        html += ' </tbody>' +
            '</table>' +
            '</div>';
    }
    $('#resultCierres').html(html);
    if (data.length > 0) {
        if (tblCierresPE != null)
            tblCierresPE.destroy();
         InitDataTableCierresPE();
    }
}

function completarCeros(valor) {
    return ('00' + valor).slice(-2);
}


function onFailureResultCierresPE() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar los cierres");
}


function InitDataTableCierresPE() {
    var NombreTabla = "tablaRepCierresPE";
    tblCierresPE = initDataTable(NombreTabla);
    if ($("#tablaRepCierresPE").length > 0) {
        new $.fn.dataTable.Buttons(tblCierresPE, {
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4,5,6,7]
                    },
                },
            ],

        });

        tblCierresPE.buttons(0, null).container().prependTo(
            tblCierresPE.table().container()
        );
    }
}

function ImprimeTicketCierrePE(idCierrePedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ImprimeTicketCierreCajas"),
        data: { idCierrePedidoEspecial: idCierrePedidoEspecial },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Imprimiendo ticket...");
        },
        success: function (data) {
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


$(document).ready(function () {   
    InitSelect2();
    InitRangePicker('rangeCierres', 'fechaIni', 'fechaFin');
    $('#rangeCierres').val('');

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarCierresPE").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarCierresPE .select-multiple").trigger("change");

    });

});