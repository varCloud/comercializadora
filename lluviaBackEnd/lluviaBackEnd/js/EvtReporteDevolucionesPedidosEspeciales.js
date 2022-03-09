var table;
var iframe;
var tablaDevolucionesPedidosEspeciales;

//busqueda
function onBeginSubmitDevolucionesPedidosEspeciales() {
    ShowLoader('Buscando...');
}

function onSuccessResultDevolucionesPedidosEspeciales(data) {
    OcultarLoader();
    $('#rowDevolucionesPedidosEspeciales').html(data);
    tablaDevolucionesPedidosEspeciales.destroy();    
    InitDataTableDevoluciones();
}
function onFailureResultDevolucionesPedidosEspeciales() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar las Devoluciones de Pedidos Especiales");
}


function InitDataTableDevoluciones() {
    var NombreTabla = "tablaRepDevolucionesPedidosEspeciales";
    tablaDevolucionesPedidosEspeciales = initDataTable(NombreTabla);

    if ($("#tablaRepDevolucionesPedidosEspeciales").length > 0) {
        new $.fn.dataTable.Buttons(tablaDevolucionesPedidosEspeciales, {
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
                    },
                },
            ],

        });

        tablaDevolucionesPedidosEspeciales.buttons(0, null).container().prependTo(
            tablaDevolucionesPedidosEspeciales.table().container()
        );
    }
}


$(document).ready(function () {

    InitDataTableDevoluciones();
    InitSelect2();
    InitRangePicker('rangeDevolucionesPedidosEspeciales', 'fechaIni', 'fechaFin');
    //$('#rangeDevolucionesPedidosEspeciales').val('');

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarDevolucionesPedidosEspeciales").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarDevolucionesPedidosEspeciales .select-multiple").trigger("change");
    });

});