var dashboardCardless = (function () {

    var links = null;
    var charts = {};
    var galeriaPaginaActual = 1;
    var galeriaUrlDescarga = null;

    // ─── helpers ────────────────────────────────────────────────────────────────

    function getFiltroGlobal() {
        return {
            PuestoDeTrabajoId: $('#filtro-puesto').val() || null,
            FechaDesde: $('#filtro-fecha-desde').val(),
            FechaHasta: $('#filtro-fecha-hasta').val()
        };
    }

    function getFiltroGaleria() {
        return {
            FechaDesde: $('#galeria-fecha-desde').val() || $('#filtro-fecha-desde').val(),
            FechaHasta: $('#galeria-fecha-hasta').val() || $('#filtro-fecha-hasta').val()
        };
    }

    function destroyChart(id) {
        if (charts[id]) {
            charts[id].destroy();
            delete charts[id];
        }
    }

    function formatDate(isoString) {
        if (!isoString) return '';
        var d = new Date(parseInt(isoString.replace('/Date(', '').replace(')/', '')));
        return d.getDate() + '/' + (d.getMonth() + 1) + '/' + d.getFullYear();
    }

    // ─── Indicador 1: Camiones por día ─────────────────────────────────────────

    function cargarCamionesPorDia() {
        $.post(links.camiones, getFiltroGlobal(), function (data) {
            destroyChart('camiones');
            if (!data || data.length === 0) {
                $('#chart-camiones-por-dia').hide();
                $('#camiones-sin-datos').show();
                return;
            }
            $('#camiones-sin-datos').hide();
            $('#chart-camiones-por-dia').show();

            var labels = data.map(function (d) { return formatDate(d.Fecha); });
            var cantidades = data.map(function (d) { return d.CantidadDiaria; });
            var acumulados = data.map(function (d) { return d.Acumulado; });

            var ctx = document.getElementById('chart-camiones-por-dia').getContext('2d');
            charts['camiones'] = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Camiones por día',
                            data: cantidades,
                            backgroundColor: 'rgba(54, 162, 235, 0.6)',
                            borderColor: 'rgba(54, 162, 235, 1)',
                            borderWidth: 1
                        },
                        {
                            label: 'Acumulado',
                            data: acumulados,
                            type: 'line',
                            fill: false,
                            borderColor: 'rgba(255, 99, 132, 1)',
                            borderWidth: 2,
                            pointRadius: 3
                        }
                    ]
                },
                options: { responsive: true, scales: { yAxes: [{ ticks: { beginAtZero: true } }] } }
            });
        });
    }

    // ─── Indicador 2: Patente por cámara ───────────────────────────────────────

    function cargarPatentePorCamara() {
        $.post(links.camara, getFiltroGlobal(), function (data) {
            destroyChart('camara');
            if (!data || data.length === 0) {
                $('#chart-patente-por-camara').hide();
                $('#camara-sin-datos').show();
                return;
            }
            $('#camara-sin-datos').hide();
            $('#chart-patente-por-camara').show();

            var labels = data.map(function (d) { return d.CodigoCamara; });
            var valores = data.map(function (d) { return d.Porcentaje; });
            var colors = labels.map(function (_, i) {
                var palette = ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'];
                return palette[i % palette.length];
            });

            var ctx = document.getElementById('chart-patente-por-camara').getContext('2d');
            charts['camara'] = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: labels,
                    datasets: [{ data: valores, backgroundColor: colors }]
                },
                options: {
                    responsive: true,
                    tooltips: {
                        intersect: false,
                        callbacks: {
                            label: function (tooltipItem, chartData) {
                                var label = chartData.labels[tooltipItem.index] || '';
                                var value = chartData.datasets[0].data[tooltipItem.index];
                                return label + ': ' + value + '%';
                            }
                        }
                    }
                }
            });
        });
    }

    // ─── Indicador 3: Reconocimiento por día de semana ─────────────────────────

    function cargarReconocimientoPorDiaSemana() {
        $.post(links.diasemana, getFiltroGlobal(), function (data) {
            destroyChart('diasemana');
            if (!data || data.length === 0) {
                $('#chart-reconocimiento-diasemana').hide();
                $('#diasemana-sin-datos').show();
                return;
            }
            $('#diasemana-sin-datos').hide();
            $('#chart-reconocimiento-diasemana').show();

            var labels = data.map(function (d) { return formatDate(d.Fecha); });
            var ctx = document.getElementById('chart-reconocimiento-diasemana').getContext('2d');
            charts['diasemana'] = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Reconocidos',
                            data: data.map(function (d) { return d.Reconocidos; }),
                            backgroundColor: 'rgba(75, 192, 75, 0.7)'
                        },
                        {
                            label: 'No Reconocidos',
                            data: data.map(function (d) { return d.NoReconocidos; }),
                            backgroundColor: 'rgba(255, 99, 132, 0.7)'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    tooltips: { mode: 'index', intersect: false },
                    scales: { xAxes: [{ stacked: false }], yAxes: [{ ticks: { beginAtZero: true } }] }
                }
            });
        });
    }

    // ─── Indicador 4: Vehículo presente vs no presente ─────────────────────────

    function cargarVehiculoPorDia() {
        $.post(links.vehiculo, getFiltroGlobal(), function (data) {
            destroyChart('vehiculo');
            if (!data || data.length === 0) {
                $('#chart-vehiculo-por-dia').hide();
                $('#vehiculo-sin-datos').show();
                return;
            }
            $('#vehiculo-sin-datos').hide();
            $('#chart-vehiculo-por-dia').show();

            var labels = data.map(function (d) { return formatDate(d.Fecha); });
            var ctx = document.getElementById('chart-vehiculo-por-dia').getContext('2d');
            charts['vehiculo'] = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Presente',
                            data: data.map(function (d) { return d.Presente; }),
                            backgroundColor: 'rgba(75, 192, 75, 0.7)'
                        },
                        {
                            label: 'No Presente',
                            data: data.map(function (d) { return d.NoPresente; }),
                            backgroundColor: 'rgba(200, 200, 200, 0.7)'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    tooltips: { mode: 'index', intersect: false },
                    scales: { xAxes: [{ stacked: false }], yAxes: [{ ticks: { beginAtZero: true } }] }
                }
            });
        });
    }

    // ─── Indicador 5: Reconocimiento por proveedor ─────────────────────────────

    function cargarReconocimientoPorProveedor() {
        $.post(links.proveedor, getFiltroGlobal(), function (data) {
            destroyChart('proveedor');
            if (!data || data.length === 0) {
                $('#chart-reconocimiento-proveedor').hide();
                $('#proveedor-sin-datos').show();
                return;
            }
            $('#proveedor-sin-datos').hide();
            $('#chart-reconocimiento-proveedor').show();

            var labels = data.map(function (d) { return d.ProveedorALPR; });
            var ctx = document.getElementById('chart-reconocimiento-proveedor').getContext('2d');
            charts['proveedor'] = new Chart(ctx, {
                type: 'horizontalBar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Tasa de Reconocimiento (%)',
                        data: data.map(function (d) { return d.TasaReconocimiento; }),
                        backgroundColor: 'rgba(54, 162, 235, 0.7)'
                    }]
                },
                options: {
                    responsive: true,
                    tooltips: { intersect: false },
                    scales: { xAxes: [{ ticks: { beginAtZero: true, max: 100 } }] }
                }
            });
        });
    }

    // ─── Indicador 6: Promedio de intentos ─────────────────────────────────────

    function cargarPromedioIntentos() {
        $.post(links.promedio, getFiltroGlobal(), function (data) {
            destroyChart('promedio');

            // KPIs
            if (data.resumen) {
                $('#kpi-promedio-general').text(data.resumen.PromedioGeneral ? data.resumen.PromedioGeneral.toFixed(1) : '—');
                $('#kpi-max-hoy').text(data.resumen.MaxHoy || '—');
            }

            var barras = data.barras || [];
            if (barras.length === 0) {
                $('#chart-promedio-intentos').hide();
                $('#promedio-sin-datos').show();
                return;
            }
            $('#promedio-sin-datos').hide();
            $('#chart-promedio-intentos').show();

            var labels = barras.map(function (d) { return formatDate(d.Fecha); });
            var ctx = document.getElementById('chart-promedio-intentos').getContext('2d');
            charts['promedio'] = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Promedio de intentos',
                        data: barras.map(function (d) { return d.PromedioIntentos; }),
                        backgroundColor: 'rgba(153, 102, 255, 0.7)'
                    }]
                },
                options: { responsive: true, scales: { yAxes: [{ ticks: { beginAtZero: true } }] } }
            });
        });
    }

    // ─── Galería de capturas fallidas ──────────────────────────────────────────

    function cargarGaleria(pagina) {
        galeriaPaginaActual = pagina || 1;
        var filtro = getFiltroGaleria();
        filtro.pagina = galeriaPaginaActual;

        $.post(links.galeria, filtro, function (data) {
            var $inner = $('#galeria-grid-inner');
            $inner.empty();

            if (!data || data.length === 0) {
                $('#galeria-sin-datos').show();
                $('#galeria-paginacion').empty();
                return;
            }
            $('#galeria-sin-datos').hide();

            $.each(data, function (i, item) {
                var srcUrl = links.imagen + '?id=' + item.DetalleId;
                var caption = item.CodigoCamara + ' — ' + formatDate(item.FechaEvento);
                var $card = $('<div class="galeria-card galeria-item"></div>');
                var $wrap = $('<div class="galeria-card-img-wrap"></div>');
                var $img = $('<img />').attr('src', srcUrl).attr('alt', caption);
                $img.on('error', function () { $(this).attr('src', links.imgFallback); });
                var $footer = $('<div class="galeria-card-footer"></div>').text(caption);
                $card.on('click', function () {
                    $('#galeria-lb-img').attr('src', srcUrl);
                    $('#galeria-lb-caption').text(caption);
                    $('#galeria-lightbox').addClass('open');
                });
                $wrap.append($img);
                $card.append($wrap).append($footer);
                $inner.append($card);
            });

            var total = data[0].TotalRegistros;
            var pageSize = 15;
            var totalPaginas = Math.ceil(total / pageSize);
            renderPaginacion(totalPaginas, galeriaPaginaActual);
        });
    }

    function renderPaginacion(totalPaginas, paginaActual) {
        var $pag = $('#galeria-paginacion');
        $pag.empty();
        if (totalPaginas <= 1) return;

        var $wrap = $('<div class="galeria-pagination"></div>');

        var $prev = $('<button>&laquo;</button>').prop('disabled', paginaActual === 1);
        $prev.on('click', function () { if (paginaActual > 1) cargarGaleria(paginaActual - 1); });
        $wrap.append($prev);

        var start = Math.max(1, paginaActual - 3);
        var end = Math.min(totalPaginas, start + 6);
        start = Math.max(1, end - 6);

        for (var p = start; p <= end; p++) {
            (function (pg) {
                var $btn = $('<button></button>').text(pg);
                if (pg === paginaActual) $btn.addClass('active');
                $btn.on('click', function () { cargarGaleria(pg); });
                $wrap.append($btn);
            })(p);
        }

        var $next = $('<button>&raquo;</button>').prop('disabled', paginaActual === totalPaginas);
        $next.on('click', function () { if (paginaActual < totalPaginas) cargarGaleria(paginaActual + 1); });
        $wrap.append($next);

        $pag.append($wrap);
    }

    // ─── Init ──────────────────────────────────────────────────────────────────

    function cargarTodos() {
        cargarCamionesPorDia();
        cargarPatentePorCamara();
        cargarReconocimientoPorDiaSemana();
        cargarVehiculoPorDia();
        cargarReconocimientoPorProveedor();
        cargarPromedioIntentos();
        cargarGaleria(1);
    }

    function init() {
        links = {
            camiones: $('#dashboard-cardless-links').data('url-camiones'),
            camara: $('#dashboard-cardless-links').data('url-camara'),
            diasemana: $('#dashboard-cardless-links').data('url-diasemana'),
            vehiculo: $('#dashboard-cardless-links').data('url-vehiculo'),
            proveedor: $('#dashboard-cardless-links').data('url-proveedor'),
            promedio: $('#dashboard-cardless-links').data('url-promedio'),
            galeria: $('#dashboard-cardless-links').data('url-galeria'),
            descarga: $('#dashboard-cardless-links').data('url-descarga'),
            imagen: $('#dashboard-cardless-links').data('url-imagen'),
            imgFallback: $('#dashboard-cardless-links').data('url-img-fallback')
        };

        galeriaUrlDescarga = links.descarga;

        // Inicializar datepickers de la galería con los mismos valores del filtro global
        var hoy = $('#filtro-fecha-hasta').val();
        var hace7 = $('#filtro-fecha-desde').val();
        $('#galeria-fecha-desde').val(hace7);
        $('#galeria-fecha-hasta').val(hoy);

        // Botón filtrar global
        $('#btn-filtrar').on('click', function () {
            cargarTodos();
        });

        // Botón buscar galería
        $('#btn-buscar-galeria').on('click', function () {
            cargarGaleria(1);
        });

        // Botón descargar galería
        $('#btn-descargar-galeria').on('click', function () {
            var filtro = getFiltroGaleria();
            var form = $('<form method="POST" action="' + galeriaUrlDescarga + '"></form>');
            form.append($('<input type="hidden" name="FechaDesde">').val(filtro.FechaDesde));
            form.append($('<input type="hidden" name="FechaHasta">').val(filtro.FechaHasta));
            $('body').append(form);
            form.submit();
            form.remove();
        });

        // Lightbox
        $('#galeria-lb-close').on('click', function () {
            $('#galeria-lightbox').removeClass('open');
        });
        $('#galeria-lightbox').on('click', function (e) {
            if ($(e.target).is('#galeria-lightbox')) {
                $(this).removeClass('open');
            }
        });

        cargarTodos();
    }

    return { init: init };

})();
