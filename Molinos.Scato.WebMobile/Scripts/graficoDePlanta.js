function DataSetChartLine2(nombreActividad, historicoCamiones, color, historicoCantidadCamiones, rango) {
    this.label = nombreActividad;
    this.fill = false;
    this.tension = 0.1; // Changed from lineTension to tension
    this.borderCapStyle = 'butt';
    this.borderDash = [];
    this.borderDashOffset = 0.0;
    this.borderJoinStyle = 'miter';
    this.pointBorderWidth = 1;
    this.pointHoverRadius = 5;
    this.pointHoverBorderWidth = 2;
    this.pointRadius = 1;
    this.pointHitRadius = 10;
    this.spanGaps = false;
    this.data = historicoCamiones;
    this.historicoCantidadCamiones = historicoCantidadCamiones;
    this.rango = rango;

    if (color == null) {
        this.borderColor = '#000000';
    } else {
        this.borderColor = color;
    }
}

function Actividad(data, datasetsLineChart) {
    this.NombreActividad = ko.observable(data.NombreActividad);
    this.NombreActividadDesc = ko.observable(data.NombreActividadDesc);
    this.Rango = ko.observable(data.Rango);
    this.RangoNuevo = ko.observable(data.Rango);
    this.CantidadCamionesNoDemorados = ko.observable(data.CantidadCamionesNoDemorados);
    this.CantidadCamionesDemorados = ko.observable(data.CantidadCamionesDemorados);
    this.Color = ko.observable(data.Color);
    this.ColorNuevo = ko.observable(data.Color);

    this.HistoricoCamiones = [];
    this.HistoricoCantidadCamiones = [];
    for (var i = 0; i < 90; i++) {
        this.HistoricoCamiones.push("-");
        this.HistoricoCantidadCamiones.push(0);
    }
    this.HistoricoCamiones.push(data.CantidadCamionesNoDemorados + data.CantidadCamionesDemorados * 100 / data.Rango);
    this.HistoricoCantidadCamiones.push(data.CantidadCamionesNoDemorados + data.CantidadCamionesDemorados);
    this.Mostrar = ko.observable(true);
    this.MostrarColor = ko.observable(false);
    this.CantidadCamiones = ko.computed(function () { return this.CantidadCamionesNoDemorados() + this.CantidadCamionesDemorados() }, this);
    this.PorcentajeNoDemorado = ko.computed(function () { return 'width: ' + this.CantidadCamionesNoDemorados() / this.Rango() * 100 + '%' }, this);
    this.PorcentajeDemorado = ko.computed(function () { return 'width: ' + this.CantidadCamionesDemorados() / this.Rango() * 100 + '%' }, this);
    this.Porcentaje = ko.computed(function () { return this.CantidadCamiones() / this.Rango() * 100 }, this);
    this.BackgroundStyle = ko.computed(function () { return 'background-color: ' + this.Color() + ';' + 'margin-top:4px; border-color: black; cursor: pointer; width: 40px; height:21px; margin-left:3px' }, this);
    this.PorcentajeStyle = ko.computed(function () { return 'width: ' + this.CantidadCamiones() / this.Rango() * 100 + '%' }, this);
    this.DataSet = new DataSetChartLine2(data.NombreActividad, this.HistoricoCamiones, this.Color(), this.HistoricoCantidadCamiones, this.Rango());
    datasetsLineChart.push(this.DataSet);

    this.Actualizate = function () {
        this.Mostrar(!this.Mostrar());
        this.MostrarColor(!this.MostrarColor());
        m2.ActualizarDataSets(this, m2.datasetsLineChart);
        return true;
    };

    this.AbrirModal = function () {
        m2.nombreSeleccionado(this.NombreActividad());
        $('#modal').modal('show');
        $.validator.addMethod("Rango", function (value, element) {
            return value.length > 0 && value > 0;
        }, "Debe ingresar un Rango válido");
        if ($('.colorPicker-picker').length == 0) {
            $('#Color').colorPicker({ showHexField: false });

        }

    }

    this.Actualizar = function (data, intervalo, encolarValor) {
        this.CantidadCamionesNoDemorados(data.CantidadCamionesNoDemorados);
        this.CantidadCamionesDemorados(data.CantidadCamionesDemorados);

        if (encolarValor != null) {
            this.HistoricoCamiones.push((((data.CantidadCamionesDemorados + data.CantidadCamionesNoDemorados) * 100) / data.Rango));
            this.HistoricoCantidadCamiones.push(data.CantidadCamionesNoDemorados + data.CantidadCamionesDemorados);

            if (this.HistoricoCamiones.length > intervalo + 1) {
                this.HistoricoCamiones.shift();
                this.HistoricoCantidadCamiones.shift();

            }
        } else {
            if (this.HistoricoCamiones.length > 0) {
                this.HistoricoCamiones.pop();
                this.HistoricoCantidadCamiones.pop();

            }
            this.HistoricoCamiones.push((((data.CantidadCamionesDemorados + data.CantidadCamionesNoDemorados) * 100) / data.Rango));
            this.HistoricoCantidadCamiones.push(data.CantidadCamionesNoDemorados + data.CantidadCamionesDemorados);

        }

        if (data.Color == null) {
            data.Color = '#000000';
        }
        if (data.Rango == null) {
            data.Rango = 10;
        }

        this.Color(data.Color);

        this.DataSet.borderColor = data.Color;

        // this.Rango(data.Rango);
    }
}

