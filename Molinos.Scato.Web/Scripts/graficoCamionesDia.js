function DataSetChartLine(nombreDeLinea, data, color) {
    this.label = nombreDeLinea,
    this.fill = false,
    this.lineTension = 0.1,
    this.backgroundColor = "rgba(75,192,192,0.4)",
    this.borderColor = "rgba(75,192,192,1)",
    this.borderCapStyle = 'butt',
    this.borderDash = [],
    this.borderDashOffset = 0.0,
    this.borderJoinStyle = 'miter',
    this.pointBorderColor = "rgba(75,192,192,1)",
    this.pointBackgroundColor = "#fff",
    this.pointBorderWidth = 1,
    this.pointHoverRadius = 5,
    this.pointHoverBackgroundColor = "rgba(75,192,192,1)",
    this.pointHoverBorderColor = "rgba(220,220,220,1)",
    this.pointHoverBorderWidth = 2,
    this.pointRadius = 1,
    this.pointHitRadius = 10,
    this.spanGaps = false,
    this.data = data;
    if (color == null) {
        this.borderColor = '#000000';
    } else {
        this.borderColor = color;
    }
}

function GraficoCamionesDiaViewModel() {
    // Inicializo observers
    var self = this;
    var intervalo = 60;
    var multiplicador = 1000;
    self.datasetDia = new DataSetChartLine(camionesDia, [], '#0070ff');
    self.materialSeleccionado = null;
    var myLineChartDia = null;
    var graficoIniciado = false;
 
    self.actualizarGrafico = function (funcionRecursiva) {
        graficoIniciado = true;
        $.getJSON(urlGenerarCamionesDia, { MaterialId: self.materialSeleccionado }, function (data) {
            self.datasetDia.data = data.CamionesDia;
        }).complete(function () {
            var ctxd = $("#myChartCamionesDia");
            if (myLineChartDia != null) {
                myLineChartDia.update();
            } else {
                myLineChartDia = new Chart(ctxd, {
                    type: 'line',
                    data: {
                        labels: ["30", "29", "28", "27", "26", "25", "24", "23", "22", "21", "20", "19", "18", "17", "16", "15", "14", "13", "12", "11", "10", "9", "8", "7", "6", "5", "4", "3", "2", "1", "0"],
                        datasets: [self.datasetDia]
                    },
                    options: {
                        animation: false,
                        legend: {
                            display: true
                        },
                        scales: {
                            yAxes: [
                                {
                                    ticks: {
                                        min: 0
                                    }
                                }
                            ],
                            xAxes: [
                            {
                                ticks: {
                                    min: 0
                                }
                            }]
                        }
                    }
                });
            }
            if (funcionRecursiva != null) {
                setTimeout(funcionRecursiva, intervalo * multiplicador);
            }
        });
    }

    function actualizarGraficoRecursivo() {
        self.actualizarGrafico(actualizarGraficoRecursivo);
    }

    self.generarGrafico = function() {
        self.materialSeleccionado = $("#materialIdDia").val();
        if (!graficoIniciado) {
            actualizarGraficoRecursivo();
        } else {
            self.actualizarGrafico();
        }
    }
}

var graficoDia;
$(document).ready(function () {
    DefinirAutocompletar('#materialDescripcionDia', '#materialIdDia', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, null, null);
    graficoDia = new GraficoCamionesDiaViewModel();
    ko.applyBindings(graficoDia, document.getElementById("graficocamionesdia"));
    graficoDia.generarGrafico();
});