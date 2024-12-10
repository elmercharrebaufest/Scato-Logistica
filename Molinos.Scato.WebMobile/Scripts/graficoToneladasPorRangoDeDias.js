function DataSetChartLine(nombreDeLinea, data, color) {
    this.label = nombreDeLinea;
    this.fill = false; // Se mantiene el llenado desactivado
    this.lineTension = 0.1; // Tensión de la línea
    this.backgroundColor = "rgba(75,192,192,0.4)"; // Color de fondo
    this.borderColor = color === null ? '#000000' : color; // Cambiado el color del borde según el parámetro color
    this.borderCapStyle = 'butt'; // Estilo de la capucha del borde
    this.borderDash = []; // Patrón de línea discontinua
    this.borderDashOffset = 0.0; // Desplazamiento del patrón de línea discontinua
    this.borderJoinStyle = 'miter'; // Estilo de unión de borde
    this.pointBorderColor = "rgba(75,192,192,1)"; // Color del borde del punto
    // this.pointBackgroundColor = "#fff", // Color de fondo del punto
    this.pointBorderWidth = 1; // Ancho del borde del punto
    this.pointHoverRadius = 5; // Radio del punto al pasar el mouse
    this.pointHoverBackgroundColor = "rgba(75,192,192,1)"; // Color de fondo al pasar el mouse
    this.pointHoverBorderColor = "rgba(220,220,220,1)"; // Color del borde al pasar el mouse
    this.pointHoverBorderWidth = 2; // Ancho del borde al pasar el mouse
    this.pointRadius = 1; // Radio del punto
    this.pointHitRadius = 10; // Radio del punto al hacer clic
    this.spanGaps = false; // Si se deben conectar los puntos de datos nulos
    this.data = data; // Datos del conjunto
}

var colores = ["#ff0000", "#8500ff", "#0400ff", "#1bff00", "#ccff00", "#ff0081", "#ff5e00", "#00ffff", "#000000", "#40bf96"];

function GraficoToneladasPorRangoDeDiasViewModel() {
    // Inicializo observers
    var self = this;
    var intervalo = 60;
    var multiplicador = 1000;
    self.fechaSeleccionada = null;
    self.materialSeleccionado = null;
    var myLineChartHoras = null; // Inicializa el gráfico como null
    var graficoIniciado = false;

    self.actualizarGrafico = function (funcionRecursiva) {
        graficoIniciado = true;
        $.getJSON(urlGenerarToneladasPorRangoDeDia, { MaterialId: self.materialSeleccionado, FechaDesde: self.fechaDesde, FechaHasta: self.fechaHasta }, function (data) {
            datosToneladas = data.Toneladas;
        }).done(function () {
            var ctxh = $("#myChartToneladas");
            var labels = [...new Set(datosToneladas.map(n => n.Fecha))]; // Usando Set para obtener etiquetas únicas

            var codigoUnico = [...new Set(datosToneladas.map(item => item.Codigo))].filter(codigo => codigo !== null); // Obteniendo códigos únicos

            var dataSets = [];
            if (codigoUnico.length !== 0) {
                codigoUnico.forEach(function (cod, index) {
                    var dataSet = labels.map(fecha => {
                        var matchingData = datosToneladas.find(e => e.Codigo === cod && e.Fecha === fecha);
                        return matchingData ? matchingData.Toneladas : 0; // Si hay coincidencia, obtiene las toneladas, de lo contrario, 0
                    });
                    dataSets.push(new DataSetChartLine(cod, dataSet, colores[index]));
                });
            } else {
                // Si no hay datos, crea un conjunto de datos de "Sin Datos"
                var dataSet = new Array(labels.length).fill(0);
                dataSets.push(new DataSetChartLine("Sin Datos", dataSet, '#9be334'));
            }

            if (myLineChartHoras) {
                myLineChartHoras.destroy(); // Destruye el gráfico anterior
            }

            myLineChartHoras = new Chart(ctxh, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: dataSets
                },
                options: {
                    animation: false,
                    plugins: { // Cambiado de `tooltips` a `plugins.tooltip` en ChartJS 4
                        tooltip: {
                            callbacks: {
                                label: function (tooltipItem) {
                                    return codigoUnico[tooltipItem.datasetIndex] + ": " + tooltipItem.formattedValue + " Tn."; // Cambiado y simplificado el acceso a valores de tooltip
                                }
                            }
                        }
                    },
                    scales: {
                        y: { // Actualizado para la nueva sintaxis de escalas
                            beginAtZero: true // Asegura que el eje Y comience en 0
                        },
                        x: { // Actualizado para la nueva sintaxis de escalas
                            ticks: {
                                min: 0 // Ajustes mínimos para el eje X
                            }
                        }
                    }
                }
            });

            // Re-implementación opcional para actualizar recursivamente
            // if (funcionRecursiva !== null) {
            //     setTimeout(funcionRecursiva, intervalo * multiplicador);
            // }
        }).fail(function () {
            console.error("Error al obtener datos."); // Manejo de errores en la llamada a la API
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