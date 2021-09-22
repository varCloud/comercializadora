var tblFacturas
$(document).ready(function () {
    if ($("#tblFacturas").length > 0) {
        InitTableFacturas();
    }
    
    InitRangePicker('rangeFacturas', 'fechaIni', 'fechaFin');
    //$('#fechaIni').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

    $('#rangeFacturas').val('');
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
        $("#frmBuscarFacturas").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
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
                    doc.content[1].table.widths = ['10%', '30%', '30%', '10%', '20%'];
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
    $("#DivtblFacturas").html(data);
    if ($("#tblFacturas").length > 0) {
        tblFacturas.destroy();
        InitTableFacturas();
    }

    OcultarLoader();
}
function onFailureResultObtenerFacturas() {
    OcultarLoader();
}


function CancelarFactura(idVenta) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas cancelar la factura de Venta?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    //url: rootUrl("/Factura/CancelarFactura"),
                    url: pathDominio + "api/WsFactura/CancelarFactura",
                    data: { idVenta: idVenta, idUsuario: idUsuarioGlobal },
                    method: 'post',
                    dataType: 'json',
                    async: false,
                    beforeSend: function (xhr) {
                        console.log("Antes ")
                    },
                    success: function (data) {
                        MuestraToast('success', data.Mensaje);
                        PintarTabla();
                    },
                    error: function (xhr, status) {
                        console.log('Hubo un problema al intentar eliminar al usuario, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });

            } else {
                console.log("cancelar");
            }
        });
}


//reenviar factura

function limpiaModalFactura() {
    $("#idVentaIVA").val("");

    var row_ = "<address>" +
        "    <strong></strong><br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "</address>";

    $("#nombreCliente").html(row_);
    $("#FormaPago").html("");
    $("#usoCFDI").html("");
    $("#previoTotal").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");
    $("#previoSubTotal").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");
    $("#previoIVA").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");
    $("#previoFinal").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");
    document.getElementById("chkEnviarCopia").checked = false;
    $("#correoCopia").val("");
    $("#divCorreoCopia").hide();
}

$('#chkEnviarCopia').click(function () {
    $("#correoCopia").val("");
    $("#divCorreoCopia").hide();
    if ($('#chkEnviarCopia').is(':checked')) {
        $("#divCorreoCopia").show();
    }


});

$('#btnReenviar').click(function () {
    var correo = $("#correoCopia").val();
    var idVenta = $("#idVentaIVA").val();
    if ($('#chkEnviarCopia').is(':checked')) {
        if (!validarEmail(correo)) {
            MuestraToast('warning', "Introduzca un correo electrónico vàlido");          
            return false;
        }
    }

    $.ajax({
        url: pathDominio + "api/WsFactura/ReenviarFactura",
        data: { idVenta: idVenta, correoAdicional: correo, idUsuario: idUsuarioGlobal  },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Reenviando factura")
        },
        success: function (data) {
            OcultarLoader();
            if (data.Estatus != 200) {
                MuestraToast('error', data.Mensaje);
                return;
            }
            else
                MuestraToast('success', data.Mensaje);
            $('#ModalFactura').modal('hide');
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });




});
    

function modalFactura(idVenta) {
    limpiaModalFactura();
    var data = ConsultaDetalleFactura(idVenta);
    //console.log(data);

    if (data.Estatus != 200) {
        MuestraToast('error', data.Mensaje);
        return;
    }

    var montoTotal = parseFloat(data.Modelo.montoTotal).toFixed(2);
    var montoIVA = parseFloat(data.Modelo.montoTotal * 0.16).toFixed(2);
    var montoFinal = parseFloat(montoTotal) + parseFloat(montoIVA);

    $("#previoTotal").html("<h4>$" + parseFloat(montoTotal).toFixed(2) + "</h4>");
    $("#previoSubTotal").html("<h4><strike>$" + parseFloat(montoTotal).toFixed(2) + "</strike></h4>");
    $("#previoIVA").html("<h4>$" + parseFloat(montoIVA).toFixed(2) + "</h4>");
    $("#previoFinal").html("<h4>$" + parseFloat(montoIVA).toFixed(2) + "</h4>");

    $('#idVentaIVA').val(idVenta);

    var row_ = "<address>" +
        "    <strong>Datos del Cliente:</strong><br>" +
        "    Nombre: " + data.Modelo.Nombre.toUpperCase() + "<br>" +
        "    Telefono: " + data.Modelo.telefono + "<br>" +
        "    E-mail: " + data.Modelo.correo + "<br>" +
        "    RFC: " + data.Modelo.Rfc + "<br>" +
        "    Tipo de Cliente: " + data.Modelo.tipoCliente + "<br>" +
        "</address>";

    $("#nombreCliente").html(row_);
    $("#FormaPago").html(data.Modelo.descripcionFormaPago);
    $("#usoCFDI").html(data.Modelo.descripcionUsoCFDI);

    $('#ModalFactura').modal({ backdrop: 'static', keyboard: false, show: true });

}

function ConsultaDetalleFactura(idVenta) {

    var result = '';
    $.ajax({
        url: rootUrl("/Factura/ObtenerDetalleFactura"),
        data: { idVenta: idVenta },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            OcultarLoader();
            console.log(JSON.parse(data));
            result = JSON.parse(data);
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });

    return result;
}




