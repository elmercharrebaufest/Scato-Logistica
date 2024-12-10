function DataSetChartLine(nombreDeLinea, data, color) {
    this.label = nombreDeLinea;
    this.fill = false; // Mantiene el llenado desactivado
    this.borderDash = []; // Patrón de línea discontinua
    this.borderDashOffset = 0.0; // Desplazamiento del patrón de línea discontinua
    this.borderJoinStyle = 'miter'; // Estilo de unión de borde
    this.borderWidth = 2; // Ancho del borde
    this.data = data; // Datos del conjunto
    // Establecer color de fondo y hover
    this.backgroundColor = color == null ? '#000000' : color;
    this.hoverBackgroundColor = color == null ? '#000000' : color;
}

function GraficoCamionesPorSectorViewModel() {
    // Inicializo observers
    var self = this;
    self.datasetTotal = new DataSetChartLine("", [], null);
    var myLineChartCamionesPorSector = null;

    self.actualizarGrafico = function () {
        $.getJSON(urlGenerarCamionesPorSector, function (data) {
            self.datasetTotal.data = data.CantidadEnSector; // Obtener datos
        }).done(function () {
            var ctxh = $("#myChartCamionesPorSector");
            var labels = self.datasetTotal.data.map(n => n.NombreSector); // Obtener etiquetas
            var valores = self.datasetTotal.data.map(n => n.CantidadCamiones); // Obtener valores

            var valorMaximo = 120; // Valor máximo para el eje Y

            // Destruir gráfico anterior si existe
            if (myLineChartCamionesPorSector) {
                myLineChartCamionesPorSector.destroy();
            }

            // Crear nuevo gráfico
            myLineChartCamionesPorSector = new Chart(ctxh, {
                type: 'bar', // Tipo de gráfico
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Porcentaje de Ocupación', // Etiqueta del conjunto de datos
                            data: valores,
                            backgroundColor: '#28a745', // Color de fondo
                            borderColor: '#28a745', // Color del borde
                            borderWidth: 1 // Ancho del borde
                        }
                    ]
                },
                options: {
                    // responsive: true, // Comentado para mantener tamaño fijo
                    maintainAspectRatio: false, // Mantener la relación de aspecto
                    animation: false, // Desactivar animación
                    plugins: { // Actualizado para usar plugins
                        legend: {
                            display: false // Desactivar leyenda
                        },
                        tooltip: { // Cambiado para la nueva estructura de tooltips
                            callbacks: {
                                label: tooltipItem => `${tooltipItem.parsed.y}: ${tooltipItem.label}`, // Acceso a datos actualizados
                                title: () => null // No mostrar título
                            }
                        }
                    },
                    scales: {
                        y: { // Actualizado para la nueva sintaxis de escalas
                            min: 0, // Valor mínimo en el eje Y
                            max: valorMaximo // Valor máximo en el eje Y
                        },
                        x: { // Actualizado para la nueva sintaxis de escalas
                            min: 0 // Valor mínimo en el eje X
                        }
                    },
                    elements: {
                        bar: { // Elementos del gráfico
                            borderWidth: 1 // Ancho del borde de las barras
                        }
                    }
                }
            });
            $.unblockUI(); // Desbloquear UI
        }).fail(function () {
            console.error("Error al actualizar gráfico."); // Manejo de errores
        });
    };

    self.generarGrafico = function () {
        BloquearPantalla(); // Bloquear la pantalla mientras se carga
        self.actualizarGrafico(); // Actualizar gráfico
    };
}

var graficoCamionesPorSector;
$(document).ready(function () {
    graficoCamionesPorSector = new GraficoCamionesPorSectorViewModel();
    //ko.applyBindings(graficoCamionesPorSector, document.getElementById("graficoeficienciahidraulica")); // Binding comentado
    graficoCamionesPorSector.generarGrafico(); // Generar gráfico
    $('.ui-helper-hidden-accessible').hide(); // Ocultar elementos de UI no accesibles
});

/*

Cambios Clave Realizados
    Actualización de Escalas: Cambié la estructura de yAxes y xAxes a y y x.
    Tooltips: Se ajustó la configuración de tooltips para utilizar la nueva API de ChartJS 4.
    Filtrado y Manejo de Datos: Usé map para obtener etiquetas y valores, mejorando la legibilidad.
    Destrucción del Gráfico Anterior: Se aseguró que el gráfico existente se destruya antes de crear uno nuevo.
    Manejo de Errores: Se agregó un mensaje de error en la función de actualización en caso de que falle la llamada a la API.

*/