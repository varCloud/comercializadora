var table;
var iframe;
var tablaConsultaEntregarPedidos; 

//busqueda
function onBeginSubmitConsultaEntregarPedido() {
    ShowLoader("Buscando...");
}

function onCompleteSubmitConsultaEntregarPedido() {
    console.log("onCompleteSubmitConsultaEntregarPedido");
}

function onFailureResultConsultaEntregarPedido() {
    OcultarLoader();
}

function onSuccessResultConsultaEntregarPedido(data) {
    $('#rowConsultaEntregarPedidos').html(data);
    tablaConsultaEntregarPedidos.destroy();
    InitDataTableConsultaEntregarPedidos();
    OcultarLoader();
}





function InitDataTableConsultaEntregarPedidos() {
    var NombreTabla = "tablaConsultaEntregarPedidos";
    tablaConsultaEntregarPedidos = initDataTable(NombreTabla);

    if ($("#tablaConsultaEntregarPedidos").length > 0) {
        new $.fn.dataTable.Buttons(tablaConsultaEntregarPedidos, {
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
                        doc.content[1].table.widths = ['5%', '20%', '15%', '10%', '10%', '15%', '10%', '15%'];
                        doc.pageMargins = [30, 85, 20, 30];
                        doc.content.splice(0, 1);
                        doc['header'] = SetHeaderPDF("Ventas");
                        doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                    },
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7]
                    },
                },
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

        tablaConsultaEntregarPedidos.buttons(0, null).container().prependTo(
            tablaConsultaEntregarPedidos.table().container()
        );

    }
}


function imprimirTicketPedidoEspecial(idPedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/imprimirTicketPedidoEspecial"),
        data: { idPedidoEspecial: idPedidoEspecial },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            MuestraToast("info", data.Mensaje);
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
        }
    });
}

//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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


function ImprimeTicketDevolucion(idVenta, idDevolucion) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicketDevolucion"),
        data: { idVenta: idVenta, idDevolucion: idDevolucion },
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

function ImprimeTicketComplemento(idVenta, idComplemento) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicketComplemento"),
        data: { idVenta: idVenta, idComplemento: idComplemento },
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

