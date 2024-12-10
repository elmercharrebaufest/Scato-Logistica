function DataSetChartLine(nombreDeLinea, data, color) {
    this.label = nombreDeLinea;
    this.fill = false; // Mantiene el llenado desactivado
    this.lineTension = 0.1; // Tensión de la línea
    this.backgroundColor = "rgba(75,192,192,0.4)"; // Color de fondo
    this.borderColor = color == null ? '#000000' : color; // Color del borde
    this.borderCapStyle = 'butt'; // Estilo del extremo del borde
    this.borderDash = []; // Patrón de línea discontinua
    this.borderDashOffset = 0.0; // Desplazamiento del patrón de línea discontinua
    this.borderJoinStyle = 'miter'; // Estilo de unión de borde
    this.pointBorderColor = "rgba(75,192,192,1)"; // Color del borde del punto
    //this.pointBackgroundColor = "#fff", // Color de fondo del punto, comentado
    this.pointBorderWidth = 1; // Ancho del borde del punto
    this.pointHoverRadius = 5; // Radio al pasar el ratón por el punto
    this.pointHoverBackgroundColor = "rgba(75,192,192,1)"; // Color de fondo al pasar el ratón
    this.pointHoverBorderColor = "rgba(220,220,220,1)"; // Color del borde al pasar el ratón
    this.pointHoverBorderWidth = 2; // Ancho del borde al pasar el ratón
    this.pointRadius = 1; // Radio del punto
    this.pointHitRadius = 10; // Radio de hit del punto
    this.spanGaps = false; // No conectar los puntos
    this.data = data; // Datos del conjunto
}

function GraficoCamionesViewModel() {
    // Inicializo observers
    var self = this;
    self.datasetHistorico = new DataSetChartLine(textoHistorico, [], '#ff5722'); // Conjunto de datos histórico
    self.datasetActual = new DataSetChartLine(textoActual, [], '#0070ff'); // Conjunto de datos actual
    var myLineChartHoras = null; // Variable para almacenar el gráfico

    self.actualizarGrafico = function () {
        $.getJSON(urlGenerarCamionesHora, null, function (data) {
            self.datasetHistorico.data = data.CamionesHistorico; // Datos históricos
            self.datasetActual.data = data.CamionesActuales; // Datos actuales
        }).done(function () {
            var ctxh = $("#myChartCamiones");

            // Si el gráfico ya existe, actualizarlo
            if (myLineChartHoras != null) {
                myLineChartHoras.update();
            } else {
                // Crear nuevo gráfico
                myLineChartHoras = new Chart(ctxh, {
                    type: 'line', // Tipo de gráfico
                    data: {
                        labels: Array.from({ length: 24 }, (_, i) => i), // Etiquetas de 0 a 23
                        datasets: [self.datasetHistorico, self.datasetActual] // Conjuntos de datos
                    },
                    options: {
                        maintainAspectRatio: false, // Mantener relación de aspecto
                        animation: false, // Desactivar animación
                        plugins: { // Actualizado para usar plugins
                            legend: {
                                display: false // Desactivar leyenda
                            },
                            tooltip: { // Actualizado para la nueva estructura de tooltips
                                callbacks: {
                                    label: tooltipItem => `${tooltipItem.parsed.y}: ${tooltipItem.label}`, // Acceso a datos actualizados
                                    title: () => null // No mostrar título
                                }
                            }
                        },
                        scales: {
                            y: { // Actualizado para la nueva sintaxis de escalas
                                min: 0 // Valor mínimo en el eje Y
                            },
                            x: { // Actualizado para la nueva sintaxis de escalas
                                min: 0 // Valor mínimo en el eje X
                            }
                        }
                    }
                });
            }
            $.unblockUI(); // Desbloquear UI
        }).fail(function () {
            // Manejo de errores
            if (document.getElementById('myFrame') == null) {
                document.getElementById('loginiframe').innerHTML =
                    '<iframe id="myFrame" src="./cupo" style="height:1px;width:100%"></iframe>';
                $("#myFrame").hide();
            }
            document.getElementById('myFrame').onload = function () {
                fetch('./cupo').then(function () { window.location.reload(); });
            }
        });
    }
}

var grafico;
$(document).ready(function () {
    BloquearPantalla(); // Bloquear la pantalla mientras se carga
    grafico = new GraficoCamionesViewModel(); // Inicializar el modelo de gráfico
    grafico.actualizarGrafico(); // Actualizar gráfico al cargar
});

function iFrameCheck() {
    var ttle = $('#ifrm').contents().find('title').text(); // Obtener el título del iframe

    setTimeout(function () {
        if (ttle.indexOf('404 - File or directory not found.') == -1) {
            return false; // Si no hay error 404, salir
        } else {
            $('#iFrameAlert').toggleClass('hidden'); // Mostrar alerta de error
        }
    }, 2000);
}

/*

Cambios Clave Realizados
    Actualización de Escalas: Se cambió la estructura de yAxes y xAxes a y y x respectivamente.
    Tooltips: La configuración de tooltips se actualizó para adaptarse a la nueva API de ChartJS 4.
    Etiquetas de Eje X: Se utilizó Array.from para generar las etiquetas de 0 a 23 de manera más legible.
    Destrucción del Gráfico Anterior: Se mantuvo la lógica para actualizar el gráfico existente, asegurando que se mantenga el estado correcto.
    Manejo de Errores: Se mejoró el manejo de errores al cargar datos, asegurando que se manejen los casos de error adecuadamente.

*/