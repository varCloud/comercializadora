var table;
var iframe;
var tblPedidosEspeciales;
var tblPedidosEspecialesDet;
var tblDevolucionesPedidosEspeciales;

//busqueda
function onBeginSubmitPedidosEspeciales() {
    ShowLoader('Buscando...');
}

function onSuccessPedidosEspeciales(data) {
    OcultarLoader();
    var html = "";
    var result = JSON.parse(data);
    if (result.Estatus !== 200) {
        html = '<div class="empty-state">' +
            '<div class="empty-state-icon" >' +
            '   <i class="fas fa-info"></i>' +
            '</div>' +
            '<h2> No se encontraron resultados</h2> ' +
            '</div>';
    }
    else {

        html = '<div class="table-responsive">' +
            '<table class="table table-striped" id = "tblPedidosEspeciales">' +
            '    <thead>' +
            '     <tr>' +
            '         <th></th>' +
            '         <th>No. pedido especial</th>' +
            '         <th>Fecha</th>' +
            '         <th>Cliente</th>' +
            '         <th>Usuario</th>' +
            '         <th>Cantidad</th>' +
            '         <th>Monto Total</th>' +
            '         <th>Estatus</th>' +
            '         <th>Facturado</th>' +
            '         <th>Liquidado</th>' +
            '         <th>Codigo de barras</th>' +
            '         <th>Observaciones</th>' +
            '         <th>Acciones</th>' +
            '     </tr>' +
            ' </thead>' +
            ' <tbody>';



        $.each(result.Modelo, function (index, dato) {
            //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));

            var estatus = "";
            switch (dato.idEstatusPedidoEspecial) {
                case 1://solicitado
                    estatus = '<div class="badge badge-light badge-shadow">' + dato.estatusPedidoEspecial + '</div>';
                    break;
                case 2://cotizado
                    estatus = '<div class="badge badge-warning badge-shadow">' + dato.estatusPedidoEspecial + '</div>';
                    break;
                case 3:// en resguardo
                    estatus = '<div class="badge badge-info badge-shadow">' + dato.estatusPedidoEspecial + '</div>';
                    break;
                case 4://entregado y pagado
                    estatus = '<div class="badge badge-success badge-shadow">' + dato.estatusPedidoEspecial + '</div>';
                    break;
                case 5://entregado a repartidor sin ser pagado
                    estatus = '<div class="badge badge-primary badge-shadow">' + dato.estatusPedidoEspecial + '</div>';
                    break;
                case 6://Pagado
                    estatus = '<div class="badge badge-success badge-shadow">' + dato.estatusPedidoEspecial + '</div>';
                    break;
                case 7://Entregado a crédito
                    estatus = '<div class="badge badge-danger badge-shadow">' + dato.estatusPedidoEspecial + '</div>';
                    break;
                default:
                    estatus = '<div class="badge badge-light badge-shadow">' + dato.estatusPedidoEspecial + '</div>';
                    break;

            }

            var liquidado = "";

            if (dato.liquidado)
                liquidado = '<div class="badge badge-success badge-shadow">SI</div>';
            else
                liquidado = '<div class="badge badge-danger badge-shadow">NO</div>';

            html += '<tr>' +
                '             <td class="details-control" idPedidoEspecial="'+dato.idPedidoEspecial+'" ></td>' +
                '             <td>' + dato.idPedidoEspecial + '</td>' +
                '             <td>' + dato.fechaAlta + '</td>' +
                '             <td>' + dato.nombreCliente + '</td>' +
                '             <td>' + dato.nombreUsuario + '</td>' +
                '             <td>' + dato.cantidad + '</td>' +
                '             <td>' + formatoMoneda(dato.montoTotal) + '</td>' +
                '             <td>' + estatus + '</td>' +
                '             <td>' + dato.facturado + '</td>' +
                '             <td>' + liquidado + '</td>' +
                '             <td>' + dato.codigoBarras + '</td>' +
                '             <td>' + dato.observaciones + '</td>' +
                '             <td>' +
                '               <div class="dropdown d-inline">' +
                '                   <button class="btn btn-primary dropdown-toggle" type="button" id="menuAccionesVentas" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Acciones</button>' +
                '                       <div class="dropdown-menu">' +
                '                           <a class="dropdown-item has-icon" href="javascript:MostrarDetalle(' + dato.idPedidoEspecial + ');"><i class="fas fa-eye"></i>Ver Detalle</a>';
                if (
                    (dato.idEstatusPedidoEspecial == 6) || // pagado
                    (dato.idEstatusPedidoEspecial == 7)    // a credito
                ) {
                    html += '<a class="dropdown-item has-icon" href="javascript:ImprimeTicket(' + dato.idPedidoEspecial + ',1,0);"><i class="fas fa-print"></i>Ticket Original</a>';
                }
            html +='<a class="dropdown-item has-icon" href="javascript:imprimirTicketAlmacenes(' + dato.idPedidoEspecial + ');"><i class="fas fa-print"></i>Imprimir Ticket Almacen </a>';
            html += '<a class="dropdown-item has-icon" href="' + rootUrl("PedidosEspecialesV2/VerTicketAlmacenes?idPedidoEspecial=" + dato.idPedidoEspecial + "") + '" target="_blank"><i class="fas fa-eye"></i>Ver Ticket Almacen</a>';
            if (dato.existe_ticket==true)
                    html += ' <a class="dropdown-item has-icon" href="javascript:Tickets(' + dato.idPedidoEspecial + ');"><i class="fas fa-list"></i>Tickets</a>';
            if (dato.puede_devolver == true)
                html += '<a class="dropdown-item has-icon" href="javascript:MostrarDetalleDevolucion(' + dato.idPedidoEspecial + ');" > <i class="far fa-minus-square"></i>Devolver Productos</a>';

            if (dato.puede_facturar == true)
                //html += ' <a class="dropdown-item has-icon" href="javascript:FacturarPedidoEspecial(' + dato.idPedidoEspecial + ');" > <i class="fas fa-file-invoice-dollar"></i>Facturar</a>';
                html += ' <a class="dropdown-item has-icon" href="javascript:modalFacturar(' + dato.idPedidoEspecial + ');" > <i class="fas fa-file-invoice-dollar"></i>Facturar</a>';

            html += '                       </div>' +
                '                           </div>' +
                '           </td>'
            '       </tr>';
        });
        html += ' </tbody>' +
            '</table>' +
            '</div>';
    }
    $('#resultPedidosEspeciales').html(html);
    if (result.Estatus == 200) {
        if (tblPedidosEspeciales != null)
            tblPedidosEspeciales.destroy();
        InitDataTablePedidosEspeciales();
       
    }
}

