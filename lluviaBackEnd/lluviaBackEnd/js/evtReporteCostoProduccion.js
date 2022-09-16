var tblCostoProduccion;
var tituloReporte;
var arrayLineasProducto = [];
var arrayMeses = [];
var MesSeleccionado = "0";
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


    $("#idAlmacen").change(function (evt) {
        evt.preventDefault();
        var idAlmacen = 0;       
        if ($("#idAlmacen").val()>0)
            idAlmacen = $("#idAlmacen").val();
        ConsultaLineaAlmacen(idAlmacen);
       

    });

    $("#idAnio").change(function (evt) {
        evt.preventDefault();
        var idAnio = 0;
        if ($("#idAnio").val() > 0)
            idAnio = $("#idAnio").val();
        ConsultaMesesAnio(idAnio);


    });
});

//busqueda
function onBeginSubmitCostoProduccion() {
    ShowLoader("Buscando...");
}

function onSuccessResultCostoProduccion(data) {
    if (tblCostoProduccion != null)
        tblCostoProduccion.destroy();

    tituloReporte = "Indicador CostoProduccion" + " " + $("#idMes option:selected").text() + " " + $("#idAnio option:selected").text();
    $('#ViewCostoProduccion').html(data);
    if ($("#tblCostoProduccion").length > 0)
        InitDataTableCostoProduccion();
    OcultarLoader();
}

function onFailureResultCostoProduccion() {
    OcultarLoader();
}

function InitDataTableCostoProduccion() {
    var NombreTabla = "tblCostoProduccion";
    tblCostoProduccion = initDataTable(NombreTabla);

    new $.fn.dataTable.Buttons(tblCostoProduccion, {
        buttons: [
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a PDF',
                title: tituloReporte,
                customize: function (doc) {
                    doc.defaultStyle.fontSize = 8;
                    doc.styles.tableHeader.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    // doc.content[1].table.widths = ['10%', '25%', '15%', '15%', '20%', '15%'];
                    doc.pageMargins = [30, 85, 20, 30];
                    doc.content.splice(0, 1);
                    doc['header'] = SetHeaderPDF(tituloReporte);
                    doc['footer'] = (function (page, pages) { return setFooterPDF(page, pages) });
                },
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                },
            },
            {
                extend: 'excel',
                messageTop: tituloReporte,
                text: '<i class="fas fa-file-excel" style="font-size:20px;"></i>',
                className: '',
                titleAttr: 'Exportar a Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6,7]
                },
            },
        ],

    });

    tblCostoProduccion.buttons(0, null).container().prependTo(
        tblCostoProduccion  .table().container()
    );
}

function ConsultaLineaAlmacen(idAlmacen) {
    arrayLineasProducto = [];
    $.ajax({
        url: rootUrl("/LineaProducto/LineasAlmacen"),
        data: { idAlmacen: idAlmacen },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Cargando...");
        },
        success: function (data) {
            OcultarLoader();
          
            var i;
            for (i = 0; i < data.length; i++) {
                arrayLineasProducto.push({
                    id: data[i]['Value'],
                    text: data[i]['Text']
                });
                
            }
        
            InitSelect2LineasProducto();
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitSelect2LineasProducto() {
    $("#idLineaProducto").html('').select2();
    $('#idLineaProducto').select2({
        width: "100%",
        placeholder: "--TODOS--",
        data: arrayLineasProducto,

        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });

    $('#idLineaProducto').val("0").trigger('change');
}

function ConsultaMesesAnio(Anio) {
    arrayMeses = [];
    MesSeleccionado = "0";
    $.ajax({
        url: rootUrl("/Reportes/ObtenerMesesAnio"),
        data: { anio: Anio },
        method: 'post',
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader("Cargando...");
        },
        success: function (data) {
            OcultarLoader();
            
            var i;
            for (i = 0; i < data.length; i++) {
                arrayMeses.push({
                    id: data[i]['Value'],
                    text: data[i]['Text']
                });
                if (data[i]['Selected'] == true)
                    MesSeleccionado = data[i]['Value'];                             

            }
            console.log(arrayMeses);
            InitSelect2Meses();
            console.log(MesSeleccionado);
        },
        error: function (xhr, status) {
            console.log('hubo un problema pongase en contacto con el administrador del sistema');
            console.log(xhr);
            console.log(status);
        }
    });
}

function InitSelect2Meses() {
    $("#idMes").html('').select2();
    $('#idMes').select2({
        width: "100%",
        //placeholder: "--TODOS--",
        data: arrayMeses,

        language: {
            noResults: function () {
                return "No hay resultado";
            },
            searching: function () {
                return "Buscando..";
            }
        }
    });

    $('#idMes').val(MesSeleccionado).trigger('change');
}

