var table;
var iframe;
var tblCuentasPorCobrar;
var tblCuentasPorCobrarDet;

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
            let nombre = "jessica almonte";
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
    var totalAdeudo = parseFloat(0);
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
                    '         <th>No. pedido</th>' +
                    '         <th>Fecha ùlt. abono</th>' +
                    '         <th>Monto inicial</th>' +
                    '         <th>Monto adeudo</th>' +                                   
                    '     </tr>' +
                    ' </thead>' +
                    ' <tbody>';



                $.each(result.Modelo, function (index, dato) {
                    //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));
                    totalAdeudo = totalAdeudo + parseFloat(dato.saldoActual);
                    html += '<tr>' +
                        '             <td>' + dato.idPedidoEspecial + '</td>' +
                        '             <td>' + dato.fechaUltimoAbono + '</td>' +
                        '             <td>' + formatoMoneda(dato.SaldoInicial) + '</td>' +
                        '             <td>' + formatoMoneda(dato.saldoActual) + '</td>' +
                        '</tr>';
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

    $("#btnAbonar").click(function (evt) {
        evt.preventDefault();
        
        if ($('#montoAbonar').val() == "") {
            MuestraToast('warning', "Debe escribir el monto a abonar.");            
            return;
        }

        var monto = parseFloat($('#montoAbonar').val());
        var idCliente = parseInt($('#idCliente').val());
        var totalAdeudo = parseFloat($("#totalAdeudo").html().replace('<h4>$', '').replace('</h4>', ''));

        if (monto<=0) {
            MuestraToast('warning', "El monto a abonar debe de ser mayor que 0.");
            return;
        }

        if (monto > totalAdeudo) {
            MuestraToast('warning', "El monto a abonar no debe de ser mayor que el total adeudo.");
            return;
        }       

        $.ajax({
            url: rootUrl("/PedidosEspecialesV2/RealizarAbonoPedidosEspeciales"),
            data: { idCliente: idCliente, montoAdeudo: monto},
            method: 'post',
            dataType: 'json',
            async: true,
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