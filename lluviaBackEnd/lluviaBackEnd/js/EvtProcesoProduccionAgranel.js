var table;
var iframe;
var tblProductos;

function onBeginSubmitCostoProduccion() {
    ShowLoader("Buscando...");
}

function onSuccessResultCostoProduccion(data) {
    if (tblCostoProduccion != null)
        tblCostoProduccion.destroy();

    tituloReporte = "Listado Productos Produccion" + " " + $("#idMes option:selected").text() + " " + $("#idAnio option:selected").text();
    $('#ViewCostoProduccion').html(data);
    if ($("#tblCostoProduccion").length > 0)
        InitDataTableCostoProduccion();
    OcultarLoader();
}

function onFailureResultCostoProduccion() {
    OcultarLoader();
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
            tblProductos?.destroy();
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

function InitBtnAgregar() {
    $('#btnAgregarRelacion').click(function (e) {
        $('.field-validation-error').html("");
        $("#frmRelacion input").prop("disabled", false);
        $("#frmRelacion select").prop("disabled", false);
        $('#btnResetGuardarUsuario').trigger('click');
        $('#btnGuardarCliente').css('display', '');
        $('#idCliente').val('0');
        //para abrir el modal
        $('#mdlAgregarRelacionProceso').modal({ backdrop: 'static', keyboard: false, show: true });
        $('#TituloModalRelacion').html("Agregar Relacion");

    });
}


$(document).ready(function () {
    ObtenerCombinacionProductos()
    InitSelect2();
    //InitRangePicker('rangeProduccionAgranel', 'fechaIni', 'fechaFin');
    //$('#fechaIni').val($('#rangeProduccionAgranel').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$('#fechaFin').val($('#rangeProduccionAgranel').data('daterangepicker').startDate.format('YYYY-MM-DD'));
    //$("#btnLimpiarForm").click(function (evt) {
    //    $("#frmBuscarCargaMercanciaLiquidos").trigger("reset");
    //    $('#fechaIni').val('');
    //    $('#fechaFin').val('');
    //    $("#frmBuscarCargaMercanciaLiquidos .select-multiple").trigger("change");
    //});
    //$("#btnBuscar").trigger('click'); 
    //$("#btnLimpiarForm").click(function (evt) {
    //    $('#mdlAgregarCliente').modal({ backdrop: 'static', keyboard: false, show: true })
    //})
    
});


