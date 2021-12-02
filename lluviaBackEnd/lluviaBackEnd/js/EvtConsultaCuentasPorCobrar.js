var table;
var iframe;
var tblCuentasPorCobrar;
var tblCuentasPorCobrarDet;
var totalAdeudo = parseFloat(0);

//busqueda
function onBeginSubmitCuentasPorCobrar() {
    ShowLoader('Buscando...');
}

function onSuccessCuentasPorCobrar(data) {
    OcultarLoader();
    var html = "";
    var result=JSON.parse(data);
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
            '<table class="table table-striped" id = "tblCuentasPorCobrar">' +
            '    <thead>' +
            '     <tr>' +
            '         <th>No. Cliente</th>' +           
            '         <th>Cliente</th>' +  
            '         <th>Monto total</th>' +      
            '         <th>Monto pagado</th>' +      
            '         <th>Monto adeudo</th>' +         
            '         <th>Acciones</th>' +
            '     </tr>' +
            ' </thead>' +
            ' <tbody>';      
      
        
        $.each(result.Modelo, function (index, dato) {
            //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));           
            html += '<tr>' +
                '             <td>' + dato.idCliente + '</td>' +              
                '             <td>' + dato.nombreCliente + '</td>' +
                '             <td><div class="badge badge-light badge-shadow">' + formatoMoneda(dato.montoTotal) + '</div></td>' +
                '             <td><div class="badge badge-success badge-shadow">' + formatoMoneda(dato.montoPagado) + '</div></td>' +
                '             <td><div class="badge badge-danger badge-shadow">' + formatoMoneda(dato.montoAdeudado) + '</div></td>' +               
                '             <td>' +
                '               <div class="buttons">' +
                '                   <a href="javascript:MostrarDetalle(' + dato.idCliente +');" class="btn btn-icon btn-primary" data-toggle="tooltip" title="Realizar abono"><i class="fas fa-credit-card"></i> Realizar abono</a>' +
                '                   <a href="javascript:GenerarPDF(' + dato.idCliente + "" + ');" class="btn btn-danger btn-icon" data-toggle="tooltip" title="Generar PDF"><i class="fas fa-credit-card"></i> Generar PDF</a>' +
                '               </div>' +
                '             </td>'+
                '</tr>';
        });
        html += ' </tbody>' +
            '</table>' +
            '</div>';
    }
    $('#resultCuentasPorCobrar').html(html);
    if (result.Estatus == 200) {
        if (tblCuentasPorCobrar!=null)
            tblCuentasPorCobrar.destroy();
        InitDataTableCuentasPorCobrar();
    }
}


function completarCeros(valor) {
    return ('00' + valor).slice(-2);
}


function onFailurePedidosEspeciales() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar los pedidos especiales");
}


function InitDataTableCuentasPorCobrar() {
    var NombreTabla = "tblCuentasPorCobrar";
    tblCuentasPorCobrar = initDataTable(NombreTabla);    
    if ($("#tblCuentasPorCobrar").length > 0) {
        new $.fn.dataTable.Buttons(tblCuentasPorCobrar, {
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2]
                    },
                },
            ],

        });

        tblCuentasPorCobrar.buttons(0, null).container().prependTo(
            tblCuentasPorCobrar.table().container()
        );
    }
}