function initClickDetalle() {
    $('#tblPedidosEspeciales tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tblPedidosEspeciales.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        } else {
            // Open this row
            //  console.log(obtenerDetalleBitacora($(this).attr("idPedido")));
            $.ajax({
                url: rootUrl("/Bitacora/_DetalleBitacora"),
                data: { idPedidoInterno: $(this).attr("idPedido") },
                method: 'post',
                dataType: 'html',
                async: true,
                beforeSend: function (xhr) {
                    ShowLoader();
                },
                success: function (view) {
                    OcultarLoader();
                    row.child(view).show();
                    tr.addClass('shown');
                },
                error: function (xhr, status) {
                    OcultarLoader();
                }
            });

        }
    });
}

function completarCeros(valor) {
    return ('00' + valor).slice(-2);
}


function onFailurePedidosEspeciales() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar los cierres");
}


function InitDataTablePedidosEspeciales() {
    var NombreTabla = "tblPedidosEspeciales";
    tblPedidosEspeciales = initDataTable(NombreTabla);
    if ($("#tblPedidosEspeciales").length > 0) {
        new $.fn.dataTable.Buttons(tblPedidosEspeciales, {
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
                    },
                },
            ],

        });

        tblPedidosEspeciales.buttons(0, null).container().prependTo(
            tblPedidosEspeciales.table().container()
        );
    }

    initClickDetalle();
}

