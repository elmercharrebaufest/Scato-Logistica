
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

function GraficoToneladasPorDiaViewModel() {
    // Inicializo observers
    var self = this;
    var intervalo = 60;
    var multiplicador = 1000;
    self.datasetTotal = new DataSetChartLine("", [], null);
    self.fechaDesdeSeleccionada = null;
    self.fechaHastaSeleccionada = null;
    self.materialSeleccionado = null;
    self.myLineChartHidraulicas = null;
    self.hidraulicas = null;
    var graficoIniciado = false;

    self.mostrarGrafico = function () {
        var ctxh = $("#myChartToneladasPorDia");
        var labels = [];
        var data = [];
        if (self.hidraulicas) {
            data = $.grep(self.datasetTotal.data, function (e) { return $.inArray(e.Clave, self.hidraulicas) >= 0 });
        } else {
            data = self.datasetTotal.data;
        }
        $.each(data, function (i, n) {
            if ($.inArray(n.Clave, labels) < 0)
                labels.push(n.Clave);
        });

        $.each(data, function (i, n) {
            if ($.inArray(n.Clave, labels) < 0)
                labels.push(n.Clave);
        });

        var materiales = [];
        $.each(data, function (i, n) {
            if (n.Material && $.inArray(n.Material, materiales) < 0)
                materiales.push(n.Material);
        });

        var totals = [];
        $.each(labels, function (i, l) {
            var materialesHidraulica = $.grep(data, function (e) { return e.Clave == l });
            var totalHidraulica = 0;
            $.each(materialesHidraulica, function (i2, m) { totalHidraulica += m.Toneladas });

            totals.push(totalHidraulica);
        });

        var coloresMateriales = obtenerColoresParaGraficos(materiales.length);

        var datasets = $.map(materiales, function (m, i) {
            var color = coloresMateriales[i];
            return {
                label: m,
                data: $.map(labels, function (l) {
                    var h = $.grep(data, function (e) { return e.Clave == l && e.Material == m });
                    return h[0] ? h[0].Toneladas : 0;
                }),
                backgroundColor: color,
                borderColor: color,
                borderWidth: 1
            };
        });

        if (self.myLineChartHidraulicas) {
            self.myLineChartHidraulicas.destroy();
        }
        self.myLineChartHidraulicas = new Chart(ctxh, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: datasets
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
                            },
                            stacked: true
                        }
                    ],
                    xAxes: [
                        {
                            ticks: {
                                min: 0
                            },
                            stacked: true
                        }]
                },
                title: {
                    display: true,
                    text: textoToneladasPorDia
                },
                elements: {
                    point: { radius: 0 }
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem) {
                            return [materiales[tooltipItem.datasetIndex] + ': ' + Number(tooltipItem.yLabel).toString()];
                        },
                        title: function (tooltipItem) {
                            return [tooltipItem[0].label + " (Total: " + totals[tooltipItem[0].index] + ")"];
                        }
                    }
                }
            }
        });
    }
    self.actualizarGrafico = function (funcionRecursiva, offline) {
        graficoIniciado = true;

        if (!offline) {
            $.getJSON(urlGenerarToneladasPorDias, { MaterialId: self.materialSeleccionado, FechaHoraDesde: self.fechaDesdeSeleccionada, FechaHoraHasta: self.fechaHastaSeleccionada }, function (data) {
                self.datasetTotal.data = data.CamionesPorHidraulicaMaterial;
            }).complete(function () {
                self.mostrarGrafico();

                if (funcionRecursiva != null) {
                    setTimeout(funcionRecursiva, intervalo * multiplicador);
                }
            });
        } else {
            self.mostrarGrafico();
        }
    }
    function actualizarGraficoRecursivo() {
        self.actualizarGrafico(actualizarGraficoRecursivo);
    }

    self.generarGraficoToneldasPorDia = function () {
        self.materialSeleccionado = $("#materialIdToneladasPorDia").val();
        self.fechaDesdeSeleccionada = $("#FechaDesdeToneladas").val();
        self.fechaHastaSeleccionada = $("#FechaHastaToneladas").val();
        $('#errorSummaryToneladas').html('');

        if (!self.materialSeleccionado) {
            self.materialSeleccionado = 0;
        }

        if (!self.fechaDesdeSeleccionada || self.fechaDesdeSeleccionada == '') {
            self.mostrarErrorObligatorio('Fecha Desde');
            return;
        }

        if (!self.fechaHastaSeleccionada || self.fechaHastaSeleccionada == '') {
            self.mostrarErrorObligatorio('Fecha Hasta');
            return;
        }

        if (!graficoIniciado) {
            actualizarGraficoRecursivo();
        } else {
            self.actualizarGrafico();
        }
    }

    self.mostrarErrorObligatorio = function (campo) {
        $('#errorSummaryToneladas').html('* El campo ' + campo + ' es obligatorio.');
    }

    self.filtrarHidraulicas = function (checkboxContainerClass) {
        var visibles = [];
        $.each($('.' + checkboxContainerClass + ' input[type = "checkbox"]'), function (i, chk) {
            if ($(chk).is(":checked"))
                visibles.push($(chk).val());
        });

        self.hidraulicas = visibles;
    }
}

var graficoToneladasPorDia;
$(document).ready(function () {
    $("#FechaDesdeToneladas").datetimepicker({
        format: "dd/mm/yyyy hh:ii",
        autoclose: true,
        pickerPosition: "bottom-left",
    });
    $("#FechaHastaToneladas").datetimepicker({
        format: "dd/mm/yyyy hh:ii",
        autoclose: true,
        pickerPosition: "bottom-left",
    });

    $.each($('.checkboxesToneladas input[type="checkbox"]'), function (i, chk) {
        $(chk).prop("checked", true);
    });

    graficoToneladasPorDia = new GraficoToneladasPorDiaViewModel();
    ko.applyBindings(graficoToneladasPorDia, document.getElementById("graficoToneladasPorDia"));
    graficoToneladasPorDia.filtrarHidraulicas("checkboxesToneladas");
    graficoToneladasPorDia.generarGraficoToneldasPorDia();

    $('.checkboxesToneladas input[type="checkbox"]').on("change", function (e) {
        graficoToneladasPorDia.filtrarHidraulicas("checkboxesToneladas");
        graficoToneladasPorDia.mostrarGrafico();
        e.preventDefault();
    });
});