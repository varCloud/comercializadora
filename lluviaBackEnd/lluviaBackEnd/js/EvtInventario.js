var table;
var iframe;
var tablaInventario;


//busqueda
function onBeginSubmitProductos() {
    ShowLoader("Buscando...");
}
function onSuccessResultProductos(data) {
    $('#rowProductos').html(data);
    tablaInventario.destroy();
    InitDataTableInventario();
    OcultarLoader();
}
function onFailureResultProductos() {
    OcultarLoader();
}

function InitDataTableInventario() {
    var NombreTabla = "tablaRepProductos";
    tablaInventario = initDataTable(NombreTabla);
    if ($("#tablaRepProductos").length > 0) {
        new $.fn.dataTable.Buttons(tablaInventario, {
            buttons: [
                {
                    extend: 'pdfHtml5',
                    text: '<i class="fas fa-file-pdf" style="font-size:20px;"></i>',
                    className: '',
                    titleAttr: 'Exportar a PDF',
                    title: "Inventario",
                    customize: function (doc) {
                        doc.defaultStyle.fontSize = 8;
                        doc.styles.tableHeader.fontSize = 10;
                        doc.defaultStyle.alignment = 'center';
                        //doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '20%', '10%'];
                        doc.pageMargins = [30, 85, 20, 30];
                        doc.content.splice(0, 1);
                        doc['header'] = SetHeaderPDF("Inventario");
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

        tablaInventario.buttons(0, null).container().prependTo(
            tablaInventario.table().container()
        );
    }
}



$("#ReporteGeneral").click(function (evt) {
    evt.preventDefault();
    $.ajax({
        url: rootUrl("/Reportes/ReporteGeneral"),
        data: JSON.stringify({ id: 1 }),
        method: 'post',
        dataType: 'text',
        async: true,
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            ShowLoader("Generando Reporte General...")
        },
        success: function (data) {
            OcultarLoader();
            const today = new Date();
            const yyyy = today.getFullYear();
            let mm = today.getMonth() + 1; 
            let dd = today.getDate();
            if (dd < 10) dd = '0' + dd;
            if (mm < 10) mm = '0' + mm;
            let nombreArchivo = "ReporteInventarioGeneral_" + dd + mm + yyyy + ".csv";
            var encodedUri = 'data:application/csv;charset=utf-8,' + encodeURIComponent(data);
            var link = document.createElement("a");
            link.setAttribute("href", encodedUri);
            link.setAttribute("download", nombreArchivo);
            link.innerHTML = "Descargar Reporte";
            document.body.appendChild(link);
            link.click();
        },
        error: function (xhr, status) {
            console.log(data);
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
})

$("#ReportePorUbicacion").click(function (evt) {
    evt.preventDefault();
    $.ajax({
        url: rootUrl("/Reportes/ReporteGeneral"),
        data: JSON.stringify({ id: 2 }),
        method: 'post',
        dataType: 'text',
        async: true,
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            ShowLoader("Generando Reporte por Ubicación...")
        },
        success: function (data) {
            OcultarLoader();
            const today = new Date();
            const yyyy = today.getFullYear();
            let mm = today.getMonth() + 1;
            let dd = today.getDate();
            if (dd < 10) dd = '0' + dd;
            if (mm < 10) mm = '0' + mm;
            let nombreArchivo = "ReporteInventarioUbicacion_" + dd + mm + yyyy + ".csv";
            var encodedUri = 'data:application/csv;charset=utf-8,' + encodeURIComponent(data);
            var link = document.createElement("a");
            link.setAttribute("href", encodedUri);
            link.setAttribute("download", nombreArchivo);
            link.innerHTML = "Descargar Reporte";
            document.body.appendChild(link);
            link.click();
        },
        error: function (xhr, status) {
            console.log(data);
            console.log('Disculpe, existió un problema');
            console.log(xhr);
            console.log(status);
            OcultarLoader();
        }
    });
})

    $(document).ready(function () {

        InitDataTableInventario();
        InitSelect2();

        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0 so need to add 1 to make it 1!
        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd
        }
        if (mm < 10) {
            mm = '0' + mm
        }

        today = yyyy + '-' + mm + '-' + dd;
        $("#fechaAlta").attr("max", today);
        //document.getElementById("datefield").setAttribute("min", today);

        //InitRangePicker('rangeInventario', 'fechaIni', 'fechaFin');
        ////$('#idLineaProductoBusqueda').val('0');
        //$('#fechaIni').val($('#rangeInventario').data('daterangepicker').startDate.format('YYYY-MM-DD'));
        //$('#fechaFin').val($('#rangeInventario').data('daterangepicker').startDate.format('YYYY-MM-DD'));

    });