function MostrarDetalle(idPedidoEspecial) {

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
            console.log("data detalle pedido especial" , data )
            OcultarLoader();
            var html = "";
            var result = JSON.parse(data);
            if (result.Estatus === 200) {
                html += '<div class="table-responsive">';
                html += '<table class="table table-striped" id="tblPedidosEspecialesDet">';
                html += '<thead>';
                html += '<tr>';
                html += '<th>No. detalle</th>';
                html += '<th>Producto</th>';
                html += '<th>Almacen</th>';
                html += '<th>Cantidad</th>';
                html += '<th>Cantidad Atendida</th>';
                html += '<th>Cantidad Rechazada</th>';
                html += '<th>Cantidad Aceptada</th>';
                html += '<th>Monto</th>';
                html += '<th>Precio Venta</th>';
                html += '<th>Estatus</th>';
                html += '</tr>';
                html += '</thead>';
                html += '<tbody>';
                $.each(result.Modelo, function (index, dato) {
                    //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));

                    var estatus = "";
                    switch (dato.idEstatusPedidoEspecialDetalle) {
                        case 1://Solicitados
                            estatus = '<div class="badge badge-light badge-shadow">' + dato.estatusPedidoEspecialDetalle + '</div>';
                            break;
                        case 2://Atendidos
                            estatus = '<div class="badge badge-info badge-shadow">' + dato.estatusPedidoEspecialDetalle + '</div>';
                            break;
                        case 3://Rechazados
                            estatus = '<div class="badge badge-danger badge-shadow">' + dato.estatusPedidoEspecialDetalle + '</div>';
                            break;
                        case 4://Aceptados
                            estatus = '<div class="badge badge-success badge-shadow">' + dato.estatusPedidoEspecialDetalle + '</div>';
                            break;
                        case 5://Atendidos/Incompletos
                            estatus = '<div class="badge badge-warning badge-shadow">' + dato.estatusPedidoEspecialDetalle + '</div>';
                            break;
                        case 6://Rechazados por el Administrador
                            estatus = '<div class="badge badge-danger badge-shadow">' + dato.estatusPedidoEspecialDetalle + '</div>';
                            break;
                        default:
                            estatus = '<div class="badge badge-light badge-shadow">' + dato.estatusPedidoEspecialDetalle + '</div>';
                            break;

                    }

                    html += '<tr>' +
                        '             <td>' + dato.idPedidoEspecialDetalle + '</td>' +
                        '             <td>' + dato.descripcion + '</td>' +
                        '             <td>' + dato.Almacen + '</td>' +
                        '             <td> <div class="badge badge-warning badge-shadow">' + dato.cantidad + '</div></td>' +
                        '             <td> <div class="badge badge-info badge-shadow">' + dato.cantidadAtendida + '</div></td>' +
                        '             <td> <div class="badge badge-danger badge-shadow">' + dato.cantidadRechazada + '</div></td>' +
                        '             <td>  <div class="badge badge-success badge-shadow">' + dato.cantidadAceptada + '</div></td>' +
                        '             <td>' + formatoMoneda(dato.monto) + '</td>' +
                        '             <td>' + formatoMoneda(dato.precioVenta) + '</td>' +
                        '             <td>' + estatus + '</td>' +
                        '</tr>';
                });
                html += '</tbody>';
                html += '</table>';
                html += '</div>';
                $("#detallePedidoEspecial").html(html);
                if (tblPedidosEspecialesDet != null)
                    tblPedidosEspecialesDet.destroy();
                InitDataTablePedidosEspecialesDetalle();
                $('#modalDetallePedidosEspeciales').modal({ backdrop: 'static', keyboard: false, show: true });
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

function MostrarDetalleDevolucion(idPedidoEspecial) {
    $("#idPedidoEspecial").val(0);
    $(".divSubTotal").html("$0.0");
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
                html += '<table class="table table-striped" id="tblDevolucionesPedidosEspeciales" style="width: 100%;">';
                html += '<thead>';
                html += '<tr>';
                html += '<th>Id Producto</th>';
                html += '<th>Producto</th>';
                html += '<th>Almacen</th>';
                html += '<th>Precio</th>';
                html += '<th>Cantidad</th>';
                html += '<th>Total</th>';
                html += '<th>Devolver articulos</th>';
                html += '<th class="text-center" style="display: none;">idPedidoEspecialDetalle</th>';
                html += '<th class="text-center" style="display: none;">montoComisionBancaria</th>';
                html += '</tr>';
                html += '</thead>';
                html += '<tbody>';
                $.each(result.Modelo, function (index, dato) {
                    //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));                  
                    if (dato.cantidad > 0) {


                        html += '<tr>' +
                            '             <td>' + dato.idProducto + '</td>' +
                            '             <td>' + dato.descripcion + '</td>' +
                            '             <td>' + dato.Almacen + '</td>' +
                            '             <td>' + formatoMoneda(dato.precioVenta) + '</td>' +
                            '             <td>' + dato.cantidad + '</td>' +
                            '             <td>' + formatoMoneda(dato.monto) + '</td>';
                        if (dato.fraccion)
                            html += '<td><input class="esDevolucion" type="text" onkeypress="return esDecimal(this, event);" style="text-align: center; border: none; border-color: transparent; background: transparent;" value="0"></td>';
                        else
                            html += '<td><input class="esDevolucion" type="text" onkeypress="return esNumero(event)" style="text-align: center; border: none; border-color: transparent; background: transparent;" value="0"></td>';
                        html +='<td style="display: none;">' + dato.idPedidoEspecialDetalle + '</td>' +
                                '<td style="display: none;">' + dato.montoComisionBancaria + '</td>' +
                                '</tr>';
                    }
                });
                html += '</tbody>';
                html += '</table>';
                html += '</div>';
                $("#devolucionPedidoEspecial").html(html);
                $("#idPedidoEspecial").val(idPedidoEspecial);
                $('#modalDevolucionPedidosEspeciales').modal({ backdrop: 'static', keyboard: false, show: true });                
                if (tblDevolucionesPedidosEspeciales != null)
                    tblDevolucionesPedidosEspeciales.destroy();
                InitDataTablePedidosEspecialesDevolucion();
                
                $('#tblDevolucionesPedidosEspeciales input').on('change', function () {
                    
                    var thisInput = $(this);
                    var cell = $(this).closest('td');
                    var row = cell.closest('tr');
                    var rowIndex = row[0].rowIndex;
                    var tblDevoluciones = document.getElementById('tblDevolucionesPedidosEspeciales');

                    if ((parseFloat(thisInput.val())) > (parseFloat(tblDevoluciones.rows[rowIndex].cells[4].innerHTML))) {
                        MuestraToast('warning', "No puede regresar mas de lo que compro.");
                        document.execCommand('undo');
                        return;
                    }

                    actualizarSubTotalDevoluciones();


                });
                $('#modalDevolucionPedidosEspeciales').modal({ backdrop: 'static', keyboard: false, show: true });
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


function InitDataTablePedidosEspecialesDetalle() {
    var NombreTabla = "tblPedidosEspecialesDet";
    tblPedidosEspecialesDet = initDataTable(NombreTabla);
    if ($("#tblPedidosEspecialesDet").length > 0) {
        new $.fn.dataTable.Buttons(tblPedidosEspecialesDet, {
            buttons: [
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

        tblPedidosEspecialesDet.buttons(0, null).container().prependTo(
            tblPedidosEspecialesDet.table().container()
        );
    }
}

function InitDataTablePedidosEspecialesDevolucion() {
    var NombreTabla = "tblDevolucionesPedidosEspeciales";
    tblDevolucionesPedidosEspeciales = initDataTable(NombreTabla);
    
}

function actualizarSubTotalDevoluciones() {

    var tblDevoluciones = document.getElementById('tblDevolucionesPedidosEspeciales');
    var rCount = tblDevoluciones.rows.length;
    var cantidadTotalDevelta = parseFloat(0);
    var comisionBancariaDevuelta = parseFloat(0);

    if (rCount >= 2) {
        for (var i = 1; i < rCount; i++) {
            const cantidadDevelta = parseFloat(tblDevoluciones.rows[i].cells[6].children[0].value);
            const cantidadInicial = parseFloat(tblDevoluciones.rows[i].cells[4].innerHTML);
            const precioVenta = parseFloat(tblDevoluciones.rows[i].cells[3].innerHTML.replace('$', ''));
            const comisionBancaria = parseFloat(tblDevoluciones.rows[i].cells[8].innerHTML);
            cantidadTotalDevelta += cantidadDevelta * precioVenta; 
            //                                              cantidad devuelta                                   monto comision                              cantidad restante
            comisionBancariaDevuelta += (cantidadDevelta) * ((comisionBancaria) / (cantidadInicial))
            console.log(comisionBancariaDevuelta);
        }
    }

    //document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(cantidadDevelta).toFixed(2) + "</h4>";
    $(".divSubTotal").html("$" + parseFloat(cantidadTotalDevelta + comisionBancariaDevuelta).toFixed(2));
    //document.getElementById("divTotalDevolver").innerHTML = "<h4>$" + parseFloat(cantidadTotalDevelta + comisionBancariaDevuelta).toFixed(2) + "</h4>";
}

function Tickets(idPedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/_ObtenerTicketsPedidoEspecial"),
        data: { idPedidoEspecial: idPedidoEspecial },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $('#ticketsPedidoEspecial').html(data);
            $('#modalVerTicketsPedidoEspecial').modal({ backdrop: 'static', keyboard: false, show: true });

        },
        error: function (xhr, status) {
            console.log('Hubo un error al procesar su solicitud, contactese con el administrador del sistema.');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    }); 
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

function VerTicket(idPedidoEspecial, idTipoTicketPedidoEspecial, idTicketPedidoEspecial) {
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
            console.log(data);


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

function imprimirTicketAlmacenes(idPedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/imprimirTicketAlmacenes"),
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

function FacturarPedidoEspecial(idPedidoEspecial) {
    alert("facturar pedido" + idPedidoEspecial);
}


function modalFacturar(idPedidoEspecial) {

    limpiaModalIVA();
    var data = ConsultaPedido(idPedidoEspecial);

    // ACUTALIZACION PARA FACTURAR TARJETA de CREDITO Y DEBITO
    // EL TEMA DE COMISIONES  NOS PEGA EN ESTA PARTE YA QUE COBRAMOS COMISIONES POR DESLIZAR LA TARJETA
    // Y CUANDO SE FACTURA DESDE EL MODULO DE VENTAS SE CONDONAN COMISIONES ES POR ESTO QUE HAY QUE CANCELAR VENTA
    if (data.Modelo[0].idFactFormaPago == 4 || data.Modelo[0].idFactFormaPago == 18) {
        MuestraToast('warning', "Debe primero cancelar este pedido, ya que se han cobrado comisiones por uso de TC/TD");
        return;
    }

    var montoTotal = parseFloat(data.Modelo[0].montoTotal).toFixed(2);
    var montoIVA = parseFloat(data.Modelo[0].montoTotal * 0.16).toFixed(2);
    //var montoFinal = parseFloat(montoTotal) + parseFloat(montoIVA);
    var idCliente = parseInt(data.Modelo[0].idCliente);

    document.getElementById("previoTotal").innerHTML = "<h4>$" + parseFloat(montoTotal).toFixed(2) + "</h4>";
    document.getElementById("previoSubTotal").innerHTML = "<h4><strike>$" + parseFloat(montoTotal).toFixed(2) + "</strike></h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(montoIVA).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(montoIVA).toFixed(2) + "</h4>";

    $('#idVentaIVA').val(idPedidoEspecial);
    $('#idClienteFact').val(idCliente).trigger('change');
    $('#ModalFacturar').modal({ backdrop: 'static', keyboard: false, show: true });

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
    document.getElementById("previoSubTotal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoIVA").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("previoFinal").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    $('#efectivo').val('');
    $('#idClienteFact').val("0");
    $('#formaPago').val("1").trigger('change');
    $('#usoCFDI').val("3").trigger('change');
    $('#idVentaIVA').val(0);

}


function ConsultaPedido(idPedidoEspecial) {

    var result = '';
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ConsultaDatosTicketPedidoEspecialV2"),
        data: { idPedidoEspecial: idPedidoEspecial },
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



$("#idClienteFact").on("change", function () {

    $("#divUsoCFDI").hide();
    $("#btnGuardarIVA").hide();
    $("#efectivoFactura").hide();
    $('#efectivo').val('');
    document.getElementById("cambio").innerHTML = "<h4>$" + parseFloat(0).toFixed(2) + "</h4>";

    var idCliente = parseFloat($('#idClienteFact').val());
    var data = ObtenerCliente(idCliente);
    var nombre = data.Modelo.nombres + "  " + data.Modelo.apellidoPaterno + "  " + data.Modelo.apellidoMaterno;

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


$('#btnGuardarIVA').click(function (e) {
    
    var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
    var total_ = parseFloat(document.getElementById("previoFinal").innerHTML.replace('<h4>$', '').replace('</h4>', '')).toFixed(2);

    if ($('#idClienteFact').val() == "0") {
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

    if ($('#idClienteFact').val() == "1") {
        MuestraToast('warning', "Debe seleccionar un cliente diferente a " + $("#idClienteFact").find("option:selected").text());
        return;
    }

    //var montoIVA = parseFloat(document.getElementById("previoIVA").innerHTML.replace("<h4>$", "").replace("</h4>", "")).toFixed(2);
    var idPedidoEspecial = $('#idVentaIVA').val();
    var idCliente = $('#idClienteFact').val();
    var formaPago = $('#formaPago').val();
    var usoCFDI = $('#usoCFDI').val();
    
    GuardarIVAPedido(idPedidoEspecial, idCliente, formaPago, usoCFDI);

});


function GuardarIVAPedido(idPedidoEspecial, idCliente, formaPago, usoCFDI) {

    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/GuardarIVAPedido"),
        data: { idPedidoEspecial: idPedidoEspecial, idCliente: idCliente, idFactFormaPago: formaPago, idFactUsoCFDI: usoCFDI },
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
                facturaPedidoEspecial(idPedidoEspecial);
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



function facturaPedidoEspecial(idPedidoEspecial) {
    $.ajax({
        url: pathDominio + "api/WsFactura/GenerarFactura",
        data: { idPedidoEspecial: idPedidoEspecial, idVenta: 0, idUsuario: idUsuarioGlobal },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Facturando Venta.");
        },
        success: function (data) {
            MuestraToast(data.Estatus == 200 ? 'success' : 'error', data.Mensaje);
            OcultarLoader();
            window.location.href = rootUrl("/PedidosEspecialesV2/ConsultarPedidosEspeciales");
        },
        error: function (xhr, status) {
            //$("#btnEntregarPedidoEspecial").removeClass('btn-progress disabled');
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}


$(document).ready(function () {

    //InitDataTableCierres();
    InitSelect2();
    InitRangePicker('rangePedidosEspeciales', 'fechaIni', 'fechaFin');
    //$('#rangePedidosEspeciales').val('');
    $('#fechaIni').val($('#rangePedidosEspeciales').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangePedidosEspeciales').data('daterangepicker').startDate.format('YYYY-MM-DD'));

    $("#btnBuscarPedidosEspeciales").click();


    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarPedidosEspeciales").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarPedidosEspeciales .select-multiple").trigger("change");

    });

    $("#btnBuscarPedidosEspeciales").trigger('click');

    $("#btnRealizarDevolucion").click(function (evt) {
        evt.preventDefault();
        var productos = [];
        var totalProductosDevueltos = parseFloat(0);
        var totalProductosOriginales = parseFloat(0);
        var montoDevuelto = parseFloat($(".divSubTotal").html().replace('$', ''));

        // si todo bien
        var tblDevoluciones = document.getElementById('tblDevolucionesPedidosEspeciales');
        var rCount = tblDevoluciones.rows.length;

        if (rCount >= 2) {            
            for (var i = 1; i < rCount; i++) {
                totalProductosDevueltos += parseFloat(tblDevoluciones.rows[i].cells[6].children[0].value);
                totalProductosOriginales += parseFloat(tblDevoluciones.rows[i].cells[4].innerHTML);
                var row_ = {
                    idProducto: parseInt(tblDevoluciones.rows[i].cells[0].innerHTML),
                    cantidad: parseFloat(tblDevoluciones.rows[i].cells[4].innerHTML),
                    productosDevueltos: parseFloat(tblDevoluciones.rows[i].cells[6].children[0].value),
                    idPedidoEspecialDetalle: parseInt(tblDevoluciones.rows[i].cells[7].innerHTML),
                };
                productos.push(row_);
            }
        }

        if (totalProductosDevueltos <= 0) {
            MuestraToast('warning', "Debe seleccionar al menos un producto para devolver.");
            return;
        }

        if (totalProductosDevueltos >= totalProductosOriginales) {
            MuestraToast('warning', "No se pueden devolver todos los productos");
            return;
        }

        if (($('#motivoDevolucion').val() == "")) {
            MuestraToast('warning', "Debe escribir el motivo de la devolución");
            return;
        }

        dataToPost = JSON.stringify({ productos: productos, idPedidoEspecial: $("#idPedidoEspecial").val(), montoDevuelto: montoDevuelto, motivoDevolucion: $('#motivoDevolucion').val()});
        console.log(dataToPost);

        $.ajax({
            url: rootUrl("/PedidosEspecialesV2/RealizaDevolucionPedidoEspecial"),
            data: dataToPost,
            method: 'post',
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            async: true,
            beforeSend: function (xhr) {
                ShowLoader("Realizando devolucion.");                
            },
            success: function (data) {
                OcultarLoader();
                var result = JSON.parse(data);
                MuestraToast(result.Estatus == 200 ? 'success' : 'error', result.Mensaje);


                if (result.Estatus == 200) {                    
                    ImprimeTicket($("#idPedidoEspecial").val(), 2, 0)
                    $('#modalDevolucionPedidosEspeciales').modal('hide');
                    $("#btnBuscarPedidosEspeciales").trigger('click');              
                    //ConsultExcesoEfectivo();
                }
               

            },
            error: function (xhr, status) {
                OcultarLoader();               
                console.log('Hubo un problema al realizar la devoluciòn, contactese con el administrador del sistema');
                console.log(xhr);
                console.log(status);
            }
        });

    });

});


