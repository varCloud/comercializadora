var table;
var iframe;
var tablaConsultaVentas; 

//busqueda
function onBeginSubmitConsultaVentas() {
    console.log("onBeginSubmitConsultaVentas");
}
function onCompleteSubmitConsultaVentas() {
    console.log("onCompleteSubmitConsultaVentas");
}
function onSuccessResultConsultaVentas(data) {
    console.log("onSuccessResultVentas", JSON.stringify(data) );    
    tablaConsultaVentas.destroy();
    $('#rowConsultaVentas').html(data);
    InitDataTableConsultaVentas();
}
function onFailureResultConsultaVentas() {
    console.log("onFailureResult___");
}

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
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            console.log(data);
            MuestraToast('success', "Se envio el ticket a la impresora.");
        },
        error: function (xhr, status) {
            MuestraToast('error', "Ocurrio un error al enviar el ticket a la impresora.");
            console.log(xhr);
            console.log(status);
            console.log(data);
        }
    });
}

function PintarTabla() {
    $.ajax({
        url: "/Ventas/_ObtenerVentas",
        data: { idVenta: 0 },
        method: 'post',
        dataType: 'html',
        async: false,
        beforeSend: function (xhr) {
        },
        success: function (data) {
            tablaConsultaVentas.destroy();
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

function InitDataTableConsultaVentas() {
    var NombreTabla = "tablaConsultaVentas";
    tablaConsultaVentas = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaConsultaVentas, {
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
                    doc.content[1].table.widths = ['10%', '20%', '20%', '10%', '20%', '20%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Ventas");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
                },
            },
        ],

    });

    tablaConsultaVentas.buttons(0, null).container().prependTo(
        tablaConsultaVentas.table().container()
    );
}



function CancelaVenta(idVenta) {

    swal({
        title: 'Mensaje',
        text: 'Estas seguro que deseas eliminar a esta Venta?',
        icon: 'warning',
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

function GenerarFactura(idVenta) {
    console.log("idventa", idVenta)
    //ShowLoader();
    
    $.ajax({
        url: rootUrl("/Factura/GenerarFactura"),
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




$(document).ready(function () {

    InitSelect2Multiple();
    InitDataTableConsultaVentas();
    InitRangePicker('rangeConsultaVentas', 'fechaIni', 'fechaFin');
    $('#fechaIni').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    $('#fechaFin').val($('#rangeConsultaVentas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

});