function MostrarDetalle(idCliente) {
    limpiarModalAbono();
    totalAdeudo = parseFloat(0);
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ObtenerCuentasPorCobrarDetalle"),
        data: { idCliente: idCliente},
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
                html = '<div class="table-responsive">' +
                    '<table class="table table-striped" id = "tblCuentasPorCobrarDet">' +
                    '    <thead>' +
                    '     <tr>' +
                    '         <th></th>'+
                    '         <th>No. pedido</th>' +
                    '         <th>Fecha ùlt. abono</th>' +
                    '         <th>Monto inicial</th>' +
                    '         <th>Monto adeudo</th>' + 
                    '         <th>Faturado</th>' +  
                    '     </tr>' +
                    ' </thead>' +
                    ' <tbody>';



                $.each(result.Modelo, function (index, dato) {
                    //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));
                    totalAdeudo = totalAdeudo + parseFloat(dato.saldoActual);
                    html += '<tr>' +
                        '             <td><input type="checkbox" name="chkPedidoEspecial" idPedidoEspecial=' + dato.idPedidoEspecial + ' onclick="soloUno(this,' + dato.saldoActual+')"></td>'+
                        '             <td>' + dato.idPedidoEspecial + '</td>' +
                        '             <td>' + dato.fechaUltimoAbono + '</td>' +
                        '             <td>' + formatoMoneda(dato.SaldoInicial) + '</td>' +
                        '             <td>' + formatoMoneda(dato.saldoActual) + '</td>' +
                        '             <td>NO</td>' +
                        '</tr>';

                    $('#NombreCliente').html(dato.nombreCliente);
                    //$('#TelefonoCliente').html(dato.telefonoCliente);
                    //$('#mailCliente').html(dato.correoCliente);
                    //$('#rfcCliente').html(dato.rfcCliente);
                    //$('#tipoCliente').html(dato.tipoClienteCliente);
                });
                html += ' </tbody>' +
                    '</table>' +
                    '</div>';
                $("#detalleCuentasPorCobrar").html(html);
                $("#tituloModal").html("Realizar abono");
                $("#idCliente").val(idCliente);
                $("#totalAdeudo").html("<h4>$" + totalAdeudo+"</h4>");
                $("#montoAbonar").val("");
                if (tblCuentasPorCobrarDet != null)
                    tblCuentasPorCobrarDet.destroy();
                InitDataTableCuentasPorCobrarDetalle();
                $('#modalDetalleCuentasPorCobrar').modal({ backdrop: 'static', keyboard: false, show: true });
            }
            else
                MuestraToast("error", result.Mensaje);


        },
        error: function (xhr, status) {
            OcultarLoader();
            MuestraToast("error", "Hubo un problema al consutar el detalle de las cuentas por cobrar, contactese con el administrador del sistema");
            console.log(xhr);
            console.log(status);
        }
    });
  
}

function soloUno(checkbox,saldoActual) {
    var checkboxes = document.getElementsByName('chkPedidoEspecial');
    var checked = false;
    checkboxes.forEach((item) => {
        if (item !== checkbox) item.checked = false;
        if (item.checked == true) {
            checked = true;
        }
    });

    if (checked)
        $("#totalAdeudo").html("<h4>$" + saldoActual + "</h4>");
    else
        $("#totalAdeudo").html("<h4>$" + totalAdeudo + "</h4>");
    calculaTotales(false);
}

function InitDataTableCuentasPorCobrarDetalle() {
    var NombreTabla = "tblCuentasPorCobrarDet";
    tblCuentasPorCobrarDet = initDataTable(NombreTabla);
    if ($("#tblCuentasPorCobrarDet").length > 0) {
        //new $.fn.dataTable.Buttons(tblCuentasPorCobrarDet, {
        //    buttons: [
        //        {
        //            extend: 'excel',
        //            text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
        //            className: '',
        //            titleAttr: 'Exportar a Excel',
        //            exportOptions: {
        //                columns: [0, 1, 2, 3, 4, 5, 6]
        //            },
        //        },
        //    ],

        //});

        //tblCuentasPorCobrarDet.buttons(0, null).container().prependTo(
        //    tblCuentasPorCobrarDet.table().container()
        //);
    }
}