function GraficoDePlantaViewModel() {
    // Inicializo observers
    var self = this;
    var intervalo = 15 * 6;
    var segundos = 10;
    var multiplicador = 1000;
    self.actividades = ko.observableArray([]);
    self.nombreSeleccionado = ko.observable();
    self.deshabilitar = ko.observable("Deshabilitar Todos");
    self.datasetsLineChart = [];
    self.datasetsLineChart.push(new DataSetChartLine2("", [100], '#fff'));
    var myLineChart = null;

    self.CancelarModal = function () {
        for (var j = 0; j < self.actividades().length; j++) {
            if (self.actividades()[j].NombreActividad() == self.nombreSeleccionado()) {
                self.actividades()[j].ColorNuevo(self.actividades()[j].Color())
                self.actividades()[j].RangoNuevo(self.actividades()[j].Rango())
            }
        }
    }

    self.Guardar = function () {
        var color, rango;
        for (var j = 0; j < self.actividades().length; j++) {
            if (self.actividades()[j].NombreActividad() == self.nombreSeleccionado()) {
                rango = self.actividades()[j].RangoNuevo();
                color = self.actividades()[j].ColorNuevo();
                self.actividades()[j].Color(self.actividades()[j].ColorNuevo())
                self.actividades()[j].Rango(self.actividades()[j].RangoNuevo())

            }
        }

        $.ajax({
            type: 'POST',
            url: modificar,
            dataType: 'json',
            data: {
                NombreActividad: self.nombreSeleccionado(),
                Color: color,
                Rango: rango

            },
            success: function (variant) {

                $('#modal').modal('hide');
                //self.actualizarGrafico(null);
            },
            error: function (ex) {
                if (ex.status == 200) {
                    $('#modal').modal('hide');
                }
            }
        });
    }

    self.actualizarGrafico = function (funcionRecursiva) {
        var ejeX = [];

        for (var i = 0; i <= intervalo; i++) {
            if (i == 0) {
                ejeX.push("15 mins");
            }
            else if (i == 30) {
                ejeX.push("10 mins");
            }
            else if (i == 60) {
                ejeX.push("5 mins");
            } else if (i == 90) {
                ejeX.push("0 mins");
            } else {
                ejeX.push("");
            }
        }

        $.getJSON(obtenerDatos, function (data) {

            $.each(data, function (index, value) {
                var match = ko.utils.arrayFirst(self.actividades(), function (item) {
                    return value.NombreActividad === item.NombreActividad();
                });

                if (!match) {

                    self.actividades.unshift(new Actividad(value, self.datasetsLineChart));
                } else {
                    match.Actualizar(value, intervalo, funcionRecursiva);
                }
            });

            $.each(self.actividades(), function (index, value) {
                var match = ko.utils.arrayFirst(data, function (item) {
                    return value.NombreActividad() === item.NombreActividad;
                });

                if (!match) {
                    value.Actualizar({ CantidadCamionesNoDemorados: 0, CantidadCamionesDemorados: 0, Color: value.Color() }, intervalo, funcionRecursiva);
                }
            });

        }).always(function () {
            self.actividades.sort(function (left, rigth) {
                var pleft = (left.CantidadCamiones() / left.Rango());
                var prigth = (rigth.CantidadCamiones() / rigth.Rango());
                return pleft > prigth ? -1 : pleft == prigth ? 0 : 1;
            });

            var ctx = $("#myChart");

            if (myLineChart != null) {
                myLineChart.update();
                DefinirAlturaMaximaY(myLineChart);
                myLineChart.update();

            } else {
                myLineChart = new Chart(ctx, {
                    type: 'line',

                    data: {
                        labels: ejeX,
                        datasets: self.datasetsLineChart
                    },
                    options: {
                        plugins: {
                            tooltip: {
                                callbacks: {
                                    label: function (tooltipItem) {
                                        if (tooltipItem.datasetIndex > 0) {
                                            return self.datasetsLineChart[tooltipItem.datasetIndex].label.replace(/([a-z])([A-Z])/g, '$1 $2') + " " + self.datasetsLineChart[tooltipItem.datasetIndex].historicoCantidadCamiones[tooltipItem.dataIndex] + "/" + self.datasetsLineChart[tooltipItem.datasetIndex].rango;
                                        }
                                    }
                                }
                            },
                            annotation: {
                                annotations: {
                                    box1: {
                                        type: 'box',
                                        xMin: 0,
                                        xMax: 90,
                                        yMin: 100,
                                        yMax: 100,
                                        backgroundColor: 'rgba(255, 0, 0, 0.3)',
                                        borderColor: 'rgba(255, 0, 0, 0.3)',
                                        borderWidth: 1,
                                        dblClick: function (e) {

                                        }
                                    }
                                }
                            }
                        },
                        responsive: true,
                        animation: false,
                        title: {
                            display: true,
                        },
                        legend: {
                            display: false,
                        },
                        scales: {
                            y: {
                                min: 0,
                                ticks: {
                                    callback: function (value, index, values) {
                                        return value + "%";
                                    }
                                }
                            },
                            x: {
                                min: 0
                            }
                        }
                    }
                });
            }

            InicializarPopover();
            if (funcionRecursiva != null) {
                setTimeout(funcionRecursiva, segundos * multiplicador);

            }

        });
    };
    function DefinirAlturaMaximaY(chartAlturaY) {
        if (chartAlturaY != null) {
            chartAlturaY.options.plugins.annotation.annotations.box1.yMax = chartAlturaY.scales.y._endValue;
        }

    };
    function actualizarGraficoRecursivo() {
        self.actualizarGrafico(actualizarGraficoRecursivo);
    }
    actualizarGraficoRecursivo();
    //////////////
    var checked = false;

    self.MostrarTodoChecked = function () {
        $.each(self.actividades(), function (index, value) {
            value.Mostrar(!checked);
            value.MostrarColor(checked);
            value.Actualizate();
        });
        checked = !checked;
        self.deshabilitar("Deshabilitar Todos")
        if (checked) {
            self.deshabilitar("Habilitar Todos")
        }
        return true;
    };

    self.ActualizarDataSets = function (actividad, datasets) {
        var index = datasets.indexOf(actividad.DataSet);
        if (actividad.Mostrar()) {
            datasets[index].hidden = false;
        }
        else {
            datasets[index].hidden = true;
        }
        DefinirAlturaMaximaY(myLineChart);
        myLineChart.update();
        return true;
    };

}

