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

function GraficoCamionesViewModel(validator) {
    // Inicializo observers
    var self = this;
    self.datasetHistorico = new DataSetChartLine(textoHistorico, [], '#ff5722');
    self.datasetActual = new DataSetChartLine(textoActual, [], '#0070ff');
    self.fechaSeleccionada = null;
    self.materialSeleccionado = null;
    var myLineChartHoras = null;
 
    self.actualizarGrafico = function () {
        $.getJSON(urlGenerarCamionesHora, { MaterialId: self.materialSeleccionado, FechaVieja: self.fechaSeleccionada }, function (data) {
            self.datasetHistorico.data = data.CamionesHistorico;
            self.datasetActual.data = data.CamionesActuales;
        }).complete(function () {
            var ctxh = $("#myChartCamiones");
            if (myLineChartHoras != null) {
                myLineChartHoras.update();
            } else {
                myLineChartHoras = new Chart(ctxh, {
                    type: 'line',
                    data: {
                        labels: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23],
                        datasets: [self.datasetHistorico, self.datasetActual]
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
            $.unblockUI();
        }).error(function () {
            location.reload();
        });
    }
    self.generarGraficoHora = function () {
        if (checkDate('#FechaVieja') && $('#formHora').valid()) {
            BloquearPantalla();
            self.fechaSeleccionada = $("#FechaVieja").val();
            self.materialSeleccionado = $("#materialIdHora").val();
            self.actualizarGrafico();
        } else {
            validator.focusInvalid();
        }
    }
}

var grafico;
$(document).ready(function () {
    BloquearPantalla();
    DefinirAutocompletar('#materialDescripcionHora', '#materialIdHora', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, null, null);
    var validator = $("#formHora").validate({ /* settings */ });
    grafico = new GraficoCamionesViewModel(validator);
    ko.applyBindings(grafico, document.getElementById("graficocamioneshora"));
    grafico.generarGraficoHora();
});