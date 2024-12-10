// Definición de DataSetChartLine para crear conjuntos de datos
function DataSetChartLine(nombreDeLinea, data, color) {
    this.label = nombreDeLinea,
        this.fill = false,  // Se mantiene en false para líneas
        this.borderDash = [], // La configuración de estilo de línea se mantiene vacía
        this.borderDashOffset = 0.0, // Sin desplazamiento
        this.borderJoinStyle = 'miter', // Estilo de unión de bordes
        this.borderWidth = 2, // Ancho del borde
        this.data = data; // Los datos que se le pasan
    if (color === null) {
        this.backgroundColor = '#000000'; // Color de fondo por defecto
        this.hoverBackgroundColor = '#000000'; // Color de fondo al pasar el mouse
    } else {
        this.backgroundColor = color; // Color de fondo especificado
        this.hoverBackgroundColor = color; // Color de fondo al pasar el mouse
    }
}

// Vista del gráfico de toneladas por día
function GraficoToneladasPorDiaViewModel() {
    var self = this; // Mantenemos el contexto del objeto
    var graficoIniciado = false;

    // Inicialización de variables
    self.datasetTotal = new DataSetChartLine("", [], null);
    self.fechaSeleccionada = null;
    self.materialSeleccionado = null;
    self.myLineChartHidraulicas = null;
    self.hidraulicas = null;

    // Función para mostrar el gráfico
    self.mostrarGrafico = function () {
        var ctxh = $("#myChartToneladasPorDia");
        var labels = [];
        var data = [];

        // Filtramos los datos de las hidráulicas
        if (self.hidraulicas) {
            data = $.grep(self.datasetTotal.data, function (e) { return $.inArray(e.Clave, self.hidraulicas) >= 0 });
        } else {
            data = self.datasetTotal.data;
        }

        // Obtener etiquetas únicas
        $.each(data, function (i, n) {
            if ($.inArray(n.Clave, labels) < 0)
                labels.push(n.Clave);
        });

        // Obtener materiales únicos
        var materiales = [];
        $.each(data, function (i, n) {
            if (n.Material && $.inArray(n.Material, materiales) < 0)
                materiales.push(n.Material);
        });

        // Calcular totales por material
        var totals = [];
        $.each(labels, function (i, l) {
            var materialesHidraulica = $.grep(data, function (e) { return e.Clave == l });
            var totalHidraulica = 0;
            $.each(materialesHidraulica, function (i2, m) { totalHidraulica += m.Toneladas });
            totals.push(totalHidraulica);
        });

        // Obtener colores para los gráficos
        var coloresMateriales = obtenerColoresParaGraficos(materiales.length);

        // Configuración de los datasets
        var datasets = $.map(materiales, function (m, i) {
            var color = coloresMateriales[i];
            return {
                label: m,
                data: $.map(labels, function (l) {
                    var h = $.grep(data, function (e) { return e.Clave == l && e.Material == m });
                    return h[0] ? h[0].Toneladas : 0; // Si no hay datos, devuelve 0
                }),
                backgroundColor: color, // Color de fondo
                borderColor: color, // Color del borde
                borderWidth: 1 // Ancho del borde
            };
        });

        // Destruir gráfico anterior si existe
        if (self.myLineChartHidraulicas) {
            self.myLineChartHidraulicas.destroy();
        }

        // Crear nuevo gráfico
        self.myLineChartHidraulicas = new Chart(ctxh, {
            type: 'bar', // Tipo de gráfico
            data: {
                labels: labels, // Etiquetas del eje x
                datasets: datasets // Conjuntos de datos
            },
            options: {
                animation: false, // Desactivar animación
                plugins: {
                    legend: {
                        display: true // Mostrar leyenda
                    },
                    tooltip: { // Configuración de tooltip
                        callbacks: {
                            label: function (tooltipItem) {
                                return [materiales[tooltipItem.datasetIndex] + ': ' + Number(tooltipItem.raw).toString()]; // Mostrar material y toneladas
                            },
                            title: function (tooltipItem) {
                                return [tooltipItem[0].label + " (Total: " + totals[tooltipItem[0].dataIndex] + ")"]; // Título del tooltip
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        min: 0, // Valor mínimo del eje y
                        stacked: true // Ejes apilados
                    },
                    x: {
                        stacked: true // Ejes apilados
                    }
                },
                elements: {
                    point: { radius: 0 } // Ocultar puntos
                },
                // Título del gráfico
                responsive: true, // Gráfico responsivo
                maintainAspectRatio: false // Mantener proporción de aspecto
            }
        });
    }

    // Función para actualizar el gráfico
    self.actualizarGrafico = function (funcionRecursiva, offline) {
        graficoIniciado = true;
        if (!offline) {
            $.getJSON(urlGenerarToneladasPorDias, { MaterialId: self.materialSeleccionado, FechaHoraDesde: self.fechaDesdeSeleccionada, FechaHoraHasta: self.fechaHastaSeleccionada }, function (data) {
                self.datasetTotal.data = data.CamionesPorHidraulicaMaterial;
            }).done(function () {
                self.mostrarGrafico(); // Mostrar gráfico después de la carga de datos
            }).fail(function () {
                // Manejar error de carga
            });
        } else {
            self.mostrarGrafico(); // Mostrar gráfico si está en offline
        }
    }

    // Función recursiva para actualizar el gráfico
    function actualizarGraficoRecursivo() {
        self.actualizarGrafico(actualizarGraficoRecursivo);
    }

    // Generar gráfico de toneladas por día
    self.generarGraficoToneldasPorDia = function () {
        self.materialSeleccionado = $("#materialIdToneladasPorDia").val();
        self.fechaDesdeSeleccionada = $("#FechaDesdeToneladas").val() + " " + $("#FechaDesdeToneladas_time").val();
        self.fechaHastaSeleccionada = $("#FechaHastaToneladas").val() + " " + $("#FechaHastaToneladas_time").val();

        if (!graficoIniciado) {
            actualizarGraficoRecursivo(); // Iniciar gráfico
        } else {
            self.actualizarGrafico(); // Actualizar gráfico existente
        }
    }

    // Filtrar hidráulicas seleccionadas
    self.filtrarHidraulicas = function (checkboxContainerClass) {
        var visibles = [];
        $.each($('.' + checkboxContainerClass + ' input[type = "checkbox"]'), function (i, chk) {
            if ($(chk).is(":checked"))
                visibles.push($(chk).val());
        });

        self.hidraulicas = visibles; // Almacenar hidráulicas seleccionadas
    }
}

// Inicialización del gráfico al cargar el documento
var graficoToneladasPorDia;
$(document).ready(function () {
    $.each($('.checkboxesToneladas input[type="checkbox"]'), function (i, chk) {
        $(chk).prop("checked", true); // Marcar todas las casillas
    });

    graficoToneladasPorDia = new GraficoToneladasPorDiaViewModel();
    ko.applyBindings(graficoToneladasPorDia, document.getElementById("graficoToneladasPorDia"));
    graficoToneladasPorDia.filtrarHidraulicas("checkboxesToneladas");
    graficoToneladasPorDia.generarGraficoToneldasPorDia();

    // Manejar cambios en las casillas de verificación
    $('.checkboxesToneladas input[type="checkbox"]').on("change", function (e) {
        graficoToneladasPorDia.filtrarHidraulicas("checkboxesToneladas");
        graficoToneladasPorDia.mostrarGrafico();
        e.preventDefault(); // Prevenir comportamiento por defecto
    });
});