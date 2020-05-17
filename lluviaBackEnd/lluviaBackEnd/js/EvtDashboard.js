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

 

});