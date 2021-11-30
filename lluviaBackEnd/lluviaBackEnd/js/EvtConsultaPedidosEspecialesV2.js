var table;
var iframe;
var tblPedidosEspeciales;
var tblPedidosEspecialesDet;

//busqueda
function onBeginSubmitPedidosEspeciales() {
    ShowLoader('Buscando...');
}

function onSuccessPedidosEspeciales(data) {
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
            '<table class="table table-striped" id = "tblPedidosEspeciales">' +
            '    <thead>' +
            '     <tr>' +
            '         <th>No. pedido especial</th>' +
            '         <th>Fecha</th>' +
            '         <th>Cliente</th>' +
            '         <th>Usuario</th>' +
            '         <th>Cantidad</th>' +
            '         <th>Monto Total</th>' +
            '         <th>Estatus</th>' +
            '         <th>Facturado</th>' +
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
           
            html += '<tr>' +
                '             <td>' + dato.idPedidoEspecial + '</td>' +
                '             <td>' + dato.fechaAlta + '</td>' +
                '             <td>' + dato.nombreCliente + '</td>' +
                '             <td>' + dato.nombreUsuario + '</td>' +
                '             <td>' + dato.cantidad + '</td>' +
                '             <td>' + formatoMoneda(dato.montoTotal) + '</td>' +
                '             <td>' + estatus + '</td>' +
                '             <td>' + dato.facturado + '</td>' +
                '             <td>' + dato.codigoBarras + '</td>' +
                '             <td>' + dato.observaciones + '</td>' +
                '             <td><a href="javascript:MostrarDetalle(' + dato.idPedidoEspecial+');" class="btn btn-icon btn-primary" data-toggle="tooltip" title="Detalle"><i class="fas fa-align-justify"></i></a></td>' +
                '</tr>';
        });
        html += ' </tbody>' +
            '</table>' +
            '</div>';
    }
    $('#resultPedidosEspeciales').html(html);
    if (result.Estatus == 200) {
        if (tblPedidosEspeciales!=null)
            tblPedidosEspeciales.destroy();
        InitDataTablePedidosEspeciales();
    }
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
                        columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
                    },
                },
            ],

        });

        tblPedidosEspeciales.buttons(0, null).container().prependTo(
            tblPedidosEspeciales.table().container()
        );
    }
}

function MostrarDetalle(idPedidoEspecial) {
    $.ajax({
        url: rootUrl("/PedidosEspecialesV2/ObtenerPedidosEspecialesDetalle"),
        data: { idPedidoEspecial: idPedidoEspecial},
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
                    '<table class="table table-striped" id = "tblPedidosEspecialesDet">' +
                    '    <thead>' +
                    '     <tr>' +
                    '         <th>No. detalle</th>' +
                    '         <th>Producto</th>' +
                    '         <th>Almacen</th>' +
                    '         <th>Cantidad</th>' +
                    '         <th>Monto</th>' +
                    '         <th>Precio Venta</th>' +
                    '         <th>Estatus</th>' +                    
                    '     </tr>' +
                    ' </thead>' +
                    ' <tbody>';



                $.each(result.Modelo, function (index, dato) {
                    //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));

                    var estatus = "";
                    switch (dato.idEstatusPedidoEspecialDetalle) {
                        case 1://Solcitados
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
                        '             <td>' + dato.cantidad + '</td>' +
                        '             <td>' + formatoMoneda(dato.monto) + '</td>' +
                        '             <td>' + formatoMoneda(dato.precioVenta) + '</td>' +
                        '             <td>' + estatus + '</td>' +
                        '</tr>';
                });
                html += ' </tbody>' +
                    '</table>' +
                    '</div>';
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


    

});


