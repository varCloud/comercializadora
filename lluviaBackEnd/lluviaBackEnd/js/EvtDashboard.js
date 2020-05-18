$(document).ready(function () {
    $(".btnGraficoVentas").click(function (evt) {
        evt.preventDefault();
        $(".btnGraficoVentas").removeClass("active");
        $(this).addClass("active");

        $.ajax({
            url: rootUrl("/DashBoard/_Grafico"),
            data: { tipoGrafico: 1, tipoReporteGrafico: $(this).attr("tipoReporteGrafico") },
            method: 'post',
            dataType: 'html',
            async: true,
            beforeSend: function (xhr) {
                ShowLoader();
            },
            success: function (data) {
                OcultarLoader();
                $("#GraficoVentasPorFecha").html(data);

            },
            error: function (xhr, status) {
                OcultarLoader();
            }
        });
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