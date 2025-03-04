var table;
var iframe;
var tablaConsultaPedidosEnRuta; 
var tblDetallePedidoRuta;

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
                        columns: [0, 1, 2, 3, 4, 5]
                    },
                },
            ],

        });

        tablaConsultaPedidosEnRuta.buttons(0, null).container().prependTo(
            tablaConsultaPedidosEnRuta.table().container()
        );

    }
}



function VerTicket(idPedidoEspecial, idTipoTicketPedidoEspecial, idTicketPedidoEspecial) {
    console.log(`VerTicket`)
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/VerTicket"),
        data: { idPedidoEspecial: idPedidoEspecial, idTipoTicketPedidoEspecial: idTipoTicketPedidoEspecial, idTicketPedidoEspecial: idTicketPedidoEspecial },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Cargando...");
        },
        success: function (data) {
            OcultarLoader();
            downloadPDF(data, "Ticket" + idPedidoEspecial + ".pdf");
            //console.log(data);
        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast("error", "Hubo un problema al generar el pdf, contactese con el administrador del sistema");
            console.log(xhr);
            console.log(status);
        }
    });

}


function downloadPDF(pdf, nombre) {
    //abrir en otra pestaña
    let pdfWindow = window.open("")
    pdfWindow.document.write(
        "<iframe width='100%' height='100%' src='data:application/pdf;base64, " +
        encodeURI(pdf) + "'></iframe>"
    )
    return;
    //descargar
    const linkSource = `data:application/pdf;base64,${pdf}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = nombre;
    downloadLink.click();
}

function ImprimeTicket(idPedidoEspecial, idTipoTicketPedidoEspecial, idTicketPedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ImprimeTicket"),
        data: { idPedidoEspecial: idPedidoEspecial, idTipoTicketPedidoEspecial: idTipoTicketPedidoEspecial, idTicketPedidoEspecial: idTicketPedidoEspecial },
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



function MostrarDetallePedidoRuta(idPedidoEspecial) {

    $("#idPedidoEspecial").val(0);

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ObtenerPedidosEspecialesDetalle"),
        data: { idPedidoEspecial: idPedidoEspecial },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Cargando...");
        },
        success: function (data) {
            OcultarLoader();
            var html = "";
            var result = JSON.parse(data);
            if (result.Estatus === 200) {
                html += '<div class="table-responsive">';
                html += '<table class="table table-striped" id="tblDetallePedidoRuta" style="width: 100%;">';
                html += '<thead>';
                html += '<tr>';
                html += '<th>Id Producto</th>';
                html += '<th>Producto</th>';
                html += '<th>Almacen</th>';
                html += '<th>Precio</th>';
                html += '<th>Cantidad</th>';
                html += '<th>Total</th>';
                //html += '<th>Devolver articulos</th>';
                //html += '<th class="text-center" style="display: none;">idPedidoEspecialDetalle</th>';
                //html += '<th class="text-center" style="display: none;">montoComisionBancaria</th>';
                html += '</tr>';
                html += '</thead>';
                html += '<tbody>';
                $.each(result.Modelo, function (index, dato) {

                    if (dato.cantidad > 0) {


                        html += '<tr>' +
                            '             <td>' + dato.idProducto + '</td>' +
                            '             <td>' + dato.descripcion + '</td>' +
                            '             <td>' + dato.Almacen + '</td>' +
                            '             <td>' + formatoMoneda(dato.precioVenta) + '</td>' +
                            '             <td>' + dato.cantidad + '</td>' +
                            '             <td>' + formatoMoneda(dato.monto) + '</td>';
                        //if (dato.fraccion)
                        //    html += '<td><input class="esDevolucion" type="text" onchange="fnActualizaDevolucion(this)" onkeypress="return esDecimal(this, event);" style="text-align: center; border: none; border-color: transparent; background: transparent;" value="0"></td>';
                        //else
                        //    html += '<td><input class="esDevolucion" type="text" onchange="fnActualizaDevolucion(this)" onkeypress="return esNumero(event)" style="text-align: center; border: none; border-color: transparent; background: transparent;" value="0"></td>';

                        html += '</tr>';
                    }
                });
                html += '</tbody>';
                html += '</table>';
                html += '</div>';

                $("#detallePedidoRuta").html(html);
                $("#idPedidoEspecial").val(idPedidoEspecial);

                if (tblDetallePedidoRuta != null)
                {
                    tblDetallePedidoRuta.destroy();
                }
                InitDataTableDetallePedidoRuta();

                $('#modalDetallePedidoRuta').modal({ backdrop: 'static', keyboard: false, show: true });
            }
            else
                MuestraToast("error", result.Mensaje);

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al guardar la compra, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });

}

function InitDataTableDetallePedidoRuta() {
    var NombreTabla = "tblDetallePedidoRuta";
    tblDetallePedidoRuta = initDataTable(NombreTabla);

    if ($("#tblDetallePedidoRuta").length > 0) {
        new $.fn.dataTable.Buttons(tblDetallePedidoRuta, {
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5]
                    },
                },
            ],

        });

        tblDetallePedidoRuta.buttons(0, null).container().prependTo(
            tblDetallePedidoRuta.table().container()
        );

    }

}

$(document).ready(function () {

    InitSelect2Multiple();
    InitDataTableConsultaPedidosEnRuta();
    InitRangePicker('rangeConsultaPedidosEnRuta', 'fechaIni', 'fechaFin');
    //$("#btnBuscarPedidosEnRuta").trigger('click');

    //$('#fechaIni').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

});