var m2;
$(document).ready(function () {

    $('#modal').modal({
        backdrop: 'static', keyboard: false
    });

    $("#modal").modal('hide');
    m2 = new GraficoDePlantaViewModel();
    ko.applyBindings(m2, document.getElementById("graficoDePlanta"));

    m2.actualizarGrafico(null);
    $(document).on('click', '.ajax-editar-link-grafico', function () {
        $.get(this.href, cargarDialogoEditarGrafico);
        return false;

    });

    $('.dialogo-editar-cancelar').click(function () {
        $('.dialogo-editar').modal('hide');
    });

    $('.dialogo-editar-guardar').click(function () {
        $('.dialogo-editar form').submit();
        if ($('.dialogo-editar form').valid())
            $(".dialogo-editar-guardar").attr("disabled", true);
    });
});

function editarRepuestaFormularioGrafico(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        $('#dialogo-editar').modal('hide');
        m2.actualizarGrafico(null);
    } else {
        cargarDialogoEditar(respuesta);
    }

    $('#dialogo-editar').modal({
        backdrop: 'static', keyboard: false
    }).css({
        'top': '30%',
        'margin-left': function () {
            return -($(this).width() / 2);
        },
        'left': '50%',
        'margin-top': function () {
            return -($(this).height() / 2.6);
        }
    });
    attachDataPickers();
}

function InicializarPopover() {
    var contenido;
    $('[data-bs-toggle="popover"]').mouseover(function () {
        $('[data-bs-toggle="popover"]').popover({
            html: true,
        });
    });
}
