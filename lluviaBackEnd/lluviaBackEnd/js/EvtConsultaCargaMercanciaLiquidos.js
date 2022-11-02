var table;
var iframe;
var tblCargaMercanciaLiquidos;

//busqueda
function onBeginSubmitCargaMercanciaLiquidos() {
    ShowLoader('Buscando...');
}

function onSuccessCargaMercanciaLiquidos(data) {
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
            '<table class="table table-striped" id = "tblCargaMercanciaLiquidos">' +
            '    <thead>' +
            '     <tr>' +
            '         <th></th>' +
            '         <th>Ubicacion</th>' +
            '         <th>Producto</th>' +
            '         <th>Cantidad</th>' +
            '         <th>Usuario</th>' +
            '         <th>Fecha</th>' +
            '         <th>Rol</th>' +
            '         <th>Costo de Compra</th>' +
            '         <th>Movimiento</th>' +
            '     </tr>' +
            ' </thead>' +
            ' <tbody>';

        $.each(result.Modelo, function (index, dato) {

            html += '<tr>' +
                '             <td>' + dato.id + '</td>' +
                '             <td>' + dato.descripcionUbicacion + '</td>' +
                '             <td>' + dato.descripcionProducto + '</td>' +
                '             <td>' + dato.cantidad + '</td>' +
                '             <td>' + dato.nombreUsuario + '</td>' +
                '             <td>' + dato.fechaAlta + '</td>' +
                '             <td>' + dato.descripcionRol + '</td>' +
                '             <td>' + dato.ultimoCostoCompra + '</td>' +
                '             <td>' + dato.descTipoMovInventario + '</td>' +
            '       </tr>';
        });

        html += ' </tbody>' +
            '</table>' +
            '</div>';
    }
    $('#resultCargaMercanciaLiquidos').html(html);
    InitDataTableCargaMercanciaLiquidos();
}

function onFailureCargaMercanciaLiquidos() {
    OcultarLoader();
    MuestraToast("error", "Ocurrio un error al consultar la carga de mercancia.");
}

function InitDataTableCargaMercanciaLiquidos() {

    var NombreTabla = "tblCargaMercanciaLiquidos";
    tblCargaMercanciaLiquidos = initDataTable(NombreTabla);
    if ($("#tblCargaMercanciaLiquidos").length > 0) {
        new $.fn.dataTable.Buttons(tblCargaMercanciaLiquidos, {
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5, 6,7]
                    },
                },
            ],

        });

        tblCargaMercanciaLiquidos.buttons(0, null).container().prependTo(
            tblCargaMercanciaLiquidos.table().container()
        );
    }

    initClickDetalle();
}

$("#idRol").on("change", function () {
    InitSelect2Usuarios();
});

function InitSelect2Usuarios() {

    var idRol = $('#idRol').val();    
    MuestraToast('info', "Actualizando usuarios...");
    $("#idUsuario").val('');
    var result = '';

    $.ajax({
        url: rootUrl("/Productos/ConsultarUsuariosLiquidos"),
        data: { idRol: idRol },
        method: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function (xhr) {
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

    arrayUsuarios = [];
    arrayUsuarios = result.Modelo;

    var i;
    for (i = 0; i < result.Modelo.length; i++) {
        result.Modelo[i].id = result.Modelo[i]['idUsuario'];
        result.Modelo[i].text = result.Modelo[i]['nombreCompleto'];
    }

    //Producto Tipo Envase
    $("#idUsuario").html('').select2();
    $('#idUsuario').select2({
        width: "100%",
        //placeholder: "--SELECCIONA--",
        data: result.Modelo,
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




$(document).ready(function () {
    InitSelect2();
    InitRangePicker('rangeCargaMercanciaLiquidos', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeCargaMercanciaLiquidos').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeCargaMercanciaLiquidos').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarCargaMercanciaLiquidos").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarCargaMercanciaLiquidos .select-multiple").trigger("change");
    });
    $("#btnBuscarCargaMercanciaLiquidos").trigger('click');  
});