function ImprimeTicketDespachadores(idVenta) {
    $.ajax({
        url: rootUrl("/Ventas/ImprimeTicketDespachadores"),
        data: { idVenta: idVenta},
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

function eliminaArchivo(rutaArchivo) {
    $.ajax({
        url: rootUrl("/Productos/EliminaArchivo"),
        data: { 'rutaArchivo': rutaArchivo },
        method: 'post',
        dataType: 'json',
        //contentType: "text/xml",
        async: true,
        beforeSend: function (xhr) {
            ShowLoader()
        },
        success: function (data) {
            console.log(data);
            OcultarLoader();
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

//function VerTicket(idVenta) {
//    $.ajax({
//        url: rootUrl("/Ventas/ImprimeTicket"),
//        data: { idVenta: idVenta, ticketVistaPrevia: true },
//        method: 'post',
//        dataType: 'json',
//        async: true,
//        beforeSend: function (xhr) {
//            ShowLoader("Generando Ticket...");
//        },
//        success: function (data) {
//            console.log(data);
//            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
//            OcultarLoader();

//            if (data.Estatus == 200)
//            {
//                setTimeout(function () { window.open("http://" + window.location.host + "/Tickets/" + idVenta + "_preview.pdf", "_blank"); }, 4000);
//            }
//        },
//        error: function (xhr, status) {
//            OcultarLoader();
//            MuestraToast('error', "Ocurrio un error al enviar el ticket a la impresora.");
//            console.log(xhr);
//            console.log(status);
//            console.log(data);
//        }
//    });
//}


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



function PintarTabla() {
    $.ajax({
        url: rootUrl("/Ventas/_ObtenerVentas"),
        data: { idVenta: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaConsultaEntregarPedidos.destroy();
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




function CancelaVenta(idVenta) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a esta Venta?',
        icon: '',
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
                       
                        if (data.Estatus == 200) {
                            MuestraToast('success', data.Mensaje);
                            ImprimeTicketVentaCancelada(idVenta);
                            PintarTabla();
                        }
                        else {
                            MuestraToast('error', data.Mensaje);
                        }
                        
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
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader("Cancelando Factura.");
                        console.log("Antes ")
                    },
                    success: function (data) {
                        OcultarLoader();
                        MuestraToast('success', data.Mensaje);
                        PintarTabla();
                    },
                    error: function (xhr, status) {
                        OcultarLoader();
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
    $('#idClienteIVA').val("0");//.trigger('change');
    $('#formaPago').val("1").trigger('change');
    $('#usoCFDI').val("3").trigger('change');
    $('#idVentaIVA').val(0);

}


function modalFacturar(idVenta) {

    limpiaModalIVA();
    var data = ConsultaVenta(idVenta);
    //console.log(data);

    // ACUTALIZACION PARA FACTURAR TARJETA de CREDITO Y DEBITO
    // EL TEMA DE COMISIONES  NOS PEGA EN ESTA PARTE YA QUE COBRAMOS COMISIONES POR DESLIZAR LA TARJETA
    // Y CUANDO SE FACTURA DESDE EL MODULO DE VENTAS SE CONDONAN COMISIONES ES POR ESTO QUE HAY QUE CANCELAR VENTA
    if (data.Modelo.idFactFormaPago == 4 || data.Modelo.idFactFormaPago == 18) {
        MuestraToast('warning', "Debe primero cancelar esta venta, ya que se han cobrado comisiones por uso de TC/TD");
        return;
    }

    if (data.Modelo.tieneCompleODev == 1) {
        MuestraToast('warning', "No es posible facturar esta venta ya que contiene devoluciones o complementos");
        return;
    }
    

    var montoTotal = parseFloat(data.Modelo.montoTotal).toFixed(2);
    var montoIVA = parseFloat(data.Modelo.montoTotal * 0.16).toFixed(2);
    var montoFinal = parseFloat(montoTotal) + parseFloat(montoIVA);
    var idCliente = parseInt(data.Modelo.idCliente);
    
    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(montoTotal).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4><strike>$" + parseFloat(montoTotal).toFixed(2) + "</strike></h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(montoIVA).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(montoIVA).toFixed(2) + "</h4>";

    $('#idVentaIVA').val(idVenta);
    $('#idClienteIVA').val(idCliente).trigger('change'); 
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
        //url: rootUrl("/Factura/GenerarFactura"),
        //data: { idVenta: idVenta },
        url: pathDominio+"api/WsFactura/GenerarFactura",
        data: { idVenta: idVenta, idUsuario: idUsuarioGlobal },
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
    //console.log($('#idClienteIVA').val());
    var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
    var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

    if ($('#idClienteIVA').val() == "0") {
        MuestraToast('warning', "Debe seleccionar un Cliente.");
        return;
    }
    
    if ($('#efectivo').val() == "") {
        MuestraToast('warning', "Debe escribir con cuanto efectivo le estan pagando.");
        return;
    }

    if (parseFloat(efectivo_) < parseFloat(total_)) {
        MuestraToast('warning', "El efectivo no alcanza a cubrir el costo del iva faltante: " + total_.toString());
        return;
    }

    if ($('#idClienteIVA').val() == "1") {
        MuestraToast('warning', "Debe seleccionar un cliente diferente a " + $("#idClienteIVA").find("option:selected").text());
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

    if (event.keyCode === 13) {

        event.preventDefault();
        document.getElementById("btnGuardarIVA").click();

    }
    else {

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
    }

});




$("#idClienteIVA").on("change", function () {
    $("#divUsoCFDI").hide();
    $("#btnGuardarIVA").hide();
    $("#efectivoFactura").hide();
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

    if ((data.Modelo.idCliente != 0) && (idCliente != 0)) {
        if (!validarEmail(data.Modelo.correo)) {
            MuestraToast('warning', "No es posible facturar a un cliente sin correo electrónico vàlido");  
            return false;
        }

        if (!validarRFC(data.Modelo.rfc)) {
            MuestraToast('warning', "No es posible facturar a un cliente sin RFC vàlido");    
            return false;
        }
    }
    else {
        MuestraToast('warning', "No es posible facturar a  este cliente");
        return false;
    }

    $("#divUsoCFDI").show();
    $("#btnGuardarIVA").show();
    $("#efectivoFactura").show();


});



function modalVerDetalleTickets(idVenta) {

   
    $.ajax({
        url: rootUrl("/Ventas/_ObtenerDetalleTickets"),
        data: { idVenta: idVenta },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $('#detallesTickets').html(data);
            //feather.replace();
               
        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
    


    $('#modalVerDetalleTickets').modal({ backdrop: 'static', keyboard: false, show: true });

}


function ImrimirTodosTickets(idVenta) {

    alert(idVenta);

}



$(document).ready(function () {

    InitSelect2Multiple();
    InitDataTableConsultaEntregarPedidos();
    InitRangePicker('rangeConsultaVentas', 'fechaIni', 'fechaFin');
    //$('#rangeConsultaVentas').val('');
    $('#codigoBarrasTicket').focus();
    $("#btnBuscarEntregarPedido").trigger('click');
    //$('#fechaIni').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

});