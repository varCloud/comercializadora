var table;
var iframe;
var tblProductos;
var CONVERSION_DE_UNIDADES = 1000;

function onBeginSubmitRelacion() {
    ShowLoader("Buscando...");
}

function onFailureResultRelacion(data) {
    OcultarLoader();
    ObtenerCombinacionProductos();
}

function onSuccessResultRelacion(data) {
    console.log(data);
    OcultarLoader();
    if (data.Estatus === 200) {
        MuestraToast("success", data.Mensaje)
        ObtenerCombinacionProductos();
        $('#mdlAgregarRelacionProceso').modal('hide');

    } else {
        MuestraToast("error", data.Mensaje)
    }

}

function ObtenerCombinacionProductos() {
    $.ajax({
        url: rootUrl("/ProductosAgranelAEnvasar/_ObtenerCombinacionProductos"),
        data: { idCliente: 0 },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("cargando clientes ...")
        },
        success: function (data) {
            OcultarLoader();
            tblProductos ?.destroy();
            $('#ViewRelacionProducto').html(data);
            initTblProductos();

        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
}

function initTblProductos() {
    var NombreTabla = "tblProductos";
    tblProductos = initDataTable(NombreTabla)

    new $.fn.dataTable.Buttons(tblProductos, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: "Clientes",
                customize: function (doc) {

                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.content[1].table.widths = ['5%', '20%', '20%', '15%', '15%', '10%', '15%'];

                    doc.content.splice(0, 1);
                    doc.pageMargins = [30, 85, 20, 30];
                    doc['header'] = (function () {
                        return {
                            columns: [
                                {
                                    image: logoBase64,
                                    width: 64,
                                    margin: [0, 20, -20, 0]
                                },
                                /*{
                                    alignment: 'left',
                                    italics: true,
                                    text: 'dataTables',
                                    fontSize: 18,
                                    margin: [10,0]
                                },*/
                                {
                                    alignment: 'center',
                                    fontSize: 14,
                                    text: "Clientes",
                                    margin: [0, 40, 80]
                                }
                            ],
                            margin: [10, 0]
                        }
                    });// fin del doc header*/
                    doc['footer'] = (function (page, pages) {
                        return {
                            columns: [
                                {
                                    alignment: 'right',
                                    text: ['pagina ', { text: page.toString() }, ' de ', { text: pages.toString() }]
                                }
                            ],
                            margin: [0, 0, 30]
                        }
                    });	// fin del doc footer*/	
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

            }
        ]
    });

    tblProductos.buttons(0, null).container().prependTo(
        tblProductos.table().container()
    );

    $('#' + NombreTabla + '_filter').append('&nbsp;&nbsp;&nbsp;<a href="#" class="btn btn-icon btn-success" name="" id="btnAgregarRelacion" data-toggle="tooltip" title="Agregar Relacion"><i class="fas fa-plus"></i></a>');
    InitBtnAgregar();
}

function resetForm() {
    $('.field-validation-error').html("");
    $("#idProductoAgranel").val("").trigger('change');
    $("#idProductoEnvasado").val("").trigger('change');
    $("#idProducoEnvase").val("").trigger('change');
    $("#idUnidadMedidad").val("").trigger('change');
    $('#btnReset').trigger('click');
    $('#idRelacionEnvasadoAgranel').val('0');
    $('#unidadMedidaConverter').html('');
}

function InitBtnAgregar() {
    $('#btnAgregarRelacion').click(function (e) {
        resetForm();
        disabledForm(false);
        $('#mdlAgregarRelacionProceso').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalRelacion').html("Agregar Relacion");
    });
}

function initEvents() {

    $("#valorUnidadMedida").keyup(function (e) {
        console.log("keyup", _esDecimal($(this), event), $(this))
        const unidadMedidaConverterDesc = $('#lstUnidadesMedida').val() == 2 ? 'K' : 'L'
        if (!_esDecimal(this, event)) {
            if (e.which == 13) {
                console.log($("#valorUnidadMedida").val())
            }
        }
        if ($("#valorUnidadMedida").val()) {
            const value = Number($("#valorUnidadMedida").val()) / CONVERSION_DE_UNIDADES
            $('#unidadMedidaConverter').html(`${value} (${unidadMedidaConverterDesc})`);
            $("#valorUnidadMedidaConverter").val(value)
        } else {//si esta limpio el control
            $('#unidadMedidaConverter').html(`0 (${unidadMedidaConverterDesc})`);
        }
    });

    $("#lstUnidadesMedida").change(function (evt) {
        var e = jQuery.Event("keyup");
        e.which = 49; // # Some key code value
        $("#valorUnidadMedida").trigger(e)
    });

}

function EditarRelacion(data, accion) {
    resetForm();
    disabledForm(accion === 'ver')
    $('#TituloModalRelacion').html((accion === 'ver' ? 'Relacion de productos ' : "Actualizar Relacion"));
    $("#idProductoAgranel").val(data.idProductoAgranel).trigger('change');
    $("#idProductoEnvasado").val(data.idProductoEnvasado).trigger('change');
    $("#idProducoEnvase").val(data.idProducoEnvase).trigger('change');
    $("#idUnidadMedidad").val(data.idUnidadMedidad).trigger('change');
    $('#idRelacionEnvasadoAgranel').val(data.idRelacionEnvasadoAgranel);
    $('#valorUnidadMedida').val(Number(data.valorUnidadMedida) * CONVERSION_DE_UNIDADES);
    $('#unidadMedidaConverter').html(data.valorUnidadMedida);
    $('#mdlAgregarRelacionProceso').modal({ backdrop: 'static', keyboard: false, show: true });
}

function disabledForm(isDisbaled) {
    $("#frmRelacion input").prop("disabled", false);
    $("#frmRelacion .select-multiple").prop("disabled", false);
    $("#btnBuscar").css("display", '');

    if (isDisbaled) {
        $("#frmRelacion input").prop("disabled", true);
        $("#frmRelacion .select-multiple").prop("disabled", true);
        $("#btnBuscar").css("display", 'none');
    }
}

function EliminarRelacion(id) {

    swal({
        title: 'Mensaje',
        text: '¿Estas seguro que desea eliminar la combinacion de los productos?',
        icon: 'info',
        buttons: ["Cancelar", "Aceptar"],
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    url: rootUrl("/ProductosAgranelAEnvasar/DesactivarCombinacionProductosEnvasadosAgranel"),
                    data: { idRelacionEnvasadoAgranel: id },
                    method: 'post',
                    dataType: 'json',
                    async: true,
                    beforeSend: function (xhr) {
                        ShowLoader("cargando clientes ...")
                    },
                    success: function (data) {
                        console.log(data.Mensaje)
                        OcultarLoader();
                        MuestraToast("success", data.Mensaje)
                        ObtenerCombinacionProductos();
                    },
                    error: function (xhr, status) {
                        console.log('Disculpe, existió un problema');
                        console.log(xhr);
                        console.log(status);
                        OcultarLoader();
                    }
                });
            } else {
                console.log("cancelar");
            }
        });
}

$(document).ready(function () {
    ObtenerCombinacionProductos()
    InitSelect2();
    initEvents();
    //Limpiar el select2  cuando se valida con Jquery validate 
    $('.select-multiple').on('change', function () {
        $(this).trigger('blur');
    });
});


