var table;
var iframe;
var tablaConsultaVentas; 

//busqueda
function onBeginSubmitConsultaVentas() {
    console.log("onBeginSubmitConsultaVentas");
}
function onCompleteSubmitConsultaVentas() {
    console.log("onCompleteSubmitConsultaVentas");
}
function onSuccessResultConsultaVentas(data) {
    console.log("onSuccessResultVentas", JSON.stringify(data) );    
    tablaConsultaVentas.destroy();
    $('#rowConsultaVentas').html(data);
    InitDataTableConsultaVentas();
}
function onFailureResultConsultaVentas() {
    console.log("onFailureResult___");
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


function ImprimeTicket(idVenta) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicket"),
        data: { idVenta: idVenta },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            console.log(data);
            MuestraToast('success', "Se envio el ticket a la impresora.");
        },
        error: function (xhr, status) {
            MuestraToast('error', "Ocurrio un error al enviar el ticket a la impresora.");
            console.log(xhr);
            console.log(status);
            console.log(data);
        }
    });
}

function PintarTabla() {
    $.ajax({
        url: "/Ventas/_ObtenerVentas",
        data: { idVenta: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaConsultaVentas.destroy();
            $('#rowConsultaVentas').html(data);
            InitDataTableConsultaVentas();
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitDataTableConsultaVentas() {
    var NombreTabla = "tablaConsultaVentas";
    tablaConsultaVentas = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaConsultaVentas, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Ventas",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '20%', '20%', '10%', '20%', '20%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Ventas");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
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

    tablaConsultaVentas.buttons(0, null).container().prependTo(
        tablaConsultaVentas.table().container()
    );
}



function CancelaVenta(idVenta) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a esta Venta?',
        icon: 'warning',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Ventas/CancelaVenta"),
                    data: { idVenta: idVenta },
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



function limpiaModalIVA() {

    var row_ = "<address>" +
        "    <strong></strong><br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "</address>";

    document.getElementById("nombreCliente").innerHTML = row_;

    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    //document.getElementById("previoDescuentoCliente").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    $('#efectivo').val('');
    $('#idClienteIVA').val("0").trigger('change');
    $('#formaPago').val("1").trigger('change');
    $('#usoCFDI').val("1").trigger('change');
    $('#idVentaIVA').val(0);

}


function modalFacturar(idVenta) {

    limpiaModalIVA();

    var data = ConsultaVenta(idVenta);
    //console.log(data);

    var montoTotal = parseFloat(data.Modelo.montoTotal).toFixed(2);
    var montoIVA = parseFloat(data.Modelo.montoTotal * 0.16).toFixed(2);
    var montoFinal = parseFloat(montoTotal) + parseFloat(montoIVA);

    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(montoTotal).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(montoTotal).toFixed(2) + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(montoIVA).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(montoFinal).toFixed(2) + "</h4>";

    $('#idVentaIVA').val(idVenta);
    $('#ModalFacturar').modal({ backdrop: 'static', keyboard: false, show: true });

}



function ConsultaVenta(idVenta) {

    var result = '';
    $.ajax({
        url: rootUrl("/Ventas/ConsultaVenta"),
        data: { idVenta: idVenta },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            OcultarLoader();
            result = data;
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

function GenerarFactura(idVenta) {
    //console.log("idventa", idVenta)
    //ShowLoader();
    
    $.ajax({
        url: rootUrl("/Factura/GenerarFactura"),
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
            $('#ModalFacturar').modal('hide');
            PintarTabla();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });   
    
}



function ObtenerCliente(idCliente) {
    var result = "";//{ "Estatus": -1, "Mensaje": "Espere un momento y vuelva a intentarlo" };
    $.ajax({
        url: rootUrl("/Clientes/ObtenerCliente"),
        data: { idCliente: idCliente },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
            console.log("Antes_")
        },
        success: function (data) {
            result = data;
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
    return result;
}




$('#btnGuardarIVA').click(function (e) {
    console.log($('#idClienteIVA').val() );
    if ($('#idClienteIVA').val() == "0") {
        MuestraToast('warning', "Debe seleccionar un Cliente.");
        return;
    }

    var montoIVA = parseFloat(document.getElementById("previoIVA").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    var idVenta = $('#idVentaIVA').val();
    var idCliente = $('#idClienteIVA').val();
    var formaPago = $('#formaPago').val();
    var usoCFDI = $('#usoCFDI').val();

    GuardarIVA(idVenta, montoIVA, idCliente, formaPago, usoCFDI);
    
});



function GuardarIVA(idVenta, montoIVA, idCliente, formaPago, usoCFDI) {

    $.ajax({
        url: rootUrl("/Ventas/GuardarIVA"),
        data: { idVenta: idVenta, montoIVA: montoIVA, idCliente: idCliente, idFactFormaPago: formaPago, idFactUsoCFDI: usoCFDI },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            OcultarLoader();

            if (data.Estatus == 200) {
                GenerarFactura(idVenta);
            }
            
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });

}

$("#efectivo").on("keyup", function () {
    var cambio_ = parseFloat(0).toFixed(2);
    var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
    var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

    if (parseFloat(efectivo_) > parseFloat(total_)) {
        cambio_ = efectivo_ - total_;
        document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>";
    }
    else {
        document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    }
});




$("#idClienteIVA").on("change", function () {

    $('#efectivo').val('');
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    var idCliente = parseFloat($('#idClienteIVA').val());
    var data = ObtenerCliente(idCliente);
    var nombre = data.Modelo.nombres + "  " + data.Modelo.apellidoPaterno + "  " + data.Modelo.apellidoMaterno;
    console.log(data );
    // para los datos del cliente
    var row_ = "<address>" +
        "    <strong></strong><br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "    <br>" +
        "</address>";

    if ((data.Modelo.idCliente != 0) && (idCliente != 0)) {
        row_ = "<address>" +
            "    <strong>Datos del Cliente:</strong><br>" +
            "    Nombre: " + nombre.toUpperCase() + "<br>" +
            "    Telefono: " + data.Modelo.telefono + "<br>" +
            "    E-mail: " + data.Modelo.correo + "<br>" +
            "    RFC: " + data.Modelo.rfc + "<br>" +
            "    Tipo de Cliente: " + data.Modelo.tipoCliente.descripcion + "<br>" +
            "</address>";
    }


    document.getElementById("nombreCliente").innerHTML = row_;

});






$(document).ready(function () {

    InitSelect2Multiple();
    InitDataTableConsultaVentas();
    InitRangePicker('rangeConsultaVentas', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

});