function GenerarPDF(idCliente) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/PDFDetalleCuentasPorCobrar"),
        data: { idCliente: idCliente },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Cargando...");
        },
        success: function (data) {
            OcultarLoader();
            downloadPDF(data, "Desglose" + idCliente + ".pdf");
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

function downloadPDF(pdf,nombre) {
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

function limpiarModalAbono() {
    /*var htmlCliente = '<address>'+
        '<strong>Datos del Cliente:</strong><br>' +
        'Nombre: <span id="NombreCliente"></span><br>' +
        'Telefono: <span id="TelefonoCliente"></span><br>' +
        'E-mail: <span id="mailCliente"></span><br>' +
        'RFC:  <span id="rfcCliente"></span><br>' +
        'Tipo de Cliente: <span id="tipoCliente"></span><br>' +
        '</address>';*/
    var htmlCliente = '<strong>Nombre del cliente: </strong><span id="NombreCliente"></span>';
    $("#datosCliente").html(htmlCliente);
    $('#formaPago').val("1").trigger('change');
    //document.getElementById("chkFacturar").checked = false;
    $('#usoCFDI').val("3").trigger('change');    
    $("#totalAdeudo").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");
    $('#montoAbonar').val(0);
    $('#idCliente').val(0);
    $("#previoComisionBancaria").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");  
    //$("#previoIVA").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");
    $("#previoFinal").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");
    $("#cambio").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");
    $('#efectivo').val('');
    
   

}

function calculaTotales(conReseteoCampos) {

    if (conReseteoCampos === true) {
        $('#efectivo').val('');
        $("#cambio").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");     
        //document.getElementById("chkFacturar").checked = false;
       // document.getElementById("divUsoCFDI").style.display = 'none';
        //$('#usoCFDI').val("3").trigger('change');
    }

    var formaPago = $('#formaPago').val();
    var porcentajeComisionBancaria = parseFloat(0);
   
    // si la forma de pago es tarjeta de debito o credito se agrega comision bancaria

    if (
        ((parseInt(formaPago) == parseInt(4)) ||  //Tarjeta de crédito
            (parseInt(formaPago) == parseInt(18))) //&&  //Tarjeta de débito
       // (!$("#chkFacturar").is(":checked"))       // y si la venta no es facturada
    ) {
        porcentajeComisionBancaria = parseFloat($('#comisionBancaria').val()).toFixed(2);
    }

    var abono = parseFloat($("#montoAbonar").val()).toFixed(2);   
   
    var comisionBancaria = (parseFloat((abono) * (porcentajeComisionBancaria / 100))).toFixed(2);    
    var iva = parseFloat(0).toFixed(2);

    // si lleva iva
    /*if ($("#chkFacturar").is(":checked")) {
        iva = parseFloat(abono * 0.16).toFixed(2);
    }*/

    var total = (parseFloat(abono) + parseFloat(comisionBancaria)+ parseFloat(iva)).toFixed(2);  

    $("#previoComisionBancaria").html("<h4>$" + comisionBancaria + "</h4>");
    //$("#previoIVA").html("<h4>$" + iva + "</h4>");
    $("#previoFinal").html("<h4>$" + total + "</h4>");

}


$(document).ready(function () {

    //InitDataTableCierres();
    InitSelect2();
    //InitRangePicker('rangePedidosEspeciales', 'fechaIni', 'fechaFin');
    //$('#rangePedidosEspeciales').val('');
    //$('#fechaIni').val($('#rangePedidosEspeciales').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangePedidosEspeciales').data('daterangepicker').startDate.format('YYYY-MM-DD'));

    $("#btnBuscarCuentasPorCobrar").click();


    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarCuentasPorCobrar").trigger("reset");
        $("#frmBuscarCuentasPorCobrar .select-multiple").trigger("change");

    });

    $('#montoAbonar').on("keyup", function (event) {
        calculaTotales(false);
    })

    $("#formaPago").on("change", function (value) {
        this.value == 1 ? $('#dvEfectivo').css('display', '') : $('#dvEfectivo').css('display', 'none');
        calculaTotales(true);
    });


    $('#chkFacturar').click(function () {

        var idCliente = $('#idCliente').val();      
      
        if (idCliente == 1) {
            MuestraToast('warning', "Debe seleccionar un cliente diferente a " + $("#idCliente").find("option:selected").text());
            document.getElementById("chkFacturar").checked = false;
            return
        }

        var cliente = new Object();
        cliente.idCliente = idCliente;
        cliente.Nombre = $('#idCliente').html();
        cliente.Telefono = $('#TelefonoCliente').html();
        cliente.correo = $('#mailCliente').html();
        cliente.rfc = $('#rfcCliente').html();
        cliente.tipoCliente = $('#tipoCliente').html();

        if ($('#chkFacturar').is(':checked')) {
        
            if (cliente) {
                if (!validarEmail(cliente.correo)) {
                    MuestraToast('warning', "No es posible facturar a un cliente sin correo electrónico vàlido");
                    document.getElementById("chkFacturar").checked = false;
                    return false;
                }

                if (!validarRFC(cliente.rfc)) {
                    MuestraToast('warning', "No es posible facturar a un cliente sin RFC vàlido");
                    document.getElementById("chkFacturar").checked = false;
                    return false;
                }
            } else {
                MuestraToast('warning', "No es posible facturar a  este cliente por favor comuníquese con el administrador web");
                document.getElementById("chkFacturar").checked = false;
                return false;
            }
        }


        $('#efectivo').val('');
        $("#cambio").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");    


        if ($(this).is(':checked')) {
            document.getElementById("divUsoCFDI").style.display = 'block';           
        } else {
            document.getElementById("divUsoCFDI").style.display = 'none';
        }
       
        //calculaTotales(false);

    });

    $("#efectivo").on("keyup", function (event) {

        if (event.keyCode === 13) {
            event.preventDefault();
            $("#btnAbonar").click();
        }
        else {

            var cambio_ = parseFloat(0).toFixed(2);
            var efectivo_ = parseFloat($('#efectivo').val()).toFixed(2);
            var total_ = parseFloat($("#previoFinal").html().replace('<h4>$', '').replace('</h4>', ''));

            if (parseFloat(efectivo_) > parseFloat(total_)) {
                cambio_ = efectivo_ - total_;
                $("#cambio").html("<h4>$" + parseFloat(cambio_).toFixed(2) + "</h4>");
                           
            }
            else {
                $("#cambio").html("<h4>$" + parseFloat(0).toFixed(2) + "</h4>");              
            }
        }

    });


    $("#btnAbonar").click(function (evt) {
        evt.preventDefault();
        
        if ($('#montoAbonar').val() == "") {
            MuestraToast('warning', "Debe escribir el monto a abonar.");            
            return;
        }

       
        var monto = parseFloat($('#montoAbonar').val());
        var idCliente = parseInt($('#idCliente').val());
        var totalAdeudo = parseFloat($("#totalAdeudo").html().replace('<h4>$', '').replace('</h4>', ''));
        var efectivo = parseFloat($('#efectivo').val()).toFixed(2);
        var total = parseFloat($("#previoFinal").html().replace('<h4>$', '').replace('</h4>', ''));
        var IVA = 0;//parseFloat($("#previoIVA").html().replace('<h4>$', '').replace('</h4>', ''));
        var comision = parseFloat($("#previoComisionBancaria").html().replace('<h4>$', '').replace('</h4>', ''));
        var formaPago = parseInt($('#formaPago').val());
        var usoCFDI = 0;
        var idPedidoEspecial = parseInt(0);

        if (monto<=0) {
            MuestraToast('warning', "El monto a abonar debe de ser mayor que 0.");
            return;
        }

        if (monto > totalAdeudo) {
            MuestraToast('warning', "El monto a abonar no debe de ser mayor que el total adeudo.");
            return;
        } 

        //pago con efectivo
        if (parseInt(formaPago) == parseInt(1)) {

            if ($('#efectivo').val() == "") {
                MuestraToast('warning', "Debe escribir con cuanto efectivo le estan pagando.");
                return;
            }

            if (parseFloat(efectivo) < parseFloat(total)) {
                MuestraToast('warning', "El efectivo no alcanza a cubrir el total: " + total.toString());               
                return;
            }
        }

        /*if ($("#chkFacturar").is(":checked")) {
            usoCFDI = parseInt($('#usoCFDI').val());
        }*/

        $("input[type=checkbox]:checked").each(function () {
            idPedidoEspecial = parseInt($(this).attr('idPedidoEspecial'));
           
        });

        

        var abono = new Object();
        abono.idCliente = idCliente;
        abono.montoAbono = monto;
        abono.montoIVA = IVA;
        abono.montoComision = comision;
        abono.requiereFactura = false;//document.getElementById("chkFacturar").checked;
        abono.idFactFormaPago = formaPago;
        abono.idFactUsoCFDI = usoCFDI;
        abono.idPedidoEspecial = idPedidoEspecial;
       
        console.log(JSON.stringify(abono));
       
                 

        $.ajax({
            url: rootUrl("/PedidosEspecialesV2/RealizarAbonoPedidosEspeciales"),
            data: JSON.stringify({ abono: abono}),
            method: 'post',
            dataType: 'json',
            async: true,
            contentType: "application/json; charset=utf-8",
            beforeSend: function (xhr) {
                ShowLoader("Abonando...");
            },
            success: function (data) {
                OcultarLoader();               
                var result = JSON.parse(data);
                if (result.Estatus === 200) {
                    MuestraToast("success", "Se ha realizado de manera correcta el abono");                   
                    $('#modalDetalleCuentasPorCobrar').modal('hide');
                    $("#btnBuscarCuentasPorCobrar").click();
                }
                else
                    MuestraToast("error", result.Mensaje);
            },
            error: function (xhr, status) {
                OcultarLoader();
                MuestraToast("error", "Hubo un problema al realizar el abono, contactese con el administrador del sistema");
                console.log(xhr);
                console.log(status);
            }
        });


    });

});