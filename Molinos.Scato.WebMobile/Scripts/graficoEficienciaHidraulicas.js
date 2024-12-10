function DataSetChartLine(nombreDeLinea, data, color) {
    this.label = nombreDeLinea;
    this.fill = false; // Se mantiene el llenado desactivado
    this.borderDash = []; // Patrón de línea discontinua
    this.borderDashOffset = 0.0; // Desplazamiento del patrón de línea discontinua
    this.borderJoinStyle = 'miter'; // Estilo de unión de borde
    this.borderWidth = 2; // Ancho del borde
    this.data = data; // Datos del conjunto
    // Establecer color de fondo y hover
    this.backgroundColor = color == null ? '#000000' : color;
    this.hoverBackgroundColor = color == null ? '#000000' : color;
}

function GraficoEficienciaHidraulicaViewModel(validator) {
    // Inicializo observers
    var self = this;
    self.datasetTotal = new DataSetChartLine("", [], null);
    self.fechaDesdeSeleccionada = null;
    self.fechaHastaSeleccionada = null;
    self.materialSeleccionado = null;
    self.myLineChartHidraulicas = null;
    self.hidraulicas = null;

    self.mostrarGrafico = function () {
        var ctxh = $("#myChartHidraulicas");
        var labels = [];
        var data = [];

        // Filtrar datos según hidraulicas seleccionadas
        if (self.hidraulicas) {
            data = self.datasetTotal.data.filter(e => self.hidraulicas.includes(e.Clave));
        } else {
            data = self.datasetTotal.data;
        }

        // Obtener etiquetas únicas
        data.forEach(n => {
            if (!labels.includes(n.Clave)) {
                labels.push(n.Clave);
            }
        });

        var materiales = [];
        data.forEach(n => {
            if (n.Material && !materiales.includes(n.Material)) {
                materiales.push(n.Material);
            }
        });

        var totals = labels.map(l => {
            var materialesHidraulica = data.filter(e => e.Clave === l);
            return materialesHidraulica.reduce((total, m) => total + m.Valor, 0); // Sumar valores
        });

        var coloresMateriales = obtenerColoresParaGraficos(materiales.length); // Obtener colores

        // Crear datasets
        var datasets = materiales.map((m, i) => {
            var color = coloresMateriales[i];
            return {
                label: m,
                data: labels.map(l => {
                    var h = data.find(e => e.Clave === l && e.Material === m);
                    return h ? h.Valor : 0; // Devolver valor o 0
                }),
                backgroundColor: color,
                borderColor: color,
                borderWidth: 1
            };
        });

        // Destruir gráfico anterior si existe
        if (self.myLineChartHidraulicas) {
            self.myLineChartHidraulicas.destroy();
        }

        // Crear nuevo gráfico
        self.myLineChartHidraulicas = new Chart(ctxh, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: datasets
            },
            options: {
                animation: false,
                plugins: { // Actualizado para usar plugins
                    legend: {
                        display: true
                    },
                    title: {
                        display: true,
                        text: textoHidraulicas // Título del gráfico
                    },
                    tooltip: { // Cambiado para la nueva estructura
                        callbacks: {
                            label: function (tooltipItem) {
                                return `${materiales[tooltipItem.datasetIndex]}: ${Number(tooltipItem.parsed.y).toString()}`; // Acceso actualizado a y
                            },
                            title: function (tooltipItem) {
                                return `${tooltipItem[0].label} (Total: ${totals[tooltipItem[0].dataIndex]})`; // Total en el tooltip
                            }
                        }
                    }
                },
                scales: {
                    y: { // Actualizado para la nueva sintaxis de escalas
                        min: 0,
                        stacked: true // Asegura que los datos se apilen
                    },
                    x: { // Actualizado para la nueva sintaxis de escalas
                        stacked: true // Asegura que los datos se apilen
                    }
                },
                elements: {
                    bar: { // Cambiar el radio del punto a cero
                        borderWidth: 1
                    }
                }
            }
        });
    };

    self.actualizarGrafico = function (offline) {
        if (!offline) {
            $.getJSON(urlGenerarEficienciaHidraulicas, { MaterialId: self.materialSeleccionado, FechaHoraDesde: self.fechaDesdeSeleccionada, FechaHoraHasta: self.fechaHastaSeleccionada }, function (data) {
                self.datasetTotal.data = data.CamionesPorHidraulicaMaterial;
            }).done(function () {
                self.mostrarGrafico();
                $.unblockUI();
            }).fail(function () {
                console.error("Error al actualizar gráfico."); // Manejo de errores
            });
        } else {
            self.mostrarGrafico();
        }
    };

    self.generarGrafico = function () {
        self.materialSeleccionado = $("#materialIdHidraulica").val();
        self.fechaDesdeSeleccionada = $("#FechaDesdeCamiones").val() + " " + $("#FechaDesdeCamiones_time").val();
        self.fechaHastaSeleccionada = $("#FechaHastaCamiones").val() + " " + $("#FechaHastaCamiones_time").val();

        if (!self.materialSeleccionado) {
            self.materialSeleccionado = 0; // Si no se selecciona material, se establece en 0
        }

        BloquearPantalla();
        self.actualizarGrafico();
    };

    self.filtrarHidraulicas = function (checkboxContainerClass) {
        var visibles = [];
        $('.' + checkboxContainerClass + ' input[type="checkbox"]').each(function () {
            if ($(this).is(":checked")) {
                visibles.push($(this).val()); // Obtener los valores de los checkboxes seleccionados
            }
        });

        self.hidraulicas = visibles; // Actualiza la lista de hidraulicas
    };
}

var graficoHidraulica;
$(document).ready(function () {
    // Inicializar checkboxes como seleccionados
    $('.checkboxes input[type="checkbox"]').prop("checked", true);

    var validator = $("#formHidraulica").validate({ /* settings */ });

    graficoHidraulica = new GraficoEficienciaHidraulicaViewModel(validator);
    ko.applyBindings(graficoHidraulica, document.getElementById("graficoeficienciahidraulica"));
    graficoHidraulica.generarGrafico();
    $('.ui-helper-hidden-accessible').hide();

    // Manejo de eventos de cambio en checkboxes
    $('.checkboxes input[type="checkbox"]').on("change", function (e) {
        graficoHidraulica.filtrarHidraulicas("checkboxes");
        graficoHidraulica.mostrarGrafico();
        e.preventDefault();
    });
});

    /*

    Cambios Clave Realizados
        Actualización de Escalas: Cambios en la estructura de yAxes y xAxes a y y x.
        Tooltips: Se modificó la configuración de tooltips para utilizar la nueva API.
        Filtrado y Manejo de Datos: Se utilizó filter y map para una mejor legibilidad y eficiencia en la obtención de datos únicos.
        Manejo de Errores: Se agregó un mensaje de error en la función de actualización en caso de que falle la llamada a la API.
        Estructura de Datos: Se mejoró la forma de calcular los totales y obtener los colores para los gráficos.
            */