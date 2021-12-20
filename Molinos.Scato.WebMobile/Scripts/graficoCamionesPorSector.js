function DataSetChartLine(nombreDeLinea, data, color) {
    this.label = nombreDeLinea,
        this.fill = false,
        this.borderDash = [],
        this.borderDashOffset = 0.0,
        this.borderJoinStyle = 'miter',
        this.borderWidth = 2,
        this.data = data;
    if (color == null) {
        this.backgroundColor = '#000000';
        this.hoverBackgroundColor = '#000000';
    } else {
        this.backgroundColor = color;
        this.hoverBackgroundColor = color;
    }
}


function GraficoCamionesPorSectorViewModel() {
    // Inicializo observers
    var self = this;
    self.datasetTotal = new DataSetChartLine("", [], null);
    var myLineChartCamionesPorSector = null;

    self.actualizarGrafico = function () {
        $.getJSON(urlGenerarCamionesPorSector, function (data) {
            self.datasetTotal.data = data.CantidadEnSector;
        }).done(function () {
            var ctxh = $("#myChartCamionesPorSector");
            var labels = $.map(self.datasetTotal.data, function (n) {
                return n.NombreSector;
            });
            var valores = $.map(self.datasetTotal.data, function (n) {
                return n.CantidadCamiones;
            });
            var valorMaximo = 120;
            if (myLineChartCamionesPorSector) {
                myLineChartCamionesPorSector.destroy();
            }
            myLineChartCamionesPorSector = new Chart(ctxh, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Porcentaje de Ocupación',
                            data: valores,
                            backgroundColor: '#28a745 ',
                            borderColor: '#28a745 ',
                            borderWidth: 1
                        }]
                },
                options: {
                    //responsive: true,
                    maintainAspectRatio: false,
                    animation: false,
                    legend: {
                        display: false
                    },
                    tooltips: {
                        callbacks: {
                            label: tooltipItem => `${tooltipItem.yLabel}: ${tooltipItem.xLabel}`,
                            title: () => null,
                        }
                    },
                    scales: {
                        yAxes: [
                            {
                                ticks: {
                                    min: 0,
                                    suggestedMax: valorMaximo
                                }
                            }
                        ],
                        xAxes: [
                            {
                                ticks: {
                                    min: 0
                                }
                            }]
                    },
                    elements: {
                        point: { radius: 0 }
                    }
                }
            });
            $.unblockUI();
        }).fail(function () {

        });
    }
    self.generarGrafico = function () {
        BloquearPantalla();
        self.actualizarGrafico();
    }
}


var graficoCamionesPorSector;
$(document).ready(function () {

    graficoCamionesPorSector = new GraficoCamionesPorSectorViewModel();
    //ko.applyBindings(graficoCamionesPorSector, document.getElementById("graficoeficienciahidraulica"));
    graficoCamionesPorSector.generarGrafico();
    $('.ui-helper-hidden-accessible').hide();
});