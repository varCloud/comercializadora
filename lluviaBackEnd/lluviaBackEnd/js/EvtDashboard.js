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

    CargaDashboardInicial();
});

/**
 * Loads an HTML fragment into a container showing a per-section loader.
 * Aborts any previous in-flight request of the same container, so a stale
 * response can never overwrite a newer one.
 * opts: { method: 'GET' | 'POST' }
 */
function LoadSection($container, url, data, opts) {
    opts = opts || {};
    $container = $($container);
    if ($container.length === 0) { return; }

    var previous = $container.data("sectionXhr");
    if (previous) { previous.abort(); }

    var seq = ($container.data("sectionSeq") || 0) + 1;
    $container.data("sectionSeq", seq);

    $container.html(BuildSkeleton(opts.skeleton));

    var xhr = $.ajax({
        url: url,
        data: data,
        method: opts.method || 'GET',
        dataType: 'html',
        cache: false,
        success: function (html, status, jqXHR) {
            if ($container.data("sectionSeq") !== seq) { return; }
            // A redirect to the session-expired page is followed transparently by the browser.
            if (jqXHR.responseURL && /SesionExpirada/i.test(jqXHR.responseURL)) {
                window.location.href = jqXHR.responseURL;
                return;
            }
            $container.removeData("sectionXhr");
            // .html() (not innerHTML) so the <script> tags returned by the partial are executed.
            $container.html(html);
            if (opts.onLoaded) { opts.onLoaded($container); }
        },
        error: function (jqXHR, status) {
            if (status === "abort" || $container.data("sectionSeq") !== seq) { return; }
            $container.removeData("sectionXhr");
            var $error = $(
                '<div class="section-loader">' +
                '<div class="text-danger mb-2"><i class="fas fa-exclamation-circle"></i></div>' +
                '<div class="section-loader-text">No se pudo cargar la información.</div>' +
                '<button type="button" class="btn btn-sm btn-outline-primary mt-2">Reintentar</button>' +
                '</div>');
            $error.find("button").on("click", function () {
                LoadSection($container, url, data, opts);
            });
            $container.empty().append($error);
        }
    });
    $container.data("sectionXhr", xhr);
}

/**
 * Placeholder markup (shimmering gray blocks) shaped like the content being loaded.
 * kind: 'cards' | 'list' | 'chart'
 */
function BuildSkeleton(kind) {
    var i, html = '';
    if (kind === 'cards') {
        for (i = 0; i < 8; i++) {
            html += '<div class="col-md-3 col-sm-6 col-xs-12"><div class="card"><div class="card-body sk-card">' +
                '<div class="sk sk-icon"></div>' +
                '<div class="sk-lines"><div class="sk sk-line sk-w70"></div><div class="sk sk-line sk-w50"></div></div>' +
                '</div></div></div>';
        }
        return html;
    }
    if (kind === 'chart') {
        return '<div class="row"><div class="col-md-6"><div class="sk sk-chart"></div></div>' +
            '<div class="col-md-6"><div class="sk sk-chart"></div></div></div>';
    }
    for (i = 0; i < 5; i++) {
        html += '<div class="sk-row"><div class="sk sk-avatar"></div>' +
            '<div class="sk-lines"><div class="sk sk-line sk-w70"></div><div class="sk sk-line sk-w40"></div></div></div>';
    }
    return html;
}

/**
 * Count-up animation (0 -> value) for the KPI cards. It parses the already rendered text
 * (e.g. "$1,587,875.00", "29.22%", "12"), keeps prefix/suffix, decimals and thousands
 * separators, and restores the exact original text at the end.
 */
function AnimateCounters($container) {
    if (window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches) { return; }

    var duration = 1200;
    var targets = $container.find("h4.font-light, .card-wrap p span.font-weight-bold");

    targets.each(function () {
        $(this).contents().filter(function () {
            return this.nodeType === 3 && /\d/.test(this.nodeValue);
        }).each(function () {
            var node = this;
            var original = node.nodeValue;
            var match = original.match(/\d[\d,]*(\.\d+)?/);
            if (!match) { return; }

            var numText = match[0];
            var target = parseFloat(numText.replace(/,/g, ""));
            if (!isFinite(target)) { return; }

            var decimals = match[1] ? match[1].length - 1 : 0;
            var useCommas = numText.indexOf(",") !== -1;
            var prefix = original.substring(0, match.index);
            var suffix = original.substring(match.index + numText.length);

            function format(value) {
                return value.toLocaleString("en-US", {
                    minimumFractionDigits: decimals,
                    maximumFractionDigits: decimals,
                    useGrouping: useCommas
                });
            }

            var start = null;
            node.nodeValue = prefix + format(0) + suffix;

            function step(ts) {
                if (start === null) { start = ts; }
                var progress = Math.min((ts - start) / duration, 1);
                var eased = 1 - Math.pow(1 - progress, 3); // easeOutCubic
                if (progress < 1) {
                    node.nodeValue = prefix + format(target * eased) + suffix;
                    requestAnimationFrame(step);
                } else {
                    node.nodeValue = original;
                }
            }
            requestAnimationFrame(step);
        });
    });
}

function CargaGrafico(tipoGrafico, tipoReporteGrafico, nameDiv) {
    LoadSection(
        $("#" + nameDiv),
        rootUrl("/DashBoardData/_Grafico"),
        { tipoGrafico: tipoGrafico, tipoReporteGrafico: tipoReporteGrafico },
        { method: 'POST', skeleton: nameDiv === "GraficoVentasPorFecha" ? 'chart' : 'list' });
}

function CargaDashboardInicial() {
    LoadSection($("#viewIndicadores"), rootUrl("/DashBoardData/_Indicadores"), null, { skeleton: 'cards', onLoaded: AnimateCounters });
    LoadSection($("#viewVentasEstaciones"), rootUrl("/DashBoardData/_VentasEstaciones"), null, { skeleton: 'list' });

    // Charts and top-ten lists: defaults come from data-* attributes rendered by the view.
    $("#GraficoVentasPorFecha, #viewTopProductos, #viewTopClientes, #viewTopProveedores").each(function () {
        var $el = $(this);
        CargaGrafico($el.data("tipoGrafico"), $el.data("tipoReporte"), this.id);
    });
}