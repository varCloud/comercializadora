var tblCompras
$(document).ready(function () {
    if ($("#tblCompras").length > 0) {       
        InitTableCompras();
    }
   
    InitRangePicker('rangeCompras', 'fechaIni', 'fechaFin');
    //$('#fechaIni').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeFacturas').data('daterangepicker').startDate.format('YYYY-MM-DD'));

    $('#rangeCompras').val('');
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
        $("#frmBuscarCompras").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarCompras .select-multiple").trigger("change");

    });

});

function InitTableCompras() {
    var NombreTabla = "tblCompras";
    tblCompras = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblCompras, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Compras",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    //doc.content[1].table.widths = ['10%', '30%', '20%', '10%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Compras");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7,8,9]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7,8,9]
                },
            },
        ],
    });

    tblCompras.buttons(0, null).container().prependTo(
        tblCompras.table().container()
    );


    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a onclick="NuevaCompra(0)" class="btn btn-icon btn-success" name="" id="btnNuevaCompra" data-toggle="tooltip" title="Nueva compra"><i class="fas fa-plus"></i></a>');


}

function onBeginSubmitObtenerCompras() {
    ShowLoader("Buscando...");
}
function onCompleteObtenerCompras() {
    //OcultarLoader();
}
function onSuccessResultObtenerCompras(data) {    
    $("#DivtblCompras").html(data);
    if ($("#tblCompras").length > 0)
    {
        tblCompras.destroy();
        InitTableCompras();
    }
    
    OcultarLoader();
}
function onFailureResultObtenerCompras() {
    OcultarLoader();
}

function EliminarCompra(idCompra) {

    swal({
        title: '',
        text: 'Estas seguro que deseas eliminar a esta Compra?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/Compras/EliminaCompra"),
                    data: { idCompra: idCompra },
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader("Eliminando Compra.");
                    },
                    success: function (data) {
                        OcultarLoader();
                        if (data.Estatus == 200) {
                            MuestraToast("success", data.Mensaje);
                            $("#btnBuscarCompras").click();
                        }
                        else
                            MuestraToast("error", data.Mensaje);

                    },
                    error: function (xhr, status) {
                        OcultarLoader();
                        console.log('Hubo un problema al intentar eliminar la compra, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });

            } else {
                console.log("cancelar");
            }
        });


}

function VerDetalleCompra(idCompra) {

    $.ajax({
        url: rootUrl("/Compras/_DetalleCompra"),
        data: { idCompra: idCompra, enableEdit: false },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            $("#detalleCompra").html(data);
            actualizaTicket();
            $('#modalDetalleCompra').modal({ backdrop: 'static', keyboard: false, show: true });

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar mostrar el detalle de la compra, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function actualizaTicket() {

    var total = parseFloat(0);

    $('#tblComprasDetalle tbody tr').each(function (index, fila) {
        fila.children[0].innerHTML = index + 1;
        //fila.children[6].innerHTML = "      <a href=\"javascript:eliminaFila(" + parseFloat(index + 1) + ")\"  data-toggle=\"tooltip\" title=\"\" data-original-title=\"Eliminar\"><i class=\"far fa-trash-alt\"></i></a>";
        total += parseFloat(fila.children[5].innerHTML.replace('$', ''));
    });

    //actualizar los totales
    //document.getElementById("divSubTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
    //document.getElementById("divIva").innerHTML = "<h4>$" + parseFloat(total * 0.16).toFixed(2) + "</h4>";
    //document.getElementById("divTotal").innerHTML = "<h4>$" + parseFloat(total * 1.16).toFixed(2) + "</h4>";  
    document.getElementById("divTotal").innerHTML = "<h4>$" + parseFloat(total).toFixed(2) + "</h4>";
}

function NuevaCompra(idCompra) {

    $.ajax({
        url: rootUrl("/Compras/_Compra"),
        data: { idCompra: idCompra},
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {
            OcultarLoader();
            
            if (idCompra>0)
                $("#titleModalCompra").html("Editar Compra");
            else
                $("#titleModalCompra").html("Nueva Compra");
            $("#NuevaCompra").html(data);
            actualizaTicket();
            $('#modalNuevaCompra').modal({ backdrop: 'static', keyboard: false, show: true });

        },
        error: function (xhr, status) {
            OcultarLoader();
            console.log('Hubo un problema al intentar mostrar el detalle de la compra, contactese con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}

