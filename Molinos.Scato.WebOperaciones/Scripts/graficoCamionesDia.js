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

function GraficoCamionesDiaViewModel(validator) {
    // Inicializo observers
    var self = this;
    self.datasetDia = new DataSetChartLine(camionesDia, [], '#0070ff');
    self.materialSeleccionado = null;
    var myLineChartDia = null;
 
    self.actualizarGrafico = function () {
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
                        labels: ["30", "", "28", "", "26", "", "24", "", "22", "", "20", "", "18", "", "16", "", "14", "", "12", "", "10", "", "8", "", "6", "", "4", "", "2", "", "0"],
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
            $.unblockUI();
        }).error(function () {
            location.reload();
        });
    }
    self.generarGrafico = function () {
        if ($('#formDia').valid()) {
            BloquearPantalla();
            self.materialSeleccionado = $("#materialIdDia").val();
            self.actualizarGrafico();
        } else {
            validator.focusInvalid();
        }
    }
}

var graficoDia;
$(document).ready(function () {
    DefinirAutocompletar('#materialDescripcionDia', '#materialIdDia', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, null, null);
    var validator = $("#formDia").validate({ /* settings */ });
    graficoDia = new GraficoCamionesDiaViewModel(validator);
    ko.applyBindings(graficoDia, document.getElementById("graficocamionesdia"));
    graficoDia.generarGrafico();
});