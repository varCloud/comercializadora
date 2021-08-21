var table;
var iframe;
var tablaCierres;

//busqueda
function onBeginSubmitCierres() {
    ShowLoader('Buscando...');
}

function onSuccessResultCierres(data) {
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
            '<table class="table table-striped" id = "tablaRepCierres">' +
            '    <thead>' +
            '     <tr>' +
            '         <th>Fecha</th>' +
            '         <th>Almacen</th>' +
            '         <th>Cajero</th>' +
            '         <th>Apertura de Caja</th>' +
            '         <th>Ingresos de Efectivo</th>' +
            '         <th>No. Ventas</th>' +
            '         <th>Monto Total Ventas Contado</th>' +
            '         <th>Monto Total Ventas Tarjeta</th>' +
            '         <th>Monto Total Ventas Transferencias</th>' +
            '         <th>Monto Total Ventas Otros</th>' +
            '         <th>Monto Total Ventas Canceladas</th>' +
            '         <th>No. Productos Devueltos</th>' +
            '         <th>Monto Total Devoluciones</th>' +
            '         <th>Monto Total Retiros</th>' +
            '         <th>Saldo Total</th>' +
            '         <th>Saldo en Caja</th>' +
            '         <th>Efectivo Entregado En Cierre</th>' +
            '     </tr>' +
            ' </thead>' +
            ' <tbody>';
        for (i = 0; i < data.length; i++) {
            var fecha = new Date(parseInt(data[i].fechaCierre.substr(6)));
            
            html += '<tr>' +
                '             <td>' + completarCeros(fecha.getDate()) + "/" + completarCeros(parseInt(fecha.getMonth()) + 1) + "/" + fecha.getFullYear() + '</td>' +
                '             <td>' + data[i].descAlmacen + '</td>' +
                '             <td>' + data[i].nombreUsuario + '</td>' +
                '             <td>' + data[i].montoApertura + '</td>' +
                '             <td>' + data[i].montoIngresosEfectivo + '</td>' +
                '             <td>' + data[i].totalVentas + '</td>' +
                '             <td>' + data[i].montoVentasContado + '</td>' +
                '             <td>' + data[i].montoVentasTarjeta + '</td>' +
                '             <td>' + data[i].montoVentasTransferencias + '</td>' +
                '             <td>' + data[i].montoVentasOtros + '</td>' +
                '             <td>' + data[i].montoVentasCanceladas + '</td>' +
                '             <td>' + data[i].ProductosDevueltos + '</td>' +
                '             <td>' + data[i].MontoTotalDevoluciones + '</td>' +
                '             <td>' + data[i].retirosExcesoEfectivo + '</td>' +
                '             <td>' + data[i].montoCierre + '</td>' +
                '             <td>' + data[i].efectivoDisponible + '</td>' +
                '             <td>' + data[i].EfectivoEntregadoEnCierre + '</td>' +
                '</tr>';
        }
        html += ' </tbody>' +
            '</table>' +
            '</div>';
    }
    $('#resultCierres').html(html);
    if (data.length > 0) {
        tablaCierres.destroy();
        InitDataTableCierres();
    }
}

function completarCeros(valor) {
    return ('00' + valor).slice(-2);
}


function onFailureResultCierres() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar los cierres");
}


function InitDataTableCierres() {
    var NombreTabla = "tablaRepCierres";
    tablaCierres = initDataTable(NombreTabla);

    if ($("#tablaRepCierres").length > 0) {
        new $.fn.dataTable.Buttons(tablaCierres, {
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7,8,9,10,11,12,13]
                    },
                },
            ],

        });

        tablaCierres.buttons(0, null).container().prependTo(
            tablaCierres.table().container()
        );
    }
}


$(document).ready(function () {

    InitDataTableCierres();
    InitSelect2();
    InitRangePicker('rangeCierres', 'fechaIni', 'fechaFin');
    $('#rangeCierres').val('');

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarCierres").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarCierres .select-multiple").trigger("change");

    });

});