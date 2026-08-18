(function () {
    'use strict';

    const config = window.ltaV2Config || {};
    const container = document.getElementById('lta-v2-container');
    const PATENTE_REGEX = /^[A-Za-z]{3}\d{3}$|^[A-Za-z]{2}\d{3}[A-Za-z]{2}$/;

    function readPuestosFromDom() {
        return Array.from(container.querySelectorAll('.lta-v2__card')).map(card => ({
            id: parseInt(card.dataset.puestoId, 10),
            colaId: parseInt(card.dataset.colaId, 10) || 0,
            patente: card.dataset.patente || null,
            reconocimientoExitoso: card.dataset.reconocimientoExitoso === 'true',
            formatoInvalido: false,
            mensajeError: card.dataset.mensajeError || null,
            imagenBase64: card.dataset.imagenBase64 || null,
            cargando: false
        }));
    }

    const puestos = readPuestosFromDom();

    function getCard(puestoId) {
        return container ? container.querySelector(`.lta-v2__card[data-puesto-id="${puestoId}"]`) : null;
    }

    function getPuesto(puestoId) {
        return puestos.find(p => p.id === puestoId);
    }

    function actualizarCard(puesto) {
        const card = getCard(puesto.id);

        if (puesto.colaId > 0) card.dataset.colaId = puesto.colaId;
        card.dataset.patente = puesto.patente || '';
        card.dataset.reconocimientoExitoso = puesto.reconocimientoExitoso ? 'true' : 'false';
        card.dataset.mensajeError = puesto.mensajeError || '';
        card.dataset.imagenBase64 = puesto.imagenBase64 || '';

        const alertBox = card.querySelector('[data-alert-mensaje-error]');
        const alertTexto = card.querySelector('[data-alert-texto]');
        if (puesto.mensajeError) {
            alertTexto.textContent = puesto.mensajeError;
            alertBox.style.display = '';
        } else {
            alertBox.style.display = 'none';
        }

        const input = card.querySelector('[data-input-patente]');
        const tieneColaId = puesto.colaId > 0;
        input.value = puesto.patente || '';
        input.disabled = !tieneColaId;
        input.classList.toggle('lta-v2__input--error', tieneColaId && !puesto.reconocimientoExitoso);

        card.querySelectorAll('[data-action="aceptar"]').forEach(btn => {
            btn.disabled = !(tieneColaId && puesto.reconocimientoExitoso) || puesto.cargando;
        });
        card.querySelectorAll('[data-action="omitir"]').forEach(btn => {
            btn.disabled = !tieneColaId || puesto.cargando;
        });

        const mensajeEstado = card.querySelector('[data-mensaje-estado]');
        mensajeEstado.classList.remove('lta-v2__message--success', 'lta-v2__message--error');
        if (!tieneColaId) {
            mensajeEstado.innerHTML = '';
        } else if (puesto.formatoInvalido) {
            mensajeEstado.textContent = 'Formato de patente inválido. Use AAA000 o AA000AA';
            mensajeEstado.classList.add('lta-v2__message--error');
        } else if (puesto.reconocimientoExitoso) {
            mensajeEstado.textContent = 'Patente validada con éxito';
            mensajeEstado.classList.add('lta-v2__message--success');
        } else {
            mensajeEstado.textContent = 'Patente sin circuito activo';
            mensajeEstado.classList.add('lta-v2__message--error');
        }

        const imagen = card.querySelector('[data-imagen]');
        const placeholder = card.querySelector('[data-imagen-placeholder]');
        if (imagen && placeholder) {
            if (puesto.imagenBase64) {
                imagen.src = 'data:image/jpeg;base64,' + puesto.imagenBase64;
                imagen.style.display = '';
                placeholder.style.display = 'none';
            } else {
                imagen.removeAttribute('src');
                imagen.style.display = 'none';
                placeholder.style.display = '';
            }
        }
    }

    function bindEvents() {
        container.querySelectorAll('[data-action="aceptar"]').forEach(btn => {
            btn.addEventListener('click', onAceptar);
        });
        container.querySelectorAll('[data-action="omitir"]').forEach(btn => {
            btn.addEventListener('click', onOmitir);
        });
        container.querySelectorAll('[data-input-patente]').forEach(input => {
            input.addEventListener('input', debounce(onPatenteChange, 400));
        });
        document.getElementById('btnPantallaPrincipal').addEventListener('click', redireccionarAHome);
    }

    let debounceTimer;
    function debounce(fn, wait) {
        return function (...args) {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => fn.apply(this, args), wait);
        };
    }

    function getPuestoId(button) {
        return parseInt(button.closest('.lta-v2__card').dataset.puestoId, 10);
    }

    function onAceptar(e) {
        const puestoId = getPuestoId(e.currentTarget);
        const puesto = getPuesto(puestoId);
        if (!puesto || puesto.cargando) return;

        const card = getCard(puestoId);
        const input = card.querySelector('[data-input-patente]');
        const patente = (input.value || '').trim().toUpperCase();
        if (!patente) {
            puesto.mensajeError = 'No se pudo leer la patente en la imagen';
            puesto.reconocimientoExitoso = false;
            puesto.formatoInvalido = false;
            actualizarCard(puesto);
            return;
        }

        puesto.cargando = true;
        puesto.patente = patente;
        actualizarCard(puesto);

        const body = new URLSearchParams();
        body.append('id', puestoId);
        body.append('patente', patente);

        fetch(config.urls.aceptar, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Accept': 'application/json'
            },
            body: body
        })
        .then(response => response.json())
        .then(data => {
            puesto.cargando = false;
            if (data.valida) {
                window.location.href = data.url;
            } else {
                puesto.mensajeError = data.mensaje || 'No se pudo procesar la identificación';
                actualizarCard(puesto);
            }
        })
        .catch(err => {
            puesto.cargando = false;
            puesto.mensajeError = 'Ocurrió un error al procesar';
            console.error(err);
            actualizarCard(puesto);
        });
    }

    function onOmitir(e) {
        const puestoId = getPuestoId(e.currentTarget);
        const puesto = getPuesto(puestoId);

        puesto.cargando = true;
        actualizarCard(puesto);

        const body = new URLSearchParams();
        body.append('puestoDeTrabajoId', puestoId);

        fetch(config.urls.omitir, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Accept': 'application/json'
            },
            body: body
        })
        .then(response => response.json())
        .then(data => {
            puesto.cargando = false;
            puesto.colaId = data.Id || 0;
            puesto.patente = data.Patente || null;
            puesto.reconocimientoExitoso = data.ReconocimientoExitoso || false;
            puesto.formatoInvalido = false;
            puesto.mensajeError = data.MensajeError || null;
            puesto.imagenBase64 = data.ImagenBase64 || null;
            actualizarCard(puesto);
        })
        .catch(err => {
            puesto.cargando = false;
            puesto.mensajeError = 'Ocurrió un error al procesar';
            console.error(err);
            actualizarCard(puesto);
        });
    }

    function aplicarNotificacion(notificacion) {
        const puesto = getPuesto(notificacion.PuestoDeTrabajoId);
        if (!puesto) {
            console.warn('Se recibió una notificación para un puesto no presente en esta pantalla:', notificacion.PuestoDeTrabajoId, notificacion);
            return;
        }

        puesto.colaId = notificacion.Id || 0;
        puesto.patente = notificacion.Patente || null;
        puesto.reconocimientoExitoso = notificacion.ReconocimientoExitoso || false;
        puesto.formatoInvalido = false;
        puesto.mensajeError = notificacion.MensajeError || null;
        puesto.imagenBase64 = notificacion.ImagenBase64 || null;
        puesto.cargando = false;

        actualizarCard(puesto);
    }

    function onPatenteChange(e) {
        const input = e.target;
        const card = input.closest('.lta-v2__card');
        const puestoId = parseInt(card.dataset.puestoId, 10);
        const puesto = getPuesto(puestoId)

        const patente = (input.value || '').trim().toUpperCase();
        puesto.patente = patente || puesto.patente;
        puesto.mensajeError = null;

        if (!patente) {
            puesto.reconocimientoExitoso = false;
            puesto.formatoInvalido = false;
            puesto.mensajeError = 'No se pudo leer la patente en la imagen';
            actualizarCard(puesto);
            return;
        }

        if (!PATENTE_REGEX.test(patente)) {
            puesto.reconocimientoExitoso = false;
            puesto.formatoInvalido = true;
            actualizarCard(puesto);
            return;
        }

        puesto.formatoInvalido = false;
        puesto.cargando = true;
        actualizarCard(puesto);

        const body = new URLSearchParams();
        body.append('patente', patente);

        fetch(config.urls.validar, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Accept': 'application/json'
            },
            body: body
        })
        .then(response => response.json())
        .then(data => {
            puesto.cargando = false;
            puesto.reconocimientoExitoso = data.valida;
            actualizarCard(puesto);
        })
        .catch(err => {
            puesto.cargando = false;
            puesto.reconocimientoExitoso = false;
            puesto.mensajeError = 'Error de comunicación con el servidor';
            console.error(err);
            actualizarCard(puesto);
        });
    }

    function redireccionarAHome() {
        $.cookie('RedireccionarABalanzaAutomatizada', true);
        $.cookie('RedireccionarAListaAutomatizada', false);
        window.location.href = document.getElementById('home').value;
    }

    function suscribirAGrupos(hub) {
        puestos.forEach(p => {
            hub.server.unirseAGrupo("Calado" + p.id.toString())
                .done(function () {
                    console.log("Suscrito exitosamente al grupo: Calado" + p.id);
                })
                .fail(function (err) {
                    console.error("Error al suscribir al grupo Calado" + p.id, err);
                });
        });
    }

    function iniciarConexionSignalR(notificaLectura) {
        var estado = $.connection.hub.state;

        if (estado === $.signalR.connectionState.connected) {
            console.log("SignalR ya conectado. Connection ID:", $.connection.hub.id);
            suscribirAGrupos(notificaLectura);
            return;
        }

        if (estado === $.signalR.connectionState.connecting) {
            console.log("SignalR ya está conectando, se espera a que finalice.");
            return;
        }

        $.connection.hub.start({ transport: ['webSockets', 'longPolling'] })
            .done(function () {
                console.log("SignalR iniciado. Connection ID:", $.connection.hub.id);
                suscribirAGrupos(notificaLectura);
            })
            .fail(function (err) {
                console.error("Error al iniciar conexión SignalR:", err);
            });
    }

    function initSignalR() {
        if (typeof $ === 'undefined' || !$.connection) {
            console.warn('SignalR no está disponible');
            return;
        }

        const notificaLectura = $.connection.notificaLectura;
        if (!notificaLectura) {
            console.warn('Hub notificaLectura no encontrado');
            return;
        }

        $.connection.hub.logging = true;

        notificaLectura.client.informarEncolamientoCalado = function (notificacion) {
            console.log("Notificación SignalR recibida:", notificacion);
            aplicarNotificacion(notificacion);
        };

        $.connection.hub.reconnected(function () {
            console.warn("SignalR reconectado. Volviendo a unirse a los grupos de puestos...");
            suscribirAGrupos(notificaLectura);
        });

        $.connection.hub.disconnected(function () {
            console.error("Conexión SignalR perdida. Intentando reconectar en 4 segundos...");
            setTimeout(function () {
                iniciarConexionSignalR(notificaLectura);
            }, 4000);
        });

        iniciarConexionSignalR(notificaLectura);
    }

    bindEvents();
    initSignalR();
})();
