$(document).ready(function () {
    $(".btnGraficoVentas").click(function (evt) {
        evt.preventDefault();
        $(".btnGraficoVentas").removeClass("active");
        $(this).addClass("active");
        CargaGrafico(1, $(this).attr("tipoReporteGrafico"), "GraficoVentasPorFecha");

    });

    $(".btnTopProductos").click(function (evt) {
        evt.preventDefault();
        $(".btnTopProductos").removeClass("active");
        $(this).addClass("active");
        $("#btnTopProductos").text($(this).text());
        CargaGrafico(2, $(this).attr("tipoReporteGrafico"), "viewTopProductos");

    });

    $(".btnTopClientes").click(function (evt) {
        evt.preventDefault();
        $(".btnTopClientes").removeClass("active");
        $(this).addClass("active");
        $("#btnTopClientes").text($(this).text());
        CargaGrafico(3, $(this).attr("tipoReporteGrafico"), "viewTopClientes");

    });

    $(".btnTopProveedores").click(function (evt) {
        evt.preventDefault();
        $(".btnTopProveedores").removeClass("active");
        $(this).addClass("active");
        $("#btnTopProveedores").text($(this).text());
        CargaGrafico(4, $(this).attr("tipoReporteGrafico"), "viewTopProveedores");

    });

    Highcharts.setOptions({
        lang: {
            loading: 'Cargando...',
            months: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            weekdays: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            shortMonths: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            exportButtonTitle: "Exportar",
            printButtonTitle: "Importar",
            rangeSelectorFrom: "Desde",
            rangeSelectorTo: "Hasta",
            rangeSelectorZoom: "Período",
            downloadPNG: 'Descargar imagen PNG',
            downloadJPEG: 'Descargar imagen JPEG',
            downloadPDF: 'Descargar imagen PDF',
            downloadSVG: 'Descargar imagen SVG',
            printChart: 'Imprimir',
            resetZoom: 'Reiniciar zoom',
            resetZoomTitle: 'Reiniciar zoom',
            thousandsSep: ",",
            decimalPoint: '.'
        }
    });

});

function CargaGrafico(tipoGrafico, tipoReporteGrafico,nameDiv) {    
    $.ajax({
        url: rootUrl("/DashBoard/_Grafico"),
        data: { tipoGrafico: tipoGrafico, tipoReporteGrafico: tipoReporteGrafico },
        method: 'post',
        dataType: 'html',
        async: true,
        beforeSend: function (xhr) {
            ShowLoader();
        },
        success: function (data) {            
            $("#" + nameDiv).html(data);
            OcultarLoader();

        },
        error: function (xhr, status) {
            OcultarLoader();
        }
    });

}