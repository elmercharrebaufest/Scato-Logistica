/* panelAvanceCamionPorPuesto.js */
(function ($) {
    'use strict';

    var avanzarUrl          = '';
    var resolucionUrl       = '';
    var liberarUrl          = '';
    var $modal              = null;
    var $modalLiberar       = null;
    var $alert              = null;
    var pendingData         = null;
    var pendingLiberarData  = null;
    var paginaActual        = 1;
    var tamanioPaginaActual = 10;

    function init() {
        var $container = $('#gridContainer');
        avanzarUrl     = $container.data('avanzarUrl') || '';
        resolucionUrl  = $container.data('resolucionUrl') || '';
        liberarUrl     = $container.data('liberarUrl') || '';
        $modal         = $('#pac-modal');
        $modalLiberar  = $('#pac-modal-liberar');
        $alert         = $('#pac-alert');

        CargarGrilla();
        bindEvents();
        initSignalR();
    }

    function CargarGrilla() {
        var $container = $('#gridContainer');

        $.ajax({
            url:  $container.data('gridUrl'),
            data: { pagina: paginaActual, tamanioPagina: tamanioPaginaActual },
            type: 'GET',
            beforeSend: function () { $container.block(); },
            success: function (html) {
                $container.unblock();
                $container.html(html);
                bindGridEvents();
            },
            error: function () {
                $container.unblock();
                $container.html('<p style="color:#991b1b;padding:12px;">Error al cargar la grilla.</p>');
            }
        });
    }

    function bindEvents() {
        // Botón Cancelar del modal
        $('#pac-btn-cancelar').on('click', function () {
            cerrarModal();
        });

        // Botón Confirmar del modal
        $('#pac-btn-confirmar').on('click', function () {
            if (pendingData) {
                ejecutarAvance(pendingData);
            }
        });

        // Cerrar modal al hacer clic fuera
        $modal.on('click', function (e) {
            if ($(e.target).is($modal)) {
                cerrarModal();
            }
        });

        // Modal Liberar — Cancelar
        $('#pac-liberar-btn-cancelar').on('click', function () {
            cerrarModalLiberar();
        });

        // Modal Liberar — Confirmar
        $('#pac-liberar-btn-confirmar').on('click', function () {
            if (!pendingLiberarData) return;
            var motivo = $.trim($('#liberar-motivo').val());
            if (!motivo) {
                $('#liberar-motivo-error').show();
                $('#liberar-motivo').css('border-color', '#dc2626').focus();
                return;
            }
            pendingLiberarData.motivoLiberar = motivo;
            ejecutarLiberar(pendingLiberarData);
        });

        // Cerrar modal liberar al hacer clic fuera
        $modalLiberar.on('click', function (e) {
            if ($(e.target).is($modalLiberar)) {
                cerrarModalLiberar();
            }
        });
    }

    function bindGridEvents() {
        // Botones "Editar" en la grilla
        $(document).off('click', '.pac-btn-editar').on('click', '.pac-btn-editar', function () {
            var logId = $(this).data('logId');

            if (resolucionUrl) {
                window.location.href = resolucionUrl + '?logId=' + encodeURIComponent(logId);
            } else {
                pendingData = { logId: logId };
                abrirModal(pendingData);
            }
        });

        // Botones "Eliminar" en la grilla
        $(document).off('click', '.pac-btn-eliminar').on('click', '.pac-btn-eliminar', function () {
            var $btn = $(this);
            pendingLiberarData = {
                logId:  $btn.data('logId'),
                patente: $btn.data('patente') || '—',
                puesto:  $btn.data('puesto')  || '—'
            };
            abrirModalLiberar(pendingLiberarData);
        });

        // Paginación — cambio de tamaño de página
        $(document).off('change', '.pac-page-size-select').on('change', '.pac-page-size-select', function () {
            tamanioPaginaActual = parseInt($(this).val(), 10);
            paginaActual = 1;
            CargarGrilla();
        });

        // Paginación — página anterior
        $(document).off('click', '.pac-btn-prev-page').on('click', '.pac-btn-prev-page', function () {
            if (paginaActual > 1) {
                paginaActual--;
                CargarGrilla();
            }
        });

        // Paginación — página siguiente
        $(document).off('click', '.pac-btn-next-page').on('click', '.pac-btn-next-page', function () {
            var totalPaginas = parseInt($('.pac-pagination-bar').data('totalPaginas'), 10) || 1;
            if (paginaActual < totalPaginas) {
                paginaActual++;
                CargarGrilla();
            }
        });
    }

    function abrirModal(data) {
        $('#modal-patente').text(data.patente || '—');
        $('#modal-workflow').text(data.workflow || '—');
        $('#modal-transportista').text(data.transportista || '—');
        $('#modal-puesto-nombre').text(data.puestoNombre || '—');
        $('#pac-btn-confirmar').prop('disabled', false);
        $modal.addClass('pac-open');
    }

    function cerrarModal() {
        $modal.removeClass('pac-open');
        pendingData = null;
    }

    function abrirModalLiberar(data) {
        $('#liberar-modal-patente').text(data.patente);
        $('#liberar-modal-puesto').text(data.puesto);
        $('#liberar-motivo').val('').css('border-color', '');
        $('#liberar-motivo-error').hide();
        $('#pac-liberar-btn-confirmar').prop('disabled', false).html(
            '<span class="material-icons-round" style="font-size:18px;">delete</span> Liberar'
        );
        $modalLiberar.addClass('pac-open');
    }

    function cerrarModalLiberar() {
        $modalLiberar.removeClass('pac-open');
        $('#liberar-motivo').val('').css('border-color', '');
        $('#liberar-motivo-error').hide();
        pendingLiberarData = null;
    }

    function ejecutarLiberar(data) {
        var $btn = $('#pac-liberar-btn-confirmar');
        $btn.prop('disabled', true).html(
            '<span class="material-icons-round" style="font-size:18px;animation:spin 1s linear infinite;">autorenew</span> Liberando...'
        );

        $.ajax({
            url:  liberarUrl,
            type: 'POST',
            data: {
                logId:          data.logId,
                motivoLiberar:  data.motivoLiberar,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (resp) {
                cerrarModalLiberar();
                if (resp && resp.ok) {
                    mostrarAlerta('success', resp.mensaje || 'El registro fue liberado correctamente.');
                } else {
                    mostrarAlerta('error', (resp && resp.mensaje) ? resp.mensaje : 'Ocurrió un error al liberar el registro.');
                }
                CargarGrilla();
            },
            error: function () {
                cerrarModalLiberar();
                mostrarAlerta('error', 'Error de comunicación con el servidor.');
            }
        });
    }

    function ejecutarAvance(data) {
        var $btn = $('#pac-btn-confirmar');
        $btn.prop('disabled', true).html(
            '<span class="material-icons-round" style="font-size:18px;animation:spin 1s linear infinite;">autorenew</span> Avanzando...'
        );

        $.ajax({
            url:  avanzarUrl,
            type: 'POST',
            data: {
                workflowInstanceId: data.workflowInstanceId,
                puestoDeTrabajoId:  data.puestoDeTrabajoId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (resp) {
                cerrarModal();
                if (resp && resp.ok) {
                    mostrarAlerta('success', resp.mensaje || 'El camión fue avanzado correctamente.');
                } else {
                    mostrarAlerta('error', (resp && resp.mensaje) ? resp.mensaje : 'Ocurrió un error al avanzar el camión.');
                }
                CargarGrilla();
            },
            error: function () {
                cerrarModal();
                mostrarAlerta('error', 'Error de comunicación con el servidor.');
            }
        });
    }

    function mostrarAlerta(tipo, mensaje) {
        var icon = tipo === 'success' ? 'check_circle' : 'error';
        $alert
            .removeClass('pac-alert-success pac-alert-error')
            .addClass('pac-alert pac-alert-' + tipo)
            .html('<span class="material-icons-round">' + icon + '</span> ' + mensaje)
            .show();

        setTimeout(function () {
            $alert.fadeOut(400, function () { $alert.removeClass('pac-alert'); });
        }, 5000);
    }

    // ---------------------------------------------------------------------------
    // Alerta sonora — generada con Web Audio API (sin archivos externos)
    // ---------------------------------------------------------------------------
    function reproducirAlerta() {
        try {
            var AudioCtx = window.AudioContext || window.webkitAudioContext;
            if (!AudioCtx) return;

            var ctx = new AudioCtx();

            function beep(frecuencia, inicio, duracion) {
                var osc  = ctx.createOscillator();
                var gain = ctx.createGain();
                osc.connect(gain);
                gain.connect(ctx.destination);
                osc.type            = 'sine';
                osc.frequency.value = frecuencia;
                gain.gain.setValueAtTime(0.6, ctx.currentTime + inicio);
                gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + inicio + duracion);
                osc.start(ctx.currentTime + inicio);
                osc.stop(ctx.currentTime  + inicio + duracion);
            }

            // Doble beep ascendente: 880 Hz → 1100 Hz
            beep(880,  0.00, 0.18);
            beep(1100, 0.22, 0.18);
        } catch (e) {
            // Navegador no soporta Web Audio API; se ignora silenciosamente
        }
    }

    // ---------------------------------------------------------------------------
    // SignalR — suscripción al hub panelAvanceCamion
    // ---------------------------------------------------------------------------
    function initSignalR() {
        if (!$.connection || !$.connection.panelAvanceCamion) return;

        var hub        = $.connection.panelAvanceCamion;
        var puestoId    = parseInt($('#gridContainer').data('puesto'), 10) || 0;

        // Nueva contingencia: sonido + refresco de grilla desde página 1
        hub.client.nuevaContingencia = function () {
            reproducirAlerta();
            paginaActual = 1;
            CargarGrilla();
        };

        // Un operador abrió la pantalla de resolución: solo refresco (el registro desaparece del grid)
        hub.client.contingenciaEditando = function () {
            CargarGrilla();
        };

        // Un operador liberó la contingencia (Atendido→0): sonido + refresco (el registro vuelve al grid)
        hub.client.contingenciaLiberada = function () {
            reproducirAlerta();
            CargarGrilla();
        };

        $.connection.hub.start()
            .done(function () {
                if (puestoId > 0) {
                    hub.server.suscribirseAlPuesto(puestoId);
                }
            })
            .fail(function (err) {
                // Conexión SignalR fallida; la grilla seguirá funcionando sin push
            });
    }

    $(document).ready(function () {
        init();
    });

}(jQuery));
