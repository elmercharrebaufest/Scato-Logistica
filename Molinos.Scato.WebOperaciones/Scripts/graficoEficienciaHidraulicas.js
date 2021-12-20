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

function GraficoEficienciaHidraulicaViewModel(validator) {
    // Inicializo observers
    var self = this;
    self.datasetTotal = new DataSetChartLine("", [], null);
    self.fechaSeleccionada = null;
    self.materialSeleccionado = null;
    var myLineChartHidraulicas = null;
 
    self.actualizarGrafico = function () {
        $.getJSON(urlGenerarEficienciaHidraulicas, { MaterialId: self.materialSeleccionado, Fecha: self.fechaSeleccionada }, function (data) {
            self.datasetTotal.data = data.CamionesPorHidraulica;
        }).complete(function () {
            var ctxh = $("#myChartHidraulicas");
            var labels = $.map(self.datasetTotal.data, function (n) {
                return n.Clave;
            });
            var valores = $.map(self.datasetTotal.data, function (n) {
                return n.Valor;
            });
            if (myLineChartHidraulicas) {
                myLineChartHidraulicas.destroy();
            }
            myLineChartHidraulicas = new Chart(ctxh, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [
                        {
                            label: textoHidraulicas,
                            data: valores,
                            backgroundColor: '#9be334',
                            borderColor: '#9be334',
                            borderWidth: 1
                        }]
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
                        },
                        elements: {
                            point: { radius: 0 }
                        }
                    }
                });
            $.unblockUI();
        }).error(function () {
            location.reload();
        });
    }
    self.generarGrafico = function () {
        if (checkDate('#Fecha') && $('#formHidraulica').valid()) {
            BloquearPantalla();
            self.materialSeleccionado = $("#materialIdHidraulica").val();
            self.fechaSeleccionada = $("#Fecha").val();
            self.actualizarGrafico();
        } else {
            validator.focusInvalid();
        }
    }
}

var graficoHidraulica;
$(document).ready(function () {
    DefinirAutocompletar('#materialDescripcionHidraulica', '#materialIdHidraulica', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, null, null);
    
    var validator = $("#formHidraulica").validate({ /* settings */ });

    graficoHidraulica = new GraficoEficienciaHidraulicaViewModel(validator);
    ko.applyBindings(graficoHidraulica, document.getElementById("graficoeficienciahidraulica"));
    graficoHidraulica.generarGrafico();
});