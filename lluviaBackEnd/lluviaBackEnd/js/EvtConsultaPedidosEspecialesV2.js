var table;
var iframe;
var tblPedidosEspeciales;

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
            '         <th>Codigo de barras</th>' +
            '         <th>Observaciones</th>' +             
            '     </tr>' +
            ' </thead>' +
            ' <tbody>';
        
      
        
        $.each(result.Modelo, function (index, dato) {
            //var fecha = new Date(parseInt(dato.fechaAlta.substr(6)));
            console.log(dato[0]);
            html += '<tr>' +
                '             <td>' + dato.idPedidoEspecial + '</td>' +
                '             <td>' + dato.fechaAlta + '</td>' +
                '             <td>' + dato.nombreCliente + '</td>' +
                '             <td>' + dato.nombreUsuario + '</td>' +
                '             <td>' + dato.cantidad + '</td>' +
                '             <td>' + dato.montoTotal + '</td>' +
                '             <td>' + dato.estatusPedidoEspecial + '</td>' +
                '             <td>' + dato.codigoBarras + '</td>' +
                '             <td>' + dato.observaciones + '</td>' +
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


$(document).ready(function () {

    //InitDataTableCierres();
    InitSelect2();
    InitRangePicker('rangePedidosEspeciales', 'fechaIni', 'fechaFin');
    $('#rangePedidosEspeciales').val('');

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarPedidosEspeciales").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarPedidosEspeciales .select-multiple").trigger("change");

    });


    

});


