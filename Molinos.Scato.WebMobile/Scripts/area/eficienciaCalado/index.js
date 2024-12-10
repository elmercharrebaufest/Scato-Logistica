function EficienciaCaladoVM(config) {
    this.containerId = config.containerId;
    this.generalIds = config.generalIds;
    this.vmData = config.vmData;
    this.labels = config.labels;
    this.messages = config.messages;
    this.constants = config.constants;
    this.models = config.models;
    this.generalUrls = config.generalUrls;
}
EficienciaCaladoVM.prototype = {
    onReady: function () {
        let self = this;
        self.vm = {
            mainModule: {
                selectors: {
                    graficoPorHora: $('#' + self.generalIds.graficoEficienciaPorHora),
                    graficoPorTurno: $('#' + self.generalIds.graficoEficienciaPorTurno),
                    cantidadCamiones: $('#' + self.generalIds.cantidadCamionesPendientes),
                    barChartPorHora: null,
                    barChartPorTurno: null
                },
                states: {},
                actions: {}, //Eventos que hacen feedback
                methods: {
                    refrescarPendientesPorCalar: function () {
                        $.ajax({
                            type: 'GET',
                            dataType: "json",
                            url: self.generalUrls.obtenerCantidadCamiones,
                            success: function (data) {
                                if (data != null && data != "") {
                                    self.vm.mainModule.selectors.cantidadCamiones.text(data);
                                } else {
                                    self.vm.mainModule.selectors.cantidadCamiones.text(0);
                                }
                            },
                            error: function (error) {
                    },
                        }).always(function () {
                        });
                    },

                    refrescarGraficoEficiencia: function () {
                        $.ajax({
                            type: 'GET',
                            dataType: "json",
                            url: self.generalUrls.obtenerEficiencia,
                            success: function (data) {
                                self.vm.mainModule.methods.dibujarGraficoEficiencia(data[0].Calles, data[0].Porcentajes, self.vm.mainModule.methods.obtenerColoresBarras(data[0].Porcentajes), self.vm.mainModule.selectors.graficoPorHora, self.vm.mainModule.selectors.barChartPorHora);
                                self.vm.mainModule.methods.dibujarGraficoEficiencia(data[1].Calles, data[1].Porcentajes, self.vm.mainModule.methods.obtenerColoresBarras(data[1].Porcentajes), self.vm.mainModule.selectors.graficoPorTurno, self.vm.mainModule.selectors.barChartPorTurno);
                            },
                            error: function (error) {
                    },
                        }).always(function () {
                        });
                    },

                    dibujarGraficoEficiencia: function (calles, porcentajes, colores, canvas, barChartRef) {
                        if (barChartRef) {
                            barChartRef.destroy(); // Destruir el gráfico existente
                        }
                        let limite = 100;
                        $.each(porcentajes, function (index, value) {
                            if (value > limite) {
                                limite = value;
                            }
                        });
                        var ctxh = canvas;
                        let newChart = new Chart(ctxh, { // Crear nuevo gráfico
                            type: 'bar',
                            data: {
                                labels: calles,
                                datasets: [{
                                    barPercentage: 0.5,
                                    barThickness: 60,
                                    maxBarThickness: 60,
                                    minBarLength: 0,
                                    data: porcentajes,
                                    backgroundColor: colores,
                                    borderColor: colores
                                }],
                            },
                            options: {
                                maintainAspectRatio: false,
                                animation: false,
                                plugins: {
                                    legend: {
                                        display: false
                                    }
                                },
                                scales: {
                                    y: {
                                        beginAtZero: true,
                                        min: 0,
                                        max: limite
                                    },
                                    x: {
                                        beginAtZero: true
                                    }
                                }
                            }
                        });

                        // Actualizar la referencia del gráfico
                        if (canvas.attr('id') === self.generalIds.graficoEficienciaPorHora) {
                            self.vm.mainModule.selectors.barChartPorHora = newChart;
                        } else if (canvas.attr('id') === self.generalIds.graficoEficienciaPorTurno) {
                            self.vm.mainModule.selectors.barChartPorTurno = newChart;
                        }
                    },

                    obtenerColoresBarras: function (porcentajes) {
                        let coloresBar = [];
                        $.each(porcentajes, function (index, value) {
                            if (value <= 20) {
                                coloresBar.push("rgb(255, 0, 0)");
                            } else if (value <= 50) {
                                coloresBar.push("rgb(236, 126, 49)");
                            } else {
                                coloresBar.push("rgb(83, 130, 53)");
                            }
                        });
                        return coloresBar;
                    }
                }, //Metodos internos
                models: {}, //Crear modelo
                events: {}, //Eventos que no hacen feedback
                validations: {}, //Validaciones
            },
        };

        self.init();
    },
    init: function () {
        let self = this;
        self.vm.mainModule.methods.refrescarGraficoEficiencia();
        self.vm.mainModule.methods.refrescarPendientesPorCalar();
        setInterval(function () {
            self.vm.mainModule.methods.refrescarGraficoEficiencia();
            self.vm.mainModule.methods.refrescarPendientesPorCalar();
        }, 10000);
    }
}