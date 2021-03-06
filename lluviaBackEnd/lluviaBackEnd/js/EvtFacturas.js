﻿var tblFacturas
$(document).ready(function () {
    if ($("#tblFacturas").length > 0) {
        InitTableFacturas();
    }
    
    InitRangePicker('rangeFacturas', 'fechaIni', 'fechaFin');
    //$('#fechaIni').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

    $('#rangeFacturas').val('');
    $('.select-multiple').select2({
        width: "100%",
        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        },

    });

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarFacturas").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarFacturas .select-multiple").trigger("change");

    });

});

function InitTableFacturas() {
    var NombreTabla = "tblFacturas";
    tblFacturas = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblFacturas, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Facturas",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['10%', '30%', '30%', '10%', '20%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Facturas");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                },
            },
        ],
    });

    tblFacturas.buttons(0, null).container().prependTo(
        tblFacturas.table().container()
    );

}

function onBeginSubmitObtenerFacturas() {
    ShowLoader("Buscando...");
}
function onCompleteObtenerFacturas() {
    //OcultarLoader();
}
function onSuccessResultObtenerFacturas(data) {
    $("#DivtblFacturas").html(data);
    if ($("#tblFacturas").length > 0) {
        tblFacturas.destroy();
        InitTableFacturas();
    }

    OcultarLoader();
}
function onFailureResultObtenerFacturas() {
    OcultarLoader();
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



