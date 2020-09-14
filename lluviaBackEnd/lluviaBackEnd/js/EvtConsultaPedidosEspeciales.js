var tablaBitacoras;
$(document).ready(function () {
    if ($("#tblBitacoras ").length > 0) {
        InitTableBitacoras();
    }
    InitRangePicker('rangeBitacoras', 'fechaIni', 'fechaFin');

    $('#rangeBitacoras').val('');
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
        $("#frmBuscarBitacoras").trigger("reset");
        $('#fechaIni').val('');
        $('#fechaFin').val('');
        $("#frmBuscarBitacoras .select-multiple").trigger("change");

    });  

});

function InitTableBitacoras() {
    var NombreTabla = "tblBitacoras";
    tablaBitacoras = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tablaBitacoras, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Bitàcoras",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['5%', '15%', '15%', '15%', '20%', '10%', '10%', '10%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Bitàcoras");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6,7,8]
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6,7,8]
                },
            },
        ],
    });

    tablaBitacoras.buttons(0, null).container().prependTo(
        tablaBitacoras.table().container()
    );

    $('#tblBitacoras tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = tablaBitacoras.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        } else {
            // Open this row
            //  console.log(obtenerDetalleBitacora($(this).attr("idPedido")));
            $.ajax({
                url: rootUrl("/Bitacora/_DetalleBitacora"),
                data: { idPedidoInterno: $(this).attr("idPedido") },
                method: 'post',
                dataType: 'html',
                async: true,
                beforeSend: function (xhr) {
                    ShowLoader();
                },
                success: function (view) {
                    OcultarLoader();
                    row.child(view).show();
                    tr.addClass('shown');
                },
                error: function (xhr, status) {
                    OcultarLoader();
                }
            });

        }
    });

    }

function onBeginSubmitObtenerBitacoras() {
    ShowLoader("Buscando...");
}
function onCompleteObtenerBitacoras() {
    //OcultarLoader();
}
function onSuccessResultObtenerBitacoras(data) {
    $("#DivtblBitacoras").html(data);
    if ($("#tblBitacoras ").length > 0) {
        tablaBitacoras.destroy();
        InitTableBitacoras();
    }

    OcultarLoader();
}
function onFailureResultObtenerBitacoras() {
    OcultarLoader();
}

function obtenerDetalleBitacora(idPedidoInterno) {
    $.ajax({
        url: rootUrl("/Bitacora/_DetalleBitacora"),
        data: { idPedidoInterno: idPedidoInterno },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (view) {           
            OcultarLoader();           
            html=view;
        },
        error: function (xhr, status) {            
            OcultarLoader();
        }
    });

}
