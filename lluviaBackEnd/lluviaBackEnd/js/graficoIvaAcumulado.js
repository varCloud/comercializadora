// Create the chart
Highcharts.chart('containerGraficoIva', {
    chart: {
        type: 'column'
    },
    title: {
        text: 'IVA COBRADO DE VENTAS & PEDIDOS ESPECIALES'
    },
    subtitle: {
        text: ''
    },
    accessibility: {
        announceNewData: {
            enabled: true
        }
    },
    xAxis: {
        type: 'category'
    },
    yAxis: {
        title: {
            text: 'IVA TOTAL ACUMULADO'
        }

    },
    legend: {
        enabled: false
    },
    plotOptions: {
        series: {
            borderWidth: 0,
            dataLabels: {
                enabled: true,
                format: '{point.y:.1f}'
            }
        }
    },

    tooltip: {
        headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
        pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.1f}</b><br/>'
    },

    series: [
        {
            name: "IVA Acumulado",
            colorByPoint: true,
            data: dataGraficoIVAAcumulado
        }
    ]
});