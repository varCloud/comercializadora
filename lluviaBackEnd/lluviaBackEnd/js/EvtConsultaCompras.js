﻿var tblCompras
$(document).ready(function () {
    InitTableCompras();
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


    $("#fechaIni").daterangepicker({
        locale: { format: "YYYY-MM-DD", cancelLabel: 'Clear' },
        singleDatePicker: true,
        autoUpdateInput: false        
    }, (from_date, to_date) => {
            $("#fechaIni").val(from_date.format('YYYY-MM-DD'));
    });
    

    $("#fechaFin").daterangepicker({
        locale: { format: "YYYY-MM-DD", cancelLabel: 'Clear' },
        singleDatePicker: true,
        autoUpdateInput: false
    }, (from_date, to_date) => {       
            $("#fechaFin").val(from_date.format('YYYY-MM-DD'));
    });

    $("#btnLimpiarForm").click(function (evt) {
        $("#frmBuscarCompras").trigger("reset");
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
                    doc.content[1].table.widths = ['10%', '30%', '20%', '10%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Compras");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            },
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

    tblCompras.buttons(0, null).container().prependTo(
        tblCompras.table().container()
    );


    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnNuevaCompra" data-toggle="tooltip" title="Nueva compra"><i class="fas fa-plus"></i></a>');

    $('#btnNuevaCompra').click(function (e) {

    });
}

function onBeginSubmitObtenerCompras() {
    ShowLoader("Buscando...");
}
function onCompleteObtenerCompras() {
    //OcultarLoader();
}
function onSuccessResultObtenerCompras(data) {
    console.log("onSuccessResultObtenerCompras", JSON.stringify(data));
    tblCompras.destroy();
    $("#DivtblCompras").html(data);
    InitTableCompras();
    OcultarLoader();
}
function onFailureResultObtenerCompras() {
    OcultarLoader();
}

function EliminarCompra(idCompra) {

    swal({
        title: '',
        text: 'Estas seguro que deseas eliminar a esta Compra?',
        icon: 'warning',
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