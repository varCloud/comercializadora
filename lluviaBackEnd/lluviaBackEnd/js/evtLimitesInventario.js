var tblLimitesInventario;
$(document).ready(function () {
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

    $("#frmBuscarLimitesInventario").submit();
});

//busqueda
function onBeginSubmitLimitesInventario() {
    ShowLoader("Buscando...");
}
function onSuccessResultLimitesInventario(data) {
    if (tblLimitesInventario != null)
        tblLimitesInventario.destroy();

    $('#ViewLimitesInventario').html(data);
    if ($("#tblLimitesInventario").length > 0)
        InitDataTableLimitesInventario();
    OcultarLoader();
}
function onFailureResultLimitesInventario() {
    OcultarLoader();
}

function InitDataTableLimitesInventario() {
    var NombreTabla = "tblLimitesInventario";
    tblLimitesInventario = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblLimitesInventario, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Indicador LimitesInventario",
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    // doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF("Indicador LimitesInventario");
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6],
                    format: {
                        body: function (data, row, column, node) {
                            //si es la 3 o la cual obtenemos el .val 
                            //var isInput = $(data).is("input") ? true : false;                          

                            return (column === 3 || column === 4) ?
                                $(data).val() :
                                data;
                        }
                    }
                },
            },
            {
                extend: 'excel',
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',             
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6],
                    format: {
                        body: function (data, row, column, node) {
                            //si es la 3 o la cual obtenemos el .val 
                            //var isInput = $(data).is("input") ? true : false;                          

                            return (column === 3 || column===4) ?
                                $(data).val():
                                data;
                        }
                    }
                },
            },
        ],

    });

    tblLimitesInventario.buttons(0, null).container().prependTo(
        tblLimitesInventario.table().container()
    );
}

function actualizaLimiteInventario(idLimiteInventario, idAlmacen, idProducto, maximo, minimo, campoActualizar, valorAnterior) {

    if ($("#" + campoActualizar + "_" + idLimiteInventario).val() == valorAnterior)
        return;

    if ($.trim($("#" + campoActualizar + "_" + idLimiteInventario).val()) == "") {
        $("#" + campoActualizar + "_" + idLimiteInventario).focus();
        MuestraToast("error", "El " + campoActualizar+" que desea actualizar no puede ir vacio");
        return;
    }

    if (minimo > maximo)
    {
        $("#" + campoActualizar + "_" + idLimiteInventario).focus();
        MuestraToast("error", "El valor minimo no puede ser mayor que el valor maximo");
        return;
    }



    swal({
        title: '',
        text: 'Estas seguro que deseas actualizar el valor ' + campoActualizar + '?',
        icon: '',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {

                var limiteInvetario = new Object();
                limiteInvetario.idProducto = idProducto;
                limiteInvetario.idAlmacen = idAlmacen;
                limiteInvetario.minimo = minimo;
                limiteInvetario.maximo = maximo;
        

                var dataToPost = JSON.stringify({ limiteInvetario: limiteInvetario });

                $.ajax({
                    url: rootUrl("/Productos/ActualizaLimiteInventario"),
                    data: dataToPost,
                    method: 'POST',
                    dataType: 'JSON',
                    contentType: "application/json; charset=utf-8",
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader("Actualizando.");
                    },
                    success: function (data) {
                        if (data.Estatus == 200) {
                            MuestraToast('success', "El valor " + campoActualizar + " se ha actualizado de manera correcta" );

                        } else {
                            MuestraToast("error", data.Mensaje);
                        }

                        $("#frmBuscarLimitesInventario").submit();

                        OcultarLoader();


                    },
                    error: function (xhr, status) {
                        OcultarLoader();
                        console.log('Hubo un problema al actualizar el limite de inventario, contactese con el administrador del sistema');
                        console.log(xhr);
                        console.log(status);
                    }
                });
            } else {
                $("#" + campoActualizar + "_" + idLimiteInventario).val(valorAnterior);
            }
        });
}
