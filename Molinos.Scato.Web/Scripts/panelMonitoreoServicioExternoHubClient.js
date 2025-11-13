$(document).ready(function () {
    var notificador = $.connection.notificarUsuario;

    notificador.client.actualizarEstadoServicioExterno = function (notificacion) {
        try {
            if (!notificacion) return;
            if (notificacion.TipoAlerta == 8) {
                let payload = JSON.parse(notificacion.Mensaje);
                let key = payload.key;
                let status = payload.status;
                let ultima = payload.ultimaVerificacion;
                let card = document.querySelector('.card__container[data-key="' + key + '"]');
                if (card) {
                    let statusEl = card.querySelector('.healthCheckEstado');
                    if (statusEl) statusEl.textContent = status;
                    
                    let ultimaEl = card.querySelector('#healthCheckFecha');
                    if (ultimaEl) ultimaEl.textContent = ultima;
                    
                    let semaforoContainer = card.querySelector('.semaforo__container');
                    if (semaforoContainer) {
                        semaforoContainer.classList.remove('semaforo--success', 'semaforo--error', 'semaforo--default');
                        
                        if (status === 'Conectado') {
                            semaforoContainer.classList.add('semaforo--success');
                        } else if (status === 'Desconectado') {
                            semaforoContainer.classList.add('semaforo--error');
                        } else {
                            semaforoContainer.classList.add('semaforo--default');
                        }
                    }
                    
                    let btn = card.querySelector('.btn--refresh-servicio-externo, .btn-refresh');
                    if (btn) btn.disabled = false;
                    card.classList.remove('loading');
                }

                computeHealthcheckSemaforo();
            }
        } catch (e) {
            console.error('Error procesando notificacion de monitoreo', e);
        }
    };

    window.hubReady.done(function () {
        try {
            notificador.server.unirseAGrupo('EstadoServicioExterno');
        } catch (e) {
            console.warn('No se pudo unir al grupo EstadoServicioExterno', e);
        }
    });
})