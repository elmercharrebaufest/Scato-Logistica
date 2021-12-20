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
        //this.pointBackgroundColor = "#fff",
        this.pointBorderWidth = 1,
        this.pointHoverRadius = 5,
        this.pointHoverBackgroundColor = "rgba(75,192,192,1)",
        this.pointHoverBorderColor = "rgba(220,220,220,1)",
        this.pointHoverBorderWidth = 2,
        this.pointRadius = 1,
        this.pointHitRadius = 10,
        this.spanGaps = false,
        this.data = data;
    if (color === null) {
        this.borderColor = '#000000';
    } else {
        this.borderColor = color;
    }
}

var colores = ["#ff0000", "#8500ff", "#0400ff", "#1bff00", "#ccff00", "#ff0081", "#ff5e00", "#00ffff", "#000000", "#40bf96"];

function GraficoToneladasPorRangoDeDiasViewModel() {
    // Inicializo observers
    var self = this;
    var intervalo = 60;
    var multiplicador = 1000;
    self.fechaSeleccionada = null;
    self.materialSeleccionado = null;
    var myLineChartHoras = null;
    var graficoIniciado = false;

    self.actualizarGrafico = function (funcionRecursiva) {

        graficoIniciado = true;
        $.getJSON(urlGenerarToneladasPorRangoDeDia, { MaterialId: self.materialSeleccionado, FechaDesde: self.fechaDesde, FechaHasta: self.fechaHasta }, function (data) {
            datosToneladas = data.Toneladas;
        }).done(function () {
            var ctxh = $("#myChartToneladas");
            var labels = $.map(datosToneladas, function (n) {
                return n.Fecha;
            }).filter(function (elem, index, self) {
                return index === self.indexOf(elem);
            });

            var codigoUnico = datosToneladas.map(function (item) {
                return item.Codigo;
            }).filter(function (elem, index, self) {
                return index === self.indexOf(elem);
            }).filter(function (codigo) {
                return codigo !== null;
            });

            var dataSets = [];
            if (codigoUnico.length !== 0) {

                codigoUnico.forEach(function (cod, index) {
                    var dataSet = [];
                    labels.forEach(function (fecha) {
                        if (datosToneladas.some(function (e) { return e.Codigo === cod && e.Fecha === fecha; })) {
                            dataSet.push(datosToneladas.filter(function (e) { return e.Codigo === cod && e.Fecha === fecha; })[0].Toneladas);
                        } else {
                            dataSet.push(0);
                        }
                    });
                    dataSets.push(new DataSetChartLine(cod, dataSet, colores[index]));
                });

            } else {
                codigoUnico.push("Sin Datos");
                var dataSet = [];
                labels.forEach(function (it) {
                    dataSet.push(0);
                });
                dataSets.push(new DataSetChartLine("Sin Datos", dataSet, '#9be334'));
            }


            if (myLineChartHoras !== null) {
                myLineChartHoras.destroy();
            }

            myLineChartHoras = null;
            if (myLineChartHoras !== null) {
                myLineChartHoras.update();
            } else {
                myLineChartHoras = new Chart(ctxh, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: dataSets
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
                        tooltips: {
                            callbacks: {
                                label: function (tooltipItem) {
                                    console.log(tooltipItem);
                                    return codigoUnico[tooltipItem.datasetIndex] + ": " + tooltipItem.yLabel + " Tn.";
                                }
                            }
                        }
                    }
                });
            }
            //if (funcionRecursiva !== null) {
            //    setTimeout(funcionRecursiva, intervalo * multiplicador);
            //}
            }).fail(function () {
            
        });
    };

    function actualizarGraficoRecursivo() {
        self.actualizarGrafico(actualizarGraficoRecursivo);
    }

    self.generarGraficoToneladasPorRangoDeDias = function () {
        self.fechaDesde = $("#FechaDesde").val();
        self.fechaHasta = $("#FechaHasta").val();
        self.materialSeleccionado = $("#materialIdTnDia").val();
        if (!graficoIniciado) {
            actualizarGraficoRecursivo();
        } else {
            self.actualizarGrafico();
        }
    };
}

var grafico;
$(document).ready(function () {
    DefinirAutocompletar('#materialDescripcionTnDia', '#materialIdTnDia', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, null, null);
    grafico = new GraficoToneladasPorRangoDeDiasViewModel();
    ko.applyBindings(grafico, document.getElementById("graficotoneladasporrangodedias"));
    grafico.generarGraficoToneladasPorRangoDeDias();
});